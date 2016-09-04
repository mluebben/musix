/////////////////////////////////////////////////////////////////////////////
// $Id $
// Copyright (C) 2015 Matthias Lübben <ml81@gmx.de>
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
// Purpose:      JSON-RPC service implementation
// Created:      07.04.2015 (dd.mm.yyyy)
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AustinHarris.JsonRpc;
using System.Net;
using MusixService.Http;

namespace MusixService
{
    public class Changeset<T>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<T> Data { get; set; }
    }


    public class CompleteChangeset
    {
        public DateTime t1 { get; set; }
        public DateTime t2 { get; set; }
        public List<Artist> artist { get; set; }
        public List<Genre> genre { get; set; }
        public List<Album> album { get; set; }
        public List<Path> path { get; set; }
        public List<Song> song { get; set; }
        public List<Art> art { get; set; }
        public List<AlbumArtist> album_artist { get; set; }
        public List<AlbumGenre> album_genre { get; set; }
        public List<SongArtist> song_artist { get; set; }
        public List<SongGenre> song_genre { get; set; }
        public List<Smartplaylist> smartplaylist { get; set; }
        public List<Playlist> playlist { get; set; }
    }




    public class ArtistChangeset
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<ArtistDelta> Data { get; set; }
    }

    public class ArtistDelta
    {
        public string changetype { get; set; }
        public long idArtist { get; set; }
        public string strArtist { get; set; }
        public string strBorn { get; set; }
        public string strFormed { get; set; }
        public string strGenres { get; set; }
        public string strMoods { get; set; }
        public string strStyles { get; set; }
        public string strInstruments { get; set; }
        public string strBiography { get; set; }
        public string strDied { get; set; }
        public string strDisbanded { get; set; }
        public string strYearsActive { get; set; }
        public DateTime? dtAdded { get; set; }
    }







    public class MusixJsonRpcContext
    {
        public MyUser User { get; set; }
    }

    public class MyUser
    {
        public string Name { get; set; }
    }

    public class MusixJsonRpcService : JsonRpcService
    {
        private Database _db;


        public MusixJsonRpcService()
        {
            _db = new Database();
            _db.ConnectionString = Program.Config.KodiConnectionString;
        }

        //public HttpListenerContext GetContext()
        //{
        //    //HttpListenerContext ctx = (HttpListenerContext)JsonRpcContext.Current().Value;
        //    JsonRpcContext.Current().Value;
        //    return ctx;
        //}

        public MusixJsonRpcContext GetContext()
        {
            return JsonRpcContext.Current().Value as MusixJsonRpcContext;
        }
        


        /// <summary>
        /// Delta changeset for artists.
        /// </summary>
        /// <param name="since">Retrieve changes since this timestamp.</param>
        /// <returns>List of changed records.</returns>
        [JsonRpcMethod]
        public ArtistChangeset Foo(DateTime since)
        {
            using (var context = new Entities.MusixContext())
            {
                var qArtistDelta =
                    from a in context.Artists
                    where a.Touched > since
                    select new ArtistDelta
                    {
                        changetype = a.Deleted ? "D" : "U",
                        idArtist = a.Id,
                        strArtist = a.Deleted ? null : a.Name,
                        strBorn = a.Deleted ? null : a.Born,
                        strFormed = a.Deleted ? null : a.Formed,
                        strGenres = a.Deleted ? null : a.Genres,
                        strMoods = a.Deleted ? null : a.Moods,
                        strStyles = a.Deleted ? null : a.Styles,
                        strInstruments = a.Deleted ? null : a.Instruments,
                        strBiography = a.Deleted ? null : a.Biography,
                        strDied = a.Deleted ? null : a.Died,
                        strDisbanded = a.Deleted ? null : a.Disbanded,
                        strYearsActive = a.Deleted ? null : a.YearsActive,
                        dtAdded = a.Deleted ? null : a.Added
                    };

                return new ArtistChangeset
                {
                    From = since,
                    To = context.Now(),
                    Data = qArtistDelta.ToList()
                };
            }
        }




        [JsonRpcMethod]
        public CompleteChangeset GetDelta(DateTime since)
        {
            string username = GetContext().User.Name;
            return new CompleteChangeset()
            {
                t1 = since,
                t2 = _db.Now(),
                artist = _db.GetArtistsDelta(since),
                genre = _db.GetGenreDelta(since),
                album = _db.GetAlbumDelta(since),
                path = _db.GetPathDelta(since),
                song = _db.GetSongDelta(since),
                art = _db.GetArtDelta(since),
                album_artist = _db.GetAlbumArtistDelta(since),
                album_genre = _db.GetAlbumGenreDelta(since),
                song_artist = _db.GetSongArtistDelta(since),
                song_genre = _db.GetSongGenreDelta(since),
                smartplaylist = _db.GetSmartplaylistDelta(since, username),
                playlist = new List<Playlist>()
            };
        }


        [JsonRpcMethod]
        public Changeset<Artist> GetArtistDelta(DateTime since)
        {
            return new Changeset<Artist>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetArtistsDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<Genre> GetGenreDelta(DateTime since)
        {
            return new Changeset<Genre>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetGenreDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<Album> GetAlbumDelta(DateTime since)
        {
            return new Changeset<Album>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetAlbumDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<Path> GetPathDelta(DateTime since)
        {
            return new Changeset<Path>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetPathDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<Song> GetSongDelta(DateTime since)
        {
            return new Changeset<Song>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetSongDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<Art> GetArtDelta(DateTime since)
        {
            return new Changeset<Art>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetArtDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<AlbumArtist> GetAlbumArtistDelta(DateTime since)
        {
            return new Changeset<AlbumArtist>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetAlbumArtistDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<AlbumGenre> GetAlbumGenreDelta(DateTime since)
        {
            return new Changeset<AlbumGenre>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetAlbumGenreDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<SongArtist> GetSongArtistDelta(DateTime since)
        {
            return new Changeset<SongArtist>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetSongArtistDelta(since)
            };
        }

        [JsonRpcMethod]
        public Changeset<SongGenre> GetSongGenreDelta(DateTime since)
        {
            return new Changeset<SongGenre>()
            {
                From = since,
                To = _db.Now(),
                Data = _db.GetSongGenreDelta(since)
            };
        }

        //[JsonRpcMethod]
        //public Changeset<Smartplaylist> GetSmartplaylistDelta(DateTime since)
        //{
        //    var ctx = GetContext();
        //    var usr = ctx.User.Identity.Name;

        //    return new Changeset<Smartplaylist>()
        //    {
        //        From = since,
        //        To = _db.Now(),
        //        Data = _db.GetSmartplaylistDelta(since, usr)
        //    };
        //}


        [JsonRpcMethod]
        public void SaveSmartplaylist(Smartplaylist data)
        {
            var ctx = GetContext();
            var usr = ctx.User.Name;
            _db.UpsertSmartplaylist(data, usr);
        }


        [JsonRpcMethod]
        public void DeleteSmartplaylist(string id)
        {
            var ctx = GetContext();
            var usr = ctx.User.Name;
            _db.DeleteSmartplaylist(id, usr);
        }


        //[JsonRpcMethod]
        //public Changeset<Playlist> GetPlaylistDelta(DateTime since)
        //{
        //    var ctx = GetContext();
        //    var usr = ctx.User.Identity.Name;

        //    return new Changeset<Playlist>()
        //    {
        //        From = since,
        //        To = _db.Now(),
        //        Data = _db.GetPlaylistDelta(since, usr)
        //    };
        //}


        [JsonRpcMethod]
        public void SavePlaylist(Playlist data)
        {
            var ctx = GetContext();
            var usr = ctx.User.Name;
            _db.UpsertPlaylist(data, usr);
        }


        [JsonRpcMethod]
        public void DeletePlaylist(string id)
        {
            var ctx = GetContext();
            var usr = ctx.User.Name;
            _db.DeletePlaylist(id, usr);
        }


        [JsonRpcMethod]
        public void SaveHistory(string id, int songid, DateTime played)
        {
            var ctx = GetContext();
            var usr = ctx.User.Name;
            _db.UpsertHistory(id, usr, songid, played);
        }


        [JsonRpcMethod]
        public void DeleteHistory(string id)
        {
            var ctx = GetContext();
            var usr = ctx.User.Name;
            _db.DeleteHistory(id, usr);
        }
    }
}
