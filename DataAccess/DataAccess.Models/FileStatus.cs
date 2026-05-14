
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public enum FileStatus
    {
        [Display(Name = "В ожидании")]
        Pending = 0,
        [Display(Name = "В обработке")]
        Processing = 1,
        [Display(Name = "Неудачно обработан")]
        Failed = 2,
        [Display(Name = "Удачно обработан")]
        Completed = 3
    }
}
