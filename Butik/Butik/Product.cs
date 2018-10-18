using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Butik
{
    

    class Product
    {
        public string Name { get; private set; }
        public double Price { get; private set; }
        public string Description { get; private set; }
        public string ImageLocation { get; private set; }
        public int Quantity { get; private set; }

        public Product(string name, int price, string description, string image, int quantity = 1)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageLocation = image;
            Quantity = quantity;
        }

        public FlowLayoutPanel GetInfoPanel()
        {
            
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = Color.Bisque

            };
            panel.Controls.Add(new PictureBox { Image = Image.FromFile(ImageLocation), Size = new Size(600, 550), SizeMode = PictureBoxSizeMode.StretchImage });

            TableLayoutPanel table = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 4,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(table);

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 350));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            Label title = new Label()
            {
                Text = Name,
                Font = new Font("Arial", 17),
                AutoSize = true
            };
            table.Controls.Add(title);
            table.SetColumnSpan(title, 2);

            Label description = new Label()
            {
                Text = Description,
                Font = new Font("Arial", 17),
                AutoSize = true
            };
            table.Controls.Add(description);
            table.SetColumnSpan(description, 2);

            Label priceLabel = new Label { Text = "$" + Price.ToString(),
                                           Font = new Font("Arial", 15),
                                           ImageAlign = ContentAlignment.MiddleCenter
                                          };
            table.Controls.Add(priceLabel);
            table.SetColumnSpan(priceLabel, 2);

            Button back = new Button()
            {
                Text = "Back",
                AutoSize = true
            };
            back.Click += (s, e) => { MyForm.ChangePanel(MonsterBay.GetPanel()); };
            table.Controls.Add(back);

            Button addToCart = new Button()
            {
                Text = "Add to cart",
                AutoSize = true
            };
            addToCart.Click += (s, e) => { Cart.AddProduct(this); };
            table.Controls.Add(addToCart);

            return panel;
        }

        public PictureBox GetPictureBox()
        {
            ToolTip popUp = new ToolTip();
            PictureBox box = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = Image.FromFile(ImageLocation),
                Size = new Size(200, 150),
                Margin = new Padding(20),
                Cursor = Cursors.Hand
            };
            popUp.SetToolTip(box, "Click to view details");

            box.Click += (s, e) => {
                MyForm.ChangePanel(GetInfoPanel());
            };

            return box;
        }

        public void ChangeQuantity(int change)
        {
            Quantity += change;
        }
    }
}
