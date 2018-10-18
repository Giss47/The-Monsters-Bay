using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;

namespace Butik
{
    class Cart
    {
        static List<Product> cart = new List<Product> { };
        static Dictionary<string, double> discountCodes = new Dictionary<string, double> { };
        static TextBox discountTextBox;

        static Label priceLabel;
        static Label discountLabel;

        static double totalCost;
        static double totalDiscount;

        static string cartFile = @"C:\Windows\Temp\cart.mbc";

        public static TableLayoutPanel GetPanel()
        {
            if (File.Exists(cartFile))
            {
                string[] temp = File.ReadAllLines(cartFile);

                foreach (string s in temp)
                {
                    string[] p = s.Split(';');
                    cart.Add(new Product(p[0], int.Parse(p[1]), p[2], p[3], int.Parse(p[4])));
                    totalCost += double.Parse(p[1]);
                }
            }
            string[] getDiscountCodes = File.ReadAllLines("DiscountList.csv");
            foreach (var item in getDiscountCodes)
            {
                string[] parts = item.Split(',');
                discountCodes.Add(parts[0], int.Parse(parts[1]));
            }


            TableLayoutPanel panel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 6
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            Label productLabel = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Products:",
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(productLabel);


            Label costLabel = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Cost:",
                Dock = DockStyle.Fill
            };
            panel.SetCellPosition(costLabel, new TableLayoutPanelCellPosition(2, 0));
            panel.Controls.Add(costLabel);

            Label discoutLabelText = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Discount:",
                Dock = DockStyle.Fill
            };
            panel.SetCellPosition(discoutLabelText, new TableLayoutPanelCellPosition(1, 2));
            panel.Controls.Add(discoutLabelText);

            discountLabel = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "$" + totalDiscount,
                Dock = DockStyle.Fill,
                ForeColor = Color.Red
            };
            panel.SetCellPosition(discountLabel, new TableLayoutPanelCellPosition(2, 2));
            panel.Controls.Add(discountLabel);

            Label totalCostLabel = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Total cost:",
                Dock = DockStyle.Fill
            };
            panel.SetCellPosition(totalCostLabel, new TableLayoutPanelCellPosition(1, 3));
            panel.Controls.Add(totalCostLabel);

            priceLabel = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "$" + totalCost,
                Dock = DockStyle.Fill
            };
            panel.SetCellPosition(priceLabel, new TableLayoutPanelCellPosition(2, 3));
            panel.Controls.Add(priceLabel);

            discountTextBox = new TextBox()
            {
                Text = "Enter discount code here...",
                Dock = DockStyle.Fill
            };
            discountTextBox.Click += DiscountTextBoxClick;
            panel.SetColumnSpan(discountTextBox, 2);
            panel.SetCellPosition(discountTextBox, new TableLayoutPanelCellPosition(0, 4));
            panel.Controls.Add(discountTextBox);

            Button discountButton = new Button()
            {
                Text = "Submit",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            discountButton.Click += DiscountButtonClick;
            panel.SetCellPosition(discountButton, new TableLayoutPanelCellPosition(3, 4));
            panel.Controls.Add(discountButton);

            Button clearCartButton = new Button()
            {
                Text = "Clear cart",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            clearCartButton.Click += ClearCart;
            panel.SetCellPosition(clearCartButton, new TableLayoutPanelCellPosition(0, 5));
            panel.Controls.Add(clearCartButton);

            Button placeOrderButton = new Button()
            {
                Text = "Place order",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            placeOrderButton.Click += PlaceOrderButtonClick;
            placeOrderButton.Click += ClearCart;
            panel.SetCellPosition(placeOrderButton, new TableLayoutPanelCellPosition(1, 5));
            panel.SetColumnSpan(placeOrderButton, 2);
            panel.Controls.Add(placeOrderButton);

            return panel;
        }

        private static void DiscountButtonClick(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, double> pair in discountCodes)
            {
                if(discountTextBox.Text == pair.Key)
                {
                    totalDiscount = totalCost * (pair.Value / 100);
                    totalCost -= totalDiscount;
                    priceLabel.Text = "$" + totalCost;
                    discountLabel.Text = "-$" + totalDiscount;
                }
            }
        }

        public static void AddProduct(Product product)
        {
            cart.Add(product);
            SaveToFile();
            totalCost += product.Price;
            priceLabel.Text = "$" + totalCost;
        }

        public static void ClearCart(object sender, EventArgs e)
        {
            cart.Clear();
            totalCost = 0;
            priceLabel.Text = "$" + totalCost;
            if (File.Exists(cartFile))
            {
                File.Delete(cartFile);
            }
        }

        public static void SaveToFile()
        {
            List<string> cartString = new List<string> { };
            cart.ForEach(p => cartString.Add(p.Name + ";" + p.Price + ";" + p.Description + ";" + p.ImageLocation + ";" + p.Quantity));
            File.WriteAllLines(@"C:\Windows\Temp\cart.mbc", cartString);
        }

        private static void DiscountTextBoxClick(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Text = "";
        }

        private static void PlaceOrderButtonClick(object sender1, EventArgs e1)
        {
            string receipt = "";
            foreach (Product p in cart)
            {
                receipt += string.Format("{0} x {1} (${2}) \r\n", p.Quantity, p.Name, p.Price);
            }
            receipt += "\r\n Total price: $" + totalCost;

            DialogResult result = MessageBox.Show(
                    receipt + "\r\n \r\n Would you like to print your receipt?",
                    "Receipt",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                    );

            if (result == DialogResult.Yes)
            {
                PrintDocument p = new PrintDocument() { DocumentName = "Receipt from the Monster Bay" };
                PrintDialog printDialog = new PrintDialog() { Document = p };
                p.PrintPage += delegate (object sender2, PrintPageEventArgs e2)
                {
                    e2.Graphics.DrawString(receipt, new Font("Arial", 12), new SolidBrush(Color.Black),
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
