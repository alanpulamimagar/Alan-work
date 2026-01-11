using System.Drawing;
using System.Windows.Forms;

namespace booseapp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private PictureBox pictureBox1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Label label1;
        private Label label2;
        private Label label3;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer1 = new SplitContainer();
            this.splitContainer2 = new SplitContainer();
            this.textBox1 = new TextBox();
            this.label1 = new Label();
            this.button1 = new Button();
            this.button2 = new Button();
            this.button3 = new Button();
            this.textBox2 = new TextBox();
            this.label2 = new Label();
            this.pictureBox1 = new PictureBox();
            this.label3 = new Label();
            this.button4 = new Button();
            this.button5 = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();

            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Location = new Point(10, 10);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel2.Controls.Add(this.button4);
            this.splitContainer1.Panel2.Controls.Add(this.button5);
            this.splitContainer1.Size = new Size(1164, 721);
            this.splitContainer1.SplitterDistance = 320;
            this.splitContainer1.TabIndex = 0;

            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = DockStyle.Fill;
            this.splitContainer2.Location = new Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBox1);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.button1);
            this.splitContainer2.Panel1.Controls.Add(this.button2);
            this.splitContainer2.Panel1.Controls.Add(this.button3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox2);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Size = new Size(1164, 320);
            this.splitContainer2.SplitterDistance = 582;
            this.splitContainer2.TabIndex = 0;

            // 
            // textBox1
            // 
            this.textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.textBox1.Font = new Font("Consolas", 10F);
            this.textBox1.Location = new Point(3, 30);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = ScrollBars.Both;
            this.textBox1.Size = new Size(576, 240);
            this.textBox1.TabIndex = 1;
            this.textBox1.WordWrap = false;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.label1.Location = new Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new Size(123, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "BOOSE Program:";
            this.label1.Click += new System.EventHandler(this.label1_Click);

            // 
            // button1
            // 
            this.button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.button1.BackColor = Color.FromArgb(0, 120, 215);
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.button1.ForeColor = Color.White;
            this.button1.Location = new Point(3, 280);
            this.button1.Name = "button1";
            this.button1.Size = new Size(100, 35);
            this.button1.TabIndex = 2;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);

            // 
            // button2
            // 
            this.button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.button2.BackColor = Color.FromArgb(220, 220, 220);
            this.button2.FlatStyle = FlatStyle.Flat;
            this.button2.Font = new Font("Segoe UI", 9F);
            this.button2.Location = new Point(109, 280);
            this.button2.Name = "button2";
            this.button2.Size = new Size(100, 35);
            this.button2.TabIndex = 3;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button3_Click);

            // 
            // button3
            // 
            this.button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.button3.BackColor = Color.FromArgb(240, 240, 240);
            this.button3.FlatStyle = FlatStyle.Flat;
            this.button3.Font = new Font("Segoe UI", 9F);
            this.button3.Location = new Point(215, 280);
            this.button3.Name = "button3";
            this.button3.Size = new Size(120, 35);
            this.button3.TabIndex = 4;
            this.button3.Text = "Load Sample";
            this.button3.UseVisualStyleBackColor = false;

            // 
            // textBox2
            // 
            this.textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.textBox2.BackColor = Color.FromArgb(250, 250, 250);
            this.textBox2.Font = new Font("Consolas", 9F);
            this.textBox2.Location = new Point(3, 30);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = ScrollBars.Both;
            this.textBox2.Size = new Size(572, 285);
            this.textBox2.TabIndex = 1;
            this.textBox2.WordWrap = false;

            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.label2.Location = new Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new Size(98, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output / Log:";

            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.pictureBox1.BackColor = Color.White;
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox1.Location = new Point(3, 30);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(1158, 330);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);

            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.label3.Location = new Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new Size(120, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "Drawing Canvas:";

            // 
            // button4
            // 
            this.button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.button4.BackColor = Color.FromArgb(240, 240, 240);
            this.button4.FlatStyle = FlatStyle.Flat;
            this.button4.Font = new Font("Segoe UI", 9F);
            this.button4.Location = new Point(961, 365);
            this.button4.Name = "button4";
            this.button4.Size = new Size(90, 30);
            this.button4.TabIndex = 6;
            this.button4.Text = "Save...";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);

            // 
            // button5
            // 
            this.button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.button5.BackColor = Color.FromArgb(240, 240, 240);
            this.button5.FlatStyle = FlatStyle.Flat;
            this.button5.Font = new Font("Segoe UI", 9F);
            this.button5.Location = new Point(1061, 365);
            this.button5.Name = "button5";
            this.button5.Size = new Size(100, 30);
            this.button5.TabIndex = 7;
            this.button5.Text = "Load...";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1184, 741);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Padding = new Padding(10);
            this.Text = "BOOSE Application - Basic Object Orientated Software Engineering";

            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}