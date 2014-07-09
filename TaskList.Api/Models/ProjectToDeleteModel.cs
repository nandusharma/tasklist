using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskList.Api.Models
{
    public class ProjectToDelete
    {
        [Required]
        public int ProjectID { get; set; }

        [Required]
        public string UserID { get; set; }
    }
}