using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 1)
            {
                string path = args[0];
                string[] content = System.IO.File.ReadAllLines(path);

            }else
            {
                Console.Error.WriteLine("For some reason there was more than one command line argument");
            }
        }
        void readGenerals(string[] generals)
        {
            int NumberOfNodes = 0;
            int VZero;
            
            for(int i = 0; i < generals.Length; i++)
            {
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
                    } else
                    {
                        Faulty = false;
                        //Not faulty, therefore do not need to create script
                        var NonFaulty = new Byzantine(id, init, NumberOfNodes);
                    }


                }
            }
        }
        struct Byzantine
        {
            int NodeId;
            int InitValue;
            int TotalNumberOfNodes;
            bool isFaulty;
            string[] ByzFaultyScript;

            public Byzantine(int Id, int Init, int Nodes)
            {
                NodeId = Id;
                InitValue = Init;
                TotalNumberOfNodes = Nodes;
                isFaulty = false;
                ByzFaultyScript = null;
            }

            public Byzantine(int Id, int Init, int Nodes, bool ByzFaulty, string[] script)
            {
                NodeId = Id;
                InitValue = Init;
                TotalNumberOfNodes = Nodes;
                isFaulty = ByzFaulty;
                ByzFaultyScript = script;
            }

        }
    }
}
