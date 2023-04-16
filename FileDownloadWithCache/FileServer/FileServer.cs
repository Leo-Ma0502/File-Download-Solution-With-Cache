using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Buffers.Text;

namespace FileServer;

public partial class FileServer : Form
{
    public FileServer()
    {
        InitializeComponent();
        Main();
    }
    private void Main()
    {
        IPAddress ipAddr = IPAddress.Loopback;
        int port = 8081;
        TcpListener listener = new(ipAddr, port);
        listener.Start();

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
                //Cache cacheServer = new();
                //string URL = cacheServer.FetchFile();
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
                string URL = string.Format(".\\asset\\{0}", fileName);
                Console.WriteLine("url: " + URL);

                // StreamWriter object is used to send data to the client
                StreamWriter writer = new(stream);
                using (Image image = Image.FromFile(URL))
                {
                    image.Save(stream_image, image.RawFormat);
                    byte[] b1 = stream_image.ToArray();

                    // TODO send file size first
                    /*writer.Write("{0}", b1.Length);
                    writer.Flush();*/
                   /* Console.WriteLine("Sent total size");
                    byte followUp = (byte)stream.ReadByte();*/
                    /*if (followUp == 3)
                    {*/
                        try
                        {
                            var blocks = getBlocks(b1, 2, 3, 2048);
                            int lengthCount = 0;
                            for (int i = 0; i < blocks.Count; i++)
                            {
                                stream.Write(blocks[i], 0, blocks[i].Length);
                                stream.Flush();
                                lengthCount += blocks[i].Length;
                                Console.WriteLine("sent {0} block(s), total length {1}", i + 1, lengthCount);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    /*}*/
                }

                Console.WriteLine("sent all blocks as request");
               
                stream.Close();
            }
            client.Close();
        }
    }
    /*
     * get list of blocks of one file, takes the argument of the base64 code of the image, 
     * p is the prime chosen
     * max_size is the maximum size of each block
     */
    private double getRabin(byte[] b, int i, int p, int max_size)
    {
        // finger print value
        double res;
        try
        {
            res = (b[i - 2] * Math.Pow(p, 2) + b[i - 1] * p + b[i]) % max_size;
            return res;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return 0;
        }

    }
    private List<byte[]> getBlocks(byte[] b, int j, int p, int max_size)
    {
        List<byte[]> blocks = new();
        int start = 0; // start point for slicing
        for (int i = j; i < b.Length; i++)
        {        
            if (getRabin(b, i, p, max_size) == 0)
            {               
                int length = i - start + 1; // length of block
                byte[] block = new byte[length];
                Array.Copy(b, start, block, 0, length);
                blocks.Add(block);
                start = i + 1;
                i += 2;
            }
            else if (i==b.Length-1 && getRabin(b, i, p, max_size) != 0)
            {
                int length = i - start + 1; // length of block
                byte[] block = new byte[length];
                Array.Copy(b, start, block, 0, length);
                blocks.Add(block);
            }
        }
        return blocks;
    }

}



