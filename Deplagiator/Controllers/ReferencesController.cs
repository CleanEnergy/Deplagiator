using BL.Validation.Functions;
using Deplagiator.Models;
using Deplagiator.Validation.Structures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Serialization;
using Deplagiator.Helpers;
using System.Data.Entity;

namespace Deplagiator.Controllers
{
    [Authorize]
    public class ReferencesController : Controller
    {
        private string referenceDocumentsFolderPath = HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["referenceDocumentsPath"]);
        private string jobInfoFileName = WebConfigurationManager.AppSettings["checkJobInfoFileName"];

        private ApplicationDbContext context = new ApplicationDbContext();
        private Validator validator = new Validator();

        // GET: References
        public ActionResult Index()
        {
            List<Author> authors = context.Authors.ToList();

            return View(authors);
        }

        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(UploadViewModel model, HttpPostedFileBase document)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ValidationResult result = validator.UploadReference(model.FirstName, model.LastName, document);

                    if (result)
                    {
                        SaveUpload(model, document);
                        return RedirectToAction("Index");
                    }

                    ControllerExtensions.AddValidationErrors(result, this);
                }
                catch (Exception e)
                {
                    return ControllerExtensions.RedirectToError(this, e);
                }
            }
            return View(model);
        }

        private void SaveUpload(UploadViewModel model, HttpPostedFileBase document)
        {
            using (DbContextTransaction t = context.Database.BeginTransaction())
            {

                try
                {
                    Author existingAuthor = context.Authors.FirstOrDefault(x => x.FirstName == model.FirstName && x.LastName == x.LastName);

                    if (existingAuthor == null)
                    {
                        context.Authors.Add(new Author() { FirstName = model.FirstName, LastName = model.LastName });
                    }
                    context.SaveChanges();

                    int authorId = context.Authors.Single(x => x.FirstName == model.FirstName && x.LastName == model.LastName).Id;

                    byte[] fileBuffer;
                    using (BinaryReader br = new BinaryReader(document.InputStream))
                    {
                        fileBuffer = br.ReadBytes((int)document.InputStream.Length);
                    }

                    context.ReferenceDocuments.Add(new ReferenceDocument()
                    {
                        AuthorId = authorId,
                        UploadDate = DateTime.Now,
                        DocumentBytes = fileBuffer,
                        DocumentType = document.ContentType,
                        DocumentName = Path.GetFileName(document.FileName)
                    });

                    context.SaveChanges();
                    t.Commit();
                }
                catch (Exception e)
                {
                    t.Rollback();
                    throw e;
                }
            }
            /*string guid = Guid.NewGuid().ToString();
            string path = Path.Combine(referenceDocumentsFolderPath, model.FirstName + "_" + model.LastName); 
            // Create a folder for the author if it doesn't exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Create a new directory with the GUID in the name
            path = Path.Combine(path, guid);
            Directory.CreateDirectory(path);

            // Create an AuthorUpload object to be serialized into the XML information file
            AuthorUpload information = new AuthorUpload
            {
                UploadedDocument = new UploadedDocument
                {
                    FileName = Path.GetFileName(document.FileName),
                    UploadedDate = DateTime.Now
                },
                Author = new Author 
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName
                },
                GUID = guid
            };
            
            // Serialize the information object and save it
            XmlSerializer serializer = new XmlSerializer(typeof(AuthorUpload));
            using (StreamWriter sw = new StreamWriter(Path.Combine(path, jobInfoFileName)))
            {
                serializer.Serialize(sw, information);    
            }

            // Save the document in the new directory
            document.SaveAs(Path.Combine(path, Path.GetFileName(document.FileName)));*/
        }
        
        public ActionResult ByAuthor(int authorId)
        {
            try
            {
                List<ReferenceDocument> documents = FindDocumentsByAuthor(authorId);
                ViewBag.Author = context.Authors.Single(x => x.Id == authorId);

                return View(documents);
            }
            catch (Exception e)
            {
                TempData.Add("ex", e);
                return Redirect(WebConfigurationManager.AppSettings["genericErrorUrl"]);
            }

        }

        private List<ReferenceDocument> FindDocumentsByAuthor(int authorId)
        {
            return context.ReferenceDocuments.Where(x => x.AuthorId == authorId).ToList();
        }

        public ActionResult Read(int documentId)
        {
            try
            {
                ReferenceDocument document = context.ReferenceDocuments.Single(x => x.Id == documentId);
                return File(document.DocumentBytes, document.DocumentType);
            }
            catch (Exception e)
            {
                TempData.Add("ex", e);
                return Redirect(WebConfigurationManager.AppSettings["genericErrorUrl"]);
            }

        }

    }

}