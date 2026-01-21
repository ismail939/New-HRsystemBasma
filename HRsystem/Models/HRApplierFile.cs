using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HRsystem.Models;

public class HRApplierFile
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(HRApplier))] // ← tells EF that ApplierId is the FK for HRApplier
    public int ApplierId { get; set; }
    [Required]
    [StringLength(100)]
    public string FileName { get; set; }

    [Required]
    public string Url { get; set; }

    public virtual HRApplier Applier { get; set; }
}
