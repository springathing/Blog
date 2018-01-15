using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; } // foreign key, points to another table
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdateReason { get; set; }
        public bool IsDeleted { get; set; }
        // ICollection will bring all the comment objects per post object (perspective from post looking at comments)
        // virtual points to one object in Post (perspective from comment looking at it's post object)
        // virtual ApplicationUser Author also only looks at one user (that set the comment)


        public virtual Post BlogPost { get; set; }      // virtual property connects to another table
        public virtual ApplicationUser Author { get; set; }
    }
}