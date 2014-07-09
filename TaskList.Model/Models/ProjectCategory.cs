using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList.Model.Interfaces;

namespace TaskList.Model.Models
{
    public class ProjectCategory : IAudited, IHasId
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int ProjectID { get; set; }
        public virtual Project Project { get; set; }

        [Required]
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
