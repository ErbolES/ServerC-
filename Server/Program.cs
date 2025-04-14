using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // создания серверного сокета для подключения клиентов
            Socket socServer = new Socket(AddressFamily.InterNetwork,
              SocketType.Stream, ProtocolType.Tcp);

            // 127.0.0.1 - localhost, локальный хост, локальный компьютер
            // 0.0.0.0 - any, любой хост, любой компьютер
            IPEndPoint srvEndPoint =
                     new IPEndPoint(IPAddress.Parse("0.0.0.0"), 12345);

            socServer.Bind(srvEndPoint); // связать сокет с IP-адресом сервера

            socServer.Listen(100); // начать прослушивать сокет сервера

            while (true)
            {
                // ждать подключения клиента
                Console.WriteLine("Wait of client for connect...");
                Socket client = socServer.Accept();

                // удаленный клиент подключился
                Console.WriteLine("Client is connected");
                Console.WriteLine($"Address of client : {client.RemoteEndPoint}"); // 192.168.1.4:45674

                // взаимодействие с клиентом на удаленной машине
                // 1 - отправить приветствие клиенту
                string msg = "Hello!";
                client.Send(Encoding.UTF8.GetBytes(msg));
                // 2 - получить ответ от клиента
                byte[] buffer = new byte[1024];
                int size = client.Receive(buffer);
                msg = Encoding.UTF8.GetString(buffer, 0, size);
                Console.WriteLine("Message from client : " + msg);

                while (true) 
                {
                    Console.WriteLine("Enter your command...");
                    string command = Console.ReadLine();

                    switch (command)
                    {
                        case "1": // refresh confid words

                            var refreshConfidWordsCommand = new
                            {
                                command = "refreshConfidWords",
                                patterns = new List<string> {
                                    "[0-9]{3}-[0-9]{2}-[0-9]{4}",
                                    "[0-9]{16}",
                                    "[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}",
                                    "пароль",
                                    "\\bpassword\\b",
                                    "конфиденциально"
                                }
                            };

                            client.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(refreshConfidWordsCommand)));

                            break;

                        case "2": // get status 
                            var getStatusCommand = new
                            {
                                command = "getStatus",
                                
                            };

                            client.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(getStatusCommand)));
                            break;

                        case "3":
                            var scan = new
                            {
                                command = "scan"
                            };
                            client.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(scan)));
                            break;
                        default:
                            break;
                    }

                };



                //client.Close();
                //while (true)
                //{ // цикл взаимодействия с клиентом
                //  // 3.1 - получить запрос от клиента
                //    size = client.Receive(buffer);
                //    msg = Encoding.UTF8.GetString(buffer, 0, size);
                //    Console.WriteLine("Message from client : " + msg);
                //    // 3.2 - отправить ответ клиенту
                //    client.Send(Encoding.UTF8.GetBytes("OK"));
                //    // проверка запроса от клиента на разрыв связи
                //    if (msg == "EXIT")
                //    { // завершения сеанса связи с клиентом
                //        client.Send(Encoding.UTF8.GetBytes("Good bye..."));
                //        client.Close();
                //        break;
                //    }
                //} // while(true) для Клиента

            } // while(true); - главный цикл сервера

        }
    }
}
