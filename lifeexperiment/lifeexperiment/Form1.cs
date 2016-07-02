using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace lifeexperiment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool pause = false;
        private void button1_Click(object sender, EventArgs e)
        {
            Graphics gr = this.CreateGraphics();
            World world = new World();
            world.gr = gr;
            for (int i = 0; i < 10000;)
            {
                if (!pause)
                {
                    world.step();
                    Thread.Sleep(100);
                    i++;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!pause)
            {
                pause = true;
                button2.Text = "resume";
            }
            else
            {
                pause = true;
                button2.Text = "resume";
            }
        }
    }
}
