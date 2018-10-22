using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Butik
{
    static class MonsterBay
    {
        static string[] products = File.ReadAllLines("Trucks.csv");
        static List<Product> productList = new List<Product> { };

        public static FlowLayoutPanel GetPanel()
        {
            foreach (string s in products)
            {
                string[] p = s.Split(';');
                productList.Add(new Product(p[0], int.Parse(p[1]), p[2], p[3]));
            }

            FlowLayoutPanel panel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                BackgroundImage = Image.FromFile(@"resources\backgrounds\2001.png"),
                BackgroundImageLayout = ImageLayout.Stretch,
            };
            
            productList.ForEach(p => panel.Controls.Add(p.GetPictureBox()));
            
            return panel;
        }
    }
}
