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
    public partial class AttackWindow : Form
    {
        public AttackWindow(Unit attacker, Unit defender)
        {
            InitializeComponent();
            Defender = defender;
            Attacker = attacker;
            TypeY.Text = "Type: " + Attacker.type;
            TypeE.Text = "Type: " + Defender.type;
            HPY.Text = "HP: " + Attacker.hp + "/100";
            HPE.Text = "HP: " + Defender.hp + "/100";
            ExpY.Text = "Expirence: " + Attacker.level + "/500";
            ExpE.Text = "Expirence: " + Defender.level + "/500";
            AttackY.Text = $"Attack: {Attacker.min_attack} - {attacker.max_attack}";
            AttackE.Text = $"Attack: {Defender.min_attack} - {Defender.max_attack}";
            m_defenceY.Text = "Melee Defence: " + Attacker.m_defence;
            m_defenceY.Text = "Melee Defence: " + Defender.m_defence;
            rdefY.Text = "Range Defence: " + Attacker.r_defence;
            rdefE.Text = "Range Defence: " + Defender.r_defence;
            dodgeY.Text = "Dodge: " + Attacker.dodge;
            dodgeE.Text = "Dodge: " + Defender.dodge;
        }
        public Unit Defender;
        public Unit Attacker;

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AttackButton_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int attackA = rand.Next(Attacker.min_attack, Attacker.max_attack);
            int attackD = rand.Next(Defender.min_attack, Defender.max_attack);
            attackA += (int)(Attacker.level / 100);
            attackD += (int)(Defender.level / 100);
            int bonusdamageA = 0;
            int bonusdamageD = 0;
            if (Attacker.type == "pike" && (Defender.type == "cav" || Defender.type == "lcav"))
                bonusdamageA = 20;
            if (Defender.type == "pike" && (Attacker.type == "cav" && Attacker.type == "lcav"))
                bonusdamageD = 20;
            int defenderloses = 0;
            int attackerloses = 0;
            if (Attacker.type != "arch" && Attacker.type != "wam")
            {
                if (rand.Next(1, 101) > Defender.dodge)
                {
                    
                        if ((Defender.m_defence + (Defender.position.Fortifications * 2)) - (attackA + bonusdamageA) > 5)
                        {
                            Defender.hp -= (attackA + bonusdamageA) - (Defender.m_defence + Defender.position.Fortifications * 2);
                            defenderloses = (attackA + bonusdamageA) - (Defender.m_defence + Defender.position.Fortifications * 2);
                        }
                        else
                        {
                            Defender.hp -= 5;
                            defenderloses = 5;
                        }
                    
                }
                Attacker.moves = 0;
                if (Defender.type != "arch" && Defender.type != "wam")
                {
                    if (rand.Next(1, 101) > Attacker.dodge)
                    {
                        if ((Attacker.m_defence + (Attacker.position.Fortifications * 2))- (attackD + bonusdamageD) > 5)
                        {
                            Attacker.hp -= (attackD + bonusdamageD)- (Attacker.m_defence +(Attacker.position.Fortifications * 2));
                            attackerloses = (attackD + bonusdamageD) - (Attacker.m_defence + (Attacker.position.Fortifications * 2));
                        }
                        else
                        {
                            Attacker.hp -= 5;
                            attackerloses = 5;
                        }
                    }
                }
                else
                {
                    if (rand.Next(1, 101) > Attacker.dodge)
                    {
                        if ((Attacker.r_defence + (Attacker.position.Fortifications * 2)) - attackD > 5)
                        {
                            Attacker.hp -= attackD - (Attacker.r_defence + (Attacker.position.Fortifications * 2));
                            attackerloses = attackD - (Attacker.r_defence + (Attacker.position.Fortifications * 2));
                        }
                        else
                        {
                            Attacker.hp -= 5;
                            attackerloses = 5;
                        }
                    }
                }
            }
            else
            {
                if (rand.Next(1, 101) > Defender.dodge)
                {
                    if ((Defender.r_defence + Defender.position.Fortifications * 2) - attackA > 5)
                    {

                        Defender.hp -= attackA - (Defender.r_defence + Defender.position.Fortifications * 2);
                        defenderloses = attackA - (Defender.r_defence + Defender.position.Fortifications * 2);
                    }
                    else
                    {
                        Defender.hp -= 5;
                        defenderloses = 5;
                    }
                    Attacker.moves = 0;
                    attackerloses = 0;
                }
            }
            Attacker.moves = 0;
            if (Attacker.hp <= 0)
                Attacker.Kill();
            if (Defender.hp <= 0)
                Defender.Kill();

            MessageBox.Show($"Your loses: {attackerloses} HP\nEnemy loses: {defenderloses} HP", "Battle", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
