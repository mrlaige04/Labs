namespace Lab1.Models
{
    public class Client
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
