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
            main();
        }
        /*TODO 
         * initiate and maintain a dictionary(hash, block)
         * link blocks together and save as image file       
         */
        TcpClient client;
        public void main()
        {
            // IP Address to listen on. Loopback is the localhost
            IPAddress ipAddr = IPAddress.Loopback;
            // Port to listen on
            int port = 8081;
            // the names of the files that we ask the server to read
            var files = new List<string>();
            files.Add("test1.bmp");
         /*   files.Add("test2.bmp");*/

            try
            {
                Greeting(ipAddr, port);
                Request(ipAddr, port, "test1.bmp");
                /*foreach(string fileName in files)
                {
                    Request(ipAddr, port, fileName);
                }*/
                
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        // greeting
        private void Greeting(IPAddress ipAddr, int port)
        {
            Console.WriteLine("start client at {0}", DateTime.Now.TimeOfDay);
            client = new(ipAddr.ToString(), port);

            byte command = 0;
            using NetworkStream stream = client.GetStream();
            Console.WriteLine("send greeting at {0}", DateTime.Now.TimeOfDay);
            stream.WriteByte(command);
            stream.Flush();
            // end session first
            // get greeting message from the cache

            /* while (true)
             {
                 client.Close();
                 StreamReader reader = new(stream, Encoding.UTF8);

                 // get greeting message from the cache
                 string response = reader.ReadLine();
                 Console.WriteLine("got response: {1} at {0}", DateTime.Now.TimeOfDay, response);
             }
 */
            StreamReader reader = new(stream, Encoding.UTF8);

            // get greeting message from the cache
            /*string response=reader.ReadLine();*/
          
            
            string response = reader.ReadLine();
            Console.WriteLine("got response: {1} at {0}", DateTime.Now.TimeOfDay, response);
            
            

            /*TcpListener listenFromCache = new(ipAddr, 8083);
            listenFromCache.Start();
            while (true)
            {
                Console.WriteLine("Client waiting for response");
                using NetworkStream streamFromCache = listenFromCache.AcceptTcpClient().GetStream();
                Console.WriteLine("fuck 711");
                StreamReader readFromCache = new(streamFromCache, Encoding.UTF8);
                Console.WriteLine("fuck 7111");
                string response = readFromCache.ReadLine();
                Console.WriteLine("fuck71111");
                Console.WriteLine("got response: {1} at {0}", DateTime.Now.TimeOfDay, response);

            }*/
        }

        // requesting file download
        private void Request(IPAddress ipAddr, int port, string fileName)
        {
            // requesting file
            byte command = 1;

            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

            // store the number of bytes representing the file name as a byte array with 4 elements as int is 4 bytes long
            byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);

            byte[] data = new byte[5 + fileNameBytes.Length];

            // Copy the command, the length and the filename to the array to be sent to the server
            data[0] = command;
            Array.Copy(fileNameLengthBytes, 0, data, 1, fileNameLengthBytes.Length);
            Array.Copy(fileNameBytes, 0, data, 5, fileNameBytes.Length);

            client = new TcpClient(ipAddr.ToString(), port);
            Console.WriteLine("===========connected to server==============");

            using (NetworkStream stream = client.GetStream())
            {
                // Send the data to the server
                stream.Write(data, 0, data.Length);
                stream.Flush();
                // the StreamReader object is used to receive reply from the server
                StreamReader reader = new(stream);

                List<byte[]> imageData = new();
                string blockData;
                /*byte[] imageByte = Array.Empty<byte>();*/
                Image image;
                /*while ((blockData = reader.ReadLine()) != null)
                {
                    blockData = reader.ReadLine(); //test
                    byte[] temp = Encoding.UTF8.GetBytes(blockData);
                    imageByte = new byte[temp.Length];
                    Array.Copy(temp, imageByte, temp.Length);
                }*/
                // print out the contents of the file received from the server
                while ((blockData = reader.ReadLine()) != null)
                {
                    /*imageData.Add(Encoding.UTF8.GetBytes(blockData));*/
                    blockData = blockData.Replace("=", "+").Trim();
                    Console.WriteLine(blockData);
                    imageData.Add(Convert.FromBase64String(blockData));
                }

                /*string imageDataString = String.Join("", imageData);*/
                /*string imageDataString = imageData[^1];*/
                /*byte[] imageByte = Encoding.UTF8.GetBytes(imageDataString);*/
                byte[] imageByte = imageData.SelectMany(bytes => bytes).ToArray();
                /*byte[] imageByte = imageData[^1];*/
                MemoryStream ms = new(imageByte, 0, imageByte.Length);
                image = Image.FromStream(ms);
                image.Save(string.Format(".\\asset\\{0}", fileName));
                Console.WriteLine(string.Format("----- Saved {0} -----", fileName));
                reader.Close();
                stream.Close();
                client.Close();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}