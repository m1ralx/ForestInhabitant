using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ForestInhabitant
{
    public class Server
    {
        //[STAThread]
        private static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            var moves = new Dictionary<ConsoleKey, Point>
            {
                {ConsoleKey.LeftArrow, new Point(-1, 0)},
                {ConsoleKey.RightArrow, new Point(1, 0)},
                {ConsoleKey.UpArrow, new Point(0, -1)},
                {ConsoleKey.DownArrow, new Point(0, 1)}
            };
            var forest = Loader.LoadForest(args[0]);

            IVisualizator visualisator = new SimpleConsoleVisualizator();
            forest.OnChangeMap += visualisator.Vizualize;
            var inhabitants = new List<Inhabitant>();

            //forest.SpawnNewInhabitant("goden", new Point(7, 2));
            //var goden = (Inhabitant)forest[new Point(7, 2)];
            //inhabitants.Add(goden);
            var server = new ThreadedServer(forest);
            server.Start();
            //Console.ReadLine();
            //forest.MoveInhabitant(new Point(0, 1), goden);
            //Console.ReadLine();
            //forest.MoveInhabitant(new Point(0, 1), goden);
            Console.ReadLine();
        }

        ///
            /// TCP Server
            /// 

        //    Socket sListener;
        //    var ipEndPoint = CreateServer(out sListener);
        //    RunServer(sListener, ipEndPoint, visualisator, forest);

        //    ///
        //    /// 
        //    /// 


        //    //forest.SpawnNewInhabitant("robot", new Point(7,6));
        //    //var robot = (Inhabitant) forest[new Point(7, 6)];
        //    //inhabitants.Add(robot);
        //    //NewGame(moves, forest, inhabitants);
        //    var dest = new Point(5, 4);
        //    //var dest = new Point(7, 4);
        //    //var dest = new Point(1, 1);
        //    var ai = new Ai(goden, dest, forest.MoveInhabitant, forest.Width, forest.Height);
        //    //while (true)
        //    //{
        //    //    var location = goden.GetLocation();
        //    //    ai.MoveInhabitantToDestination();
        //    //    if (goden.GetLocation() == dest || goden.GetHitPoints() == 0)
        //    //        break;
        //    //    //if (location != goden.GetLocation())
        //    //    //    Console.ReadKey();
        //    //}
        //}

        //private static byte[] ConvertInt2Bytes(int intValue)
        //{
        //    byte[] intBytes = BitConverter.GetBytes(intValue);
        //    if (BitConverter.IsLittleEndian)
        //        Array.Reverse(intBytes);
        //    byte[] result = intBytes;
        //    return result;
        //}
        
        //private static byte[] GetMap(IVisualizator visualizator, IForest forest)
        //{
        //    Point p;
        //    var ans = new List<byte>();
        //    ans.AddRange(ConvertInt2Bytes(forest.Width));
        //    ans.AddRange(ConvertInt2Bytes(forest.Height));
        //    for (var y = 0; y < forest.Height; y++)
        //    {
        //        for (var x = 0; x < forest.Width; x++)
        //        {
        //            p = new Point(x, y);
        //            ans.AddRange(Encoding.UTF8.GetBytes(forest[p].GetSymbol()));
        //        }
        //    }
        //    return ans.ToArray();
        //}
        //private static void RunServer(Socket sListener, IPEndPoint ipEndPoint, IVisualizator visualizator, IForest forest)
        //{
        //    try
        //    {
        //        sListener.Bind(ipEndPoint);
        //        sListener.Listen(10);

        //        /*
        //         * Сделать так, чтобы сервер просто отсылал всем визуализаторам данные, когда изменяется,
        //         * а визуализаторы уже отрисовывают
        //         */
        //        while (true)
        //        {
        //            Console.WriteLine("Waiting connection to port {0}", ipEndPoint);
        //            Socket handler = sListener.Accept();
                    
        //            if (HandleClient(visualizator, forest, handler)) continue;
        //            else break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //    finally
        //    {
        //        Console.ReadLine();
        //    }
        //}

        //private static bool HandleClient(IVisualizator visualizator, IForest forest, Socket handler)
        //{
        //    string data = null;
        //    byte[] bytes = new byte[1024];
        //    int bytesRec = handler.Receive(bytes);
        //    data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
        //    Console.WriteLine("Recieved {0}\n\n", data);
        //    if (data == "UPD")
        //    {
        //        handler.Send(GetMap(visualizator, forest));
        //        return true;
        //    }
        //    string reply = "Thank you for request in " + data.Length.ToString() + " symbols";
        //    byte[] msg = Encoding.UTF8.GetBytes(reply);
        //    handler.Send(msg);
        //    handler.Shutdown(SocketShutdown.Both);
        //    handler.Close();
        //    if (data.IndexOf("<TheEnd>", StringComparison.Ordinal) > -1)
        //    {
        //        Console.WriteLine("Session ended");
        //        return true;
        //    }
        //    return false;
        //}

        //private static IPEndPoint CreateServer(out Socket sListener)
        //{
        //    IPHostEntry ipHost = Dns.GetHostEntry("localhost");
        //    IPAddress ipAddress = ipHost.AddressList[0];
        //    IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 11000);
        //    sListener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //    return ipEndPoint;
        //}

        private static void NewGame(Dictionary<ConsoleKey, Point> moves, IForest forest, List<Inhabitant> inhabitants)
        {
            while (forest.HasAliveInhabitants)
            {
                var currentInhabitant = inhabitants.First(inh => inh.GetHitPoints() > 0);
                var inputKey = Console.ReadKey().Key;
                try
                {
                    forest.MoveInhabitant(moves[inputKey], currentInhabitant);
                }
                catch
                {
                    var exit = new Dictionary<ConsoleKey, Action<int>>{
                        { ConsoleKey.Escape, System.Environment.Exit},
                    };
                    try
                    {
                        exit[inputKey](0);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
