namespace MusicPlayer
{
    partial class form1
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
            this.MusicListBox = new System.Windows.Forms.TreeView();
            this.BeforeButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.PlayingMusicTextBox = new System.Windows.Forms.Label();
            this.playingForderTextBox = new System.Windows.Forms.Label();
            this.NextButton = new System.Windows.Forms.Button();
            this.PlayingMusicText = new System.Windows.Forms.Label();
            this.playingForderText = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.AllMusicSpdBox = new System.Windows.Forms.TextBox();
            this.AllMusicSoundBox = new System.Windows.Forms.TextBox();
            this.AllMusicSpd = new System.Windows.Forms.Label();
            this.AllMusicSound = new System.Windows.Forms.Label();
            this.TargetMusicSound = new System.Windows.Forms.Label();
            this.TargetMusicSpd = new System.Windows.Forms.Label();
            this.TargetMusicSoundBox = new System.Windows.Forms.TextBox();
            this.TargetMusicSpdBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.AllMusicLabel = new System.Windows.Forms.Label();
            this.TargetMusicLabel = new System.Windows.Forms.Label();
            this.RepeateModeBox = new System.Windows.Forms.ComboBox();
            this.NextModeBox = new System.Windows.Forms.ComboBox();
            this.PlaySelectedButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // MusicListBox
            // 
            this.MusicListBox.Location = new System.Drawing.Point(12, 12);
            this.MusicListBox.Name = "MusicListBox";
            this.MusicListBox.Size = new System.Drawing.Size(255, 426);
            this.MusicListBox.TabIndex = 0;
            this.MusicListBox.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.MusicListBox_NodeMouseClick);
            // 
            // BeforeButton
            // 
            this.BeforeButton.Location = new System.Drawing.Point(372, 290);
            this.BeforeButton.Name = "BeforeButton";
            this.BeforeButton.Size = new System.Drawing.Size(75, 23);
            this.BeforeButton.TabIndex = 1;
            this.BeforeButton.Text = "before";
            this.BeforeButton.UseVisualStyleBackColor = true;
            this.BeforeButton.Click += new System.EventHandler(this.BeforeButton_Click);
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(453, 290);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(75, 23);
            this.PlayButton.TabIndex = 2;
            this.PlayButton.Text = "resume";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(534, 290);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(75, 23);
            this.StopButton.TabIndex = 3;
            this.StopButton.Text = "stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // PlayingMusicTextBox
            // 
            this.PlayingMusicTextBox.AutoSize = true;
            this.PlayingMusicTextBox.Location = new System.Drawing.Point(370, 163);
            this.PlayingMusicTextBox.Name = "PlayingMusicTextBox";
            this.PlayingMusicTextBox.Size = new System.Drawing.Size(64, 12);
            this.PlayingMusicTextBox.TabIndex = 4;
            this.PlayingMusicTextBox.Text = "nowMusic";
            this.PlayingMusicTextBox.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // playingForderTextBox
            // 
            this.playingForderTextBox.AutoSize = true;
            this.playingForderTextBox.Location = new System.Drawing.Point(366, 264);
            this.playingForderTextBox.Name = "playingForderTextBox";
            this.playingForderTextBox.Size = new System.Drawing.Size(37, 12);
            this.playingForderTextBox.TabIndex = 5;
            this.playingForderTextBox.Text = "forder";
            this.playingForderTextBox.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(615, 290);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.TabIndex = 7;
            this.NextButton.Text = "next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // PlayingMusicText
            // 
            this.PlayingMusicText.AutoSize = true;
            this.PlayingMusicText.Location = new System.Drawing.Point(451, 163);
            this.PlayingMusicText.Name = "PlayingMusicText";
            this.PlayingMusicText.Size = new System.Drawing.Size(38, 12);
            this.PlayingMusicText.TabIndex = 8;
            this.PlayingMusicText.Text = "label3";
            // 
            // playingForderText
            // 
            this.playingForderText.AutoSize = true;
            this.playingForderText.Location = new System.Drawing.Point(453, 264);
            this.playingForderText.Name = "playingForderText";
            this.playingForderText.Size = new System.Drawing.Size(38, 12);
            this.playingForderText.TabIndex = 9;
            this.playingForderText.Text = "label4";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(347, 192);
            this.trackBar1.Maximum = 100000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(213, 45);
            this.trackBar1.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(464, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "apply ";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // AllMusicSpdBox
            // 
            this.AllMusicSpdBox.Location = new System.Drawing.Point(347, 64);
            this.AllMusicSpdBox.Name = "AllMusicSpdBox";
            this.AllMusicSpdBox.Size = new System.Drawing.Size(100, 21);
            this.AllMusicSpdBox.TabIndex = 12;
            // 
            // AllMusicSoundBox
            // 
            this.AllMusicSoundBox.Location = new System.Drawing.Point(347, 107);
            this.AllMusicSoundBox.Name = "AllMusicSoundBox";
            this.AllMusicSoundBox.Size = new System.Drawing.Size(100, 21);
            this.AllMusicSoundBox.TabIndex = 13;
            // 
            // AllMusicSpd
            // 
            this.AllMusicSpd.AutoSize = true;
            this.AllMusicSpd.Location = new System.Drawing.Point(289, 67);
            this.AllMusicSpd.Name = "AllMusicSpd";
            this.AllMusicSpd.Size = new System.Drawing.Size(27, 12);
            this.AllMusicSpd.TabIndex = 14;
            this.AllMusicSpd.Text = "Spd";
            // 
            // AllMusicSound
            // 
            this.AllMusicSound.AutoSize = true;
            this.AllMusicSound.Location = new System.Drawing.Point(289, 118);
            this.AllMusicSound.Name = "AllMusicSound";
            this.AllMusicSound.Size = new System.Drawing.Size(41, 12);
            this.AllMusicSound.TabIndex = 15;
            this.AllMusicSound.Text = "Sound";
            // 
            // TargetMusicSound
            // 
            this.TargetMusicSound.AutoSize = true;
            this.TargetMusicSound.Location = new System.Drawing.Point(543, 118);
            this.TargetMusicSound.Name = "TargetMusicSound";
            this.TargetMusicSound.Size = new System.Drawing.Size(41, 12);
            this.TargetMusicSound.TabIndex = 20;
            this.TargetMusicSound.Text = "Sound";
            // 
            // TargetMusicSpd
            // 
            this.TargetMusicSpd.AutoSize = true;
            this.TargetMusicSpd.Location = new System.Drawing.Point(543, 67);
            this.TargetMusicSpd.Name = "TargetMusicSpd";
            this.TargetMusicSpd.Size = new System.Drawing.Size(27, 12);
            this.TargetMusicSpd.TabIndex = 19;
            this.TargetMusicSpd.Text = "Spd";
            // 
            // TargetMusicSoundBox
            // 
            this.TargetMusicSoundBox.Location = new System.Drawing.Point(601, 107);
            this.TargetMusicSoundBox.Name = "TargetMusicSoundBox";
            this.TargetMusicSoundBox.Size = new System.Drawing.Size(100, 21);
            this.TargetMusicSoundBox.TabIndex = 18;
            // 
            // TargetMusicSpdBox
            // 
            this.TargetMusicSpdBox.Location = new System.Drawing.Point(601, 64);
            this.TargetMusicSpdBox.Name = "TargetMusicSpdBox";
            this.TargetMusicSpdBox.Size = new System.Drawing.Size(100, 21);
            this.TargetMusicSpdBox.TabIndex = 17;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(718, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "apply ";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // AllMusicLabel
            // 
            this.AllMusicLabel.AutoSize = true;
            this.AllMusicLabel.Location = new System.Drawing.Point(345, 28);
            this.AllMusicLabel.Name = "AllMusicLabel";
            this.AllMusicLabel.Size = new System.Drawing.Size(58, 12);
            this.AllMusicLabel.TabIndex = 21;
            this.AllMusicLabel.Text = "All Music";
            // 
            // TargetMusicLabel
            // 
            this.TargetMusicLabel.AutoSize = true;
            this.TargetMusicLabel.Location = new System.Drawing.Point(564, 28);
            this.TargetMusicLabel.Name = "TargetMusicLabel";
            this.TargetMusicLabel.Size = new System.Drawing.Size(76, 12);
            this.TargetMusicLabel.TabIndex = 22;
            this.TargetMusicLabel.Text = "TargetMusic";
            // 
            // RepeateModeBox
            // 
            this.RepeateModeBox.FormattingEnabled = true;
            this.RepeateModeBox.Location = new System.Drawing.Point(601, 202);
            this.RepeateModeBox.Name = "RepeateModeBox";
            this.RepeateModeBox.Size = new System.Drawing.Size(67, 20);
            this.RepeateModeBox.TabIndex = 25;
            this.RepeateModeBox.SelectedIndexChanged += new System.EventHandler(this.RepeateModeBox_SelectedIndexChanged);
            // 
            // NextModeBox
            // 
            this.NextModeBox.FormattingEnabled = true;
            this.NextModeBox.Location = new System.Drawing.Point(684, 202);
            this.NextModeBox.Name = "NextModeBox";
            this.NextModeBox.Size = new System.Drawing.Size(67, 20);
            this.NextModeBox.TabIndex = 26;
            this.NextModeBox.SelectedIndexChanged += new System.EventHandler(this.NextModeBox_SelectedIndexChanged);
            // 
            // PlaySelectedButton
            // 
            this.PlaySelectedButton.Location = new System.Drawing.Point(453, 319);
            this.PlaySelectedButton.Name = "PlaySelectedButton";
            this.PlaySelectedButton.Size = new System.Drawing.Size(75, 23);
            this.PlaySelectedButton.TabIndex = 27;
            this.PlaySelectedButton.Text = "PlaySelect";
            this.PlaySelectedButton.UseVisualStyleBackColor = true;
            this.PlaySelectedButton.Click += new System.EventHandler(this.PlaySelectedButton_Click);
            // 
            // form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.PlaySelectedButton);
            this.Controls.Add(this.NextModeBox);
            this.Controls.Add(this.RepeateModeBox);
            this.Controls.Add(this.TargetMusicLabel);
            this.Controls.Add(this.AllMusicLabel);
            this.Controls.Add(this.TargetMusicSound);
            this.Controls.Add(this.TargetMusicSpd);
            this.Controls.Add(this.TargetMusicSoundBox);
            this.Controls.Add(this.TargetMusicSpdBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.AllMusicSound);
            this.Controls.Add(this.AllMusicSpd);
            this.Controls.Add(this.AllMusicSoundBox);
            this.Controls.Add(this.AllMusicSpdBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.playingForderText);
            this.Controls.Add(this.PlayingMusicText);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.playingForderTextBox);
            this.Controls.Add(this.PlayingMusicTextBox);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.BeforeButton);
            this.Controls.Add(this.MusicListBox);
            this.Name = "form1";
            this.Text = "PlaySelect";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView MusicListBox;
        private System.Windows.Forms.Button BeforeButton;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Label PlayingMusicTextBox;
        private System.Windows.Forms.Label playingForderTextBox;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Label PlayingMusicText;
        private System.Windows.Forms.Label playingForderText;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox AllMusicSpdBox;
        private System.Windows.Forms.TextBox AllMusicSoundBox;
        private System.Windows.Forms.Label AllMusicSpd;
        private System.Windows.Forms.Label AllMusicSound;
        private System.Windows.Forms.Label TargetMusicSound;
        private System.Windows.Forms.Label TargetMusicSpd;
        private System.Windows.Forms.TextBox TargetMusicSoundBox;
        private System.Windows.Forms.TextBox TargetMusicSpdBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label AllMusicLabel;
        private System.Windows.Forms.Label TargetMusicLabel;
        private System.Windows.Forms.ComboBox RepeateModeBox;
        private System.Windows.Forms.ComboBox NextModeBox;
        private System.Windows.Forms.Button PlaySelectedButton;
    }
}

