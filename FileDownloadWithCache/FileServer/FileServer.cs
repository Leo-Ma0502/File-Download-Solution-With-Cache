//FileServer.cs
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        int port = 8082;
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
            else // command for a image file
            {
                byte[] data = new byte[4];
                stream.Read(data, 0, 4);
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

                    // send file size first
                    writer.Write("{0}\n", b1.Length);
                    writer.Flush();
                    Console.WriteLine("Sent total size");
                    try
                    {
                        var blocks = GetBlocks(b1, 5, 7, 2048);
                        int lengthCount = 0;
                        StreamReader reader4C = new(stream, Encoding.UTF8);
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            string proceed = reader4C.ReadLine();
                            Console.WriteLine(proceed);
                            if (proceed == "OK")
                            {
                                var temp = blocks[i].Length;
                                writer.Write("{0}", temp);
                                writer.Write("\r\n");
                                writer.Flush();
                                proceed = reader4C.ReadLine();
                                if (proceed == "OK")
                                {
                                    stream.Write(blocks[i], 0, blocks[i].Length);
                                    stream.Flush();
                                    lengthCount += blocks[i].Length;
                                    Console.WriteLine("sent {0} block(s), total length {1}", i + 1, lengthCount);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                Console.WriteLine("sent all blocks as request");
                stream.Close();
            }
            client.Close();
        }
    }
    /*
     * get list of blocks of one file,
     * p is the prime chosen
     * M is used to do mod computation
     */
    private static double GetRabin(byte[] b, int p, int M)
    {
        // finger print value
        double res = 0;
        try
        {
            for (int i = 0; i < b.Length; i++)
            {
                res += (b[i] * Math.Pow(p, b.Length - i - 1));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return res % M;
    }
    /*
     * get blocks,
     * takes argument of image byte array
    */
    private static List<byte[]> GetBlocks(byte[] b, int window_length, int p, int M)
    {
        /*int size_control = 2048;
        int margin = 100;*/
        List<byte[]> blocks = new();
        int start = 0; // start point for slicing
        for (int i = 0; i < b.Length - window_length; i++)
        {
            byte[] temp = new byte[window_length];
            Array.Copy(b, i, temp, 0, window_length);
            int length = i - start + window_length + 1; // length of potential block
            double fingerPrint = GetRabin(temp, p, M);
            if (fingerPrint == 0)
            {
                byte[] block = new byte[length];
                Array.Copy(b, start, block, 0, length);
                blocks.Add(block);
                start = i + window_length + 1;
                i = start - 1;
            }
            else if (i == b.Length - window_length - 1)
            {
                byte[] block = new byte[length];
                Array.Copy(b, start, block, 0, length);
                blocks.Add(block);
            }
        }
        return blocks;
    }

}



