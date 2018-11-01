using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Media;


namespace Butik
{
    class MyForm : Form
    {
        private static TableLayoutPanel mainPanel;
        private static FlowLayoutPanel bayPanel;
        private static FlowLayoutPanel productPanel;
        private static Button musicONOFF;
        private static SoundPlayer WannaRock = new SoundPlayer { SoundLocation = @"resources\WR.wav" };

        private static bool MusicON = true;
        private static string[] stringProducts = File.ReadAllLines("Trucks.csv");
        private static Product[] products = new Product[stringProducts.Length];

        public MyForm()
        {

            var count = 0;
            var errorLines = "";
            for (var i = 0; i < stringProducts.Length; i++)
            {
                string[] p = stringProducts[i].Split(';');

                if (p.Length < 4)
                {
                    errorLines += " . " + (i + 1);
                    Array.Resize(ref products, products.Length - 1);
                }
                else
                {
                    products[count] = new Product(p[0], int.Parse(p[1]), p[2], p[3]);

                    count++;
                }
            }
            if (errorLines != "")
            {
                MessageBox.Show("\tWarning! " +
                                "\nInformation missing in products file, " +
                                "Line/Lines: " + errorLines);
            }

            WannaRock.PlayLooping();
            MinimumSize = new Size(585, 310);
            Width = 1240;
            Height = 700;
            Text = "The Monsters Bay";
            Icon = new Icon("resources/icon.ico");
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(5);

            mainPanel = new TableLayoutPanel()
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
            Controls.Add(mainPanel);

            ComboBox sort = new ComboBox()
            {
                Anchor = AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            sort.Items.Add("Sort by:");
            sort.Items.Add("Price - ascending");
            sort.Items.Add("Price - descending");
            sort.Items.Add("Name - ascending");
            sort.Items.Add("Name - descending");
            sort.SelectedIndex = 0;
            sort.SelectedIndexChanged += SortProducts;
            mainPanel.Controls.Add(sort);

            musicONOFF = new Button { Text = "Music OFF", Anchor = AnchorStyles.Right };
            mainPanel.Controls.Add(musicONOFF);
            musicONOFF.Click += MusicONOFFClick;

            Label titleLabel = CreateLabel("Welcome to the Monsters Bay", 18, ContentAlignment.MiddleLeft);
            mainPanel.Controls.Add(titleLabel);

            Label cartLabel = CreateLabel("Shopping cart", 14, ContentAlignment.MiddleCenter);
            mainPanel.Controls.Add(cartLabel, 1, 1);

            bayPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                AutoSize = true,
                BackgroundImage = Image.FromFile(@"resources\backgrounds\2001.png"),
                BackgroundImageLayout = ImageLayout.Stretch
            };
            mainPanel.Controls.Add(bayPanel, 0, 2);
            foreach (var p in products)
            {
                bayPanel.Controls.Add(p.GetProductPanel());
            }

            // Creating cart using Cart Class.
            var cartPanel = new Cart();
            mainPanel.Controls.Add(cartPanel, 1, 2);

            FormClosing += MyFormClosing;
        }

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

        // Applied in Product Class EventHandler.
        public static void InsertProductInfoPanel(FlowLayoutPanel panel)
        {
            productPanel = panel;
            bayPanel.Hide();
            mainPanel.Controls.Add(productPanel, 0, 2);
        }
        // Applied in Product Class EventHandler. 
        public static void InsertBayPanel()
        {
            mainPanel.Controls.Add(bayPanel);
            productPanel.Hide();
            bayPanel.Show();
        }

        private void SortProducts(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            Product[] sortedArray = new Product[] { };
            if (c.SelectedIndex == 0)
            { return; }
            else if (c.SelectedIndex == 1)
            {
                sortedArray = products.OrderBy(p => p.Price).ToArray();
            }
            else if (c.SelectedIndex == 2)
            {
                sortedArray = products.OrderByDescending(p => p.Price).ToArray();
            }
            else if (c.SelectedIndex == 3)
            {
                sortedArray = products.OrderBy(p => p.Name).ToArray();
            }
            else
            {
                sortedArray = products.OrderByDescending(p => p.Name).ToArray();
            }

            bayPanel.Controls.Clear();
            foreach (var p in sortedArray)
            {
                bayPanel.Controls.Add(p.GetProductPanel());
            }
        }

        private void MusicONOFFClick(object sender, EventArgs e)
        {
            var b = sender as Button;
            if (MusicON)
            {
                WannaRock.Stop();
                MusicON = false;
                b.Text = "Music ON";
            }
            else
            {
                WannaRock.PlayLooping();
                MusicON = true;
                b.Text = "Music OFF";
            }
        }

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
