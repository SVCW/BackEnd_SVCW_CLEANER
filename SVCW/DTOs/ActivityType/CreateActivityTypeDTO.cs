using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.ActivityType
{
    public class CreateActivityTypeDTO
    {
        public string ActivityTypeName { get; set; }
        public string Description { get; set; }
    }
}
