using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Meek.Content
{
    public class Manage
    {
        public string ManageUrl {get;set;}
        public string ContentTitle { get; set; }
        public bool Partial { get; set; }
        public string EditorContents { get; set; }
    }
}
