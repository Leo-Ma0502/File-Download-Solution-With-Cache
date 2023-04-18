//client.cs
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }
        TcpClient client;
        Dictionary<string, byte[]> cache = new();
        // greeting
        private void Greeting(IPAddress ipAddr, int port)
        {
            Console.WriteLine("start client at {0}", DateTime.Now.TimeOfDay);
            client = new(ipAddr.ToString(), port);

            byte command = 0;
            using NetworkStream stream = client.GetStream();
            stream.WriteByte(command);
            stream.Flush();
            Console.WriteLine("send greeting at {0}", DateTime.Now.TimeOfDay);

            StreamReader reader = new(stream, Encoding.UTF8);

            // get confirm message from the cache
            string confirm = reader.ReadLine();
            Console.WriteLine("got confirm: {1} at {0}", DateTime.Now.TimeOfDay, confirm);

            // get response forwared by cache
            string response = reader.ReadLine();
            Console.WriteLine("got response: {1} at {0}", DateTime.Now.TimeOfDay, response);
        }
        // requesting file download
        private void Request(IPAddress ipAddr, int port, string fileName)
        {
            // requesting file
            byte command = 1;
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
            byte[] data = new byte[5 + fileNameBytes.Length];

            data[0] = command;
            Array.Copy(fileNameLengthBytes, 0, data, 1, fileNameLengthBytes.Length);
            Array.Copy(fileNameBytes, 0, data, 5, fileNameBytes.Length);

            client = new TcpClient(ipAddr.ToString(), port);
            Console.WriteLine("===========connected to server==============");

            using NetworkStream stream = client.GetStream();
            // Send the data to the server
            stream.Write(data, 0, data.Length);
            stream.Flush();

            StreamReader reader = new(stream);

            // get confirm message from the cache
            string confirm = reader.ReadLine();
            Console.WriteLine("got confirm: {1} at {0}", DateTime.Now.TimeOfDay, confirm);

            List<byte[]> imageData = new();
            Image image;
            string totalLength = reader.ReadLine();
            ulong totalSize = ulong.Parse(totalLength);
            Console.WriteLine("Total size: {0}", totalSize);
            byte[] imageByte = new byte[totalSize];
            ulong remainingSize = totalSize;
            int offset = 0;

            StreamWriter proceed = new(stream);
            int count = 0;
            while (remainingSize > 0 && stream != null)
            {
                Console.WriteLine("=====================");
                proceed.Write("OK\n");
                proceed.Flush();
                Console.WriteLine("sent proceed request");

                // be informed whether block cached or not
                string cached = reader.ReadLine();
                Console.WriteLine("Received cached: {0}", cached);
                if (cached == "cached")
                {
                    Console.WriteLine("This block is cached");
                    proceed.Write("OK\n");
                    proceed.Flush();
                    // get index of block
                    string hash_string = reader.ReadLine();
                    byte[] index = Convert.FromBase64String(hash_string);
                    // get block from local cache
                    var block = cache[Convert.ToBase64String(index)];
                    Array.Copy(block, 0, imageByte, offset, block.Length);
                    remainingSize -= (ulong)block.Length;
                    offset += block.Length;
                }
                else if (cached == "new")
                {
                    Console.WriteLine("This block is new");
                    proceed.Write("OK\n");
                    proceed.Flush();
                    // get hash value
                    string hash_string = reader.ReadLine();
                    byte[] hash = Convert.FromBase64String(hash_string);
                    Console.WriteLine("received hash");
                    // get block
                    proceed.Write("OK\n");
                    proceed.Flush();
                    int readSize = stream.Read(imageByte, offset, (int)remainingSize);
                    Console.WriteLine("received block");
                    var block_new = new ArraySegment<byte>(imageByte, offset, readSize).ToArray();
                    cache.Add(Convert.ToBase64String(hash), block_new);
                    Console.WriteLine("added new block to cache");
                    remainingSize -= (ulong)readSize;
                    offset += readSize;
                    Console.WriteLine("Ready for next run");
                }
                Console.WriteLine("retrieved {0} blocks", count + 1);
                count++;
            }
            MemoryStream ms = new(imageByte, 0, imageByte.Length);
            image = Image.FromStream(ms);
            image.Save(string.Format(".\\asset\\{0}", fileName));
            Console.WriteLine(string.Format("----- Saved {0} -----", fileName));
            reader.Close();
            stream.Close();
            client.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {

            IPAddress ipAddr = IPAddress.Loopback;
            int port = 8081;
            try
            {
                Greeting(ipAddr, port);
            }
            catch (Exception err) { Console.WriteLine(err.Message); }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            IPAddress ipAddr = IPAddress.Loopback;
            int port = 8081;
            try
            {
                Request(ipAddr, port, "test1.bmp");
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            IPAddress ipAddr = IPAddress.Loopback;
            int port = 8081;
            try
            {
                Request(ipAddr, port, "test2.bmp");
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }
    }
}