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
        public MonsterBay()
        {
            Dock = DockStyle.Fill;
            BorderStyle = BorderStyle.FixedSingle;
            AutoSize = true;
            BackgroundImage = Image.FromFile(@"resources\backgrounds\2001.png");
            BackgroundImageLayout = ImageLayout.Stretch;


            ToolTip popUp = new ToolTip();
           string[] pics = Directory.GetFiles(@"resources\tucksPics\", "*.jpg")
                                     .Select(Path.GetFileName)
                                     .ToArray();

            for (int i = 1; i < pics.Length +1; i++)
            {
                PictureBox picture = new PictureBox { SizeMode = PictureBoxSizeMode.StretchImage,
                                                      Image = Image.FromFile(string.Format(@"resources\tucksPics\Bild{0}.jpg", i)),
                                                      Size = new Size(200, 150),
                                                      Margin = new Padding(20)};
                Controls.Add(picture);
                popUp.SetToolTip(picture, "Click to view details");
            }
        }
    }
}
