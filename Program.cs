using Actress;
using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ByzantineAgreementSync.Program;
using System.Collections;
using System.Collections.ObjectModel;
//TODO Implement tree structure
namespace ByzantineAgreementSync
{
    class Program
    {
        public class Node<T>
        {
            // Private member-variables
            private T data;
             NodeList<T> neighbors = null;
            public String id { get; set; }
            public List<Node<T>> children
            {
                get; set;
            }
            Node<T> parent;
            public Node() { }
            public Node(T data, String ID)
            {
                children = new List<Node<T>>();
                this.data = data;
                this.id = ID;
            }
            public Node(Node<T> parent, T data)
            {
                this.parent = parent;
                this.data = data;
                children = new List<Node<T>>();
            }
            public Node(T data, List<Node<T>> children)
            {
                this.data = data;
                this.children = children;
            }

            public T Value
            {
                get
                {
                    return data;
                }
                set
                {
                    data = value;
                }
            }
            public void AddChild(Node<T> child)
            {
                children.Add(child);
                child.parent = this;
                
            }
            

           
        }
    public class NodeList<T> : List<Node<T>>
    {
        List<Node<T>> List;
        public NodeList()
        {
                List = new List<Node<T>>();
        }

        public NodeList(int initialSize)
        {
                // Add the specified number of items
            List = new List<Node<T>>(initialSize);
            
        }
        public void Add(Node<T> child)
        {
                this.List.Add(child);
        }

       
    }
    //    public class Tree<T> : Node<T>
    //    {
     //       private Node<T> root;
     //       public NodeList<T> children;
     //       public Tree(T value)
     //       {
     //           root = new Node<T>(value);
      //      }
          //  Node<T> getNode(int depth, )
           // {
                //depth--;
             //   foreach(Node<T> child in children){

               // }
               // getNode(depth);
            //}

        //}
    public class Message
        {
            public Message(int from, String value) { From = from; Value = value; }
            public int From { get; private set; }
            public String Value { get; private set; }
        }
        public class Message2 
        {
            //public Message2(int from, int value) { From = from; Value = value; }
       
            public Message2(int from, int[] value) { From = from; Value = value; }
            public int From { get; private set; }
            public int[] Value { get; private set; }
        }
        public static MailboxProcessor<Message>[] Byz;
       // public static MailboxProcessor<Message2>[] Byz2;
        public static int NumberOfRounds;
        public static int round;
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
                EIG = new Node<int>(Init, "");
                var temp = N;
               
            }
            public ByzProcess(int Id, int init, bool ByzFaulty, string[] ByzScript)
            {
                ID = Id;
                Init = init;

                faulty = ByzFaulty;
                script = ByzScript;
                
                var temp = N;
                EIG = new Node<int>(Init, "");

            }
            public int ID { get; private set; }
            public int Init { get; private set;}
            public bool faulty { get; private set; }
            public string[] script { get; private set; }
            public Node<int> EIG { get; private set; }

            public int getValue(Node<int> node, int depth)
            {
                //Get the required values from the EIG tree, essentially, get the next NodeList Values
                //Using recursion
                //round for depth
                
                 // concatenate all values together, then return this to send the message
                foreach(Node<int> child in node.children )
                {
                    Console.WriteLine("Child "+child.ToString());
                    if(depth == round)
                    {
                        return child.Value;
                    }
                    else
                    {
                        getValue(child, depth++);
                    }
                }
                return 70;
            }
            public String getLevel(int level, Node<int> node, String result)
            {
                if (level != 1) {
                    //Console.WriteLine("Before loop");
                    
                    foreach (Node<int> child in node.children)
                    {
                       // Console.WriteLine("At loop: " + i++);
                        result += getLevel(level-1, child, "");
                    }
                }
                else
                {
                    return node.Value.ToString();
                }
                return result;
            }
            public Node<int> GetLeaves(Node<int> parent)
            {
                if(parent.children.Count == 0)
                {
                    return parent;
                }
                else
                {
                    foreach(var child in parent.children)
                    {
                        GetLeaves(child);
                    }
                   
                }
                return null;
            }
            public void FindLeaves(Node<int> parent, List<Node<int>> leaves)
            {
                if (parent.children.Count != 0)
                {
                    foreach (var child in parent.children)
                    {
                       FindLeaves(child, leaves);
                    }
                }
                else
                {
                    if (!leaves.Contains(parent))
                    {
                        leaves.Add(parent);
                    }
                }
            }
            public void GetLeavesFromSubTree(int ID, List<Node<int>> leaves)
            {
                
            }



            public async Task ByzBody (MailboxProcessor<Message> inbox)
            {
                
                for(var i=0; i < N; i++)
                {
                    //    if(round == 1)
                    //    {
                    //var msg = new Message(ID, Init+"");
                   // Console.WriteLine("Before getting level: " + i);
                    var mess = String.Concat(getLevel(round, this.EIG, ""));
                   

                   // var mes = mess.Append(getLevel(round, this.EIG)).ToString();
                   // Console.WriteLine("Message value: " + mess);
                    var msg = new Message(ID, mess);
                        Byz[i].Post(msg);
                        Console.WriteLine(round + " [" + ID + "]: Posting: " + msg.Value + " to " + (i + 1));
                  //  }
                  //  else
                  //  {
                  //      Console.WriteLine("WE HUR");
                  //      var mes = getValue(EIG, 0);
                  //
                  //     var msg = new Message(ID, "");
                  //     Byz[i].Post(msg);
                  //      Console.WriteLine(round + " [" + ID + "]: Posting: " + msg.Value + " to " + (i + 1));
                  //  }
                }
                // Console.WriteLine("BeforeBefore receive: " + ID + " ,  " );
                //Buffer for storing messages, ensureing tree is built in order
                List<Message> buffer = new List<Message>();
                var leaves = new List<Node<int>>();

                FindLeaves(EIG, leaves);
                var leavesQueue = new Queue<Node<int>>(leaves);
                for (var i = 0; i < N; i++)
                {
                    Console.WriteLine("Before receive: " + ID + " ,  " + i);
                    var msg = await inbox.Receive();
                    Console.WriteLine(round + ": Receiving[" + ID + "]: Value: " + msg.Value + ", From: " + msg.From);
                    var values = msg.Value.ToCharArray();
                    var receivedValues = values.Select(x => x.ToString()).ToList<String>();
                    
                    
                     
                   
                    if (round == 1)
                    { 
                    foreach (var value in receivedValues)
                        {
                            var temp = Convert.ToInt32(value);
                            this.EIG.AddChild(new Node<int>(temp, msg.From.ToString()));
                            // Console.WriteLine("Child added");
                            // Console.WriteLine("Successfully adding to tree");
                        }
                    }
                    else
                    {
                        //Need to remove the value that is from the node that is being stored
                        // Each leaf should not get the value from the node they represent
                        //E.G., if Node 1 is sending 0011 (where N=4), then the first leaf should
                        //have 011.
                        //If Node 3, sends 0011, then each third leaf will store 001
                        var storedValues = receivedValues.ToArray();
                        for (int j = msg.From-1; j < receivedValues.Count; j+=N)
                        {
                            storedValues[j] = null;
                        }
                        storedValues = storedValues.Where(x => x != null).ToArray<String>();
                        //Now stored values does not include those from the ID
                        // Console.WriteLine("Values to be stored: ");
                        // foreach(var x in storedValues) { Console.WriteLine(x + " ,  "); }
                        //problem with adding, for some reason not adding into final leaf
                        //Change trying, (msg.From*(round-1)*(N-1)-> (msg.From*(round-1)*(N)

                        //var leafQueue = new Queue<Node<int>>(leaves);
                        //msg.Value is 0011 or something
                        
                        for(int j = 0; j < storedValues.Length; j++)
                        {
                            var leaf = leavesQueue.Dequeue();
                            for(int k = 0; k < N-1; k++)
                            {
                                leaf.AddChild(new Node<int>(Convert.ToInt32(storedValues[j]), leaf.id +"."+msg.From));
                            }
                        }


                        //var leaves = new List<Node<int>>();
                        //leaves.Add(GetLeaves(EIG));
                        // (int r = ((msg.From-1 * (round - 1) * (N)) - (N)); r < (msg.From * (round - 1) * (N)); r++)
                        //

                      /*  for (int r = ((msg.From * (round - 1) * (N)) - (N)); r < (msg.From * (round - 1) * (N)); r++)
                        {
                            for (int c = 0; c < N-1; c++)
                            {
                               
                                leaves[r].AddChild(new Node<int>(Convert.ToInt32(storedValues[c]), leaves[r].id + "."+ c.ToString()));
                               
                                //Console.WriteLine("Child added at: r={0}, c={1}, stopping val:{2} ", r, c, (msg.From * (round - 1) * (N)));
 
                            }
                           // strValues.RemoveRange(0, N - 1);
                        }*/
                    
                    }

                    //Console.WriteLine("The number of children in tree: " + this.EIG.children.Count );
                    //Console.WriteLine("Finished adding to tree");
                    //if (msg.From > 2) Console.WriteLine("3  and 4 are making it through");
                   // EIG.Add(msg.From, msg.Value);
                    
                    //TODO put into tree-- - - - - 
                    //Receive message and put first round values into first row
                    //  if(msg.Value.Length > 1)
                    //  {
                    //   var temp = msg.Value;
                    //  var split = temp.Split(' ');
                    //String[] values = split.Select(x => x.ToString()).ToArray<String>();
                    //Figure out how to insert values into array at these points
                    //Array is of length N(N-1)
                    //insert N-1 values N times
                    //  EIG[round][msg.From-1] = split;
                    //  }
                    //  else
                    //  {
                    //First round
                    //EIG.Add(new TreeNode(round +"."+ msg.From +"."+ msg.Value));
                    //Console.WriteLine(EIG[round][msg.From - 1]);
                   // }
                    
                }
                //This is the synchronizer, start round two afterwards?

                var count = Interlocked.Decrement(ref CompletionCount);
                if (count == 0) Completion.SetResult(true);         
            }

            /*  public async Task ByzBody2(MailboxProcessor<Message2> inbox)
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
              */
            
        }

    void Evaluation(ByzProcess[] generals)
        {

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
            NumberOfRounds = (N / 3) + 1;
            Completion = new TaskCompletionSource<bool>();
            
            round = 1;
            while (round <= NumberOfRounds) { 
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
            Console.WriteLine("Finished "+round+ " round messaging");
            Thread.Sleep(1000);

                    /*    Byz2 = new MailboxProcessor<Message2>[N];
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
                            Console.WriteLine("Finished second round messaging");*/
                    round++;
                }
                Console.Read();
            }
            else
        {
            Console.Error.WriteLine("For some reason there was more than one command line argument");
        }
    }

    }
}
