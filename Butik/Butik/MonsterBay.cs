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
    class MonsterBay : FlowLayoutPanel
    {
        static string[] products = File.ReadAllLines("Trucks.csv");
        static List<Product> productList = new List<Product> { };

        public MonsterBay()
        {
            foreach (string s in products)
            {
                string[] p = s.Split(';');
                productList.Add(new Product(p[0], int.Parse(p[1]), p[2], p[3]));
            }

            Dock = DockStyle.Fill;
            BorderStyle = BorderStyle.FixedSingle;
            AutoScroll = true;
            AutoSize = true;
            BackgroundImage = Image.FromFile(@"resources\backgrounds\2001.png");
            BackgroundImageLayout = ImageLayout.Stretch;

            productList.ForEach(p => Controls.Add(p.GetPictureBox()));
        }
    }
}
