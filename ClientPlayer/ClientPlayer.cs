using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using ForestInhabitant;
using ForestSolverPackages;

namespace ClientPlayer
{
    class ClientPlayer
    {
        private static int Width { get; set; }
        private static int Height { get; set; }
        static void Main(string[] args)
        {
            //Console.WriteLine(Encoding.UTF8.GetBytes("S").Length);
            //return;
            try
            {
                var port = 11000;
                //SendMessageFromSocket(11000);
                var ipHost = Dns.GetHostEntry("localhost");
                var ipAddr = ipHost.AddressList[0];
                var ipEndPoint = new IPEndPoint(ipAddr, port);
                var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // Буфер для входящих данных
                var buffer = new byte[8192];

                // Соединяемся с удаленным устройством

                // Устанавливаем удаленную точку для сокета

                // Соединяем сокет с удаленной точкой
                sender.Connect(ipEndPoint);
                //Console.Write("Введите сообщение: ");
                var helo = new Hello() { IsVisualizator = false, Name = "PLAYER" };
                sender.Send(Serializer.Serialize(helo));
                sender.Receive(buffer);
                var clientInfo = Serializer.Deserialize<ClientInfo>(buffer);
                Width = clientInfo.MapSize.X;
                Height = clientInfo.MapSize.Y;
                Move move = new Move();
                //Console.WriteLine("HERE");
                var dirs = new Dictionary<Point, int>
                {
                    {new Point(0, -1), 0},
                    {new Point(1, 0), 1},
                    {new Point(0, 1), 2},
                    {new Point(-1, 0), 3}
                };
                var ai = new Ai(clientInfo.StartPosition, clientInfo.Target, Width, Height, clientInfo.Hp);
                //while (clientInfo.Target != ai.Location)
                while (true)
                {
                    var direction = ai.GetNextMove();
                    move.Direction = dirs[direction];
                    //Console.WriteLine(move.Direction);
                    sender.Send(Serializer.Serialize(move));
                    sender.Receive(buffer);
                    var resultInfo = Serializer.Deserialize<MoveResultInfo>(buffer);
                    var destination = new Point(ai.Location.X + direction.X, ai.Location.Y + direction.Y);
                    if (resultInfo.Result == 1)
                    {
                        ai.Location = destination;
                        ai.Map[ai.Location.X, ai.Location.Y] = CellTypes.Empty;
                    }
                    else if (resultInfo.Result == 0)
                    {
                        ai.Map[destination.X, destination.Y] = CellTypes.Wall;
                    }
                    else
                    {
                        Console.WriteLine("DONE!");
                        break;
                    }
                }
                
                //var worldInfo = Serializer.Deserialize<WorldInfo>(buffer);
                //VisualizeMap(worldInfo);
                //Width = worldInfo.Map.GetLength(0);
                //Height = worldInfo.Map.GetLength(1);
                //var ans = new Answer() { AnswerCode = 0 };
                //sender.Send(Serializer.Serialize(ans));
                //while (true)
                //{
                //    sender.Receive(buffer);
                //    worldInfo = Serializer.Deserialize<WorldInfo>(buffer);
                //    VisualizeMap(worldInfo);
                //    sender.Send(Serializer.Serialize(ans));
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
