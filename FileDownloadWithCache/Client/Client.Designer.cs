namespace Client
{
    partial class Client
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button2 = new Button();
            listView1 = new ListView();
            button1 = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            pictureBox1 = new PictureBox();
            button3 = new Button();
            button4 = new Button();
            pb1 = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button2
            // 
            button2.Location = new Point(12, 1047);
            button2.Name = "button2";
            button2.Size = new Size(286, 46);
            button2.TabIndex = 1;
            button2.Text = "Download selected file";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // listView1
            // 
            listView1.FullRowSelect = true;
            listView1.Location = new Point(12, 102);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new Size(380, 898);
            listView1.TabIndex = 3;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.List;
            listView1.SelectedIndexChanged += getSelected;
            // 
            // button1
            // 
            button1.Location = new Point(12, 40);
            button1.Name = "button1";
            button1.Size = new Size(380, 46);
            button1.TabIndex = 4;
            button1.Text = "Load or reload available files";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 1131);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(380, 118);
            textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(433, 40);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(240, 39);
            textBox2.TabIndex = 7;
            textBox2.Text = "Downloaded images";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(433, 102);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1303, 1053);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // button3
            // 
            button3.Location = new Point(433, 1203);
            button3.Name = "button3";
            button3.Size = new Size(150, 46);
            button3.TabIndex = 9;
            button3.Text = "Previous";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            button3.Click += button3_Click_1;
            // 
            // button4
            // 
            button4.Location = new Point(1586, 1203);
            button4.Name = "button4";
            button4.Size = new Size(150, 46);
            button4.TabIndex = 10;
            button4.Text = "Next";
            button4.UseVisualStyleBackColor = true;
            button4.Visible = false;
            button4.Click += button4_Click;
            // 
            // pb1
            // 
            pb1.Location = new Point(433, 1172);
            pb1.Name = "pb1";
            pb1.Size = new Size(1303, 25);
            pb1.TabIndex = 11;
            pb1.Visible = false;
            // 
            // Client
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1801, 1300);
            Controls.Add(pb1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(pictureBox1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(listView1);
            Controls.Add(button2);
            Name = "Client";
            Text = "Client";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button2;
        private ListView listView1;
        private Button button1;
        private TextBox textBox1;
        private TextBox textBox2;
        private PictureBox pictureBox1;
        private Button button3;
        private Button button4;
        private ProgressBar pb1;
    }
}