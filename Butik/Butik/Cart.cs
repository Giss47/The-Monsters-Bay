using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Butik
{
    class Cart : TableLayoutPanel
    {
        public Cart()
        {
            Dock = DockStyle.Fill;
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }



        // Följande kod öppnar en messagebox med kvitto (ej specificerat än) och låter en
        // välja om man vill skriva ut eller ej.
        private void PlaceOrderButtonClick(object sender, EventArgs e)
        {
            string receipt = "This is your receipt.\r\n" +
                "You bought some stuff.\r\n" +
                "This is just random text.\r\n" +
                "\r\n" +
                "Would you like to print a copy?";

            DialogResult result = MessageBox.Show(
                    receipt,
                    "Receipt",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                    );

            if (result == DialogResult.Yes)
            {
                PrintDocument p = new PrintDocument() { DocumentName = "Receipt from the Monster Bay" };
                PrintDialog printDialog = new PrintDialog() { Document = p };
                p.PrintPage += delegate (object sender2, PrintPageEventArgs e1)
                {
                    e1.Graphics.DrawString(receipt, new Font("Arial", 12), new SolidBrush(Color.Black),
                        new RectangleF(50, 50, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
                };
                DialogResult printResult = printDialog.ShowDialog();

                if (printResult == DialogResult.OK)
                {
                    p.Print();
                }
            }
        }
    }
}
