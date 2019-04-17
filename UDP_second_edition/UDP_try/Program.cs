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
        static List<byte> signal = new List<byte>();//静态u全局变量，一次采集的全部数据
        static int g = 1;
        static void Main(string[] args)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.11"), 2014));
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
            port = new SerialPort("COM7", 9600);
            port.Open();
            while (true)
            {    //int as=1;
                string key = Console.ReadLine();
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
                        
                        List<byte> signal = new List<byte>();
                       /* receiveMsg();
                        datapro2();
                        datapro3();
                        g++;
                        //Thread.Sleep(400);
                        port.Write("1");*/
                       // Thread.Sleep(400);
                        string s = port.ReadExisting();
                        //string b = ReadSerialData();
                        //dtring a=port.res
                        //int k=1;
                        if (s == "1"||s=="")
                        {
                            Thread.Sleep(500);//Directory.CreateDirectory(@"C:\Users\radar\Desktop\data\data_" + g);//创建新的文件夹
                            receiveMsg();
                            Thread.Sleep(200);
                           // datapro2();
                            datapro3();                           
                            g++;
                            //Thread.Sleep(400);
                            port.Write("1");
                            //k++;
                            //向下位机发送运动指令              
                            //Thread.Sleep(10000);//延时15s
                            //Console.WriteLine(g);
                        }
                        else
                        {
                            break;
                        }
                    }
                    Console.WriteLine("done");
                    port.Close();
                    continue;
                }
            }
        }
        //接受发送给本机ip特定端口的数据包         
        static void receiveMsg()
        {
            double wt;
            List<double> ad=new List<double>();
            for (int j = 1; j < 90; j++)
            {                
                //double w;
                // DateTime before = DateTime.Now;
                EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2014);
                byte[] buffer = new byte[1206];
                int length = client.ReceiveFrom(buffer, ref point);
                wt = (Convert.ToInt32(buffer[3]) * 256 + Convert.ToInt32(buffer[2])) * 0.01;//计算方位角，转化成double格式
                /*if (buffer != null)
                {*/                        
                if (wt > 90 && wt < 270)
                {
                    ad.Add(wt);            
                    foreach (byte i in buffer)  //将所有符合条件的数据一起收集到一个list中
                    {
                        Program.signal.Add(i);
                    }
                }
                else
                {
                    j = j - 1;
                }
                //DateTime after = DateTime.Now;
                //TimeSpan ts = after.Subtract(before);
                // Console.WriteLine(ts);
            }
            ad.Sort();
            if (ad[0] > 100)
            {

            }
            Console.WriteLine("数据采集完成");
            StreamWriter sa = new StreamWriter("C:/Users/cj/Desktop/data/"+g+"data" + ".txt");
            foreach(double w in ad)
            {
               // Console.WriteLine(w);
                sa.WriteLine(w.ToString());
            }
            sa.Close();
        }
        static void datapro()   //分组数据的三维坐标计算
        {
            DateTime before = DateTime.Now;
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
                                //double angle = (-wt - w * dt[i] - H_ang[i]);
                                double y = range[i] * Math.Cos(V_ang[i] / 180 * Math.PI) * Math.Sin(angle);
                                double x = range[i] * Math.Cos(V_ang[i] / 180 * Math.PI) * Math.Cos(angle);
                                double z = range[i] * Math.Sin(V_ang[i] / 180 * Math.PI);
                                /*data.Add(angle);
                               data.Add(range[i]);
                                data.Add(V_ang[i]);*/
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
                              /* double angle = (-wt - w * (dt[i - 16] + 50) - H_ang[i - 16]);
                                data.Add(angle);
                                data.Add(range[i]);
                                data.Add(V_ang[i-16]);*/
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
            DateTime after = DateTime.Now;
            TimeSpan ts = after.Subtract(before);
            Console.WriteLine(ts);
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
        static void datapro2()   //分组数据的三维坐标计算
        {
            #region
            List<double> data2 = new List<double>();//全部数据分组计算后的数据
            StreamWriter sw = new StreamWriter("C:/Users/cj/Desktop/data/" + g + "_data" + ".txt");   //创建文件
            for (int a = 0; a < signal.Count; a = a + 1206)   //数据分组
            {
                byte[] group = new byte[1206];
                for (int b = 0; b < 1206; b++)
                {
                    group[b] = signal[b + a];
                }
                //List<double> data = new List<double>();
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
                    double w = 0.0036;
                    for (int i = 0; i < 32; i++)
                    {
                        if (i < 16)
                        {
                            //double angle = (-wt - w * dt[i] - H_ang[i]);
                             double angle = (-wt - w * dt[i] - H_ang[i]) / 180 * Math.PI;
                             double y = range[i] * Math.Cos((V_ang[i]-0.1*(g-1))/ 180 * Math.PI) * Math.Sin(angle);
                             double x = range[i] * Math.Cos((V_ang[i]-0.1*(g-1))/ 180 * Math.PI) * Math.Cos(angle);
                             double z = range[i] * Math.Sin((V_ang[i]-0.1*(g-1))/ 180 * Math.PI);
                             data2.Add(x);
                             data2.Add(y);
                             data2.Add(z);
                            /*data2.Add(angle);
                            data2.Add(range[i]);
                            data2.Add(V_ang[i]);*/
                            data2.Add(reflet[i]);
                        }
                        else
                        {
                            double angle = (-wt - w * (dt[i - 16] + 50) - H_ang[i - 16]) / 180 * Math.PI;
                            double y = range[i] * Math.Cos(V_ang[i - 16] / 180 * Math.PI) * Math.Sin(angle);
                            double x = range[i] * Math.Cos(V_ang[i - 16] / 180 * Math.PI) * Math.Cos(angle);
                            double z = range[i] * Math.Sin(V_ang[i - 16] / 180 * Math.PI);
                            data2.Add(x);
                            data2.Add(y);
                            data2.Add(z);
                            /*double angle = (-wt - w * (dt[i - 16] + 50) - H_ang[i - 16]);
                            data2.Add(angle);
                            data2.Add(range[i]);
                            data2.Add(V_ang[i - 16]);*/
                            data2.Add(reflet[i]);
                        }
                    }
                }
            }
            foreach (double s in data2)
            {
                sw.WriteLine(s.ToString());
            }
            #endregion
            Console.WriteLine("数据写入文件完成");
            sw.Close();
            signal.Clear();
            data2.Clear();
        }
        static void datapro3()   //分组数据输出角度数据
        {
            #region
            List<double> data3 = new List<double>();//全部数据分组计算后的数据
            StreamWriter sw = new StreamWriter("C:/Users/cj/Desktop/data/" + g + "_angle_data" + ".txt");   //创建文件
            for (int a = 0; a < signal.Count; a = a + 1206)   //数据分组
            {
                byte[] group = new byte[1206];
                for (int b = 0; b < 1206; b++)
                {
                    group[b] = signal[b + a];
                }
                //List<double> data = new List<double>();
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
                    double w = 0.0036;
                    for (int i = 0; i < 32; i++)
                    {
                        if (i < 16)
                        {
                            double angle = (-wt - w * dt[i] - H_ang[i]);
                            /*double angle = (-wt - w * dt[i] - H_ang[i]) / 180 * Math.PI;
                            double y = range[i] * Math.Cos(V_ang[i] / 180 * Math.PI) * Math.Sin(angle);
                            double x = range[i] * Math.Cos(V_ang[i] / 180 * Math.PI) * Math.Cos(angle);
                            double z = range[i] * Math.Sin(V_ang[i] / 180 * Math.PI);
                            data2.Add(x);
                            data2.Add(y);
                            data2.Add(z);*/
                            data3.Add(angle);
                            data3.Add(range[i]);
                            data3.Add(V_ang[i] - 0.1 * (g - 1));
                            data3.Add(reflet[i]);
                        }
                        else
                        {
                            /*double angle = (-wt - w * (dt[i - 16] + 50) - H_ang[i - 16]) / 180 * Math.PI;
                            double y = range[i] * Math.Cos(V_ang[i - 16] / 180 * Math.PI) * Math.Sin(angle);
                            double x = range[i] * Math.Cos(V_ang[i - 16] / 180 * Math.PI) * Math.Cos(angle);
                            double z = range[i] * Math.Sin(V_ang[i - 16] / 180 * Math.PI);
                            data3.Add(x);
                            data3.Add(y);
                            data3.Add(z);*/
                            double angle = (-wt - w * (dt[i - 16] + 50) - H_ang[i - 16]);
                            data3.Add(angle);
                            data3.Add(range[i]);
                            data3.Add(V_ang[i - 16]-0.1*(g-1));
                            data3.Add(reflet[i]);
                        }
                    }
                }
            }
            foreach (double s in data3)
            {
                sw.WriteLine(s.ToString());
            }
            #endregion
            Console.WriteLine("数据写入文件完成2");
            sw.Close();
            signal.Clear();
            data3.Clear();
        }
    }
}