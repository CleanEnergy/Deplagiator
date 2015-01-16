using AlgorithmCollection;
using Deplagiator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml.Serialization;
using Microsoft.AspNet.SignalR;

namespace Deplagiator.Classes
{
    public enum AlgorithmType
    {
        BagOfWords = 0,
        StringMatching = 1,
        CitationAnalysis = 2,
        Fingerprinting = 3
    }

    public enum JobType
    {
        CrossSearch = 1,
        ReferenceSearch = 2
    }

    public class AlgorithmResult
    {
        public int ReferenceId { get; set; }
        public List<string> DetectedWords { get; set; }
        public AlgorithmType AlgorithmType { get; set; }
        public float MatchPercent { get; set; }

        public AlgorithmResult()
        {
            DetectedWords = new List<string>();
        }

        public AlgorithmResult(int referenceId, AlgorithmType type)
        {
            AlgorithmType = type;
            ReferenceId = referenceId;
            DetectedWords = new List<string>();
        }

    }

    public class Word
    {
        public string Text { get; set; }
        public bool Marked { get; set; }
    }

    public class Sentence
    {
        public List<Word> Words { get; set; }

        public Sentence()
        {
            Words = new List<Word>();
        }
    }

    public class Algorithms
    {
        private static string referenceDocumentsPath = HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["referenceDocumentsPath"]);
        private static string uploadedReferenceInfoFileName = WebConfigurationManager.AppSettings["uploadedReferenceInfoFileName"];
        private static string stringMatchingLogFileName = WebConfigurationManager.AppSettings["stringMatchingLogFileName"];
        private static string fingerprintingLogFileName = WebConfigurationManager.AppSettings["fingerprintingLogFileName"];
        private static string documentFingerprintFileName = WebConfigurationManager.AppSettings["documentFingerprintFileName"];
        private static int nGramLength = int.Parse(WebConfigurationManager.AppSettings["fingerprintingNgram"]);
        private static float percentMatchBorder = float.Parse(WebConfigurationManager.AppSettings["percentMatchBorder"]);

        public static Task<JobResult> StartReferenceSearch(AlgorithmType[] chosenTypes, string token, ApplicationDbContext context, CheckHub hub)
        {
            JobResult result = new JobResult();
            try
            {
                string suspiciousContent = System.Text.Encoding.UTF8.GetString(context.SearchJobs.Single(x => x.Token == token).DocumentBytes);

                // Run each requested algorithm and gather the results
                if (chosenTypes.Contains(AlgorithmType.StringMatching))
                {
                    result.Results.Add(RunStringMatching(suspiciousContent, context, hub, token));
                }
                if (chosenTypes.Contains(AlgorithmType.Fingerprinting))
                {
                    result.Results.Add(RunFingerprinting(suspiciousContent, context, hub, token));
                }

                return Task.FromResult<JobResult>(result);
            }
            catch (Exception e)
            {
                return Task.FromResult<JobResult>(null);
            }
        }

        private static List<AlgorithmResult> RunStringMatching(string unfilteredContent, ApplicationDbContext context, CheckHub hub, string token)
        {

            List<AlgorithmResult> results = new List<AlgorithmResult>();

            foreach (ReferenceDocument reference in context.ReferenceDocuments.ToList())
            {
                string referenceContent = System.Text.Encoding.UTF8.GetString(reference.DocumentBytes);

                StringMatchingResult matchResult = StringMatching.Start(referenceContent, unfilteredContent);

                AlgorithmResult tempResult = new AlgorithmResult(reference.Id, AlgorithmType.StringMatching)
                {
                    DetectedWords = matchResult.MatchingWords,
                    MatchPercent = matchResult.PercentMatch
                };
                results.Add(tempResult);

                string entryGuid = Guid.NewGuid().ToString();
                int jobId = context.SearchJobs.Single(x => x.Token == token).Id;
                string matchedWords = "";
                tempResult.DetectedWords.ForEach((word) => 
                {
                    matchedWords += word + ",";
                });

                context.SearchJobTempResults.Add(new SearchJobTempResult() 
                {
                    MatchedWords = matchedWords,
                    MatchPercent = tempResult.MatchPercent,
                    ReferenceDocumentId = reference.Id,
                    SearchJobId = jobId,
                    AlgorithmType = (int)AlgorithmType.StringMatching,
                    Guid = entryGuid
                });
                context.SaveChanges();

                if (matchResult.PercentMatch > percentMatchBorder)
                {
                    hub.Clients.Caller.showDocumentReport(reference.Id, jobId, entryGuid);
                }
            }
            return results;
        }

        private static List<AlgorithmResult> RunFingerprinting(string unfilteredContent, ApplicationDbContext context, CheckHub hub, string token)
        {
            List<AlgorithmResult> results = new List<AlgorithmResult>();

            char[] separators = { ' ', '.' };

            int nGram = nGramLength;

            string[] suspiciousDocumentPrint = Fingerprinting.ComputePrint(nGram, unfilteredContent, separators);

            foreach (ReferenceDocument reference in context.ReferenceDocuments.ToList())
            {
                string referenceContent = System.Text.Encoding.UTF8.GetString(reference.DocumentBytes);
                string[] referencePrint = GetReferencePrint(separators, nGram, referenceContent);
                FingerprintingResult matchResult = Fingerprinting.Compare(referencePrint, suspiciousDocumentPrint);

                AlgorithmResult tempResult = new AlgorithmResult(reference.Id, AlgorithmType.Fingerprinting)
                {
                    DetectedWords = matchResult.MatchedPrints,
                    MatchPercent = matchResult.PercentMatch
                };
                results.Add(tempResult);

                string entryGuid = Guid.NewGuid().ToString();
                int jobId = context.SearchJobs.Single(x => x.Token == token).Id;
                string matchedWords = string.Empty;
                tempResult.DetectedWords.ForEach((word) =>
                {
                    matchedWords += word + ",";
                });

                context.SearchJobTempResults.Add(new SearchJobTempResult()
                {
                    MatchedWords = matchedWords,
                    MatchPercent = tempResult.MatchPercent,
                    ReferenceDocumentId = reference.Id,
                    SearchJobId = jobId,
                    AlgorithmType = (int)AlgorithmType.Fingerprinting,
                    Guid = entryGuid
                });
                context.SaveChanges();

                if (matchResult.PercentMatch > percentMatchBorder)
                {
                    hub.Clients.Caller.showDocumentReport(reference.Id, jobId, entryGuid);
                }
            }

            return results;
        }
        private static string[] GetReferencePrint(char[] separators, int nGram, string referenceContent)
        {
            return Fingerprinting.ComputePrint(nGram, referenceContent, separators);
        }

        private static List<AlgorithmResult> BagOfWords(string unfilteredContent, string[] referencesPaths)
        {
            List<AlgorithmResult> result = new List<AlgorithmResult>();



            return new List<AlgorithmResult>();
        }
        private static List<AlgorithmResult> CitationAnalysis(string unfilteredContent)
        {
            return new List<AlgorithmResult>();
        }



    }

    public class JobInformation
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FileName { get; set; }

        public bool BagOfWords { get; set; }

        public bool StringMatching { get; set; }

        public bool CitationAnalysis { get; set; }

        public bool Fingerprinting { get; set; }

        public JobInformation()
        { }

        public JobInformation(ReferenceSearchViewModel model, string fileName)
        {
            BagOfWords = model.BagOfWords;
            StringMatching = model.StringMatching;
            CitationAnalysis = model.CitationAnalysis;
            Fingerprinting = model.Fingerprinting;
            FileName = fileName;
            FirstName = model.FirstName;
            LastName = model.LastName;
        }

        public JobInformation(SearchJob searchJob)
        {
            FirstName = searchJob.AuthorFirstName;
            LastName = searchJob.AuthorLastName;
            FileName = searchJob.DocumentName;
            BagOfWords = searchJob.BagOfWords;
            StringMatching = searchJob.StringMatching;
            CitationAnalysis = searchJob.CitationAnalysis;
            Fingerprinting = searchJob.Fingerprinting;
        }
    }

    public class JobResult
    {
        /// <summary>
        /// For each document that is checked, a list is created. That list contains another list of results of each algorithm.
        /// </summary>
        public List<List<AlgorithmResult>> Results { get; set; }

        public JobResult()
        {
            Results = new List<List<AlgorithmResult>>();
        }

    }

    public class DocumentFingerprints
    {
        public string[] Hashes { get; set; }

        public DocumentFingerprints()
        {
            Hashes = new string[] { };
        }
    }

}
