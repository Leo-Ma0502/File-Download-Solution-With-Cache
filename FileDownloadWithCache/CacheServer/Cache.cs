using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace CacheServer
{
    public class Cache
    {
        public void HandleRequest()
        {
            Console.WriteLine("Cache starting");
            // IP Address to listen on. Loopback is the localhost
            IPAddress ipAddr = IPAddress.Loopback;

            // port of client 
            int portC = 8081;

            // port of origin server
            int portO = 8082;

            // Create and start a listener for client connection
            TcpListener listenerC = new(ipAddr, portC);
            listenerC.Start();

            // Cache
            List<byte[]> cache = new();

            while (true)
            {
                Console.WriteLine("Server side cache listening on: {0}:{1} at {2}", ipAddr, portC, DateTime.Now.TimeOfDay);
                Console.WriteLine("current cache: {0}", cache.Count == 0 ? "is null" : "is not null");
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
                    Console.WriteLine("Cache sent greeting at {0}", DateTime.Now.TimeOfDay);

                    // get greeting response from the server
                    StreamReader reader4O = new(stream2O, Encoding.UTF8);
                    string response = reader4O.ReadLine();
                    Console.WriteLine("Cache received response at {1}: {0}", response, DateTime.Now.TimeOfDay);

                    // forward server's message to client
                    temRes.Write("{0}\n", response);
                    temRes.Flush();
                    Console.WriteLine("Cache sent to client at {1}: {0}", response, DateTime.Now.TimeOfDay);

                }
                else // forward download request
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
                    Console.WriteLine("received filename:" + fileName);
                    string URL = string.Format(".\\asset\\{0}", fileName);
                    Console.WriteLine("url: " + URL);

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
                    Console.WriteLine("Cache sent request to origin server at {0}", DateTime.Now.TimeOfDay);

                    // get response from the server and forward file size
                    StreamReader reader4O = new(stream2O, Encoding.UTF8);
                    string totalLength = reader4O.ReadLine();
                    Console.WriteLine("Cache received total length at {1}: {0}", totalLength, DateTime.Now.TimeOfDay);
                    temRes.Write("{0}\n", totalLength);
                    temRes.Flush();

                    // receiving blocks and forwarding to client
                    ulong totalSize = ulong.Parse(totalLength);
                    Console.WriteLine("Total size: {0}", totalSize);
                    byte[] imageByte = new byte[totalSize];
                    ulong remainingSize = totalSize;
                    int offset = 0;

                    StreamReader reader4C = new(streamC, Encoding.UTF8);
                    StreamWriter proceed2O = new(stream2O);
                    int count = 0;
                    int fromCache = 0; // constructed from cache
                    while (remainingSize > 0 && stream2O != null)
                    {
                        Console.WriteLine("=====================");
                        Console.WriteLine("Remaining size: {0}", remainingSize);
                        string proceed = reader4C.ReadLine();
                        Console.WriteLine(proceed);
                        if (proceed == "OK")
                        {
                            proceed2O.Write("OK\n");
                            proceed2O.Flush();
                            Console.WriteLine("forwarded proceed request");
                            string length_string = "";
                            length_string = reader4O.ReadLine();
                            Console.WriteLine("Current read: {0}", length_string);
                            int length_block = Convert.ToInt32(length_string);
                            Console.WriteLine("Current block length: {0}", length_block);
                            proceed2O.Write("OK\n");
                            proceed2O.Flush();
                            byte[] block = new byte[length_block];
                            int temp = stream2O.Read(block, 0, length_block);
                            Console.WriteLine("Received block from server");
                            int readSize = temp;

                            // compute hash value of the block 
                            using SHA256 sha256 = SHA256.Create();
                            byte[] hashValue = sha256.ComputeHash(block);

                            // check if the block is cached
                            /*if (cache.Contains(hashValue))*/ // send fingerprint of the block
                            if (cache.Exists(x=>x.SequenceEqual(hashValue))) // test
                            {
                                // tell client the block is already cached
                                string cached = "cached";
                                temRes.Write("{0}\n", cached);
                                temRes.Flush();
                                Console.WriteLine("told client it is {0}", cached);
                                proceed = reader4C.ReadLine();
                                if (proceed == "OK")
                                {
                                    temRes.Write("{0}\n", Convert.ToBase64String(hashValue));
                                    temRes.Flush();
                                    Console.WriteLine("sent hash value to client");
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
                                Console.WriteLine("told client it is {0}", cached);
                                proceed = reader4C.ReadLine();
                                if (proceed == "OK")
                                {
                                    temRes.Write("{0}\n", Convert.ToBase64String(hashValue));
                                    temRes.Flush();
                                    Console.WriteLine("sent hash value to client");
                                    proceed = reader4C.ReadLine();
                                    if (proceed == "OK")
                                    {
                                        // add hash to cache
                                        cache.Add(hashValue);
                                        Console.WriteLine("cached a block");
                                        // send block
                                        streamC.Write(block, 0, block.Length);
                                        streamC.Flush();
                                        Console.WriteLine("forwarded block[{0}]", count);
                                        offset += readSize;
                                        remainingSize -= (ulong)length_block;
                                        offset += length_block;
                                    }
                                }
                            }
                            Console.WriteLine("forwarded {0} blocks", count + 1);
                            count++;
                            Console.WriteLine("=====================");
                        }
                    }
                    Console.WriteLine(".......... Constructed from cache: {0} % ...........", (double)fromCache/(double)totalSize*100);
                    reader4O.Close();
                    stream2O.Close();
                    streamC.Close();
                    client2O.Close();

                }
            }
        }
    }
}

