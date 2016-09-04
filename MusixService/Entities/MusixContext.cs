using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace MusixService.Entities
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MusixContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<CatalogPath> CatalogPaths { get; set; }

        public DbSet<Path> Paths { get; set; }

        public DbSet<Artist> Artists { get; set; }

        public DbSet<Album> Albums { get; set; }
        
        public DbSet<Genre> Genres { get; set; }
        
        public DbSet<Song> Songs { get; set; }
        

        public DbSet<AlbumArtist> AlbumArtist { get; set; }
        public DbSet<AlbumGenre> AlbumGenre { get; set; }
        public DbSet<SongArtist> SongArtist { get; set; }
        public DbSet<SongGenre> SongGenre { get; set; }



        public static void SeedCatalogs()
        {
            using (var c = new MusixContext())
            {
                var mx = new Catalog
                {
                    Guid = Guid.NewGuid(),
                    Name = "Musik",
                    Enabled = true
                };
                var hb = new Catalog
                {
                    Guid = Guid.NewGuid(),
                    Name = "Hörbücher",
                    Enabled = true
                };
                var hs = new Catalog
                {
                    Guid = Guid.NewGuid(),
                    Name = "Hörspiele",
                    Enabled = true
                };

                mx.Paths.Add(new CatalogPath
                {
                    Guid = Guid.NewGuid(),
                    Path = "C:\\Temp\\Musik1"
                });
                mx.Paths.Add(new CatalogPath
                {
                    Guid = Guid.NewGuid(),
                    Path = "C:\\Temp\\Musik2"
                });

                hb.Paths.Add(new CatalogPath
                {
                    Guid = Guid.NewGuid(),
                    Path = "C:\\Temp\\Hörbücher"
                });

                hs.Paths.Add(new CatalogPath
                {
                    Guid = Guid.NewGuid(),
                    Path = "C:\\Temp\\Hörspiele"
                });

                c.Catalogs.Add(mx);
                c.Catalogs.Add(hb);
                c.Catalogs.Add(hs);
                c.SaveChanges();
            }

        }


        public static void SeedUsers()
        {
            using (var c = new MusixContext())
            {
                var admin = new User
                {
                    Name = "Administrator",
                    Login = "admin",
                    Password = "admin",
                    Enabled = true
                };

                c.Users.Add(admin);
                c.SaveChanges();
            }

        }


        public DateTime Now()
        {
            DateTime scalar;
            using (DbConnection connection = Database.Connection)
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "SELECT NOW()";
                    scalar = Convert.ToDateTime(command.ExecuteScalar());
                }
                connection.Close();
            }
            return scalar;
        }

    }






    




    [Table("user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idUser")]
        public int Id { get; set; }

        [Column("strLogin")]
        [Required]
        [MaxLength(45)]
        [MinLength(2)]
        public string Login { get; set; }

        [Column("strPassword")]
        [Required]
        [MaxLength(45)]
        public string Password { get; set; }

        [Column("strName")]
        [Required]
        [MaxLength(45)]
        public string Name { get; set; }

        [Column("bolEnabled")]
        [Required]
        public bool Enabled { get; set; }
    }


    [Table("catalog")]
    public class Catalog
    {
        public Catalog()
        {
            Paths = new List<CatalogPath>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idCatalog")]
        public int Id { get; set; }

        [Column("guidCatalog")]
        [Required]
        public Guid Guid { get; set; }

        [Column("strName")]
        [Required]
        [MaxLength(45)]
        public string Name { get; set; }

        [Column("bolEnabled")]
        [Required]
        public bool Enabled { get; set; }

        public virtual ICollection<CatalogPath> Paths { get; set; }
    }


    [Table("catalogpath")]
    public class CatalogPath
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idCatalogPath")]
        public int Id { get; set; }

        [Column("guidCatalogPath")]
        [Required]
        public Guid Guid { get; set; }

        [Column("strPath")]
        [Required]
        [MaxLength(2048)]
        public string Path { get; set; }

        [Column("idCatalog")]
        [Required]
        public int CatalogId { get; set; }

        public virtual Catalog Catalog { get; set; }
    }

    
    [Table("path")]
    public class Path
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idPath")]
        public int Id { get; set; }

        [Column("guidPath")]
        public Guid Guid { get; set; }

        [Column("strPathName")]
        public string PathName { get; set; }

        [Column("dtPathCreated")]
        public DateTime PathCreated { get; set; }

        [Column("dtPathModified")]
        public DateTime PathModified { get; set; }


        [Column("dtRowCreated")]
        public DateTime RowCreated { get; set; }
        [Column("dtRowModified")]
        public DateTime? RowModified { get; set; }
        [Column("dtRowDeleted")]
        public DateTime? RowDeleted { get; set; }
    }
    

    [Table("genre")]
    public class Genre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idGenre")]
        public int Id { get; set; }

        [Column("guidGenre")]
        [Required]
        public Guid Guid { get; set; }

        [Column("strName")]
        [MaxLength(100)]
        public string Name { get; set; }
    }

    
    [Table("artist")]
    public class Artist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idArtist")]
        public int Id { get; set; }

        [Column("guidArtist")]
        [Required]
        public Guid Guid { get; set; }
        
        [Column("strName")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("strBorn")]
        public string Born { get; set; }
        [Column("strFormed")]
        public string Formed { get; set; }
        [Column("strGenres")]
        public string Genres { get; set; }
        [Column("strMoods")]
        public string Moods { get; set; }
        [Column("strStyles")]
        public string Styles { get; set; }
        [Column("strInstruments")]
        public string Instruments { get; set; }
        [Column("strBiography")]
        public string Biography { get; set; }
        [Column("strDied")]
        public string Died { get; set; }
        [Column("strDisbanded")]
        public string Disbanded { get; set; }
        [Column("strYearsActive")]
        public string YearsActive { get; set; }
        [Column("dtAdded")]
        public DateTime? Added { get; set; }

        [Column("dtTouched")]
        public DateTime Touched { get; set; }
        [Column("boolDeleted")]
        public bool Deleted { get; set; }
    }
       


    [Table("album")]
    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idAlbum")]
        public int idAlbum { get; set; }

        [Column("guidAlbum")]
        public Guid Guid { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        
        
        public virtual ICollection<AlbumArtist> Artists { get; set; }

        public virtual ICollection<AlbumGenre> Genres { get; set; }
    }





    [Table("song")]
    public class Song
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idSong")]
        public int Id { get; set; }

        [Column("guidSong")]
        [Required]
        public Guid Guid { get; set; }

        [Column("strTitle")]
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Column("strFilename")]
        [Required]
        [MaxLength(512)]
        public string Filename { get; set; }

        [Column("idPath")]
        public int PathId { get; set; }



        public long FileSize { get; set; }
        public DateTime FileCreated { get; set; }
        public DateTime FileModified { get; set; }
        


        [Column("dtRowCreated")]
        public DateTime RowCreated { get; set; }
        [Column("dtRowModified")]
        public DateTime? RowModified { get; set; }
        [Column("dtRowDeleted")]
        public DateTime? RowDeleted { get; set; }
        

        public virtual Path Path { get; set; }


        public virtual ICollection<SongArtist> Artists { get; set; }

        public virtual ICollection<SongGenre> Genres { get; set; }
    }


    [Table("album_artist")]
    public class AlbumArtist
    {
        [Key, Column("idAlbum", Order = 0)]
        public int AlbumId { get; set; }

        [Key, Column("idArtist", Order = 1)]
        public int ArtistId { get; set; }

        [Column("iPosition")]
        public int Position { get; set; }

        [Column("bFeatured")]
        public int Featured { get; set; }

        public virtual Album Album { get; set; }

        public virtual Artist Artist { get; set; }
    }


    [Table("album_genre")]
    public class AlbumGenre
    {
        [Key, Column("idAlbum", Order = 0)]
        public int AlbumId { get; set; }

        [Key, Column("idGenre", Order = 1)]
        public int GenreId { get; set; }

        [Column("iPosition")]
        public int Position { get; set; }

        public virtual Album Album { get; set; }

        public virtual Genre Genre { get; set; }
    }


    [Table("song_artist")]
    public class SongArtist
    {
        [Key, Column("idSong", Order = 0)]
        public int SongId { get; set; }

        [Key, Column("idArtist", Order = 1)]
        public int ArtistId { get; set; }

        [Column("iPosition")]
        public int Position { get; set; }

        [Column("bFeatured")]
        public int Featured { get; set; }

        public virtual Album Album { get; set; }

        public virtual Song Song { get; set; }
    }


    [Table("song_genre")]
    public class SongGenre
    {
        [Key, Column("idSong", Order = 0)]
        public int SongId { get; set; }

        [Key, Column("idGenre", Order = 1)]
        public int GenreId { get; set; }

        [Column("iPosition")]
        public int Position { get; set; }

        public virtual Song Song { get; set; }

        public virtual Genre Genre { get; set; }
    }

}
