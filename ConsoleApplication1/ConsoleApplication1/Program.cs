using System;
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
        static List<byte> signal = new List<byte>();
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
            while (true)
            {
                string key = Console.ReadLine();
                if (key == "s")
                {
                    client.SendTo(stop, point);
                    continue;
                }
                else if (key == "5")
                {
                    client.SendTo(f_hz, point);
                    receiveMsg();
                    datapro();
                    Gathertxt();
                    continue;
                }
                else if (key == "t")
                {
                    client.SendTo(t_hz, point);
                    receiveMsg();
                    datapro();
                    // Gathertxt();
                    continue;
                }
                else if (key == "tw")
                {
                    client.SendTo(tw_hz, point);
                    receiveMsg();
                    datapro();
                    //Gathertxt();
                    continue;
                }
            }
        }
        //接受发送给本机ip特定端口的数据包         
        static void receiveMsg()
        {
            double wt;
            // List<byte> signal = new List<byte>();           
            for (int j = 0; j < 1800; j++)
            {
                EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2014);
                byte[] buffer = new byte[1206];
                int length = client.ReceiveFrom(buffer, ref point);
                //string meg = BitConverter.ToString(buffer);
                //Console.WriteLine(point.ToString() + "点云数据16进制：" + meg);//显示数据到控制台查看是否正非常
                wt = (Convert.ToInt32(buffer[3]) * 256 + Convert.ToInt32(buffer[2])) * 0.01;//计算方位角，转化成double格式
                // List<double> zuobiao = datapro(wt,buffer[4]);
                /*foreach (double s in zuobiao)
                 {Console.WriteLine(s);//遍历list}*/
                //Thread.Sleep(100);
                /*if (wt > 0 && wt < 180)
                {
                    foreach (byte i in buffer)  //将所有符合条件的数据一起收集到一个list中
                    {
                        Program.signal.Add(i);
                    }
                }*/
                foreach (byte i in buffer)  //将所有符合条件的数据一起收集到一个list中
                {
                    Program.signal.Add(i);
                }
                // Console.WriteLine(j);
            }
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
                using (StreamWriter sw = new StreamWriter(@"C:\Users\radar\Desktop\data\" + f + "_data" + ".txt"))
                {
                    List<double> data = new List<double>();
                    for (int c = 0; c < 12; c++)
                    {
                        double wt = (Convert.ToInt32(group[3]) * 256 + Convert.ToInt32(group[2])) * 0.01;
                        double[] range = new double[32];
                        double[] reflet = new double[32];
                        for (int d = 0; d < 32; d++)
                        {
                            int e = 4;
                            range[d] = (Convert.ToInt32(group[e + 1]) * 256 + Convert.ToInt32(group[e])) * 4;
                            reflet[d] = Convert.ToInt32(group[e + 2]);
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
                                double angle = -wt - w * dt[i] - H_ang[i];
                                double y = range[i] * Math.Cos(V_ang[i]) * Math.Sin(angle);
                                double x = range[i] * Math.Cos(V_ang[i]) * Math.Cos(angle);
                                double z = range[i] * Math.Sin(V_ang[i]);
                                data.Add(x);
                                data.Add(y);
                                data.Add(z);
                                data.Add(reflet[i]);
                            }
                            else
                            {
                                double angle = -wt - w * (dt[i - 16] + 50) - H_ang[i - 16];
                                double y = range[i] * Math.Cos(V_ang[i - 16]) * Math.Sin(angle);
                                double x = range[i] * Math.Cos(V_ang[i - 16]) * Math.Cos(angle);
                                double z = range[i] * Math.Sin(V_ang[i - 16]);
                                data.Add(x);
                                data.Add(y);
                                data.Add(z);
                                data.Add(reflet[i]);
                            }
                        }
                        foreach (double s in data)
                        {
                            sw.WriteLine(s.ToString());
                        }
                        //sw.Close();
                    }
                    #endregion
                }
                f++;
            }
            #region   16进制格式写入文件
            /*  using (StreamWriter sw = new StreamWriter(@"C:\Users\radar\Desktop\" + j + "_R" + ".txt"))
              {
                  for (int k = 0; k < 1106; k = k + 100)
                  {
                      byte[] pit = new byte[100];
                      for (int z = 0; z < 100; z = z + 1)
                      {
                          int m = k + z;
                          pit[z] = buffer[m];
                      }
                      string pz = BitConverter.ToString(pit);
                      //int pi = Convert.ToInt32(pit[z]);//转化16进制到十进制                       
                      sw.WriteLine(pz);
                      // fs.WriteByte(pit[z]);
                  }
              }*/
            #endregion
            Console.WriteLine("数据写入完成");
            // datapro(wt);
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
        static void change_data()//暂时没想好写啥
        {

        }
        static List<double> datapro(double wt, double range)//数据处理
        {
            double[] H_ang = { -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35 };
            double[] V_ang = { -19, -17, -15, -13, -11, -9, -7, -5, -3, -1, 1, 3, 5, 7, 9, 11 };
            double[] dt = { 0, 3.125, 6.25, 9.375, 12.5, 15.625, 18.75, 21.875, 25, 28.125, 31.25, 34.375, 37.5, 40.625, 43.75, 46.875 };
            //double range = 596.233;
            double w = 0.0018;
            //double[] data = { };
            List<double> data = new List<double>();
            for (int i = 0; i < 32; i++)
            {
                if (i < 16)
                {
                    double angle = -wt - w * dt[i] - H_ang[i];
                    double y = range * Math.Cos(V_ang[i]) * Math.Sin(angle);
                    double x = range * Math.Cos(V_ang[i]) * Math.Cos(angle);
                    double z = range * Math.Sin(V_ang[i]);
                    data.Add(x);
                    data.Add(y);
                    data.Add(z);
                }
                else
                {
                    double angle = -wt - w * (dt[i - 16] + 50) - H_ang[i - 16];
                    double y = range * Math.Cos(V_ang[i - 16]) * Math.Sin(angle);
                    double x = range * Math.Cos(V_ang[i - 16]) * Math.Cos(angle);
                    double z = range * Math.Sin(V_ang[i - 16]);
                    data.Add(x);
                    data.Add(y);
                    data.Add(z);
                }
            }
            return data;
        }
        static byte[] GetFileData(string fileUrl)//读取txt文件数据，从ASCII码转换过来，较复杂，再不影响实时性的情况下，不适用
        {
            FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
                byte[] n_buf = new byte[fs.Length];
                int m = 0;
                for (int j = 0; j < fs.Length; j = j + 1)
                {
                    if (buffur[j] == 45)               //处理ASCII转化时候产生的“-”的影响，但是未消除换行的影响，导致每组数据有201个，每组最后的10再ASCII中表示换行
                    {
                        n_buf[m] = buffur[j + 1];
                        j++;
                        m++;
                    }
                    else
                    {
                        n_buf[m] = buffur[j];
                        m++;
                    }
                }
                //int i = Convert.ToInt32(buffur[0]);
                // Console.WriteLine(i);
                return buffur;
            }
            /*catch (Exception ex)
            {
                //MessageBoxHelper.ShowPrompt(ex.Message);
                return null;
            }*/
            finally
            {
                if (fs != null)//关闭资源
                {
                    fs.Close();
                }
            }
        }
    }
}
