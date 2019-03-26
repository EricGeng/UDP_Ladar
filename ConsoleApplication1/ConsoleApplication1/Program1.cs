using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static Socket client;
        static void Main(string[] args)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.10"), 2014));
            Thread t = new Thread(sendMsg);
            t.Start();
            Thread t2 = new Thread(receiveMsg);
            t2.Start();
            Console.WriteLine("客户端开启");
           
           /* 
           */
         }
        //给特定的ip的主机端口发生送数据包
        static void sendMsg()
        {
            EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 2015);
            while (true)
            {
                string msg = Console.ReadLine();
                client.SendTo(Encoding.UTF8.GetBytes(msg), point);
            }
        }
        //接受发送给本机ip特定端口的数据包         
        static void receiveMsg()
        {
            while (true)
            {  
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer = new byte[2048];
                int length = client.ReceiveFrom(buffer, ref point);
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Console.WriteLine(point.ToString() + message);
                 string i = Console.ReadLine();
                if (i == "s")
                {
                    break;
                }
            }
        }
     }
}
