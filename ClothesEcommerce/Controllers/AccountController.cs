using ClothesEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace ClothesEcommerce.Controllers
{
    
    public class AccountController : Controller
    {
        ClothesEcomerceEntities3 db=new ClothesEcomerceEntities3();

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            var usr = (from x in db.Users where x.Email == user.Email && x.Password== user.Password  select x).FirstOrDefault();

          

            if (usr == null)
            {
                ViewBag.Error = "Your Informations is incorect";
            }
            else if (usr.UserID == 1)
            {
                Session["admin"] = usr.UserID;
                return RedirectToAction("Index", "Clothes");
            }
            else
            {
                Session["user"]= usr.UserID;
                return RedirectToAction("Index","Clothes");
            }
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User user)
        {
            try
            {
                User user1 = new User();
                user1.UserName = user.UserName;
                user1.Email = user.Email;
                user1.FirstName = user.FirstName;
                user1.LastName = user.LastName;
                user1.Password = user.Password;

                db.Users.Add(user1);
                db.SaveChanges();

                return RedirectToAction("Login");

            }
            catch (Exception)
            {
                ViewBag.Error = "Something went wrong. Please try again.";
            }

            return View();
        }
        [AllowUserAccess]

        public ActionResult UpdateProfile()
        {
            var usr = Convert.ToInt32(Session["user"]);
            var u = (from x in db.Users where x.UserID ==usr select x).First();
            User user = new User();
            user.UserName = u.UserName;
            user.FirstName = u.FirstName;
            user.LastName = u.LastName;  
            user.Email = u.Email;
            user.Password = u.Password;


            return View(user);
        }
        [AllowUserAccess]

        [HttpPost]
        public ActionResult UpdateProfile(User user)
        {
            var userid = Convert.ToInt32(Session["user"]);
            var user1 = (from x in db.Users where x.UserID == userid select x ).First();
            user1.UserName = user.UserName;
            user1.FirstName = user.FirstName;
            user1.LastName = user.LastName;
            if(user1.Password == user.Password)
            {
                ViewBag.Success = "Succefly updated";
                db.SaveChanges();
            }
            else
            {
                ViewBag.PassError = "your password is Incorect";
            }
            
            return View();
        }
        [AllowUserAccess]

        public ActionResult ChangePassword()
        {
            int userid = Convert.ToInt32(Session["user"]);
            var user = (from x in db.Users where x.UserID == userid select x).First();
            return View(user);
        }

        [AllowUserAccess]
        [HttpPost]
        public ActionResult ChangePassword(User user,string NewPassword, string ConfirmNewPassword)
        {
            int userid = Convert.ToInt32(Session["user"]);
            var user1 = (from x in db.Users where x.UserID == userid select x).First();
            if (user.Password == user1.Password && NewPassword == ConfirmNewPassword)
            {
                user1.Password = NewPassword;
                db.SaveChanges();
                ViewBag.Msg = "The password Successfully Changed";

            }
            else
            {
                ViewBag.Msg = "Something Went Wrong";
            }
            return View();
        }
        
        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index","Clothes");
        }

    }
}