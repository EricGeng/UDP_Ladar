using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace ConsoleApplication1
{
    class Program
    {
        static Socket client;
        static SerialPort port;
        static List<byte> signal = new List<byte>();
        static int g = 1;
        static void Main(string[] args)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.10"), 2014));
            Thread t = new Thread(sendMsg);
            t.Start();
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
        static void sendMsg()
        {
            byte[] stop = HexStrTobyte("A5 4F 00 40 0F 00 00 00");
            byte[] f_hz = HexStrTobyte("A5 56 00 40 0F 00 00 07");
            byte[] t_hz = HexStrTobyte("A5 A6 00 40 0F 00 00 57");
            byte[] tw_hz = HexStrTobyte("A5 46 00 40 0F 00 00 F7");
            EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2015);
            port = new SerialPort("COM4", 9600);
            port.Open();
            while (true)
            {
                string key = Console.ReadLine();
                /*if (key == "s")
                {
                    client.SendTo(stop, point);
                    while(value!="2")
                    {
                        port.Write("1");
                    }
                    Console.WriteLine("end");
                    continue;
                }*/
                if (key == "s")
                {
                    client.SendTo(f_hz, point);
                    //DateTime before = DateTime.Now;
                    // receiveMsg();
                    //datapro();     //经过计算，写入数据时间至少需要15s
                    //DateTime after = DateTime.Now;
                    // TimeSpan ts = after.Subtract(before);
                    //Console.WriteLine(ts);
                    //port.Write("1");                    
                    while (true)
                    {
                        string s = ReadSerialData();
                        List<byte> signal = new List<byte>();
                        if (s != "1")
                        {
                            Directory.CreateDirectory(@"C:\Users\radar\Desktop\data\data_" + g);//创建新的文件夹
                            receiveMsg();
                            datapro();
                            g++;
                            port.Write("1");                  //向下位机发送运动指令              
                            //  Thread.Sleep(10000);//延时15s
                            //Console.WriteLine(g);
                        }
                        else
                        {
                            break;
                        }
                    }
                    //client.SendTo(f_hz, point);
                    Console.WriteLine("done");
                    continue;
                }
                /*else if (key == "5")
                {
                    client.SendTo(f_hz, point);
                    receiveMsg();                    
                    datapro();
                    for(int a=0;a<10000;a++)
                    {
                        port.Write("1");
                    }

                    // Gathertxt();   //文件合并速度太慢，在实验中选择不处理数据，结束后处理
                    continue;
                }*/
            }
        }
        //接受发送给本机ip特定端口的数据包         
        static void receiveMsg()
        {
            double wt;
            for (int j = 0; j < 1800; j++)
            {
                EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2014);
                byte[] buffer = new byte[1206];
                int length = client.ReceiveFrom(buffer, ref point);
                wt = (Convert.ToInt32(buffer[3]) * 256 + Convert.ToInt32(buffer[2])) * 0.01;//计算方位角，转化成double格式
                if (wt >= 180 && wt < 360)
                {
                    foreach (byte i in buffer)  //将所有符合条件的数据一起收集到一个list中
                    {
                        Program.signal.Add(i);
                    }
                }
            }
            Console.WriteLine("数据采集完成");
        }
        static void datapro()   //分组数据的三维坐标计算
        {
            int f = 1;
            #region      
            for (int a = 0; a < signal.Count; a = a + 1206)   //数据分组
            {
                //StreamWriter sy = new StreamWriter("C:/Users/EricGeng/Desktop/23.txt");
                byte[] group = new byte[1206];
                for (int b = 0; b < 1206; b++)
                {
                    group[b] = signal[b + a];
                }
                // Directory.CreateDirectory(@"C:\Users\radar\Desktop\data\data_"+f);
                using (StreamWriter sw = new StreamWriter("C:/Users/radar/Desktop/data/data_" + g + " / " + f + "_data" + ".txt"))
                {
                    List<double> data = new List<double>();
                    for (int c = 0; c < 12; c++)
                    {
                        double wt = (Convert.ToInt32(group[3 + 100 * c]) * 256 + Convert.ToInt32(group[2 + 100 * c])) * 0.01;//the azimuth angle of one group
                        double[] range = new double[32];
                        double[] reflet = new double[32];
                        int e = 4;
                        for (int d = 0; d < 32; d++)
                        {
                            range[d] = (Convert.ToInt32(group[100 * c + e + 1]) * 256 + Convert.ToInt32(group[100 * c + e])) * 4;
                            reflet[d] = Convert.ToInt32(group[100 * c + e + 2]);
                            e = e + 3;
                        }
                        double[] H_ang = { -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35 };
                        double[] V_ang = { -19, -17, -15, -13, -11, -9, -7, -5, -3, -1, 1, 3, 5, 7, 9, 11 };
                        double[] dt = { 0, 3.125, 6.25, 9.375, 12.5, 15.625, 18.75, 21.875, 25, 28.125, 31.25, 34.375, 37.5, 40.625, 43.75, 46.875 };
                        double w = 0.0018;
                        for (int i = 0; i < 32; i++)
                        {
                            if (i < 16)
                            {
                                double angle = (-wt - w * dt[i] - H_ang[i]) / 180 * Math.PI;
                                double y = range[i] * Math.Cos(V_ang[i] / 180 * Math.PI) * Math.Sin(angle);
                                double x = range[i] * Math.Cos(V_ang[i] / 180 * Math.PI) * Math.Cos(angle);
                                double z = range[i] * Math.Sin(V_ang[i] / 180 * Math.PI);
                                data.Add(x);
                                data.Add(y);
                                data.Add(z);
                                data.Add(reflet[i]);
                            }
                            else
                            {
                                double angle = (-wt - w * (dt[i - 16] + 50) - H_ang[i - 16]) / 180 * Math.PI;
                                double y = range[i] * Math.Cos(V_ang[i - 16] / 180 * Math.PI) * Math.Sin(angle);
                                double x = range[i] * Math.Cos(V_ang[i - 16] / 180 * Math.PI) * Math.Cos(angle);
                                double z = range[i] * Math.Sin(V_ang[i - 16] / 180 * Math.PI);
                                data.Add(x);
                                data.Add(y);
                                data.Add(z);
                                data.Add(reflet[i]);
                            }
                        }
                    }
                    foreach (double s in data)
                    {
                        sw.WriteLine(s.ToString());
                    }
                    #endregion
                }
                f++;
            }
            Console.WriteLine("数据写入文件完成");
            signal.Clear();
        }
        static void Gathertxt()        //合并txt文件，整合一次实验数据
        {
            for (int n = 0; n < signal.Count / 1206; n++)
            {
                int m = n + 1;
                string a = File.ReadAllText("C:/Users/radar/Desktop/全部数据.txt");
                string b = File.ReadAllText(@"C:\Users\radar\Desktop\data\" + m + "_data" + ".txt");
                File.WriteAllText("C:/Users/radar/Desktop/全部数据.txt", a + b);
            }
            Console.WriteLine("数据合并完成");
        }
        static string ReadSerialData()   //读取串口的数据
        {
            string value = "";
            if (port != null && port.BytesToRead > 0)
            {
                value = port.ReadExisting();
            }
            return value;
        }
    }
}