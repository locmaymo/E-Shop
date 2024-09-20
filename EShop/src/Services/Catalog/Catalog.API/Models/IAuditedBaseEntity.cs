using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models
{
    public interface IAuditedEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        string? CreatedBy { get; set; }

        DateTime? CreatedDate { get; set; }

        string? LastModifiedBy { get; set; }

        DateTime? LastModifiedDate { get; set; }
        bool? IsDeleted { get; set; }
        string? DeletedBy { get; set; }
    }
}
