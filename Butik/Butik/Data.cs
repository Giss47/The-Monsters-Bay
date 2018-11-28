using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;

namespace Butik
{
    class Data
    {
        public string CartFile => @"C:\Windows\Temp\cart.mbc";
        private string DiscountFile => @"resources\DiscountList.csv";
        
        private static SqlConnection con;

        public Product[] Products { get; }

        public Dictionary<string, double> discountCodes = new Dictionary<string, double> { };
        public List<CartProduct> cart = new List<CartProduct> { };

        public Data()
        {
            con = Con();
            Products = ImportProducts();

            if (File.Exists(CartFile))
            {
                string[] temp = File.ReadAllLines(CartFile);

                foreach (var s in temp)
                {
                    string[] p = s.Split(';');
                    cart.Add(new CartProduct(p[0], int.Parse(p[1]), double.Parse(p[2])));
                }
            }
            if (File.Exists(DiscountFile))
            {
                string[] getDiscountCodes = File.ReadAllLines(DiscountFile);
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

        public void EraseDiscountCode(string code, double discount)
        {
            string[] temp = File.ReadLines(DiscountFile).Where(d => d != $"{code},{discount}").ToArray();
            File.WriteAllLines(DiscountFile, temp);
            discountCodes.Remove(code);
        }

        private SqlConnection Con()
        {
            return new SqlConnection("Data Source=den1.mssql7.gear.host;" +
                "Initial Catalog=monsterbay;" +
                "Persist Security Info=True;" +
                "User ID=monsterbay;" +
                "Password=monsterbay2018!");
        }
        
        private Product[] ImportProducts()
        {
            Product[] products;
            con.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products", con))
            {
                List<Product> listProducts = new List<Product> { };
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Image img;
                            using (WebClient client = new WebClient())
                            {
                                byte[] imageData = client.DownloadData(Convert.ToString(reader["ImageLocation"]));
                                MemoryStream stream = new MemoryStream(imageData);
                                img = Image.FromStream(stream);
                                stream.Close();
                            }

                            listProducts.Add(new Product(
                                Convert.ToString(reader["Name"]),
                                Convert.ToDouble(reader["Price"]),
                                Convert.ToString(reader["Description"]), img));
                        }
                    }
                }
                return products = listProducts.ToArray();
            }
        }
    }
}
