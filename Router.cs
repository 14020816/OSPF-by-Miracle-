using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LastestVersionOSPF_OK
{

    class Metric
    {
        public int ID;
        public int metric;
        public Metric(int id, int _metric)
        {
            ID = id;
            metric = _metric;
        }
    }
    class Router
    {
        public int ID { get; set; }
        public List<Metric> myLSA = new List<Metric>();
        public int[,] LSDB { get; set; }

        public List<Router> ListConnected = new List<Router>();
        public List<Router> Neighbour = new List<Router>();

        public bool Alive { get; set; }

        public int[,] RouteTable { get; set; }

        public int NumberNode { get; set; }

        // Kiem tra xem co ket noi voi mot router khong
        public bool CheckConected(Router Destination)
        {
            for (int i = 0; i < ListConnected.Count; i++)
            {
                if (ListConnected[i].ID == Destination.ID)
                    return true;
            }
            return false;
        }

        public void NewConected(Router newR, int Metric)
        {
            ListConnected.Add(newR);
            Metric tmp = new Metric(newR.ID, Metric);
            myLSA.Add(tmp);
        }

        public bool CheckNeigbour(Router Destination)
        {
            for (int i = 0; i < Neighbour.Count; i++)
            {
                if (Neighbour[i].ID == Destination.ID)
                    return true;
            }
            return false;
        }
        void UpdateLSA()
        {
            foreach (Router t in Neighbour)
            {
                Metric tmp = new Metric(0, 0);
                tmp.ID = t.ID;
                tmp.metric = 4;
            }
        }

        public void UpdateLSDB(Router Destination, int Time)
        {
            Console.WriteLine("Router 192.168.{0}.0 update LSDB from 192.168.{1}.0 Time : {2}", this.ID, Destination.ID,Time);
            for (int i = 0; i < Destination.myLSA.Count; i++)
            {
                    
                       LSDB[Destination.ID, Destination.myLSA[i].ID] = Destination.myLSA[i].metric;
                       LSDB[Destination.myLSA[i].ID, Destination.ID] = Destination.myLSA[i].metric;
            }

        }
        // Gui Hello toi mot router khac
        public void SendHello(ref int Time, Router Destination)
        {


            if (CheckConected(Destination) && !CheckNeigbour(Destination))
            {
                Random mt = new Random();
                Console.WriteLine("Router 192.168.{0}.0 discovered new neighbour ID = 192.168.{1}.0\n", this.ID, Destination.ID);
                this.Neighbour.Add(Destination);
                Destination.Neighbour.Add(this);
                //  Time += mt.Next(15);
                // myLSA.Add(new Metric(Destination.ID, 3 * mt.Next(5)));
            }
            else if (CheckNeigbour(Destination) && !CheckConected(Destination))
            {
                Console.WriteLine("Neighbour 192.168.{0}.0 not response , either it down or the link has some problem. Send LSA to other neighbour to advertise them somethings has changed\n");
                int tmp = Neighbour.FindIndex(0, c => c.ID == Destination.ID);
                Neighbour.RemoveAt(tmp);

            }
            else if (!CheckConected(Destination) && !CheckNeigbour(Destination))
            {
                Console.WriteLine("Time out\n");
            }

        }

        //public void ResponeHello(ref int Time, Router Destination)
        //{
        //   // Random rdn = new Random();
        //   // int tmp = rdn.Next(20);
        //    Console.WriteLine("Router 192.168.{0}.0 response and confirm Router 192.168.{1}.0 is its neighbour at time {2}ms", this.ID, Destination.ID, tmp);
        //}
        public void SendLSA(ref int Time, Router Destination)
        {
            Console.WriteLine("Router 192.168.{1}.0 send LSA to 192.168.{0}.0 at time {2}ms\n", Destination.ID, this.ID, Time);
           // Destination.UpdateLSDB(this);

        }

        Dijkstra myDij = new Dijkstra();
        public void SPF()
        {
            myDij.Graph = LSDB;
            myDij.DijkstraIm(LSDB, this.ID, NumberNode);
            RouteTable = myDij.KQ;
        }

        public void ShowRouteTable()
        {
            Console.WriteLine("==================================================================================");
           
            Console.WriteLine("Active Routes : \t192.168.{0}.0", this.ID);
            Console.Write("Network Destination\t ");
            Console.Write("Netmask\t\t");
            Console.Write("NextHop \t\t");
            Console.WriteLine("Metric");
            for (int i = 0; i < NumberNode; i++) 
            {
                if(i!= this.ID)
                {
                    Console.Write("192.168.{0}.0\t\t", i);
                    Console.Write("255.255.255.0\t\t");
                    Console.Write("192.168.{0}.0\t\t", myDij.GetNextHop(i));
                    Console.Write("{0}", myDij.Metric[i]);
                    Console.WriteLine(); 
                }
               
                
            }
            Console.WriteLine("===================================================================================");
        }

        public void SendPacket(Router Destination)
        {
            int[] Path = new int[10];
            Console.WriteLine("Send packet from router 192.168.{0}.0 to router 192.168.{1}.0", this.ID, Destination.ID);
            for(int i = 0 ; i < 60 ; i++)
            {
                Console.Write("/");
               Thread.Sleep(10);
                
            }
            Console.WriteLine("");  
 
            Path = myDij.GetPath(Destination.ID);
            int Start = 0;
            for(int i = 0 ; i < this.NumberNode ; i++)
            {
                if(Path[i] == -1)
                {
                    Start = i - 1;
                    break;
                }
            }
            int n = 1;
            for(int k = Start ; k >= 0 ; k--)
            {
                Random sdr = new Random();
                int v = sdr.Next(10);
                Console.WriteLine("{0} \t\t {2}ms \t\t 192.168.{1}.0", n, Path[k],v);
                n++;
                Thread.Sleep(10);
            }

        }
        public bool CheckLSDB(Router Destination)
        {
            for(int i = 0 ; i < NumberNode ; i++)
            {
                for(int j = 0 ; j < NumberNode ; j++)
                {
                    if (this.LSDB[i, j] != Destination.LSDB[i, j])
                        return false;
                }
            }
            return true;
        }
        public bool CheckSPF()
        {
            foreach (Router r in this.Neighbour)
            {
                if (!this.CheckLSDB(r))
                    return false;
            }
            return true;
        }
    }

    enum EventType
    {
        SendHello, SendLSA, UpdateLSA, UpdateLSDB, SFP, RouteTable, RPHello, SendPacket
    };
    class Event
    {
        public int ID { get; set; }

        public int IDDestination { get; set; }
        public int Time { get; set; }
        public int Type { get; set; }
    }

     class Dijkstra
    {
        public int[,] Graph { get; set; }
        public int[,] KQ { get; set; }

        public int Source { get; set; }

        public int[] Metric { get; set; }
        private  int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
        {
            int min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (shortestPathTreeSet[v] == false && distance[v] <= min)
                {
                    min = distance[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }

        public int[] Parent {get; set;}

        public int GetParent(int Node)
        {
            return Parent[Node];
        }
        public int GetNextHop(int Destination)
        {
            int nexthop = Destination;
            int tmp = Destination;
            int k = 5;
            while (Parent[tmp] != -1 && k > 0)
            {
                nexthop = tmp;
                tmp = Parent[tmp];
                k--;
            }
            return nexthop;
        }

        public  int [] GetPath(int Destination)
        {
            int[] path = new int[10];
            for(int j = 0 ; j < 10 ; j++)
            {
                path[j] = -1;
            }
            int nexthop = Destination;
            int tmp = Destination;
            int k = 10;
            int i = 0;
            while (Parent[tmp] != -1 && k > 0)
            {
                path[i] = tmp;
                tmp = Parent[tmp];
                k--;
                i++;
            }
            return path;
        }
        

        private void Print(int[] distance, int verticesCount)
        {
            Console.WriteLine("Vertex    Distance from source");

            for (int i = 0; i < verticesCount; ++i)
                Console.WriteLine("{0}\t  {1}", i, distance[i]);
        }


        public  void DijkstraIm(int[,] graph, int source, int verticesCount)
        {
            int[] distance = new int[verticesCount];
            bool[] shortestPathTreeSet = new bool[verticesCount];
            int[] Parent = new int[verticesCount];
            this.Parent = new int[6];
            for (int i = 0; i < verticesCount; ++i)
            {
                Parent[source] = -1;
                this.Parent[source] = -1;
                distance[i] = int.MaxValue;
                shortestPathTreeSet[i] = false;
            }

            distance[source] = 0;

            int[,] kq = new int[verticesCount, verticesCount];



            for (int count = 0; count < verticesCount - 1; ++count)
            {
                int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
                shortestPathTreeSet[u] = true;

                for (int v = 0; v < verticesCount; ++v)
                {
                    if (!shortestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MaxValue && distance[u] + graph[u, v] < distance[v])
                    {
                        distance[v] = distance[u] + graph[u, v];
                        Parent[v] = u;
                        this.Parent[v] = u;
                    }
                }


            }

        //    Print(distance, verticesCount);
            Metric = distance;
        }
    }
}


