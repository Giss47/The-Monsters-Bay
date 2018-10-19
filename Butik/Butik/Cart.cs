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
        private static List<Product> cart = new List<Product> { };
        private static Dictionary<string, double> discountCodes = new Dictionary<string, double> { };

        private static TextBox discountTextBox;
        private static Label priceLabel;
        private static Label discountLabel;

        private static double totalCost;
        private static double totalDiscount;
        private static string cartFile = @"C:\Windows\Temp\cart.mbc";


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

            Label productLabel = CreateLabel("Products:", ContentAlignment.MiddleLeft, Color.Black);
            panel.Controls.Add(productLabel);

            Label costLabel = CreateLabel("Cost:", ContentAlignment.MiddleRight, Color.Black);
            panel.SetCellPosition(costLabel, new TableLayoutPanelCellPosition(2, 0));
            panel.Controls.Add(costLabel);

            Label discountLabelText = CreateLabel("Discount:", ContentAlignment.MiddleRight, Color.Black);
            panel.SetCellPosition(discountLabelText, new TableLayoutPanelCellPosition(1, 2));
            panel.Controls.Add(discountLabelText);

            discountLabel = CreateLabel("$" + totalDiscount, ContentAlignment.MiddleCenter, Color.Red);
            panel.SetCellPosition(discountLabel, new TableLayoutPanelCellPosition(2, 2));
            panel.Controls.Add(discountLabel);

            Label totalCostLabel = CreateLabel("Total cost:", ContentAlignment.MiddleRight, Color.Black);
            panel.SetCellPosition(totalCostLabel, new TableLayoutPanelCellPosition(1, 3));
            panel.Controls.Add(totalCostLabel);

            priceLabel = CreateLabel("$" + totalCost, ContentAlignment.MiddleCenter, Color.Black);
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

            Button discountButton = CreateButton("Submit");
            discountButton.Click += SubmitDiscountButtonClick;
            panel.SetCellPosition(discountButton, new TableLayoutPanelCellPosition(3, 4));
            panel.Controls.Add(discountButton);

            Button clearCartButton = CreateButton("Clear cart");
            clearCartButton.Click += ClearCart;
            panel.SetCellPosition(clearCartButton, new TableLayoutPanelCellPosition(0, 5));
            panel.Controls.Add(clearCartButton);

            Button placeOrderButton = CreateButton("Place order");
            placeOrderButton.Click += PlaceOrderButtonClick;
            placeOrderButton.Click += ClearCart;
            panel.SetCellPosition(placeOrderButton, new TableLayoutPanelCellPosition(1, 5));
            panel.SetColumnSpan(placeOrderButton, 2);
            panel.Controls.Add(placeOrderButton);

            return panel;
        }

        // Controls methods
        private static Button CreateButton(string text)
        {
            return new Button()
            {
                Text = text,
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
        }
        private static Label CreateLabel(string text, ContentAlignment align, Color color)
        {
            return new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = align,
                Text = text,
                Dock = DockStyle.Fill,
                ForeColor = color
            };
        }
        
        // Operation methods
        public static void AddProduct(Product product)
        {
            if (cart.Contains(product))
            {
                product.ChangeQuantity(1);
            }
            else
            {
                cart.Add(product);
            }
            SaveToFile();
            totalCost += product.Price;
            priceLabel.Text = "$" + totalCost;
        }
        private static void SaveToFile()
        {
            List<string> cartString = new List<string> { };
            cart.ForEach(p => cartString.Add(p.Name + ";" + p.Price + ";" + p.Description + ";" + p.ImageLocation + ";" + p.Quantity));
            File.WriteAllLines(cartFile, cartString);
        }

        // Eventlisteners
        private static void ClearCart(object sender, EventArgs e)
        {
            cart.Clear();
            totalCost = 0;
            totalDiscount = 0;
            priceLabel.Text = "$" + totalCost;
            discountLabel.Text = "$" + totalDiscount;
            if (File.Exists(cartFile))
            {
                File.Delete(cartFile);
            }
        }
        private static void DiscountTextBoxClick(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Text = "";
        }
        private static void SubmitDiscountButtonClick(object sender, EventArgs e)
        {
            double keyCeck = 0;
            string valueCheck = "";

            foreach (KeyValuePair<string, double> pair in discountCodes)
            {
                if (discountTextBox.Text == pair.Key)
                {
                    keyCeck = pair.Value;
                    valueCheck = pair.Key;
                }
            }

            if (keyCeck == 0)
            {
                MessageBox.Show("INVALID CODE!\nPlease try again", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                totalDiscount = totalCost * (keyCeck / 100);
                totalCost -= totalDiscount;
                priceLabel.Text = "$" + totalCost;
                discountLabel.Text = "-$" + totalDiscount;
                string[] temp = File.ReadLines("DiscountList.csv").Where(l => l != $"{keyCeck},{valueCheck}").ToArray();
                File.WriteAllLines("DiscountList.csv", temp);
            }

        }
        private static void PlaceOrderButtonClick(object sender, EventArgs e1)
        {
            if (cart.Count != 0)
            {

                string receipt = "Your order has been placed.\r\n" +
                    "\r\n\r\n" +
                    "Qty:\tProduct\t\tUnit price:\tAmount:\r\n";
                foreach (Product p in cart)
                {
                    string doubleTab = "\t";
                    if (p.Name.Length < 8)
                        doubleTab = "\t\t";
                    receipt += string.Format("{0}\t{1}" + doubleTab + "${2}\t\t{3}\r\n", p.Quantity, p.Name, p.Price, p.Price * p.Quantity);
                }
                receipt += "\r\n \t\t\tTotal price: $" + totalCost;

                DialogResult result = MessageBox.Show(
                        receipt + "\r\n \r\n Would you like to print the receipt?",
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
            else
            {
                MessageBox.Show("You have no products in your cart!",
                    "Empty cart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
