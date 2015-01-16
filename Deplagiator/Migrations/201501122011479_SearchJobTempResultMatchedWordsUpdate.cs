namespace Deplagiator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchJobTempResultMatchedWordsUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SearchJobTempResults", "MatchedWords", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SearchJobTempResults", "MatchedWords", c => c.String(nullable: false));
        }
    }
}
