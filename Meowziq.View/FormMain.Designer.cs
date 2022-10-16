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
            this._piano_control = new Sanford.Multimedia.Midi.UI.PianoControl();
            this._button_play = new System.Windows.Forms.Button();
            this._button_stop = new System.Windows.Forms.Button();
            this._button_load = new System.Windows.Forms.Button();
            this._button_convert = new System.Windows.Forms.Button();
            this._textbox_meas = new System.Windows.Forms.TextBox();
            this._textbox_beat = new System.Windows.Forms.TextBox();
            this._textbox_song_name = new System.Windows.Forms.TextBox();
            this._textbox_key = new System.Windows.Forms.TextBox();
            this._textbox_degree = new System.Windows.Forms.TextBox();
            this._textbox_key_mode = new System.Windows.Forms.TextBox();
            this._textbox_code = new System.Windows.Forms.TextBox();
            this._textbox_Mode = new System.Windows.Forms.TextBox();
            this._label_meas = new System.Windows.Forms.Label();
            this._label_title = new System.Windows.Forms.Label();
            this._label_sub_title = new System.Windows.Forms.Label();
            this._label_version = new System.Windows.Forms.Label();
            this._label_beat = new System.Windows.Forms.Label();
            this._label_play = new System.Windows.Forms.Label();
            this._label_song_name = new System.Windows.Forms.Label();
            this._label_key = new System.Windows.Forms.Label();
            this._label_degree = new System.Windows.Forms.Label();
            this._label_key_mode = new System.Windows.Forms.Label();
            this._label_code = new System.Windows.Forms.Label();
            this._label_mode = new System.Windows.Forms.Label();
            this._label_modulation = new System.Windows.Forms.Label();
            this._label_modulation_text = new System.Windows.Forms.Label();
            this._label_repeat_begin = new System.Windows.Forms.Label();
            this._label_repeat_end = new System.Windows.Forms.Label();
            this._numericupdown_repeat_begin = new System.Windows.Forms.NumericUpDown();
            this._numericupdown_repeat_end = new System.Windows.Forms.NumericUpDown();
            this._folderbrowserdialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this._numericupdown_repeat_begin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numericupdown_repeat_end)).BeginInit();
            this.SuspendLayout();
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
            // _piano_control
            // 
            this._piano_control.HighNoteID = 109;
            this._piano_control.Location = new System.Drawing.Point(15, 322);
            this._piano_control.LowNoteID = 21;
            this._piano_control.Name = "_piano_control";
            this._piano_control.NoteOnColor = System.Drawing.Color.SkyBlue;
            this._piano_control.Size = new System.Drawing.Size(915, 140);
            this._piano_control.TabIndex = 4;
            this._piano_control.TabStop = false;
            this._piano_control.Text = "pianoControl";
            // 
            // _button_play
            // 
            this._button_play.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._button_play.Location = new System.Drawing.Point(23, 132);
            this._button_play.Name = "_button_play";
            this._button_play.Size = new System.Drawing.Size(88, 53);
            this._button_play.TabIndex = 1;
            this._button_play.Text = "PLAY";
            this._button_play.UseVisualStyleBackColor = true;
            this._button_play.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // _button_stop
            // 
            this._button_stop.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._button_stop.Location = new System.Drawing.Point(129, 132);
            this._button_stop.Name = "_button_stop";
            this._button_stop.Size = new System.Drawing.Size(88, 53);
            this._button_stop.TabIndex = 2;
            this._button_stop.Text = "STOP";
            this._button_stop.UseVisualStyleBackColor = true;
            this._button_stop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // _button_load
            // 
            this._button_load.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._button_load.Location = new System.Drawing.Point(237, 132);
            this._button_load.Name = "_button_load";
            this._button_load.Size = new System.Drawing.Size(88, 53);
            this._button_load.TabIndex = 3;
            this._button_load.Text = "LOAD";
            this._button_load.UseVisualStyleBackColor = true;
            this._button_load.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // _button_convert
            // 
            this._button_convert.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._button_convert.Location = new System.Drawing.Point(343, 132);
            this._button_convert.Name = "_button_convert";
            this._button_convert.Size = new System.Drawing.Size(107, 53);
            this._button_convert.TabIndex = 26;
            this._button_convert.Text = "to SMF";
            this._button_convert.UseVisualStyleBackColor = true;
            this._button_convert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // _textbox_meas
            // 
            this._textbox_meas.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_meas.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_meas.Location = new System.Drawing.Point(129, 60);
            this._textbox_meas.Name = "_textbox_meas";
            this._textbox_meas.Size = new System.Drawing.Size(87, 37);
            this._textbox_meas.TabIndex = 4;
            this._textbox_meas.TabStop = false;
            this._textbox_meas.Text = "0";
            this._textbox_meas.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textbox_beat
            // 
            this._textbox_beat.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_beat.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_beat.Location = new System.Drawing.Point(21, 62);
            this._textbox_beat.Name = "_textbox_beat";
            this._textbox_beat.Size = new System.Drawing.Size(87, 37);
            this._textbox_beat.TabIndex = 0;
            this._textbox_beat.TabStop = false;
            this._textbox_beat.Text = "0";
            this._textbox_beat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textbox_song_name
            // 
            this._textbox_song_name.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_song_name.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_song_name.Location = new System.Drawing.Point(237, 60);
            this._textbox_song_name.Name = "_textbox_song_name";
            this._textbox_song_name.Size = new System.Drawing.Size(211, 37);
            this._textbox_song_name.TabIndex = 12;
            this._textbox_song_name.TabStop = false;
            this._textbox_song_name.Text = "------------";
            // 
            // _textbox_key
            // 
            this._textbox_key.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_key.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_key.Location = new System.Drawing.Point(688, 157);
            this._textbox_key.Name = "_textbox_key";
            this._textbox_key.Size = new System.Drawing.Size(87, 37);
            this._textbox_key.TabIndex = 14;
            this._textbox_key.TabStop = false;
            this._textbox_key.Text = "---";
            // 
            // _textbox_degree
            // 
            this._textbox_degree.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_degree.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_degree.Location = new System.Drawing.Point(471, 62);
            this._textbox_degree.Name = "_textbox_degree";
            this._textbox_degree.Size = new System.Drawing.Size(87, 37);
            this._textbox_degree.TabIndex = 16;
            this._textbox_degree.TabStop = false;
            this._textbox_degree.Text = "---";
            // 
            // _textbox_key_mode
            // 
            this._textbox_key_mode.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_key_mode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_key_mode.Location = new System.Drawing.Point(577, 157);
            this._textbox_key_mode.Name = "_textbox_key_mode";
            this._textbox_key_mode.Size = new System.Drawing.Size(87, 37);
            this._textbox_key_mode.TabIndex = 18;
            this._textbox_key_mode.TabStop = false;
            this._textbox_key_mode.Text = "---";
            // 
            // _textbox_code
            // 
            this._textbox_code.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_code.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_code.Location = new System.Drawing.Point(687, 60);
            this._textbox_code.Name = "_textbox_code";
            this._textbox_code.Size = new System.Drawing.Size(87, 37);
            this._textbox_code.TabIndex = 22;
            this._textbox_code.TabStop = false;
            this._textbox_code.Text = "---";
            // 
            // _textbox_Mode
            // 
            this._textbox_Mode.BackColor = System.Drawing.Color.PaleGreen;
            this._textbox_Mode.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._textbox_Mode.Location = new System.Drawing.Point(579, 60);
            this._textbox_Mode.Name = "_textbox_Mode";
            this._textbox_Mode.Size = new System.Drawing.Size(87, 37);
            this._textbox_Mode.TabIndex = 24;
            this._textbox_Mode.TabStop = false;
            this._textbox_Mode.Text = "---";
            // 
            // _label_meas
            // 
            this._label_meas.AutoSize = true;
            this._label_meas.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_meas.ForeColor = System.Drawing.Color.White;
            this._label_meas.Location = new System.Drawing.Point(128, 27);
            this._label_meas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_meas.Name = "_label_meas";
            this._label_meas.Size = new System.Drawing.Size(60, 22);
            this._label_meas.TabIndex = 5;
            this._label_meas.Text = "MEAS";
            // 
            // _label_title
            // 
            this._label_title.AutoSize = true;
            this._label_title.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._label_title.ForeColor = System.Drawing.Color.Gold;
            this._label_title.Location = new System.Drawing.Point(792, 50);
            this._label_title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_title.Name = "_label_title";
            this._label_title.Size = new System.Drawing.Size(138, 42);
            this._label_title.TabIndex = 6;
            this._label_title.Text = "MeowziQ";
            // 
            // _label_sub_title
            // 
            this._label_sub_title.AutoSize = true;
            this._label_sub_title.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._label_sub_title.ForeColor = System.Drawing.Color.Gold;
            this._label_sub_title.Location = new System.Drawing.Point(803, 25);
            this._label_sub_title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_sub_title.Name = "_label_sub_title";
            this._label_sub_title.Size = new System.Drawing.Size(117, 19);
            this._label_sub_title.TabIndex = 7;
            this._label_sub_title.Text = "music sequencer";
            // 
            // _label_version
            // 
            this._label_version.AutoSize = true;
            this._label_version.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._label_version.ForeColor = System.Drawing.Color.Gold;
            this._label_version.Location = new System.Drawing.Point(860, 107);
            this._label_version.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_version.Name = "_label_version";
            this._label_version.Size = new System.Drawing.Size(60, 19);
            this._label_version.TabIndex = 8;
            this._label_version.Text = "ver 1.0.0";
            // 
            // _label_beat
            // 
            this._label_beat.AutoSize = true;
            this._label_beat.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_beat.ForeColor = System.Drawing.Color.White;
            this._label_beat.Location = new System.Drawing.Point(19, 27);
            this._label_beat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_beat.Name = "_label_beat";
            this._label_beat.Size = new System.Drawing.Size(56, 22);
            this._label_beat.TabIndex = 9;
            this._label_beat.Text = "BEAT";
            // 
            // _label_play
            // 
            this._label_play.AutoSize = true;
            this._label_play.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._label_play.ForeColor = System.Drawing.Color.DimGray;
            this._label_play.Location = new System.Drawing.Point(48, 188);
            this._label_play.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_play.Name = "_label_play";
            this._label_play.Size = new System.Drawing.Size(37, 30);
            this._label_play.TabIndex = 10;
            this._label_play.Text = "●";
            // 
            // _label_song_name
            // 
            this._label_song_name.AutoSize = true;
            this._label_song_name.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_song_name.ForeColor = System.Drawing.Color.White;
            this._label_song_name.Location = new System.Drawing.Point(235, 27);
            this._label_song_name.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_song_name.Name = "_label_song_name";
            this._label_song_name.Size = new System.Drawing.Size(62, 22);
            this._label_song_name.TabIndex = 13;
            this._label_song_name.Text = "SONG";
            // 
            // _label_key
            // 
            this._label_key.AutoSize = true;
            this._label_key.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_key.ForeColor = System.Drawing.Color.White;
            this._label_key.Location = new System.Drawing.Point(685, 122);
            this._label_key.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_key.Name = "_label_key";
            this._label_key.Size = new System.Drawing.Size(46, 22);
            this._label_key.TabIndex = 15;
            this._label_key.Text = "KEY";
            // 
            // _label_degree
            // 
            this._label_degree.AutoSize = true;
            this._label_degree.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_degree.ForeColor = System.Drawing.Color.White;
            this._label_degree.Location = new System.Drawing.Point(467, 25);
            this._label_degree.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_degree.Name = "_label_degree";
            this._label_degree.Size = new System.Drawing.Size(82, 22);
            this._label_degree.TabIndex = 17;
            this._label_degree.Text = "DEGREE";
            // 
            // _label_key_mode
            // 
            this._label_key_mode.AutoSize = true;
            this._label_key_mode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_key_mode.ForeColor = System.Drawing.Color.White;
            this._label_key_mode.Location = new System.Drawing.Point(573, 125);
            this._label_key_mode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_key_mode.Name = "_label_key_mode";
            this._label_key_mode.Size = new System.Drawing.Size(94, 19);
            this._label_key_mode.TabIndex = 19;
            this._label_key_mode.Text = "KEY MODE";
            // 
            // _label_code
            // 
            this._label_code.AutoSize = true;
            this._label_code.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_code.ForeColor = System.Drawing.Color.White;
            this._label_code.Location = new System.Drawing.Point(684, 27);
            this._label_code.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_code.Name = "_label_code";
            this._label_code.Size = new System.Drawing.Size(60, 22);
            this._label_code.TabIndex = 23;
            this._label_code.Text = "CODE";
            // 
            // _label_mode
            // 
            this._label_mode.AutoSize = true;
            this._label_mode.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_mode.ForeColor = System.Drawing.Color.White;
            this._label_mode.Location = new System.Drawing.Point(573, 28);
            this._label_mode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_mode.Name = "_label_mode";
            this._label_mode.Size = new System.Drawing.Size(57, 19);
            this._label_mode.TabIndex = 25;
            this._label_mode.Text = "MODE";
            // 
            // _label_modulation
            // 
            this._label_modulation.AutoSize = true;
            this._label_modulation.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._label_modulation.ForeColor = System.Drawing.Color.DimGray;
            this._label_modulation.Location = new System.Drawing.Point(572, 210);
            this._label_modulation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_modulation.Name = "_label_modulation";
            this._label_modulation.Size = new System.Drawing.Size(37, 30);
            this._label_modulation.TabIndex = 27;
            this._label_modulation.Text = "●";
            // 
            // _label_modulation_text
            // 
            this._label_modulation_text.AutoSize = true;
            this._label_modulation_text.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_modulation_text.ForeColor = System.Drawing.Color.White;
            this._label_modulation_text.Location = new System.Drawing.Point(605, 218);
            this._label_modulation_text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_modulation_text.Name = "_label_modulation_text";
            this._label_modulation_text.Size = new System.Drawing.Size(120, 19);
            this._label_modulation_text.TabIndex = 28;
            this._label_modulation_text.Text = "MODULATION";
            // 
            // _label_repeat_begin
            // 
            this._label_repeat_begin.AutoSize = true;
            this._label_repeat_begin.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_repeat_begin.ForeColor = System.Drawing.Color.White;
            this._label_repeat_begin.Location = new System.Drawing.Point(27, 227);
            this._label_repeat_begin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_repeat_begin.Name = "_label_repeat_begin";
            this._label_repeat_begin.Size = new System.Drawing.Size(88, 19);
            this._label_repeat_begin.TabIndex = 29;
            this._label_repeat_begin.Text = "REPE BEG";
            // 
            // _label_repeat_end
            // 
            this._label_repeat_end.AutoSize = true;
            this._label_repeat_end.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._label_repeat_end.ForeColor = System.Drawing.Color.White;
            this._label_repeat_end.Location = new System.Drawing.Point(27, 271);
            this._label_repeat_end.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label_repeat_end.Name = "_label_repeat_end";
            this._label_repeat_end.Size = new System.Drawing.Size(89, 19);
            this._label_repeat_end.TabIndex = 30;
            this._label_repeat_end.Text = "REPE END";
            // 
            // _numericupdown_repeat_begin
            // 
            this._numericupdown_repeat_begin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._numericupdown_repeat_begin.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._numericupdown_repeat_begin.Location = new System.Drawing.Point(130, 220);
            this._numericupdown_repeat_begin.Name = "_numericupdown_repeat_begin";
            this._numericupdown_repeat_begin.Size = new System.Drawing.Size(87, 33);
            this._numericupdown_repeat_begin.TabIndex = 31;
            this._numericupdown_repeat_begin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _numericupdown_repeat_end
            // 
            this._numericupdown_repeat_end.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._numericupdown_repeat_end.Font = new System.Drawing.Font("Meiryo UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._numericupdown_repeat_end.Location = new System.Drawing.Point(130, 264);
            this._numericupdown_repeat_end.Name = "_numericupdown_repeat_end";
            this._numericupdown_repeat_end.Size = new System.Drawing.Size(87, 33);
            this._numericupdown_repeat_end.TabIndex = 32;
            this._numericupdown_repeat_end.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(945, 483);
            this.Controls.Add(this._numericupdown_repeat_end);
            this.Controls.Add(this._label_repeat_end);
            this.Controls.Add(this._label_repeat_begin);
            this.Controls.Add(this._numericupdown_repeat_begin);
            this.Controls.Add(this._piano_control);
            this.Controls.Add(this._label_modulation_text);
            this.Controls.Add(this._label_modulation);
            this.Controls.Add(this._label_mode);
            this.Controls.Add(this._label_code);
            this.Controls.Add(this._label_key_mode);
            this.Controls.Add(this._label_degree);
            this.Controls.Add(this._label_key);
            this.Controls.Add(this._label_song_name);
            this.Controls.Add(this._label_play);
            this.Controls.Add(this._label_beat);
            this.Controls.Add(this._label_version);
            this.Controls.Add(this._label_sub_title);
            this.Controls.Add(this._label_title);
            this.Controls.Add(this._label_meas);
            this.Controls.Add(this._textbox_Mode);
            this.Controls.Add(this._textbox_code);
            this.Controls.Add(this._textbox_key_mode);
            this.Controls.Add(this._textbox_degree);
            this.Controls.Add(this._textbox_key);
            this.Controls.Add(this._textbox_song_name);
            this.Controls.Add(this._textbox_meas);
            this.Controls.Add(this._textbox_beat);
            this.Controls.Add(this._button_convert);
            this.Controls.Add(this._button_load);
            this.Controls.Add(this._button_stop);
            this.Controls.Add(this._button_play);
            this.Name = "FormMain";
            this.Text = "© STUDIO MeowToon";
            ((System.ComponentModel.ISupportInitialize)(this._numericupdown_repeat_begin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numericupdown_repeat_end)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sanford.Multimedia.Midi.Sequence _sequence;
        private Sanford.Multimedia.Midi.Sequencer _sequencer;
        private Sanford.Multimedia.Midi.UI.PianoControl _piano_control;
        private System.Windows.Forms.Button _button_play;
        private System.Windows.Forms.Button _button_stop;
        private System.Windows.Forms.Button _button_load;
        private System.Windows.Forms.Button _button_convert;
        private System.Windows.Forms.TextBox _textbox_beat;
        private System.Windows.Forms.TextBox _textbox_meas;
        private System.Windows.Forms.TextBox _textbox_song_name;
        private System.Windows.Forms.TextBox _textbox_key;
        private System.Windows.Forms.TextBox _textbox_degree;
        private System.Windows.Forms.TextBox _textbox_key_mode;
        private System.Windows.Forms.TextBox _textbox_code;
        private System.Windows.Forms.TextBox _textbox_Mode;
        private System.Windows.Forms.Label _label_meas;
        private System.Windows.Forms.Label _label_title;
        private System.Windows.Forms.Label _label_sub_title;
        private System.Windows.Forms.Label _label_version;
        private System.Windows.Forms.Label _label_beat;
        private System.Windows.Forms.Label _label_play;
        private System.Windows.Forms.Label _label_song_name;
        private System.Windows.Forms.Label _label_key;
        private System.Windows.Forms.Label _label_degree;
        private System.Windows.Forms.Label _label_key_mode;
        private System.Windows.Forms.Label _label_code;
        private System.Windows.Forms.Label _label_mode;
        private System.Windows.Forms.Label _label_modulation;
        private System.Windows.Forms.Label _label_modulation_text;
        private System.Windows.Forms.Label _label_repeat_begin;
        private System.Windows.Forms.Label _label_repeat_end;
        private System.Windows.Forms.NumericUpDown _numericupdown_repeat_begin;
        private System.Windows.Forms.NumericUpDown _numericupdown_repeat_end;
        private System.Windows.Forms.FolderBrowserDialog _folderbrowserdialog;
    }
}
