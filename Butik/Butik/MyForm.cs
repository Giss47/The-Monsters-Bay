using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Media;


namespace Butik
{
    class MyForm : Form
    {
        private TableLayoutPanel mainPanel;
        private FlowLayoutPanel availableProductsPanel;
        private FlowLayoutPanel productDetailsPanel;
        private Button musicONOFF;
        private SoundPlayer iWannaRock;
        private Data data;
        private Cart cart;

        private bool musicON = true;

        public MyForm()
        {
            MinimumSize = new Size(585, 310);
            Width = 1300;
            Height = 700;
            Text = "The Monsters Bay";
            Icon = new Icon("resources/icon.ico");
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(5);            
            
            iWannaRock = new SoundPlayer { SoundLocation = @"resources\WR.wav" };
            iWannaRock.PlayLooping();            

            mainPanel = GetMainPanel();
            Controls.Add(mainPanel);

            FormClosing += MyFormClosing;
        }        

        private TableLayoutPanel GetMainPanel()
        {
            var mainPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));            

            ComboBox sort = GetSortBox();
            mainPanel.Controls.Add(sort);

            musicONOFF = new Button { Text = "Music OFF", Anchor = AnchorStyles.Right };
            mainPanel.Controls.Add(musicONOFF);
            musicONOFF.Click += MusicONOFFClick;

            Label titleLabel = CreateLabel("Welcome to the Monsters Bay", 18, ContentAlignment.MiddleLeft);
            mainPanel.Controls.Add(titleLabel);

            Label cartLabel = CreateLabel("Shopping cart", 14, ContentAlignment.MiddleCenter);
            mainPanel.Controls.Add(cartLabel, 1, 1);

            data = new Data();

            availableProductsPanel = GetAvailableProductsPanel();
            mainPanel.Controls.Add(availableProductsPanel, 0, 2);
            
            cart = new Cart(data);
            mainPanel.Controls.Add(cart, 1, 2);

            return mainPanel;
        }
        
        // -------- MainPanel Components ---------//
        private Label CreateLabel(string text, int size, ContentAlignment align)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Arial", size),
                TextAlign = align,
                Dock = DockStyle.Fill
            };
        }

        private ComboBox GetSortBox()
        {
            var sort = new ComboBox()
            {
                Anchor = AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            sort.Items.Add("Sort Products by:");
            sort.Items.Add("Price - ascending");
            sort.Items.Add("Price - descending");
            sort.Items.Add("Name A - Z");
            sort.Items.Add("Name Z - A");
            sort.SelectedIndex = 0;
            sort.SelectedIndexChanged += SortProducts;
            return sort;
        }

        private void SortProducts(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            Product[] sortedArray = new Product[] { };
            if (c.SelectedIndex == 0)
            { return; }
            else if (c.SelectedIndex == 1)
            {
                sortedArray = data.Products.OrderBy(p => p.Price).ToArray();
            }
            else if (c.SelectedIndex == 2)
            {
                sortedArray = data.Products.OrderByDescending(p => p.Price).ToArray();
            }
            else if (c.SelectedIndex == 3)
            {
                sortedArray = data.Products.OrderBy(p => p.Name).ToArray();
            }
            else
            {
                sortedArray = data.Products.OrderByDescending(p => p.Name).ToArray();
            }

            availableProductsPanel.Controls.Clear();
            foreach (var p in sortedArray)
            {
                availableProductsPanel.Controls.Add(ProductBox(p));
            }
        }

        private void MusicONOFFClick(object sender, EventArgs e)
        {
            var b = sender as Button;
            if (musicON)
            {
                iWannaRock.Stop();
                musicON = false;
                b.Text = "Music ON";
            }
            else
            {
                iWannaRock.PlayLooping();
                musicON = true;
                b.Text = "Music OFF";
            }
        }

                    // ------ AvailableProducts components ------- //

        private FlowLayoutPanel GetAvailableProductsPanel()
        {
             var availableProductsPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                AutoSize = true,
                BackgroundImage = Image.FromFile(@"resources\backgrounds\2001.png"),
                BackgroundImageLayout = ImageLayout.Stretch
            };
            foreach (var p in data.Products)
            {
                availableProductsPanel.Controls.Add(ProductBox(p));
            }

            return availableProductsPanel;
        }

        private TableLayoutPanel ProductBox(Product p)
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

            box.Click += (s, e) => { ShowProductDetailsPanel(GetProductDetailsPanel(p)); };
            try
            {
                box.Image = Image.FromFile(p.ImageLocation);
                box.MouseHover += (s, e) => { box.BackColor = Color.Red; };
                box.MouseLeave += (s, e) => { box.BackColor = Color.LightGray; };
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"Image not found for {p.Name} \nCheck products file or image directory");
            }

            panel.Controls.Add(ProductBoxLabel(p.Name, ContentAlignment.TopLeft));

            panel.Controls.Add(ProductBoxLabel("$" + p.Price, ContentAlignment.TopRight));

            return panel;
        }

        private Label ProductBoxLabel(string text, ContentAlignment align)
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

        private void ShowAvailableProductsPanel()
        {            
            productDetailsPanel.Hide();
            availableProductsPanel.Show();
        }

                    // ------ ProductDetails components ------- //

        private FlowLayoutPanel GetProductDetailsPanel(Product p)
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoScroll = true,
                BackgroundImage = Image.FromFile(@"resources\backgrounds\secondWindow.jpg"),
                BackgroundImageLayout = ImageLayout.Stretch
            };

            var picBox = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Width = 400,
                Height = 300
            };
            panel.Controls.Add(picBox);

            // In case image is missing or direcory is writen wrong.
            try
            {
                picBox.Image = Image.FromFile(p.ImageLocation);
            }
            catch
            {
                picBox.BackColor = Color.LightGray;
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

            Label title = ProductDetailsPanelLabel(p.Name, 20, ContentAlignment.MiddleLeft);
            table.Controls.Add(title);
            table.SetColumnSpan(title, 2);

            Label description = ProductDetailsPanelLabel(p.Description, 17, ContentAlignment.TopLeft);
            table.Controls.Add(description);
            table.SetColumnSpan(description, 2);

            Label price = ProductDetailsPanelLabel("Price: $" + p.Price, 17, ContentAlignment.MiddleRight);
            table.Controls.Add(price);
            table.SetColumnSpan(price, 2);

            Button back = ProductDetailsPanelButton("Back");
            back.Click += (s, e) =>
            {
                ShowAvailableProductsPanel();
            };
            table.Controls.Add(back);

            Button addToCart = ProductDetailsPanelButton("Add to cart");
            addToCart.Click += (s, e) =>
            {
                cart.AddProduct(p);
            };
            table.Controls.Add(addToCart);

            return panel;
        }

        private Label ProductDetailsPanelLabel(string text, int size, ContentAlignment align)
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

        private Button ProductDetailsPanelButton(string text)
        {
            return new Button()
            {
                Text = text,
                Size = new Size(200, 50)
            };
        }

        private void ShowProductDetailsPanel(FlowLayoutPanel panel)
        {
            productDetailsPanel = panel;
            availableProductsPanel.Hide();
            mainPanel.Controls.Add(productDetailsPanel, 0, 2);
        }

        //-------//

        private void MyFormClosing(object sender, FormClosingEventArgs e)
        {
            var message = "Are you sure  you would like to exit The Monsters Bay?";
            var caption = "Exit";
            DialogResult result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }    
    }
}
