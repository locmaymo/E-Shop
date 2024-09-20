namespace Catalog.API.Models
{
    public class Category : AuditedBaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
