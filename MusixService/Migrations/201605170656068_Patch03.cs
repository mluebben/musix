namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patch03 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.artist",
                c => new
                    {
                        idArtist = c.Int(nullable: false, identity: true),
                        guidArtist = c.Guid(nullable: false),
                        strName = c.String(maxLength: 100, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.idArtist);
            
            AlterColumn("dbo.genre", "strName", c => c.String(maxLength: 100, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.genre", "strName", c => c.String(maxLength: 45, storeType: "nvarchar"));
            DropTable("dbo.artist");
        }
    }
}
