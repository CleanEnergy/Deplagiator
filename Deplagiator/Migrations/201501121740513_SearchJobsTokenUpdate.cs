namespace Deplagiator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchJobsTokenUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SearchJobs", "Token", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SearchJobs", "Token", c => c.Guid(nullable: false));
        }
    }
}
