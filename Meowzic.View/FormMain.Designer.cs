namespace Meowzic.View {
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
            this.textBoxBeat = new System.Windows.Forms.TextBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
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
            this.sequencer.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.handleChannelMessagePlayed);
            // 
            // textBoxBeat
            // 
            this.textBoxBeat.Location = new System.Drawing.Point(12, 71);
            this.textBoxBeat.Name = "textBoxBeat";
            this.textBoxBeat.Size = new System.Drawing.Size(100, 22);
            this.textBoxBeat.TabIndex = 0;
            this.textBoxBeat.Text = "0";
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(12, 23);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(100, 29);
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Text = "play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(136, 23);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(100, 29);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 120);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.textBoxBeat);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sanford.Multimedia.Midi.Sequence sequence;
        private Sanford.Multimedia.Midi.Sequencer sequencer;
        private System.Windows.Forms.TextBox textBoxBeat;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonStop;
    }
}

