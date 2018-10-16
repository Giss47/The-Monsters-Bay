using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace Butik
{
    class MyForm : Form
    {
        public MyForm()
        {
            Width = 1000;
            Height = 650;
            Text = "the Monsters Bay";
            Icon = new Icon("resources/icon.ico");
        }
    }
}
