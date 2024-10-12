using System.ComponentModel.DataAnnotations;

namespace IfinionBackendAssessment.Entity.Entities
{
    public class UserRole: BaseEntity
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }
    }
}
