using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Cache
{
    public partial class Cache : Form
    {
        // Cache
        List<byte[]> cache = new();
        public Cache()
        {
            InitializeComponent();
            var thread = new Thread(HandleRequest);
            thread.Start();
            LoadCacheContent();
        }
        public void HandleRequest()
        {
            IPAddress ipAddr = IPAddress.Loopback;

            // port of client 
            int portC = 8081;

            // port of origin server
            int portO = 8082;

            // Create and start a listener for client connection
            TcpListener listenerC = new(ipAddr, portC);
            listenerC.Start();

            while (true)
            {
                UpdateLog(string.Format("Cache started listening on: {0}:{1} at {2}", ipAddr, portC, DateTime.Now.TimeOfDay));
                Thread.Sleep(1000);

                TcpClient clientC = listenerC.AcceptTcpClient();

                // NetworkStream object is used for passing data between client and cache
                NetworkStream streamC = clientC.GetStream();

                // read the first byte that represents the command of the client
                byte command = (byte)streamC.ReadByte();

                if (command == 0) // forward greeting 
                {
                    StreamWriter temRes = new(streamC, Encoding.UTF8);
                    temRes.Write("command {0} received\n", command);
                    temRes.Flush();

                    // forward greeting to origin server
                    TcpClient client2O = new(ipAddr.ToString(), portO);
                    using NetworkStream stream2O = client2O.GetStream();
                    stream2O.WriteByte(command);
                    stream2O.Flush();

                    // get greeting response from the server
                    StreamReader reader4O = new(stream2O, Encoding.UTF8);
                    string response = reader4O.ReadLine();

                    // forward server's message to client
                    temRes.Write("{0}\n", response);
                    temRes.Flush();

                }
                else if (command == 1) // forward download request
                {
                    StreamWriter temRes = new(streamC);
                    temRes.Write("command {0} received\n", command);
                    temRes.Flush();

                    // receive file name
                    byte[] data = new byte[4];
                    streamC.Read(data, 0, 4);
                    int fileNameBytesLength = BitConverter.ToInt32(data, 0);
                    data = new byte[fileNameBytesLength];
                    streamC.Read(data, 0, fileNameBytesLength);

                    // get the path to the file
                    string fileName = Encoding.UTF8.GetString(data);
                    UpdateLog(string.Format("..........Client requested for file: {0} at {1}........", fileName, DateTime.Now.TimeOfDay));
                    Thread.Sleep(1000);

                    string URL = string.Format(".\\asset\\{0}", fileName);

                    // forward request to origin server
                    TcpClient client2O = new(ipAddr.ToString(), portO);
                    using NetworkStream stream2O = client2O.GetStream();
                    byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                    byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
                    byte[] req = new byte[5 + fileNameBytes.Length];
                    req[0] = command;
                    Array.Copy(fileNameLengthBytes, 0, req, 1, fileNameLengthBytes.Length);
                    Array.Copy(fileNameBytes, 0, req, 5, fileNameBytes.Length);
                    stream2O.Write(req, 0, req.Length);
                    stream2O.Flush();

                    // get response from the server and forward file size
                    StreamReader reader4O = new(stream2O, Encoding.UTF8);
                    string totalLength = reader4O.ReadLine();
                    temRes.Write("{0}\n", totalLength);
                    temRes.Flush();

                    // receiving blocks and forwarding to client
                    ulong totalSize = ulong.Parse(totalLength);
                    byte[] imageByte = new byte[totalSize];
                    ulong remainingSize = totalSize;
                    int offset = 0;

                    StreamReader reader4C = new(streamC, Encoding.UTF8);
                    StreamWriter proceed2O = new(stream2O);
                    int count = 0;
                    int fromCache = 0; // constructed from cache
                    int count_block = 0; // count cached blocks
                    while (remainingSize > 0 && stream2O != null)
                    {
                        string proceed = reader4C.ReadLine();
                        Console.WriteLine(proceed);

                        if (proceed == "OK")
                        {
                            proceed2O.Write("OK\n");
                            proceed2O.Flush();
                            string length_string = "";
                            length_string = reader4O.ReadLine();
                            int length_block = Convert.ToInt32(length_string);
                            proceed2O.Write("OK\n");
                            proceed2O.Flush();
                            byte[] block = new byte[length_block];
                            int temp = stream2O.Read(block, 0, length_block);
                            int readSize = temp;

                            // compute hash value of the block 
                            using SHA256 sha256 = SHA256.Create();
                            byte[] hashValue = sha256.ComputeHash(block);

                            // check if the block is cached
                            if (cache.Exists(x => x.SequenceEqual(hashValue)))
                            {
                                // tell client the block is already cached
                                string cached = "cached";
                                temRes.Write("{0}\n", cached);
                                temRes.Flush();
                                proceed = reader4C.ReadLine();
                                if (proceed == "OK")
                                {
                                    temRes.Write("{0}\n", Convert.ToBase64String(hashValue));
                                    temRes.Flush();
                                    offset += readSize;
                                    remainingSize -= (ulong)length_block;
                                    offset += length_block;
                                    fromCache += length_block;
                                }
                            }
                            else // save fingerprint and send fingerprint of block
                            {
                                // tell client the block is new
                                string cached = "new";
                                temRes.Write("{0}\n", cached);
                                temRes.Flush();
                                proceed = reader4C.ReadLine();
                                if (proceed == "OK")
                                {
                                    temRes.Write("{0}\n", Convert.ToBase64String(hashValue));
                                    temRes.Flush();
                                    proceed = reader4C.ReadLine();
                                    if (proceed == "OK")
                                    {
                                        // add hash to cache
                                        cache.Add(hashValue);
                                        listView2.Items.Add(string.Format("{0}Cached block {1}", Environment.NewLine, cache.Count - 1));
                                        count_block++;
                                        // send block
                                        streamC.Write(block, 0, block.Length);
                                        streamC.Flush();
                                        offset += readSize;
                                        remainingSize -= (ulong)length_block;
                                        offset += length_block;
                                    }
                                }
                            }
                            count++;
                        }
                    }
                    UpdateLog(string.Format("..........{2} {0}% of file {1} was constructed from cache {2}...........", (double)fromCache / (double)totalSize * 100, fileName, Environment.NewLine));
                    UpdateLog("==============================================");
                    UpdateLog("==============================================");
                    Thread.Sleep(1000);
                    reader4O.Close();
                    stream2O.Close();
                    streamC.Close();
                    client2O.Close();

                }
                else if (command == 3) // forward request for file names
                {
                    StreamWriter temRes = new(streamC, Encoding.UTF8);
                    StreamReader reader4C = new(streamC, Encoding.UTF8);
                    temRes.Write("command {0} received\n", command);
                    temRes.Flush();

                    // forward request to origin server
                    TcpClient client2O = new(ipAddr.ToString(), portO);
                    using NetworkStream stream2O = client2O.GetStream();
                    StreamWriter proceed2O = new(stream2O);
                    stream2O.WriteByte(command);
                    stream2O.Flush();
                    Console.WriteLine("Cache forwarded request at {0}", DateTime.Now.TimeOfDay);

                    StreamReader reader4O = new(stream2O, Encoding.UTF8);
                    string res = reader4O.ReadLine();
                    if (res != "no files available")
                    {
                        int length_list = int.Parse(res);
                        proceed2O.Write("OK\n");
                        proceed2O.Flush();
                        temRes.Write("{0}\n", length_list);
                        temRes.Flush();

                        for (int i = 0; i < length_list; i++)
                        {
                            string proceed = reader4C.ReadLine();
                            Console.WriteLine(proceed);
                            if (proceed == "OK")
                            {
                                proceed2O.Write("OK\n");
                                proceed2O.Flush();
                                string temp = reader4O.ReadLine();
                                temRes.Write(temp);
                                temRes.Write("\n");
                                temRes.Flush();
                            }
                        }
                    }
                    else
                    {
                        temRes.Write("Rejected\n");
                        temRes.Flush();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var thread = new Thread(HandleRequest);
            thread.Start();
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
            MessageBox.Show("Are you sure about clearing the cache?");
            cache = new();
            listView2.Items.Clear();
        }
        private void LoadCacheContent()
        {
            if (cache != null)
            {
                for (int i = 0; i < cache.Count; i++)
                {
                    listView2.Items.Add(string.Format("{0}Cached block {1}", Environment.NewLine, i));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count != 0)
            {
                for (int i = 0; i < listView2.SelectedItems.Count; i++)
                {
                    string selected = listView2.SelectedItems[i].Text[15..];
                    try
                    {
                        textBox4.Text = BitConverter.ToString(cache[int.Parse(selected)]).Replace("-", "");
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message + Environment.NewLine + string.Format("got index: {0}, length of cache:{1}", int.Parse(selected), cache.Count()));
                    }
                }
            }
            else
            {
                textBox4.Text = string.Format("Selected: {0}No block selected", Environment.NewLine);
            }
        }
    }
}