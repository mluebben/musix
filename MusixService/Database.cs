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
// Purpose:      Database abstraction layer
// Created:      07.04.2015 (dd.mm.yyyy)
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace MusixService
{
    public class Database
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// Factory method to create a new database connection.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }


        /// <summary>
        /// Returns the current time as seen by the database server.
        /// </summary>
        /// <returns>Now.</returns>
        public DateTime Now()
        {
            var sql = "SELECT NOW()";

            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    return Convert.ToDateTime(command.ExecuteScalar());
                }
            }

            throw new Exception("SELECT NOW() failed");
        }


        /// <summary>
        /// Delta changeset for artists.
        /// </summary>
        /// <param name="since">Retrieve changes since this timestamp.</param>
        /// <returns>List of changed records.</returns>
        public List<Artist> GetArtistsDelta(DateTime since)
        {
            string table = "artist";
            string[] idColumns = { "idArtist" };
            string[] columns = { "idArtist", "strArtist", "strBorn", "strFormed", "strGenres", "strMoods", "strStyles", "strInstruments", "strBiography", "strDied", "strDisbanded", "strYearsActive", "dtAdded" };

            PrepareDelta(table, idColumns);

            var result = new List<Artist>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Artist()
                            {
                                changetype = reader.GetString(0),
                                idArtist = reader.GetInt64(1),
                                strArtist = reader.GetNullableString(2),
                                strBorn = reader.GetNullableString(3),
                                strFormed = reader.GetNullableString(4),
                                strGenres = reader.GetNullableString(5),
                                strMoods = reader.GetNullableString(6),
                                strStyles = reader.GetNullableString(7),
                                strInstruments = reader.GetNullableString(8),
                                strBiography = reader.GetNullableString(9),
                                strDied = reader.GetNullableString(10),
                                strDisbanded = reader.GetNullableString(11),
                                strYearsActive = reader.GetNullableString(12),
                                dtAdded = reader.GetNullableDateTime(13)
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }

        
        public List<Genre> GetGenreDelta(DateTime since)
        {
            string table = "genre";
            string[] idColumns = { "idGenre" };
            string[] columns = { "idGenre", "strGenre" };

            PrepareDelta(table, idColumns);

            var result = new List<Genre>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Genre()
                            {
                                changetype = reader.GetString(0),
                                idGenre = reader.GetInt64(1),
                                strGenre = reader.GetNullableString(2)
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<Album> GetAlbumDelta(DateTime since)
        {
            string table = "album";
            string[] idColumns = { "idAlbum" };
            string[] columns = { "idAlbum", "strAlbum", "strArtists", "strGenres", "iYear", "bCompilation", "strMoods", "strStyles", "strThemes", "strReview", "strLabel", "strType", "iRating", "dtAdded" };

            PrepareDelta(table, idColumns);

            var result = new List<Album>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql("album", idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Album()
                            {
                                changetype = reader.GetString(0),
                                idAlbum = reader.GetInt64(1),
                                strAlbum = reader.GetNullableString(2),
                                strArtists = reader.GetNullableString(3),
                                strGenres = reader.GetNullableString(4),
                                iYear = reader.GetNullableInt32(5),
                                bCompilation = reader.GetNullableInt32(6),
                                strMoods = reader.GetNullableString(7),
                                strStyles = reader.GetNullableString(8),
                                strThemes = reader.GetNullableString(9),
                                strReview = reader.GetNullableString(10),
                                strLabel = reader.GetNullableString(11),
                                strType = reader.GetNullableString(12),
                                iRating = reader.GetNullableInt32(13),
                                dtAdded = reader.GetNullableDateTime(14)
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<Path> GetPathDelta(DateTime since)
        {
            string table = "path";
            string[] idColumns = { "idPath" };
            string[] columns = { "idPath", "strPath" };

            PrepareDelta(table, idColumns);
            
            var result = new List<Path>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();
                
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Path()
                            {
                                changetype = reader.GetString(0),
                                idPath = reader.GetInt64(1),
                                strPath = reader.GetNullableString(2)
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<Song> GetSongDelta(DateTime since)
        {
            string table = "song";
            string[] idColumns = { "idSong" };
            string[] columns = { "idSong", "idAlbum", "idPath", "strArtists", "strGenres", "strTitle", "iTrack", "iDuration", "iYear", "strFileName", "iTimesPlayed", "idThumb", "lastplayed", "rating", "comment", "dtAdded" };

            PrepareDelta(table, idColumns);

            var result = new List<Song>();
            using (var connection = CreateConnection(ConnectionString))
            { 
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql("song", idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Song()
                            {
                                changetype = reader.GetString(0),
                                idSong = reader.GetInt64(1),
                                idAlbum = reader.GetNullableInt64(2),
                                idPath = reader.GetNullableInt64(3),
                                strArtists = reader.GetNullableString(4),
                                strGenres = reader.GetNullableString(5),
                                strTitle = reader.GetNullableString(6),
                                iTrack = reader.GetNullableInt32(7),
                                iDuration = reader.GetNullableInt32(8),
                                iYear = reader.GetNullableInt32(9),
                                strFileName = reader.GetNullableString(10),
                                iTimesPlayed = reader.GetNullableInt32(11),
                                idThumb = reader.GetNullableInt64(12),
                                lastplayed = reader.GetNullableDateTime(13),
                                rating = reader.GetNullableString(14),
                                comment = reader.GetNullableString(15),
                                dtAdded = reader.GetNullableDateTime(16)
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<Art> GetArtDelta(DateTime since)
        {
            string table = "art";
            string[] idColumns = { "art_id" };
            string[] columns = { "art_id", "media_id", "media_type", "type", "url" };

            PrepareDelta(table, idColumns);
                
            var result = new List<Art>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Art()
                            {
                                changetype = reader.GetString(0),
                                idArt = reader.GetInt64(1),
                                idMedia = reader.GetNullableInt64(2),
                                strMediaType = reader.GetNullableString(3),
                                strType = reader.GetNullableString(4),
                                strUrl = reader.GetNullableString(5)
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public Art GetArtById(int id)
        {
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT art_id, media_id, media_type, type, url FROM art WHERE art_id = @id";
                    command.AddParamWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Art()
                            {
                                idArt = reader.GetInt64(0),
                                idMedia = reader.GetNullableInt64(1),
                                strMediaType = reader.GetNullableString(2),
                                strType = reader.GetNullableString(3),
                                strUrl = reader.GetNullableString(4)
                            };
                            
                        }
                    }
                }
            }
            return null;
        }


        public string GetSongPath(int id)
        {
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT CONCAT(path.strPath, song.strFileName) AS strFileName FROM song JOIN path ON song.idPath = path.idPath WHERE song.idSong = @id";
                    command.AddParamWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }
            return null;
        }



        public List<AlbumArtist> GetAlbumArtistDelta(DateTime since)
        {
            string table = "album_artist";
            string[] idColumns = { "idArtist", "idAlbum" };
            string[] columns = { "idArtist", "idAlbum", "boolFeatured", "iOrder" };
            
            PrepareDelta(table, idColumns);

            var result = new List<AlbumArtist>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();
                
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);
    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new AlbumArtist()
                            {
                                changetype = reader.IsDBNull(0) ? null : reader.GetString(0),
                                idArtist = reader.GetInt64(1),
                                idAlbum = reader.GetInt64(2),
                                boolFeatured = reader.GetNullableInt32(3),
                                iOrder = reader.GetNullableInt32(4),
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<AlbumGenre> GetAlbumGenreDelta(DateTime since)
        {
            string table = "album_genre";
            string[] idColumns = { "idGenre", "idAlbum" };
            string[] columns = { "idGenre", "idAlbum", "iOrder" };

            PrepareDelta(table, idColumns);

            var result = new List<AlbumGenre>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);
    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new AlbumGenre()
                            {
                                changetype = reader.IsDBNull(0) ? null : reader.GetString(0),
                                idGenre = reader.GetInt64(1),
                                idAlbum = reader.GetInt64(2),
                                iOrder = reader.GetNullableInt32(3),
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<SongArtist> GetSongArtistDelta(DateTime since)
        {
            string table = "song_artist";
            string[] idColumns = { "idArtist", "idSong" };
            string[] columns = { "idArtist", "idSong", "boolFeatured", "iOrder" };

            PrepareDelta(table, idColumns);
            
            var result = new List<SongArtist>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new SongArtist()
                            {
                                changetype = reader.GetString(0),
                                idArtist = reader.GetInt64(1),
                                idSong = reader.GetInt64(2),
                                boolFeatured = reader.GetNullableInt32(3),
                                iOrder = reader.GetNullableInt32(4),
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<SongGenre> GetSongGenreDelta(DateTime since)
        {
            string table = "song_genre";
            string[] idColumns = { "idGenre", "idSong" };
            string[] columns = { "idGenre", "idSong", "iOrder" };

            PrepareDelta(table, idColumns);

            var result = new List<SongGenre>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new SongGenre()
                            {
                                changetype = reader.GetString(0),
                                idGenre = reader.GetInt64(1),
                                idSong = reader.GetInt64(2),
                                iOrder = reader.GetNullableInt32(3),
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public List<Smartplaylist> GetSmartplaylistDelta(DateTime since, string idUser)
        {
            // TODO Use idUser in SELECT statement

            string table = "smartplaylist";
            string[] idColumns = { "idSmartPlaylist" };
            string[] columns = { "idSmartPlaylist", "strName", "strDescription", "iType", "strSql", "strOrderBy", "dtRowLastModified" };

            var result = new List<Smartplaylist>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Smartplaylist()
                            {
                                changetype = reader.GetString(0),
                                idSmartPlaylist = reader.GetString(1),
                                strName = reader.GetNullableString(2),
                                strDescription = reader.GetNullableString(3),
                                iType = reader.GetNullableInt32(4),
                                strSql = reader.GetNullableString(5),
                                strOrderBy = reader.GetNullableString(6),
                                dtRowLastModified = reader.GetNullableDateTime(7)
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }


        public bool UpsertSmartplaylist(Smartplaylist smartplaylist, string idUser)
        {
            // TODO Use idUser in UPDATE statement

            var sqlUpdate = @"
                UPDATE smartplaylist SET
                    strName = @strName,
                    strDescription = @strDescription,
                    iType = @iType,
                    strSql = @strSql,
                    strOrderBy = @strOrderBy
                WHERE idSmartPlaylist = @idSmartPlaylist;
                ";

            var sqlInsert = @"
                INSERT INTO smartplaylist(idSmartPlaylist, strName, strDescription, iType, strSql, strOrderBy)
                VALUES(@idSmartPlaylist, @strName, @strDescription, @iType, @strSql, @strOrderBy);
                ";

            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sqlUpdate;
                    command.AddParamWithValue("@idSmartPlaylist", smartplaylist.idSmartPlaylist);
                    command.AddParamWithValue("@strName", smartplaylist.strName);
                    command.AddParamWithValue("@strDescription", smartplaylist.strDescription);
                    command.AddParamWithValue("@iType", smartplaylist.iType);
                    command.AddParamWithValue("@strSql", smartplaylist.strSql);
                    command.AddParamWithValue("@strOrderBy", smartplaylist.strOrderBy);

                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows > 0)
                        return true;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sqlInsert;
                    command.AddParamWithValue("@idSmartPlaylist", smartplaylist.idSmartPlaylist);
                    command.AddParamWithValue("@strName", smartplaylist.strName);
                    command.AddParamWithValue("@strDescription", smartplaylist.strDescription);
                    command.AddParamWithValue("@iType", smartplaylist.iType);
                    command.AddParamWithValue("@strSql", smartplaylist.strSql);
                    command.AddParamWithValue("@strOrderBy", smartplaylist.strOrderBy);

                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows > 0)
                        return true;
                }
            }

            return false;
        }


        public void DeleteSmartplaylist(string id, string idUser)
        {
            // TODO Use idUser in DELETE statement

            var sql = @"
                DELETE FROM smartplaylist WHERE idSmartPlaylist = @id
                ";
            var sqlDeleteHistlog = @"
                DELETE FROM xsync__smartplaylist WHERE idSmartPlaylist = @id
                ";
            var sqlInsertHistlog = @"
                INSERT INTO xsync__smartplaylist(idSmartPlaylist) VALUES(@id)
                ";

            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                int affectedRows = 0;
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.AddParamWithValue("@id", id);
                    affectedRows = command.ExecuteNonQuery();
                }

                if (affectedRows > 0)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlDeleteHistlog;
                        command.AddParamWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlInsertHistlog;
                        command.AddParamWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        public List<Playlist> GetPlaylistDelta(DateTime since, string idUser)
        {

            string table = "playlist";
            string[] idColumns = { "idPlaylist" };
            string[] columns = { "idPlaylist", "strName", "strDescription", "strOwner", "strType", "strSongs", "dtRowLastModified" };

            //var sqlSongs = @"
            //   SELECT idSong FROM playlist_song WHERE idPlaylist = @idPlaylist ORDER BY iPosition
            //   ";

            var result = new List<Playlist>();
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = GetDeltaSql(table, idColumns, columns);
                    command.AddParamWithValue("@since", since);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Playlist()
                            {
                                changetype = reader.GetString(0),
                                idPlaylist = reader.GetString(1),
                                strName = reader.GetNullableString(2),
                                strDescription = reader.GetNullableString(3),
                                strOwner = reader.GetNullableString(4),
                                strType = reader.GetNullableString(5),
                                songs = (from song in ((reader.GetNullableString(6) ?? string.Empty).Split(',')) select Convert.ToInt64(song)).ToArray(),
                                dtRowLastModified = reader.GetNullableDateTime(7)
                            };
                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }


        public void UpsertPlaylist(Playlist playlist, string idUser)
        {
            var sqlUpdatePlaylist = @"
               UPDATE playlist SET strName = @strName, strDescription = @strDescription, strOwner = @strOwner, strType = @strType, strSongs = @strSongs WHERE idPlaylist = @idPlaylist
               ";

            var sqlInsertPlaylist = @"
               INSERT INTO playlist(idPlaylist, strName, strDescription, strOwner, strType, strSongs) VALUES(@idPlaylist, @strName, @strDescription, @strOwner, @strType, @strSongs)
               ";

            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                string songs = String.Join(",", from song in playlist.songs select Convert.ToString(song));

                int affectedRows;
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sqlUpdatePlaylist;
                    command.AddParamWithValue("@idPlaylist", playlist.idPlaylist);
                    command.AddParamWithValue("@strName", playlist.strName);
                    command.AddParamWithValue("@strDescription", playlist.strDescription);
                    command.AddParamWithValue("@strOwner", playlist.strOwner);
                    command.AddParamWithValue("@strType", playlist.strType);
                    command.AddParamWithValue("@strSongs", songs);
                    affectedRows = command.ExecuteNonQuery();
                }

                if (affectedRows == 0)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = sqlInsertPlaylist;
                        command.AddParamWithValue("@idPlaylist", playlist.idPlaylist);
                        command.AddParamWithValue("@strName", playlist.strName);
                        command.AddParamWithValue("@strDescription", playlist.strDescription);
                        command.AddParamWithValue("@strOwner", playlist.strOwner);
                        command.AddParamWithValue("@strType", playlist.strType);
                        command.AddParamWithValue("@strSongs", songs);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        public void DeletePlaylist(string idPlaylist, string idUser)
        {
            //var sqlSelectPlaylistUser = @"
            //   SELECT idUser FROM playlist WHERE idPlaylist = @idPlaylist
            //   ";

            var sqlDeletePlaylist = @"
                DELETE FROM playlist WHERE idPlaylist = @idPlaylist
                ";

            var sqlDeleteHistlog = @"
                DELETE FROM xsync__playlist_histlog WHERE idPlaylist = @idPlaylist
                ";

            var sqlInsertHistlog = @"
                INSERT INTO xsync__playlist_histlog(idPlaylist) VALUES(@idPlaylist)
                ";

            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    int affectedRows = 0;
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = sqlDeletePlaylist;
                        command.AddParamWithValue("@idPlaylist", idPlaylist);
                        affectedRows = command.ExecuteNonQuery();
                    }
                    if (affectedRows > 0)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = sqlDeleteHistlog;
                            command.AddParamWithValue("@idPlaylist", idPlaylist);
                            command.ExecuteNonQuery();
                        }
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = sqlInsertHistlog;
                            command.AddParamWithValue("@idPlaylist", idPlaylist);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }


        //public List<History> GetHistoryDelta(DateTime since, string idUser)
        //{
        //    var result = new List<History>();
        //    var sql = @"
        //       SELECT
        //   'I' AS changetype,
        //   idHistory,
        //         idSong,
        //   dtPlayed,
        //   iRowVersion
        // FROM history
        // WHERE dtRowAdded > @since
        //   AND dtRowUpdated IS NULL
        //         AND idUser = @user

        // UNION ALL
        // SELECT
        //   'U' AS changetype,
        //   idHistory,
        //   idSong,
        //   dtPlayed,
        //   iRowVersion
        // FROM history
        // WHERE dtRowUpdated > @since
        //         AND idUser = @user

        // UNION ALL
        // SELECT
        //   'D' AS changetype,
        //   idHistory,
        //   NULL AS idSong,
        //   NULL AS dtPlayed,
        //   NULL AS iRowVersion
        // FROM xx_history
        // WHERE dtRowDeleted > @since
        //         AND idUser = @user;
        // ";

        //    using (var connection = CreateConnection(ConnectionString))
        //    {
        //        connection.Open();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandType = System.Data.CommandType.Text;
        //            command.CommandText = sql;
        //            command.AddParamWithValue("@since", since);
        //            command.AddParamWithValue("@user", user);

        //            using (var reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var row = new History()
        //                    {
        //                        changetype = reader.IsDBNull(0) ? null : reader.GetString(0),
        //                        idHistory = reader.GetString(1),
        //                        idSong = reader.GetInt64(2),
        //                        dtPlayed = reader.GetDateTime(3),
        //                        iRowVersion = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
        //                    };
        //                    result.Add(row);
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        public void UpsertHistory(string idHistory, string idUser, long idSong, DateTime dtPlayed)
        {
            var sqlUserPrecheck = @"
                SELECT idUser FROM history WHERE idHistory = @idHistory
                ";
            
            var sqlUpdate = @"
                UPDATE history SET idSong = @idSong, dtPlayed = @dtPlayed WHERE idHistory = @idHistory
                ";

            var sqlInsert = @"
                INSERT INTO history(idHistory, idUser, idSong, dtPlayed) VALUES(@idHistory, @idUser, @idSong, @dtPlayed)
                ";


            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                object result = null;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlUserPrecheck;
                    command.AddParamWithValue("@idHistory", idHistory);
                    result = command.ExecuteScalar();
                   
                }

                if (result != null)
                {
                    if (string.Compare(result.ToString(), idUser, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        throw new Exception("User mismatch on history record");
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlUpdate;
                        command.AddParamWithValue("@idHistory", idHistory);
                        command.AddParamWithValue("@idUser", idUser);
                        command.AddParamWithValue("@idSong", idSong);
                        command.AddParamWithValue("@dtPlayed", dtPlayed);
                        int affectedRows = command.ExecuteNonQuery();
                        if (affectedRows == 0)
                        {
                            throw new Exception("Unable to save history record (update)");
                        }
                    }
                }
                else
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlInsert;
                        command.AddParamWithValue("@idHistory", idHistory);
                        command.AddParamWithValue("@idUser", idUser);
                        command.AddParamWithValue("@idSong", idSong);
                        command.AddParamWithValue("@dtPlayed", dtPlayed);
                        int affectedRows = command.ExecuteNonQuery();
                        if (affectedRows == 0)
                        {
                            throw new Exception("Unable to save history record (insert)");
                        }
                    }
                }
            }
        }

        public void DeleteHistory(string idHistory, string idUser)
        {
            var sql = @"
                DELETE FROM history WHERE idHistory = @idHistory AND idUser = @idUser
                ";

            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.AddParamWithValue("@idHistory", idHistory);
                    command.AddParamWithValue("@idUser", idUser);
                    command.ExecuteNonQuery();
                }
            }
        }


        private void PrepareDelta(string table, string[] idColumns)
        {
            using (var connection = CreateConnection(ConnectionString))
            {
                connection.Open();

                // Transaktionslog bereinigen
                using (var command = connection.CreateCommand())
                {
                    //command.CommandText = "DELETE xsync__" + table + "_histlog FROM xsync__" + table + "_histlog WHERE dtDeleted > NOW() - interval 6 month";
                    command.CommandText = "DELETE xsync__" + table + "_histlog FROM xsync__" + table + "_histlog WHERE dtDeleted < NOW() - interval 9 month";
                    command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    var joinConditions = new string[idColumns.Length];
                    for (int i = 0; i < idColumns.Length; i++)
                    {
                        joinConditions[i] = "xsync__" + table + "_histlog." + idColumns[i] + " = " + table + "." + idColumns[i];
                    }

                    command.CommandText = "DELETE xsync__" + table + "_histlog FROM xsync__" + table + "_histlog JOIN " + table + " ON " + string.Join(" AND ", joinConditions);
                    command.ExecuteNonQuery();
                }

                // Gelöschte Datensätze ermitteln
                using (var command = connection.CreateCommand())
                {
                    var joinConditions = new string[idColumns.Length];
                    for (int i = 0; i < idColumns.Length; i++)
                    {
                        joinConditions[i] = "xsync__" + table + "_histstat." + idColumns[i] + " = " + table + "." + idColumns[i];
                    }

                    command.CommandText = "DELETE xsync__" + table + "_histstat FROM xsync__" + table + "_histstat JOIN " + table + " ON " + string.Join(" AND ", joinConditions);
                    command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO xsync__" + table + "_histlog(" + string.Join(", ", idColumns) + ") SELECT " + string.Join(", ", idColumns) + " FROM xsync__" + table + "_histstat";
                    command.ExecuteNonQuery();
                }

                // Aktuellen Stand merken
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "TRUNCATE TABLE xsync__" + table + "_histstat";
                    command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO xsync__" + table + "_histstat(" + string.Join(", ", idColumns) + ") SELECT " + string.Join(", ", idColumns) + " FROM " + table;
                    command.ExecuteNonQuery();
                }
            }
        }




        private string GetDeltaSql(string table, string[] idColumns, string[] columns)
        {
            var projection1 = new List<string>();
            var projection2 = new List<string>();

            projection1.Add("'U' AS changetype");
            foreach (string column in columns)
            {
                projection1.Add("`" + column + "`");
            }

            projection2.Add("'D' AS changetype");
            foreach (string column in columns)
            {
                if (idColumns.Contains(column))
                {
                    projection2.Add("`" + column + "`");
                }
                else
                {
                    projection2.Add("NULL AS `" + column + "`");
                }
            }

            string sql = "";
            sql = "SELECT " + string.Join(", ", projection1);
            sql += " FROM " + table;
            sql += " WHERE dtRowLastModified > @since";

            sql += " UNION ALL";
            sql += " SELECT " + string.Join(", ", projection2);
            sql += " FROM xsync__" + table + "_histlog";
            sql += " WHERE dtDeleted > @since";

            return sql;
        }
    }


    public class Artist
    {
        public string changetype { get; set; }
        public long idArtist { get; set; }
        public string strArtist { get; set; }      
        public string strBorn{get;set;}
        public string strFormed {get;set;}		   
        public string strGenres { get; set; }
        public string strMoods { get; set; }
        public string strStyles { get; set; }
        public string strInstruments { get; set; }
        public string strBiography{ get; set; }
        public string strDied { get; set; }
        public string strDisbanded { get; set; }
        public string strYearsActive { get; set; }
        public DateTime? dtAdded { get; set; }
    }


    public class Album
    {
        public string changetype { get; set; }
        public long idAlbum { get; set; }
        public string strAlbum { get; set; }
	    public string strArtists { get; set; }
	    public string strGenres { get; set; }
	    public int? iYear { get; set; }
	    public int? bCompilation { get; set; }
	    public string strMoods { get; set; }
	    public string strStyles { get; set; }
	    public string strThemes { get; set; }
	    public string strReview { get; set; }
	    public string strLabel { get; set; }
	    public string strType { get; set; }
	    public int? iRating { get; set; }
        public DateTime? dtAdded { get; set; }
    }


    public class Genre
    {
        public string changetype { get; set; }
        public long idGenre { get; set; }
        public string strGenre { get; set; }
    }


    public class Path
    {
        public string changetype { get; set; }
		public long idPath { get; set; }
		public string strPath { get; set; }
    }


    public class Song
    {
        public string changetype { get; set; }
        public long? idSong { get; set; }
        public long? idAlbum { get; set; }
        public long? idPath { get; set; }
        public string strArtists { get; set; }
        public string strGenres { get; set; }
        public string strTitle { get; set; }
        public int? iTrack { get; set; }
        public int? iDuration { get; set; }
        public int? iYear { get; set; }
        public string strFileName { get; set; }
        public int? iTimesPlayed { get; set; }
        public long? idThumb { get; set; }
        public DateTime? lastplayed { get; set; }
        public string rating { get; set; }
        public string comment { get; set; }
        public DateTime? dtAdded { get; set; }
    }


    public class Art
    {
        public string changetype { get; set; }
        public long idArt { get; set; }
        public long? idMedia { get; set; }
        public string strMediaType { get; set; }
        public string strType { get; set; }
        public string strUrl { get; set; }
    }


    public class AlbumArtist
    {
        public string changetype { get; set; }
        public long idArtist { get; set; }
        public long idAlbum { get; set; }
        public int? boolFeatured { get; set; }
        public int? iOrder { get; set; }
    }


    public class AlbumGenre
    {
        public string changetype { get; set; }
        public long idGenre { get; set; }
        public long idAlbum { get; set; }
        public int? iOrder { get; set; }
    }


    public class SongArtist
    {
        public string changetype { get; set; }
        public long idArtist { get; set; }
        public long idSong { get; set; }
        public int? boolFeatured { get; set; }
        public int? iOrder { get; set; }
    }


    public class SongGenre
    {
        public string changetype { get; set; }
        public long idGenre { get; set; }
        public long idSong { get; set; }
        public int? iOrder { get; set; }
    }


    public class Smartplaylist
    {
        public string changetype { get; set; }
        public string idSmartPlaylist { get; set; }
        public string strName { get; set; }
        public string strDescription { get; set; }
        public int? iType { get; set; }
        public string strSql { get; set; }
        public string strOrderBy { get; set; }
        public DateTime? dtRowLastModified { get; set; }
    }


    public class Playlist
    {
        public string changetype { get; set; }
        public string idPlaylist { get; set; }
        public string strName { get; set; }
        public string strDescription { get; set; }
        public string strOwner { get; set; }
        public string strType { get; set; }
        public long[] songs { get; set; }
        public DateTime? dtRowLastModified { get; set; }
    }


    public class History
    {
        public string changetype { get; set; }
        public string idHistory { get; set; }
        public long idSong { get; set; }
        public DateTime dtPlayed { get; set; }
        public DateTime dtRowLastModified { get; set; }
    }


    public static class Extensions
    {
        public static DbParameter AddParamWithValue<T>(this DbCommand command, string name, T value)
        {
            var param = command.CreateParameter();

            param.Direction = System.Data.ParameterDirection.Input;
            param.ParameterName = name;
            param.Value = value;

            command.Parameters.Add(param);

            return param;
        }

        public static string GetNullableString(this DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public static DateTime? GetNullableDateTime(this DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? new DateTime?() : reader.GetDateTime(ordinal);
        }

        public static long? GetNullableInt64(this DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? new long?() : reader.GetInt64(ordinal);
        }

        public static int? GetNullableInt32(this DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? new int?() : reader.GetInt32(ordinal);
        }

    }
}
