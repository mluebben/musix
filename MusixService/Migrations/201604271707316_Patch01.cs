namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patch01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.catalogpath",
                c => new
                    {
                        idCatalogPath = c.Int(nullable: false, identity: true),
                        guidCatalogPath = c.Guid(nullable: false),
                        strPath = c.String(nullable: false, maxLength: 2048, storeType: "nvarchar"),
                        idCatalog = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idCatalogPath)
                .ForeignKey("dbo.catalog", t => t.idCatalog, cascadeDelete: true)
                .Index(t => t.idCatalog);
            
            CreateTable(
                "dbo.catalog",
                c => new
                    {
                        idCatalog = c.Int(nullable: false, identity: true),
                        guidCatalog = c.Guid(nullable: false),
                        strName = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        bolEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.idCatalog);
            
            CreateTable(
                "dbo.user",
                c => new
                    {
                        idUser = c.Int(nullable: false, identity: true),
                        strLogin = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        strPassword = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        strName = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        bolEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.idUser);
            
            DropTable("dbo.Foos");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Foos",
                c => new
                    {
                        FooId = c.Int(nullable: false, identity: true),
                        Wert = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.FooId);
            
            DropForeignKey("dbo.catalogpath", "idCatalog", "dbo.catalog");
            DropIndex("dbo.catalogpath", new[] { "idCatalog" });
            DropTable("dbo.user");
            DropTable("dbo.catalog");
            DropTable("dbo.catalogpath");
        }
    }
}
