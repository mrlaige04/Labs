using Lab1.Models.Enums;

namespace Lab1.Models
{
    public class ClientBooking
    {
        public Client client { get; set; }
        public RoomClass RoomClass { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public List<Option> Options { get; set; }
    }
}
