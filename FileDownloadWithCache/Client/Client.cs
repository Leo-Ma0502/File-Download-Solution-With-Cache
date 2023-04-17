//client.cs
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Drawing;
using System.Windows.Forms.Design;
using System.IO;
using System.Buffers.Text;
using System.CodeDom.Compiler;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            /*progressBar1.Visible = false;*/
        }
        TcpClient client;
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
            while (remainingSize != 0 && stream != null)
            {
                int readSize = stream.Read(imageByte, offset, (int)remainingSize);
                remainingSize -= (ulong)readSize;             
                offset += readSize;
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
    }
}