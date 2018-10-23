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
        static TableLayoutPanel mainPanel;
        static MonsterBay bayPanel;
        static Cart cartPanel;

        public MyForm()
        {
            MinimumSize = new Size(585, 310);
            Width = 1330;
            Height = 685;
            Text = "The Monsters Bay";
            Icon = new Icon("resources/icon.ico");
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(5);
            BackColor = Color.LightGray;

            mainPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
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

            bayPanel = new MonsterBay();
            mainPanel.SetCellPosition(bayPanel, new TableLayoutPanelCellPosition(0, 1));
            mainPanel.Controls.Add(bayPanel);

            cartPanel = new Cart();
            mainPanel.SetCellPosition(cartPanel, new TableLayoutPanelCellPosition(1, 1));

            mainPanel.Controls.Add(cartPanel);
            

            FormClosing += MyForm_FormClosing;
        }

        public static void InsertProductPanel(FlowLayoutPanel productPanel)
        {
            mainPanel.Controls.Remove(mainPanel.GetControlFromPosition(0, 1));
            mainPanel.Controls.Add(productPanel, 0, 1);
        }

        public static void InsertBayPanel()
        {
            mainPanel.Controls.Remove(mainPanel.GetControlFromPosition(0, 1));
            mainPanel.Controls.Add(bayPanel, 0, 1);
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "Are you sure  you would like to exit The Monsters Bay?";
            string caption = "Exit";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.No)
                e.Cancel = true;

        }
    }
}
