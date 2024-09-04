using System.ComponentModel.DataAnnotations;

namespace Nicholas_Project.Models
{
    public class BookingDetail
    {
        [Key]
        public int BookingId { get; set; }
        public string? FacilityDescription { get; set; }
        public string? BookoingDateFrom { get; set; }
        public string? BookingDateTo { get; set; }
        public string? BookedBy { get; set; }
        public string? Status { get; set; }
    }
}
