namespace MusixService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patch06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.artist", "dtTouched", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.artist", "bolDeleted", c => c.Boolean(nullable: false));
            DropColumn("dbo.artist", "dtRowAdded");
            DropColumn("dbo.artist", "dtRowModified");
            DropColumn("dbo.artist", "dtRowDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.artist", "dtRowDeleted", c => c.DateTime(precision: 0));
            AddColumn("dbo.artist", "dtRowModified", c => c.DateTime(precision: 0));
            AddColumn("dbo.artist", "dtRowAdded", c => c.DateTime(precision: 0));
            DropColumn("dbo.artist", "bolDeleted");
            DropColumn("dbo.artist", "dtTouched");
        }
    }
}
