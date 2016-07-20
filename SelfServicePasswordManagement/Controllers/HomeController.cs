using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SelfServicePasswordManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("ChangePassword");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string user, string oldPass, string newPass, string newPassCheck)
        {
            if (newPass != newPassCheck)
            {
                return RedirectToAction("dontmatch");
            }

            try
            {
                if (user.Contains("\\"))
                {
                    user = user.Substring(user.IndexOf("\\") + 1);
                }

                if (user.Contains("@"))
                {
                    user = user.Substring(0, user.IndexOf("@"));
                }
                
                ChangePasswordInternal(user, oldPass, newPass);
                return RedirectToAction("success");
            }
            catch (Exception e)
            {
                return RedirectToAction("fail");
            }

        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Fail()
        {
            ViewBag.Message = "Make sure the old password is correct and that the new one meets the complexity requirements (10+ chars, uppercase, lowercase and numeric chars)";
            return View("Fail");
        }

        public ActionResult DontMatch()
        {
            ViewBag.Message = "Passwords don't match";
            return View("Fail");
        }

        public static void ChangePasswordInternal(string userName, string currentPassword, string newPassword)
        {
            var adAddress = ConfigurationManager.AppSettings["ADAddress"];
            string ldapPath = "LDAP://" + adAddress;
            string domain = ConfigurationManager.AppSettings["Domain"];

            DirectoryEntry directionEntry = new DirectoryEntry(ldapPath, domain + "\\" + userName, currentPassword);
            if (directionEntry != null)
            {
                Debug.WriteLine("AD found!");
                DirectorySearcher search = new DirectorySearcher(directionEntry);
                search.Filter = "(SAMAccountName=" + userName + ")";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    Debug.WriteLine("Search done");
                    DirectoryEntry userEntry = result.GetDirectoryEntry();
                    if (userEntry != null)
                    {
                        Debug.WriteLine("User found!");
                        userEntry.Invoke("ChangePassword", new object[] { currentPassword, newPassword });
                        userEntry.CommitChanges();
                        Debug.WriteLine("Password changed!");
                    }
                }
            }

        }
    }
}