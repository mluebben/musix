namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Foos",
                c => new
                    {
                        FooId = c.Int(nullable: false, identity: true),
                        Wert = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.FooId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Foos");
        }
    }
}
