namespace SmartHome
{
    partial class FormSmartHome
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSmartHome));
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.buttonMQTTConnection = new System.Windows.Forms.Button();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.textBoxClientID = new System.Windows.Forms.TextBox();
            this.labelClientID = new System.Windows.Forms.Label();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.labelHost = new System.Windows.Forms.Label();
            this.textBoxRemoteClientID = new System.Windows.Forms.TextBox();
            this.labelRemoteClientID = new System.Windows.Forms.Label();
            this.labelRemoteClientStatus = new System.Windows.Forms.Label();
            this.labelRemoteClientStatusLabel = new System.Windows.Forms.Label();
            this.groupBoxConsole = new System.Windows.Forms.GroupBox();
            this.textBoxConsole = new System.Windows.Forms.TextBox();
            this.groupBoxRemoteClient = new System.Windows.Forms.GroupBox();
            this.buttonMQTTSubscription = new System.Windows.Forms.Button();
            this.groupBoxRemoteClientState = new System.Windows.Forms.GroupBox();
            this.groupBoxWeatherAndWindowControl = new System.Windows.Forms.GroupBox();
            this.comboBoxWindowState = new System.Windows.Forms.ComboBox();
            this.labelWindowState = new System.Windows.Forms.Label();
            this.labelWeatherState = new System.Windows.Forms.Label();
            this.labelWeatherLabel = new System.Windows.Forms.Label();
            this.labelWindowSpeedLabel = new System.Windows.Forms.Label();
            this.labelWindowSpeedState = new System.Windows.Forms.Label();
            this.comboBoxWindowSpeedState = new System.Windows.Forms.ComboBox();
            this.labelWindowStateLabel = new System.Windows.Forms.Label();
            this.groupBoxHeatingAndFanControl = new System.Windows.Forms.GroupBox();
            this.labelFanSpeedState = new System.Windows.Forms.Label();
            this.comboBoxFanSpeedState = new System.Windows.Forms.ComboBox();
            this.labelHeaterState = new System.Windows.Forms.Label();
            this.labelTemperatureDegrees = new System.Windows.Forms.Label();
            this.labelTemperatureLabel = new System.Windows.Forms.Label();
            this.labelFanSpeedLabel = new System.Windows.Forms.Label();
            this.labelHeaterStateLabel = new System.Windows.Forms.Label();
            this.comboBoxHeaterState = new System.Windows.Forms.ComboBox();
            this.groupBoxLightingControl = new System.Windows.Forms.GroupBox();
            this.comboBoxLampLevelState = new System.Windows.Forms.ComboBox();
            this.labelLampLevelState = new System.Windows.Forms.Label();
            this.labelLuminousFluxLumen = new System.Windows.Forms.Label();
            this.labelLuminousFluxLabel = new System.Windows.Forms.Label();
            this.labelIlluminanceLux = new System.Windows.Forms.Label();
            this.labelLampLevelLabel = new System.Windows.Forms.Label();
            this.labelIlluminanceLabel = new System.Windows.Forms.Label();
            this.groupBoxWhatsapp = new System.Windows.Forms.GroupBox();
            this.buttonWhatsappSend = new System.Windows.Forms.Button();
            this.labelWhatsappNumber = new System.Windows.Forms.Label();
            this.textBoxWhatsappNumber = new System.Windows.Forms.TextBox();
            this.groupBoxClearConsole = new System.Windows.Forms.GroupBox();
            this.buttonSaveLog = new System.Windows.Forms.Button();
            this.buttonClearConsole = new System.Windows.Forms.Button();
            this.groupBoxConnection.SuspendLayout();
            this.groupBoxConsole.SuspendLayout();
            this.groupBoxRemoteClient.SuspendLayout();
            this.groupBoxRemoteClientState.SuspendLayout();
            this.groupBoxWeatherAndWindowControl.SuspendLayout();
            this.groupBoxHeatingAndFanControl.SuspendLayout();
            this.groupBoxLightingControl.SuspendLayout();
            this.groupBoxWhatsapp.SuspendLayout();
            this.groupBoxClearConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConnection.Controls.Add(this.textBoxUserName);
            this.groupBoxConnection.Controls.Add(this.labelUserName);
            this.groupBoxConnection.Controls.Add(this.textBoxPassword);
            this.groupBoxConnection.Controls.Add(this.labelPassword);
            this.groupBoxConnection.Controls.Add(this.buttonMQTTConnection);
            this.groupBoxConnection.Controls.Add(this.textBoxPort);
            this.groupBoxConnection.Controls.Add(this.labelPort);
            this.groupBoxConnection.Controls.Add(this.textBoxClientID);
            this.groupBoxConnection.Controls.Add(this.labelClientID);
            this.groupBoxConnection.Controls.Add(this.textBoxHost);
            this.groupBoxConnection.Controls.Add(this.labelHost);
            this.groupBoxConnection.Location = new System.Drawing.Point(12, 12);
            this.groupBoxConnection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxConnection.Size = new System.Drawing.Size(1234, 107);
            this.groupBoxConnection.TabIndex = 1;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "Connection";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(111, 63);
            this.textBoxUserName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(310, 26);
            this.textBoxUserName.TabIndex = 4;
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(12, 66);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(93, 20);
            this.labelUserName.TabIndex = 4;
            this.labelUserName.Text = "User Name:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(534, 63);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(310, 26);
            this.textBoxPassword.TabIndex = 5;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(446, 66);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(82, 20);
            this.labelPassword.TabIndex = 5;
            this.labelPassword.Text = "Password:";
            // 
            // buttonMQTTConnection
            // 
            this.buttonMQTTConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMQTTConnection.Location = new System.Drawing.Point(872, 61);
            this.buttonMQTTConnection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonMQTTConnection.Name = "buttonMQTTConnection";
            this.buttonMQTTConnection.Size = new System.Drawing.Size(344, 30);
            this.buttonMQTTConnection.TabIndex = 6;
            this.buttonMQTTConnection.Text = "CONNECT";
            this.buttonMQTTConnection.UseVisualStyleBackColor = true;
            this.buttonMQTTConnection.Click += new System.EventHandler(this.ButtonMqttConnection_Click);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(916, 27);
            this.textBoxPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(300, 26);
            this.textBoxPort.TabIndex = 3;
            this.textBoxPort.Text = "1883";
            this.textBoxPort.Leave += new System.EventHandler(this.TextBoxPort_Leave);
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(868, 30);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(42, 20);
            this.labelPort.TabIndex = 3;
            this.labelPort.Text = "Port:";
            // 
            // textBoxClientID
            // 
            this.textBoxClientID.Location = new System.Drawing.Point(111, 27);
            this.textBoxClientID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxClientID.Name = "textBoxClientID";
            this.textBoxClientID.Size = new System.Drawing.Size(310, 26);
            this.textBoxClientID.TabIndex = 1;
            this.textBoxClientID.Text = "SmartHome";
            // 
            // labelClientID
            // 
            this.labelClientID.AutoSize = true;
            this.labelClientID.Location = new System.Drawing.Point(31, 30);
            this.labelClientID.Name = "labelClientID";
            this.labelClientID.Size = new System.Drawing.Size(74, 20);
            this.labelClientID.TabIndex = 1;
            this.labelClientID.Text = "Client ID:";
            // 
            // textBoxHost
            // 
            this.textBoxHost.Location = new System.Drawing.Point(534, 27);
            this.textBoxHost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(310, 26);
            this.textBoxHost.TabIndex = 2;
            this.textBoxHost.Text = "localhost";
            // 
            // labelHost
            // 
            this.labelHost.AutoSize = true;
            this.labelHost.Location = new System.Drawing.Point(481, 30);
            this.labelHost.Name = "labelHost";
            this.labelHost.Size = new System.Drawing.Size(47, 20);
            this.labelHost.TabIndex = 2;
            this.labelHost.Text = "Host:";
            // 
            // textBoxRemoteClientID
            // 
            this.textBoxRemoteClientID.Location = new System.Drawing.Point(153, 28);
            this.textBoxRemoteClientID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxRemoteClientID.Name = "textBoxRemoteClientID";
            this.textBoxRemoteClientID.Size = new System.Drawing.Size(300, 26);
            this.textBoxRemoteClientID.TabIndex = 7;
            this.textBoxRemoteClientID.Text = "esp32c3-32s";
            this.textBoxRemoteClientID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxRemoteClientID_KeyDown);
            this.textBoxRemoteClientID.Leave += new System.EventHandler(this.TextBoxRemoteClientID_Leave);
            // 
            // labelRemoteClientID
            // 
            this.labelRemoteClientID.AutoSize = true;
            this.labelRemoteClientID.Location = new System.Drawing.Point(12, 31);
            this.labelRemoteClientID.Name = "labelRemoteClientID";
            this.labelRemoteClientID.Size = new System.Drawing.Size(135, 20);
            this.labelRemoteClientID.TabIndex = 6;
            this.labelRemoteClientID.Text = "Remote Client ID:";
            // 
            // labelRemoteClientStatus
            // 
            this.labelRemoteClientStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelRemoteClientStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelRemoteClientStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRemoteClientStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelRemoteClientStatus.Location = new System.Drawing.Point(965, 28);
            this.labelRemoteClientStatus.Name = "labelRemoteClientStatus";
            this.labelRemoteClientStatus.Size = new System.Drawing.Size(251, 26);
            this.labelRemoteClientStatus.TabIndex = 8;
            this.labelRemoteClientStatus.Text = "UNKNOWN";
            this.labelRemoteClientStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelRemoteClientStatusLabel
            // 
            this.labelRemoteClientStatusLabel.AutoSize = true;
            this.labelRemoteClientStatusLabel.Location = new System.Drawing.Point(794, 31);
            this.labelRemoteClientStatusLabel.Name = "labelRemoteClientStatusLabel";
            this.labelRemoteClientStatusLabel.Size = new System.Drawing.Size(165, 20);
            this.labelRemoteClientStatusLabel.TabIndex = 7;
            this.labelRemoteClientStatusLabel.Text = "Remote Client Status:";
            // 
            // groupBoxConsole
            // 
            this.groupBoxConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConsole.Controls.Add(this.textBoxConsole);
            this.groupBoxConsole.Location = new System.Drawing.Point(12, 568);
            this.groupBoxConsole.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxConsole.Name = "groupBoxConsole";
            this.groupBoxConsole.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxConsole.Size = new System.Drawing.Size(1234, 164);
            this.groupBoxConsole.TabIndex = 9;
            this.groupBoxConsole.TabStop = false;
            this.groupBoxConsole.Text = "Console";
            // 
            // textBoxConsole
            // 
            this.textBoxConsole.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxConsole.Location = new System.Drawing.Point(6, 26);
            this.textBoxConsole.Multiline = true;
            this.textBoxConsole.Name = "textBoxConsole";
            this.textBoxConsole.ReadOnly = true;
            this.textBoxConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxConsole.Size = new System.Drawing.Size(1222, 131);
            this.textBoxConsole.TabIndex = 0;
            this.textBoxConsole.TabStop = false;
            // 
            // groupBoxRemoteClient
            // 
            this.groupBoxRemoteClient.Controls.Add(this.buttonMQTTSubscription);
            this.groupBoxRemoteClient.Controls.Add(this.labelRemoteClientID);
            this.groupBoxRemoteClient.Controls.Add(this.textBoxRemoteClientID);
            this.groupBoxRemoteClient.Controls.Add(this.labelRemoteClientStatus);
            this.groupBoxRemoteClient.Controls.Add(this.labelRemoteClientStatusLabel);
            this.groupBoxRemoteClient.Enabled = false;
            this.groupBoxRemoteClient.Location = new System.Drawing.Point(12, 126);
            this.groupBoxRemoteClient.Name = "groupBoxRemoteClient";
            this.groupBoxRemoteClient.Size = new System.Drawing.Size(1234, 76);
            this.groupBoxRemoteClient.TabIndex = 2;
            this.groupBoxRemoteClient.TabStop = false;
            this.groupBoxRemoteClient.Text = "Remote Client";
            // 
            // buttonMQTTSubscription
            // 
            this.buttonMQTTSubscription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMQTTSubscription.Location = new System.Drawing.Point(481, 26);
            this.buttonMQTTSubscription.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonMQTTSubscription.Name = "buttonMQTTSubscription";
            this.buttonMQTTSubscription.Size = new System.Drawing.Size(292, 30);
            this.buttonMQTTSubscription.TabIndex = 8;
            this.buttonMQTTSubscription.Text = "SUBSCRIBE";
            this.buttonMQTTSubscription.UseVisualStyleBackColor = true;
            this.buttonMQTTSubscription.Click += new System.EventHandler(this.ButtonMQTTSubscription_Click);
            // 
            // groupBoxRemoteClientState
            // 
            this.groupBoxRemoteClientState.Controls.Add(this.groupBoxWeatherAndWindowControl);
            this.groupBoxRemoteClientState.Controls.Add(this.groupBoxHeatingAndFanControl);
            this.groupBoxRemoteClientState.Controls.Add(this.groupBoxLightingControl);
            this.groupBoxRemoteClientState.Enabled = false;
            this.groupBoxRemoteClientState.Location = new System.Drawing.Point(12, 208);
            this.groupBoxRemoteClientState.Name = "groupBoxRemoteClientState";
            this.groupBoxRemoteClientState.Size = new System.Drawing.Size(1234, 272);
            this.groupBoxRemoteClientState.TabIndex = 3;
            this.groupBoxRemoteClientState.TabStop = false;
            this.groupBoxRemoteClientState.Text = "Remote Client State";
            // 
            // groupBoxWeatherAndWindowControl
            // 
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.comboBoxWindowState);
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.labelWindowState);
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.labelWeatherState);
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.labelWeatherLabel);
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.labelWindowSpeedLabel);
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.labelWindowSpeedState);
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.comboBoxWindowSpeedState);
            this.groupBoxWeatherAndWindowControl.Controls.Add(this.labelWindowStateLabel);
            this.groupBoxWeatherAndWindowControl.Location = new System.Drawing.Point(6, 25);
            this.groupBoxWeatherAndWindowControl.Name = "groupBoxWeatherAndWindowControl";
            this.groupBoxWeatherAndWindowControl.Size = new System.Drawing.Size(1222, 76);
            this.groupBoxWeatherAndWindowControl.TabIndex = 4;
            this.groupBoxWeatherAndWindowControl.TabStop = false;
            this.groupBoxWeatherAndWindowControl.Text = "Weather and Window Control";
            // 
            // comboBoxWindowState
            // 
            this.comboBoxWindowState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxWindowState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWindowState.FormattingEnabled = true;
            this.comboBoxWindowState.Items.AddRange(new object[] {
            "OPEN",
            "CLOSE",
            "STOP"});
            this.comboBoxWindowState.Location = new System.Drawing.Point(1095, 29);
            this.comboBoxWindowState.Name = "comboBoxWindowState";
            this.comboBoxWindowState.Size = new System.Drawing.Size(121, 27);
            this.comboBoxWindowState.TabIndex = 10;
            this.comboBoxWindowState.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBox_DrawItem);
            this.comboBoxWindowState.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxWindowState_SelectionChangeCommitted);
            // 
            // labelWindowState
            // 
            this.labelWindowState.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelWindowState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelWindowState.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWindowState.Location = new System.Drawing.Point(949, 29);
            this.labelWindowState.Name = "labelWindowState";
            this.labelWindowState.Size = new System.Drawing.Size(140, 27);
            this.labelWindowState.TabIndex = 14;
            this.labelWindowState.Text = "UNKNOWN";
            this.labelWindowState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWeatherState
            // 
            this.labelWeatherState.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelWeatherState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelWeatherState.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWeatherState.Location = new System.Drawing.Point(172, 29);
            this.labelWeatherState.Name = "labelWeatherState";
            this.labelWeatherState.Size = new System.Drawing.Size(140, 27);
            this.labelWeatherState.TabIndex = 10;
            this.labelWeatherState.Text = "UNKNOWN";
            this.labelWeatherState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWeatherLabel
            // 
            this.labelWeatherLabel.AutoSize = true;
            this.labelWeatherLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWeatherLabel.Location = new System.Drawing.Point(66, 29);
            this.labelWeatherLabel.Name = "labelWeatherLabel";
            this.labelWeatherLabel.Size = new System.Drawing.Size(100, 26);
            this.labelWeatherLabel.TabIndex = 9;
            this.labelWeatherLabel.Text = "Weather:";
            // 
            // labelWindowSpeedLabel
            // 
            this.labelWindowSpeedLabel.AutoSize = true;
            this.labelWindowSpeedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWindowSpeedLabel.Location = new System.Drawing.Point(331, 29);
            this.labelWindowSpeedLabel.Name = "labelWindowSpeedLabel";
            this.labelWindowSpeedLabel.Size = new System.Drawing.Size(165, 26);
            this.labelWindowSpeedLabel.TabIndex = 11;
            this.labelWindowSpeedLabel.Text = "Window Speed:";
            // 
            // labelWindowSpeedState
            // 
            this.labelWindowSpeedState.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelWindowSpeedState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelWindowSpeedState.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWindowSpeedState.Location = new System.Drawing.Point(502, 29);
            this.labelWindowSpeedState.Name = "labelWindowSpeedState";
            this.labelWindowSpeedState.Size = new System.Drawing.Size(140, 27);
            this.labelWindowSpeedState.TabIndex = 12;
            this.labelWindowSpeedState.Text = "UNKNOWN";
            this.labelWindowSpeedState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxWindowSpeedState
            // 
            this.comboBoxWindowSpeedState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxWindowSpeedState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWindowSpeedState.FormattingEnabled = true;
            this.comboBoxWindowSpeedState.Items.AddRange(new object[] {
            "SLOW",
            "MEDIUM",
            "FAST"});
            this.comboBoxWindowSpeedState.Location = new System.Drawing.Point(648, 29);
            this.comboBoxWindowSpeedState.Name = "comboBoxWindowSpeedState";
            this.comboBoxWindowSpeedState.Size = new System.Drawing.Size(121, 27);
            this.comboBoxWindowSpeedState.TabIndex = 9;
            this.comboBoxWindowSpeedState.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBox_DrawItem);
            this.comboBoxWindowSpeedState.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxWindowSpeedState_SelectionChangeCommitted);
            // 
            // labelWindowStateLabel
            // 
            this.labelWindowStateLabel.AutoSize = true;
            this.labelWindowStateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWindowStateLabel.Location = new System.Drawing.Point(790, 29);
            this.labelWindowStateLabel.Name = "labelWindowStateLabel";
            this.labelWindowStateLabel.Size = new System.Drawing.Size(153, 26);
            this.labelWindowStateLabel.TabIndex = 13;
            this.labelWindowStateLabel.Text = "Window State:";
            // 
            // groupBoxHeatingAndFanControl
            // 
            this.groupBoxHeatingAndFanControl.Controls.Add(this.labelFanSpeedState);
            this.groupBoxHeatingAndFanControl.Controls.Add(this.comboBoxFanSpeedState);
            this.groupBoxHeatingAndFanControl.Controls.Add(this.labelHeaterState);
            this.groupBoxHeatingAndFanControl.Controls.Add(this.labelTemperatureDegrees);
            this.groupBoxHeatingAndFanControl.Controls.Add(this.labelTemperatureLabel);
            this.groupBoxHeatingAndFanControl.Controls.Add(this.labelFanSpeedLabel);
            this.groupBoxHeatingAndFanControl.Controls.Add(this.labelHeaterStateLabel);
            this.groupBoxHeatingAndFanControl.Controls.Add(this.comboBoxHeaterState);
            this.groupBoxHeatingAndFanControl.Location = new System.Drawing.Point(6, 107);
            this.groupBoxHeatingAndFanControl.Name = "groupBoxHeatingAndFanControl";
            this.groupBoxHeatingAndFanControl.Size = new System.Drawing.Size(1222, 76);
            this.groupBoxHeatingAndFanControl.TabIndex = 5;
            this.groupBoxHeatingAndFanControl.TabStop = false;
            this.groupBoxHeatingAndFanControl.Text = "Heating and Fan Control";
            // 
            // labelFanSpeedState
            // 
            this.labelFanSpeedState.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelFanSpeedState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFanSpeedState.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFanSpeedState.Location = new System.Drawing.Point(949, 29);
            this.labelFanSpeedState.Name = "labelFanSpeedState";
            this.labelFanSpeedState.Size = new System.Drawing.Size(140, 27);
            this.labelFanSpeedState.TabIndex = 20;
            this.labelFanSpeedState.Text = "UNKNOWN";
            this.labelFanSpeedState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxFanSpeedState
            // 
            this.comboBoxFanSpeedState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxFanSpeedState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFanSpeedState.FormattingEnabled = true;
            this.comboBoxFanSpeedState.Items.AddRange(new object[] {
            "OFF",
            "SLOW",
            "MEDIUM",
            "FAST"});
            this.comboBoxFanSpeedState.Location = new System.Drawing.Point(1095, 29);
            this.comboBoxFanSpeedState.Name = "comboBoxFanSpeedState";
            this.comboBoxFanSpeedState.Size = new System.Drawing.Size(121, 27);
            this.comboBoxFanSpeedState.TabIndex = 12;
            this.comboBoxFanSpeedState.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBox_DrawItem);
            this.comboBoxFanSpeedState.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxFanSpeedState_SelectionChangeCommitted);
            // 
            // labelHeaterState
            // 
            this.labelHeaterState.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelHeaterState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelHeaterState.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeaterState.Location = new System.Drawing.Point(502, 29);
            this.labelHeaterState.Name = "labelHeaterState";
            this.labelHeaterState.Size = new System.Drawing.Size(140, 27);
            this.labelHeaterState.TabIndex = 18;
            this.labelHeaterState.Text = "UNKNOWN";
            this.labelHeaterState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTemperatureDegrees
            // 
            this.labelTemperatureDegrees.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelTemperatureDegrees.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTemperatureDegrees.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureDegrees.Location = new System.Drawing.Point(172, 29);
            this.labelTemperatureDegrees.Name = "labelTemperatureDegrees";
            this.labelTemperatureDegrees.Size = new System.Drawing.Size(140, 27);
            this.labelTemperatureDegrees.TabIndex = 16;
            this.labelTemperatureDegrees.Text = "UNKNOWN";
            this.labelTemperatureDegrees.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTemperatureLabel
            // 
            this.labelTemperatureLabel.AutoSize = true;
            this.labelTemperatureLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperatureLabel.Location = new System.Drawing.Point(25, 29);
            this.labelTemperatureLabel.Name = "labelTemperatureLabel";
            this.labelTemperatureLabel.Size = new System.Drawing.Size(141, 26);
            this.labelTemperatureLabel.TabIndex = 15;
            this.labelTemperatureLabel.Text = "Temperature:";
            // 
            // labelFanSpeedLabel
            // 
            this.labelFanSpeedLabel.AutoSize = true;
            this.labelFanSpeedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFanSpeedLabel.Location = new System.Drawing.Point(819, 29);
            this.labelFanSpeedLabel.Name = "labelFanSpeedLabel";
            this.labelFanSpeedLabel.Size = new System.Drawing.Size(124, 26);
            this.labelFanSpeedLabel.TabIndex = 19;
            this.labelFanSpeedLabel.Text = "Fan Speed:";
            // 
            // labelHeaterStateLabel
            // 
            this.labelHeaterStateLabel.AutoSize = true;
            this.labelHeaterStateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeaterStateLabel.Location = new System.Drawing.Point(356, 29);
            this.labelHeaterStateLabel.Name = "labelHeaterStateLabel";
            this.labelHeaterStateLabel.Size = new System.Drawing.Size(140, 26);
            this.labelHeaterStateLabel.TabIndex = 17;
            this.labelHeaterStateLabel.Text = "Heater State:";
            // 
            // comboBoxHeaterState
            // 
            this.comboBoxHeaterState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxHeaterState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHeaterState.FormattingEnabled = true;
            this.comboBoxHeaterState.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.comboBoxHeaterState.Location = new System.Drawing.Point(648, 29);
            this.comboBoxHeaterState.Name = "comboBoxHeaterState";
            this.comboBoxHeaterState.Size = new System.Drawing.Size(121, 27);
            this.comboBoxHeaterState.TabIndex = 11;
            this.comboBoxHeaterState.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBox_DrawItem);
            this.comboBoxHeaterState.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxHeaterState_SelectionChangeCommitted);
            // 
            // groupBoxLightingControl
            // 
            this.groupBoxLightingControl.Controls.Add(this.comboBoxLampLevelState);
            this.groupBoxLightingControl.Controls.Add(this.labelLampLevelState);
            this.groupBoxLightingControl.Controls.Add(this.labelLuminousFluxLumen);
            this.groupBoxLightingControl.Controls.Add(this.labelLuminousFluxLabel);
            this.groupBoxLightingControl.Controls.Add(this.labelIlluminanceLux);
            this.groupBoxLightingControl.Controls.Add(this.labelLampLevelLabel);
            this.groupBoxLightingControl.Controls.Add(this.labelIlluminanceLabel);
            this.groupBoxLightingControl.Location = new System.Drawing.Point(6, 189);
            this.groupBoxLightingControl.Name = "groupBoxLightingControl";
            this.groupBoxLightingControl.Size = new System.Drawing.Size(1222, 76);
            this.groupBoxLightingControl.TabIndex = 6;
            this.groupBoxLightingControl.TabStop = false;
            this.groupBoxLightingControl.Text = "Brightness and Lighting Control";
            // 
            // comboBoxLampLevelState
            // 
            this.comboBoxLampLevelState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxLampLevelState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLampLevelState.FormattingEnabled = true;
            this.comboBoxLampLevelState.Items.AddRange(new object[] {
            "OFF",
            "LOW",
            "MEDIUM",
            "HIGH"});
            this.comboBoxLampLevelState.Location = new System.Drawing.Point(1095, 29);
            this.comboBoxLampLevelState.Name = "comboBoxLampLevelState";
            this.comboBoxLampLevelState.Size = new System.Drawing.Size(121, 27);
            this.comboBoxLampLevelState.TabIndex = 13;
            this.comboBoxLampLevelState.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBox_DrawItem);
            this.comboBoxLampLevelState.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxLampLevelState_SelectionChangeCommitted);
            // 
            // labelLampLevelState
            // 
            this.labelLampLevelState.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelLampLevelState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLampLevelState.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLampLevelState.Location = new System.Drawing.Point(949, 29);
            this.labelLampLevelState.Name = "labelLampLevelState";
            this.labelLampLevelState.Size = new System.Drawing.Size(140, 27);
            this.labelLampLevelState.TabIndex = 26;
            this.labelLampLevelState.Text = "UNKNOWN";
            this.labelLampLevelState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLuminousFluxLumen
            // 
            this.labelLuminousFluxLumen.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelLuminousFluxLumen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLuminousFluxLumen.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLuminousFluxLumen.Location = new System.Drawing.Point(172, 29);
            this.labelLuminousFluxLumen.Name = "labelLuminousFluxLumen";
            this.labelLuminousFluxLumen.Size = new System.Drawing.Size(140, 27);
            this.labelLuminousFluxLumen.TabIndex = 22;
            this.labelLuminousFluxLumen.Text = "UNKNOWN";
            this.labelLuminousFluxLumen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLuminousFluxLabel
            // 
            this.labelLuminousFluxLabel.AutoSize = true;
            this.labelLuminousFluxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLuminousFluxLabel.Location = new System.Drawing.Point(6, 29);
            this.labelLuminousFluxLabel.Name = "labelLuminousFluxLabel";
            this.labelLuminousFluxLabel.Size = new System.Drawing.Size(160, 26);
            this.labelLuminousFluxLabel.TabIndex = 21;
            this.labelLuminousFluxLabel.Text = "Luminous Flux:";
            // 
            // labelIlluminanceLux
            // 
            this.labelIlluminanceLux.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelIlluminanceLux.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelIlluminanceLux.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIlluminanceLux.Location = new System.Drawing.Point(502, 29);
            this.labelIlluminanceLux.Name = "labelIlluminanceLux";
            this.labelIlluminanceLux.Size = new System.Drawing.Size(140, 27);
            this.labelIlluminanceLux.TabIndex = 24;
            this.labelIlluminanceLux.Text = "UNKNOWN";
            this.labelIlluminanceLux.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLampLevelLabel
            // 
            this.labelLampLevelLabel.AutoSize = true;
            this.labelLampLevelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLampLevelLabel.Location = new System.Drawing.Point(812, 29);
            this.labelLampLevelLabel.Name = "labelLampLevelLabel";
            this.labelLampLevelLabel.Size = new System.Drawing.Size(131, 26);
            this.labelLampLevelLabel.TabIndex = 25;
            this.labelLampLevelLabel.Text = "Lamp Level:";
            // 
            // labelIlluminanceLabel
            // 
            this.labelIlluminanceLabel.AutoSize = true;
            this.labelIlluminanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIlluminanceLabel.Location = new System.Drawing.Point(367, 29);
            this.labelIlluminanceLabel.Name = "labelIlluminanceLabel";
            this.labelIlluminanceLabel.Size = new System.Drawing.Size(129, 26);
            this.labelIlluminanceLabel.TabIndex = 23;
            this.labelIlluminanceLabel.Text = "Illuminance:";
            // 
            // groupBoxWhatsapp
            // 
            this.groupBoxWhatsapp.Controls.Add(this.buttonWhatsappSend);
            this.groupBoxWhatsapp.Controls.Add(this.labelWhatsappNumber);
            this.groupBoxWhatsapp.Controls.Add(this.textBoxWhatsappNumber);
            this.groupBoxWhatsapp.Enabled = false;
            this.groupBoxWhatsapp.Location = new System.Drawing.Point(12, 486);
            this.groupBoxWhatsapp.Name = "groupBoxWhatsapp";
            this.groupBoxWhatsapp.Size = new System.Drawing.Size(665, 76);
            this.groupBoxWhatsapp.TabIndex = 7;
            this.groupBoxWhatsapp.TabStop = false;
            this.groupBoxWhatsapp.Text = "Send Status via WhatsApp";
            // 
            // buttonWhatsappSend
            // 
            this.buttonWhatsappSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonWhatsappSend.Location = new System.Drawing.Point(481, 26);
            this.buttonWhatsappSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonWhatsappSend.Name = "buttonWhatsappSend";
            this.buttonWhatsappSend.Size = new System.Drawing.Size(166, 30);
            this.buttonWhatsappSend.TabIndex = 15;
            this.buttonWhatsappSend.Text = "SEND";
            this.buttonWhatsappSend.UseVisualStyleBackColor = true;
            this.buttonWhatsappSend.Click += new System.EventHandler(this.ButtonWhatsappSend_Click);
            // 
            // labelWhatsappNumber
            // 
            this.labelWhatsappNumber.AutoSize = true;
            this.labelWhatsappNumber.Location = new System.Drawing.Point(12, 31);
            this.labelWhatsappNumber.Name = "labelWhatsappNumber";
            this.labelWhatsappNumber.Size = new System.Drawing.Size(146, 20);
            this.labelWhatsappNumber.TabIndex = 27;
            this.labelWhatsappNumber.Text = "Whatsapp Number:";
            // 
            // textBoxWhatsappNumber
            // 
            this.textBoxWhatsappNumber.Location = new System.Drawing.Point(164, 28);
            this.textBoxWhatsappNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxWhatsappNumber.Name = "textBoxWhatsappNumber";
            this.textBoxWhatsappNumber.Size = new System.Drawing.Size(289, 26);
            this.textBoxWhatsappNumber.TabIndex = 14;
            this.textBoxWhatsappNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxWhatsappNumber_KeyDown);
            // 
            // groupBoxClearConsole
            // 
            this.groupBoxClearConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxClearConsole.Controls.Add(this.buttonSaveLog);
            this.groupBoxClearConsole.Controls.Add(this.buttonClearConsole);
            this.groupBoxClearConsole.Location = new System.Drawing.Point(683, 486);
            this.groupBoxClearConsole.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxClearConsole.Name = "groupBoxClearConsole";
            this.groupBoxClearConsole.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxClearConsole.Size = new System.Drawing.Size(563, 76);
            this.groupBoxClearConsole.TabIndex = 8;
            this.groupBoxClearConsole.TabStop = false;
            this.groupBoxClearConsole.Text = "Console Control";
            // 
            // buttonSaveLog
            // 
            this.buttonSaveLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSaveLog.Location = new System.Drawing.Point(18, 26);
            this.buttonSaveLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonSaveLog.Name = "buttonSaveLog";
            this.buttonSaveLog.Size = new System.Drawing.Size(256, 30);
            this.buttonSaveLog.TabIndex = 16;
            this.buttonSaveLog.Text = "SAVE LOG";
            this.buttonSaveLog.UseVisualStyleBackColor = true;
            this.buttonSaveLog.Click += new System.EventHandler(this.ButtonSaveLog_Click);
            // 
            // buttonClearConsole
            // 
            this.buttonClearConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClearConsole.Location = new System.Drawing.Point(289, 26);
            this.buttonClearConsole.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonClearConsole.Name = "buttonClearConsole";
            this.buttonClearConsole.Size = new System.Drawing.Size(256, 30);
            this.buttonClearConsole.TabIndex = 17;
            this.buttonClearConsole.Text = "CLEAR CONSOLE";
            this.buttonClearConsole.UseVisualStyleBackColor = true;
            this.buttonClearConsole.Click += new System.EventHandler(this.ButtonClearConsole_Click);
            // 
            // FormSmartHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 744);
            this.Controls.Add(this.groupBoxClearConsole);
            this.Controls.Add(this.groupBoxWhatsapp);
            this.Controls.Add(this.groupBoxRemoteClientState);
            this.Controls.Add(this.groupBoxRemoteClient);
            this.Controls.Add(this.groupBoxConsole);
            this.Controls.Add(this.groupBoxConnection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(1280, 800);
            this.MinimumSize = new System.Drawing.Size(1280, 800);
            this.Name = "FormSmartHome";
            this.Text = "SmartHome";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSmartHome_FormClosing);
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            this.groupBoxConsole.ResumeLayout(false);
            this.groupBoxConsole.PerformLayout();
            this.groupBoxRemoteClient.ResumeLayout(false);
            this.groupBoxRemoteClient.PerformLayout();
            this.groupBoxRemoteClientState.ResumeLayout(false);
            this.groupBoxWeatherAndWindowControl.ResumeLayout(false);
            this.groupBoxWeatherAndWindowControl.PerformLayout();
            this.groupBoxHeatingAndFanControl.ResumeLayout(false);
            this.groupBoxHeatingAndFanControl.PerformLayout();
            this.groupBoxLightingControl.ResumeLayout(false);
            this.groupBoxLightingControl.PerformLayout();
            this.groupBoxWhatsapp.ResumeLayout(false);
            this.groupBoxWhatsapp.PerformLayout();
            this.groupBoxClearConsole.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.GroupBox groupBoxConsole;
        private System.Windows.Forms.Label labelHost;
        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.TextBox textBoxClientID;
        private System.Windows.Forms.Label labelClientID;
        private System.Windows.Forms.Label labelRemoteClientStatus;
        private System.Windows.Forms.Label labelRemoteClientStatusLabel;
        private System.Windows.Forms.Label labelRemoteClientID;
        private System.Windows.Forms.TextBox textBoxRemoteClientID;
        private System.Windows.Forms.Button buttonMQTTConnection;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.GroupBox groupBoxRemoteClient;
        private System.Windows.Forms.TextBox textBoxConsole;
        private System.Windows.Forms.Button buttonMQTTSubscription;
        private System.Windows.Forms.GroupBox groupBoxRemoteClientState;
        private System.Windows.Forms.GroupBox groupBoxWhatsapp;
        private System.Windows.Forms.Button buttonWhatsappSend;
        private System.Windows.Forms.Label labelWhatsappNumber;
        private System.Windows.Forms.TextBox textBoxWhatsappNumber;
        private System.Windows.Forms.Label labelWeatherState;
        private System.Windows.Forms.Label labelWeatherLabel;
        private System.Windows.Forms.Label labelTemperatureLabel;
        private System.Windows.Forms.Label labelTemperatureDegrees;
        private System.Windows.Forms.Label labelLuminousFluxLabel;
        private System.Windows.Forms.Label labelLuminousFluxLumen;
        private System.Windows.Forms.Label labelWindowSpeedLabel;
        private System.Windows.Forms.ComboBox comboBoxWindowSpeedState;
        private System.Windows.Forms.ComboBox comboBoxWindowState;
        private System.Windows.Forms.Label labelWindowStateLabel;
        private System.Windows.Forms.ComboBox comboBoxHeaterState;
        private System.Windows.Forms.Label labelHeaterStateLabel;
        private System.Windows.Forms.ComboBox comboBoxFanSpeedState;
        private System.Windows.Forms.Label labelFanSpeedLabel;
        private System.Windows.Forms.Label labelIlluminanceLabel;
        private System.Windows.Forms.Label labelIlluminanceLux;
        private System.Windows.Forms.ComboBox comboBoxLampLevelState;
        private System.Windows.Forms.Label labelLampLevelLabel;
        private System.Windows.Forms.GroupBox groupBoxClearConsole;
        private System.Windows.Forms.Button buttonClearConsole;
        private System.Windows.Forms.Label labelHeaterState;
        private System.Windows.Forms.Label labelWindowSpeedState;
        private System.Windows.Forms.Label labelFanSpeedState;
        private System.Windows.Forms.Label labelWindowState;
        private System.Windows.Forms.Label labelLampLevelState;
        private System.Windows.Forms.GroupBox groupBoxLightingControl;
        private System.Windows.Forms.GroupBox groupBoxHeatingAndFanControl;
        private System.Windows.Forms.GroupBox groupBoxWeatherAndWindowControl;
        private System.Windows.Forms.Button buttonSaveLog;
    }
}

