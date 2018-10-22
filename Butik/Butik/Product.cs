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
                BackgroundImage = Image.FromFile(@"resources\backgrounds\secondWindow.jpg"),               

            };
            panel.Controls.Add(new PictureBox { Image = Image.FromFile(ImageLocation),
                                                Size = new Size(590, 540),
                                                SizeMode = PictureBoxSizeMode.Zoom,
                                                Margin = new Padding(10),
                                                BackColor = Color.Transparent });

            TableLayoutPanel table = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 4,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(table);

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 300));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            table.Size = new Size(350, 300);
            table.BackColor = Color.Transparent;

            Label title = new Label()
            {
                Text = Name,
                Font = new Font("Arial", 20),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.White
            };
            table.Controls.Add(title);
            table.SetColumnSpan(title, 2);

            Label description = new Label()
            {
                Text = Description,
                Font = new Font("Arial", 17),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White

            };
            table.Controls.Add(description);
            table.SetColumnSpan(description, 2);

            Label priceLabel = new Label
            {
                Text = "$" + Price.ToString(),
                Font = new Font("Arial", 30),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Dock = DockStyle.Fill,
                ForeColor = Color.Purple

            };
            table.Controls.Add(priceLabel);
            table.SetColumnSpan(priceLabel, 2);

            Button back = new Button()
            {
                Text = "Back",
                Size = new Size(200,50)
            };
            back.Click += (s, e) => { MyForm.BayPanel(); };
            table.Controls.Add(back);

            Button addToCart = new Button()
            {
                Text = "Add to cart",
                Size = new Size(200, 50)               
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
                Cursor = Cursors.Hand,
                Padding = new Padding(5)
            };
            popUp.SetToolTip(box, "Click to view details");

            box.Click += (s, e) =>
            {
                MyForm.ChangePanel(GetInfoPanel());
            };
            box.MouseHover += (s, e) =>
            {
                box.BackColor = Color.Red;
            };
            box.MouseLeave += (s, e) =>
            {
                box.BackColor = Color.LightGray;
            };

            return box;
        }

        public void ChangeQuantity(int change)
        {
            Quantity += change;
        }
    }
}
