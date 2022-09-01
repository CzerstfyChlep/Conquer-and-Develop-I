using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Media;
using System.Web;


namespace Conquer_and_Develop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            Login l = new Login();
            l.ShowDialog();
            Form.CheckForIllegalCrossThreadCalls = false;
            this.AcceptButton = SendMessageButton;
            #region tooltips
            ToolTip tt1 = new ToolTip();
            tt1.SetToolTip(PaymentLabel, "HERE INSERT ALL PAYMENTS!");
            tt1.AutoPopDelay = 10000;
            tt1.InitialDelay = 500;

            #endregion
        }
        public void Connect()
        {
            if (IP == "EXIT")
            {
                foreach (Form f in OwnedForms)
                    f.Close();
                this.Close();
            }
            string Version = "2.2.0.0";
            TCPClient.Connect(IP, 2055);
            s = TCPClient.GetStream();
            sr = new StreamReader(s);
            sw = new StreamWriter(s);
            sw.AutoFlush = true;
            int id = 0;
            for (int a = 0; a < 20; a++)
            {
                for (int b = 0; b < 20; b++)
                {
                    Province p = new Province(new Rectangle((a * 23) + 4, (b * 23) + 4, 23, 23));
                    Provinces.Add(p);
                    p.ID = id;
                    id++;
                }
            }
            t = new System.Windows.Forms.Timer();
            SystemSounds.Beep.Play();
            this.Text = "Conquer & Develop - #" + Number;
            t.Interval = (500);
            t.Tick += new EventHandler(timertick);
            sw.WriteLine(Version);

            string result = sr.ReadLine();
            if (result == "right")
            {
                sw.WriteLine("false");
                sw.WriteLine(Number);
                NumberOfPlayers = int.Parse(sr.ReadLine());
                sw.WriteLine(nick);
            }
            else
            {
                MessageBox.Show("Wersja gry, której używasz jest nieaktualna!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                s.Close();
                TCPClient.Close();
                Application.Exit();
            }
            foreach (Province p in Provinces)
            {
                p.TradeValue = int.Parse(sr.ReadLine());
            }
            if (Number != 1)
            {
                t.Start();
                NextTurnButton.Enabled = false;
                RecruitButton.Enabled = false;
                UpgradeDevButton.Enabled = false;
                UpgradeFortsButton.Enabled = false;
                TrainButton.Enabled = false;
                ReinforceButton.Enabled = false;
                TaxToManpowerBar.Enabled = false;
                Turn = false;
                Gold--;
            }
            
            th = new Thread(ChatUpdater);
            th.Start();
        }
        public void ChatUpdater()
        {
            ChatBox.Font = new Font("Consolas", 8f);
            tc = new TcpClient();
            tc.Connect(IP, 2055);
            Stream st = tc.GetStream();
            StreamReader sre = new StreamReader(st);
            StreamWriter swr = new StreamWriter(st);
            swr.AutoFlush = true;
            swr.WriteLine("2.0.0.0");
            string v = sre.ReadLine();
            swr.WriteLine("true");
            ChatBox.SelectionColor = Col;
            ChatBox.AppendText("Welcome!");
            ChatBox.ReadOnly = true;
            ChatBox.SelectionColor = Color.Black;
            string oldtext = "";
            string newtext = "";
            while (true)
            {
                oldtext = newtext;
                swr.WriteLine("check");
                newtext = sre.ReadLine();
                if (newtext != oldtext)
                {
                    string[] texts = newtext.Split('~');
                    ChatBox.SelectionColor = Color.FromName(texts[0]);
                    ChatBox.AppendText(Environment.NewLine + texts[1]);
                    ChatBox.SelectionColor = Color.Black;
                    ChatBox.AppendText(texts[2]);
                    ChatBox.ScrollToCaret();
                    if (Form.ActiveForm == null)
                    {
                        SystemSounds.Beep.Play();
                    }

                }
                Thread.Sleep(5);
            }
        }
        private void timertick(object sender, EventArgs e)
        {
            sw.WriteLine("check");
            string response = sr.ReadLine();
            if (response == "true")
            {
                t.Stop();
                Gold += int.Parse(sr.ReadLine());
                Turn = true;
                NextTurnButton.Enabled = true;
                RecruitButton.Enabled = true;              
                TrainButton.Enabled = true;
                UnitTypeBox.Text = "";
                ReinforceButton.Enabled = true;
                TaxToManpowerBar.Enabled = true;

                GetData();
                sw.WriteLine("checktrade");
                for (int b = 0; b < 8; b++)
                {
                    string result = sr.ReadLine();
                    if (result == "trade")
                    {
                        if (!AI)
                        {
                            if (MessageBox.Show("Gracz " + (b + 1) + " chce z tobą handlować.\nTwój zysk: " + (int)Math.Ceiling((double)TradeValueSum[Number - 1] / (NumberOfPlayers - 1)) + "\nZysk gracza: " + (int)Math.Ceiling((double)TradeValueSum[b] / (NumberOfPlayers - 1)), "Oferta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Trade[Number - 1, b] = true;
                                Trade[b, Number - 1] = true;
                                sw.WriteLine("true");
                            }
                            else
                            {
                                sw.WriteLine("false");
                            }
                        }
                        else
                        {
                            sw.WriteLine("false");
                        }

                    }
                    else
                    {
                        sw.WriteLine("false");
                    }
                }
                CalculateIncome();
                Gold += GoldIncome;
                Manpower += ManpowerIncome;
                PowerPointsUpdate();
                TechnologyPoints += TechPointsIncome;
                UpdateMap();
                CalculateTradeSum();
                for (int a = 0; a < 8; a++)
                {
                    if (Trade[Number - 1, a] == true && a != Number - 1)
                    {
                        Gold += (int)Math.Ceiling((double)TradeValueSum[Number - 1] / (NumberOfPlayers - 1));

                    }
                }
                if (AI == true)
                {
                    AITurn();
                }
            }
            else if (response == "update")
            {
                t.Stop();
                GetData();
                /*UpdateMap()*/;
                CalculateIncome();
                CalculateTradeSum();
                t.Start();
            }
            else if (response == "false")
            {
                Gold += int.Parse(sr.ReadLine());
                PowerPointsUpdate();
            }

        }

        public static int TurnN = 1;
        public System.Windows.Forms.Timer t;
        public Stream s;
        public bool Turn = true;
        public static string nick;
        public StreamReader sr;
        public StreamWriter sw;
        public static Thread th;
        Graphics g;
        TcpClient TCPClient = new TcpClient();
        public static string IP;
        public int NumberOfPlayers = 0;
        public static int Number = 1;
        public static int Gold = 20;
        public static int Manpower = 30;
        public static double TechnologyPoints = 0;
        public static int TechPointsIncome = 2;
        public static int DefenceTechCost = 25;
        public static int DevTechCost = 25;
        public static int[] Upgrades = new int[240];
        public bool[,] Trade = new bool[8, 8];
        public int[] TradeValueSum = new int[8];
        public static bool AI = false;
        public static int GoldIncome = 0;
        public static int ManpowerIncome = 0;
        public static Color Col = Color.Black;
        public static TcpClient tc;
        public static List<Unit> Units = new List<Unit>();
        public static int Tax = 3;
        public static bool ArmyClicked = false;
        public static int UnitCount = 0;
        public Province PrevClicked;
        public static Province Capital;
        public static Unit SelectedUnit;
        public void CalculateTradeSum()
        {
            for (int a = 0; a < 8; a++)
            {
                TradeValueSum[a] = 0;
                foreach (Province prov in Provinces)
                {
                    if (prov.Owner == a + 1)
                        TradeValueSum[a] += prov.TradeValue;
                }
            }
        }
        public void CalculateIncome()
        {
            GoldIncome = 5;
            ManpowerIncome = 5;
            foreach (Province prov in Provinces)
            {
                if (prov.Owner == Number)
                {
                    GoldIncome += Tax;
                    ManpowerIncome += 6 - Tax;
                    GoldIncome += prov.Development - prov.Fortifications;
                }
            }
            foreach (Unit u in Units)
            {
                if (u.position.Owner == Number)
                {
                    GoldIncome -= u.Payment;
                    ManpowerIncome -= 2;
                }
            }
            for (int a = 0; a < 8; a++)
            {
                if (Trade[Number - 1, a] == true)
                    GoldIncome += (int)Math.Ceiling((double)TradeValueSum[Number - 1] / (NumberOfPlayers - 1));
            }
            IncomeLabel.Text = "GPT: " + GoldIncome;
            ManpowerIncomeLabel.Text = "Manpower income: " + ManpowerIncome;

        }
        public static List<Province> Provinces = new List<Province> { };
        public void PowerPointsUpdate()
        {
            label1.Text = "Gold: " + Gold;
            ManpowerLabel.Text = "Manpower: " + Manpower;
        }
        public void UpdateMap(Province prov = null)
        {
            if (prov == null)
            {
                foreach (Province pro in Provinces)
                {
                    switch (pro.Owner)
                    {
                        case -1:
                            g.FillRectangle(Brushes.Black, pro.Rect);
                            break;
                        case 0:
                            g.FillRectangle(Brushes.White, pro.Rect);
                            break;
                        case 1:
                            g.FillRectangle(Brushes.Blue, pro.Rect);
                            break;
                        case 2:
                            g.FillRectangle(Brushes.Red, pro.Rect);
                            break;
                        case 3:
                            g.FillRectangle(Brushes.Green, pro.Rect);
                            break;
                        case 4:
                            g.FillRectangle(Brushes.Orange, pro.Rect);
                            break;
                        case 5:
                            g.FillRectangle(Brushes.Purple, pro.Rect);
                            break;
                        case 6:
                            g.FillRectangle(Brushes.Pink, pro.Rect);
                            break;
                        case 7:
                            g.FillRectangle(Brushes.Navy, pro.Rect);
                            break;
                        case 8:
                            g.FillRectangle(Brushes.Maroon, pro.Rect);
                            break;
                    }
                    foreach (Unit u in Units)
                    {
                        if (u.position.ID == pro.ID && u != SelectedUnit)
                        {
                            g.FillRectangle(Brushes.Black, pro.ArmyRect);
                            string s = "";
                            switch (u.type)
                            {
                                case "pike":
                                    s = "P";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                    break;
                                case "inf":
                                    s = "I";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                    break;
                                case "lcav":
                                    s = "L";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                    break;
                                case "cav":
                                    s = "C";

                                    g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                    break;
                                case "arch":
                                    s = "A";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                    break;
                                case "wam":
                                    s = "M";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                    break;
                            }
                        }
                           
                        else if (u.position.ID == pro.ID && u == SelectedUnit)
                        {
                            g.FillRectangle(Brushes.White, pro.ArmyRect);
                            string s = "";
                            switch (u.type)
                            {
                                case "pike":
                                    s = "P";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                    break;
                                case "inf":
                                    s = "I";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                    break;
                                case "lcav":
                                    s = "L";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                    break;
                                case "cav":
                                    s = "c" ;
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                    break;
                                case "arch":
                                    s = "A" ;
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                    break;
                                case "wam":
                                    s = "M";
                                    g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                    break;
                            }
                        }
                    }
                    g.DrawRectangle(Pens.Black, pro.Rect);
                }
            }
            else
            {
                switch (prov.Owner)
                {
                    case -1:
                        g.FillRectangle(Brushes.Black, prov.Rect);
                        break;
                    case 0:
                        g.FillRectangle(Brushes.White, prov.Rect);
                        break;
                    case 1:
                        g.FillRectangle(Brushes.Blue, prov.Rect);
                        break;
                    case 2:
                        g.FillRectangle(Brushes.Red, prov.Rect);
                        break;
                    case 3:
                        g.FillRectangle(Brushes.Green, prov.Rect);
                        break;
                    case 4:
                        g.FillRectangle(Brushes.Orange, prov.Rect);
                        break;
                    case 5:
                        g.FillRectangle(Brushes.Purple, prov.Rect);
                        break;
                    case 6:
                        g.FillRectangle(Brushes.Pink, prov.Rect);
                        break;
                    case 7:
                        g.FillRectangle(Brushes.Navy, prov.Rect);
                        break;
                    case 8:
                        g.FillRectangle(Brushes.Maroon, prov.Rect);
                        break;
                }
                switch (prov.Fortifications)
                {
                    case 1:
                        g.DrawLine(Pens.Black, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        break;
                    case 2:
                        g.DrawLine(Pens.Black, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Black, prov.Rect.X + 20, prov.Rect.Y, prov.Rect.X, prov.Rect.Y + 20);
                        break;
                    case 3:
                        g.DrawLine(Pens.Black, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Black, prov.Rect.X + 20, prov.Rect.Y, prov.Rect.X, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Black, prov.Rect.X + 10, prov.Rect.Y, prov.Rect.X + 10, prov.Rect.Y + 20);
                        break;
                    case 4:
                        g.DrawLine(Pens.Black, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Black, prov.Rect.X + 20, prov.Rect.Y, prov.Rect.X, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Black, prov.Rect.X + 10, prov.Rect.Y, prov.Rect.X + 10, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Black, prov.Rect.X, prov.Rect.Y + 10, prov.Rect.X + 20, prov.Rect.Y + 10);
                        break;
                    case 5:
                        g.DrawLine(Pens.Gray, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        break;
                    case 6:
                        g.DrawLine(Pens.Gray, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Gray, prov.Rect.X + 20, prov.Rect.Y, prov.Rect.X, prov.Rect.Y + 20);
                        break;
                    case 7:
                        g.DrawLine(Pens.Gray, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Gray, prov.Rect.X + 20, prov.Rect.Y, prov.Rect.X, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Gray, prov.Rect.X + 10, prov.Rect.Y, prov.Rect.X + 10, prov.Rect.Y + 20);
                        break;
                    case 8:
                        g.DrawLine(Pens.Gray, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Gray, prov.Rect.X + 20, prov.Rect.Y, prov.Rect.X, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Gray, prov.Rect.X + 10, prov.Rect.Y, prov.Rect.X + 10, prov.Rect.Y + 20);
                        g.DrawLine(Pens.Gray, prov.Rect.X, prov.Rect.Y + 10, prov.Rect.X + 20, prov.Rect.Y + 10);
                        break;
                    case 9:
                        g.DrawLine(Pens.White, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        break;
                    case 10:
                        g.DrawLine(Pens.White, prov.Rect.X, prov.Rect.Y, prov.Rect.X + 20, prov.Rect.Y + 20);
                        g.DrawLine(Pens.White, prov.Rect.X + 20, prov.Rect.Y, prov.Rect.X, prov.Rect.Y + 20);
                        break;


                }
                foreach (Unit u in Units)
                {
                    if (u.position.ID == prov.ID && u != SelectedUnit)
                    {
                        g.FillRectangle(Brushes.Black, prov.ArmyRect);
                        string s = "";
                        switch (u.type)
                        {
                            case "pike":
                                s = "P";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                break;
                            case "inf":
                                s = "I";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                break;
                            case "lcav":
                                s = "L";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                break;
                            case "cav":
                                s = "C";

                                g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                break;
                            case "arch":
                                s = "A";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                break;
                            case "wam":
                                s = "M";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.White, u.position.ArmyRect);
                                break;
                        }
                    }

                    else if (u.position.ID == prov.ID && u == SelectedUnit)
                    {
                        g.FillRectangle(Brushes.White, prov.ArmyRect);
                        string s = "";
                        switch (u.type)
                        {
                            case "pike":
                                s = "P";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                break;
                            case "inf":
                                s = "I";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                break;
                            case "lcav":
                                s = "L";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                break;
                            case "cav":
                                s = "c";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                break;
                            case "arch":
                                s = "A";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                break;
                            case "wam":
                                s = "M";
                                g.DrawString(s, new Font("Consolas", 8), Brushes.Black, u.position.ArmyRect);
                                break;
                        }
                    }
                }
                g.DrawRectangle(Pens.Black, prov.Rect);
            }
        }
        public static Province GetByID(int id)
        {
            IEnumerable<Province> q = from p in Provinces
                                      where p.ID == id
                                      select p;
            return q.First();
        }
        public void SetData()
        {
            sw.WriteLine("set");
            foreach (Province prov in Provinces)
            {
                sw.WriteLine(prov.Owner);
            }
            foreach (Province prov in Provinces)
            {
                sw.WriteLine(prov.Development);
            }
            foreach (Province prov in Provinces)
            {
                sw.WriteLine(prov.Fortifications);
            }
           
            for (int a = 0; a < 8; a++)
            {
                for (int b = 0; b < 8; b++)
                {
                    sw.WriteLine(Trade[a, b]);
                }
            }
            sw.WriteLine(Units.Count());
            foreach (Unit u in Units)
            {
                sw.WriteLine(u.position.ID + "~" + u.hp + "~" + u.min_attack + "~" + u.max_attack + "~" + u.m_defence + "~" + u.r_defence + "~" + u.type + "~" + u.level + "~" + u.dodge);
            }
            for(int a = 0; a < 8; a++)
            {
                string text = "";
                for(int b = 0; b < 30; b++)
                {
                    text += Upgrades[b + (a * 30)] + "~";
                    
                }
                sw.WriteLine(text);
            }
            try
            {
                for (int a = 0; a < Units.Count(); a++)
                {
                    Units[a].Kill();
                }
            }
            catch
            {

            }
            SelectedUnit = null;
            PrevClicked = null;
            Units.Clear();

        }

        public void GetData()
        {
            sw.WriteLine("get");
            foreach (Province prov in Provinces)
            {
                prov.Owner = int.Parse(sr.ReadLine());
            }
            foreach (Province prov in Provinces)
            {
                prov.Development = int.Parse(sr.ReadLine());
            }
            foreach (Province prov in Provinces)
            {
                prov.Fortifications = int.Parse(sr.ReadLine());
            }
            for (int a = 0; a < 8; a++)
            {
                for (int b = 0; b < 8; b++)
                {
                    
                    Trade[a, b] = Boolean.Parse(sr.ReadLine());
                }
            }
            int unitsnumber = int.Parse(sr.ReadLine());
            try
            {
                for (int a = 0; a < unitsnumber; a++)
                {
                    Units[a].Kill();
                }
            }
            catch
            {

            }
            SelectedUnit = null;
            PrevClicked = null;
            Units.Clear();
            foreach(Province p in Provinces)
            {
                p.unit = null;
            }
            for (int a = 0; a < unitsnumber; a++)
            {
                string text = sr.ReadLine();
                string[] splited = text.Split('~');
                Unit u = new Unit(splited[6], GetByID(int.Parse(splited[0])));
                u.hp = int.Parse(splited[1]);              
                u.level = int.Parse(splited[7]);
                u.position.unit = u;
                u.SetUnitData();
                u.moves = u.maxmoves;
            }
            for (int a = 0; a < 8; a++)
            {
                string text = sr.ReadLine();
                string[] splited = text.Split('~');
                for(int b =0; b < 30; b++)
                {
                    Upgrades[b + (a * 30)] = int.Parse(splited[b]);
                }
             
            }
        }
        private void StartGameButton_Click(object sender, EventArgs e)
        {
            Connect();
            StartGameButton.Enabled = false;
            foreach (Province p in Provinces)
            {
                g.DrawRectangle(Pens.Black, p.Rect);
            }
            GetData();
            foreach (Province prov in Provinces)
            {
                if (prov.Owner == Number)
                {
                    Capital = prov;
                    Capital.Capital = true;
                }
            }
            UpdateMap();
            StartGameButton.Enabled = false;
        }
        public void AITurn()
        {

            foreach (Province pro in Provinces)
            {
                if (pro.Owner == Number)
                    goto after;
            }
            goto end;
            after:
            Random rand = new Random();
            List<Province> AttackTargets = new List<Province>();
            List<Province> FortifyTargets = new List<Province>();
            List<Province> DevelopTargets = new List<Province>();

            if (TurnN < 10)
            {
                AttackTargets.Clear();
                FortifyTargets.Clear();
                DevelopTargets.Clear();
                foreach (Province p in Provinces)
                {
                    if (p.Owner == Number)
                    {
                        Province[] arr = GetBordering(p);
                        foreach (Province prov in arr)
                        {
                            if (prov.Owner != Number && prov.Owner > -1)
                            {
                                AttackTargets.Add(prov);
                            }
                        }
                        if (p.Fortifications == 0)
                            DevelopTargets.Add(p);
                        if (p.Development == 0)
                            FortifyTargets.Add(p);
                    }
                }
                for (int a = 0; a < 2; a++)
                {
                    int n = rand.Next(0, AttackTargets.Count());
                    if (AttackTargets[n].Owner == 0)
                        AttackTargets[n].Owner = Number;
                    else
                    {
                        if (AttackTargets[n].Fortifications > 0)
                        {
                            AttackTargets[n].Fortifications -= 1;
                        }
                        else
                        {
                            AttackTargets[n].Owner = Number;
                        }
                    }
                }
                for (int a = 0; a < 1; a++)
                {
                    int n = rand.Next(0, FortifyTargets.Count());
                    if (FortifyTargets[n].Fortifications < 5)
                        FortifyTargets[n].Fortifications++;
                }
            }
            if (TurnN >= 10 && TurnN < 20)
            {
                AttackTargets.Clear();
                FortifyTargets.Clear();
                DevelopTargets.Clear();
                foreach (Province p in Provinces)
                {
                    if (p.Owner == Number)
                    {
                        Province[] arr = GetBordering(p);
                        foreach (Province prov in arr)
                        {
                            if (prov.Owner != Number && prov.Owner > -1)
                            {
                                AttackTargets.Add(prov);
                            }
                        }
                        if (p.Fortifications == 0)
                            DevelopTargets.Add(p);
                        if (p.Development == 0)
                            FortifyTargets.Add(p);
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    int n = rand.Next(0, AttackTargets.Count());
                    if (AttackTargets[n].Owner == 0)
                        AttackTargets[n].Owner = Number;
                    else
                    {
                        if (AttackTargets[n].Fortifications > 0)
                        {
                            AttackTargets[n].Fortifications -= 1;
                        }
                        else
                        {
                            AttackTargets[n].Owner = Number;
                        }
                    }
                }
                for (int a = 0; a < 2; a++)
                {
                    int n = rand.Next(0, FortifyTargets.Count());
                    if (FortifyTargets[n].Fortifications < 5)
                        FortifyTargets[n].Fortifications++;
                }
                for (int a = 0; a < 1; a++)
                {
                    int n = rand.Next(0, DevelopTargets.Count());
                    if (DevelopTargets[n].Development < 5)
                        DevelopTargets[n].Development++;
                }


            }
            if (TurnN >= 20)
            {
                AttackTargets.Clear();
                FortifyTargets.Clear();
                DevelopTargets.Clear();
                foreach (Province p in Provinces)
                {
                    if (p.Owner == Number)
                    {
                        Province[] arr = GetBordering(p);
                        foreach (Province prov in arr)
                        {
                            if (prov.Owner != Number && prov.Owner > -1)
                            {
                                AttackTargets.Add(prov);
                            }
                        }
                        if (p.Fortifications == 0)
                            DevelopTargets.Add(p);
                        if (p.Development == 0)
                            FortifyTargets.Add(p);
                    }
                }
                for (int a = 0; a < 6; a++)
                {
                    int n = rand.Next(0, AttackTargets.Count());
                    if (AttackTargets[n].Owner == 0)
                        AttackTargets[n].Owner = Number;
                    else
                    {
                        if (AttackTargets[n].Fortifications > 0)
                        {
                            AttackTargets[n].Fortifications -= 1;
                        }
                        else
                        {
                            AttackTargets[n].Owner = Number;
                        }
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    int n = rand.Next(0, FortifyTargets.Count());
                    if (FortifyTargets[n].Fortifications < 5)
                        FortifyTargets[n].Fortifications++;
                }
                for (int a = 0; a < 3; a++)
                {
                    int n = rand.Next(0, DevelopTargets.Count());
                    if (DevelopTargets[n].Development < 5)
                        DevelopTargets[n].Development++;
                }


            }
            end:
            NextTurn();
        }
        public void TradeUpdateMap()
        {
            foreach (Province p in Provinces)
            {
                switch (p.TradeValue)
                {
                    case 0:
                        g.FillRectangle(Brushes.Gray, p.Rect);
                        break;
                    case 1:
                        g.FillRectangle(Brushes.Yellow, p.Rect);
                        break;
                    case 2:
                        g.FillRectangle(Brushes.Green, p.Rect);
                        break;
                }
                g.DrawRectangle(Pens.Black, p.Rect);
            }
        }
        public Province[] GetBordering(Province p)
        {
            List<Province> pr = new List<Province>();
            Point Left = new Point(p.Rect.X - 10, p.Rect.Y + 1);
            Point Right = new Point(p.Rect.X + 30, p.Rect.Y + 1);
            Point Up = new Point(p.Rect.X + 1, p.Rect.Y - 10);
            Point Down = new Point(p.Rect.X + 1, p.Rect.Y + 30);
            foreach (Province g in Provinces)
            {
                if (g.Rect.Contains(Left) || g.Rect.Contains(Right) || g.Rect.Contains(Down) || g.Rect.Contains(Up))
                {
                    pr.Add(g);
                }
            }
            return pr.ToArray();
        }
        public void ProvinceTableUpdate(Province province)
        {
            if (province.Owner == Number)
            {
                ProvinceIDLabel.Text = "Province: " + province.ID;
                ProvinceDevelopmentLabel.Text = $"Development: {province.Development}";
                ProvinceFortificationsLabel.Text = $"Fortifications: {province.Fortifications}";
                UpgradeDevButton.Text = $"-{(province.Development + province.Fortifications + 1) * 3}";
                UpgradeFortsButton.Text = $"-{(province.Fortifications + province.Development + 1) * 3}";
                if (Gold >= (province.Development + 1 + province.Fortifications) * 3)
                    UpgradeDevButton.Enabled = true;
                else
                    UpgradeDevButton.Enabled = false;
                if (Gold >= (province.Fortifications + 1 + province.Development) * 3)
                    UpgradeFortsButton.Enabled = true;
                else
                    UpgradeFortsButton.Enabled = false;
            }
            else if (province.Owner != Number && province.Owner > 0)
            {
                ProvinceIDLabel.Text = "Province: " + province.ID;
                ProvinceDevelopmentLabel.Text = $"Development: {province.Development}";
                ProvinceFortificationsLabel.Text = $"Fortifications: {province.Fortifications}";
                UpgradeDevButton.Text = "Locked";
                UpgradeFortsButton.Text = "Locked";
                UpgradeDevButton.Enabled = false;
                UpgradeFortsButton.Enabled = false;
            }
        }
        public void UnitPageUpdate(Unit u = null)
        {
            if (SelectedUnit != null)
            {
                UnitTypeLabel.Text = "Type: " + SelectedUnit.type;
                LevelLabel.Text = "Experience: " + SelectedUnit.level + "/500";
                HPLabel.Text = $"HP: {SelectedUnit.hp}/100";
                AttackLabel.Text = $"Attack: {SelectedUnit.min_attack} - {SelectedUnit.max_attack}";
                MDefenceLabel.Text = "Melee Defence: " + SelectedUnit.m_defence;
                RDefenceLabel.Text = "Range Defence: " + SelectedUnit.r_defence;
                MovesLabel.Text = "Moves: " + SelectedUnit.moves;
                DodgeLabel.Text = "Dodge: " + SelectedUnit.dodge + "%";
                
            }
            else
            {
                UnitTypeLabel.Text = "Type: No unit";
                LevelLabel.Text = "Experience: No unit";
                HPLabel.Text = $"HP: No unit";
                AttackLabel.Text = $"";
            }
        }
        public void ProvinceClick(object sender, MouseEventArgs e)
        {
            foreach (Province province in Provinces)
            {
                if (province.Rect.Contains(e.Location))
                {
                    if (Turn)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            if (province.unit == null)
                            {
                                SelectedUnit = null;
                                UpdateMap(province);
                                if (PrevClicked != null)
                                    UpdateMap(PrevClicked);
                                PrevClicked = province;
                                ProvinceTableUpdate(province);
                            }
                            else if (province.unit != null)
                            {
                                if (province.ArmyRect.Contains(e.Location))
                                {
                                    SelectedUnit = province.unit;
                                    PrevClicked = province;
                                    Tabs.SelectedTab = UnitPage;
                                    UnitPageUpdate(province.unit);
                                }
                                else
                                {
                                    ProvinceTableUpdate(province);
                                    UnitPageUpdate();
                                    SelectedUnit = null;
                                    UpdateMap(province);
                                    if (PrevClicked != null)
                                        UpdateMap(PrevClicked);
                                    PrevClicked = province;
                                }

                            }

                            if (PrevClicked != null)
                                UpdateMap(PrevClicked);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (SelectedUnit != null && SelectedUnit.position.Owner == Number && SelectedUnit.position != province)
                            {
                                bool b = false;
                                foreach (Province p in GetBordering(province))
                                {
                                    if (p == PrevClicked)
                                    {
                                        b = true;
                                        break;
                                    }
                                }
                                if (b)
                                {
                                    if (province.Owner == Number && province.unit == null && SelectedUnit.moves > 0)
                                    {
                                        SelectedUnit.moves--;
                                        SelectedUnit = null;
                                        PrevClicked.unit.position = province;
                                        province.unit = PrevClicked.unit;
                                        PrevClicked.unit = null;
                                        if (PrevClicked != null)
                                            UpdateMap(PrevClicked);
                                        PrevClicked = null;
                                        UpdateMap(province);


                                    }
                                    else if (province.Owner >= 0 && province.Owner != Number && province.unit == null && SelectedUnit.moves > 0)
                                    {
                                        SelectedUnit.moves--;
                                        SelectedUnit = null;
                                        PrevClicked.unit.position = province;
                                        province.unit = PrevClicked.unit;
                                        PrevClicked.unit = null;
                                        province.Owner = Number;
                                        if (PrevClicked != null)
                                            UpdateMap(PrevClicked);
                                        PrevClicked = null;
                                        UpdateMap(province);
                                    }
                                    else if (province.Owner >= 0 && province.Owner != Number && province.unit != null && SelectedUnit.moves > 0)
                                    {
                                        AttackWindow aw = new AttackWindow(SelectedUnit, province.unit);
                                        aw.ShowDialog();
                                        UnitPageUpdate();
                                    }
                                }

                                UpdateMap(province);
                                if (PrevClicked != null)
                                    UpdateMap(PrevClicked);
                            }
                        }
                    }
                    break;
                }
                else
                {

                }


            }
           

            
            CalculateTradeSum();
            PowerPointsUpdate();
            CalculateIncome();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                sw.WriteLine("exit");
                TCPClient.Close();
                tc.Close();
                th.Abort();
            }
            catch
            {

            }
            this.Close();
        }
        public void NextTurn()
        {
            SetData();
            sw.WriteLine("next turn");
            NextTurnButton.Enabled = false;
            RecruitButton.Enabled = false;
            UpgradeDevButton.Enabled = false;
            UpgradeFortsButton.Enabled = false;
            TrainButton.Enabled = false;
            ReinforceButton.Enabled = false;
            TaxToManpowerBar.Enabled = false;
            Turn = false;
            t.Start();
        }
        private void NextTurnButton_Click(object sender, EventArgs e)
        {
            NextTurn();
        }




        private void GivePointsButton_Click(object sender, EventArgs e)
        {
            if (Gold >= PowerPointsNumer.Value && PlayerNumeric.Value != Number && PowerPointsNumer.Value > 0)
            {
                Gold -= (int)PowerPointsNumer.Value;
                sw.WriteLine("give points");
                sw.WriteLine((int)PlayerNumeric.Value);
                sw.WriteLine((int)PowerPointsNumer.Value);
                PowerPointsUpdate();
            }
            else
            {
                MessageBox.Show("Nie masz wystarczająco punktów lub próbowałeś wysłać je do siebie.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TradeButton_Click(object sender, EventArgs e)
        {
            if (TradePlayerNumeric.Value != Number)
            {
                if (Trade[Number - 1, (int)TradePlayerNumeric.Value - 1] != true)
                {
                    if (MessageBox.Show("Chcesz handlować z graczem nr " + TradePlayerNumeric.Value + "\nTwój zysk: " + (int)Math.Ceiling((double)TradeValueSum[Number - 1] / (NumberOfPlayers - 1)) + "Zysk gracza: " + (int)Math.Ceiling((double)TradeValueSum[(int)TradePlayerNumeric.Value - 1] / (NumberOfPlayers - 1)), "Oferta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        sw.WriteLine("ask");
                        sw.WriteLine((int)TradePlayerNumeric.Value - 1);
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (MessageBox.Show("Chcesz przestać handlować z graczem nr " + TradePlayerNumeric.Value + "?\nTwój zysk: " + (int)Math.Ceiling((double)TradeValueSum[Number - 1] / (NumberOfPlayers - 1)) + "Zysk gracza: " + (int)Math.Ceiling((double)TradeValueSum[(int)TradePlayerNumeric.Value - 1] / (NumberOfPlayers - 1)), "Oferta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Trade[Number - 1, (int)TradePlayerNumeric.Value - 1] = false;
                        Trade[(int)TradePlayerNumeric.Value - 1, Number - 1] = false;
                    }
                    else
                    {

                    }
                }
            }
            else
                MessageBox.Show("Błąd!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void TradeListButton_Click(object sender, EventArgs e)
        {
            CalculateTradeSum();
            string TEXT = "Łączna wartość handlowa: " + TradeValueSum[Number - 1] + "\nLista:";

            for (int a = 0; a < 8; a++)
            {
                if (a != Number - 1)
                {
                    if (Trade[Number - 1, a])
                    {
                        TEXT += "\n" + (a + 1) + ": " + TradeValueSum[Number - 1] + "/" + TradeValueSum[a - 1];
                    }
                    else
                    {
                        TEXT += "\n" + (a + 1) + ": 0/0";
                    }
                }

            }
            MessageBox.Show(TEXT, "Handel", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TradePlayerNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (Trade[Number - 1, (int)TradePlayerNumeric.Value] == true)
            {
                TradeButton.Text = "Break";
            }
            else if (Trade[Number - 1, (int)TradePlayerNumeric.Value] == false)
            {
                TradeButton.Text = "Trade";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TradeUpdateMap();
        }

        public void SendMessage()
        {
            sw.WriteLine("write");
            sw.WriteLine("main");
            sw.WriteLine(MsgBox.Text);
            MsgBox.Text = "";
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void TaxToManpowerBar_Scroll(object sender, EventArgs e)
        {
            Tax = TaxToManpowerBar.Value;
            SliderLabel1.Text = "Manpower: " + (6 - Tax);
            SliderLabel2.Text = "Tax: " + Tax;
            CalculateIncome();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Unit u = new Unit("inf");
            UpdateMap();
            u.SetUnitData();
            u.moves = u.maxmoves;
        }

        private void UpgradeDevButton_Click(object sender, EventArgs e)
        {
            Gold += int.Parse(UpgradeDevButton.Text);
            string[] s = ProvinceIDLabel.Text.Split(' ');
            GetByID(int.Parse(s[1])).Development++;
            ProvinceTableUpdate(GetByID(int.Parse(s[1])));
            CalculateIncome();
            PowerPointsUpdate();
        }

        private void UpgradeFortsButton_Click(object sender, EventArgs e)
        {
            Gold += int.Parse(UpgradeFortsButton.Text);
            string[] s = ProvinceIDLabel.Text.Split(' ');
            GetByID(int.Parse(s[1])).Fortifications++;
            ProvinceTableUpdate(GetByID(int.Parse(s[1])));
            CalculateIncome();
            PowerPointsUpdate();
        }

        private void ArmypageUpdate()
        {
            int size = 0;
            int payments = 0;
            int reinforcments = 2;
            int[] armycomposition = {0,0,0,0,0};
            foreach (Unit u in Units)
            {
                if (u.position.Owner == Number)
                {
                    size++;
                    payments += u.Payment;
                    reinforcments++;
                    switch (u.type)
                    {
                        case "pike":
                            armycomposition[0]++;
                            break;
                        case "inf":
                            armycomposition[1]++;
                            break;
                        case "lcav":
                            armycomposition[2]++;
                            break;
                        case "cav":
                            armycomposition[3]++;
                            break;
                        case "arch":
                            armycomposition[4]++;
                            break;
                        case "wam":
                            armycomposition[5]++;
                            break;
                    }
                }
            }
            ArmySizeLabel.Text = $"Army: {size}";
            PaymentLabel.Text = "Payments: " + payments;
            ReinforcementsLabel.Text = "Reinforcments: " + reinforcments;
            ArmyCompositionLabel.Text = $"Army composition:\nPikemen: {armycomposition[0]}\nInfantry: {armycomposition[1]}\nL. Cavalry: {armycomposition[2]}\nCavalry: {armycomposition[3]}\nArchers: {armycomposition[4]}\nWar Machines: {armycomposition[5]}";

        }

        private void PageChanged(object sender, EventArgs e)
        {
            if (Tabs.SelectedTab.Text == "Army")
            {
                ArmypageUpdate();
                UnitTypeBox.Text = "";
            }
        }

        private void UnitTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (UnitTypeBox.Text)
            {
                case "Pikemen":
                    CostsLabel.Text = "Gold: 8\nManpower: 20\nGold per turn: 2";
                    if (Gold >= 6 && Manpower >= 20)
                        RecruitButton.Enabled = true;
                    else
                        RecruitButton.Enabled = false;
                    break;
                case "Infantry":
                    CostsLabel.Text = "Gold: 10\nManpower: 20\nGold per turn: 5";
                    if (Gold >= 10 && Manpower >= 20)
                        RecruitButton.Enabled = true;
                    else
                        RecruitButton.Enabled = false;
                    break;
                case "Light Cavalry":
                    CostsLabel.Text = "Gold: 16\nManpower: 20\nGold per turn: 7";
                    if (Gold >= 16 && Manpower >= 20)
                        RecruitButton.Enabled = true;
                    else
                        RecruitButton.Enabled = false;
                    break;
                case "Cavalry":
                    CostsLabel.Text = "Gold: 20\nManpower: 20\nGold per turn: 12";
                    if (Gold >= 20 && Manpower >= 20)
                        RecruitButton.Enabled = true;
                    else
                        RecruitButton.Enabled = false;
                    break;
                case "Archers":
                    CostsLabel.Text = "Gold: 12\nManpower: 20\nGold per turn: 8";
                    if (Gold >= 12 && Manpower >= 20)
                        RecruitButton.Enabled = true;
                    else
                        RecruitButton.Enabled = false;
                    break;
                case "War machines":
                    CostsLabel.Text = "Gold: 30\nManpower: 20\nGold per turn: 20";
                    if (Gold >= 30 && Manpower >= 20)
                        RecruitButton.Enabled = true;
                    else
                        RecruitButton.Enabled = false;
                    break;
                default:
                    CostsLabel.Text = "Pick an unit type";
                    RecruitButton.Enabled = false;
                    break;
            }

        }

        private void RecruitButton_Click(object sender, EventArgs e)
        {
            if (Capital.unit == null)
            {
                switch (UnitTypeBox.Text)
                {
                    case "Pikemen":
                        if (Gold >= 6 && Manpower >= 20)
                        {
                            Unit u = new Unit("pike");
                            Gold -= 6;
                            Manpower -= 20;
                        }
                        else
                            MessageBox.Show("Nie masz wystarczająco zasobów!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "Infantry":
                        if (Gold >= 10 && Manpower >= 20)
                        {
                            Unit u = new Unit("inf");
                            Gold -= 10;
                            Manpower -= 20;
                        }
                        else
                            MessageBox.Show("Nie masz wystarczająco zasobów!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "Light Cavalry":
                        if (Gold >= 16 && Manpower >= 20)
                        {
                            Unit u = new Unit("lcav");
                            Gold -= 16;
                            Manpower -= 20;
                        }
                        else
                            MessageBox.Show("Nie masz wystarczająco zasobów!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "Cavalry":
                        if (Gold >= 20 && Manpower >= 20)
                        {
                            Unit u = new Unit("cav");
                            Gold -= 20;
                            Manpower -= 20;
                        }
                        else
                            MessageBox.Show("Nie masz wystarczająco zasobów!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "Archers":
                        if (Gold >= 12 && Manpower >= 20)
                        {
                            Unit u = new Unit("arch");
                            Gold -= 12;
                            Manpower -= 20;
                        }
                        else
                            MessageBox.Show("Nie masz wystarczająco zasobów!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "War machines":
                        if (Gold >= 30 && Manpower >= 20)
                        {
                            Unit u = new Unit("wam");
                            Gold -= 30;
                            Manpower -= 20;
                        }
                        else
                            MessageBox.Show("Nie masz wystarczająco zasobów!", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                }
                UpdateMap(Capital);
                ArmypageUpdate();
                UnitTypeBox.Text = "";
            }
        }
        /* ArmyClassBeta
        public class UnitInBattle
        {
            public Unit UnitRef { get; set; }
            public int row { get; set; }
            public int position { get; set; }
            public bool madeturn { get; set; }
            public UnitInBattle(Unit u, int r, int p)
            {
                UnitRef = u;
                row = r;
                position = p;
            }
        }
        */
        /* AttackBeta
        public void Attack(List<Unit> attackers, Province defenders)
        {
            UnitInBattle[,] battleground = new UnitInBattle[4, 14];
            #region creating battleground
            battleground[0, 0] = null;
            battleground[1, 0] = null;
            battleground[2, 0] = null;
            battleground[3, 0] = null;
            battleground[0, 13] = null;
            battleground[1, 13] = null;
            battleground[2, 13] = null;
            battleground[3, 13] = null;
            int pos = 0;
            bool change = false;
            foreach (Unit u in attackers)
            {
                if (u.type == "arch" || u.type == "wam")
                {
                    if (change == false)
                    {
                        UnitInBattle ub = new UnitInBattle(u, 0, 7 + pos);
                        battleground[0, 7 + pos] = ub;
                        change = true;

                    }
                    else
                    {
                        UnitInBattle ub = new UnitInBattle(u, 0, 7 - pos);
                        battleground[0, 7 - pos] = ub;
                        change = false;
                    }
                    pos++;
                    if (pos == 6 && change == false)
                    {
                        pos = 0;
                        break;
                    }
                }
            }
            foreach (Unit u in attackers)
            {
                if (u.type == "pike" || u.type == "inf" || u.type == "lcav" || u.type == "cav")
                {
                    if (change == false)
                    {
                        UnitInBattle ub = new UnitInBattle(u, 1, 7 + pos);
                        battleground[1, 7 + pos] = ub;
                        change = true;

                    }
                    else
                    {
                        UnitInBattle ub = new UnitInBattle(u, 1, 7 - pos);
                        battleground[1, 7 - pos] = ub;
                        change = false;
                    }
                    pos++;
                    if (pos == 6 && change == false)
                    {
                        pos = 0;
                        break;
                    }

                }

            }
            foreach (Unit u in defenders.units)
            {
                if (u.type == "arch" || u.type == "wam")
                {
                    if (change == false)
                    {
                        UnitInBattle ub = new UnitInBattle(u, 3, 7 + pos);
                        battleground[0, 7 + pos] = ub;
                        change = true;

                    }
                    else
                    {
                        UnitInBattle ub = new UnitInBattle(u, 3, 7 - pos);
                        battleground[0, 7 - pos] = ub;
                        change = false;
                    }
                    pos++;
                    if (pos == 6 && change == false)
                    {
                        pos = 0;
                        break;
                    }
                }
            }
            foreach (Unit u in defenders.units)
            {
                if (u.type == "pike" || u.type == "inf" || u.type == "lcav" || u.type == "cav")
                {
                    if (change == false)
                    {
                        UnitInBattle ub = new UnitInBattle(u, 2, 7 + pos);
                        battleground[1, 7 + pos] = ub;
                        change = true;

                    }
                    else
                    {
                        UnitInBattle ub = new UnitInBattle(u, 2, 7 - pos);
                        battleground[1, 7 - pos] = ub;
                        change = false;
                    }
                    pos++;
                    if (pos == 6 && change == false)
                    {
                        pos = 0;
                        break;
                    }

                }

            }
            #endregion
            int alltroops = battleground.Length;
            bool[] usednumbers = new bool[alltroops];
            bool con = false;
            Random rand = new Random();
            do
            {
                con = false;
                int randomn = rand.Next(0, alltroops);
                UnitInBattle[] Defenders = new UnitInBattle[6];
                int opposite1 = 0;
                int opposite2 = 0;
                if (u.row == 1)
                {
                    opposite1 = 2;
                    opposite2 = 3;
                }
                else if (u.row == 2)
                {
                    opposite1 = 1;
                    opposite2 = 0;
                }
                else if (u.row == 0)
                {
                    opposite1 = 2;
                    opposite2 = 3;
                }
                else if (u.row == 3)
                {
                    opposite1 = 1;
                    opposite2 = 0;
                }
                Defenders[0] = battleground[opposite2, u.position - 1];
                Defenders[1] = battleground[opposite2, u.position];
                Defenders[2] = battleground[opposite2, u.position + 1];
                Defenders[3] = battleground[opposite1, u.position - 1];
                Defenders[4] = battleground[opposite1, u.position];
                Defenders[5] = battleground[opposite1, u.position + 1];
                switch (u.UnitRef.type)
                {
                    case "pike":
                        if (Defenders[4].UnitRef == null)
                        {
                            if (Defenders[1].UnitRef != null)
                            {

                            }
                        }
                        else
                        {
                            if (u.UnitRef.attack - Defenders[4].UnitRef.m_defence >= 5)
                                Defenders[4].UnitRef.hp -= u.UnitRef.attack - Defenders[4].UnitRef.m_defence;
                            else
                                Defenders[4].UnitRef.hp -= 5;
                            if (Defenders[4].UnitRef.hp <= 0)
                            {
                                Defenders[4].UnitRef.Kill();
                            }
                        }

                        break;
                    case "dead":
                        break;


                }

                foreach (bool b in usednumbers)
                {
                    if (b)
                        ;
                    else
                        con = true;
                }
            }
            while (con);
        }
        */
        private void TechButton_Click(object sender, EventArgs e)
        {
            TechnologyWindow tw = new TechnologyWindow(Number);
            tw.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void TrainButton_Click(object sender, EventArgs e)
        {
            if(Gold >= 10)
            {
                SelectedUnit.level += 10;
                PowerPointsUpdate();
                Gold -= 10;
            }
        }

        private void ReinforceButton_Click(object sender, EventArgs e)
        {
            if(Manpower >= 5)
            {
                SelectedUnit.hp += 10;
                PowerPointsUpdate();
                Manpower -= 5;
            }

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Unit b = new Unit("inf", Capital);
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            UpdateMap();
        }
    }
    public class Unit
    {
        public int dodge { get; set; }
        public int moves { get; set; }
        public int level { get; set; }
        public int maxmoves { get; set; }
        public int hp { get; set; }
        public int m_defence { get; set; }
        public int r_defence { get; set; }
        public int min_attack { get; set; }
        public int max_attack { get; set; }
        public string type { get; set; }
        public int owner { get; set; }
        public int Payment { get; set; }
        public Province position { get; set;}
        public void Kill()
        {
            Form1.Units.Remove(this);
            position.unit = null;
            position = null;
        }
        public void Heal()
        {
            if(Form1.Manpower >= (100 - hp) / 5)
            {
                
            }
        }
        public Unit(string Type, Province prov)
        {
            type = Type;
            Form1.Units.Add(this);
            prov.unit = this;
            position = prov;
            SetUnitData();
            moves = maxmoves;
            hp = 100;
        }
        public Unit(string Type)
        {
            type = Type;
            Form1.Units.Add(this);
            position = Form1.Capital;
            Form1.Capital.unit = this;            
            SetUnitData();
            moves = maxmoves;
            hp = 100;
        }
        public void SetUnitData()
        {
            switch (type)
            {
                #region pike
                case "pike":
                    min_attack = 16;
                    max_attack = 23;
                    m_defence = 12;
                    r_defence = 12;
                    maxmoves = 2;
                    dodge = 6;
                    Payment = 2;
                    min_attack += (Form1.Upgrades[(Form1.Number - 1) * 30] * 2);
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 1])
                    {
                        case 1:
                            dodge += 3;
                            break;
                        case 2:
                            m_defence += 3;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 2])
                    {
                        case 1:
                            max_attack += 5;
                            break;
                        case 2:
                            maxmoves += 1;
                            r_defence += 4;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 3])
                    {
                        case 1:
                            dodge += 5;
                            break;
                        case 2:
                            min_attack += 2;
                            max_attack += 3;
                            break;                            
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 4])
                    {
                        case 1:
                            m_defence += 5;
                            r_defence += 5;
                            break;
                        case 2:
                            dodge += 5;
                            break;
                    }
                    break;
                #endregion
                #region inf
                case "inf":
                    min_attack = 24;
                    max_attack = 28;
                    m_defence = 18;
                    r_defence = 12;
                    maxmoves = 2;
                    dodge = 4;
                    Payment = 5;     
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 5])
                    {
                        case 1:
                            dodge += 5;
                            break;
                        case 2:
                            r_defence += 4;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 6])
                    {
                        case 1:
                            m_defence += 10;
                            dodge -= 10;
                            break;
                        case 2:
                            max_attack += 4;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 7])
                    {
                        case 1:
                            m_defence += 3;
                            r_defence += 3;
                            break;
                        case 2:
                            maxmoves += 1;                            
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 8])
                    {
                        case 1:
                            min_attack += 4;
                            break;
                        case 2:                           
                            max_attack += 5;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 9])
                    {
                        case 1:
                            max_attack += 5;
                            break;
                        case 2:
                            m_defence += 5;
                            r_defence += 5;
                            break;
                    }
                    break;
                #endregion
                #region lcav
                case "lcav":
                    min_attack = 18;
                    max_attack = 24;
                    m_defence = 11;
                    r_defence = 4;
                    maxmoves = 4;
                    dodge = 23;
                    Payment = 7;                                    
                    
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 10])
                    {
                        case 1:
                            min_attack += 3;
                            break;
                        case 2:
                            r_defence += 3;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 11])
                    {
                        case 1:

                            dodge += 3;
                            break;
                        case 2:
                            m_defence += 5;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 12])
                    {
                        case 1:
                            min_attack += 3;
                            max_attack += 3;
                            break;
                        case 2:
                            r_defence += 8;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 13])
                    {
                        case 1:
                            m_defence += 3;
                            break;
                        case 2:
                            max_attack += 2;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 14])
                    {
                        case 1:
                            m_defence += 5;
                            r_defence += 5;
                            break;
                        case 2:
                            dodge += 5;
                            break;
                    }
                    break;
                #endregion
                #region cav
                case "cav":
                    min_attack = 34;
                    max_attack = 38;
                    m_defence = 23;
                    r_defence = 18;
                    maxmoves = 3;
                    dodge = 12;
                    Payment = 12;

                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 15])
                    {
                        case 1:
                            dodge += 3;
                            break;
                        case 2:
                            min_attack += 4;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 16])
                    {
                        case 1:

                            min_attack += 2;
                            break;
                        case 2:
                            max_attack += 3;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 17])
                    {
                        case 1:
                            r_defence += 3;
                            break;
                        case 2:
                            m_defence += 3;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 18])
                    {
                        case 1:
                            r_defence += 6;
                            break;
                        case 2:
                            max_attack += 3;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 19])
                    {
                        case 1:
                            min_attack += 3;
                            max_attack += 3;
                            break;
                        case 2:
                            m_defence +=3;
                            r_defence += 3;
                            break;
                    }
                    break;
                #endregion
                #region arch
                case "arch":
                    min_attack = 30;
                    max_attack = 36;
                    m_defence = 10;
                    r_defence = 5;
                    maxmoves = 2;
                    dodge = 8;
                    Payment = 8;
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 15])
                    {
                        case 1:
                            m_defence += 4;
                            break;
                        case 2:
                            r_defence += 5;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 16])
                    {
                        case 1:

                            dodge += 3;
                            break;
                        case 2:
                            min_attack += 4;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 17])
                    {
                        case 1:
                            maxmoves++;
                            break;
                        case 2:
                            max_attack += 4;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 18])
                    {
                        case 1:
                            m_defence += 5;
                            r_defence += 5;
                            break;
                        case 2:
                            min_attack += 4;
                            break;
                    }
                    switch (Form1.Upgrades[(Form1.Number - 1) * 30 + 19])
                    {
                        case 1:
                            m_defence += 6;
                            break;
                        case 2:
                            max_attack += 6;
                            break;
                    }
                    break;
                #endregion
                #region wam
                case "wam":
                    min_attack = 30;
                    max_attack = 60;
                    m_defence = 0;
                    r_defence = 0;
                    maxmoves = 1;
                    dodge = 0;
                    Payment = 20;
                    break;
#endregion
            }
        }
        
       

    }
    public class Province
    {
        public int ID { get; set; }
        public int Owner { get; set; }
        public Rectangle Rect { get; set; }
        public Rectangle ArmyRect { get; set; }
        public int Manpower { get; set; }
        public int Development { get; set; }
        public int Fortifications { get; set; }
        public int TradeValue { get; set; }
        public Unit unit { get; set; }
        public Province(Rectangle rect)
        {
            Rect = rect;
            ArmyRect = new Rectangle(rect.X + 5, rect.Y + 5, 12, 12);
        }
        public bool Capital { get; set; }

    }
}

/*
unit types shorts
pike
inf
lcav
cav
arch
wam


stats:
hp = 100

    pike:
    attack = 10 - 15 (x3 vs cavalry)
    m_defence = 5
    r_defence = 5
    moves = 2
    dodge = 10%

    inf:
    attack = 20 - 25 (+10 vs arch)
    m_defence = 20
    r_defence = 10
    moves = 2
    dodge = 5%

    lcav:
    attack = 17-22
    m_defence = 7
    r_defence = 0
    moves = 4
    dodge = 17%

    cav:
    attack = 30-36 (+10 vs arch)
    m_defence = 20
    r_defence = 15
    moves = 3
    dodge = 7%

    arch:
    attack = 24-32
    m_defence = 0
    r_defence = 0
    moves = 2
    dodge = 5%
    miss = 20%

    wam
    attack = 30-60
    m_defence = 0
    r_defence = 0
    moves = 1
    dodge = 0%


    terrains:
    this will be in the update (TerrainUpdate ;) )
    grassland = nothing
    hills = +3dmg for defender


 */

