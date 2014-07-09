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
    public class Category : IAudited, IHasId
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(AllowEmptyStrings=false)]
        [StringLength(maximumLength: 50)]
        public string Name { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
