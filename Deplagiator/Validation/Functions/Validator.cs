using Deplagiator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deplagiator.Validation.Structures;
using System.IO;

namespace BL.Validation.Functions
{
    public class Validator
    {
        private ApplicationDbContext context;

        public Validator()
        {
            context = new ApplicationDbContext();
        }

        internal ValidationResult UploadReference(string authorFirstName, string authorLastName, System.Web.HttpPostedFileBase document)
        {
            ValidationResult result = new ValidationResult();

            if (document.ContentLength == 0 || document.FileName == string.Empty || document.InputStream == null)
                result.AddError("", "Please select a file to upload.");
            else if (Path.GetExtension(document.FileName).ToLower() != ".txt")
                result.AddError("", "Uploads must be in .txt format.");
            else if (Path.GetFileNameWithoutExtension(document.FileName).Contains('#'))
                result.AddError("", "The uploaded file name cannot contain the '#' character.");

            return result;
        }

        internal ValidationResult CheckWithReferences(System.Web.HttpPostedFileBase document)
        {
            ValidationResult result = new ValidationResult();

            if (document.ContentLength == 0 || document.FileName == string.Empty || document.InputStream == null)
                result.AddError("", "Please select a file to upload.");
            else if (Path.GetExtension(document.FileName).ToLower() != ".txt")
                result.AddError("", "Uploads must be in .txt format.");
            else if (Path.GetFileNameWithoutExtension(document.FileName).Contains('#'))
                result.AddError("", "The uploaded file name cannot contain the '#' character.");

            return result;
        }

        internal ValidationResult CanCheckWithReferences()
        {
            ValidationResult result = new ValidationResult();

            if (context.ReferenceDocuments.ToList().Count == 0)
            {
                result.AddError("", "Cannot check with references, because no references exist.");
            }

            return result;
        }
    }
}
