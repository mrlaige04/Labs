namespace Lab1.Models
{
    public class Option
    {
        public string Name { get; set; }
        public double Price { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Option option)
            {
                return Name == option.Name && Price == option.Price;
            }
            return false;
        }
    }
}
