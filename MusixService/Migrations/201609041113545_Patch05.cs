namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patch05 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.album_artist",
                c => new
                    {
                        idAlbum = c.Int(nullable: false),
                        idArtist = c.Int(nullable: false),
                        iPosition = c.Int(nullable: false),
                        bFeatured = c.Int(nullable: false),
                        Album_idAlbum = c.Int(),
                    })
                .PrimaryKey(t => new { t.idAlbum, t.idArtist })
                .ForeignKey("dbo.album", t => t.Album_idAlbum)
                .ForeignKey("dbo.artist", t => t.idArtist, cascadeDelete: true)
                .Index(t => t.idArtist)
                .Index(t => t.Album_idAlbum);
            
            CreateTable(
                "dbo.album_genre",
                c => new
                    {
                        idAlbum = c.Int(nullable: false),
                        idGenre = c.Int(nullable: false),
                        iPosition = c.Int(nullable: false),
                        Album_idAlbum = c.Int(),
                    })
                .PrimaryKey(t => new { t.idAlbum, t.idGenre })
                .ForeignKey("dbo.album", t => t.Album_idAlbum)
                .ForeignKey("dbo.genre", t => t.idGenre, cascadeDelete: true)
                .Index(t => t.idGenre)
                .Index(t => t.Album_idAlbum);
            
            CreateTable(
                "dbo.song_artist",
                c => new
                    {
                        idSong = c.Int(nullable: false),
                        idArtist = c.Int(nullable: false),
                        iPosition = c.Int(nullable: false),
                        bFeatured = c.Int(nullable: false),
                        Album_idAlbum = c.Int(),
                    })
                .PrimaryKey(t => new { t.idSong, t.idArtist })
                .ForeignKey("dbo.album", t => t.Album_idAlbum)
                .ForeignKey("dbo.song", t => t.idSong, cascadeDelete: true)
                .Index(t => t.idSong)
                .Index(t => t.Album_idAlbum);
            
            CreateTable(
                "dbo.song_genre",
                c => new
                    {
                        idSong = c.Int(nullable: false),
                        idGenre = c.Int(nullable: false),
                        iPosition = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.idSong, t.idGenre })
                .ForeignKey("dbo.genre", t => t.idGenre, cascadeDelete: true)
                .ForeignKey("dbo.song", t => t.idSong, cascadeDelete: true)
                .Index(t => t.idSong)
                .Index(t => t.idGenre);
            
            AddColumn("dbo.artist", "strBorn", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strFormed", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strGenres", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strMoods", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strStyles", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strInstruments", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strBiography", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strDied", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strDisbanded", c => c.String(unicode: false));
            AddColumn("dbo.artist", "strYearsActive", c => c.String(unicode: false));
            AddColumn("dbo.artist", "dtAdded", c => c.DateTime(precision: 0));
            AddColumn("dbo.artist", "dtRowAdded", c => c.DateTime(precision: 0));
            AddColumn("dbo.artist", "dtRowModified", c => c.DateTime(precision: 0));
            AddColumn("dbo.artist", "dtRowDeleted", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.song_genre", "idSong", "dbo.song");
            DropForeignKey("dbo.song_genre", "idGenre", "dbo.genre");
            DropForeignKey("dbo.song_artist", "idSong", "dbo.song");
            DropForeignKey("dbo.song_artist", "Album_idAlbum", "dbo.album");
            DropForeignKey("dbo.album_artist", "idArtist", "dbo.artist");
            DropForeignKey("dbo.album_genre", "idGenre", "dbo.genre");
            DropForeignKey("dbo.album_genre", "Album_idAlbum", "dbo.album");
            DropForeignKey("dbo.album_artist", "Album_idAlbum", "dbo.album");
            DropIndex("dbo.song_genre", new[] { "idGenre" });
            DropIndex("dbo.song_genre", new[] { "idSong" });
            DropIndex("dbo.song_artist", new[] { "Album_idAlbum" });
            DropIndex("dbo.song_artist", new[] { "idSong" });
            DropIndex("dbo.album_genre", new[] { "Album_idAlbum" });
            DropIndex("dbo.album_genre", new[] { "idGenre" });
            DropIndex("dbo.album_artist", new[] { "Album_idAlbum" });
            DropIndex("dbo.album_artist", new[] { "idArtist" });
            DropColumn("dbo.artist", "dtRowDeleted");
            DropColumn("dbo.artist", "dtRowModified");
            DropColumn("dbo.artist", "dtRowAdded");
            DropColumn("dbo.artist", "dtAdded");
            DropColumn("dbo.artist", "strYearsActive");
            DropColumn("dbo.artist", "strDisbanded");
            DropColumn("dbo.artist", "strDied");
            DropColumn("dbo.artist", "strBiography");
            DropColumn("dbo.artist", "strInstruments");
            DropColumn("dbo.artist", "strStyles");
            DropColumn("dbo.artist", "strMoods");
            DropColumn("dbo.artist", "strGenres");
            DropColumn("dbo.artist", "strFormed");
            DropColumn("dbo.artist", "strBorn");
            DropTable("dbo.song_genre");
            DropTable("dbo.song_artist");
            DropTable("dbo.album_genre");
            DropTable("dbo.album_artist");
        }
    }
}
