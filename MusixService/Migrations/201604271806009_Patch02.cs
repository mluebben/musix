namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patch02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.genre",
                c => new
                    {
                        idGenre = c.Int(nullable: false, identity: true),
                        guidGenre = c.Guid(nullable: false),
                        strName = c.String(maxLength: 45, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.idGenre);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.genre");
        }
    }
}
