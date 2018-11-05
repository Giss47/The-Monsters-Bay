namespace Butik
{
    class Product
    {
        public string Name { get; }
        public double Price { get; }
        public string Description { get; }
        public string ImageLocation { get; }

        public Product(string name, double price, string description, string image)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageLocation = image;
        }
    }
}