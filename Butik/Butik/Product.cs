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

        public Product(string name, double price, string description, string image)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageLocation = image;
        }

        public FlowLayoutPanel GetInfoPanel()
        {

            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoScroll = true,
                BackgroundImage = Image.FromFile(@"resources\backgrounds\secondWindow.jpg"),
                BackgroundImageLayout = ImageLayout.Stretch
            };

            PictureBox picBox = new PictureBox()
            {
                Image = Image.FromFile(ImageLocation),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Width = 400,
                Height = 300
            };
            panel.Controls.Add(picBox);

            TableLayoutPanel table = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 4,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(table);

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

            Label title = new Label()
            {
                Text = Name,
                Font = new Font("Arial", 20),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
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
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.White
            };
            table.Controls.Add(description);
            table.SetColumnSpan(description, 2);

            Label priceLabel = new Label
            {
                Text = "Price: $" + Price.ToString(),
                Font = new Font("Arial", 17),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                Dock = DockStyle.Fill,
                ForeColor = Color.White
            };
            table.Controls.Add(priceLabel);
            table.SetColumnSpan(priceLabel, 2);

            Button back = new Button()
            {
                Text = "Back",
                Size = new Size(200,50)
            };
            back.Click += (s, e) => { MyForm.InsertBayPanel(); };
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

        public TableLayoutPanel GetProductPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 2,
                Width = 220,
                Height = 185,
                BackColor = Color.Transparent
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

            PictureBox box = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = Image.FromFile(ImageLocation),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Fill,
                Padding = new Padding(3),
                BackColor = Color.LightGray
            };
            panel.SetColumnSpan(box, 2);
            panel.Controls.Add(box);

            ToolTip popUp = new ToolTip();
            popUp.SetToolTip(box, "Click to view details");

            box.Click += (s, e) =>
            {
                MyForm.InsertProductPanel(GetInfoPanel());
            };
            box.MouseHover += (s, e) =>
            {
                box.BackColor = Color.Red;
            };
            box.MouseLeave += (s, e) =>
            {
                box.BackColor = Color.LightGray;
            };

            Label productLabel = new Label()
            {
                Font = new Font("Arial", 10, FontStyle.Bold),
                Text = Name,
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                ForeColor = Color.White
            };
            panel.Controls.Add(productLabel);

            Label priceLabel = new Label()
            {
                Font = new Font("Arial", 10, FontStyle.Bold),
                Text = "$" + Convert.ToString(Price),
                TextAlign = ContentAlignment.TopRight,
                Dock = DockStyle.Fill,
                ForeColor = Color.White
            };
            panel.Controls.Add(priceLabel);

            return panel;
        }
    }
}
