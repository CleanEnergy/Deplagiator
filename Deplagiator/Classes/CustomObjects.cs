using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deplagiator
{
    public class UploadedDocument
    {
        public string FileName { get; set; }

        public DateTime UploadedDate { get; set; }
    }

    public class AuthorUpload
    {
        public Author Author { get; set; }

        public UploadedDocument UploadedDocument { get; set; }

        public string GUID { get; set; }
    }

    public class CheckResult
    {
        public string CheckedDocumentPath { get; set; }

        public float AverageMatch { get; set; }

        public List<string> ReferenceDocumentsPaths { get; set; }

        public DateTime CheckedDate { get; set; }

        public CheckResult()
        {
            ReferenceDocumentsPaths = new List<string>();
        }
    }
}