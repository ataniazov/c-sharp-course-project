namespace SmartHome
{
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Extensions.ManagedClient;
    using MQTTnet.Formatter;
    using MQTTnet.Packets;
    using MQTTnet.Protocol;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class FormSmartHome : Form
    {
        //private readonly Timer timer;

        private IManagedMqttClient managedMqttClient;

        public enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected
        }

        private const string topicBase = "SmartHome";

        private bool clientSubscribed = false;

        string windowSpeedStateValue = "fast";
        string windowStateValue = "close";

        //private bool altF4Pressed = false;

        public FormSmartHome()
        {
            this.InitializeComponent();

            //timer = new Timer
            //{
            //    AutoReset = true,
            //    Interval = 1000
            //};

            //this.timer.Elapsed += this.TimerElapsed;
            //this.timer.Enabled = true;

            //this.KeyPreview = true;
        }

        private void LogToConsole(string message)
        {
            if (this.textBoxConsole.InvokeRequired)
            {
                this.textBoxConsole.BeginInvoke((MethodInvoker)(() => LogToConsole(message)));
                return;
            }

            const int MaxConsoleLength = 8000;

            string text = $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}{this.textBoxConsole.Text}";

            if (text.Length > MaxConsoleLength)
            {
                text = text.Substring(0, MaxConsoleLength);
            }

            this.textBoxConsole.Text = text;
        }

        private void SetConnectionState(ConnectionState state)
        {
            if (state == ConnectionState.Connected)
            {
                this.buttonMQTTConnection.Enabled = true;
                this.buttonMQTTConnection.Text = "CONNECTED";
                this.buttonMQTTConnection.BackColor = Color.Lime;
                //this.buttonMQTTConnection.ForeColor = Color.Black;

                this.textBoxClientID.Enabled = false;
                this.textBoxHost.Enabled = false;
                this.textBoxPort.Enabled = false;
                this.textBoxUserName.Enabled = false;
                this.textBoxPassword.Enabled = false;

                this.groupBoxRemoteClient.Enabled = true;
            }
            else if (state == ConnectionState.Connecting)
            {
                this.buttonMQTTConnection.Enabled = false;
                this.buttonMQTTConnection.Text = "CONNECTING";
                this.buttonMQTTConnection.BackColor = Color.Yellow;
                //this.buttonMQTTConnection.ForeColor = Color.Black;

                this.textBoxClientID.Enabled = false;
                this.textBoxHost.Enabled = false;
                this.textBoxPort.Enabled = false;
                this.textBoxUserName.Enabled = false;
                this.textBoxPassword.Enabled = false;

                this.groupBoxRemoteClient.Enabled = false;
            }
            else
            {
                this.buttonMQTTConnection.Enabled = true;
                this.buttonMQTTConnection.Text = "CONNECT";
                this.buttonMQTTConnection.BackColor = SystemColors.ControlLight;
                //this.buttonMQTTConnection.ForeColor = SystemColors.ControlText;

                this.textBoxClientID.Enabled = true;
                this.textBoxHost.Enabled = true;
                this.textBoxPort.Enabled = true;
                this.textBoxUserName.Enabled = true;
                this.textBoxPassword.Enabled = true;

                this.groupBoxRemoteClient.Enabled = false;
            }
        }

        private Task OnMqttConnected(MqttClientConnectedEventArgs _)
        {
            //MessageBox.Show("MQTT Connected", "ConnectHandler", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.BeginInvoke(new MethodInvoker(() => SetConnectionState(ConnectionState.Connected)));

            return Task.CompletedTask;
        }

        private Task OnMqttDisconnected(MqttClientDisconnectedEventArgs _)
        {
            //MessageBox.Show("MQTT Disconnected", "ConnectHandler", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.SetSubscriptionState(false);
                this.SetConnectionState(ConnectionState.Disconnected);
            }));

            return Task.CompletedTask;
        }

        private class StatusPayload
        {
            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public class WindowPayload
        {
            [JsonProperty("speed")]
            public string Speed { get; set; }

            [JsonProperty("state")]
            public string State { get; set; }
        }

        public class LightPayload
        {
            [JsonProperty("lux")]
            public string Lux { get; set; }

            [JsonProperty("lumen_surf")]
            public string LumenSurface { get; set; }

            [JsonProperty("lumen_iso")]
            public string LumenIso { get; set; }

            [JsonProperty("lumen_lamb")]
            public string LumenLambertian { get; set; }

            [JsonProperty("lumen_angle")]
            public string LumenAngle { get; set; }
        }

        public class StatePayload
        {
            [JsonProperty("weather")]
            public string Weather { get; set; }

            [JsonProperty("window")]
            public WindowPayload Window { get; set; }

            [JsonProperty("heater")]
            public string Heater { get; set; }

            [JsonProperty("temp")]
            public string Temp { get; set; }

            [JsonProperty("fan")]
            public string Fan { get; set; }

            [JsonProperty("light")]
            public LightPayload Light { get; set; }

            [JsonProperty("lamp")]
            public string Lamp { get; set; }
        }

        private static float? ParseNullableFloat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        private void SetRemoteClientStatus(string status)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetRemoteClientStatus), status);
                return;
            }

            if (string.Equals(status, "online", StringComparison.OrdinalIgnoreCase))
            {
                this.labelRemoteClientStatus.Text = "ONLINE";
                this.labelRemoteClientStatus.BackColor = Color.Lime;
                this.labelRemoteClientStatus.ForeColor = SystemColors.ControlText;

                //this.groupBoxRemoteClientState.Enabled = true;
            }
            else if (string.Equals(status, "offline", StringComparison.OrdinalIgnoreCase))
            {
                this.labelRemoteClientStatus.Text = "OFFLINE";
                this.labelRemoteClientStatus.BackColor = Color.Red;
                this.labelRemoteClientStatus.ForeColor = SystemColors.HighlightText;

                //this.groupBoxRemoteClientState.Enabled = false;
            }
            else
            {
                this.labelRemoteClientStatus.Text = "UNKNOWN";
                this.labelRemoteClientStatus.BackColor = SystemColors.ControlDark;
                this.labelRemoteClientStatus.ForeColor = SystemColors.ControlText;

                //this.groupBoxRemoteClientState.Enabled = false;
            }
        }

        private void SetWeatherStateLabel(string state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetWeatherStateLabel), state);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownWeatherStateLabel()
            {
                this.labelWeatherState.Text = "UNKNOWN";
                this.labelWeatherState.BackColor = SystemColors.ControlDark;
                this.labelWeatherState.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                SetUnknownWeatherStateLabel();
                return;
            }

            string value = state.Trim();

            if (string.Equals(value, "dry", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWeatherState.Text = "DRY";
                this.labelWeatherState.BackColor = Color.Lime;
                this.labelWeatherState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "damp", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWeatherState.Text = "DAMP";
                this.labelWeatherState.BackColor = Color.Yellow;
                this.labelWeatherState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "wet", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWeatherState.Text = "WET";
                this.labelWeatherState.BackColor = Color.Red;
                this.labelWeatherState.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                SetUnknownWeatherStateLabel();
            }
        }

        private void SetWindowSpeedStateLabel(string state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetWindowSpeedStateLabel), state);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownWindowSpeedStateLabel()
            {
                this.labelWindowSpeedState.Text = "UNKNOWN";
                this.labelWindowSpeedState.BackColor = SystemColors.ControlDark;
                this.labelWindowSpeedState.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                SetUnknownWindowSpeedStateLabel();
                return;
            }

            string value = state.Trim();

            if (string.Equals(value, "slow", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowSpeedState.Text = "SLOW";
                this.labelWindowSpeedState.BackColor = Color.Lime;
                this.labelWindowSpeedState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "medium", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowSpeedState.Text = "MEDIUM";
                this.labelWindowSpeedState.BackColor = Color.Yellow;
                this.labelWindowSpeedState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "fast", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowSpeedState.Text = "FAST";
                this.labelWindowSpeedState.BackColor = Color.Red;
                this.labelWindowSpeedState.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                SetUnknownWindowSpeedStateLabel();
            }
        }

        private void SetWindowStateLabel(string state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetWindowStateLabel), state);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownWindowStateLabel()
            {
                this.labelWindowState.Text = "UNKNOWN";
                this.labelWindowState.BackColor = SystemColors.ControlDark;
                this.labelWindowState.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                SetUnknownWindowStateLabel();
                return;
            }

            string value = state.Trim();

            if (string.Equals(value, "closed", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowState.Text = "CLOSED";
                this.labelWindowState.BackColor = Color.Blue;
                this.labelWindowState.ForeColor = SystemColors.HighlightText;
            }
            else if (string.Equals(value, "open", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowState.Text = "OPEN";
                this.labelWindowState.BackColor = Color.Lime;
                this.labelWindowState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "opening", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowState.Text = "OPENING";
                this.labelWindowState.BackColor = Color.Red;
                this.labelWindowState.ForeColor = SystemColors.HighlightText;
            }
            else if (string.Equals(value, "closing", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowState.Text = "CLOSING";
                this.labelWindowState.BackColor = Color.Red;
                this.labelWindowState.ForeColor = SystemColors.HighlightText;
            }
            else if (string.Equals(value, "idle", StringComparison.OrdinalIgnoreCase))
            {
                this.labelWindowState.Text = "IDLE";
                this.labelWindowState.BackColor = Color.Yellow;
                this.labelWindowState.ForeColor = SystemColors.ControlText;
            }
            else
            {
                SetUnknownWindowStateLabel();
            }
        }

        private void SetTemperatureDegrees(string degrees)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetTemperatureDegrees), degrees);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownTemperatureDegrees()
            {
                this.labelTemperatureDegrees.Text = "UNKNOWN";
                this.labelTemperatureDegrees.BackColor = SystemColors.ControlDark;
                this.labelTemperatureDegrees.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(degrees))
            {
                SetUnknownTemperatureDegrees();
                return;
            }

            string value = degrees.Trim();

            if (string.Equals(value, "unknown", StringComparison.OrdinalIgnoreCase))
            {
                SetUnknownTemperatureDegrees();
            }
            else
            {
                this.labelTemperatureDegrees.Text = degrees + " °C";
                this.labelTemperatureDegrees.BackColor = SystemColors.Control;
                this.labelTemperatureDegrees.ForeColor = SystemColors.ControlText;
            }
        }

        private void SetHeaterStateLabel(string state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetHeaterStateLabel), state);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownHeaterStateLabel()
            {
                this.labelHeaterState.Text = "UNKNOWN";
                this.labelHeaterState.BackColor = SystemColors.ControlDark;
                this.labelHeaterState.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                SetUnknownHeaterStateLabel();
                return;
            }

            string value = state.Trim();

            if (string.Equals(value, "off", StringComparison.OrdinalIgnoreCase))
            {
                this.labelHeaterState.Text = "OFF";
                this.labelHeaterState.BackColor = Color.Blue;
                this.labelHeaterState.ForeColor = SystemColors.HighlightText;
            }
            else if (string.Equals(value, "on", StringComparison.OrdinalIgnoreCase))
            {
                this.labelHeaterState.Text = "ON";
                this.labelHeaterState.BackColor = Color.Red;
                this.labelHeaterState.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                SetUnknownHeaterStateLabel();
            }
        }

        private void SetFanSpeedStateLabel(string state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetFanSpeedStateLabel), state);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownFanSpeedStateLabel()
            {
                this.labelFanSpeedState.Text = "UNKNOWN";
                this.labelFanSpeedState.BackColor = SystemColors.ControlDark;
                this.labelFanSpeedState.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                SetUnknownFanSpeedStateLabel();
                return;
            }

            string value = state.Trim();

            if (string.Equals(value, "off", StringComparison.OrdinalIgnoreCase))
            {
                this.labelFanSpeedState.Text = "OFF";
                this.labelFanSpeedState.BackColor = Color.Blue;
                this.labelFanSpeedState.ForeColor = SystemColors.HighlightText;
            }
            else if (string.Equals(value, "slow", StringComparison.OrdinalIgnoreCase))
            {
                this.labelFanSpeedState.Text = "SLOW";
                this.labelFanSpeedState.BackColor = Color.Lime;
                this.labelFanSpeedState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "medium", StringComparison.OrdinalIgnoreCase))
            {
                this.labelFanSpeedState.Text = "MEDIUM";
                this.labelFanSpeedState.BackColor = Color.Yellow;
                this.labelFanSpeedState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "fast", StringComparison.OrdinalIgnoreCase))
            {
                this.labelFanSpeedState.Text = "FAST";
                this.labelFanSpeedState.BackColor = Color.Red;
                this.labelFanSpeedState.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                SetUnknownFanSpeedStateLabel();
            }
        }

        private void SetLuminousFluxLumen(string lumen)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetLuminousFluxLumen), lumen);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownLuminousFluxLumen()
            {
                this.labelLuminousFluxLumen.Text = "UNKNOWN";
                this.labelLuminousFluxLumen.BackColor = SystemColors.ControlDark;
                this.labelLuminousFluxLumen.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(lumen))
            {
                SetUnknownLuminousFluxLumen();
                return;
            }

            string value = lumen.Trim();

            if (string.Equals(value, "unknown", StringComparison.OrdinalIgnoreCase))
            {
                SetUnknownLuminousFluxLumen();
            }
            else
            {
                this.labelLuminousFluxLumen.Text = lumen + " lm";
                this.labelLuminousFluxLumen.BackColor = SystemColors.Control;
                this.labelLuminousFluxLumen.ForeColor = SystemColors.ControlText;
            }
        }

        private void SetIlluminanceLux(string lux)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetIlluminanceLux), lux);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownIlluminanceLux()
            {
                this.labelIlluminanceLux.Text = "UNKNOWN";
                this.labelIlluminanceLux.BackColor = SystemColors.ControlDark;
                this.labelIlluminanceLux.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(lux))
            {
                SetUnknownIlluminanceLux();
                return;
            }

            string value = lux.Trim();

            if (string.Equals(value, "unknown", StringComparison.OrdinalIgnoreCase))
            {
                SetUnknownIlluminanceLux();
            }
            else
            {
                this.labelIlluminanceLux.Text = lux + " lx";
                this.labelIlluminanceLux.BackColor = SystemColors.Control;
                this.labelIlluminanceLux.ForeColor = SystemColors.ControlText;
            }
        }

        private void SetLampLevelStateLabel(string state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetLampLevelStateLabel), state);
                return;
            }

            if (!this.groupBoxRemoteClientState.Enabled)
            {
                return;
            }

            void SetUnknownLampLevelStateLabel()
            {
                this.labelLampLevelState.Text = "UNKNOWN";
                this.labelLampLevelState.BackColor = SystemColors.ControlDark;
                this.labelLampLevelState.ForeColor = SystemColors.ControlText;
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                SetUnknownLampLevelStateLabel();
                return;
            }

            string value = state.Trim();

            if (string.Equals(value, "off", StringComparison.OrdinalIgnoreCase))
            {
                this.labelLampLevelState.Text = "OFF";
                this.labelLampLevelState.BackColor = Color.Blue;
                this.labelLampLevelState.ForeColor = SystemColors.HighlightText;
            }
            else if (string.Equals(value, "low", StringComparison.OrdinalIgnoreCase))
            {
                this.labelLampLevelState.Text = "LOW";
                this.labelLampLevelState.BackColor = Color.Lime;
                this.labelLampLevelState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "medium", StringComparison.OrdinalIgnoreCase))
            {
                this.labelLampLevelState.Text = "MEDIUM";
                this.labelLampLevelState.BackColor = Color.Yellow;
                this.labelLampLevelState.ForeColor = SystemColors.ControlText;
            }
            else if (string.Equals(value, "high", StringComparison.OrdinalIgnoreCase))
            {
                this.labelLampLevelState.Text = "HIGH";
                this.labelLampLevelState.BackColor = Color.Red;
                this.labelLampLevelState.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                SetUnknownLampLevelStateLabel();
            }
        }

        private Task OnMqttMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var topic = eventArgs.ApplicationMessage.Topic;
            var payload = eventArgs.ApplicationMessage.ConvertPayloadToString();

            this.BeginInvoke((MethodInvoker)delegate
            {
                LogToConsole($"Received | QoS: {eventArgs.ApplicationMessage.QualityOfServiceLevel} | Topic: {topic} | Payload: {payload}");

                if (topic.EndsWith("/status", StringComparison.OrdinalIgnoreCase))
                {
                    // SmartHome/esp32c3-32s/status
                    try
                    {
                        // {"status":"offline"} or {"status":"online"}
                        var status = JsonConvert.DeserializeObject<StatusPayload>(payload);

                        if (status != null && !string.IsNullOrEmpty(status.Status))
                        {
                            this.SetRemoteClientStatus(status.Status);
                        }
                        else
                        {
                            this.SetRemoteClientStatus("unknown");
                        }
                    }
                    catch (JsonException ex)
                    {
                        LogToConsole($"Status JSON parse error on topic '{topic}': {ex.Message}");
                    }
                }
                else if (topic.EndsWith("/state", StringComparison.OrdinalIgnoreCase))
                {
                    if (this.groupBoxRemoteClientState.Enabled)
                    {
                        // SmartHome/esp32c3-32s/state
                        try
                        {
                            // {"weather":"unknown","window":{"speed":"unknown","state":"unknown"},"heater":"unknown","temp":"0.000","fan":"unknown","light":{"lux":"0.000","lumen_surf":"0.000","lumen_iso":"0.000","lumen_lamb":"0.000","lumen_angle":"0.000"},"lamp":"unknown"}
                            var state = JsonConvert.DeserializeObject<StatePayload>(payload);

                            if (state == null)
                            {
                                return;
                            }

                            float? temperatureDegrees = ParseNullableFloat(state.Temp);             // null, "0.000" - "999.999"
                            float? lux = ParseNullableFloat(state.Light?.Lux);                      // null, "0.000" - "999.999"
                            //float? lumenSurf = ParseNullableFloat(state.Light?.LumenSurface);       // null, "0.000" - "999.999"
                            //float? lumenIso = ParseNullableFloat(state.Light?.LumenIso);            // null, "0.000" - "999.999"
                            //float? lumenLamb = ParseNullableFloat(state.Light?.LumenLambertian);    // null, "0.000" - "999.999"
                            float? lumenAngle = ParseNullableFloat(state.Light?.LumenAngle);        // null, "0.000" - "999.999"

                            this.SetWeatherStateLabel(state.Weather);           // "unknown", "dry", "damp", "wet"
                            this.SetWindowSpeedStateLabel(state.Window?.Speed); // "unknown", "slow", "medium", "fast"
                            this.SetWindowStateLabel(state.Window?.State);      // "unknown", "closed", "open", "opening", "closing", "idle"

                            this.SetTemperatureDegrees(temperatureDegrees.HasValue ? temperatureDegrees.Value.ToString("0.000") : "unknown");
                            this.SetHeaterStateLabel(state.Heater);             // "unknown", "off", "on"
                            this.SetFanSpeedStateLabel(state.Fan);              // "unknown", "off", "slow", "medium", "fast"

                            this.SetLuminousFluxLumen(lumenAngle.HasValue ? lumenAngle.Value.ToString("0.000") : "unknown");
                            this.SetIlluminanceLux(lux.HasValue ? lux.Value.ToString("0.000") : "unknown");
                            this.SetLampLevelStateLabel(state.Lamp);            // "unknown", "off", "low", "medium", "high"           
                        }
                        catch (JsonException ex)
                        {
                            LogToConsole($"State JSON parse error on topic '{topic}': {ex.Message}");
                        }
                    }
                }
            });

            return Task.CompletedTask;
        }

        private Task OnMqttConnectionFailed(ConnectingFailedEventArgs eventArgs)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                LogToConsole($"Connecting failed: {eventArgs.Exception?.Message}");
                this.SetConnectionState(ConnectionState.Connecting);
                this.buttonMQTTConnection.Enabled = true;
            });

            return Task.CompletedTask;
        }

        private async void ButtonMqttConnection_Click(object sender, EventArgs e)
        {
            this.SetConnectionState(ConnectionState.Connecting);

            try
            {
                if (this.managedMqttClient == null)
                {
                    var mqttFactory = new MqttFactory();

                    //var mqttClient = mqttFactory.CreateMqttClient();
                    //
                    //var mqttTlsOptions = new MqttClientTlsOptions
                    //{
                    //    UseTls = false,
                    //    IgnoreCertificateChainErrors = true,
                    //    IgnoreCertificateRevocationErrors = true,
                    //    AllowUntrustedCertificates = true
                    //};

                    if (!int.TryParse(this.textBoxPort.Text.Trim(), out int port))
                    {
                        port = 1883;
                    }

                    var mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
                        .WithClientId(this.textBoxClientID.Text.Trim())
                        .WithTcpServer(this.textBoxHost.Text.Trim(), port)
                        .WithProtocolVersion(MqttProtocolVersion.V311)
                        //.WithTlsOptions(mqttTlsOptions)
                        .WithCleanSession()
                        .WithKeepAlivePeriod(TimeSpan.FromSeconds(5));
                    //.WithCredentials(this.textBoxUserName.Text.Trim(), Encoding.UTF8.GetBytes(this.textBoxPassword.Text.Trim()))
                    //.Build();

                    if (!string.IsNullOrWhiteSpace(textBoxUserName.Text.Trim()) && !string.IsNullOrWhiteSpace(textBoxPassword.Text.Trim()))
                    {
                        mqttClientOptionsBuilder = mqttClientOptionsBuilder
                            .WithCredentials(this.textBoxUserName.Text.Trim(), Encoding.UTF8.GetBytes(this.textBoxPassword.Text.Trim()));
                    }

                    var mqttClientOptions = mqttClientOptionsBuilder.Build();

                    if (mqttClientOptions.ChannelOptions == null)
                    {
                        throw new InvalidOperationException();
                    }

                    this.managedMqttClient = mqttFactory.CreateManagedMqttClient();

                    this.managedMqttClient.ConnectedAsync += this.OnMqttConnected;
                    this.managedMqttClient.DisconnectedAsync += this.OnMqttDisconnected;
                    this.managedMqttClient.ApplicationMessageReceivedAsync += this.OnMqttMessageReceived;
                    this.managedMqttClient.ConnectingFailedAsync += this.OnMqttConnectionFailed;

                    var managedMqttOptions = new ManagedMqttClientOptionsBuilder()
                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                        .WithClientOptions(mqttClientOptions)
                        .Build();

                    managedMqttOptions.ConnectionCheckInterval = TimeSpan.FromSeconds(5);

                    await this.managedMqttClient.StartAsync(managedMqttOptions);
                }
                else
                {
                    await this.managedMqttClient.StopAsync();

                    this.managedMqttClient.ConnectedAsync -= this.OnMqttConnected;
                    this.managedMqttClient.DisconnectedAsync -= this.OnMqttDisconnected;
                    this.managedMqttClient.ApplicationMessageReceivedAsync -= this.OnMqttMessageReceived;
                    this.managedMqttClient.ConnectingFailedAsync -= this.OnMqttConnectionFailed;

                    this.managedMqttClient.Dispose();
                    this.managedMqttClient = null;

                    this.SetSubscriptionState(false);
                    this.SetConnectionState(ConnectionState.Disconnected);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MQTT connection error: " + ex.Message, "MQTT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.buttonMQTTConnection.Enabled = true;
            }
        }

        private void TextBoxPort_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(this.textBoxPort.Text.Trim(), out _))
            {
                this.textBoxPort.Text = "1883";
                this.textBoxPort.SelectionStart = this.textBoxPort.Text.Length;
                this.textBoxPort.SelectionLength = 0;
            }
        }

        private void SetSubscriptionState(bool subscribed)
        {
            this.clientSubscribed = subscribed;

            this.textBoxRemoteClientID.ReadOnly = subscribed;

            if (subscribed)
            {
                this.buttonMQTTSubscription.Text = "SUBSCRIBED";
                this.buttonMQTTSubscription.BackColor = Color.Lime;
                //this.buttonMQTTSubscription.ForeColor = Color.Black;

                this.groupBoxRemoteClientState.Enabled = true;

                this.groupBoxWhatsapp.Enabled = true;
                this.textBoxConsole.Clear();
            }
            else
            {
                this.buttonMQTTSubscription.Text = "SUBSCRIBE";
                this.buttonMQTTSubscription.BackColor = SystemColors.ControlLight;
                //this.buttonMQTTSubscription.ForeColor = SystemColors.ControlText;

                this.SetRemoteClientStatus("unknown");

                this.SetWeatherStateLabel("unknown");
                this.SetWindowSpeedStateLabel("unknown");
                this.SetWindowStateLabel("unknown");
                this.SetTemperatureDegrees("unknown");
                this.SetHeaterStateLabel("unknown");
                this.SetFanSpeedStateLabel("unknown");
                this.SetLuminousFluxLumen("unknown");
                this.SetIlluminanceLux("unknown");
                this.SetLampLevelStateLabel("unknown");

                this.groupBoxRemoteClientState.Enabled = false;

                this.groupBoxWhatsapp.Enabled = false;
            }
        }

        private void CheckRemoteClientID()
        {
            var text = textBoxRemoteClientID.Text.Trim();

            if (text.Length == 0)
            {
                textBoxRemoteClientID.Text = "esp32c3-32s";
            }
            else
            {
                textBoxRemoteClientID.Text = text;
            }
        }

        private async void ButtonMQTTSubscription_Click(object sender, EventArgs e)
        {
            if (this.managedMqttClient == null)
            {
                return;
            }

            this.buttonMQTTSubscription.Enabled = false;

            try
            {
                this.CheckRemoteClientID();

                string clientID = this.textBoxRemoteClientID.Text.Trim();

                string topicStatus = (topicBase.Trim() + "/" + clientID + "/status").Trim();
                string topicState = (topicBase.Trim() + "/" + clientID + "/state").Trim();

                if (!clientSubscribed)
                {
                    var topicFilters = new List<MqttTopicFilter>
                    {
                        new MqttTopicFilter
                        {
                            Topic = topicStatus,
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                        },
                        new MqttTopicFilter
                        {
                            Topic = topicState,
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
                        },
                    };

                    await this.managedMqttClient.SubscribeAsync(topicFilters);

                    this.SetSubscriptionState(true);

                    //MessageBox.Show("Topic " + topicString + " is subscribed", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var topics = new List<string> { topicStatus, topicState };

                    await this.managedMqttClient.UnsubscribeAsync(topics);

                    this.SetSubscriptionState(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MQTT subscription error: " + ex.Message, "MQTT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.buttonMQTTSubscription.Enabled = true;
            }
        }

        private void TextBoxRemoteClientID_Leave(object sender, EventArgs e)
        {
            CheckRemoteClientID();
        }

        private void TextBoxRemoteClientID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.buttonMQTTSubscription.PerformClick();
            }
        }

        private bool isComboBoxWindowSpeedStateBusy = false;

        private async void ComboBoxWindowSpeedState_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // SmartHome/esp32c3-32s/window
            // {"speed":"<speed>","state":"<state>"}
            // speed: slow, medium, fast
            // state: open, close, stop

            if (isComboBoxWindowSpeedStateBusy)
            {
                return;
            }

            isComboBoxWindowSpeedStateBusy = true;

            this.comboBoxWindowSpeedState.Enabled = false;

            var selectedText = this.comboBoxWindowSpeedState.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                isComboBoxWindowSpeedStateBusy = false;
                this.comboBoxWindowSpeedState.Enabled = true;

                return;
            }

            switch (selectedText.Trim().ToLowerInvariant())
            {
                case "slow":
                    windowSpeedStateValue = "slow";
                    break;

                case "medium":
                    windowSpeedStateValue = "medium";
                    break;

                case "fast":
                    windowSpeedStateValue = "fast";
                    break;

                default:
                    return;
            }

            try
            {
                string topic = (topicBase.Trim() + "/" + textBoxRemoteClientID.Text.Trim() + "/window").Trim();

                try
                {
                    var payloadObject = new { speed = windowSpeedStateValue, state = windowStateValue };
                    var payloadString = JsonConvert.SerializeObject(payloadObject, Formatting.None);
                    var payloadBytes = Encoding.UTF8.GetBytes(payloadString);

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(payloadBytes)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                        .Build();

                    await this.managedMqttClient.EnqueueAsync(message);

                    LogToConsole($"Sent | Topic: {topic} | Payload: {payloadString}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MQTT publish error: " + topic + " " + ex.Message, "MQTT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                this.comboBoxWindowSpeedState.SelectedIndex = -1;
                this.comboBoxWindowSpeedState.Invalidate();
                this.comboBoxWindowSpeedState.Enabled = true;

                isComboBoxWindowSpeedStateBusy = false;
            }
        }

        private bool isComboBoxWindowStateBusy = false;

        private async void ComboBoxWindowState_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // SmartHome/esp32c3-32s/window
            // {"speed":"<speed>","state":"<state>"}
            // speed: slow, medium, fast
            // state: open, close, stop

            if (isComboBoxWindowStateBusy)
            {
                return;
            }

            isComboBoxWindowStateBusy = true;

            this.comboBoxWindowState.Enabled = false;

            var selectedText = this.comboBoxWindowState.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                isComboBoxWindowStateBusy = false;
                this.comboBoxWindowState.Enabled = true;

                return;
            }

            switch (selectedText.Trim().ToLowerInvariant())
            {
                case "open":
                    windowStateValue = "open";
                    break;

                case "close":
                    windowStateValue = "close";
                    break;

                case "stop":
                    windowStateValue = "stop";
                    break;

                default:
                    isComboBoxWindowStateBusy = false;
                    this.comboBoxWindowState.Enabled = true;
                    return;
            }

            try
            {
                string topic = (topicBase.Trim() + "/" + textBoxRemoteClientID.Text.Trim() + "/window").Trim();

                try
                {
                    var payloadObject = new { speed = windowSpeedStateValue, state = windowStateValue };
                    var payloadString = JsonConvert.SerializeObject(payloadObject, Formatting.None);
                    var payloadBytes = Encoding.UTF8.GetBytes(payloadString);

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(payloadBytes)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                        .Build();

                    await this.managedMqttClient.EnqueueAsync(message);

                    LogToConsole($"Sent | Topic: {topic} | Payload: {payloadString}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MQTT publish error: " + topic + " " + ex.Message, "MQTT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                this.comboBoxWindowState.SelectedIndex = -1;
                this.comboBoxWindowState.Invalidate();
                this.comboBoxWindowState.Enabled = true;

                isComboBoxWindowStateBusy = false;
            }
        }

        private bool isComboBoxHeaterStateBusy = false;

        private async void ComboBoxHeaterState_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // SmartHome/esp32c3-32s/heater
            // {"state":"<state>"}
            // state: off, on

            if (isComboBoxHeaterStateBusy)
            {
                return;
            }

            isComboBoxHeaterStateBusy = true;

            this.comboBoxHeaterState.Enabled = false;

            var selectedText = this.comboBoxHeaterState.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                isComboBoxHeaterStateBusy = false;
                this.comboBoxHeaterState.Enabled = true;

                return;
            }

            string state;
            switch (selectedText.Trim().ToLowerInvariant())
            {
                case "on":
                    state = "on";
                    break;

                case "off":
                    state = "off";
                    break;

                default:
                    isComboBoxHeaterStateBusy = false;
                    this.comboBoxHeaterState.Enabled = true;
                    return;
            }

            string topic = (topicBase.Trim() + "/" + textBoxRemoteClientID.Text.Trim() + "/heater").Trim();

            try
            {
                var payloadObject = new { state };
                var payloadString = JsonConvert.SerializeObject(payloadObject, Formatting.None);
                var payloadBytes = Encoding.UTF8.GetBytes(payloadString);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payloadBytes)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                    .Build();

                await this.managedMqttClient.EnqueueAsync(message);

                LogToConsole($"Sent | Topic: {topic} | Payload: {payloadString}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("MQTT publish error: " + topic + " " + ex.Message, "MQTT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.comboBoxHeaterState.SelectedIndex = -1;
                this.comboBoxHeaterState.Invalidate();
                this.comboBoxHeaterState.Enabled = true;

                isComboBoxHeaterStateBusy = false;
            }
        }

        private bool isComboBoxFanSpeedStateBusy = false;

        private async void ComboBoxFanSpeedState_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // SmartHome/esp32c3-32s/fan
            // {"speed":"<speed>"}
            // speed: off, slow, medium, fast

            if (isComboBoxFanSpeedStateBusy)
            {
                return;
            }

            isComboBoxFanSpeedStateBusy = true;

            this.comboBoxFanSpeedState.Enabled = false;

            var selectedText = this.comboBoxFanSpeedState.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                isComboBoxFanSpeedStateBusy = false;
                this.comboBoxFanSpeedState.Enabled = true;

                return;
            }

            string speed;
            switch (selectedText.Trim().ToLowerInvariant())
            {
                case "off":
                    speed = "off";
                    break;

                case "slow":
                    speed = "slow";
                    break;

                case "medium":
                    speed = "medium";
                    break;

                case "fast":
                    speed = "fast";
                    break;

                default:
                    isComboBoxFanSpeedStateBusy = false;
                    this.comboBoxFanSpeedState.Enabled = true;
                    return;
            }

            try
            {
                string topic = (topicBase.Trim() + "/" + textBoxRemoteClientID.Text.Trim() + "/fan").Trim();

                try
                {
                    var payloadObject = new { speed };
                    var payloadString = JsonConvert.SerializeObject(payloadObject, Formatting.None);
                    var payloadBytes = Encoding.UTF8.GetBytes(payloadString);

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(payloadBytes)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                        .Build();

                    await this.managedMqttClient.EnqueueAsync(message);

                    LogToConsole($"Sent | Topic: {topic} | Payload: {payloadString}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MQTT publish error: " + topic + " " + ex.Message, "MQTT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                this.comboBoxFanSpeedState.SelectedIndex = -1;
                this.comboBoxFanSpeedState.Invalidate();
                this.comboBoxFanSpeedState.Enabled = true;

                isComboBoxFanSpeedStateBusy = false;
            }
        }

        private bool isComboBoxLampLevelStateBusy = false;

        private async void ComboBoxLampLevelState_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // SmartHome/esp32c3-32s/lamp
            // {"level":"<level>"}
            // level: off, low, medium, high

            if (isComboBoxLampLevelStateBusy)
            {
                return;
            }

            isComboBoxLampLevelStateBusy = true;

            this.comboBoxLampLevelState.Enabled = false;

            var selectedText = this.comboBoxLampLevelState.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                isComboBoxLampLevelStateBusy = false;
                this.comboBoxLampLevelState.Enabled = true;

                return;
            }

            string level;
            switch (selectedText.Trim().ToLowerInvariant())
            {
                case "off":
                    level = "off";
                    break;

                case "low":
                    level = "low";
                    break;

                case "medium":
                    level = "medium";
                    break;

                case "high":
                    level = "high";
                    break;

                default:
                    isComboBoxLampLevelStateBusy = false;
                    this.comboBoxLampLevelState.Enabled = true;
                    return;
            }

            try
            {
                string topic = (topicBase.Trim() + "/" + textBoxRemoteClientID.Text.Trim() + "/lamp").Trim();

                try
                {
                    var payloadObject = new { level };
                    var payloadString = JsonConvert.SerializeObject(payloadObject, Formatting.None);
                    var payloadBytes = Encoding.UTF8.GetBytes(payloadString);

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(payloadBytes)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                        .Build();

                    await this.managedMqttClient.EnqueueAsync(message);

                    LogToConsole($"Sent | Topic: {topic} | Payload: {payloadString}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MQTT publish error: " + topic + " " + ex.Message, "MQTT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                this.comboBoxLampLevelState.SelectedIndex = -1;
                this.comboBoxLampLevelState.Invalidate();
                this.comboBoxLampLevelState.Enabled = true;

                isComboBoxLampLevelStateBusy = false;
            }
        }

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var cb = (ComboBox)sender;
            e.DrawBackground();

            string text;
            Color foreColor;

            if (e.Index < 0)
            {
                text = "SELECT";
            }
            else
            {
                text = cb.GetItemText(cb.Items[e.Index]);
            }

            if (groupBoxRemoteClientState.Enabled)
            {
                foreColor = e.ForeColor;
            }
            else
            {
                foreColor = SystemColors.GrayText;
            }

            TextRenderer.DrawText(
                e.Graphics,
                text,
                e.Font,
                e.Bounds,
                foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
            );

            e.DrawFocusRectangle();
        }

        private bool TryNormalizeTurkishWhatsappNumber(string input, out string normalizedDigits, out string errorMessage)
        {
            normalizedDigits = null;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Please enter a phone number.";
                return false;
            }

            string digits = Regex.Replace(input.Trim(), @"\D", "");

            if (digits.StartsWith("0090"))
            {
                digits = digits.Substring(2); // remove leading "00"
            }

            string nsn10;
            if (digits.StartsWith("90") && digits.Length == 12)
            {
                nsn10 = digits.Substring(2);
            }
            else if (digits.StartsWith("0") && digits.Length == 11)
            {
                nsn10 = digits.Substring(1);
            }
            else if (digits.Length == 10)
            {
                nsn10 = digits;
            }
            else
            {
                errorMessage = "Invalid Turkish phone number.\nExamples: +90 5xx xxx xx xx, 05xx xxx xx xx, or 5xx xxx xx xx.";
                return false;
            }

            if (nsn10.Length != 10 || nsn10[0] != '5')
            {
                errorMessage = "Invalid Turkish mobile number.\nAfter removing 0 / +90, it must be 10 digits and start with 5.";
                return false;
            }

            normalizedDigits = "90" + nsn10;
            return true;
        }

        private bool ValidateAndFixWhatsappNumberInTextbox(out string normalizedDigits)
        {
            normalizedDigits = null;

            if (!TryNormalizeTurkishWhatsappNumber(this.textBoxWhatsappNumber.Text, out normalizedDigits, out var err))
            {
                MessageBox.Show(err, "Invalid WhatsApp Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBoxWhatsappNumber.Focus();
                this.textBoxWhatsappNumber.SelectAll();
                return false;
            }

            this.textBoxWhatsappNumber.Text = normalizedDigits;
            this.textBoxWhatsappNumber.SelectionStart = this.textBoxWhatsappNumber.Text.Length;
            this.textBoxWhatsappNumber.SelectionLength = 0;

            return true;
        }

        private async void ButtonWhatsappSend_Click(object sender, EventArgs e)
        {
            if (!ValidateAndFixWhatsappNumberInTextbox(out string whatsappNumber))
            {
                return;
            }

            string remoteClientID = this.textBoxRemoteClientID.Text.Trim();
            string remoteClientStatus = this.labelRemoteClientStatus.Text.Trim();
            string weatherState = this.labelWeatherState.Text.Trim();
            string windowSpeedState = this.labelWindowSpeedState.Text.Trim();
            string windowState = this.labelWindowState.Text.Trim();
            string heaterState = this.labelHeaterState.Text.Trim();
            string temperatureDegrees = this.labelTemperatureDegrees.Text.Trim();
            string fanSpeedState = this.labelFanSpeedState.Text.Trim();
            string lampLevelState = this.labelLampLevelState.Text.Trim();
            string lightLux = this.labelIlluminanceLux.Text.Trim();
            string lightLumen = this.labelLuminousFluxLumen.Text.Trim();

            string text =
                $"SmartHome:{Environment.NewLine}" +
                $"Remote Client ID: {remoteClientID}{Environment.NewLine}" +
                $"Remote Client Status: {remoteClientStatus}{Environment.NewLine}" +
                $"Weather: {weatherState}{Environment.NewLine}" +
                $"Window: {windowState} ({windowSpeedState}){Environment.NewLine}" +
                $"Heater: {heaterState}{Environment.NewLine}" +
                $"Temperature: {temperatureDegrees}{Environment.NewLine}" +
                $"Fan: {fanSpeedState}{Environment.NewLine}" +
                $"Lamp: {lampLevelState}{Environment.NewLine}" +
                $"Light: {lightLux}, {lightLumen}";

            string encodedText = Uri.EscapeDataString(text);

            //string whatsappNumber = textBoxWhatsappNumber.Text.Trim();

            string link = $"whatsapp://send?phone={whatsappNumber}&text={encodedText}";

            try
            {
                Process.Start(link);
                await Task.Delay(1500);
                SendKeys.SendWait("{ENTER}");
                await Task.Delay(500);
                SendKeys.SendWait("%{F4}");
            }
            catch (Exception)
            {
                string fallback = $"https://wa.me/{whatsappNumber}?text={encodedText}";
                Process.Start(fallback);
            }
        }

        private void TextBoxWhatsappNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.buttonWhatsappSend.PerformClick();
            }
        }

        private void ButtonSaveLog_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Console Log";
                sfd.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*";
                sfd.DefaultExt = "txt";
                sfd.AddExtension = true;
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.FileName = $"SmartHome_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (sfd.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    var content = this.textBoxConsole.Text ?? string.Empty;

                    File.WriteAllText(sfd.FileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

                    MessageBox.Show("Log saved:\n" + sfd.FileName, "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save log:\n" + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ButtonClearConsole_Click(object sender, EventArgs e)
        {
            this.textBoxConsole.Clear();
        }

        //private void FormSmartHome_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Alt && e.KeyCode == Keys.F4)
        //    {
        //        altF4Pressed = true;
        //        e.Handled = true;
        //        e.SuppressKeyPress = true;
        //    }
        //}

        //private void TimerElapsed(object sender, ElapsedEventArgs e)
        //{
        //    this.BeginInvoke(
        //        (MethodInvoker)delegate
        //        {

        //        });
        //}

        private async void FormSmartHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
            {
                return;
            }

            //if (altF4Pressed && e.CloseReason == CloseReason.UserClosing)
            //{
            //    e.Cancel = true;
            //    altF4Pressed = false;
            //
            //    return;
            //}

            var result =
                MessageBox.Show("Do you really want to exit " + this.Text.Trim() + "?", "Confirm Exit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            if (this.managedMqttClient != null)
            {
                try
                {
                    await this.managedMqttClient.StopAsync();
                }
                catch (Exception ex)
                {
                    this.BeginInvoke((MethodInvoker)(() => LogToConsole("Error while stopping MQTT client: " + ex.Message)));
                }

                this.managedMqttClient.ConnectedAsync -= this.OnMqttConnected;
                this.managedMqttClient.DisconnectedAsync -= this.OnMqttDisconnected;
                this.managedMqttClient.ApplicationMessageReceivedAsync -= this.OnMqttMessageReceived;
                this.managedMqttClient.ConnectingFailedAsync -= this.OnMqttConnectionFailed;

                this.managedMqttClient.Dispose();
                this.managedMqttClient = null;
            }

            //this.timer.Stop();
            //this.timer.Dispose();
        }
    }
}
