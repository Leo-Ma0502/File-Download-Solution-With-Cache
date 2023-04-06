using System.Net.Sockets;
using System.Net;
using System.Text;

namespace FileServer;

public partial class FileServer : Form
{
    public FileServer()
    {
        InitializeComponent();
        main();
    }

    private void main()
    {
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
            Console.WriteLine("client connected");

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
                StreamWriter writer = new StreamWriter(stream);

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
                stream.Close();
            }
            client.Close();
        }
    }
}
