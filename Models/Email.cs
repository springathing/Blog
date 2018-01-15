using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mblyakher_blog.Models
{
    public class Email
    {
        [Required, Display(Name = "Name")]
        public string FromName { get; set; }
        [Required, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }
        //[Required] taken out since subject is hardcoded
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}