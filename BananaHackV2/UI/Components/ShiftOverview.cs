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
    }
}
