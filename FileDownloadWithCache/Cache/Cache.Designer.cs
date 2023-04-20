namespace Cache
{
    partial class Cache
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
            textBox1 = new TextBox();
            button1 = new Button();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            listView2 = new ListView();
            button2 = new Button();
            textBox4 = new TextBox();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(4, 754);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(1699, 567);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(216, 25);
            button1.Name = "button1";
            button1.Size = new Size(150, 46);
            button1.TabIndex = 2;
            button1.Text = "Clear Cache";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(12, 29);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(184, 39);
            textBox2.TabIndex = 10;
            textBox2.Text = "Cache contents";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(12, 709);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(184, 39);
            textBox3.TabIndex = 12;
            textBox3.Text = "Logs";
            // 
            // listView2
            // 
            listView2.FullRowSelect = true;
            listView2.Location = new Point(12, 86);
            listView2.MultiSelect = false;
            listView2.Name = "listView2";
            listView2.Size = new Size(710, 567);
            listView2.TabIndex = 13;
            listView2.UseCompatibleStateImageBehavior = false;
            listView2.View = View.List;
            // 
            // button2
            // 
            button2.Location = new Point(784, 22);
            button2.Name = "button2";
            button2.Size = new Size(346, 46);
            button2.TabIndex = 14;
            button2.Text = "Check selected block content";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(784, 86);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.ScrollBars = ScrollBars.Vertical;
            textBox4.Size = new Size(919, 567);
            textBox4.TabIndex = 15;
            // 
            // Cache
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1715, 1333);
            Controls.Add(textBox4);
            Controls.Add(button2);
            Controls.Add(listView2);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Name = "Cache";
            Text = "Cache";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBox1;
        private Button button1;
        private TextBox textBox2;
        private TextBox textBox3;
        private ListView listView2;
        private Button button2;
        private TextBox textBox4;
    }
}