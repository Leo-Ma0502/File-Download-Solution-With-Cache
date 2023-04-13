using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CacheServer
{
    public class Cache
    {
        public void HandleRequest()
        {
            Console.WriteLine("Cache starting");
            // IP Address to listen on. Loopback is the localhost
            IPAddress ipAddr = IPAddress.Loopback;

            // port of client 
            int portC = 8081;

            // port of origin server
            int portO = 8082;

            // Create and start a listener for client connection
            TcpListener listenerC = new(ipAddr, portC);
            listenerC.Start();

            while(true)
            {
                Console.WriteLine("Server side cache listening on: {0}:{1} at {2}", ipAddr, portC, DateTime.Now.TimeOfDay);
                var clientC = listenerC.AcceptTcpClient();
                Console.WriteLine("client connected with cache server at {0}", DateTime.Now.TimeOfDay);

                // NetworkStream object is used for passing data between client and cache
                NetworkStream streamC = clientC.GetStream();

                // read the first byte that represents the command of the client
                byte command = (byte)streamC.ReadByte();

                if (command == 0)
                {               
                    // Write a string to the stream
                    /*string greeting = "Hello, client!";
                    writer2C.Write(greeting);*/

                    // forward greeting to origin server
                    TcpClient client2O = new(ipAddr.ToString(), portO);
                    using (NetworkStream stream2O = client2O.GetStream())
                    {
                        Console.WriteLine("send greeting at {0}", DateTime.Now.TimeOfDay);
                        stream2O.WriteByte(command);
                        stream2O.Flush();

                        // get greeting message from the server
                        Console.WriteLine("read response at {0}", DateTime.Now.TimeOfDay);
                        StreamReader reader4O = new(stream2O, Encoding.UTF8);
                        Console.WriteLine("got response at {0}", DateTime.Now.TimeOfDay);
                        string response = reader4O.ReadLine();
                        Console.WriteLine("Cache received string at {1}: {0}", response, DateTime.Now.TimeOfDay);

                        StreamWriter writer2C = new(streamC, Encoding.UTF8);
                        writer2C.Write(response);
                        Console.WriteLine("Cache sent to client at {1}: {0}", response, DateTime.Now.TimeOfDay);
                        writer2C.Flush();
                    }

                }
                else
                {
                    Console.WriteLine("waiting...");
                }
            }
        }
        /*public string HandleRequest()
        {
            *//*
             * TODO 
             * initiate a cache as a list of hash(fragment), 
             * each fragment sizing around 2048 Bytes;
             * forward whatever requested by the client to the origin server, 
             * caculate the hash of each block returned by the origin server,
             * transmit what's not in cache as block and what's in cache as hash;
             * 
             *//*
            string res;
            int port_server = 8082;
            // IP Address to listen on. Loopback is the localhost
            IPAddress ipAddr = IPAddress.Loopback;
            // Port to listen on
            int port = 8081;
            // Create and start a listener for client connection
            TcpListener listener = new(ipAddr, port);
            listener.Start();
            Console.WriteLine("Cache server listening on: {0}:{1}", ipAddr, port);
            var client = listener.AcceptTcpClient();
            Console.WriteLine("client connected with server side proxy");

            // NetworkStream object is used for passing data between client and server
            NetworkStream stream = client.GetStream();

            // For image data
            MemoryStream stream_image = new();

            // read the first byte that represents the command of the client
            byte command = (byte)stream.ReadByte();

            while (true)
            {
                try
                {
                    switch (command)
                    {
                        case 0:
                            {
                                // Create a StreamWriter object to write the message to the stream using UTF-8 encoding
                                StreamWriter writer = new(stream, Encoding.UTF8);
                                TcpClient client_proxy = new(ipAddr.ToString(), port_server);
                                using (NetworkStream streamWithServer = client_proxy.GetStream())
                                {
                                    streamWithServer.WriteByte(command);
                                    streamWithServer.Flush();

                                    // get greeting message from the server
                                    StreamReader reader = new(streamWithServer, Encoding.UTF8);
                                    string response = reader.ReadLine();
                                    writer.Write(response);
                                }
                                // Flush the StreamWriter to ensure that all data is written to the stream
                                writer.Flush();
                                client.Close();
                                continue;
                            }
                        case 1:
                            {
                                // the four bytes following the command is the number of bytes storing the file name
                                byte[] data = new byte[4];
                                stream.Read(data, 0, 4); // read bytes from the stream into the buffer

                                // find out the length of the file name and read the bytes representing the file name
                                int fileNameBytesLength = BitConverter.ToInt32(data, 0);
                                data = new byte[fileNameBytesLength];
                                stream.Read(data, 0, fileNameBytesLength);

                                // get the path to the file
                                string fileName = Encoding.UTF8.GetString(data);
                                string URL = string.Format(".\\asset\\{0}", fileName);
                                client.Close();
                                res = URL;
                                return res;
                            }
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                    res = error.Message;
                    return res;
                }
            }
        }*/
    }
}

