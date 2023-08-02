using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Roles
{
    public class RoleDTO
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
