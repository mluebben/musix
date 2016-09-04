namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patch04 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.album",
                c => new
                    {
                        idAlbum = c.Int(nullable: false, identity: true),
                        guidAlbum = c.Guid(nullable: false),
                        strName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.idAlbum);
            
            CreateTable(
                "dbo.path",
                c => new
                    {
                        idPath = c.Int(nullable: false, identity: true),
                        guidPath = c.Guid(nullable: false),
                        strPathName = c.String(unicode: false),
                        dtPathCreated = c.DateTime(nullable: false, precision: 0),
                        dtPathModified = c.DateTime(nullable: false, precision: 0),
                        dtRowCreated = c.DateTime(nullable: false, precision: 0),
                        dtRowModified = c.DateTime(precision: 0),
                        dtRowDeleted = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.idPath);
            
            CreateTable(
                "dbo.song",
                c => new
                    {
                        idSong = c.Int(nullable: false, identity: true),
                        guidSong = c.Guid(nullable: false),
                        strTitle = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        strFilename = c.String(nullable: false, maxLength: 512, storeType: "nvarchar"),
                        idPath = c.Int(nullable: false),
                        FileSize = c.Long(nullable: false),
                        FileCreated = c.DateTime(nullable: false, precision: 0),
                        FileModified = c.DateTime(nullable: false, precision: 0),
                        dtRowCreated = c.DateTime(nullable: false, precision: 0),
                        dtRowModified = c.DateTime(precision: 0),
                        dtRowDeleted = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.idSong)
                .ForeignKey("dbo.path", t => t.idPath, cascadeDelete: true)
                .Index(t => t.idPath);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.song", "idPath", "dbo.path");
            DropIndex("dbo.song", new[] { "idPath" });
            DropTable("dbo.song");
            DropTable("dbo.path");
            DropTable("dbo.album");
        }
    }
}
