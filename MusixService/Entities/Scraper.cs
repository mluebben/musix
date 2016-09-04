using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MusixService.Entities
{
    public class ScrapeJob
    {
        public string Path { get; set; }
        public bool Recursive { get; set; }
    }

    



    public class Scraper
    {
        public static object jobsLock = new object();
        public static bool running = false;
        public static List<ScrapeJob> Jobs { get; set; }

        public static void RunJob(ScrapeJob job)
        {
            bool startThread = false;
            lock (jobsLock)
            {
                Jobs.Add(job);
                if (!running)
                {
                    running = true;
                    startThread = true;
                }
            }

            if (startThread)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Scraper.MyThread), null);
            }
        }


        public static void MyThread(object state)
        {
            ScrapeJob job = null;
            lock (jobsLock)
            {
                if (Jobs.Count > 0)
                {
                    job = Jobs[0];
                    Jobs.RemoveAt(0);
                }
                else
                {
                    running = false;
                    return;
                }
            }

            Scraper s = new Scraper();
            s.Run();
        }




        public void Run()
        {
            using (var context = new MusixContext())
            {
                var catalogs = (from c in context.Catalogs where c.Enabled == true select c).ToList();
                foreach (var catalog in catalogs)
                {
                    var catalogpaths = catalog.Paths.ToList();
                    foreach (var catalogpath in catalogpaths)
                    {
                        Walk(catalogpath.Path);
                    }
                }
            }
        }

        public void Walk(string path)
        {
            Debug.WriteLine("Process directory: " + path);

            var fileInfo = new FileInfo(path);

            DateTime pathCreated = new DateTime(fileInfo.CreationTime.Year, fileInfo.CreationTime.Month, fileInfo.CreationTime.Day, fileInfo.CreationTime.Hour, fileInfo.CreationTime.Minute, fileInfo.CreationTime.Second, DateTimeKind.Local);
            DateTime pathModified = new DateTime(fileInfo.LastWriteTime.Year, fileInfo.LastWriteTime.Month, fileInfo.LastWriteTime.Day, fileInfo.LastWriteTime.Hour, fileInfo.LastWriteTime.Minute, fileInfo.LastWriteTime.Second, DateTimeKind.Local);
            DateTime now = DateTime.Now;

            bool indexFiles = false;
            bool indexDirectories = false;

            using (var context = new MusixContext())
            {
                var q = from o in context.Paths where o.PathName == path select o;
                var p = q.FirstOrDefault();

                
                if (p == null)
                {
                    // Directory not listed in database
                    p = new Path();
                    p.Guid = Guid.NewGuid();
                    p.PathName = path;
                    p.PathCreated = pathCreated;
                    p.PathModified = pathModified;
                    p.RowCreated = now;
                    p.RowModified = new DateTime?(now);
                    p.RowDeleted = new DateTime?();

                    context.Paths.Add(p);
                    context.SaveChanges();

                    indexFiles = true;
                    indexDirectories = true;
                }
                else
                {
                    // Directory exists in database

//                    if (!pathCreated.Equals(p.PathCreated) || !pathModified.Equals(p.PathModified))
//                    {
                        p.PathCreated = pathCreated;
                        p.PathModified = pathModified;
                        p.RowModified = new DateTime?(now);

                        context.SaveChanges();

                        indexFiles = true;
                        indexDirectories = true;
//                    }
//                    else
//                    {
//                        // No changes within directory
//                        indexFiles = false;
//                        indexDirectories = false;
//                    }
                }
            }


            if (indexFiles)
            {
                foreach (string file in Directory.EnumerateFiles(path))
                {
                    Process(file);
                }
            }
            if (indexDirectories)
            {
                foreach (string directory in Directory.EnumerateDirectories(path))
                {
                    Walk(directory);
                }
            }
        }

        public void Process(string path)
        {
            Debug.WriteLine("Process file: " + path);

            FileInfo fileInfo = new FileInfo(path);


            if (fileInfo.Extension.ToLower() != ".mp3")
            {
                return;
            }

            DateTime fileCreated = new DateTime(fileInfo.CreationTime.Year, fileInfo.CreationTime.Month, fileInfo.CreationTime.Day, fileInfo.CreationTime.Hour, fileInfo.CreationTime.Minute, fileInfo.CreationTime.Second, DateTimeKind.Local);
            DateTime fileModified = new DateTime(fileInfo.LastWriteTime.Year, fileInfo.LastWriteTime.Month, fileInfo.LastWriteTime.Day, fileInfo.LastWriteTime.Hour, fileInfo.LastWriteTime.Minute, fileInfo.LastWriteTime.Second, DateTimeKind.Local);
            DateTime now = DateTime.Now;
            long fileSize = fileInfo.Length;

            string directoryName = fileInfo.DirectoryName;
            string fileName = fileInfo.Name;

            bool updateMetadata = false;
            bool saveChanges = false;


            using (var db = new MusixContext())
            {
                var pathQuery = from o in db.Paths where o.PathName == directoryName select o;
                int pathId = pathQuery.First().Id;

                var songQuery = from o in db.Songs where o.PathId == pathId && o.Filename == fileName select o;
                var song = songQuery.FirstOrDefault();

                if (song == null)
                {
                    // File is new
                    song = new Song();
                    song.Guid = Guid.NewGuid();

                    song.PathId = pathId;
                    song.Filename = fileName;
                    song.FileCreated = fileCreated;
                    song.FileModified = fileModified;
                    song.FileSize = fileSize;
                    
                    song.RowCreated = now;
                    song.RowModified = new DateTime?(now);
                    song.RowDeleted = new DateTime?();

                    db.Songs.Add(song);

                    updateMetadata = true;
                    saveChanges = true;
                }
                else
                {
                    if (fileSize != song.FileSize || !fileCreated.Equals(song.FileCreated) || !fileModified.Equals(song.FileModified))
                    {
                        // File has been modified

                        song.PathId = pathId;
                        song.Filename = fileName;
                        song.FileCreated = fileCreated;
                        song.FileModified = fileModified;
                        song.FileSize = fileSize;

                        song.RowModified = new DateTime?(now);
                        song.RowDeleted = new DateTime?();

                        updateMetadata = true;
                        saveChanges = true;
                    }
                    else
                    {
                        // File is unchanged
                        updateMetadata = false;
                        saveChanges = false;
                    }
                }





                if (updateMetadata)
                {
                    saveChanges = true;

                    TagLib.File f = TagLib.File.Create(path);
                    Console.WriteLine("Title: " + f.Tag.Title);
                    Console.WriteLine("Performer: " + f.Tag.FirstPerformer);

                    //string s = f.Tag.FirstPerformer;


                    var artistList = new List<Artist>();
                    foreach (var artist in f.Tag.Performers)
                    {
                        Artist artistObj = null;
                        if (db.Artists.Any(o => o.Name == artist))
                        {
                            artistObj = (from x in db.Artists where x.Name == artist select x).First();
                        }
                        else
                        {
                            artistObj = new Artist
                            {
                                Guid = Guid.NewGuid(),
                                Name = artist
                            };
                            db.Artists.Add(artistObj);
                        }
                        artistList.Add(artistObj);
                    }



                    var genreList = new List<Genre>();
                    foreach (var genre in f.Tag.Genres)
                    {
                        Genre genreObj = null;
                        if (db.Genres.Any(o => o.Name == genre))
                        {
                            genreObj = (from x in db.Genres where x.Name == genre select x).First();
                        }
                        else
                        {
                            genreObj = new Genre
                            {
                                Guid = Guid.NewGuid(),
                                Name = genre
                            };
                            db.Genres.Add(genreObj);
                        }
                        genreList.Add(genreObj);
                    }


                    //var album = null;
                    //if (!db.Albums.Any(o => o.Name == f.Tag.Album))
                    //{
                    //    db
                    //}

                    //if (!db)
                    //foreach (var album in f.Tag.Album)


                    song.Title = f.Tag.Title;
                }

                if (saveChanges)
                {
                    db.SaveChanges();
                }
            }
        }
    }

}

