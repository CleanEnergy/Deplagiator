using Deplagiator.Classes;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.IO;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System;
using Deplagiator.Models;

namespace Deplagiator
{
    public class CheckHub : Hub
    {
        private string referenceDocumentsServerPath = HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["referenceDocumentsPath"]);
        private string temporaryDocumentsServerPath = HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["temporaryDocumentsPath"]);
        private string checkResultFileName = WebConfigurationManager.AppSettings["checkResultReportFileName"];
        private string checkJobInfoFileName = WebConfigurationManager.AppSettings["checkJobInfoFileName"];
        private float percentMatchBorder = float.Parse(WebConfigurationManager.AppSettings["percentMatchBorder"]);


        public async Task StartReferenceCheck(string token)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();

                // Read the information file for job details and serialize it into a JobInformation object
                JobInformation information = ReadJobInformation(token, context);
            
                // Create a list holding requested algorithm types
                List<AlgorithmType> requestedTypes = GetRequestedAlgorithms(information);

                // Start the algorithms
                JobResult result = await Algorithms.StartReferenceSearch(requestedTypes.ToArray(), token, context, this);

                Clients.Caller.finalizeSearch();

            }
            catch (Exception e)
            {
                ReportProgress("Error: " + e.Message);
            }
        }

        private static List<AlgorithmType> GetRequestedAlgorithms(JobInformation information)
        {
            List<AlgorithmType> requestedTypes = new List<AlgorithmType>();

            if (information.BagOfWords)
                requestedTypes.Add(AlgorithmType.BagOfWords);
            if (information.CitationAnalysis)
                requestedTypes.Add(AlgorithmType.CitationAnalysis);
            if (information.Fingerprinting)
                requestedTypes.Add(AlgorithmType.Fingerprinting);
            if (information.StringMatching)
                requestedTypes.Add(AlgorithmType.StringMatching);
            return requestedTypes;
        }
        private JobInformation ReadJobInformation(string token, ApplicationDbContext context)
        {
            context = new ApplicationDbContext();
            return new JobInformation(
                context.SearchJobs.Single(x => x.Token == token)
            );
        }

        private bool CheckFileIntegrity(string token, string fileName)
        {
            if (!CheckValidFolder(token))
            {
                ReportProgress("Folder not found. Search stopped.");
                return false;
            }

            if (!CheckForUploadedDocument(token, fileName))
            {
                ReportProgress("Uploaded document not found. Search stopped.");
                return false;
            }

            if (!CheckForInfoFile(token))
            {
                ReportProgress("Information file not found. Search stopped.");
                return false;
            }

            return true;
        }
        private bool CheckForInfoFile(string token)
        {
            return File.Exists(Path.Combine(temporaryDocumentsServerPath, token, checkJobInfoFileName));
        }
        private bool CheckForUploadedDocument(string token, string fileName)
        {
            return File.Exists(Path.Combine(temporaryDocumentsServerPath, token, fileName));
        }
        private bool CheckValidFolder(string token)
        {
            return Directory.Exists(Path.Combine(temporaryDocumentsServerPath, token));
        }

        #region Helpers
        private void ReportProgress(object message)
        {
            Clients.Caller.reportProgress(message);
        }
        private void ReportResult(object resultObject)
        {
            Clients.Caller.checkComplete(resultObject);
        }
        #endregion

    }
}