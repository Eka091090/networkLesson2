using System.Net;
using System.Net.Sockets;
using System.Text;

namespace lesson2;

internal class Server
{
    private static bool isRunning = true;
    public static void AcceptMsg()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        UdpClient udpClient = new UdpClient(16874);
        System.Console.WriteLine("Server wait message...");

        try
        {
            while(isRunning)
            {
                if (udpClient.Available > 0)
                {
                    byte[] buffer = udpClient.Receive(ref ep);
                    string data = Encoding.UTF8.GetString(buffer);

                    if (data.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    {
                        isRunning = false;
                        break;
                    }

                    new Thread(() => 
                    {
                        Message msg = Message.FromJson(data);
                        System.Console.WriteLine(msg.ToString());
                        
                        Message responseMsg = new Message("Server", "Message accept on server!");
                        string responseMsgJs = responseMsg.ToJson();
                        byte[] responseDate = Encoding.UTF8.GetBytes(responseMsgJs);
                        udpClient.Send(responseDate, ep);
                    }).Start();
                } 
            }
            udpClient.Close();
        }
        catch
        {
            System.Console.WriteLine("Сlient has stopped execution");
        }
    }
}