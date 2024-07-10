using MQTTnet;
using MQTTnet.Client;

namespace MQTTClientTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter command 'exit' to stop sending meesages");
            string message = string.Empty;
            try
            {

            PublishMessage.Publish_Multiple_Application_Messages();
            }
            catch (Exception)
            {

                throw;
            }
            do
            {
       
                Console.Write("Enter your next message:");
                message = Console.ReadLine();
            } while (!message.Equals("exit", StringComparison.OrdinalIgnoreCase));
        }
    }
}