using System;

namespace TaskList.Model.Interfaces
{
    public interface IAudited
    {
        string CreatedBy { get; set; }
        DateTime CreationDate { get; set; }
        string EditedBy { get; set; }
        DateTime? EditDate { get; set; }
    }
}
