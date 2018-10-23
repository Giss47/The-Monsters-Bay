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
    class Cart : TableLayoutPanel
    {
        private static List<Product> cart = new List<Product> { };
        private static Dictionary<string, double> discountCodes = new Dictionary<string, double> { };

        private static TextBox discountTextBox;
        private static Label priceLabel;
        private static Label discountLabel;
        private static DataGridView productGrid;

        private static double totalCost;
        private static double totalDiscount;
        private static string cartFile = @"C:\Windows\Temp\cart.mbc";

        public Cart()
        {
            if (File.Exists(cartFile))
            {
                string[] temp = File.ReadAllLines(cartFile);

                foreach (string s in temp)
                {
                    string[] p = s.Split(';');
                    cart.Add(new Product(p[0], int.Parse(p[1]), p[2], p[3], int.Parse(p[4])));
                }
            }
            if (File.Exists("DiscountList.csv"))
            {
                string[] getDiscountCodes = File.ReadAllLines("DiscountList.csv");

                foreach (string s in getDiscountCodes)
                {
                    string[] codes = s.Split(',');
                    discountCodes.Add(codes[0], int.Parse(codes[1]));
                }
            }

            cart.ForEach(p => totalCost += p.Cost);

            Dock = DockStyle.Fill;
            ColumnCount = 3;
            RowCount = 6;

            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            productGrid = new DataGridView()
            {
                Dock = DockStyle.Fill,
                DataSource = cart,
                RowHeadersVisible = false
            };
            SetCellPosition(productGrid, new TableLayoutPanelCellPosition(0, 0));
            SetColumnSpan(productGrid, 3);
            Controls.Add(productGrid);

            Label discountLabelText = CreateLabel("Discount:", ContentAlignment.MiddleRight, Color.Black);
            SetCellPosition(discountLabelText, new TableLayoutPanelCellPosition(1, 1));
            Controls.Add(discountLabelText);

            discountLabel = CreateLabel("$" + totalDiscount, ContentAlignment.MiddleCenter, Color.Red);
            SetCellPosition(discountLabel, new TableLayoutPanelCellPosition(2, 1));
            Controls.Add(discountLabel);

            Label totalCostLabel = CreateLabel("Total cost:", ContentAlignment.MiddleRight, Color.Black);
            SetCellPosition(totalCostLabel, new TableLayoutPanelCellPosition(1, 2));
            Controls.Add(totalCostLabel);

            priceLabel = CreateLabel("$" + totalCost, ContentAlignment.MiddleCenter, Color.Black);
            SetCellPosition(priceLabel, new TableLayoutPanelCellPosition(2, 2));
            Controls.Add(priceLabel);

            discountTextBox = new TextBox()
            {
                Text = "Enter discount code here...",
                Dock = DockStyle.Fill
            };
            discountTextBox.Click += DiscountTextBoxClick;
            SetColumnSpan(discountTextBox, 2);
            SetCellPosition(discountTextBox, new TableLayoutPanelCellPosition(0, 3));
            Controls.Add(discountTextBox);

            Button discountButton = CreateButton("Submit");
            discountButton.Click += SubmitDiscountButtonClick;
            SetCellPosition(discountButton, new TableLayoutPanelCellPosition(3, 3));
            Controls.Add(discountButton);

            Button clearCartButton = CreateButton("Clear cart");
            clearCartButton.Click += ClearCart;
            SetCellPosition(clearCartButton, new TableLayoutPanelCellPosition(0, 4));
            Controls.Add(clearCartButton);

            Button placeOrderButton = CreateButton("Place order");
            placeOrderButton.Click += PlaceOrderButtonClick;
            placeOrderButton.Click += ClearCart;
            SetCellPosition(placeOrderButton, new TableLayoutPanelCellPosition(1, 4));
            SetColumnSpan(placeOrderButton, 2);
            Controls.Add(placeOrderButton);
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
                product.IncreaseQuantity();
            }
            else
            {
                cart.Add(product);
            }
            SaveToFile();
            totalCost = 0;
            foreach (Product p in cart)
            {
                totalCost += p.Cost;
            }
            priceLabel.Text = "$" + totalCost;

            RefreshDataGrid();
        }
        private static void SaveToFile()
        {
            List<string> cartString = new List<string> { };
            cart.ForEach(p => cartString.Add(p.Name + ";" + p.Price + ";" + p.Description + ";" + p.ImageLocation + ";" + p.Quantity));
            File.WriteAllLines(cartFile, cartString);
        }
        private static void RefreshDataGrid()
        {
            productGrid.DataSource = null;
            productGrid.DataSource = cart;
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

            RefreshDataGrid();
        }
        private static void DiscountTextBoxClick(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Text = "";
        }
        private static void SubmitDiscountButtonClick(object sender, EventArgs e)
        {
            string keyCeck = "";
            double valueCheck = 0;

            foreach (KeyValuePair<string, double> pair in discountCodes)
            {
                if (discountTextBox.Text == pair.Key)
                {
                    keyCeck = pair.Key;
                    valueCheck = pair.Value;
                }
            }

            if (keyCeck == "")
            {
                MessageBox.Show("INVALID CODE!\nPlease try again", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                totalDiscount = totalCost * (valueCheck / 100);
                totalCost -= totalDiscount;
                priceLabel.Text = "$" + totalCost;
                discountLabel.Text = "-$" + totalDiscount;
                string[] temp = File.ReadLines("DiscountList.csv").Where(l => l != $"{keyCeck},{valueCheck}").ToArray();
                File.WriteAllLines("DiscountList.csv", temp);
                discountCodes.Remove(keyCeck);

                RefreshDataGrid();
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
                RefreshDataGrid();
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