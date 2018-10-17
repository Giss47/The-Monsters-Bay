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
            MaximumSize = new Size(1300, 675);
            Width = 1300;
            Height = 675;
            Text = "The Monsters Bay";
            Icon = new Icon("resources/icon.ico");
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel mainPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            Controls.Add(mainPanel);

            Label titleLabel = new Label()
            {
                Font = new Font("Arial", 18),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Welcome to \"The Monsters Bay\"",
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(titleLabel);

            Label cartLabel = new Label()
            {
                Font = new Font("Arial", 14),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "The Shopping Cart",
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(cartLabel);
            
            mainPanel.Controls.Add(new MonsterBay());
            
            mainPanel.Controls.Add(new Cart());
        }
    }
}
