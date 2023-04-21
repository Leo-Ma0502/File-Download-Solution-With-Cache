//client.cs
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public partial class Client : Form
    {
        ImageList images;
        int count = 0; // used to count the blocks
        int idx_img = 0; // used to index the image
        public Client()
        {
            InitializeComponent();
            images = new();
        }

        TcpClient client;
        readonly Dictionary<string, byte[]> cache = new();
        private void SetItems()
        {
            listView1.Items.Clear();
            listView1.Focus();
            IPAddress ipAddr = IPAddress.Loopback;
            int port = 8081;
            string[] files_available = ReqForFileNames(ipAddr, port);
            if (files_available[0] != "null")
            {
                Console.WriteLine("retrieved file list");
                for (int i = 0; i < files_available.Length; i++)
                {
                    listView1.Items.Add(files_available[i]);
                }
                if (listView1.SelectedItems.Count != 0)
                {
                    for (int i = 0; i < listView1.SelectedItems.Count; i++)
                    {
                        textBox1.Text = string.Format("Selected:{1}{0}", listView1.SelectedItems[i].Text, Environment.NewLine);
                    }
                }
                else
                {
                    textBox1.Text = string.Format("Selected: {0}No file selected", Environment.NewLine);
                }
            }
            else
            {
                MessageBox.Show("No files available");
            }
        }

        // get file names 
        private string[] ReqForFileNames(IPAddress ipAddr, int port)
        {
            Console.WriteLine("start client at {0}", DateTime.Now.TimeOfDay);
            client = new(ipAddr.ToString(), port);

            byte command = 3;
            using NetworkStream stream = client.GetStream();
            stream.WriteByte(command);
            stream.Flush();
            Console.WriteLine("send request at {0}", DateTime.Now.TimeOfDay);

            StreamReader reader = new(stream, Encoding.UTF8);
            StreamWriter proceed = new(stream);

            // get confirm message from the cache
            string confirm = reader.ReadLine();
            Console.WriteLine("got confirm: {1} at {0}", DateTime.Now.TimeOfDay, confirm);

            // get response forwared by cache
            string res = reader.ReadLine();
            if (res != "Rejected")
            {
                int length_list = int.Parse(res);
                Console.WriteLine("got length of file list");
                string[] fileList = new string[length_list];
                for (int i = 0; i < length_list; i++)
                {
                    proceed.Write("OK\n");
                    proceed.Flush();
                    fileList[i] = reader.ReadLine();
                    Console.WriteLine("received file name {0}", i);
                }
                return fileList;
            }
            else
            {
                return new string[] { "null" };
            }
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
            while (remainingSize > 0 && stream != null)
            {
                // progress bar
                pb1.Visible = true;
                pb1.Minimum = 0;
                pb1.Maximum = (int)totalSize;
                pb1.Value = 1;
                pb1.Step = 1;
                pb1.Style = ProgressBarStyle.Marquee;

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
                    string potentialKey = Convert.ToBase64String(hash);
                    if (!cache.ContainsKey(potentialKey))
                    {
                        cache.Add(potentialKey, block_new);
                        Console.WriteLine("added new block to cache");
                    }
                    remainingSize -= (ulong)readSize;
                    offset += readSize;
                    Console.WriteLine("Ready for next run");
                }
                pb1.Value = offset;
                Console.WriteLine("retrieved {0} blocks", count + 1);
                count++;
            }
            pb1.Visible = false;
            MemoryStream ms = new(imageByte, 0, imageByte.Length);
            image = Image.FromStream(ms);
            image.Save(string.Format(".\\asset\\{0}", fileName));
            images.Images.Add(idx_img.ToString(), new Bitmap(string.Format(".\\asset\\{0}", fileName)));
            Console.WriteLine(string.Format("----- Saved {0} -----", fileName));
            reader.Close();
            stream.Close();
            client.Close();
            LoadDownloadedImages(idx_img);
            idx_img++;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            IPAddress ipAddr = IPAddress.Loopback;
            int port = 8081;
            if (listView1.SelectedItems.Count != 0)
            {
                string fileName = listView1.SelectedItems[0].Text;
                try
                {
                    Request(ipAddr, port, fileName);
                }
                catch (Exception err) { MessageBox.Show(err.Message); }
            }
            else
            {
                MessageBox.Show("No file selected");
            }
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
        private void button1_Click(object sender, EventArgs e)
        {
            SetItems();
        }
        private void getSelected(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    textBox1.Text = string.Format("Selected:{1}{0}", listView1.SelectedItems[i].Text, Environment.NewLine);
                }
            }
            else
            {
                textBox1.Text = string.Format("Selected: {0}No file selected", Environment.NewLine);
            }
        }
        private void LoadDownloadedImages(int idx)
        {
            button3.Visible = true;
            button4.Visible = true;
            if (!images.Images.Empty)
            {
                pictureBox1.Image = images.Images[idx.ToString()];
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            int curr_idx = idx_img; // currently displayed image
            if (curr_idx == 0)
            {
                MessageBox.Show("Already the first image");
            }
            else
            {
                curr_idx -= 1;
                LoadDownloadedImages(curr_idx);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int curr_idx = idx_img; // currently displayed image
            if (curr_idx == images.Images.Count - 1)
            {
                MessageBox.Show("Already the last image");
            }
            else
            {
                curr_idx += 1;
                LoadDownloadedImages(curr_idx);
            }
        }
    }
}