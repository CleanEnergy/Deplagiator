namespace Deplagiator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchJobs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SearchJobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        AuthorFirstName = c.String(nullable: false),
                        AuthorLastName = c.String(nullable: false),
                        BagOfWords = c.Boolean(nullable: false),
                        StringMatching = c.Boolean(nullable: false),
                        CitationAnalysis = c.Boolean(nullable: false),
                        Fingerprinting = c.Boolean(nullable: false),
                        DocumentName = c.String(nullable: false),
                        DocumentBytes = c.Binary(nullable: false),
                        DocumentType = c.String(nullable: false),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SearchJobs", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SearchJobs", new[] { "UserId" });
            DropTable("dbo.SearchJobs");
        }
    }
}
