using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace lifeexperiment
{
    class World
    {
        public List<Point> foods;
        public List<creature> creatures;
        int numberoffood = 100;
        int numberofcreatures = 50;
        public Graphics gr;
        public int width = 400;
        public int height = 400;
        public World()
        {
            Random rnd = new Random();
            foods = new List<Point>();
            creatures = new List<creature>();
            for (int i = 0; i < numberoffood; i++)
                foods.Add(new Point(rnd.Next(width), rnd.Next(height)));
            for (int i = 0; i < numberofcreatures; i++)
            {
                creature cr = new creature(rnd,this);
                creatures.Add(cr);
            }
        }
        public void render()
        {
            gr.Clear(Color.White);
            Pen p1 = new Pen(Color.Red);
            Pen p2 = new Pen(Color.Blue);
            foreach (Point f in foods)
                gr.DrawRectangle(p1, f.X, f.Y, 5, 5);
            foreach (creature cr in creatures)
            {
                gr.DrawRectangle(p2, cr.location.X, cr.location.Y, cr.energy / 1000, cr.energy / 1000);
                gr.DrawString((cr.numoflayers * cr.numofneurons).ToString(), new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, cr.location);
            }
        }
        public void step()
        {
            foreach (creature cr in creatures)
                cr.think();
            List<creature> deadcreatures = new List<creature>();
            foreach (creature cr in creatures)
                if (cr.dead)
                    deadcreatures.Add(cr);
            foreach (creature cr in deadcreatures)
                creatures.Remove(cr);
            render();
        }
    }
}
