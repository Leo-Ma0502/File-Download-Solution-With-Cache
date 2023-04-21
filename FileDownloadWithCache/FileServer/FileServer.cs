//FileServer.cs
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileServer;

public partial class FileServer : Form
{
    string[] fileList;
    string[] availableForClient;
    // allow administrator to choose which folder to host
    string hostingDirec = "..\\";
    public FileServer()
    {
        InitializeComponent();
        var thread = new Thread(StartServing);
        thread.Start();
        LoadFileList();
    }
    public void StartServing()
    {
        IPAddress ipAddr = IPAddress.Loopback;
        int port = 8082;
        TcpListener listener = new(ipAddr, port);
        listener.Start();
        while (true)
        {
            UpdateLog(string.Format("Server started listening on: {0}:{1} at {2}", ipAddr, port, DateTime.Now.TimeOfDay));
            Thread.Sleep(1000);
            var client = listener.AcceptTcpClient();
            UpdateLog(string.Format("cache connected at {0}", DateTime.Now.TimeOfDay));
            Thread.Sleep(1000);
            // NetworkStream object is used for passing data between client and server
            NetworkStream stream = client.GetStream();

            // For image data
            MemoryStream stream_image = new();

            // read the first byte that represents the command of the client
            byte command = (byte)stream.ReadByte();

            // 0 is for a greeting message
            if (command == 0)
            {
                StreamWriter writer = new(stream, Encoding.UTF8);
                string greeting = "Hello, client!";
                writer.Write(greeting);
                writer.Flush();
            }
            else if (command == 1) // command for a image file
            {
                byte[] data = new byte[4];
                stream.Read(data, 0, 4);
                int fileNameBytesLength = BitConverter.ToInt32(data, 0);
                data = new byte[fileNameBytesLength];
                stream.Read(data, 0, fileNameBytesLength);

                string fileName = Encoding.UTF8.GetString(data);
                UpdateLog(string.Format("received filename {0}", fileName));
                Thread.Sleep(1000);
                string URL = string.Format("{1}\\{0}", fileName, hostingDirec);
                StreamWriter writer = new(stream);
                using (Image image = Image.FromFile(URL))
                {
                    image.Save(stream_image, image.RawFormat);
                    byte[] b1 = stream_image.ToArray();

                    // send file size first
                    writer.Write("{0}\n", b1.Length);
                    writer.Flush();

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
            else if (command == 3)
            {
                StreamWriter writer = new(stream, Encoding.UTF8);
                if (availableForClient != null)
                {
                    writer.Write("{0}\n", availableForClient.Length);
                    writer.Flush();
                    StreamReader reader4C = new(stream, Encoding.UTF8);
                    for (int i = 0; i < availableForClient.Length; i++)
                    {
                        string proceed = reader4C.ReadLine();
                        Console.WriteLine(proceed);
                        if (proceed == "OK")
                        {
                            writer.Write(availableForClient[i]);
                            writer.Write("\n");
                            writer.Flush();
                            Console.WriteLine("sent {0} file names", i + 1);
                        }
                    }
                }
                else
                {
                    writer.Write("no files available\n");
                    writer.Flush();
                }
            }
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
    // get local file lists
    private string[] GetFiles()
    {    
        FolderBrowserDialog directory = new()
        {
            Description = "please choose the folder to host"
        };
        if (directory.ShowDialog() == DialogResult.OK || directory.ShowDialog() == DialogResult.Yes)
        {
            if (hostingDirec == "")
            {
                throw new InvalidOperationException("Please select the folder");
            }
            hostingDirec = directory.SelectedPath;
            return Directory.GetFiles(hostingDirec);          
        }
        return Directory.GetFiles(hostingDirec);
    }
    private void UpdateLog(string text)
    {
        // Update the UI with the current count on the UI thread
        if (textBox1.InvokeRequired)
        {
            textBox1.Invoke(new Action(() => textBox1.Text += Environment.NewLine + text));
        }
        else
        {
            textBox1.Text += Environment.NewLine + text;
        }
    }

    private void button1_Click_1(object sender, EventArgs e)
    {
        textBox4.Text = "";
        if (listView2.SelectedItems.Count != 0)
        {
            availableForClient = new string[listView2.SelectedItems.Count];
            for (int i = 0; i < listView2.SelectedItems.Count; i++)
            {
                textBox4.Text += string.Format("{1}{0}", listView2.SelectedItems[i].Text, Environment.NewLine);
                availableForClient[i] = listView2.SelectedItems[i].Text;
            }
        }
        else
        {
            textBox4.Text = "No file available";
        }
    }
    private void LoadFileList()
    {
        fileList = GetFiles();
        if (fileList.Length != 0)
        {
            listView2.Items.Clear();
            for (int i = 0; i < fileList.Length; i++)
            {
                listView2.Items.Add(fileList[i].Split("\\")[^1]);
            }
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        LoadFileList();
    }
}



