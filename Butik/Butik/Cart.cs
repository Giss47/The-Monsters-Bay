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
    class CartProduct
    {
        public string Name { get; private set; }
        public int Quantity { get; set; }

        [System.ComponentModel.Browsable(false)]
        public double Price { get; private set; }

        public double Cost { get; set; }

        public CartProduct(string name, int quantity, double price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
            Cost = Quantity * Price;
        }
    }

    class Cart : TableLayoutPanel
    {
        private static List<CartProduct> cart = new List<CartProduct> { };
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
                    cart.Add(new CartProduct(p[0], int.Parse(p[1]), double.Parse(p[2])));
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
            RowCount = 2;

            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            RowStyles.Add(new RowStyle(SizeType.Absolute, 140));

            productGrid = new DataGridView()
            {
                Dock = DockStyle.Fill,
                DataSource = cart,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            SetColumnSpan(productGrid, 3);
            Controls.Add(productGrid);

            TableLayoutPanel checkoutBox = CreateCheckoutBox();
            Controls.Add(checkoutBox);
            SetColumnSpan(checkoutBox, 3);
        }

        // Controls methods
        private static TableLayoutPanel CreateCheckoutBox()
        {
            TableLayoutPanel panel = new TableLayoutPanel()
            {
                ColumnCount = 3,
                RowCount = 4,
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            Label discountLabelText = CreateLabel("Discount:", ContentAlignment.MiddleRight, Color.Black);
            panel.SetColumnSpan(discountLabelText, 2);
            panel.Controls.Add(discountLabelText);

            discountLabel = CreateLabel("$" + totalDiscount, ContentAlignment.MiddleCenter, Color.Red);
            panel.Controls.Add(discountLabel);

            Label totalCostLabel = CreateLabel("Total cost:", ContentAlignment.MiddleRight, Color.Black);
            panel.SetColumnSpan(totalCostLabel, 2);
            panel.Controls.Add(totalCostLabel);

            priceLabel = CreateLabel("$" + totalCost, ContentAlignment.MiddleCenter, Color.Black);
            panel.Controls.Add(priceLabel);

            discountTextBox = new TextBox()
            {
                Text = "Enter discount code here...",
                Dock = DockStyle.Fill
            };
            discountTextBox.Click += DiscountTextBoxClick;
            panel.SetColumnSpan(discountTextBox, 2);
            panel.Controls.Add(discountTextBox);

            Button discountButton = CreateButton("Submit");
            discountButton.Click += SubmitDiscountButtonClick;
            panel.Controls.Add(discountButton);

            Button clearCartButton = CreateButton("Clear cart");
            clearCartButton.Click += ClearCart;
            panel.Controls.Add(clearCartButton);

            Button placeOrderButton = CreateButton("Place order");
            placeOrderButton.Click += PlaceOrderButtonClick;
            placeOrderButton.Click += ClearCart;
            panel.SetColumnSpan(placeOrderButton, 2);
            panel.Controls.Add(placeOrderButton);

            return panel;
        }

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
            bool productCheck = false;
            
            foreach (CartProduct c in cart)
            {
                if (product.Name == c.Name)
                {
                    productCheck = true;
                    c.Quantity++;
                    c.Cost = c.Price * c.Quantity;
                }
            }
            if (!productCheck)
            {
                cart.Add(new CartProduct(product.Name, 1, product.Price));
            }
            SaveToFile();
            totalCost = 0;
            foreach (CartProduct p in cart)
            {
                totalCost += p.Cost;
            }
            priceLabel.Text = "$" + totalCost;
            
            RefreshDataGrid();
        }
        private static void SaveToFile()
        {
            List<string> cartString = new List<string> { };
            cart.ForEach(p => cartString.Add(p.Name + ";" + p.Quantity + ";" + p.Price));
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
                foreach (CartProduct p in cart)
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