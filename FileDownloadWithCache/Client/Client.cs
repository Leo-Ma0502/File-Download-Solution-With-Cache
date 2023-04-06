using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            main();
        }

        private void main()
        {
            // IP Address to listen on. Loopback is the localhost
            IPAddress ipAddr = IPAddress.Loopback;
            // Port to listen on
            int port = 8081;

            try
            {
                Console.WriteLine("start client");
                TcpClient client = new TcpClient(ipAddr.ToString(), port); // Create a new connection  
                Console.WriteLine("connected to server");

                // send 0 to the server
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

                // send 1 to the server
                command = 1;

                // the name of the file that we ask the server to read
                string fileName = "test1.bmp";

                // text need to be sent as binary number
                // so, we store file name in a binary aray
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

                // store the number of bytes representing the file name as a byte array with 4 elements as int is 4 bytes long
                byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);

                // Create a new byte array for holding the data to be sent to the server
                // element 0 is the command
                // element 1 to 4 is the length of the bytes representing the file name
                // the remaining elements represent the file name
                byte[] data = new byte[5 + fileNameBytes.Length];

                // Copy the command, the length and the filename to the array to be sent to the server
                data[0] = command;
                Array.Copy(fileNameLengthBytes, 0, data, 1, fileNameLengthBytes.Length);
                Array.Copy(fileNameBytes, 0, data, 5, fileNameBytes.Length);

                client = new TcpClient(ipAddr.ToString(), port); // Create a new connection  
                Console.WriteLine("===========connected to server==============");

                using (NetworkStream stream = client.GetStream())
                {
                    // Send the data to the server
                    stream.Write(data, 0, data.Length);
                    stream.Flush();

                    // the StreamReader object is used to receive reply from the server
                    StreamReader reader = new StreamReader(stream);
                    string imageData;
                    Image image;
                    // print out the contents of the file received from the server
                    while ((imageData = reader.ReadLine()) != null)
                    {
                        /*richTextBox1.Text = imageData;*/
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(imageData)))
                        {
                            image = Image.FromStream(ms);
                            image.Save(".\\test1.jpg");
                        }
                    }





                    reader.Close();
                    stream.Close();
                    client.Close();

                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            /*Application.Run(new Client());*/

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}