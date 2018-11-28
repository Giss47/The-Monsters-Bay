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
        private Data data;
        private DataGridView cartGrid;
        private TableLayoutPanel checkOutCalculationsCorner;
        private TextBox discountTextBox;
        private Label priceLabel;
        private Label discountLabel;

        private double totalCost;
        private double totalDiscount;

        public Cart(Data data)
        {
            Dock = DockStyle.Fill;
            ColumnCount = 3;
            RowCount = 2;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            RowStyles.Add(new RowStyle(SizeType.Absolute, 140));

            

            this.data = data;

            this.data.cart.ForEach(p => totalCost += p.Cost);

            cartGrid = CreateCartGrid();
            SetColumnSpan(cartGrid, 3);
            Controls.Add(cartGrid);

            checkOutCalculationsCorner = CreateCheckoutCalculationsCorner();
            SetColumnSpan(checkOutCalculationsCorner, 3);
            Controls.Add(checkOutCalculationsCorner);
        }

        private DataGridView CreateCartGrid()
        {
            var cartGrid = new DataGridView()
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
                cartGrid.DataSource = data.cart;
            }
            return cartGrid;
        }

        // -------- Cart Side Components ------- //

        private TableLayoutPanel CreateCheckoutCalculationsCorner()
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

            Button removeAll = CreateButton("Remove All");
            removeAll.Click += RemoveAllClick;
            panel.Controls.Add(removeAll);

            Label totalCostLabel = CreateLabel("Total cost:", ContentAlignment.MiddleRight, Color.Black);
            //panel.SetColumnSpan(totalCostLabel, 2);
            panel.Controls.Add(totalCostLabel);

            priceLabel = CreateLabel("$" + totalCost, ContentAlignment.MiddleCenter, Color.Black);
            panel.Controls.Add(priceLabel);

            discountTextBox = new TextBox()
            {
                Text = "Enter discount code here...",
                Dock = DockStyle.Fill
            };
            panel.SetColumnSpan(discountTextBox, 2);
            panel.Controls.Add(discountTextBox);
            discountTextBox.Click += DiscountTextBoxClick;

            Button discountButton = CreateButton("Submit");
            panel.Controls.Add(discountButton);
            discountButton.Click += SubmitDiscountButtonClick;

            Button clearCartButton = CreateButton("Clear cart");
            panel.Controls.Add(clearCartButton);
            clearCartButton.Click += ClearCart;

            Button placeOrderButton = CreateButton("Place order");
            panel.SetColumnSpan(placeOrderButton, 2);
            panel.Controls.Add(placeOrderButton);
            placeOrderButton.Click += PlaceOrderButtonClick;
            placeOrderButton.Click += ClearCart;

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

        // --------CheckOut Operatoins ------- //

        public void AddProduct(Product product)
        {
            bool productExists = false;

            foreach (var c in data.cart)
            {
                if (product.Name == c.Name)
                {
                    productExists = true;
                    c.IncreaseQuantity();
                }
            }
            if (!productExists)
            {
                data.cart.Add(new CartProduct(product.Name, 1, product.Price));
            }

            data.SaveToFile();
            RecalculateTotalCost();
            RefreshCartGrid();
        }

        // ---------- Operations Buttons Event Handlers --------- // 

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            if (!(data.cart.Count == 0))
            {
                int i = cartGrid.CurrentCell.RowIndex;
                if (data.cart[i].Quantity > 1)
                {
                    data.cart[i].DecreaseQuantity();
                }
                else
                {
                    data.cart.Remove(data.cart[i]);
                }

                RefreshCartGrid();
                data.SaveToFile();
                RecalculateTotalCost();
            }
        }

        private void RemoveAllClick(object sender, EventArgs e)
        {
            if (!(data.cart.Count == 0))
            {
                int i = cartGrid.CurrentCell.RowIndex;
                data.cart.Remove(data.cart[i]);
                RefreshCartGrid();
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
            string message;
            string caption;

            if (data.cart.Count == 0)
            {
                message = "Your shopping cart is empty!";
                caption = "";
                DialogResult r = MessageBox.Show(message, caption,
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Warning);
            }
            else
            {

                var submit = sender as Button;
                var code = "";
                double discount = 0;

                message = "Make sure you finished buying before using your discount code!" +
                                "\n Would you like to proceed?";
                caption = "";
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
                        code = pair.Key;
                        discount = pair.Value;
                    }
                }

                if (code == "")
                {
                    MessageBox.Show("INVALID CODE!\nPlease try again", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    totalDiscount = totalCost * (discount / 100);
                    totalCost -= totalDiscount;
                    priceLabel.Text = "$" + totalCost;
                    discountLabel.Text = "-$" + totalDiscount;
                    
                    data.EraseDiscountCode(code, discount);
                    submit.Enabled = false;

                    RefreshCartGrid();
                }
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
            RefreshCartGrid();
        }

        private void PlaceOrderButtonClick(object sender, EventArgs e1)
        {
            if (data.cart.Count != 0)
            {
                var receipt = "Your order has been placed.\r\n" +
                    "\r\n\r\n" +
                    "Qty:\tProduct\t\tUnit price:\tCost:\r\n";

                foreach (var p in data.cart)
                {
                    var tab = "\t";
                    if (p.Name.Length < 8)
                        tab = "\t\t";
                    receipt += string.Format("{0}\t{1}" + tab + "${2}\t\t${3}\r\n", p.Quantity, p.Name, p.Price, (p.Price * p.Quantity));
                }
                receipt += "\r\n \t\t\tTotal discount:\t$" + totalDiscount;
                receipt += "\r\n \t\t\tTotal price:\t$" + totalCost;

                DialogResult result = MessageBox.Show(
                        receipt + "\r\n \r\n Would you like to print the receipt?",
                        "Receipt",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2
                        );

                if (result == DialogResult.Yes)
                {
                    var p = new PrintDocument() { DocumentName = "Receipt from the Monster Bay" };
                    var printDialog = new PrintDialog() { Document = p };
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
                RefreshCartGrid();
            }
            else
            {
                MessageBox.Show("You have no products in your cart!",
                    "Empty cart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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

        private void RefreshCartGrid()
        {
            cartGrid.DataSource = null;
            cartGrid.DataSource = data.cart;
        }
    }
}