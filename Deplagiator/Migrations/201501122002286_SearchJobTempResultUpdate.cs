namespace Deplagiator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchJobTempResultUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SearchJobTempResults", "AlgorithmType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SearchJobTempResults", "AlgorithmType");
        }
    }
}
