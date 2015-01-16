namespace Deplagiator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchJobResultPlagiarisedDocuments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlagiarisedDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SearchJobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SearchJobs", t => t.SearchJobId)
                .Index(t => t.SearchJobId);
            
            CreateTable(
                "dbo.SearchJobResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SearchJobId = c.Int(nullable: false),
                        ReferenceDocumentId = c.Int(nullable: false),
                        MatchedWords = c.String(),
                        MatchPercent = c.Single(nullable: false),
                        AlgorithmType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReferenceDocuments", t => t.ReferenceDocumentId)
                .ForeignKey("dbo.SearchJobs", t => t.SearchJobId)
                .Index(t => t.SearchJobId)
                .Index(t => t.ReferenceDocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SearchJobResults", "SearchJobId", "dbo.SearchJobs");
            DropForeignKey("dbo.SearchJobResults", "ReferenceDocumentId", "dbo.ReferenceDocuments");
            DropForeignKey("dbo.PlagiarisedDocuments", "SearchJobId", "dbo.SearchJobs");
            DropIndex("dbo.SearchJobResults", new[] { "ReferenceDocumentId" });
            DropIndex("dbo.SearchJobResults", new[] { "SearchJobId" });
            DropIndex("dbo.PlagiarisedDocuments", new[] { "SearchJobId" });
            DropTable("dbo.SearchJobResults");
            DropTable("dbo.PlagiarisedDocuments");
        }
    }
}
