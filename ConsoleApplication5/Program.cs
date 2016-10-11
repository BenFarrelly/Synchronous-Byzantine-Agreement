using Actress;
using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ByzantineAgreementSync.Program;
//TODO Implement tree structure
namespace ByzantineAgreementSync
{
    class Program
    {
      
        public class Message
        {
            public Message(int from, int value) { From = from; Value = value; }
            public int From { get; private set; }
            public int Value { get; private set; }
        }
        public class Message2 
        {
            //public Message2(int from, int value) { From = from; Value = value; }
       
            public Message2(int from, int[] value) { From = from; Value = value; }
            public int From { get; private set; }
            public int[] Value { get; private set; }
        }
        public static MailboxProcessor<Message>[] Byz;
        public static MailboxProcessor<Message2>[] Byz2;

        public static int N;
        public static TaskCompletionSource<bool> Completion;
        static int CompletionCount;

        public class ByzProcess
        {
            //  public readonly int NodeId;
            //  public int InitValue { get; }
            //  int TotalNumberOfNodes;
            //  bool isFaulty;
            //  string[] ByzFaultyScript;
            //  Dictionary<string, Dictionary<string, Int32>> EIG;

            public ByzProcess(int Id, int init)
            {
                ID = Id;
                Init = init;
                faulty = false;
                script = null;
                EIG = new int[N][];
                for (int i = 0; i < N; i++)
                {
                    EIG[i] = new int[N];
                }

            }
            public ByzProcess(int Id, int init, bool ByzFaulty, string[] ByzScript)
            {
                ID = Id;
                Init = init;

                faulty = ByzFaulty;
                script = ByzScript;
                EIG = new int[N][];
                for(int i = 0; i < N; i++)
                {
                    EIG[i] = new int[N];
                }
                
            }
            public int ID { get; private set; }
            public int Init { get; private set;}
            public bool faulty { get; private set; }
            public string[] script { get; private set; }
            public int[][] EIG { get; private set; }

            public async Task ByzBody (MailboxProcessor<Message> inbox)
            {
                for(var i=0; i < N; i++)
                {
                    var msg = new Message(ID, Init);
                    Byz[i].Post(msg);
                    Console.WriteLine("1: Posting: " + ID + ", " + Init);
                }

                for(var i = 0; i < N; i++)
                {
                    var msg = await inbox.Receive();
                   // EIG.Add(msg.From, msg.Value);
                    Console.WriteLine("1: Receiving["+ID+ "]: Value: "+msg.Value+", From: " + msg.From);
                    //Receive message and put first round values into first row
                    EIG[0][ msg.From - 1] = msg.Value;
                }
                //This is the synchronizer, start round two afterwards?

                var count = Interlocked.Decrement(ref CompletionCount);
                if (count == 0) Completion.SetResult(true);         
            }

            public async Task ByzBody2(MailboxProcessor<Message2> inbox)
            {//Need to include options for faulty node
            
                //begin round 2 message passing
                //int count2 = Interlocked.Decrement(ref CompletionCount);
                //Round 2, pass the first row of the EIG table to every other node.
                for (int i = 0; i < N; i++)
                {
                    
                    var msg2 = new Message2(ID, EIG[0]);
                    Byz2[i].Post(msg2);
                    Console.WriteLine("2: Posting: " + ID + ", " + EIG[0]);
                }
                for(int i = 0; i < N; i++)
                {
                    var msg2 = await inbox.Receive();
                    Console.WriteLine("2: Receiving[" + ID + "]: Value: " + msg2.Value + ", From: " + msg2.From);
                    EIG[msg2.From] = msg2.Value;
                }
                var count2 = Interlocked.Decrement(ref CompletionCount);
                if (count2 == 0) Completion.SetResult(true);
            }
        }

    
    void CreateEIGTree(ByzProcess[] Byzantines)
    {
        for(int i = 0; i < Byzantines.Length; i++)
            {
                var temp = Byzantines[i];

                for (int j = 0; j < Byzantines.Length; j++)
                {
                       

                }
            }
    }
    static ByzProcess[] readGenerals(string[] generals)
    {
       // int NumberOfNodes;
        int VZero;
        ByzProcess[] Byzantines;

        string[] temp = generals[0].Split(' ');
        N = Int32.Parse(temp[0]);
        VZero = Int32.Parse(temp[1]);
        Byzantines = new ByzProcess[N];

            for (int i = 0; i < generals.Length-1; i++)
            {
            ByzProcess Byz = null;
           
                temp = generals[i+1].Split(' ');
                int id = Int32.Parse(temp[0]);
                int init = Int32.Parse(temp[1]);
                
                bool Faulty;

                if (Int32.Parse(temp[2]) == 1)
                {
                    Faulty = true;
                    //Since true read the script
                    //Expecting N + N strings, with second part being strings of length N-1
                    string[] script = new string[N * 2];
                    for (int j = 2; j < N * 2; j++)
                    {
                        //For creating string[] from script
                        script[j - 2] = temp[j];
                    }
                    Byzantines[i] = new ByzProcess(id, init, true, script);
                }
                else
                {
                    Faulty = false;
                    //Not faulty, therefore do not need to create script
                    Byzantines[i] = new ByzProcess(id, init);
                }


            
           if (Byz != null)
            {
                    Byzantines[i] = Byz;
            }

        }
        var SortedByzantines = Byzantines.OrderBy(x => x.ID).ToArray();
        return SortedByzantines;
    }

    static void Main(string[] args)
    {

        if (args.Length == 1)
        {
            string path = args[0];
            string[] content = System.IO.File.ReadAllLines(path);
            ByzProcess[] generals = readGenerals(content);
            Console.Out.WriteLine("The first byzantine is: " + generals[0].ID);
            CompletionCount = N = generals.Length;
            Completion = new TaskCompletionSource<bool>();
            Byz = new MailboxProcessor<Message>[N];
            
            for(int i = 0; i < N; i++)
            {
                var bp = generals[i];
                Byz[i] = new MailboxProcessor<Message>(bp.ByzBody);
            }
            for(int i = 0; i < N; i++)
                {
                    Byz[i].Start();
                    Thread.Sleep(100);
                }

            Completion.Task.Wait();
                Console.WriteLine("Finished first round messaging");
            Byz2 = new MailboxProcessor<Message2>[N];
                for (int i = 0; i < N; i++)
                {
                    var bp = generals[i];
                    Byz2[i] = new MailboxProcessor<Message2>(bp.ByzBody2);
                }
                for (int i = 0; i < N; i++)
                {
                    Byz2[i].Start();
                    Thread.Sleep(100);
                }
               
                
                Completion.Task.Wait();
                Console.WriteLine("Finished second round messaging");
               // Console.Read();
        }
        else
        {
            Console.Error.WriteLine("For some reason there was more than one command line argument");
        }
    }

    }
}
