using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Protocol;
using MQTTnet.Server;

using System;
using System.Text;
using System.Threading;

namespace MqttServerTest
{

    class Program
    {
        private static MqttServer mqttServer = null;
        static void Main(string[] args)
        {

        }

        private static async void StartServer()
        {
            try
            {
                // 1. 创建 MQTT 连接验证，用于连接鉴权



                // 2. 创建 MqttServerOptions 的实例，用来定制 MQTT 的各种参数
                MqttServerOptions options = new MqttServerOptions();

                // 3. 设置各种选项
                // 客户端鉴权
                //options.ConnectionValidator = connectionValidatorDelegate;

                // 设置服务器端地址和端口号
                options.DefaultEndpointOptions.Port = 1883;

                // 4. 创建 MqttServer 实例
                mqttServer = new MqttFactory().CreateMqttServer(options);

                //// 5. 设置 MqttServer 的属性
                //// 设置消息订阅通知
                //mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(SubScribedTopic);
                //// 设置消息退订通知
                //mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(UnScribedTopic);
                //// 设置消息处理程序
                //mqttServer.UseApplicationMessageReceivedHandler(MessageReceived);
                //// 设置客户端连接成功后的处理程序
                //mqttServer.UseClientConnectedHandler(ClientConnected);
                //// 设置客户端断开后的处理程序
                //mqttServer.UseClientDisconnectedHandler(ClientDisConnected);

                // 启动服务器
                await mqttServer.StartAsync();


                Console.WriteLine("服务器启动成功！直接按回车停止服务");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.Write($"服务器启动失败:{ex}");
            }
        }
        public static async Task Validating_Connections()
        {
            /*
             * This sample starts a simple MQTT server which will check for valid credentials and client ID.
             *
             * See _Run_Minimal_Server_ for more information.
             */

            var mqttFactory = new MqttFactory();

            var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();

            using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
            {
                // Setup connection validation before starting the server so that there is 
                // no change to connect without valid credentials.
                mqttServer.ValidatingConnectionAsync += e =>
                {
                    if (e.ClientId != "ValidClientId")
                    {
                        e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                    }
                    if (e.UserName != "ValidUser")
                    {
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                    if (e.Password != "SecretPassword")
                    {
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                    return Task.CompletedTask;
                };

                await mqttServer.StartAsync();

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();

                await mqttServer.StopAsync();
            }
        }

        // 客户端发起订阅主题通知
        private static void SubScribedTopic(MqttServerClientSubscribedTopicEventArgs args)
        {
            // 获取客户端识别码
            string clientId = args.ClientId;
            // 获取客户端发起的订阅主题
            string topic = args.TopicFilter.Topic;

            Console.WriteLine($"客户端[{clientId}]已订阅主题:{topic}");
        }

        // 客户端取消主题订阅通知
        private static void UnScribedTopic(MqttServerClientUnsubscribedTopicEventArgs args)
        {
            // 获取客户端识别码
            string clientId = args.ClientId;
            // 获取客户端发起的订阅主题
            string topic = args.TopicFilter;

            Console.WriteLine($"客户端[{clientId}]已退订主题:{topic}");
        }

        // 接收客户端发送的消息
        private static void MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            // 获取消息的客户端识别码
            string clientId = args.ClientId;
            // 获取消息的主题
            string topic = args.ApplicationMessage.Topic;
            // 获取发送的消息内容
            string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            // 获取消息的发送级别(Qos)
            var qos = args.ApplicationMessage.QualityOfServiceLevel;
            // 获取消息的保持形式
            bool retain = args.ApplicationMessage.Retain;

            Console.WriteLine($"客户端[{clientId}] >> 主题: [{topic}] 内容: [{payload}] Qos: [{qos}] Retain:[{retain}]");

        }

        // 客户端连接成功后的的处理通知
        private static void ClientConnected(MqttServerClientConnectedEventArgs args)
        {
            // 获取客户端识别码
            string clientId = args.ClientId;

            Console.WriteLine($"新客户端[{clientId}] 加入");
        }

        // 客户端断开连接通知
        private static void ClientDisConnected(MqttServerClientDisconnectedEventArgs args)
        {
            // 获取客户端识别码
            string clientId = args.ClientId;

            Console.WriteLine($"新客户端[{clientId}] 已经离开");
        }
    }
}