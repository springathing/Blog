using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mblyakher_blog.Models;
using mblyakherShoppingApp.Models;
using Microsoft.AspNet.Identity;

namespace mblyakher_blog.Controllers
{
    public class CommentsController : Universal
    {

        // GET: Comments
        //public ActionResult Index()
        //{
        //    var comments = db.Comments.Include(c => c.Author).Include(c => c.BlogPost);
        //    return View(comments.ToList());
        //}

        //// GET: Comments/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = db.Comments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(comment);
        //}

        // GET: Comments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
        //    ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title");
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,BlogPostId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Comments.Add(comment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName", comment.AuthorId);
        //    ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title", comment.BlogPostId);
        //    return View(comment);
        //}

        // GET: Comments/Edit/5
        [Authorize]
        [RequireHttps]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                return View(comment);
            }
            else
            {
                var post = db.Posts.Find(comment.BlogPostId);
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BlogPostId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var post = db.Posts.Find(comment.BlogPostId);
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(comment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
                }
                return View(comment);
            }
            else
            {
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
        }

        // GET: Comments/Delete/5
        [Authorize]
        [RequireHttps]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                return View(comment);
            }
            else
            {
                var post = db.Posts.Find(comment.BlogPostId);
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }

        }

        // POST: Comments/Delete/5
        [RequireHttps]
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            var post = db.Posts.Find(comment.BlogPostId);
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
            else
            {
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
