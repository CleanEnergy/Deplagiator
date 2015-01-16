namespace Deplagiator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchJobTempResultGuid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SearchJobTempResults", "Guid", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SearchJobTempResults", "Guid");
        }
    }
}
