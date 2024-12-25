using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace radioCabs.Dtos.Advertise.Request
{
    public class CreateAdvertiseDTO
    {
        public int CompanyId { get; set; }

        public string? Designation { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsActive { get; set; }

        public IFormFile? Images { get; set; }
    }
}
