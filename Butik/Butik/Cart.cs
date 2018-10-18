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
    class ProductTest
    {
        public string Name { get; private set; }
        public int Price { get; private set; }
        public int Quantity { get; private set; }
        
        public ProductTest(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }

    class Cart
    {
        static List<ProductTest> cartTest = new List<ProductTest> { };

        static List<Product> cart = new List<Product> { };

        static Label priceLabel;

        static double totalCost;

        public static TableLayoutPanel GetPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 5
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
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

            Label totalCostLabel = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Total cost:",
                Dock = DockStyle.Fill
            };
            panel.SetCellPosition(totalCostLabel, new TableLayoutPanelCellPosition(1, 2));
            panel.Controls.Add(totalCostLabel);
            
            priceLabel = new Label()
            {
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "$" + totalCost,
                Dock = DockStyle.Fill
            };
            panel.SetCellPosition(priceLabel, new TableLayoutPanelCellPosition(2, 2));
            panel.Controls.Add(priceLabel);

            TextBox discountTextBox = new TextBox()
            {
                Text = "Enter discount code here...",
                Dock = DockStyle.Fill
            };
            discountTextBox.Click += DiscountTextBoxClick;
            panel.SetColumnSpan(discountTextBox, 2);
            panel.SetCellPosition(discountTextBox, new TableLayoutPanelCellPosition(0, 3));
            panel.Controls.Add(discountTextBox);

            Button discountButton = new Button()
            {
                Text = "Submit",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            panel.SetCellPosition(discountButton, new TableLayoutPanelCellPosition(3, 3));
            panel.Controls.Add(discountButton);

            Button clearCartButton = new Button()
            {
                Text = "Clear cart",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            panel.SetCellPosition(clearCartButton, new TableLayoutPanelCellPosition(0, 4));
            panel.Controls.Add(clearCartButton);

            Button placeOrderButton = new Button()
            {
                Text = "Place order",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            placeOrderButton.Click += PlaceOrderButtonClick;
            panel.SetCellPosition(placeOrderButton, new TableLayoutPanelCellPosition(1, 4));
            panel.SetColumnSpan(placeOrderButton, 2);
            panel.Controls.Add(placeOrderButton);
            
            return panel;
        }

        public static void AddProduct(Product product)
        {
            cart.Add(product);
            totalCost += product.Price;
            priceLabel.Text = "$" + totalCost;
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
