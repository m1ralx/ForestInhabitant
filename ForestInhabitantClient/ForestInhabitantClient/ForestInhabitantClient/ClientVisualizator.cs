using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ForestInhabitant;
using ForestSolverPackages;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ForestInhabitantClient
{
    class Client
    {
        private static int Width { get; set; }
        private static int Height { get; set; }
        //private static int Vi
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
                var helo = new Hello() { IsVisualizator = true, Name = "first" };
                sender.Send(Serializer.Serialize(helo));
                sender.Receive(buffer);
                var worldInfo = Serializer.Deserialize<WorldInfo>(buffer);
                VisualizeMap(worldInfo);
                Width = worldInfo.Map.GetLength(0);
                Height = worldInfo.Map.GetLength(1);
                var ans = new Answer() {AnswerCode = 0};
                sender.Send(Serializer.Serialize(ans));
                while (true)
                {
                    sender.Receive(buffer);
                    worldInfo = Serializer.Deserialize<WorldInfo>(buffer);
                    VisualizeMap(worldInfo);
                    sender.Send(Serializer.Serialize(ans));
                    //if (!worldInfo.Players.Any())
                        //return;
                }
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

        private static void VisualizeMap(WorldInfo worldInfo)
        {
            Console.Clear();
            for (var y = 0; y < worldInfo.Map.GetLength(1); y++)
            {
                for (var x = 0; x < worldInfo.Map.GetLength(0); x++)
                {
                    Console.Write(worldInfo.Map[x, y]);
                }
                Console.WriteLine();
            }
        }

        private static int ConvertBytes2Int(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }
        static void SendMessageFromSocket(int port)
        {
            var helo = new Hello() {IsVisualizator = true, Name = "first"};
            // Буфер для входящих данных
            var bytes = new byte[8192];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            var ipHost = Dns.GetHostEntry("localhost");
            var ipAddr = ipHost.AddressList[0];
            var ipEndPoint = new IPEndPoint(ipAddr, port);

            var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);
            Console.Write("Введите сообщение: ");
            var message = Console.ReadLine();

            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            var msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            var bytesSent = sender.Send(msg);
            if (message != "UPD")
            {
                // Получаем ответ от сервера
                var bytesRec = sender.Receive(bytes);

                Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));
            }
            else
            {
                var bytesRec = sender.Receive(bytes);
                var widthBytes = bytes.Take(4).ToArray();
                var heightBytes = bytes.Skip(4).Take(4).ToArray();
                var width = ConvertBytes2Int(widthBytes);
                var heigth = ConvertBytes2Int(heightBytes);
                Console.Clear();
                Console.WriteLine("Width: {0} Height: {1}", width, heigth);
                var strMap = Encoding.UTF8.GetString(bytes.Skip(8).ToArray());
                Console.WriteLine(strMap.Length);
                for (var i = 0; i < width * heigth; i++)
                {
                    Console.Write(strMap[i]);
                    if ((i + 1) % width == 0)
                        Console.WriteLine();
                }
                
            }

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            if (message.IndexOf("<TheEnd>") == -1)
                SendMessageFromSocket(port);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
