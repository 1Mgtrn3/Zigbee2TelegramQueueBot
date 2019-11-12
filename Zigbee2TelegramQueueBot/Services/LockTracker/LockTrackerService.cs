using Microsoft.Extensions.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Options;
using MQTTnet;
using System.Text;
using Newtonsoft.Json;

namespace Zigbee2TelegramQueueBot.Services.LockTracker
{
    public class LockTrackerService : ILockTrackerService//, INotifyPropertyChanged
    {
        private readonly IOptions<LockTrackerConfiguration> _lockTrackerConfig;
        private readonly ILogHelper _logHelper;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsLockTrackerInstalled { get; set; }
        private IManagedMqttClient mqttClient { get; set; }
        private bool PreviousState { get; set; }
        private bool isRoomFree;
        public bool IsRoomFree
        {
            get { return isRoomFree; }
            set
            {
                isRoomFree = value;
                OnPropertyChanged("IsRoomFree");
            }
        }

        public LockTrackerService(IOptions<LockTrackerConfiguration> lockTrackerConfig, ILogHelper logHelper)
        {

            _lockTrackerConfig = lockTrackerConfig;
            _logHelper = logHelper;

            _logHelper.Log("HJ64K54J45", "LockTracker instance created", LogLevel.Information);
            IsLockTrackerInstalled = _lockTrackerConfig.Value.IsLockTrackerInstalled;
            if (IsLockTrackerInstalled && !_lockTrackerConfig.Value.TestMode)
            {
                Subscribe();
            }


        }

        async void Subscribe()
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(_lockTrackerConfig.Value.ClientId)
                    .WithTcpServer(_lockTrackerConfig.Value.TcpServerIp, _lockTrackerConfig.Value.TcpServerPort)
                    //.WithWebSocketServer("172.30.3.103:37136")
                    .WithCleanSession()
                ).Build();
            mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(_lockTrackerConfig.Value.MqttTopic).Build());

            await mqttClient.StartAsync(options);

            mqttClient.UseApplicationMessageReceivedHandler(HandleLockTrackerMessage);
        }

        void HandleLockTrackerMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var LockTrackerMessage = JsonConvert.DeserializeObject<LockTrackerMessage>(Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload));
            _logHelper.Log("HJKF4J343JK", $"GOT A MESSAGE FROM MQTT: |{LockTrackerMessage.contact}|", LogLevel.Information);

            if (PreviousState != LockTrackerMessage.contact)
            {
                PreviousState = LockTrackerMessage.contact;
                IsRoomFree = !LockTrackerMessage.contact;

            }

        }

    }
}
