using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;

namespace Butik
{
    class Cart : TableLayoutPanel
    {
        private TextBox discountTextBox;
        private Label priceLabel;
        private Label discountLabel;
        private DataGridView productGrid;
        private Data data;

        private double totalCost;
        private double totalDiscount;

        public Cart(Data d)
        {
            data = d;

            data.cart.ForEach(p => totalCost += p.Cost);

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
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
            };

            if (!(data.cart.Count == 0))
            {
                productGrid.DataSource = data.cart;
            }
            SetColumnSpan(productGrid, 3);
            Controls.Add(productGrid);

            TableLayoutPanel checkoutBox = CreateCheckoutBox();
            Controls.Add(checkoutBox);
            SetColumnSpan(checkoutBox, 3);
        }

        // Control methods.
        private TableLayoutPanel CreateCheckoutBox()
        {
            var panel = new TableLayoutPanel()
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

            Button removeButton = CreateButton("Remove item");
            removeButton.Click += RemoveButtonClick;
            panel.Controls.Add(removeButton);

            Label discountLabelText = CreateLabel("Discount:", ContentAlignment.MiddleRight, Color.Black);
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

        private Button CreateButton(string text)
        {
            return new Button()
            {
                Text = text,
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
        }

        private Label CreateLabel(string text, ContentAlignment align, Color color)
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

        // Operation methods.
        public void AddProduct(Product product)
        {
            bool productExists = false;
            
            foreach (var c in data.cart)
            {
                if (product.Name == c.Name)
                {
                    productExists = true;
                    c.IncreaseQuantity();
                    c.RecalculateCost();
                }
            }
            if (!productExists)
            {
                data.cart.Add(new CartProduct(product.Name, 1, product.Price));
            }

            data.SaveToFile();
            RecalculateTotalCost();
            RefreshDataGrid();
        }
        
        private void RefreshDataGrid()
        {
            productGrid.DataSource = null;
            productGrid.DataSource = data.cart;
        }

        private void RecalculateTotalCost()
        {
            totalCost = 0;
            foreach (var p in data.cart)
            {
                totalCost += p.Cost;
            }
            priceLabel.Text = "$" + totalCost;
        }

        // Eventlisteners.
        private void RemoveButtonClick(object sender, EventArgs e)
        {
            if(!(data.cart.Count == 0))
            {
                int i = productGrid.CurrentCell.RowIndex;
                data.cart.Remove(data.cart[i]);
                RefreshDataGrid();
                data.SaveToFile();
                RecalculateTotalCost();
            } 
        }

        private void DiscountTextBoxClick(object sender, EventArgs e)
        {
            var t = (TextBox)sender;
            t.Text = "";
        }

        private void SubmitDiscountButtonClick(object sender, EventArgs e)
        {
            var submit = sender as Button; 
            string keyCeck = "";
            double valueCheck = 0;

            string message = "Make sure you finished buying before using your discount code!" +
                             "\n Would you like to proceed?"; 
            string caption = "";
            DialogResult result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }


            foreach (KeyValuePair<string, double> pair in data.discountCodes)
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

                // Erasing Discount Code from file.
                string[] temp = File.ReadLines("DiscountList.csv").Where(d => d != $"{keyCeck},{valueCheck}").ToArray();
                File.WriteAllLines("DiscountList.csv", temp);
                data.discountCodes.Remove(keyCeck);
                submit.Enabled = false; 

                RefreshDataGrid();
            }

        }

        private void ClearCart(object sender, EventArgs e)
        {
            data.cart.Clear();
            totalDiscount = 0;
            discountLabel.Text = "$" + totalDiscount;
            
            if (File.Exists(data.CartFile))
            {
                File.Delete(data.CartFile);
            }

            RecalculateTotalCost();
            RefreshDataGrid();
        }

        private void PlaceOrderButtonClick(object sender, EventArgs e1)
        {
            if (data.cart.Count != 0)
            {
                string receipt = "Your order has been placed.\r\n" +
                    "\r\n\r\n" +
                    "Qty:\tProduct\t\tUnit price:\tCost:\r\n";

                foreach (var p in data.cart)
                {
                    var doubleTab = "\t";
                    if (p.Name.Length < 8)
                        doubleTab = "\t\t";
                    receipt += string.Format("{0}\t{1}" + doubleTab + "${2}\t\t${3}\r\n", p.Quantity, p.Name, p.Price, (p.Price * p.Quantity));
                }
                receipt += "\r\n \t\t\tTotal price: $" + totalCost;

                DialogResult result = MessageBox.Show(
                        receipt + "\r\n \r\n Would you like to print the receipt?",
                        "Receipt",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2
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