using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Drawing;
using System.Windows.Forms.Design;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            main();
        }
        TcpClient client;
        private void main()
        {
            // IP Address to listen on. Loopback is the localhost
            IPAddress ipAddr = IPAddress.Loopback;
            // Port to listen on
            int port = 8081;
            // the names of the files that we ask the server to read
            var files = new List<string>();
            files.Add("test1.bmp");
            files.Add("test2.bmp");

            try
            {
                Greeting(ipAddr, port);
                foreach(string fileName in files)
                {
                    Request(ipAddr, port, fileName);
                }
                
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        // greeting to server
        private void Greeting(IPAddress ipAddr, int port)
        {
            Console.WriteLine("start client");
            Console.WriteLine("connected to server");
            client = new(ipAddr.ToString(), port);
            byte command = 0;
            using (NetworkStream stream = client.GetStream())
            {
                stream.WriteByte(command);
                stream.Flush();

                // get greeting message from the server
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string response = reader.ReadLine();

                Console.WriteLine("Received string: " + response);
            }

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

                string imageData;

                // print out the contents of the file received from the server
                while ((imageData = reader.ReadLine()) != null)
                {
                    Console.WriteLine(imageData);
                    byte[] imageBytes = Convert.FromBase64String(imageData);
                    Image image;
                    MemoryStream ms = new(imageBytes, 0, imageBytes.Length);
                    image = Image.FromStream(ms);
                    image.Save(string.Format(".\\asset\\{0}", fileName));
                    Console.WriteLine(string.Format("----- Saved {0} -----", fileName));
                }

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