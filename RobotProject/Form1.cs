using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotProject
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      throw new System.NotImplementedException();
    }
    
    private int holder = 0;

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.button1.Location = new System.Drawing.Point(30, 100);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(120, 50);
      this.button1.TabIndex = 0;
      this.button1.Text = "Bağlantılar";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.button2.Location = new System.Drawing.Point(30, 170);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(120, 50);
      this.button2.TabIndex = 1;
      this.button2.Text = "Çalışma";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.label7);
      this.panel1.Controls.Add(this.label8);
      this.panel1.Controls.Add(this.label9);
      this.panel1.Controls.Add(this.label10);
      this.panel1.Controls.Add(this.label6);
      this.panel1.Controls.Add(this.label5);
      this.panel1.Controls.Add(this.label4);
      this.panel1.Controls.Add(this.label3);
      this.panel1.Location = new System.Drawing.Point(170, 100);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(300, 200);
      this.panel1.TabIndex = 4;
      this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
      // 
      // label3
      // 
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label3.Location = new System.Drawing.Point(30, 20);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(100, 30);
      this.label3.TabIndex = 0;
      this.label3.Text = "thing 1";
      this.label3.Click += new System.EventHandler(this.label3_Click);
      // 
      // label4
      // 
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label4.Location = new System.Drawing.Point(30, 60);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(100, 30);
      this.label4.TabIndex = 1;
      this.label4.Text = "thing 2";
      // 
      // label5
      // 
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label5.Location = new System.Drawing.Point(30, 100);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(100, 30);
      this.label5.TabIndex = 2;
      this.label5.Text = "thing 3";
      // 
      // label6
      // 
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label6.Location = new System.Drawing.Point(30, 140);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(100, 30);
      this.label6.TabIndex = 5;
      this.label6.Text = "thing 4";
      // 
      // label7
      // 
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label7.Location = new System.Drawing.Point(150, 140);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(100, 30);
      this.label7.TabIndex = 9;
      this.label7.Text = "kopuk";
      // 
      // label8
      // 
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label8.Location = new System.Drawing.Point(150, 100);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(100, 30);
      this.label8.TabIndex = 8;
      this.label8.Text = "kopuk";
      // 
      // label9
      // 
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label9.Location = new System.Drawing.Point(150, 60);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(100, 30);
      this.label9.TabIndex = 7;
      this.label9.Text = "bağlı";
      // 
      // label10
      // 
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
      this.label10.Location = new System.Drawing.Point(150, 20);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(100, 30);
      this.label10.TabIndex = 6;
      this.label10.Text = "bağlı";
      // 
      // Form1
      // 
      this.ClientSize = new System.Drawing.Size(624, 441);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Name = "Form1";
      this.Text = "Robot Projesi";
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;

    private System.Windows.Forms.Label label3;

    private System.Windows.Forms.Panel panel1;

    private System.Windows.Forms.Button button2;
    //private System.Windows.Forms.Label label1;

    private System.Windows.Forms.Button button1;

    //private System.Windows.Forms.Label label1;

 


    private void button1_Click(object sender, EventArgs e)
    {
      holder += 1;
      //label1.Text = $"{holder}";
      panel1.Visible = true;
    }

    private void label1_Click(object sender, EventArgs e)
    {
      throw new System.NotImplementedException();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      holder -= 1;
      //label1.Text = $"{holder}";
      panel1.Visible = false;
    }

    private void label2_Click(object sender, EventArgs e)
    {
      throw new System.NotImplementedException();
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
      //throw new System.NotImplementedException();
    }

    private void label3_Click(object sender, EventArgs e)
    {
      throw new System.NotImplementedException();
    }
  }
}
