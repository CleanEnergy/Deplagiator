using Deplagiator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Deplagiator
{
    public class Author
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        public string FirstName { get; set; }

        [Column]
        [Required]
        public string LastName { get; set; }
    }

    public class ReferenceDocument
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        public string DocumentName { get; set; }

        [Column]
        [Required]
        public string DocumentType { get; set; }

        [Column]
        [Required]
        public byte[] DocumentBytes { get; set; }

        [Column]
        [Required]
        public DateTime UploadDate { get; set; }

        [Column]
        [Required]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }
    }

    public class SearchJob
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        public string Token { get; set; }

        [Column]
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Column]
        [Required]
        public string AuthorFirstName { get; set; }

        [Column]
        [Required]
        public string AuthorLastName { get; set; }

        [Column]
        [Required]
        public bool BagOfWords { get; set; }

        [Column]
        [Required]
        public bool StringMatching { get; set; }

        [Column]
        [Required]
        public bool CitationAnalysis { get; set; }

        [Column]
        [Required]
        public bool Fingerprinting { get; set; }

        [Column]
        [Required]
        public string DocumentName { get; set; }

        [Column]
        [Required]
        public byte[] DocumentBytes { get; set; }

        [Column]
        [Required]
        public string DocumentType { get; set; }

        [Column]
        [Required]
        public DateTime UploadDate { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    public class SearchJobTempResult
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        [ForeignKey("SearchJob")]
        public int SearchJobId { get; set; }

        [Column]
        [Required]
        [ForeignKey("ReferenceDocument")]
        public int ReferenceDocumentId { get; set; }

        [Column]
        public string MatchedWords { get; set; }

        [Column]
        [Required]
        public float MatchPercent { get; set; }

        [Column]
        [Required]
        public int AlgorithmType { get; set; }

        [Column]
        [Required]
        public string Guid { get; set; }

        public virtual SearchJob SearchJob { get; set; }
        public virtual ReferenceDocument ReferenceDocument { get; set; }
    }

    public class SearchJobResult
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        [ForeignKey("SearchJob")]
        public int SearchJobId { get; set; }

        [Column]
        [Required]
        [ForeignKey("ReferenceDocument")]
        public int ReferenceDocumentId { get; set; }

        [Column]
        public string MatchedWords { get; set; }

        [Column]
        [Required]
        public float MatchPercent { get; set; }

        [Column]
        [Required]
        public int AlgorithmType { get; set; }

        public virtual SearchJob SearchJob { get; set; }
        public virtual ReferenceDocument ReferenceDocument { get; set; }
    }

    public class PlagiarisedDocument
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        [ForeignKey("SearchJob")]
        public int SearchJobId { get; set; }

        public virtual SearchJob SearchJob { get; set; }
    }
}