using System.Drawing;
using System.Windows.Forms;

namespace Butik
{
    class Product
    {
        public string Name { get; private set; }
        public double Price { get; private set; }
        public string Description { get; private set; }
        public string ImageLocation { get; private set; }

        public Product(string name, double price, string description, string image)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageLocation = image;
        }
    }
}
