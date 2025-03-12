using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using DAL.Entities;
using DAL.Entities.Devices;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Utilities.Logging;

namespace Services.Drivers
{
    public class MQTTDriver : Driver
    {
        private IMqttClient mqttClient = null;

        private MQTTDevice Source;
        String FinalUrl;
        List<MQTTDataPoint> MQTTDataPoints = new List<MQTTDataPoint>();
        public MQTTDriver(IAquariumLogger logger, MQTTDevice src, List<MQTTDataPoint> datapoints) : base(logger, src.DeviceName)
        {
            this.Source = src;
            this.MQTTDataPoints = datapoints;
        }

        public override async Task Connect()
        {
            try
            {
                FinalUrl = Source.Host + ":" + Source.Port;
                
                log.Information("Created Client - trying to connect to {Url}", FinalUrl);

                var mqttFactory = new MqttClientFactory();
                
                var mqttClient = mqttFactory.CreateMqttClient();

                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(Source.Host, Source.Port)
                    .WithTimeout(TimeSpan.FromSeconds(60))
                    .WithCleanSession()
                    .WithClientId(Guid.NewGuid().ToString())
                    .Build();                
                
                mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var topic = e.ApplicationMessage.Topic;
                    var payloadString = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    try
                    {
                        var mqttMessage = JsonConvert.DeserializeObject<MQTTItem>(payloadString);

                        if (mqttMessage != null)
                        {
                            HyperEntity? entity;
                            if (mqttMessage.Value is bool boolValue)
                            {
                                entity = new BinarySample{ Value = boolValue };
                            }
                            else
                            {
                                entity = new NumericSample { Value = Convert.ToSingle(mqttMessage.Value) };
                            }

                            Measurements.GetOrAdd(topic, _ => []).Add(entity);

                            log.Information("Received MQTT JSON message: Topic={Topic}, Value={Value}", topic, mqttMessage.Value);
                        }
                        else
                        {
                            log.Error("Deserialized MQTT message is null: {Payload}", payloadString);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Failed to parse MQTT JSON payload: {Payload}", payloadString);
                    }

                    await Task.CompletedTask;
                };
                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                
                await mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter("#").Build());
                IsConnected = true;
                
                
                log.Information("The MQTT client is connected.");
                
            }
            catch (Exception ex)
            {
                log.Fatal("Could not connect to MQTT Broker", ex);
            }

        }
        
        
        public async override Task Disconnect()
        {
            try
            {

                log.Information("Stopping Client");
                if (mqttClient != null)
                {
                    await mqttClient.DisconnectAsync();
                    mqttClient.Dispose();
                }
            }
            catch (Exception e)
            {
                log.Warning("Stopping failed " + e.ToString());
            }
        }


    }
}
