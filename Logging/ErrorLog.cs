using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logging
{
    public static class ErrorLog
    {
        public static object Lock;

        static ErrorLog()
        {
            Lock = Lock ?? new object();
        }

        public static string Write(Exception error, string absoluteLogPath, string userName)
        {
            string stamp = string.Empty;
            lock (Lock)
            {
                XmlSerializer ser = new XmlSerializer(typeof(ExceptionWrapper));
                stamp = Guid.NewGuid().ToString();
                lock (Lock)
                {
                    using (FileStream fs = File.Open(absoluteLogPath, FileMode.Append, FileAccess.Write))
                        ser.Serialize(fs, new ExceptionWrapper(error.Message, DateTime.Now, error.StackTrace, userName, error.GetType().Name, stamp, error.InnerException != null ? error.InnerException.Message : "No inner exception"));
                }
            }
            return stamp;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes, Base64FormattingOptions.None);
        }

        public static string Base64Decode(string encodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public class ExceptionWrapper
    {
        public string Message { get; set; }
        public DateTime OccuredOn { get; set; }
        public string StackTrace { get; set; }
        public string UserName { get; set; }
        public string ExceptionType { get; set; }
        public string Stamp { get; set; }
        public string InnerException { get; set; }

        public ExceptionWrapper() { }

        public ExceptionWrapper(string message, DateTime occuredOn, string stackTrace, string userName, string type, string stamp, string innerException)
        {
            Message = message;
            OccuredOn = occuredOn;
            StackTrace = stackTrace;
            UserName = userName;
            ExceptionType = type;
            Stamp = stamp;
            InnerException = innerException;
        }
    }
}
