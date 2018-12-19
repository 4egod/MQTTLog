using Industrial;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTLog
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Start();

            try
            {
                byte qos = Properties.Settings.Default.qos;
                if (qos > 3 && qos < 0)
                {
                    throw new InvalidOperationException("Error: Invalid QoS param...");
                }

                string ip, user, password;
                ip = Properties.Settings.Default.ip;
                if (string.IsNullOrEmpty(ip))
                {
                    throw new InvalidOperationException("Error: Invalid IP Address...");
                }

                user = Properties.Settings.Default.user;
                if (string.IsNullOrEmpty(user))
                {
                    user = null;
                }

                password = Properties.Settings.Default.password;
                if (string.IsNullOrEmpty(password))
                {
                    password = null;
                }

                ushort port = Properties.Settings.Default.port == 0 ? (ushort)1883 : Properties.Settings.Default.port;

                string empty = "empty";
                Debug.Print($"Address: {ip}:{port}, User: {user??empty}, Password: {password??"empty"}, QoS: {qos}");

                MqttClient client = new MqttClient(ip, port, false, null, null, MqttSslProtocols.None);

                string clientId = Guid.NewGuid().ToString();
                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                client.ConnectionClosed += Client_ConnectionClosed;
                client.MqttMsgPublished += Client_MqttMsgPublished;
                client.MqttMsgSubscribed += Client_MqttMsgSubscribed;

                if (user != null && password != null)
                {
                    client.Connect(clientId, user, password);
                }
                else
                {
                    client.Connect(clientId);
                }    

                client.Subscribe(new string[] { "#" }, new byte[] { qos });

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
            }

            Debug.Print("Press ENTER for exit...");
            Console.ReadLine();
        }

        private static void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Debug.Print($"Subscribed {e.MessageId} {e.GrantedQoSLevels.ToHex()}");
        }

        private static void Client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Debug.Print("Published...");
        }

        private static void Client_ConnectionClosed(object sender, EventArgs e)
        {
            Debug.Print("Connection lost...");
        }

        private static void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            Debug.Print("");
            string header = $"{e.Topic} ({e.QosLevel}, {e.Retain}, {e.DupFlag}):";
            Debug.Print(header);
            string msg = Encoding.UTF8.GetString(e.Message);
            Debug.Print(msg);
        }
    }
}
