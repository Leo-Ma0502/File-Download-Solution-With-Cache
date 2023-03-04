using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

class CacheServer
{
    static Dictionary<string, string> cache = new Dictionary<string, string>();

    static void Main(string[] args)
    {
        // Create a TCP/IP socket
        var listener = new TcpListener(IPAddress.Any, 1234);
        listener.Start();

        Console.WriteLine("Cache server running...");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");

            var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytes;

            while ((bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                var data = Encoding.ASCII.GetString(buffer, 0, bytes);
                Console.WriteLine("Received: {0}", data);

                // Parse the incoming request
                var parts = data.Split(' ');
                var command = parts[0];
                var key = parts[1];

                // Handle the command
                string response;
                switch (command)
                {
                    case "GET":
                        if (cache.ContainsKey(key))
                        {
                            response = cache[key];
                        }
                        else
                        {
                            response = "Key not found";
                        }
                        break;
                    case "SET":
                        var value = parts[2];
                        cache[key] = value;
                        response = "OK";
                        break;
                    default:
                        response = "Invalid command";
                        break;
                }

                // Send the response back to the client
                var bytesToSend = Encoding.ASCII.GetBytes(response);
                stream.Write(bytesToSend, 0, bytesToSend.Length);
                Console.WriteLine("Sent: {0}", response);
            }

            client.Close();
            Console.WriteLine("Client disconnected");
        }
    }
}
