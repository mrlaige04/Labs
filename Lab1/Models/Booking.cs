namespace Lab1.Models
{
    public class Booking
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }       
        public Client Client { get; set; }

        public int RoomNumber { get; set; }
    }
}
