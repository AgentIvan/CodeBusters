using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Entity
{
    public int Id; // buster id or ghost id
    public int x;
    public int y; // position of this buster / ghost
    public int x1 = 8000;
    public int y1 = 4500; // position of this buster / ghost
    public int entityType; // the team id if it is a buster, -1 if it is a ghost.
    public int state; // For bus: 0=idle, 1=carrying a ghost.
    public int value; // For bus: Ghost id being carried. For ghosts: number of bus attempting to trap this ghost.
    public int canStun = 0;
    public int EToStun = -1;
    public Entity()
    {
    }
    public Entity(int x1, int y1)
    {
        x = x1;
        y = y1;
    }
    public int Dist(Entity e)
    {
        return (int)Math.Sqrt(Math.Pow(x - e.x, 2) + Math.Pow(y - e.y, 2));
    }
}

class Player
{
    static void Main(string[] args)
    {
        int busPerPlayer = int.Parse(Console.ReadLine()); // the amount of bus you control
        int ghostCount = int.Parse(Console.ReadLine()); // the amount of ghosts on the map
        int myTeamId = int.Parse(Console.ReadLine()); // if this is 0, your base is on the top left of the map, if it is one, on the bottom right
        int baseX = (myTeamId==0)?0:16000;
        int baseY = (myTeamId==0)?0:9000;
        Entity Base = new Entity(baseX, baseY);
        Entity[] bus = new Entity[busPerPlayer];
        // game loop
        while (true)
        {
            int entities = int.Parse(Console.ReadLine()); // the number of bus and ghosts visible to you
            Entity[] e = new Entity[entities];
            int k = 0;
            List<Entity> Ghosts = new List<Entity>();
            List<Entity> Enemy = new List<Entity>();
            for (int i = 0; i < entities; i++)
            {
                e[i] = new Entity();
                string input = Console.ReadLine();
                Console.Error.WriteLine(input);
                string[] inputs = input.Split(' ');
                e[i].Id = int.Parse(inputs[0]); // buster id or ghost id
                e[i].x = int.Parse(inputs[1]);
                e[i].y = int.Parse(inputs[2]); // position of this buster / ghost
                e[i].entityType = int.Parse(inputs[3]); // the team id if it is a buster, -1 if it is a ghost.
                e[i].state = int.Parse(inputs[4]); // For bus: 0=idle, 1=carrying a ghost.
                e[i].value = int.Parse(inputs[5]); // For bus: Ghost id being carried. For ghosts: number of bus attempting to trap this ghost.
                if(myTeamId==e[i].entityType)
                {
                    if(bus[i]==null)bus[k++]=e[i];
                    else
                    {
                        bus[i].x=e[i].x;
                        bus[i].y=e[i].y;
                        bus[i].state=e[i].state;
                        bus[i].value=e[i].value;
                    }
                }
                else if(e[i].entityType==-1) Ghosts.Add(e[i]);
                else
                {
                    if(e[i].state==1) Enemy.Add(e[i]);
                    Console.Error.WriteLine("e[i].Id:{0} e[i].state:{1}", e[i].Id, e[i].state);
                }
            }
            
            for (int bn = 0; bn < busPerPlayer; bn++)
            {
                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");
                int minD = 100000, D;
                if(bus[bn].state==0 && Ghosts.Count>0)
                {
                    Entity minG = Ghosts[0];
                    Console.Error.WriteLine("Ghosts.Count:{0}", Ghosts.Count);
                    foreach(Entity g in Ghosts)
                    if((D = g.Dist(bus[bn]))<minD)
                    {
                        minD = D;
                        minG = g;
                        Console.Error.WriteLine("minD:{0} minG.Id:{1}", minD, minG.Id);
                    }
                    if(minG.Dist(bus[bn])<1760)
                        Console.WriteLine("BUST {0}", minG.Id);
                    else
                        Console.WriteLine("MOVE {0} {1}", minG.x, minG.y); // MOVE x y | BUST id | RELEASE
                }
                else if(bus[bn].state==1)
                    if(Base.Dist(bus[bn])<1600)
                        Console.WriteLine("RELEASE");
                    else
                        Console.WriteLine("MOVE {0} {1}", baseX, baseY);
                else
                {
                    if(Math.Abs(bus[bn].x1-bus[bn].x)<5 & Math.Abs(bus[bn].y1-bus[bn].y)<4)
                    {
                        Random rnd = new Random();
                        double degree = rnd.Next(1200*bn, 16000-1200*(busPerPlayer-bn)) % 360;
                        double angle = Math.PI * degree / 180.0;
                        Console.Error.WriteLine("degree:{0} angle:{1:3} x:{2}:{3} y:{4}:{5}", degree, angle, bus[bn].x, bus[bn].x1, bus[bn].y, bus[bn].y1);
                        bus[bn].x1 = (int)((Math.Sin(angle)>0)?15000:1000);
                        bus[bn].y1 = (int)((Math.Cos(angle)>0)?8000:1000);
                        Console.Error.WriteLine("bn:{0} x1:{1} y1:{2}", bn, bus[bn].x1, bus[bn].y1);
                    }
                    if(Enemy.Count>0)
                    {
                        minD = 100000;
                        Entity minE = Enemy[0];
                        foreach(Entity en in Enemy)
                        if((D = en.Dist(bus[bn]))<minD)
                        {
                            minD = D;
                            minE = en;
                            Console.Error.WriteLine("minD:{0} minE.Id:{1}", minD, minE.Id);
                        }
                        if(minE.Dist(bus[bn])<1760)
                            Console.WriteLine("STUN {0}", minE.Id);
                        else Console.WriteLine("MOVE {0} {1}", bus[bn].x1, bus[bn].y1);
                    }
                    else Console.WriteLine("MOVE {0} {1}", bus[bn].x1, bus[bn].y1);
                }
            }
        }
    }
}
