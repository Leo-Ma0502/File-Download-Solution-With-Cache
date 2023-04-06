using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace FileServer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            /*Application.Run(new FileServer());*/

            Console.WriteLine("Server starting !");

            // IP Address to listen on. Loopback is the localhost
            IPAddress ipAddr = IPAddress.Loopback;

            // Port to listen on
            int port = 8081;

            // Create and start a listener for client connection
            TcpListener listener = new TcpListener(ipAddr, port);
            listener.Start();

            Console.WriteLine("Server listening on: {0}:{1}", ipAddr, port);

            // keep running
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("clinet connected");

                // NetworkStream object is used for passing data between client and server
                NetworkStream stream = client.GetStream();

                // For image data
                MemoryStream stream_image = new MemoryStream();

                // read the first byte that represents the command of the client
                byte command = (byte)stream.ReadByte();

                // 0 is for a greeting message
                if (command == 0)
                {
                    // Create a StreamWriter object to write the message to the stream using UTF-8 encoding
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

                    // Write a string to the stream
                    string myString = "Hello, client!";
                    writer.Write(myString);

                    // Flush the StreamWriter to ensure that all data is written to the stream
                    writer.Flush();
                }
                else  // command for a text file
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
                    Console.WriteLine("received filename:" + fileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "asset");
                    Console.WriteLine("temp path1:" + path);
                    string fileNamePath = Path.Combine(path, fileName);
                    Console.WriteLine("temp path2:" + fileNamePath);

                    // StreamWriter object is used to send data to the client
                    StreamWriter writer = new StreamWriter(stream);

                    using (Image image = Image.FromFile(fileNamePath))
                    {
                        // Convert the image to a byte array
                        byte[] byteimage = stream_image.ToArray(); ;

                        while (byteimage != null)
                        {
                            // Write the line of text to the network stream
                            writer.Write(byteimage);
                        }
                    }
                
                    

                    // read the contents of the file and send them to the client
                    /*using (StreamReader reader = new StreamReader(fileNamePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                         {
                             // Write the line of text to the network stream
                             writer.WriteLine(line);
                        }
                     }*/

                    writer.Flush(); // ask the system send the data now
                    writer.Close();
                    stream.Close();
                }
               /* Application.Run(new FileServer());*/
                client.Close();
               
            }
            


        }
    }
}