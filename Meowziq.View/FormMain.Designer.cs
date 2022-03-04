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
            // sequence
            // 
            this._sequence.Format = 1;
            // 
            // sequencer
            // 
            this._sequencer.Position = 0;
            this._sequencer.Sequence = this._sequence;
            this._sequencer.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.sequencer_ChannelMessagePlayed);
            // 
            // pianoControl
            // 
            this._pianoControl.HighNoteID = 109;
            this._pianoControl.Location = new System.Drawing.Point(11, 193);
            this._pianoControl.LowNoteID = 21;
            this._pianoControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._pianoControl.Name = "pianoControl";
            this._pianoControl.NoteOnColor = System.Drawing.Color.SkyBlue;
            this._pianoControl.Size = new System.Drawing.Size(688, 84);
            this._pianoControl.TabIndex = 4;
            this._pianoControl.TabStop = false;
            this._pianoControl.Text = "pianoControl";
            // 
            // buttonPlay
            // 
            this._buttonPlay.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._buttonPlay.Location = new System.Drawing.Point(17, 79);
            this._buttonPlay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._buttonPlay.Name = "buttonPlay";
            this._buttonPlay.Size = new System.Drawing.Size(66, 32);
            this._buttonPlay.TabIndex = 1;
            this._buttonPlay.Text = "PLAY";
            this._buttonPlay.UseVisualStyleBackColor = true;
            this._buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this._buttonStop.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._buttonStop.Location = new System.Drawing.Point(97, 79);
            this._buttonStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._buttonStop.Name = "buttonStop";
            this._buttonStop.Size = new System.Drawing.Size(66, 32);
            this._buttonStop.TabIndex = 2;
            this._buttonStop.Text = "STOP";
            this._buttonStop.UseVisualStyleBackColor = true;
            this._buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonLoad
            // 
            this._buttonLoad.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._buttonLoad.Location = new System.Drawing.Point(178, 79);
            this._buttonLoad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._buttonLoad.Name = "buttonLoad";
            this._buttonLoad.Size = new System.Drawing.Size(66, 32);
            this._buttonLoad.TabIndex = 3;
            this._buttonLoad.Text = "LOAD";
            this._buttonLoad.UseVisualStyleBackColor = true;
            this._buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonConvert
            // 
            this._buttonConvert.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._buttonConvert.Location = new System.Drawing.Point(257, 79);
            this._buttonConvert.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._buttonConvert.Name = "buttonConvert";
            this._buttonConvert.Size = new System.Drawing.Size(80, 32);
            this._buttonConvert.TabIndex = 26;
            this._buttonConvert.Text = "to SMF";
            this._buttonConvert.UseVisualStyleBackColor = true;
            this._buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // textBoxBeat
            // 
            this._textBoxBeat.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxBeat.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxBeat.Location = new System.Drawing.Point(16, 37);
            this._textBoxBeat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxBeat.Name = "textBoxBeat";
            this._textBoxBeat.Size = new System.Drawing.Size(66, 31);
            this._textBoxBeat.TabIndex = 0;
            this._textBoxBeat.TabStop = false;
            this._textBoxBeat.Text = "0";
            this._textBoxBeat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxMeas
            // 
            this._textBoxMeas.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxMeas.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxMeas.Location = new System.Drawing.Point(97, 36);
            this._textBoxMeas.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxMeas.Name = "textBoxMeas";
            this._textBoxMeas.Size = new System.Drawing.Size(66, 31);
            this._textBoxMeas.TabIndex = 4;
            this._textBoxMeas.TabStop = false;
            this._textBoxMeas.Text = "0";
            this._textBoxMeas.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxSongName
            // 
            this._textBoxSongName.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxSongName.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxSongName.Location = new System.Drawing.Point(178, 36);
            this._textBoxSongName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxSongName.Name = "textBoxSongName";
            this._textBoxSongName.Size = new System.Drawing.Size(159, 31);
            this._textBoxSongName.TabIndex = 12;
            this._textBoxSongName.TabStop = false;
            this._textBoxSongName.Text = "------------";
            // 
            // textBoxKey
            // 
            this._textBoxKey.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxKey.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxKey.Location = new System.Drawing.Point(516, 94);
            this._textBoxKey.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxKey.Name = "textBoxKey";
            this._textBoxKey.Size = new System.Drawing.Size(66, 31);
            this._textBoxKey.TabIndex = 14;
            this._textBoxKey.TabStop = false;
            this._textBoxKey.Text = "---";
            // 
            // textBoxDegree
            // 
            this._textBoxDegree.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxDegree.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxDegree.Location = new System.Drawing.Point(353, 37);
            this._textBoxDegree.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxDegree.Name = "textBoxDegree";
            this._textBoxDegree.Size = new System.Drawing.Size(66, 31);
            this._textBoxDegree.TabIndex = 16;
            this._textBoxDegree.TabStop = false;
            this._textBoxDegree.Text = "---";
            // 
            // textBoxKeyMode
            // 
            this._textBoxKeyMode.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxKeyMode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxKeyMode.Location = new System.Drawing.Point(433, 94);
            this._textBoxKeyMode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxKeyMode.Name = "textBoxKeyMode";
            this._textBoxKeyMode.Size = new System.Drawing.Size(66, 31);
            this._textBoxKeyMode.TabIndex = 18;
            this._textBoxKeyMode.TabStop = false;
            this._textBoxKeyMode.Text = "---";
            // 
            // textBoxCode
            // 
            this._textBoxCode.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxCode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxCode.Location = new System.Drawing.Point(515, 36);
            this._textBoxCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxCode.Name = "textBoxCode";
            this._textBoxCode.Size = new System.Drawing.Size(66, 31);
            this._textBoxCode.TabIndex = 22;
            this._textBoxCode.TabStop = false;
            this._textBoxCode.Text = "---";
            // 
            // textBoxMode
            // 
            this._textBoxMode.BackColor = System.Drawing.Color.PaleGreen;
            this._textBoxMode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._textBoxMode.Location = new System.Drawing.Point(434, 36);
            this._textBoxMode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBoxMode.Name = "textBoxMode";
            this._textBoxMode.Size = new System.Drawing.Size(66, 31);
            this._textBoxMode.TabIndex = 24;
            this._textBoxMode.TabStop = false;
            this._textBoxMode.Text = "---";
            // 
            // labelMeas
            // 
            this._labelMeas.AutoSize = true;
            this._labelMeas.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelMeas.ForeColor = System.Drawing.Color.White;
            this._labelMeas.Location = new System.Drawing.Point(96, 16);
            this._labelMeas.Name = "labelMeas";
            this._labelMeas.Size = new System.Drawing.Size(47, 17);
            this._labelMeas.TabIndex = 5;
            this._labelMeas.Text = "MEAS";
            // 
            // labelTitle
            // 
            this._labelTitle.AutoSize = true;
            this._labelTitle.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelTitle.ForeColor = System.Drawing.Color.Gold;
            this._labelTitle.Location = new System.Drawing.Point(594, 30);
            this._labelTitle.Name = "labelTitle";
            this._labelTitle.Size = new System.Drawing.Size(110, 34);
            this._labelTitle.TabIndex = 6;
            this._labelTitle.Text = "MeowziQ";
            // 
            // labelSubTitle
            // 
            this._labelSubTitle.AutoSize = true;
            this._labelSubTitle.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelSubTitle.ForeColor = System.Drawing.Color.Gold;
            this._labelSubTitle.Location = new System.Drawing.Point(602, 15);
            this._labelSubTitle.Name = "labelSubTitle";
            this._labelSubTitle.Size = new System.Drawing.Size(92, 16);
            this._labelSubTitle.TabIndex = 7;
            this._labelSubTitle.Text = "music sequencer";
            // 
            // labelVersion
            // 
            this._labelVersion.AutoSize = true;
            this._labelVersion.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelVersion.ForeColor = System.Drawing.Color.Gold;
            this._labelVersion.Location = new System.Drawing.Point(645, 64);
            this._labelVersion.Name = "labelVersion";
            this._labelVersion.Size = new System.Drawing.Size(47, 16);
            this._labelVersion.TabIndex = 8;
            this._labelVersion.Text = "ver 1.0.0";
            // 
            // labelBeat
            // 
            this._labelBeat.AutoSize = true;
            this._labelBeat.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelBeat.ForeColor = System.Drawing.Color.White;
            this._labelBeat.Location = new System.Drawing.Point(14, 16);
            this._labelBeat.Name = "labelBeat";
            this._labelBeat.Size = new System.Drawing.Size(44, 17);
            this._labelBeat.TabIndex = 9;
            this._labelBeat.Text = "BEAT";
            // 
            // labelPlay
            // 
            this._labelPlay.AutoSize = true;
            this._labelPlay.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelPlay.ForeColor = System.Drawing.Color.DimGray;
            this._labelPlay.Location = new System.Drawing.Point(36, 113);
            this._labelPlay.Name = "labelPlay";
            this._labelPlay.Size = new System.Drawing.Size(29, 24);
            this._labelPlay.TabIndex = 10;
            this._labelPlay.Text = "●";
            // 
            // labelSongName
            // 
            this._labelSongName.AutoSize = true;
            this._labelSongName.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelSongName.ForeColor = System.Drawing.Color.White;
            this._labelSongName.Location = new System.Drawing.Point(176, 16);
            this._labelSongName.Name = "labelSongName";
            this._labelSongName.Size = new System.Drawing.Size(48, 17);
            this._labelSongName.TabIndex = 13;
            this._labelSongName.Text = "SONG";
            // 
            // labelKey
            // 
            this._labelKey.AutoSize = true;
            this._labelKey.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelKey.ForeColor = System.Drawing.Color.White;
            this._labelKey.Location = new System.Drawing.Point(514, 73);
            this._labelKey.Name = "labelKey";
            this._labelKey.Size = new System.Drawing.Size(35, 17);
            this._labelKey.TabIndex = 15;
            this._labelKey.Text = "KEY";
            // 
            // labelDegree
            // 
            this._labelDegree.AutoSize = true;
            this._labelDegree.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelDegree.ForeColor = System.Drawing.Color.White;
            this._labelDegree.Location = new System.Drawing.Point(350, 15);
            this._labelDegree.Name = "labelDegree";
            this._labelDegree.Size = new System.Drawing.Size(62, 17);
            this._labelDegree.TabIndex = 17;
            this._labelDegree.Text = "DEGREE";
            // 
            // labelKeyMode
            // 
            this._labelKeyMode.AutoSize = true;
            this._labelKeyMode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelKeyMode.ForeColor = System.Drawing.Color.White;
            this._labelKeyMode.Location = new System.Drawing.Point(430, 75);
            this._labelKeyMode.Name = "labelKeyMode";
            this._labelKeyMode.Size = new System.Drawing.Size(74, 15);
            this._labelKeyMode.TabIndex = 19;
            this._labelKeyMode.Text = "KEY MODE";
            // 
            // labelCode
            // 
            this._labelCode.AutoSize = true;
            this._labelCode.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelCode.ForeColor = System.Drawing.Color.White;
            this._labelCode.Location = new System.Drawing.Point(513, 16);
            this._labelCode.Name = "labelCode";
            this._labelCode.Size = new System.Drawing.Size(45, 17);
            this._labelCode.TabIndex = 23;
            this._labelCode.Text = "CODE";
            // 
            // labelMode
            // 
            this._labelMode.AutoSize = true;
            this._labelMode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelMode.ForeColor = System.Drawing.Color.White;
            this._labelMode.Location = new System.Drawing.Point(430, 17);
            this._labelMode.Name = "labelMode";
            this._labelMode.Size = new System.Drawing.Size(45, 15);
            this._labelMode.TabIndex = 25;
            this._labelMode.Text = "MODE";
            // 
            // labelModulation
            // 
            this._labelModulation.AutoSize = true;
            this._labelModulation.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelModulation.ForeColor = System.Drawing.Color.DimGray;
            this._labelModulation.Location = new System.Drawing.Point(429, 126);
            this._labelModulation.Name = "labelModulation";
            this._labelModulation.Size = new System.Drawing.Size(29, 24);
            this._labelModulation.TabIndex = 27;
            this._labelModulation.Text = "●";
            // 
            // labelModulationText
            // 
            this._labelModulationText.AutoSize = true;
            this._labelModulationText.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._labelModulationText.ForeColor = System.Drawing.Color.White;
            this._labelModulationText.Location = new System.Drawing.Point(454, 131);
            this._labelModulationText.Name = "labelModulationText";
            this._labelModulationText.Size = new System.Drawing.Size(95, 15);
            this._labelModulationText.TabIndex = 28;
            this._labelModulationText.Text = "MODULATION";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(709, 290);
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
            this.Controls.Add(this._pianoControl);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
