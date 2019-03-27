﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static Socket client;
        //  private static readonly byte[] pit;

        static void Main(string[] args)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.10"), 2014));
            Thread t = new Thread(sendMsg);
            t.Start();
            //Thread t2 = new Thread(receiveMsg);
        }
        //把16进制字符串转化为byte格式
        private static byte[] HexStrTobyte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }
        //给特定的ip的主机端口发生送数据包
        /*static void sendMsg()
        {
            EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2015);
            client.SendTo(Encoding.UTF8.GetBytes(msg), point);
        }*/
        static void sendMsg()
        {
            byte[] stop = HexStrTobyte("A5 4F 00 40 0F 00 00 00");
            byte[] f_hz = HexStrTobyte("A5 56 00 40 0F 00 00 07");
            byte[] t_hz = HexStrTobyte("A5 A6 00 40 0F 00 00 57");
            byte[] tw_hz = HexStrTobyte("A5 46 00 40 0F 00 00 F7");
            EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2015);
            while (true)
            {
                string key = Console.ReadLine();
                if (key == "s")
                {
                    client.SendTo(stop, point);
                    receiveMsg();
                    continue;
                }
                else if (key == "5")
                {
                    client.SendTo(f_hz, point);
                    receiveMsg();
                    continue;
                }
                else if (key == "t")
                {
                    client.SendTo(t_hz, point);
                    receiveMsg();
                    continue;
                }
                else if (key == "tw")
                {
                    client.SendTo(tw_hz, point);
                    receiveMsg();
                    continue;
                }
            }
        }
        //接受发送给本机ip特定端口的数据包         
        static void receiveMsg()
        {
            for (int j = 0; j < 30; j++)
            {
                EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2014);
                byte[] buffer = new byte[1248];
                int length = client.ReceiveFrom(buffer, ref point);
                //string message = Encoding.UTF8.GetString(buffer, 0, length);
                // byte[] data = buffer; 
                string meg = BitConverter.ToString(buffer);
                Console.WriteLine(point.ToString() + "点云数据16进制：" + meg);
                Thread.Sleep(500);

                if (j == 29)
                {
                    /*FileStream fs = new FileStream("C:/Users/radar/Desktop/1.txt", FileMode.Create, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs);*/
                    using (StreamWriter sw = new StreamWriter(@"C:/Users/radar/Desktop/1.txt"))
                    {
                        //
                        for (int k = 42; k < 1242; k = k + 100)
                        {
                            byte[] pit = new byte[100];
                            for (int z = 0; z < 100; z = z + 1)
                            {
                                pit[z] = buffer[k + z];
                                //Console.WriteLine(pit1);
                            }
                            string pz = BitConverter.ToString(pit);
                            //string pz = System.Text.Encoding.UTF8.GetString(pit);
                            //File.WriteAllText("C:/Users/radar/Desktop/1.txt",pz,Encoding.Default);
                            sw.WriteLine(pz);
                            // fs.WriteByte(pit[z]);
                        }
                    }

                    //System.IO.File.WriteAllText("C:/Users/radar/Desktop/1.txt",message , Encoding.Default);   
                    Console.WriteLine("数据写入完成");
                }
            }

        }
    }
}
