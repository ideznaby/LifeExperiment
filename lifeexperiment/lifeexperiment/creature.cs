using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BrainNet.NeuralFramework;
using System.Collections;
using System.Windows.Forms;

namespace lifeexperiment
{
    class creature
    {
        public Point location;
        public int energy;
        public int age;
        public World world;
        Point nearestfood;
        double nearestfooddist;
        creature nearestoponent;
        double nearestoponentdist;
        public bool dead;
        Dictionary<String, String> perceptions;
        Dictionary<String, String> badperceptions;
        INeuralNetwork brain = new NeuralNetwork();
        public int numoflayers;
        public int numofneurons;
        //INeuralNetwork badbrain = new NeuralNetwork();
        Random rnd;
        public creature(Random rand,World _world)
        {
            world = _world;
            dead = false;
            rnd = rand;
            location = new Point(rnd.Next(world.width), rnd.Next(world.height));
            age = 0;
            energy = 10000;
            numoflayers = rnd.Next(1,5);
            numofneurons = rnd.Next(1,9);
            BackPropNetworkFactory factory = new BackPropNetworkFactory();
            ArrayList layers = new ArrayList();
            layers.Add(9);
            for (int i = 0; i < numoflayers; i++)
                layers.Add(numofneurons);
            layers.Add(6);
            brain = factory.CreateNetwork(layers);
          //  badbrain = factory.CreateNetwork(layers);
            perceptions = new Dictionary<string, string>();
           // badperceptions = new Dictionary<string, string
           /* for(int i=0;i<10;i++)
                for (int j = 0; j < 10; j++)
                {
                    perceptions.Add(i + " " + j + " " + 0 + " " + 0 + " " + 0 + " " + 0, "0 0 0 0 1");
                    perceptions.Add(i + " " + j + " " + (i+1) + " " + 0 + " " + 1 + " " + 0, "1 0 0 0 0");
                }*/
        }
        public double distancetonearestfood()
        {
            double mindistance;
            if (world.foods.Count > 0)
            {
                mindistance = distance(location, world.foods.ElementAt<Point>(0));
                nearestfood = world.foods.ElementAt<Point>(0);
                foreach (Point f in world.foods)
                {
                    double d = distance(location, f);
                    if (d < mindistance)
                    {
                        mindistance = d;
                        nearestfood = f;
                    }
                }
                nearestfooddist = mindistance;
                return mindistance;
            }
            else
            {
                nearestfooddist = 10000;
                return 10000;
            }
        }
        public double distancetonearestoponent()
        {
            double mindistance;
            mindistance = 10000;
            bool othercreatures = false;
            foreach (creature c in world.creatures)
            {
                if (c == this)
                    continue;
                othercreatures = true;
                double d = distance(location, c.location);
                if (d < mindistance)
                {
                    mindistance = d;
                    nearestoponentdist = d;
                    nearestoponent = c;
                }
            }
            if (othercreatures)
                return mindistance;
            else
                return 0;
        }
        public double distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public double satisfaction()
        {
            return (0.001 * energy) - (distancetonearestfood() / 500);//+ (distancetonearestoponent() / 1000);
        }
        public void Goto(string direction)
        {
            switch (direction)
            {
                case "left": if(location.X>0) location.X--; break;
                case "right": if (location.X < world.width) location.X++; break;
                case "up": if(location.Y > 0) location.Y--; break;
                case "down": if(location.Y < world.height) location.Y++; break;
            }
            energy--;
            age++;
        }
        public void eat()
        {
            if (nearestfooddist <= 5)
            {
                world.foods.Remove(nearestfood);
                energy += 1000;
            }
            energy--;
            age++;
        }
        public void kill()
        {
            if (nearestoponentdist <= 5)
            {
                if (nearestoponent.energy + rnd.Next(100) < energy + rnd.Next(200))
                {
                    //world.creatures.Remove(nearestoponent);
                    energy += 1000;
                    nearestoponent.dead = true;
                }
                else
                {
                    //world.creatures.Remove(this);
                    this.dead = true;
                    nearestoponent.energy += 1000;
                }
            }
            energy--;
            age++;
        }
        public void Action(int index)
        {
            switch (index)
            {
                case 0: Goto("left"); break;
                case 1: Goto("up"); break;
                case 2: Goto("right"); break;
                case 3: Goto("down"); break;
                case 4: eat(); break;
                case 5: kill(); break;
            }
        }
        public void think()
        {
            if (energy == 0)
            {
                dead = true;
            }
            bool anyperception = false;
                foreach (KeyValuePair<string, string> P in perceptions)
                {
                    TrainingData td = new TrainingData();
                    string[] perceptioninput = P.Key.Split(' ');
                    string[] perceptionoutput = P.Value.Split(' ');
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[0]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[1]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[2]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[3]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[4]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[5]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[6]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[7]));
                    td.Inputs.Add(Convert.ToDouble(perceptioninput[8]));
                    td.Outputs.Add(Convert.ToInt32(perceptionoutput[0]));
                    td.Outputs.Add(Convert.ToInt32(perceptionoutput[1]));
                    td.Outputs.Add(Convert.ToInt32(perceptionoutput[2]));
                    td.Outputs.Add(Convert.ToInt32(perceptionoutput[3]));
                    td.Outputs.Add(Convert.ToInt32(perceptionoutput[4]));
                    td.Outputs.Add(Convert.ToInt32(perceptionoutput[5]));
                    brain.TrainNetwork(td);
                    anyperception = true;
                }
                perceptions.Clear();
            double presat = satisfaction();
            ArrayList inputs = new ArrayList();
            inputs.Add(location.X);
            inputs.Add(location.Y);
            inputs.Add(nearestfooddist);
            inputs.Add(distancetonearestoponent());
            inputs.Add(Math.Sign(nearestfood.X - location.X));
            inputs.Add(Math.Sign(nearestfood.Y - location.Y));
            inputs.Add(Math.Sign(nearestoponent.location.X - location.X));
            inputs.Add(Math.Sign(nearestoponent.location.Y - location.Y));
            inputs.Add(Math.Sign(nearestoponent.energy - energy));
            ArrayList outputs = brain.RunNetwork(inputs);
            int action = 0;
            double max = 0;
            int[] actionout = {0,0,0,0,0,0};
            if (nearestfooddist <= 5)
            {
                action = 4;
                Action(action);
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Convert.ToDouble(outputs[i]) > max)
                    {
                        max = Convert.ToDouble(outputs[i]);
                        action = i;
                    }

                }

                if (anyperception && age > 100)
                {
                    Action(action);
                    actionout[action] = 1;
                }
                else
                {
                    int randomact = rnd.Next(6);
                    action = randomact;
                    Action(randomact);
                    actionout[randomact] = 1;
                }
            }
            double newsat = satisfaction();
            string state = location.X + " " + location.Y + " " + nearestfooddist + " " + distancetonearestoponent() + " " + Math.Sign(nearestfood.X - location.X) + " " + Math.Sign(nearestfood.Y - location.Y) + " " + Math.Sign(nearestoponent.location.X - location.X) + " " + Math.Sign(nearestoponent.location.Y - location.Y) + " " + Math.Sign(nearestoponent.energy - energy);
            if (newsat >= presat)
            {
                for (int i = 0; i <= (newsat - presat) * 10;i++ )
                    perceptions.Add(state + " " + (i+Environment.TickCount), actionout[0] + " " + actionout[1] + " " + actionout[2] + " " + actionout[3] + " " + actionout[4] + " " + actionout[5] + " " + newsat);
            }
            else
            {
                int randaction = 0;
                do
                {
                    randaction = rnd.Next(6);
                    actionout = new int[] { 0, 0, 0, 0, 0, 0 };
                    actionout[randaction] = 1;
                } while (randaction == action);
                    perceptions.Add(state + " " + Environment.TickCount, actionout[0] + " " + actionout[1] + " " + actionout[2] + " " + actionout[3] + " " + actionout[4] + " " + actionout[5] + " " + newsat);
            }
        }
    }
}
