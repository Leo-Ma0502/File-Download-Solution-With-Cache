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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2, 353);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(917, 268);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(116, 12);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 22);
            this.button1.TabIndex = 2;
            this.button1.Text = "Clear Cache";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(6, 14);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(101, 23);
            this.textBox2.TabIndex = 10;
            this.textBox2.Text = "Cache contents";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(6, 332);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(101, 23);
            this.textBox3.TabIndex = 12;
            this.textBox3.Text = "Logs";
            // 
            // listView2
            // 
            this.listView2.FullRowSelect = true;
            this.listView2.Location = new System.Drawing.Point(6, 40);
            this.listView2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(384, 268);
            this.listView2.TabIndex = 13;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.List;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(422, 10);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(186, 22);
            this.button2.TabIndex = 14;
            this.button2.Text = "Check selected block content";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(422, 40);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox4.Size = new System.Drawing.Size(497, 268);
            this.textBox4.TabIndex = 15;
            // 
            // Cache
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 640);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "Cache";
            this.Text = "Cache";
            this.ResumeLayout(false);
            this.PerformLayout();

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