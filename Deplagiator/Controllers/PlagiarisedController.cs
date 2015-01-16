using Deplagiator.Helpers;
using Deplagiator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deplagiator.Controllers
{
    public class PlagiarisedController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public ActionResult Index()
        {
            try
            {
                List<PlagiarisedDocument> documents = context.PlagiarisedDocuments.OrderBy(x => x.SearchJob.UploadDate).ToList();

                List<PlagiarisedDocumentViewModel> model = new List<PlagiarisedDocumentViewModel>();

                foreach (PlagiarisedDocument document in documents)
                {
                    model.Add(new PlagiarisedDocumentViewModel() 
                    {
                        AuhtorLastName = document.SearchJob.AuthorLastName,
                        AuthorFirstName = document.SearchJob.AuthorFirstName,
                        DocumentName = document.SearchJob.DocumentName,
                        JobId = document.SearchJobId,
                        UploadDate = document.SearchJob.UploadDate
                    });
                }

                return View(model);
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);   
            }
        }

        public ActionResult Details(int jobId)
        {
            try
            {
                PlagiarisedDocument document = context.PlagiarisedDocuments.Single(x => x.SearchJobId == jobId);

                PlagiarisedDocumentDetailsViewModel model = new PlagiarisedDocumentDetailsViewModel() 
                {
                    AuhtorLastName = document.SearchJob.AuthorLastName,
                    AuthorFirstName = document.SearchJob.AuthorFirstName,
                    DocumentName = document.SearchJob.DocumentName,
                    JobId = document.SearchJobId,
                    UploadDate = document.SearchJob.UploadDate,
                    UsedTypes = GetUsedAlgorithmTypes(document),
                    SearchJobResults = context.SearchJobResults.Where(x => x.SearchJobId == jobId).ToList()
                };

                return View(model);
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);
            }
        }
        private List<Classes.AlgorithmType> GetUsedAlgorithmTypes(PlagiarisedDocument document)
        {
            List<Classes.AlgorithmType> types = new List<Classes.AlgorithmType>();

            if (document.SearchJob.BagOfWords)
            {
                types.Add(Classes.AlgorithmType.BagOfWords);
            }
            if (document.SearchJob.CitationAnalysis)
            {
                types.Add(Classes.AlgorithmType.CitationAnalysis);
            }
            if (document.SearchJob.Fingerprinting)
            {
                types.Add(Classes.AlgorithmType.Fingerprinting);
            }
            if (document.SearchJob.StringMatching)
            {
                types.Add(Classes.AlgorithmType.StringMatching);
            }

            return types;
        }

        public ActionResult OpenPlagiarisedDocument(int jobId)
        {
            try
            {
                SearchJob job = context.SearchJobs.Single(x => x.Id == jobId);

                ViewBag.DocumentTitle = job.DocumentName;
                ViewBag.DocumentContent = System.Text.Encoding.UTF8.GetString(job.DocumentBytes);

                return PartialView("_DocumentContentView");
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);   
            }
        }
        public ActionResult OpenReferenceDocument(int referenceDocumentId)
        {
            try
            {
                ReferenceDocument document = context.ReferenceDocuments.Single(x => x.Id == referenceDocumentId);

                ViewBag.DocumentTitle = document.DocumentName;
                ViewBag.DocumentContent = System.Text.Encoding.UTF8.GetString(document.DocumentBytes);

                return PartialView("_DocumentContentView");
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);
            }
        }
        public ActionResult ShowMatchingWords(int jobResultId)
        {
            try
            {
                SearchJobResult result = context.SearchJobResults.Single(x => x.Id == jobResultId);

                ViewBag.DocumentTitle = result.ReferenceDocument.DocumentName;
                ViewBag.MatchingWords = result.MatchedWords;

                return PartialView("_MatchingWordsView");
            }
            catch (Exception e)
            {
                return ControllerExtensions.RedirectToError(this, e);
            }
        }
    }
}