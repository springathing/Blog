using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mblyakher_blog.Models
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int Week { get; set; }
        public string Description { get; set; }
        public string MediaUrl { get; set; }
        public bool Published { get; set; }
        public string Slug { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}