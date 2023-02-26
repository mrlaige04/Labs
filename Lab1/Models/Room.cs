using Lab1.Models.Enums;

namespace Lab1.Models
{
    public class Room
    {
        public int Number { get; set; }
        public RoomClass Class { get; set; }
        public int Capacity { get; set; }
        public List<Option> Options { get; set; }
    }
}
