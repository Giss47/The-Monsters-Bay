﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Media;


namespace Butik
{
    class MyForm : Form
    {
        static TableLayoutPanel mainPanel;
        static FlowLayoutPanel bayPanel;
        static FlowLayoutPanel productPanel;
        static bool MusicON = true;
        private Button musicONOFF;
        static SoundPlayer WannaRock = new SoundPlayer
        {
            SoundLocation = @"resources\1.wav"
        };

        private static string[] stringProducts = File.ReadAllLines("Trucks.csv");
        private Product[] products = new Product[stringProducts.Length];

        public MyForm()
        {
            WannaRock.PlayLooping();


            for (int i = 0; i < products.Length; i++)
            {
                string[] p = stringProducts[i].Split(';');
                products[i] = new Product(p[0], int.Parse(p[1]), p[2], p[3]);
            }

            MinimumSize = new Size(585, 310);
            Width = 1330;
            Height = 700;
            Text = "The Monsters Bay";
            Icon = new Icon("resources/icon.ico");
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(5);
            BackColor = Color.LightGray;
            
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

            Label titleLabel = new Label()
            {
                Font = new Font("Arial", 18),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Welcome to \"The Monsters Bay\"",
                Dock = DockStyle.Fill
            };
            mainPanel.SetRowSpan(titleLabel, 2);
            mainPanel.Controls.Add(titleLabel);


            musicONOFF = new Button { Text = "Music OFF",  Anchor = AnchorStyles.Right };
            mainPanel.Controls.Add(musicONOFF);           
            musicONOFF.Click += MusicONOFFClick;

            Label cartLabel = new Label()
            {
                Font = new Font("Arial", 14),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "The Shopping Cart",
                Dock = DockStyle.Fill
            };
            mainPanel.SetCellPosition(cartLabel, new TableLayoutPanelCellPosition(1, 1));
            mainPanel.Controls.Add(cartLabel);

            bayPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                AutoSize = true,
                BackgroundImage = Image.FromFile(@"resources\backgrounds\2001.png"),
                BackgroundImageLayout = ImageLayout.Stretch
            };
            mainPanel.SetCellPosition(bayPanel, new TableLayoutPanelCellPosition(0, 2));
            mainPanel.Controls.Add(bayPanel);
            foreach (Product p in products)
            {
                bayPanel.Controls.Add(p.GetPictureBox());
            }

            TableLayoutPanel cartPanel = new Cart();
            mainPanel.SetCellPosition(cartPanel, new TableLayoutPanelCellPosition(1, 2));
            mainPanel.Controls.Add(cartPanel);
            
            FormClosing += MyForm_FormClosing;
        }

        private void MusicONOFFClick(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if(MusicON)
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

        public static void InsertProductPanel(FlowLayoutPanel panel)
        {
            productPanel = panel;
            bayPanel.Hide();
            mainPanel.SetCellPosition(productPanel, new TableLayoutPanelCellPosition(0,2));
            mainPanel.Controls.Add(productPanel);
        }

        public static void InsertBayPanel()
        {
            productPanel.Hide();
            bayPanel.Show();
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "Are you sure  you would like to exit The Monsters Bay?";
            string caption = "Exit";
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
