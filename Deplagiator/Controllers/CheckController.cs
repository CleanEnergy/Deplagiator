using BL.Validation.Functions;
using Deplagiator.Classes;
using Deplagiator.Helpers;
using Deplagiator.Models;
using Deplagiator.Validation.Structures;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Deplagiator.Controllers
{
    [Authorize]
    public class CheckController : Controller
    {
        private string tempDocumentsPath = HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["temporaryDocumentsPath"]);
        private string checkJobInfoFileName = WebConfigurationManager.AppSettings["checkJobInfoFileName"];

        private ApplicationDbContext context = new ApplicationDbContext();
        private Validator validator = new Validator();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WithReferences()
        {
            try
            {
                ValidationResult result = validator.CanCheckWithReferences();

                if (result)
                {
                    return View();                    
                }
                return ControllerExtensions.RedirectToInformation(this, result);
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WithReferences(HttpPostedFileBase document, ReferenceSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ValidationResult result = validator.CheckWithReferences(document);

                    if (result)
                    {
                        string guid = string.Empty;
                        SaveTemporaryDocument(document, model, out guid);

                        TempData.Add("Token", guid);

                        return RedirectToAction("RunWithReferences");
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

        private void SaveTemporaryDocument(HttpPostedFileBase document, ReferenceSearchViewModel model, out string guid)
        {
            guid = Guid.NewGuid().ToString();

            byte[] fileBuffer;
            using (BinaryReader br = new BinaryReader(document.InputStream))
            {
                fileBuffer = br.ReadBytes((int)document.InputStream.Length);
            }

            context.SearchJobs.Add(new SearchJob() 
            {
                AuthorFirstName = model.FirstName,
                AuthorLastName = model.LastName,
                BagOfWords = model.BagOfWords,
                CitationAnalysis = model.CitationAnalysis,
                DocumentBytes = fileBuffer,
                DocumentName = Path.GetFileName(document.FileName),
                DocumentType = document.ContentType,
                Fingerprinting = model.Fingerprinting,
                StringMatching = model.StringMatching,
                Token = guid,
                UploadDate = DateTime.Now,
                UserId = context.Users.Single(x => x.UserName == User.Identity.Name).Id
            });

            context.SaveChanges();
        }

        public ActionResult RunWithReferences()
        {
            try
            {
                string token = (string)TempData["Token"];

                return View(context.SearchJobs.Single(x => x.Token == token));
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);
            }
        }

        public ActionResult GetDocumentReportView(int referenceDocumentId, int jobId, string guid)
        {
            SearchJobTempResult tempResult = context.SearchJobTempResults.Single(x => x.ReferenceDocumentId == referenceDocumentId && x.SearchJobId == jobId && x.Guid == guid);

            ReferenceDocument reference = tempResult.ReferenceDocument;

            AlgorithmResultViewModel model = new AlgorithmResultViewModel()
            {
                AlgorithmType = (AlgorithmType)tempResult.AlgorithmType,
                DetectedWords = tempResult.MatchedWords.Split(','),
                DocumentName = reference.DocumentName,
                MatchPercent = tempResult.MatchPercent,
                ReferenceId = reference.Id
            };

            return PartialView("_AlgorithmDocumentReport", model);
        }
        public ActionResult OpenReference(int referenceId)
        {
            try
            {
                ReferenceDocument reference = context.ReferenceDocuments.Single(x => x.Id == referenceId);

                return File(reference.DocumentBytes, reference.DocumentType);
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);
            }
        }
        public ActionResult OpenSuspiciousDocument(int jobId)
        {
            try
            {
                SearchJob job = context.SearchJobs.Single(x => x.Id == jobId);

                return File(job.DocumentBytes, job.DocumentType);
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);
            }
        }

        [HttpPost]
        public ActionResult MoveToPlagiarised(int jobId)
        {
            try
            {
                using (DbContextTransaction t = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.PlagiarisedDocuments.Add(new PlagiarisedDocument()
                        {
                            SearchJobId = jobId
                        });

                        foreach (SearchJobTempResult tempResult in context.SearchJobTempResults.ToList())
                        {
                            context.SearchJobResults.Add(new SearchJobResult()
                            {
                                AlgorithmType = tempResult.AlgorithmType,
                                MatchedWords = tempResult.MatchedWords,
                                MatchPercent = tempResult.MatchPercent,
                                ReferenceDocumentId = tempResult.ReferenceDocumentId,
                                SearchJobId = tempResult.SearchJobId
                            });
                        }

                        context.SearchJobTempResults.RemoveRange(context.SearchJobTempResults.Where(x => x.SearchJobId == jobId).ToList());

                        context.SaveChanges();
                        t.Commit();
                    }
                    catch (Exception e)
                    {
                        t.Rollback();
                        throw e;
                    }
                }
                return Json(new { action = "Redirect", url = Url.Action("Index") });
            }
            catch (Exception e)
            {
                ControllerExtensions.RedirectToError(this, e);
                return Json(new { action = "Error" });
            }
        }
        [HttpPost]
        public ActionResult UseAsReference(int jobId)
        {
            try
            {
                using (DbContextTransaction t = context.Database.BeginTransaction())
                {
                    try
                    {
                        SearchJob job = context.SearchJobs.Single(x => x.Id == jobId);

                        Author existingAuthor = context.Authors.FirstOrDefault(x => x.FirstName == job.AuthorFirstName && x.LastName == job.AuthorLastName);

                        if (existingAuthor == null)
                        {
                            context.Authors.Add(new Author()
                            {
                                FirstName = job.AuthorFirstName,
                                LastName = job.AuthorLastName
                            });
                            context.SaveChanges();
                            existingAuthor = context.Authors.Single(x => x.FirstName == job.AuthorFirstName && x.LastName == job.AuthorLastName);
                        }

                        context.ReferenceDocuments.Add(new ReferenceDocument()
                        {
                            AuthorId = existingAuthor.Id,
                            DocumentBytes = job.DocumentBytes,
                            DocumentName = job.DocumentName,
                            DocumentType = job.DocumentType,
                            UploadDate = job.UploadDate
                        });
                        context.SaveChanges();

                        context.SearchJobTempResults.RemoveRange(context.SearchJobTempResults.Where(x => x.SearchJobId == job.Id).ToList());
                        context.SaveChanges();

                        context.SearchJobs.Remove(job);
                        context.SaveChanges();

                        t.Commit();
                        return Json(new { action = "Redirect", url = Url.Action("Index") });
                    }
                    catch (Exception e)
                    {
                        t.Rollback();
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                ControllerExtensions.RedirectToError(this, e);
                return Json(new { action = "Error" });
            }
        }
        [HttpPost]
        public ActionResult DismissResults(int jobId)
        {
            try
            {
                using (DbContextTransaction t = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.SearchJobTempResults.RemoveRange(context.SearchJobTempResults.Where(x => x.SearchJobId == jobId).ToList());
                        context.SaveChanges();

                        context.SearchJobs.Remove(context.SearchJobs.Single(x => x.Id == jobId));
                        context.SaveChanges();

                        t.Commit();
                        return Json(new { action = "Redirect", url = Url.Action("Index") });
                    }
                    catch (Exception e)
                    {
                        t.Rollback();
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                ControllerExtensions.RedirectToError(this, e);
                return Json(new { action = "Error" });
            }
        }

        public ActionResult Cross()
        {
            return View();
        }

        #region Helpers

        public void AddModelErrors(List<KeyValuePair<string, string>> validationErrors)
        {
            foreach (KeyValuePair<string, string> pair in validationErrors)
            {
                ModelState.AddModelError(pair.Key, pair.Value);
            }
        }

        #endregion
    }

    
}