using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRsystem.Models
{
    public class DepartmentPayrollPolicy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(HRDepartment))]
        public int DepartmentId { get; set; }

        [JsonIgnore]
        public virtual HRDepartment HRDepartment { get; set; }

        [Required]
        [ForeignKey(nameof(PayrollPolicy))]
        public int PayrollPolicyId { get; set; }

        public virtual PayrollPolicy PayrollPolicy { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}