using System.Drawing;

namespace Butik
{
    class Product
    {
        public string Name { get; }
        public double Price { get; }
        public string Description { get; }
        public Image ImageData { get; }

        public Product(string name, double price, string description, Image imageData)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageData = imageData;
        }
    }
}