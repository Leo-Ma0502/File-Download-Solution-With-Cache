using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace FileServer;

public partial class FileServer : Form
{
    public FileServer()
    {
        InitializeComponent();
        main();
    }
    public void main()
    {
        // IP Address to listen on. Loopback is the localhost
        IPAddress ipAddr = IPAddress.Loopback;

        // Port to listen on
        int port = 8082;

        // Create and start a listener for client connection
        TcpListener listener = new(ipAddr, port);
        listener.Start();
        /*        Dictionary<int, string> resFromCache = cacheServer.FetchFile();
                foreach (KeyValuePair<int, string> kvp in resFromCache)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
                Console.WriteLine("Server starting !");*/
        /*Cache cacheServer = new();
        cacheServer.HandleRequest();*/
        // keep running
        while (true)
        {
            Console.WriteLine("Server listening on: {0}:{1} at {2}", ipAddr, port, DateTime.Now.TimeOfDay);
            var client = listener.AcceptTcpClient();
            Console.WriteLine("cache connected at {0}", DateTime.Now.TimeOfDay);

            // NetworkStream object is used for passing data between client and server
            NetworkStream stream = client.GetStream();

            // For image data
            MemoryStream stream_image = new();

            // read the first byte that represents the command of the client
            byte command = (byte)stream.ReadByte();

            // 0 is for a greeting message
            if (command == 0)
            {
                // Create a StreamWriter object to write the message to the stream using UTF-8 encoding
                StreamWriter writer = new(stream, Encoding.UTF8);

                // Write a string to the stream
                string greeting = "Hello, client!";
                writer.Write(greeting);

                // Flush the StreamWriter to ensure that all data is written to the stream
                writer.Flush();
            }
            else  // command for a image file
            {
                Console.WriteLine("waiting");
                /*Cache cacheServer = new();
                string URL = cacheServer.FetchFile();

                // StreamWriter object is used to send data to the client
                StreamWriter writer = new(stream);

                using (Image image = Image.FromFile(URL))
                {
                    image.Save(stream_image, image.RawFormat);
                    // Convert the image to a byte array, then to BASE64 code
                    string base64 = Convert.ToBase64String(stream_image.ToArray());
                    try
                    {
                        // Write the line of text to the network stream
                        writer.Write(base64);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                writer.Flush(); // ask the system send the data now
                writer.Close();
                stream.Close();*/
            }
            client.Close();
        }
    }
}
