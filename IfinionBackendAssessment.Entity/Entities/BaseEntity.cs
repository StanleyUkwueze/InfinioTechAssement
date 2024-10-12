using System.ComponentModel.DataAnnotations;

namespace IfinionBackendAssessment.Entity.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }

    }
}
