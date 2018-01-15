using mblyakher_blog.Models;
using mblyakherShoppingApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace mblyakher_blog.Controllers
{
    public class HomeController : Universal
    {
        //[RequireHttps]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult About()
        //{
        //    //ViewBag.Message = "Your application description page.";

        //    return View();
        //}
        [RequireHttps]
        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(Email model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold> ({1})</p><p> Message:</p><p>{2}</p>";
                    var from = model.FromName + "<" + model.FromEmail + ">"; //MyPortfolio part is the name that shows up on email preview
                    //model.Body = "This is a message from your portfolio site. The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {           //composing body here,
                        Subject = "Blog Contact", 
                        Body = string.Format(body, model.FromName, model.FromEmail,
                        model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail(); //instantiation to be able to access SendAsync method(it actually sends the email)
                    await svc.SendAsync(email);

                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model); //return to view with empty email model
        }
    }
}