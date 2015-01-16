using Deplagiator.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deplagiator.Models
{
    public class UploadViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

    }

    public class ReferencesViewModel
    {
        public List<AuthorUpload> AuthorUploads { get; set; }
    }

    public class ReferenceSearchViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Bag of words")]
        public bool BagOfWords { get; set; }

        [Display(Name = "String matching")]
        public bool StringMatching { get; set; }

        [Display(Name = "Citation analysis")]
        public bool CitationAnalysis { get; set; }

        [Display(Name = "Fingerprinting")]
        public bool Fingerprinting { get; set; }

    }

    public class AlgorithmResultViewModel
    {
        [Display(Name = "Algorithm type")]
        public AlgorithmType AlgorithmType { get; set; }

        [Display(Name = "Document name")]
        public string DocumentName { get; set; }

        [Display(Name = "Reference")]
        public int ReferenceId { get; set; }

        [Display(Name = "Match percent")]
        public float MatchPercent { get; set; }

        [Display(Name = "Detected words")]
        public string[] DetectedWords { get; set; }
    }

    public class PlagiarisedDocumentViewModel
    {
        [Display(Name = "Job")]
        public int JobId { get; set; }

        [Display(Name = "First name")]
        public string AuthorFirstName { get; set; }

        [Display(Name = "Last name")]
        public string AuhtorLastName { get; set; }

        [Display(Name = "Document name")]
        public string DocumentName { get; set; }

        [Display(Name = "Upload date")]
        public DateTime UploadDate { get; set; }
    }

    public class PlagiarisedDocumentDetailsViewModel
    {
        [Display(Name = "Job")]
        public int JobId { get; set; }

        [Display(Name = "First name")]
        public string AuthorFirstName { get; set; }

        [Display(Name = "Last name")]
        public string AuhtorLastName { get; set; }

        [Display(Name = "Document name")]
        public string DocumentName { get; set; }

        [Display(Name = "Upload date")]
        public DateTime UploadDate { get; set; }

        [Display(Name = "Used types")]
        public List<AlgorithmType> UsedTypes { get; set; }

        [Display(Name = "Search job results")]
        public List<SearchJobResult> SearchJobResults { get; set; }
    }
}