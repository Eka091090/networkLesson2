using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace lesson2;

internal class Client
{
    private static bool isRunning = true;
    public static async Task SendMsg(string name)
    {   
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16874);
        UdpClient udpClient = new UdpClient();

        while(isRunning)
        {
            System.Console.WriteLine("Enter message");
            string text = Console.ReadLine();
            
            if(text.Equals("Exit", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(text))
            {
                isRunning = false;
                Message msg1 = new Message(name, text);
                string responseMsgJs1 = msg1.ToJson();
                byte[] responseData1 = Encoding.UTF8.GetBytes(responseMsgJs1);
                await udpClient.SendAsync(responseData1, ep);
                break;
            }

            Message msg = new Message(name, text);
            string responseMsgJs = msg.ToJson();
            byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJs);
            await udpClient.SendAsync(responseData, ep);

            byte[] answerData = udpClient.Receive(ref ep);
            string answerMsgJs = Encoding.UTF8.GetString(answerData);
            Message answerMsg = Message.FromJson(answerMsgJs);
            System.Console.WriteLine(answerMsg.ToString());
        }
        udpClient.Close();       
    }
}