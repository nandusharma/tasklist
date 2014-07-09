using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskList.Model.Enums;
using TaskList.Model.Interfaces;

namespace TaskList.Model.Models
{
    public class Task : IAudited, IHasId
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(maximumLength: 500)]
        public string Name { get; set; }

        [Required]
        public PriorityType Priority { get; set; }

        public int Order { get; set; }

        public TaskStatus Status { get; set; }

        public DateTime? Completion { get; set; }

        [Required]
        public string UserID { get; set; }
        public virtual TaskUser User { get; set; }

        [Required]
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        public int ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
