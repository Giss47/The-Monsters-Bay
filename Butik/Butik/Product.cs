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

        // Get Product's Panel (Image, Name, Price).
        public TableLayoutPanel GetProductPanel()
        {
            var panel = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 2,
                Width = 220,
                Height = 180,
                BackColor = Color.Transparent
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

            try
            {
                var box = new PictureBox()
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
                box.Click += (s, e) => { MyForm.InsertProductInfoPanel(GetInfoPanel()); };
                box.MouseHover += (s, e) => { box.BackColor = Color.Red; };
                box.MouseLeave += (s, e) => { box.BackColor = Color.LightGray; };
            }
            catch
            {
                MessageBox.Show("Image not found \nCheck products file or image directory");
                var box = new PictureBox()
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Cursor = Cursors.Hand,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(3),
                    BackColor = Color.LightGray
                };
                panel.SetColumnSpan(box, 2);
                panel.Controls.Add(box);
                ToolTip popUp = new ToolTip();
                popUp.SetToolTip(box, "Click to view details");
                box.Click += (s, e) => { MyForm.InsertProductInfoPanel(GetInfoPanel()); };
            }



            panel.Controls.Add(CreateProductPanelLabel(Name, ContentAlignment.TopLeft));

            panel.Controls.Add(CreateProductPanelLabel("$" + Price, ContentAlignment.TopRight));

            return panel;
        }
        // Get controls-method - Product's Panel.
        private Label CreateProductPanelLabel(string text, ContentAlignment align)
        {
            return new Label()
            {
                Font = new Font("Arial", 10, FontStyle.Bold),
                Text = text,
                TextAlign = align,
                Dock = DockStyle.Fill,
                ForeColor = Color.White
            };
        }



        // Get product's Info panel and funcionality.
        public FlowLayoutPanel GetInfoPanel()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoScroll = true,
                BackgroundImage = Image.FromFile(@"resources\backgrounds\secondWindow.jpg"),
                BackgroundImageLayout = ImageLayout.Stretch
            };



            // In case image is missing or direcory is writen wrong.
            try
            {
                var picBox = new PictureBox()
                {
                    Image = Image.FromFile(ImageLocation),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent,
                    Width = 400,
                    Height = 300
                };
                panel.Controls.Add(picBox);
            }
            catch
            {
                var picBox = new PictureBox()
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent,
                    Width = 400,
                    Height = 300
                };
                panel.Controls.Add(picBox);
            }

            var table = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 4,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            panel.Controls.Add(table);

            Label title = CreateInfoPanelLabel(Name, 20, ContentAlignment.MiddleLeft);
            table.Controls.Add(title);
            table.SetColumnSpan(title, 2);

            Label description = CreateInfoPanelLabel(Description, 17, ContentAlignment.TopLeft);
            table.Controls.Add(description);
            table.SetColumnSpan(description, 2);

            Label price = CreateInfoPanelLabel("Price: $" + Price, 17, ContentAlignment.MiddleRight);
            table.Controls.Add(price);
            table.SetColumnSpan(price, 2);

            Button back = CreateInfoPanelButton("Back");
            back.Click += (s, e) => { MyForm.InsertBayPanel();};
            table.Controls.Add(back);

            Button addToCart = CreateInfoPanelButton("Add to cart");
            addToCart.Click += (s, e) => { Cart.AddProduct(this); };
            table.Controls.Add(addToCart);

            return panel;
        }

        // Get controls-methods - Info Panel.
        private Label CreateInfoPanelLabel(string text, int size, ContentAlignment align)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Arial", size),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = align,
                ForeColor = Color.White
            };
        }

        private Button CreateInfoPanelButton(string text)
        {
            return new Button()
            {
                Text = text,
                Size = new Size(200, 50)
            };
        }


    }
}
