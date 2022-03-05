namespace Meowziq.View {
    partial class FormMain {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this._sequence = new Sanford.Multimedia.Midi.Sequence();
            this._sequencer = new Sanford.Multimedia.Midi.Sequencer();
            this._pianoControl = new Sanford.Multimedia.Midi.UI.PianoControl();
            this._buttonPlay = new System.Windows.Forms.Button();
            this._buttonStop = new System.Windows.Forms.Button();
            this._buttonLoad = new System.Windows.Forms.Button();
            this._buttonConvert = new System.Windows.Forms.Button();
            this._textBoxMeas = new System.Windows.Forms.TextBox();
            this._textBoxBeat = new System.Windows.Forms.TextBox();
            this._textBoxSongName = new System.Windows.Forms.TextBox();
            this._textBoxKey = new System.Windows.Forms.TextBox();
            this._textBoxDegree = new System.Windows.Forms.TextBox();
            this._textBoxKeyMode = new System.Windows.Forms.TextBox();
            this._textBoxCode = new System.Windows.Forms.TextBox();
            this._textBoxMode = new System.Windows.Forms.TextBox();
            this._labelMeas = new System.Windows.Forms.Label();
            this._labelTitle = new System.Windows.Forms.Label();
            this._labelSubTitle = new System.Windows.Forms.Label();
            this._labelVersion = new System.Windows.Forms.Label();
            this._labelBeat = new System.Windows.Forms.Label();
            this._labelPlay = new System.Windows.Forms.Label();
            this._labelSongName = new System.Windows.Forms.Label();
            this._labelKey = new System.Windows.Forms.Label();
            this._labelDegree = new System.Windows.Forms.Label();
            this._labelKeyMode = new System.Windows.Forms.Label();
            this._labelCode = new System.Windows.Forms.Label();
            this._labelMode = new System.Windows.Forms.Label();
            this._labelModulation = new System.Windows.Forms.Label();
            this._labelModulationText = new System.Windows.Forms.Label();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // _buttonPlay
            // 
            this._buttonPlay.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._buttonPlay.Location = new System.Drawing.Point(23, 132);
            this._buttonPlay.Name = "_buttonPlay";
            this._buttonPlay.Size = new System.Drawing.Size(88, 53);
            this._buttonPlay.TabIndex = 1;
            this._buttonPlay.Text = "PLAY";
            this._buttonPlay.UseVisualStyleBackColor = true;
            this._buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // _buttonStop
            // 
            this._buttonStop.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._buttonStop.Location = new System.Drawing.Point(129, 132);
            this._buttonStop.Name = "_buttonStop";
            this._buttonStop.Size = new System.Drawing.Size(88, 53);
            this._buttonStop.TabIndex = 2;
            this._buttonStop.Text = "STOP";
            this._buttonStop.UseVisualStyleBackColor = true;
            this._buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // _buttonLoad
            // 
            this._buttonLoad.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._buttonLoad.Location = new System.Drawing.Point(237, 132);
            this._buttonLoad.Name = "_buttonLoad";
            this._buttonLoad.Size = new System.Drawing.Size(88, 53);
            this._buttonLoad.TabIndex = 3;
            this._buttonLoad.Text = "LOAD";
            this._buttonLoad.UseVisualStyleBackColor = true;
            this._buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // _buttonConvert
            // 
            this._buttonConvert.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._buttonConvert.Location = new System.Drawing.Point(343, 132);
            this._buttonConvert.Name = "_buttonConvert";
            this._buttonConvert.Size = new System.Drawing.Size(107, 53);
            this._buttonConvert.TabIndex = 26;
            this._buttonConvert.Text = "to SMF";
            this._buttonConvert.UseVisualStyleBackColor = true;
            this._buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // _textBoxMeas
            // 
            this._textBoxMeas.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxMeas.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxMeas.Location = new System.Drawing.Point(129, 60);
            this._textBoxMeas.Name = "_textBoxMeas";
            this._textBoxMeas.Size = new System.Drawing.Size(87, 37);
            this._textBoxMeas.TabIndex = 4;
            this._textBoxMeas.TabStop = false;
            this._textBoxMeas.Text = "0";
            this._textBoxMeas.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textBoxBeat
            // 
            this._textBoxBeat.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxBeat.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxBeat.Location = new System.Drawing.Point(21, 62);
            this._textBoxBeat.Name = "_textBoxBeat";
            this._textBoxBeat.Size = new System.Drawing.Size(87, 37);
            this._textBoxBeat.TabIndex = 0;
            this._textBoxBeat.TabStop = false;
            this._textBoxBeat.Text = "0";
            this._textBoxBeat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textBoxSongName
            // 
            this._textBoxSongName.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxSongName.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxSongName.Location = new System.Drawing.Point(237, 60);
            this._textBoxSongName.Name = "_textBoxSongName";
            this._textBoxSongName.Size = new System.Drawing.Size(211, 37);
            this._textBoxSongName.TabIndex = 12;
            this._textBoxSongName.TabStop = false;
            this._textBoxSongName.Text = "------------";
            // 
            // _textBoxKey
            // 
            this._textBoxKey.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxKey.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxKey.Location = new System.Drawing.Point(688, 157);
            this._textBoxKey.Name = "_textBoxKey";
            this._textBoxKey.Size = new System.Drawing.Size(87, 37);
            this._textBoxKey.TabIndex = 14;
            this._textBoxKey.TabStop = false;
            this._textBoxKey.Text = "---";
            // 
            // _textBoxDegree
            // 
            this._textBoxDegree.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxDegree.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxDegree.Location = new System.Drawing.Point(471, 62);
            this._textBoxDegree.Name = "_textBoxDegree";
            this._textBoxDegree.Size = new System.Drawing.Size(87, 37);
            this._textBoxDegree.TabIndex = 16;
            this._textBoxDegree.TabStop = false;
            this._textBoxDegree.Text = "---";
            // 
            // _textBoxKeyMode
            // 
            this._textBoxKeyMode.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxKeyMode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxKeyMode.Location = new System.Drawing.Point(577, 157);
            this._textBoxKeyMode.Name = "_textBoxKeyMode";
            this._textBoxKeyMode.Size = new System.Drawing.Size(87, 37);
            this._textBoxKeyMode.TabIndex = 18;
            this._textBoxKeyMode.TabStop = false;
            this._textBoxKeyMode.Text = "---";
            // 
            // _textBoxCode
            // 
            this._textBoxCode.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxCode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxCode.Location = new System.Drawing.Point(687, 60);
            this._textBoxCode.Name = "_textBoxCode";
            this._textBoxCode.Size = new System.Drawing.Size(87, 37);
            this._textBoxCode.TabIndex = 22;
            this._textBoxCode.TabStop = false;
            this._textBoxCode.Text = "---";
            // 
            // _textBoxMode
            // 
            this._textBoxMode.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxMode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textBoxMode.Location = new System.Drawing.Point(579, 60);
            this._textBoxMode.Name = "_textBoxMode";
            this._textBoxMode.Size = new System.Drawing.Size(87, 37);
            this._textBoxMode.TabIndex = 24;
            this._textBoxMode.TabStop = false;
            this._textBoxMode.Text = "---";
            // 
            // _labelMeas
            // 
            this._labelMeas.AutoSize = true;
            this._labelMeas.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelMeas.ForeColor = System.Drawing.Color.White;
            this._labelMeas.Location = new System.Drawing.Point(128, 27);
            this._labelMeas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelMeas.Name = "_labelMeas";
            this._labelMeas.Size = new System.Drawing.Size(60, 22);
            this._labelMeas.TabIndex = 5;
            this._labelMeas.Text = "MEAS";
            // 
            // _labelTitle
            // 
            this._labelTitle.AutoSize = true;
            this._labelTitle.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._labelTitle.ForeColor = System.Drawing.Color.Gold;
            this._labelTitle.Location = new System.Drawing.Point(792, 50);
            this._labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelTitle.Name = "_labelTitle";
            this._labelTitle.Size = new System.Drawing.Size(138, 42);
            this._labelTitle.TabIndex = 6;
            this._labelTitle.Text = "MeowziQ";
            // 
            // _labelSubTitle
            // 
            this._labelSubTitle.AutoSize = true;
            this._labelSubTitle.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._labelSubTitle.ForeColor = System.Drawing.Color.Gold;
            this._labelSubTitle.Location = new System.Drawing.Point(803, 25);
            this._labelSubTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelSubTitle.Name = "_labelSubTitle";
            this._labelSubTitle.Size = new System.Drawing.Size(117, 19);
            this._labelSubTitle.TabIndex = 7;
            this._labelSubTitle.Text = "music sequencer";
            // 
            // _labelVersion
            // 
            this._labelVersion.AutoSize = true;
            this._labelVersion.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._labelVersion.ForeColor = System.Drawing.Color.Gold;
            this._labelVersion.Location = new System.Drawing.Point(860, 107);
            this._labelVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelVersion.Name = "_labelVersion";
            this._labelVersion.Size = new System.Drawing.Size(60, 19);
            this._labelVersion.TabIndex = 8;
            this._labelVersion.Text = "ver 1.0.0";
            // 
            // _labelBeat
            // 
            this._labelBeat.AutoSize = true;
            this._labelBeat.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelBeat.ForeColor = System.Drawing.Color.White;
            this._labelBeat.Location = new System.Drawing.Point(19, 27);
            this._labelBeat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelBeat.Name = "_labelBeat";
            this._labelBeat.Size = new System.Drawing.Size(56, 22);
            this._labelBeat.TabIndex = 9;
            this._labelBeat.Text = "BEAT";
            // 
            // _labelPlay
            // 
            this._labelPlay.AutoSize = true;
            this._labelPlay.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._labelPlay.ForeColor = System.Drawing.Color.DimGray;
            this._labelPlay.Location = new System.Drawing.Point(48, 188);
            this._labelPlay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelPlay.Name = "_labelPlay";
            this._labelPlay.Size = new System.Drawing.Size(37, 30);
            this._labelPlay.TabIndex = 10;
            this._labelPlay.Text = "●";
            // 
            // _labelSongName
            // 
            this._labelSongName.AutoSize = true;
            this._labelSongName.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelSongName.ForeColor = System.Drawing.Color.White;
            this._labelSongName.Location = new System.Drawing.Point(235, 27);
            this._labelSongName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelSongName.Name = "_labelSongName";
            this._labelSongName.Size = new System.Drawing.Size(62, 22);
            this._labelSongName.TabIndex = 13;
            this._labelSongName.Text = "SONG";
            // 
            // _labelKey
            // 
            this._labelKey.AutoSize = true;
            this._labelKey.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelKey.ForeColor = System.Drawing.Color.White;
            this._labelKey.Location = new System.Drawing.Point(685, 122);
            this._labelKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelKey.Name = "_labelKey";
            this._labelKey.Size = new System.Drawing.Size(46, 22);
            this._labelKey.TabIndex = 15;
            this._labelKey.Text = "KEY";
            // 
            // _labelDegree
            // 
            this._labelDegree.AutoSize = true;
            this._labelDegree.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelDegree.ForeColor = System.Drawing.Color.White;
            this._labelDegree.Location = new System.Drawing.Point(467, 25);
            this._labelDegree.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelDegree.Name = "_labelDegree";
            this._labelDegree.Size = new System.Drawing.Size(82, 22);
            this._labelDegree.TabIndex = 17;
            this._labelDegree.Text = "DEGREE";
            // 
            // _labelKeyMode
            // 
            this._labelKeyMode.AutoSize = true;
            this._labelKeyMode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelKeyMode.ForeColor = System.Drawing.Color.White;
            this._labelKeyMode.Location = new System.Drawing.Point(573, 125);
            this._labelKeyMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelKeyMode.Name = "_labelKeyMode";
            this._labelKeyMode.Size = new System.Drawing.Size(94, 19);
            this._labelKeyMode.TabIndex = 19;
            this._labelKeyMode.Text = "KEY MODE";
            // 
            // _labelCode
            // 
            this._labelCode.AutoSize = true;
            this._labelCode.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelCode.ForeColor = System.Drawing.Color.White;
            this._labelCode.Location = new System.Drawing.Point(684, 27);
            this._labelCode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelCode.Name = "_labelCode";
            this._labelCode.Size = new System.Drawing.Size(60, 22);
            this._labelCode.TabIndex = 23;
            this._labelCode.Text = "CODE";
            // 
            // _labelMode
            // 
            this._labelMode.AutoSize = true;
            this._labelMode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelMode.ForeColor = System.Drawing.Color.White;
            this._labelMode.Location = new System.Drawing.Point(573, 28);
            this._labelMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelMode.Name = "_labelMode";
            this._labelMode.Size = new System.Drawing.Size(57, 19);
            this._labelMode.TabIndex = 25;
            this._labelMode.Text = "MODE";
            // 
            // _labelModulation
            // 
            this._labelModulation.AutoSize = true;
            this._labelModulation.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._labelModulation.ForeColor = System.Drawing.Color.DimGray;
            this._labelModulation.Location = new System.Drawing.Point(572, 210);
            this._labelModulation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelModulation.Name = "_labelModulation";
            this._labelModulation.Size = new System.Drawing.Size(37, 30);
            this._labelModulation.TabIndex = 27;
            this._labelModulation.Text = "●";
            // 
            // _labelModulationText
            // 
            this._labelModulationText.AutoSize = true;
            this._labelModulationText.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._labelModulationText.ForeColor = System.Drawing.Color.White;
            this._labelModulationText.Location = new System.Drawing.Point(605, 218);
            this._labelModulationText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelModulationText.Name = "_labelModulationText";
            this._labelModulationText.Size = new System.Drawing.Size(120, 19);
            this._labelModulationText.TabIndex = 28;
            this._labelModulationText.Text = "MODULATION";
            // 
            // _sequence
            // 
            this._sequence.Format = 1;
            // 
            // _sequencer
            // 
            this._sequencer.Position = 0;
            this._sequencer.Sequence = this._sequence;
            this._sequencer.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.sequencer_ChannelMessagePlayed);
            // 
            // _pianoControl
            // 
            this._pianoControl.HighNoteID = 109;
            this._pianoControl.Location = new System.Drawing.Point(15, 322);
            this._pianoControl.LowNoteID = 21;
            this._pianoControl.Name = "pianoControl";
            this._pianoControl.NoteOnColor = System.Drawing.Color.SkyBlue;
            this._pianoControl.Size = new System.Drawing.Size(915, 140);
            this._pianoControl.TabIndex = 4;
            this._pianoControl.TabStop = false;
            this._pianoControl.Text = "pianoControl";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(945, 483);
            this.Controls.Add(this._pianoControl);
            this.Controls.Add(this._labelModulationText);
            this.Controls.Add(this._labelModulation);
            this.Controls.Add(this._labelMode);
            this.Controls.Add(this._labelCode);
            this.Controls.Add(this._labelKeyMode);
            this.Controls.Add(this._labelDegree);
            this.Controls.Add(this._labelKey);
            this.Controls.Add(this._labelSongName);
            this.Controls.Add(this._labelPlay);
            this.Controls.Add(this._labelBeat);
            this.Controls.Add(this._labelVersion);
            this.Controls.Add(this._labelSubTitle);
            this.Controls.Add(this._labelTitle);
            this.Controls.Add(this._labelMeas);
            this.Controls.Add(this._textBoxMode);
            this.Controls.Add(this._textBoxCode);
            this.Controls.Add(this._textBoxKeyMode);
            this.Controls.Add(this._textBoxDegree);
            this.Controls.Add(this._textBoxKey);
            this.Controls.Add(this._textBoxSongName);
            this.Controls.Add(this._textBoxMeas);
            this.Controls.Add(this._textBoxBeat);
            this.Controls.Add(this._buttonConvert);
            this.Controls.Add(this._buttonLoad);
            this.Controls.Add(this._buttonStop);
            this.Controls.Add(this._buttonPlay);
            this.Name = "FormMain";
            this.Text = "© STUDIO MeowToon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sanford.Multimedia.Midi.Sequence _sequence;
        private Sanford.Multimedia.Midi.Sequencer _sequencer;
        private Sanford.Multimedia.Midi.UI.PianoControl _pianoControl;
        private System.Windows.Forms.Button _buttonPlay;
        private System.Windows.Forms.Button _buttonStop;
        private System.Windows.Forms.Button _buttonLoad;
        private System.Windows.Forms.Button _buttonConvert;
        private System.Windows.Forms.TextBox _textBoxBeat;
        private System.Windows.Forms.TextBox _textBoxMeas;
        private System.Windows.Forms.TextBox _textBoxSongName;
        private System.Windows.Forms.TextBox _textBoxKey;
        private System.Windows.Forms.TextBox _textBoxDegree;
        private System.Windows.Forms.TextBox _textBoxKeyMode;
        private System.Windows.Forms.TextBox _textBoxCode;
        private System.Windows.Forms.TextBox _textBoxMode;
        private System.Windows.Forms.Label _labelMeas;
        private System.Windows.Forms.Label _labelTitle;
        private System.Windows.Forms.Label _labelSubTitle;
        private System.Windows.Forms.Label _labelVersion;
        private System.Windows.Forms.Label _labelBeat;
        private System.Windows.Forms.Label _labelPlay;
        private System.Windows.Forms.Label _labelSongName;
        private System.Windows.Forms.Label _labelKey;
        private System.Windows.Forms.Label _labelDegree;
        private System.Windows.Forms.Label _labelKeyMode;
        private System.Windows.Forms.Label _labelCode;
        private System.Windows.Forms.Label _labelMode;
        private System.Windows.Forms.Label _labelModulation;
        private System.Windows.Forms.Label _labelModulationText;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
    }
}
