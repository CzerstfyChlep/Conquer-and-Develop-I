using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conquer_and_Develop
{
    public partial class TechnologyWindow : Form
    {
        public TechnologyWindow(int number)
        {
            InitializeComponent();
            TechPointsLabel.Text = "TechPoints: " + Form1.TechnologyPoints;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            foreach(Unit u in Form1.Units)
            {
                u.SetUnitData();
            }
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "Pikemen":
                    if (Form1.TechnologyPoints >= Form1.Upgrades[(Form1.Number - 1) * 30])
                        MinDamageButton.Enabled = true;
                    MinDamageLabel.Text = "16 + " + (Form1.Upgrades[(Form1.Number - 1) * 30] * 2);
                    MinDamageBar.Value = Form1.Upgrades[(Form1.Number - 1) * 30];
                    if (MinDamageBar.Value == MinDamageBar.Maximum)
                        MinDamageButton.Enabled = false;

                    break;
            }
        }
    }
}
