﻿using System;
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

        static Label priceLabel;

        static double totalCost;

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
            clearCartButton.Click += ClearCart;
            panel.SetCellPosition(clearCartButton, new TableLayoutPanelCellPosition(0, 4));
            panel.Controls.Add(clearCartButton);

            Button placeOrderButton = new Button()
            {
                Text = "Place order",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            placeOrderButton.Click += PlaceOrderButtonClick;
            placeOrderButton.Click += ClearCart;
            panel.SetCellPosition(placeOrderButton, new TableLayoutPanelCellPosition(1, 4));
            panel.SetColumnSpan(placeOrderButton, 2);
            panel.Controls.Add(placeOrderButton);

            return panel;
        }

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