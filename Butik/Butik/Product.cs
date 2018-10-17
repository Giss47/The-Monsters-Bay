using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Butik
{
    // Jag tror vi behöver skapa objekt av samtliga produkter, därav påbörjade jag en ny klass, men vi kan prata om det imorgn. /Jocke

    class Product
    {
        public string Name { get; private set; }
        public double Price { get; private set; }
        public string Description { get; private set; }
        public string ImageLocation { get; private set; }

        public Product(string name, int price, string description, string imageLocation)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageLocation = imageLocation;
        }
    }
}
