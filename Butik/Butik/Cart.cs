﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Butik
{
    class Cart : TableLayoutPanel
    {
        public Cart()
        {
            Dock = DockStyle.Fill;
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }
    }
}
