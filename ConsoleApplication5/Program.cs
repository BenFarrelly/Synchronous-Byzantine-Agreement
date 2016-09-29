using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ByzantineAgreementSync.Program.ByzActor;

namespace ByzantineAgreementSync
{
    class Program
    {
        public class ByzActor : ReceiveActor
        {


           public struct Byzantine
            {
                int NodeId;
                int InitValue;
                int TotalNumberOfNodes;
                bool isFaulty;
                string[] ByzFaultyScript;
                Dictionary<string, Dictionary<string, Int32>> EIG;

                public Byzantine(int Id, int Init, int Nodes)
                {
                    NodeId = Id;
                    InitValue = Init;
                    TotalNumberOfNodes = Nodes;
                    isFaulty = false;
                    ByzFaultyScript = null;
                    EIG = null;
                }

                public Byzantine(int Id, int Init, int Nodes, bool ByzFaulty, string[] script)
                {
                    NodeId = Id;
                    InitValue = Init;
                    TotalNumberOfNodes = Nodes;
                    isFaulty = ByzFaulty;
                    ByzFaultyScript = script;
                    EIG = null;
                }
                
            }
            public ByzActor()
            {
                Receive(x =>
                {
                    Sender.Tell("Hello");
                });
            }
        }
        void CreateEIGTree(ByzActor.Byzantine[] Byzantines)
        {
           
        }
        Byzantine[] readGenerals(string[] generals)
        {
            int NumberOfNodes = 0;
            int VZero;
            Byzantine[] Byzantines = new Byzantine[NumberOfNodes];
            for(int i = 0; i < generals.Length; i++)
            {
                Byzantine? Byz = null;
                if(i == 0)
                {
                    string[] temp = generals[i].Split(' ');
                    NumberOfNodes = Int32.Parse(temp[0]);
                    VZero = Int32.Parse(temp[1]);
                }
                else
                {
                    string[] temp = generals[i].Split(' ');
                    int id = Int32.Parse(temp[0]);
                    int init = Int32.Parse(temp[1]);
                    bool Faulty;

                    if (Int32.Parse(temp[2]) == 1)
                    {
                        Faulty = true;
                        //Since true read the script
                        //Expecting N + N strings, with second part being strings of length N-1
                        string[] script = new string[NumberOfNodes*2];
                        for(int j = 2; j < NumberOfNodes*2; j++)
                        {
                            //For creating string[] from script
                            script[j - 2] = temp[j];
                        }
                        Byz = new Byzantine(id, init, NumberOfNodes, true, script);
                    } else
                    {
                        Faulty = false;
                        //Not faulty, therefore do not need to create script
                        Byz = new Byzantine(id, init, NumberOfNodes);
                    }


                }
                if (Byz.HasValue)
                {
                    Byzantines[i] = Byz.GetValueOrDefault();
                }
                
            }
            return Byzantines;
        }
        
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string path = args[0];
                string[] content = System.IO.File.ReadAllLines(path);

            }
            else
            {
                Console.Error.WriteLine("For some reason there was more than one command line argument");
            }
        }
    }

}
