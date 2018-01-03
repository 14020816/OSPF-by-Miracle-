using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LastestVersionOSPF_OK
{
    class OSPF
    {
        int Time;
        public int TimeHT { get; set; }
        public int TimeLimit { get; set; }
        public List<Router> Topo = new List<Router>();
        public  int Node { get; set; }
        public int[,] Graph { get; set; }

        List<Event> ListEvent  = new List<Event>();
        public void Intizi()
        {
            //Console.WriteLine("Number Node ?");
            //Node = Console.Read();
            Node = 6;
            for(int i = 0 ; i < Node ; i++)
            {
                Router tmp = new Router();
                tmp.ID = i;
                Topo.Add(tmp);
            }

            for(int i = 0 ; i < Node ; i++)
            {
                Topo[i].LSDB = new int[Node, Node];
                Topo[i].Alive = false;
                Topo[i].NumberNode = Node;
            }
           int [,] Graph = {{0,7,15,2,0,1},{7,0,0,4,21,0},{15,0,0,25,0,0}, {2,4,25,0,8,0},{0,21,0,8,0,0}, {1,0,0,0,0,0}};
           this.Graph = Graph;
            for(int i = 0 ; i < Node ; i++)
            {
                for(int j = i ; j < Node ; j++)
                {
                    if(Graph[i,j] !=0 && i!= j)
                    {
                        Topo[i].NewConected(Topo[j], Graph[i, j]);
                        Topo[j].NewConected(Topo[i], Graph[i, j]);
                    }
                }
            }
        }

        public void InsertEvent(Event newE)
        {
            if(ListEvent.Count == 0)
            {
                ListEvent.Add(newE);
            }
            else if(ListEvent[ListEvent.Count - 1].Time <= newE.Time)
            {
                ListEvent.Add(newE);
            }
            else if(ListEvent[0].Time == newE.Time)
            {
                ListEvent.Insert(1, newE);
            }
            else if(ListEvent[0].Time > newE.Time)
            {
                ListEvent.Insert(0, newE);
            }
            else
            {
                for(int i = 0 ; i < ListEvent.Count-1 ; i++)
                {
                    if (newE.Time >= ListEvent[i].Time && newE.Time <= ListEvent[i + 1].Time)
                    {
                        ListEvent.Insert(i+1, newE);
                        break;
                    }
                }
            }
        }

        
        public void TurnOn ()
        {
            for(int i = 0 ; i < Node ; i++)
            {
                Random rdn = new Random();
                int tmp;
                tmp = rdn.Next(20);
                Console.WriteLine("Router 192.168.{0}.0 turn on at {1}ms ", Topo[i].ID,tmp );
              //  Topo[i].Alive = true;
                Event newEvent = new Event();
                newEvent.ID = Topo[i].ID;
                newEvent.Type = (int)EventType.SendHello;
                newEvent.Time = tmp;
                InsertEvent(newEvent);
                Thread.Sleep(50);
                
            }
        }

        public void Run()
        {
            Intizi();
            TurnOn();
            while (ListEvent.Count > 0)
            {
                Console.WriteLine("OSPF v1.0");
                Event DoNow = new Event();
                this.Time = DoNow.Time;
                DoNow = ListEvent[0];
                if(DoNow.Type == (int)EventType.SendHello)
                {
                    Console.WriteLine("Router 192.168.{0}.0 send hello packet to broadcass adress at time {1}ms\n", DoNow.ID, DoNow.Time);
                   
                    for (int i = 0; i < Topo.Count; i++ )
                    {   
                       // Time = DoNow.Time;
                        Random rdn = new Random();
                        int TimeNow = DoNow.Time;
                        Topo[DoNow.ID].SendHello(ref TimeNow, Topo[i]);
                        Event newEvent = new Event();
                        newEvent.Type = (int)EventType.SendLSA;
                        newEvent.ID = DoNow.ID;
                        newEvent.IDDestination = Topo[i].ID;
                        newEvent.Time = TimeNow + rdn.Next(15);
                        InsertEvent(newEvent);

                    }
                ListEvent.RemoveAt(0);
                       

                }
                else if(DoNow.Type == (int)EventType.SendLSA)
                {
                    int TimeNow = DoNow.Time;
                    Topo[DoNow.ID].SendLSA(ref TimeNow, Topo[DoNow.IDDestination]);
                    Event newEvent = new Event();
                    newEvent.ID = DoNow.IDDestination;
                    newEvent.IDDestination = DoNow.ID;
                    newEvent.Time = TimeNow;
                    newEvent.Type = (int)EventType.UpdateLSDB;
                    InsertEvent(newEvent);
                    ListEvent.RemoveAt(0);

                }
                else if(DoNow.Type == (int)EventType.UpdateLSDB)
                {
                    int TimeNow = DoNow.Time;
                    Topo[DoNow.ID].UpdateLSDB(Topo[DoNow.IDDestination], TimeNow);
                    //foreach (Router a in Topo[DoNow.ID].Neighbour)
                    //{
                    //    Topo[DoNow.ID].UpdateLSDB(a, TimeNow);
                    //}
                    //if(!Topo[DoNow.ID].CheckLSDB(Topo[DoNow.IDDestination]))
                    //{
                    //    foreach (Router a in Topo[DoNow.IDDestination].Neighbour)
                    //    {
                    //        Topo[DoNow.ID].UpdateLSDB(a, TimeNow);
                    //    }
                    //}
                    
                    if(Topo[DoNow.ID].CheckSPF())
                    {
                        Event newEvent = new Event();
                        newEvent.ID = DoNow.ID;
                        newEvent.IDDestination = 0;
                        newEvent.Time = TimeNow;
                        newEvent.Type = (int)EventType.SFP;
                        InsertEvent(newEvent);
                        ListEvent.RemoveAt(0);

                    }
                    else
                    {
                        Event newEvent = new Event();
                        newEvent.ID = DoNow.ID;
                        newEvent.IDDestination = DoNow.IDDestination;
                        newEvent.Time = TimeNow + 15;
                        newEvent.Type = (int)EventType.UpdateLSDB;
                        InsertEvent(newEvent);
                        ListEvent.RemoveAt(0);
                    }
                   
                    
                }
                else if(DoNow.Type == (int)EventType.SFP)
                {
                    TimeHT = DoNow.Time;
                    int TimeNow = DoNow.Time;
                    Topo[DoNow.ID].SPF();
                    Topo[DoNow.ID].ShowRouteTable();
                    Event NewEvent = new Event();
                    NewEvent.Type = (int)EventType.SendPacket;
                    NewEvent.ID = DoNow.ID;
                    NewEvent.Time = DoNow.Time + 40;
                 //   InsertEvent(NewEvent);
                    ListEvent.RemoveAt(0);
                    

                }
                else if(DoNow.Type == (int)EventType.SendPacket)
                {
                    int TimeNow = DoNow.Time;
                    for(int i = 0 ; i < Node ; i++)
                    {
                        if(DoNow.ID != i)
                        {
                            Topo[DoNow.ID].SendPacket(Topo[i]);
                        }
                       

                    }
                    ListEvent.RemoveAt(0);
                }
            }
          //  Console.WriteLine("Time : {0}", this.Time);
        }
        
        public void ComandLine()
        {
            Console.WriteLine("OSPF v1.0");
            Console.WriteLine("CONVERGE TIME : {0}", TimeHT);
            Console.WriteLine("Comand Line");
            Console.WriteLine("--------------------------------------------------------------------");
                    Console.WriteLine("Option \t\t\t ");
                    Console.WriteLine("");
                    Console.WriteLine("send \t\t\t Send Packet from Router to an other");
                    Console.WriteLine("");
                    Console.WriteLine("surnoff \t\t Turn Off a router");
                    Console.WriteLine("");
                    Console.WriteLine("ping \t\t\t");
                    Console.WriteLine("");  
                    Console.WriteLine("dis \t\t\t Disconnect a link");
                    Console.WriteLine("close \t\t\t close program");
                    Console.WriteLine("Type your comand ");
            string Comand = Console.ReadLine();
            while(Comand != "close")
            {
                if (Comand == "send")
                {
                    Console.WriteLine("Source ?");
                    int scoure = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Destinartion");
                    int destination = Convert.ToInt32(Console.ReadLine());
                    Event NewEvent = new Event();
                    Topo[scoure].SendPacket(Topo[destination]);
                    Comand = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Sorry for your inconvenience! The biggest update  will be release in next Spring, follow us add take it free !! Thank for your trial ");
                    break;
                }
            }
           // Console.WriteLine("Thank for your trial");
           
        }
        
    }
}
