using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAT2
{
    public partial class Form1 : Form
    {
        private UserControl1[] uc;
        private int val, addpensIDX, pens, BlueorRed;
        private ToolStripMenuItem last, HumOrCmp;
        private Button BR;
        private Bitmap comp;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Nim";
            last = toolS5;
            last.Checked = true;
            val = 5;
            comp = new Bitmap("../../Computer.ico");
            BR = new Button();
            BR.Click += new EventHandler(ButtonPressed);
            BR.Size = new Size(34, 126);
            Controls.Add(BR);
            HumOrCmp = MShuman;
            MShuman.Checked = true;
            newGame(val);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void newGame(int val)
        {
            if (uc != null)                           //
                for (int x = 0; x < uc.Length; x++)  //  **  Initializing  **
                    Controls.Remove(uc[x]);         //
            Random r = new Random();
            BR.Image = null;
            BR.BackColor = Color.Blue;
            uc = new UserControl1[val];
            for (int i = 0; i < val; i++)
            {
                uc[i] = new UserControl1(r.Next(1, 31));
                uc[i].Location = new Point(i * uc[i].Width, 24);
                uc[i].myEvent += new myDelegate(Updating_Combo_Box);
                Controls.Add(uc[i]);
            }
            ClientSize = new Size(uc[0].Width * val + BR.Width + 2, uc[0].Height + 24);
            BR.Location = new Point(ClientSize.Width - BR.Width, this.Height / 3);
            BlueorRed = 1;
        }

        private void ButtonPressed(object sender, EventArgs e)
        {
            if ((uc[0].isEnabled() && HumOrCmp == MShuman) || (HumOrCmp == MScomputer && BR.BackColor == Color.Blue && uc[0].isEnabled()))
                return;

            if (HumOrCmp == MScomputer && BR.BackColor == Color.Red)
            {
                computerLogic();
                BR.BackColor = Color.Blue;
            }
            else if (BR.BackColor == Color.Red)
            {
                BR.BackColor = Color.Blue;
                uc[addpensIDX].AddPens(pens, BlueorRed); // 0- Painting in Red
            }
            else
            {
                BR.BackColor = Color.Red;
                uc[addpensIDX].AddPens(pens, BlueorRed); // 1- Painting in Blue
            }
            for (int i = 0; i < uc.Length; i++)
            {
                if (HumOrCmp == MScomputer && BR.BackColor == Color.Blue || HumOrCmp == MShuman)
                {
                    uc[i].CBenable();
                    BR.Image = null;
                }
                else
                {
                    uc[i].CBdisable();
                    BR.Image = comp;                
                }
                    uc[i].CBforeColor(BlueorRed);
                }
            BlueorRed = 1 - BlueorRed;
            check();            
        }

        public void check()
        {
            DialogResult res;
            for (int i = 0; i < uc.Length; i++)
            {
                if (uc[i].HowManyGray() != 0)
                    return;
            }
            if (BlueorRed == 0)
            {
                BR.BackColor = Color.Blue;
                res = MessageBox.Show("New Game?", "The game is over. Blue won.", MessageBoxButtons.YesNo);
            }
            else
            {
                BR.BackColor = Color.Red;
                if (HumOrCmp == MScomputer)
                    BR.Image = comp;
                res = MessageBox.Show("New Game?", "The game is over. Red won", MessageBoxButtons.YesNo);
            }
            if (res == DialogResult.Yes)
                newGame(val);
            else
                Close();
        }


        private void Updating_Combo_Box(object sender, myEventArgs e)
        {
            UserControl1 tmp = (UserControl1)sender;
            pens = int.Parse(e.myString);
            for (int i = 0; i < uc.Length; i++)
            {
                if (tmp == uc[i])
                    addpensIDX = i;
                uc[i].CBdisable();
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame(val);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ts = (ToolStripMenuItem)sender;
            last.Checked = false;
            last = ts;
            ts.Checked = true;
            val = int.Parse(ts.Text);
            newGame(val);
        }
            
        private void HumanOrComputer(object sender, EventArgs e)
        {
            ToolStripMenuItem ts = (ToolStripMenuItem)sender;
            if (ts == MScomputer && HumOrCmp==MScomputer)
                return;
            else
            {
                HumOrCmp.Checked = false;
                HumOrCmp = ts;
                HumOrCmp.Checked = true;
            }
            BR.Image = null;
            if (ts == MShuman)
            {
                for (int i = 0; i < uc.Length; i++)
                    uc[i].CBenable();
            }
            else if (ts == MScomputer && BR.BackColor == Color.Red)
            {
                for (int i = 0; i < uc.Length; i++)
                    uc[i].CBdisable();
                BR.Image = comp;
            }
        }

        public void computerLogic()
        {
            Random rand = new Random();
            int num;
            int S = 0;
            int grayp = 0;
            int idx = 0;
            for (num = 0; num < val; num++)
            {
                S ^= uc[num].HowManyGray();
            }
            if (S != 0)
            {
                for (int i = 0; i < val; i++)
                {
                    if (uc[i].HowManyGray() != 0)
                    {
                        idx = i;
                        grayp = uc[i].HowManyGray() + 1;
                        while ((S != 0) && (grayp > 0))
                        {
                            grayp--;
                            S = 0;
                            for (num = 0; num < val; num++)
                            {
                                if (num == idx)
                                {
                                    S ^= uc[num].HowManyGray() - grayp;
                                }
                                else
                                {
                                    S ^= uc[num].HowManyGray();
                                }
                            }
                        }
                        if (S == 0)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                while (true)
                {
                    num = rand.Next(val);
                    if (uc[num].HowManyGray() != 0)
                    {
                        grayp = rand.Next(uc[num].HowManyGray() - 1) + 1;
                        idx = num;
                        break;
                    }
                }
            }
            uc[idx].AddPens(grayp, BlueorRed);
        }

    }
}