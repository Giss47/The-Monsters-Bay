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
        static FlowLayoutPanel panel1;

        public MyForm()
        {
            MaximumSize = new Size(1300, 675);
            MinimumSize = new Size(570, 280);
            Width = 1300;
            Height = 675;
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

            panel1 = MonsterBay.GetPanel();
            mainPanel.Controls.Add(panel1);

            mainPanel.Controls.Add(Cart.GetPanel());

            FormClosing += MyForm_FormClosing;

        }

        public static void ChangePanel(FlowLayoutPanel newPanel)
        {
            mainPanel.Controls.Remove(panel1);
            panel1 = newPanel;
            mainPanel.SetCellPosition(panel1, new TableLayoutPanelCellPosition(0, 1));
            mainPanel.Controls.Add(panel1);
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
