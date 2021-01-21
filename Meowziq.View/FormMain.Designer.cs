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
            this.sequence = new Sanford.Multimedia.Midi.Sequence();
            this.sequencer = new Sanford.Multimedia.Midi.Sequencer();
            this.pianoControl = new Sanford.Multimedia.Midi.UI.PianoControl();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.textBoxMeas = new System.Windows.Forms.TextBox();
            this.textBoxBeat = new System.Windows.Forms.TextBox();
            this.textBoxSongName = new System.Windows.Forms.TextBox();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.textBoxDegree = new System.Windows.Forms.TextBox();
            this.textBoxKeyMode = new System.Windows.Forms.TextBox();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.textBoxMode = new System.Windows.Forms.TextBox();
            this.labelMeas = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelSubTitle = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelBeat = new System.Windows.Forms.Label();
            this.labelPlay = new System.Windows.Forms.Label();
            this.labelSongName = new System.Windows.Forms.Label();
            this.labelKey = new System.Windows.Forms.Label();
            this.labelDegree = new System.Windows.Forms.Label();
            this.labelKeyMode = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.labelModulation = new System.Windows.Forms.Label();
            this.labelModulationText = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // sequence
            // 
            this.sequence.Format = 1;
            // 
            // sequencer
            // 
            this.sequencer.Position = 0;
            this.sequencer.Sequence = this.sequence;
            this.sequencer.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.sequencer_ChannelMessagePlayed);
            // 
            // pianoControl
            // 
            this.pianoControl.HighNoteID = 109;
            this.pianoControl.Location = new System.Drawing.Point(11, 193);
            this.pianoControl.LowNoteID = 21;
            this.pianoControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pianoControl.Name = "pianoControl";
            this.pianoControl.NoteOnColor = System.Drawing.Color.SkyBlue;
            this.pianoControl.Size = new System.Drawing.Size(688, 84);
            this.pianoControl.TabIndex = 4;
            this.pianoControl.TabStop = false;
            this.pianoControl.Text = "pianoControl";
            // 
            // buttonPlay
            // 
            this.buttonPlay.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay.Location = new System.Drawing.Point(17, 79);
            this.buttonPlay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(66, 32);
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Text = "PLAY";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonStop.Location = new System.Drawing.Point(97, 79);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(66, 32);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "STOP";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonLoad.Location = new System.Drawing.Point(178, 79);
            this.buttonLoad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(66, 32);
            this.buttonLoad.TabIndex = 3;
            this.buttonLoad.Text = "LOAD";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonConvert
            // 
            this.buttonConvert.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonConvert.Location = new System.Drawing.Point(257, 79);
            this.buttonConvert.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(80, 32);
            this.buttonConvert.TabIndex = 26;
            this.buttonConvert.Text = "to SMF";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // textBoxBeat
            // 
            this.textBoxBeat.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxBeat.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxBeat.Location = new System.Drawing.Point(16, 37);
            this.textBoxBeat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxBeat.Name = "textBoxBeat";
            this.textBoxBeat.Size = new System.Drawing.Size(66, 31);
            this.textBoxBeat.TabIndex = 0;
            this.textBoxBeat.TabStop = false;
            this.textBoxBeat.Text = "0";
            this.textBoxBeat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxMeas
            // 
            this.textBoxMeas.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxMeas.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxMeas.Location = new System.Drawing.Point(97, 36);
            this.textBoxMeas.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxMeas.Name = "textBoxMeas";
            this.textBoxMeas.Size = new System.Drawing.Size(66, 31);
            this.textBoxMeas.TabIndex = 4;
            this.textBoxMeas.TabStop = false;
            this.textBoxMeas.Text = "0";
            this.textBoxMeas.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxSongName
            // 
            this.textBoxSongName.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxSongName.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxSongName.Location = new System.Drawing.Point(178, 36);
            this.textBoxSongName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxSongName.Name = "textBoxSongName";
            this.textBoxSongName.Size = new System.Drawing.Size(159, 31);
            this.textBoxSongName.TabIndex = 12;
            this.textBoxSongName.TabStop = false;
            this.textBoxSongName.Text = "------------";
            // 
            // textBoxKey
            // 
            this.textBoxKey.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxKey.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxKey.Location = new System.Drawing.Point(516, 94);
            this.textBoxKey.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(66, 31);
            this.textBoxKey.TabIndex = 14;
            this.textBoxKey.TabStop = false;
            this.textBoxKey.Text = "---";
            // 
            // textBoxDegree
            // 
            this.textBoxDegree.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxDegree.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxDegree.Location = new System.Drawing.Point(353, 37);
            this.textBoxDegree.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxDegree.Name = "textBoxDegree";
            this.textBoxDegree.Size = new System.Drawing.Size(66, 31);
            this.textBoxDegree.TabIndex = 16;
            this.textBoxDegree.TabStop = false;
            this.textBoxDegree.Text = "---";
            // 
            // textBoxKeyMode
            // 
            this.textBoxKeyMode.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxKeyMode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxKeyMode.Location = new System.Drawing.Point(433, 94);
            this.textBoxKeyMode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxKeyMode.Name = "textBoxKeyMode";
            this.textBoxKeyMode.Size = new System.Drawing.Size(66, 31);
            this.textBoxKeyMode.TabIndex = 18;
            this.textBoxKeyMode.TabStop = false;
            this.textBoxKeyMode.Text = "---";
            // 
            // textBoxCode
            // 
            this.textBoxCode.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxCode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxCode.Location = new System.Drawing.Point(515, 36);
            this.textBoxCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.Size = new System.Drawing.Size(66, 31);
            this.textBoxCode.TabIndex = 22;
            this.textBoxCode.TabStop = false;
            this.textBoxCode.Text = "---";
            // 
            // textBoxMode
            // 
            this.textBoxMode.BackColor = System.Drawing.Color.PaleGreen;
            this.textBoxMode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxMode.Location = new System.Drawing.Point(434, 36);
            this.textBoxMode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxMode.Name = "textBoxMode";
            this.textBoxMode.Size = new System.Drawing.Size(66, 31);
            this.textBoxMode.TabIndex = 24;
            this.textBoxMode.TabStop = false;
            this.textBoxMode.Text = "---";
            // 
            // labelMeas
            // 
            this.labelMeas.AutoSize = true;
            this.labelMeas.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelMeas.ForeColor = System.Drawing.Color.White;
            this.labelMeas.Location = new System.Drawing.Point(96, 16);
            this.labelMeas.Name = "labelMeas";
            this.labelMeas.Size = new System.Drawing.Size(47, 17);
            this.labelMeas.TabIndex = 5;
            this.labelMeas.Text = "MEAS";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.Gold;
            this.labelTitle.Location = new System.Drawing.Point(594, 30);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(110, 34);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.Text = "MeowziQ";
            // 
            // labelSubTitle
            // 
            this.labelSubTitle.AutoSize = true;
            this.labelSubTitle.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubTitle.ForeColor = System.Drawing.Color.Gold;
            this.labelSubTitle.Location = new System.Drawing.Point(602, 15);
            this.labelSubTitle.Name = "labelSubTitle";
            this.labelSubTitle.Size = new System.Drawing.Size(92, 16);
            this.labelSubTitle.TabIndex = 7;
            this.labelSubTitle.Text = "music sequencer";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.ForeColor = System.Drawing.Color.Gold;
            this.labelVersion.Location = new System.Drawing.Point(645, 64);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(47, 16);
            this.labelVersion.TabIndex = 8;
            this.labelVersion.Text = "ver 1.0.0";
            // 
            // labelBeat
            // 
            this.labelBeat.AutoSize = true;
            this.labelBeat.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelBeat.ForeColor = System.Drawing.Color.White;
            this.labelBeat.Location = new System.Drawing.Point(14, 16);
            this.labelBeat.Name = "labelBeat";
            this.labelBeat.Size = new System.Drawing.Size(44, 17);
            this.labelBeat.TabIndex = 9;
            this.labelBeat.Text = "BEAT";
            // 
            // labelPlay
            // 
            this.labelPlay.AutoSize = true;
            this.labelPlay.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelPlay.ForeColor = System.Drawing.Color.DimGray;
            this.labelPlay.Location = new System.Drawing.Point(36, 113);
            this.labelPlay.Name = "labelPlay";
            this.labelPlay.Size = new System.Drawing.Size(29, 24);
            this.labelPlay.TabIndex = 10;
            this.labelPlay.Text = "●";
            // 
            // labelSongName
            // 
            this.labelSongName.AutoSize = true;
            this.labelSongName.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelSongName.ForeColor = System.Drawing.Color.White;
            this.labelSongName.Location = new System.Drawing.Point(176, 16);
            this.labelSongName.Name = "labelSongName";
            this.labelSongName.Size = new System.Drawing.Size(48, 17);
            this.labelSongName.TabIndex = 13;
            this.labelSongName.Text = "SONG";
            // 
            // labelKey
            // 
            this.labelKey.AutoSize = true;
            this.labelKey.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelKey.ForeColor = System.Drawing.Color.White;
            this.labelKey.Location = new System.Drawing.Point(514, 73);
            this.labelKey.Name = "labelKey";
            this.labelKey.Size = new System.Drawing.Size(35, 17);
            this.labelKey.TabIndex = 15;
            this.labelKey.Text = "KEY";
            // 
            // labelDegree
            // 
            this.labelDegree.AutoSize = true;
            this.labelDegree.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelDegree.ForeColor = System.Drawing.Color.White;
            this.labelDegree.Location = new System.Drawing.Point(350, 15);
            this.labelDegree.Name = "labelDegree";
            this.labelDegree.Size = new System.Drawing.Size(62, 17);
            this.labelDegree.TabIndex = 17;
            this.labelDegree.Text = "DEGREE";
            // 
            // labelKeyMode
            // 
            this.labelKeyMode.AutoSize = true;
            this.labelKeyMode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelKeyMode.ForeColor = System.Drawing.Color.White;
            this.labelKeyMode.Location = new System.Drawing.Point(430, 75);
            this.labelKeyMode.Name = "labelKeyMode";
            this.labelKeyMode.Size = new System.Drawing.Size(74, 15);
            this.labelKeyMode.TabIndex = 19;
            this.labelKeyMode.Text = "KEY MODE";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCode.ForeColor = System.Drawing.Color.White;
            this.labelCode.Location = new System.Drawing.Point(513, 16);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(45, 17);
            this.labelCode.TabIndex = 23;
            this.labelCode.Text = "CODE";
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelMode.ForeColor = System.Drawing.Color.White;
            this.labelMode.Location = new System.Drawing.Point(430, 17);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(45, 15);
            this.labelMode.TabIndex = 25;
            this.labelMode.Text = "MODE";
            // 
            // labelModulation
            // 
            this.labelModulation.AutoSize = true;
            this.labelModulation.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelModulation.ForeColor = System.Drawing.Color.DimGray;
            this.labelModulation.Location = new System.Drawing.Point(429, 126);
            this.labelModulation.Name = "labelModulation";
            this.labelModulation.Size = new System.Drawing.Size(29, 24);
            this.labelModulation.TabIndex = 27;
            this.labelModulation.Text = "●";
            // 
            // labelModulationText
            // 
            this.labelModulationText.AutoSize = true;
            this.labelModulationText.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelModulationText.ForeColor = System.Drawing.Color.White;
            this.labelModulationText.Location = new System.Drawing.Point(454, 131);
            this.labelModulationText.Name = "labelModulationText";
            this.labelModulationText.Size = new System.Drawing.Size(95, 15);
            this.labelModulationText.TabIndex = 28;
            this.labelModulationText.Text = "MODULATION";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(709, 290);
            this.Controls.Add(this.labelModulationText);
            this.Controls.Add(this.labelModulation);
            this.Controls.Add(this.labelMode);
            this.Controls.Add(this.labelCode);
            this.Controls.Add(this.labelKeyMode);
            this.Controls.Add(this.labelDegree);
            this.Controls.Add(this.labelKey);
            this.Controls.Add(this.labelSongName);
            this.Controls.Add(this.labelPlay);
            this.Controls.Add(this.labelBeat);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelSubTitle);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelMeas);
            this.Controls.Add(this.textBoxMode);
            this.Controls.Add(this.textBoxCode);
            this.Controls.Add(this.textBoxKeyMode);
            this.Controls.Add(this.textBoxDegree);
            this.Controls.Add(this.textBoxKey);
            this.Controls.Add(this.textBoxSongName);
            this.Controls.Add(this.textBoxMeas);
            this.Controls.Add(this.textBoxBeat);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.pianoControl);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormMain";
            this.Text = "© STUDIO MeowToon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Sanford.Multimedia.Midi.Sequence sequence;
        private Sanford.Multimedia.Midi.Sequencer sequencer;
        private Sanford.Multimedia.Midi.UI.PianoControl pianoControl;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.TextBox textBoxBeat;
        private System.Windows.Forms.TextBox textBoxMeas;
        private System.Windows.Forms.TextBox textBoxSongName;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.TextBox textBoxDegree;
        private System.Windows.Forms.TextBox textBoxKeyMode;
        private System.Windows.Forms.TextBox textBoxCode;
        private System.Windows.Forms.TextBox textBoxMode;
        private System.Windows.Forms.Label labelMeas;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelSubTitle;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelBeat;
        private System.Windows.Forms.Label labelPlay;
        private System.Windows.Forms.Label labelSongName;
        private System.Windows.Forms.Label labelKey;
        private System.Windows.Forms.Label labelDegree;
        private System.Windows.Forms.Label labelKeyMode;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Label labelModulation;
        private System.Windows.Forms.Label labelModulationText;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

