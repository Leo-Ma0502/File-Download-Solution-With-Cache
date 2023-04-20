namespace FileServer;

partial class FileServer
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
        textBox2 = new TextBox();
        listView2 = new ListView();
        textBox3 = new TextBox();
        button1 = new Button();
        textBox4 = new TextBox();
        SuspendLayout();
        // 
        // textBox1
        // 
        textBox1.Location = new Point(7, 741);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.ScrollBars = ScrollBars.Vertical;
        textBox1.Size = new Size(1643, 314);
        textBox1.TabIndex = 1;
        textBox1.WordWrap = false;
        // 
        // textBox2
        // 
        textBox2.Location = new Point(911, 25);
        textBox2.Name = "textBox2";
        textBox2.ReadOnly = true;
        textBox2.Size = new Size(269, 39);
        textBox2.TabIndex = 5;
        textBox2.Text = "Files available to client";
        // 
        // listView2
        // 
        listView2.FullRowSelect = true;
        listView2.Location = new Point(12, 85);
        listView2.Name = "listView2";
        listView2.Size = new Size(605, 620);
        listView2.TabIndex = 6;
        listView2.UseCompatibleStateImageBehavior = false;
        listView2.View = View.List;
        // 
        // textBox3
        // 
        textBox3.Location = new Point(25, 25);
        textBox3.Name = "textBox3";
        textBox3.ReadOnly = true;
        textBox3.Size = new Size(269, 39);
        textBox3.TabIndex = 7;
        textBox3.Text = "File in list";
        // 
        // button1
        // 
        button1.Location = new Point(636, 355);
        button1.Name = "button1";
        button1.Size = new Size(249, 46);
        button1.TabIndex = 8;
        button1.Text = "make available >>";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click_1;
        // 
        // textBox4
        // 
        textBox4.Location = new Point(911, 85);
        textBox4.Multiline = true;
        textBox4.Name = "textBox4";
        textBox4.ReadOnly = true;
        textBox4.ScrollBars = ScrollBars.Vertical;
        textBox4.Size = new Size(662, 620);
        textBox4.TabIndex = 9;
        // 
        // FileServer
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1648, 1056);
        Controls.Add(textBox4);
        Controls.Add(button1);
        Controls.Add(textBox3);
        Controls.Add(listView2);
        Controls.Add(textBox2);
        Controls.Add(textBox1);
        Name = "FileServer";
        Text = "File Server";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private TextBox textBox1;
    private TextBox textBox2;
    private ListView listView2;
    private TextBox textBox3;
    private Button button1;
    private TextBox textBox4;
}
