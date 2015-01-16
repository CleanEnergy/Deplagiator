namespace Deplagiator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchJobTempResult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SearchJobTempResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SearchJobId = c.Int(nullable: false),
                        ReferenceDocumentId = c.Int(nullable: false),
                        MatchedWords = c.String(nullable: false),
                        MatchPercent = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReferenceDocuments", t => t.ReferenceDocumentId)
                .ForeignKey("dbo.SearchJobs", t => t.SearchJobId)
                .Index(t => t.SearchJobId)
                .Index(t => t.ReferenceDocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SearchJobTempResults", "SearchJobId", "dbo.SearchJobs");
            DropForeignKey("dbo.SearchJobTempResults", "ReferenceDocumentId", "dbo.ReferenceDocuments");
            DropIndex("dbo.SearchJobTempResults", new[] { "ReferenceDocumentId" });
            DropIndex("dbo.SearchJobTempResults", new[] { "SearchJobId" });
            DropTable("dbo.SearchJobTempResults");
        }
    }
}
