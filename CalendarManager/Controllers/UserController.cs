using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CalendarManager.Helpers;
using CalendarManager.Models;

namespace CalendarManager.Controllers
{
    public class UserController : Controller
    {
        #region Login

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // Fetch all new Events
            if (UserHelper.Authenticate(email, password))
            {
                Session[SessionNames.Email] = email;
                return RedirectToAction("Index", "Calendar", new { email = email });
            }

            return new EmptyResult();
        }
        
        #endregion Login

        #region Requests

        public ActionResult Requests()
        {
            if (CheckSession())
            {
                return RedirectToAction("Login");
            }

            string email = Session[SessionNames.Email].ToString();
            return View("Requests", UserHelper.GetTempEvents(email));
        }

        [HttpPost]
        public ActionResult Approve(long id)
        {
            TempEvent temp = TempEventHelper.GetEvent(id);
            TempEventHelper.HandleApprove(temp);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetRequestsCount()
        {
            int count = TempEventHelper.GetRequestCount(Session[SessionNames.Email].ToString());
            return Content(count.ToString());
        }

        #endregion Requests

        #region Private Methods

        private bool CheckSession()
        {
            return Session[SessionNames.Email] == null;
        }

        #endregion Private Methods
    }
}
