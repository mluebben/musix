namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patch07 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.artist", name: "bolDeleted", newName: "boolDeleted");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.artist", name: "boolDeleted", newName: "bolDeleted");
        }
    }
}
