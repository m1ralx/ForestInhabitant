using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ForestInhabitant;
using ForestSolverPackages;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Web.Script.Serialization;


class ThreadedServer
{
    private Socket _serverSocket;
    private readonly int _port;
    private readonly IForest _forest;
    private readonly List<Player> _players;
    private readonly List<Inhabitant> _inhabitants;
    private Settings settings;

    public ThreadedServer(IForest forest) 
    { 
        //_port = port;
        _forest = forest;
        _players = new List<Player>();
        _inhabitants = new List<Inhabitant>();
        settings = new Settings();
        //settings.Port = 11000;
        //settings.StartPoint = new Point(7, 1);
        //settings.TargetPoint = new Point(5, 6);
        //settings.StartHp = 3;
        JavaScriptSerializer s = new JavaScriptSerializer();
        //string res = s.Serialize(settings);
        //Console.WriteLine(res);
        //File.WriteAllText("config.conf", res);
        //Console.WriteLine("sdsdsdsd");
        //Console.ReadLine();
        string conf = File.ReadAllText("config.conf");
        settings = s.Deserialize<Settings>(conf);
        _port = settings.Port;
    }

    private class ConnectionInfo
    {
        public Socket Socket;
        public Thread Thread;
    }

    private Thread _acceptThread;
    private readonly List<ConnectionInfo> _connections =
        new List<ConnectionInfo>();

    public void Start()
    {
        SetupServerSocket();
        _acceptThread = new Thread(AcceptConnections) {IsBackground = true};
        _acceptThread.Start();
    }

    private void SetupServerSocket()
    {
        // Получаем информацию о локальном компьютере
        var localMachineInfo =
            Dns.GetHostEntry("localhost");
        var ipAddr = localMachineInfo.AddressList[0];
        var myEndpoint = new IPEndPoint(ipAddr, _port);

        // Создаем сокет, привязываем его к адресу
        // и начинаем прослушивание
        _serverSocket = new Socket(
            myEndpoint.Address.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        _serverSocket.Bind(myEndpoint);
        _serverSocket.Listen((int)
            SocketOptionName.MaxConnections);
    }

    private void AcceptConnections()
    {
        while (true)
        {
            // Принимаем соединение
            var socket = _serverSocket.Accept();
            var connection = new ConnectionInfo
            {
                Socket = socket,
                Thread = new Thread(ProcessConnection) {IsBackground = true}
            };

            // Создаем поток для получения данных
            connection.Thread.Start(connection);

            // Сохраняем сокет
            lock (_connections) 
                _connections.Add(connection);
        }
    }

    private WorldInfo ConstructWorldInfoPacket()
    {
        var map = new string[_forest.Width, _forest.Height];
        var forestMap = _forest.GetMap();
        for (var x = 0; x < _forest.Width; x++)
            for (var y = 0; y < _forest.Height; y++)
            {
                map[x, y] = forestMap[x, y].GetSymbol();
            }
        return new WorldInfo() {Map = map, Players = _players.ToArray()};
    }

    private string[,] GetVisibleMap(Player player, int radius)
    {
        var visibleMap = new string[2*radius + 1, 2*radius + 1];
        //var forestMap = _forest.GetMap();
        //var location = _inhabitants[player.Id].GetLocation();
        //for (var x = 0; x < forestMap.GetLength(0); x++)
        //    for (var y = 0; y < forestMap.GetLength(1); y++)
        //        if (Math.Abs(x - location.X) <= radius && Math.Abs(x - location.Y) <= radius)
        //            try
        //            {
        //                var x0 = x - location.X;
        //                visibleMap[x - location.X, y - location.Y] = forestMap[x, y].GetSymbol();
        //            }
        //            catch (Exception)
        //            {
        //                Console.WriteLine("Coord: {0} {1}", x, y);
        //                Console.WriteLine("Location: " + location.ToString());
        //                Console.ReadKey();
        //            }
        return visibleMap;
    }

    private void ProcessConnection(object state)
    {
        var connection = (ConnectionInfo)state;
        var buffer = new byte[8192];
        try
        {
            var bytesRead = connection.Socket.Receive(buffer);
            var hello = Serializer.Deserialize<Hello>(buffer);
            if (hello.IsVisualizator)
            {
                var worldInfo = ConstructWorldInfoPacket();
                connection.Socket.Send(Serializer.Serialize(worldInfo));
                buffer = new byte[8192];
                connection.Socket.Receive(buffer);
                var answer = Serializer.Deserialize<Answer>(buffer);

                while (answer.AnswerCode == 0)
                {
                    connection.Socket.Send(Serializer.Serialize(ConstructWorldInfoPacket()));
                    connection.Socket.Receive(buffer);
                    answer = Serializer.Deserialize<Answer>(buffer);
                }
            }
            else
            {
                var player = new Player(_players.Count, hello.Name, settings.StartPoint, settings.TargetPoint, settings.StartHp);
                var playerInhabitant = _forest.SpawnNewInhabitant(
                    player.Nick, 
                    new Point(player.StartPosition.X, player.StartPosition.Y), settings.StartHp);
                _players.Add(player);
                _inhabitants.Add(playerInhabitant);
                var clientInfo = new ClientInfo()
                {
                    Hp = player.Hp,
                    MapSize = new Point(_forest.Width, _forest.Height),
                    StartPosition = settings.StartPoint,
                    Target = settings.TargetPoint,
                    VisibleMap = GetVisibleMap(player, 0)
                };
                connection.Socket.Send(Serializer.Serialize(clientInfo));
                var dirs = new Dictionary<int, Point>
                {
                    {0, new Point(0, -1)},
                    {1, new Point(1, 0)},
                    {2, new Point(0, 1)},
                    {3, new Point(-1, 0)}
                };
                while (true)
                {
                    connection.Socket.Receive(buffer);
                    var move = Serializer.Deserialize<Move>(buffer);
                    //Console.WriteLine(move.Direction);
                    int result = Convert.ToInt32(_forest.MoveInhabitant(dirs[move.Direction], playerInhabitant));
                    MoveResultInfo moveResultInfo = new MoveResultInfo()
                    {
                        Result = Convert.ToInt32(result),
                        VisibleMap = GetVisibleMap(player, 0)
                    };
                    if (playerInhabitant.GetHitPoints() == 0 || playerInhabitant.GetLocation() == clientInfo.Target)
                    {
                        result = 2;
                        //_forest.RemoveInhabitant(playerInhabitant.GetLocation(), player.Nick);
                        _inhabitants.RemoveAt(player.Id);
                        _players.RemoveAt(player.Id);
                        moveResultInfo.Result = result;
                        connection.Socket.Send(Serializer.Serialize(moveResultInfo));
                        connection.Socket.Close();
                        return;
                    }
                    //MoveResultInfo moveResultInfo = new MoveResultInfo()
                    //{
                    //    Result = Convert.ToInt32(result),
                    //    VisibleMap = GetVisibleMap(player, 0)
                    //};
                    connection.Socket.Send(Serializer.Serialize(moveResultInfo));
                    //Console.ReadKey();
                }
            }

            //while (true)
            //{
            //    connection.Socket.Receive(
            //        buffer);
            //    if (bytesRead > 0)
            //    {
            //        //Serializer.Deserialize<>()
            //        //var ms = new MemoryStream(buffer);
            //        //using (var reader = new BsonReader(ms))
            //        //{
            //        //    var serializer = new JsonSerializer();
            //        //    var hello = serializer.Deserialize<Hello>(reader);
            //        //    Console.WriteLine("DESERIALIZED: {0} {1}", hello.Name, hello.IsVisualizator);
            //        //    if (hello.IsVisualizator)
            //        //    {
            //        //        // Send WorldInfo
            //        //        var worldInfo = ConstructWorldInfoPacket();

            //        //    }
            //        //}
            //        connection.Socket.Send(Encoding.UTF8.GetBytes("Okay"));
            //        lock (_connections)
            //        {
            //            foreach (var conn in _connections.Where(conn => conn != connection))
            //            {
            //                conn.Socket.Send(
            //                    buffer, bytesRead,
            //                    SocketFlags.None);
            //            }
            //        }
            //    }
            //    else if (bytesRead == 0) return;
            //}
        }
        catch (SocketException exc)
        {
            Console.WriteLine("Socket exception: " +
                exc.SocketErrorCode);
        }
        catch (Exception exc)
        {
            Console.WriteLine("Exception: " + exc);
        }
        finally
        {
            connection.Socket.Close();
            lock (_connections) _connections.Remove(
                connection);
        }
    }
}


