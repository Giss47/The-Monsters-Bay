using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Butik
{
    class Data
    {
        public string CartFile => @"C:\Windows\Temp\cart.mbc";

        private static string[] stringProducts = File.ReadAllLines("Trucks.csv");

        private Product[] products = new Product[stringProducts.Length];
        public Product[] Products => products;

        public Dictionary<string, double> discountCodes = new Dictionary<string, double> { };
        public List<CartProduct> cart = new List<CartProduct> { };

        public Data()
        {
            var count = 0;
            var errorLines = "";
            for (var i = 0; i < stringProducts.Length; i++)
            {
                string[] p = stringProducts[i].Split(';');

                if (p.Length < 4)
                {
                    errorLines += " . " + (i + 1);
                    Array.Resize(ref products, products.Length - 1);
                }
                else
                {
                    products[count] = new Product(p[0], int.Parse(p[1]), p[2], p[3]);

                    count++;
                }
            }
            if (errorLines != "")
            {
                MessageBox.Show("\tWarning! " +
                                "\nInformation missing in products file, " +
                                "Line/Lines: " + errorLines);
            }

            if (File.Exists(CartFile))
            {
                string[] temp = File.ReadAllLines(CartFile);

                foreach (var s in temp)
                {
                    string[] p = s.Split(';');
                    cart.Add(new CartProduct(p[0], int.Parse(p[1]), double.Parse(p[2])));
                }
            }
            if (File.Exists("DiscountList.csv"))
            {
                string[] getDiscountCodes = File.ReadAllLines("DiscountList.csv");
                foreach (var s in getDiscountCodes)
                {
                    string[] codes = s.Split(',');
                    if (!(codes[0] == "") && !(codes[1] == ""))
                    {
                        discountCodes.Add(codes[0], int.Parse(codes[1]));
                    }
                }
            }
        }

        public void SaveToFile()
        {
            var cartString = new List<string> { };
            cart.ForEach(p => cartString.Add(p.Name + ";" + p.Quantity + ";" + p.Price));
            File.WriteAllLines(CartFile, cartString);
        }
    }
}
