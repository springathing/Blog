using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mblyakher_blog.Models; // namespaces, using directives point at different namespaces, using statements are used namespace.bla.class if directive isn't present
using mblyakher_blog.Helpers;
using mblyakherShoppingApp.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace mblyakher_blog.Controllers
{
    public class PostsController : Universal // access modifier public available to all other classes // : class inherited from
    {

        // GET: Posts
        [RequireHttps]
        public ActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            if (Request.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(db.Posts.OrderByDescending(p => p.Week).ToPagedList(pageNumber, pageSize));
            }
            return View(db.Posts.Where(p => p.Published == true).OrderByDescending(p => p.Week).ToPagedList(pageNumber, pageSize));
        }

        // POST
        [HttpPost]
        [RequireHttps]
        public ActionResult Index(string searchStr, int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            ViewBag.Search = searchStr; // using viewbag to send the search string to controller, once on second page(paganation) the search term is gone
            SearchHelper search = new SearchHelper();
            var blogList = search.IndexSearch(searchStr);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (Request.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(blogList.OrderByDescending(p => p.Week).ToPagedList(pageNumber, pageSize));
            }
            return View(blogList.Where(p => p.Published == true).OrderByDescending(p => p.Week).ToPagedList(pageNumber, pageSize));
        }

        // GET: Posts/Details/5
        [RequireHttps]
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }


        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        [RequireHttps]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken] //vulnerable part, where data is sent to controller from user on views, you want to authenticate any data sent (token = GUID(for example)), checks for matching tokens to let data through or not
        //[ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL,Published,Week,Description")] Post blogPost, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp") // curly braces not needed for 1 line if statements
                {
                    ModelState.AddModelError("image", "Invalid Format.");
                }
            }
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }

                var filePath = "/Assets/img/";
                var absPath = Server.MapPath("~" + filePath);
                blogPost.MediaUrl = filePath + image.FileName;
                image.SaveAs(Path.Combine(absPath, image.FileName));

                blogPost.Published = true;
                blogPost.Slug = Slug;
                blogPost.Created = DateTime.Now;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }


        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        [RequireHttps]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,MediaUrl,Week,Description")] Post post, string MediaUrl, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp") // curly braces not needed for 1 line if statements
                {
                    ModelState.AddModelError("image", "Invalid Format.");
                }
            }

            if (ModelState.IsValid)
            {
                db.Posts.Attach(post);
                db.Entry(post).Property("Title").IsModified = true;
                db.Entry(post).Property("Description").IsModified = true;
                db.Entry(post).Property("Week").IsModified = true;
                db.Entry(post).Property("MediaUrl").IsModified = true;
                db.Entry(post).Property("Body").IsModified = true;
                db.Entry(post).Property("Updated").IsModified = true;

                if (image != null)
                {
                    var filePath = "/Assets/img/";
                    var absPath = Server.MapPath("~" + filePath);
                    post.MediaUrl = filePath + image.FileName;
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }

                else
                {
                    post.MediaUrl = MediaUrl;
                }
                post.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        [RequireHttps]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        [RequireHttps]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) //override
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing); // after action is completed it goes to dispose the temporary object created
            // if you don't dispose the objects will continue to pile up and you'll run out of memory
        }



        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult CommentCreate([Bind(Include = "Id,BlogPostId,Body,Created,AuthorId")] Comment comment)
        { // only pass in the bind the attributes that have forms
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(comment.Body))
                {
                    comment.Created = DateTime.Now;
                    comment.AuthorId = User.Identity.GetUserId();
                    db.Comments.Add(comment);
                    db.SaveChanges();

                    var post = db.Posts.Find(comment.BlogPostId);
                    return RedirectToAction("Details", new { Slug = post.Slug });
                }
            }
            return RedirectToAction("Index");
        }

    }
}