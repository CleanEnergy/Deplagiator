using Deplagiator.Validation.Structures;
using Logging;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Deplagiator.Helpers
{
    internal static class ControllerExtensions
    {
        private static string _absoluteErrorLogPath = HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["errorLogPath"]);
        private static string _errorPageUrl = WebConfigurationManager.AppSettings["errorPageUrl"];
        private static string _infoErrorPageUrl = WebConfigurationManager.AppSettings["infoErrorPageUrl"];
        
        public static void AddErrors(this Controller controller, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                controller.ModelState.AddModelError("", error);
            }
        }

        /// <summary>
        /// Writes the error to the log and redirects to the error page.
        /// </summary>
        public static RedirectResult RedirectToError(this Controller controller, Exception e)
        {
            string returnUrl = string.Empty;

            returnUrl = _errorPageUrl + "?error=" + ErrorLog.Base64Encode(e.Message) + "&stamp=" + ErrorLog.Base64Encode(ErrorLog.Write(e, _absoluteErrorLogPath, controller.User != null ? controller.User.Identity.Name : string.Empty));

            return new RedirectResult(returnUrl);
        }

        public static RedirectResult RedirectToInformation(this Controller controller, ValidationResult validationResult)
        {
            List<string> messages = new List<string>();

            foreach (KeyValuePair<string, string> message in validationResult.ValidationMessages)
            {
                messages.Add(message.Value);
            }

            controller.TempData.Add("Messages", messages);

            return new RedirectResult(_infoErrorPageUrl);
        }

        public static bool CheckValidDate(int day, int month, int year)
        {
            DateTime result = new DateTime();

            return DateTime.TryParse(day.ToString() + "." + month.ToString() + "." + year.ToString(), out result);
        }


        public static bool CheckValidDate(int year, int month, int day, int hour, int minute, int second)
        {
            DateTime result = new DateTime();

            return DateTime.TryParse(day.ToString() + "." + month.ToString() + "." + year.ToString() + " " + hour + ":" + minute + ":" + second, out result);
        }

        internal static void AddValidationErrors(ValidationResult result, Controller studentsOfficeController)
        {
            for (int i = 0; i < result.ValidationMessages.Count; i++)
            {
                KeyValuePair<string, string> message = result.ValidationMessages[i];
                studentsOfficeController.ModelState.AddModelError(message.Key, message.Value);
            }
        }

    }
}