using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BananaHackV2.UI.Components
{
    public partial class ShiftOverview : UserControl
    {
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            tstb_Month.Font = new Font(this.Font, FontStyle.Bold);
            tstb_Month.ReadOnly = true;
        }


        public ShiftOverview()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int m = monthPanel1.Month;
            if (m - 1 < 1)
            {
                monthPanel1.Year--;
                monthPanel1.Month = 12;
            }
            else {
                monthPanel1.Month--;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int m = monthPanel1.Month;
            if (m + 1 > 12)
            {
                monthPanel1.Year++;
                monthPanel1.Month = 1;
            }
            else
            {
                monthPanel1.Month++;
            }
        }
    }
}
