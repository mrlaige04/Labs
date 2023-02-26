namespace Lab1.Models
{
    public class Hotel
    {
        public IList<Room> Rooms { get; set; }
        public IList<Client> Clients { get; set; }
        public IList<Booking> Bookings { get; set; }
        public Hotel()
        {
            Rooms = new List<Room>();
            Clients = new List<Client>();
            Bookings = new List<Booking>();
        }
    }
}
