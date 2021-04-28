using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form2";

            this.tl = new ModifiedLabel("tl1","hey");
            this.mb = new ModifiedButton("mb1","button");
            
             
            tl.Reorient(w:500);
            //tl.ClickFunction = (object sender, EventArgs e) => { };
            tl.clickAction = () => {
                tl.Text = "heyoo";
            };

            this.Controls.Add(tl);
            this.Controls.Add(mb);
            //NN nn = new NN();
            systemControls.implement(this.Controls);
            connectionIndicators.implement(this.Controls);
            
        }

        #endregion

        private SystemControls systemControls = new SystemControls(200, 200, 400, 100);
        private ConnectionIndicators connectionIndicators = new ConnectionIndicators(200, 300, 400, 100);
        private ModifiedLabel tl;
        private ModifiedButton mb;
    }

    class ConnectionIndicators
    {
        public ConnectionIndicators(int x, int y, int w, int h)
        {
            Rectangle r = new Rectangle(w, h, new Point( (float)x,(float)y ));
            this.plcIndicator = new Indicator("plc", "E:\\supreme_command\\cs_projects\\RobotProject\\RobotProject\\Images\\placeholder.jpg");
            Rectangle r1 = r.sliceVertical(0.0f, 0.3f);
            this.plcIndicator.Reorient((int)r1.x,(int)r1.y,(int)r1.w,(int)r1.h );
        }
        
        public void implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(plcIndicator);
        }
        
        private Indicator plcIndicator;
    }
    
    
    class SystemControls
    {
        public SystemControls(int x, int y, int w, int h)
        {
            Rectangle r = new Rectangle(w, h, new Point( (float)x,(float)y ));
            this.runButton = new ModifiedButton("run", "run");
            Rectangle r1 = r.sliceVertical(0.0f, 0.3f);
            this.runButton.Reorient((int)r1.x,(int)r1.y,(int)r1.w,(int)r1.h );
            //this.runButton.Reorient(50,300,50,100 );
            
            this.pauseButton = new ModifiedButton("pause", "pause");
            Rectangle r2 = r.sliceVertical(0.35f, 0.65f);
            this.pauseButton.Reorient((int)r2.x,(int)r2.y,(int)r2.w,(int)r2.h );
            
            this.stopButton = new ModifiedButton("stop", "stop");
            Rectangle r3 = r.sliceVertical(0.7f, 1f);
            this.stopButton.Reorient((int)r3.x,(int)r3.y,(int)r3.w,(int)r3.h );
        }

        public void implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(runButton);
            motherControlCollection.Add(pauseButton);
            motherControlCollection.Add(stopButton);
        }
        
        private ModifiedButton runButton;
        private ModifiedButton pauseButton;
        private ModifiedButton stopButton;
    }

    class Rectangle
    {
        public Rectangle(float w, float h, Point p)
        {
            this.w = w;
            this.h = h;
            this.x = p.x;
            this.y = p.y;
            this.l = this.x - w / 2;
            this.r = this.x + w / 2;
            this.t = this.y + h / 2;
            this.b = this.y - h / 2;
        }

        public Rectangle(float w1, float w2, float h1, float h2)
        {
            this.b = Math.Min(h1, h2);
            this.t = Math.Max(h1, h2);
            this.l = Math.Min(w1, w2);
            this.r = Math.Max(w1, w2);
            this.x = (l + r) / 2;
            this.y = (t + b) / 2;
            this.w = r - l;
            this.h = t - b;
        }

        /** slice vertical slices the rectangle to floats ratios of 1
         * 
         */
        public Rectangle sliceVertical(float wStart, float wEnd)
        {
            return new Rectangle((int )(l + w * wStart), (int)( l + w * wEnd), b, t);
        }

        public float x;
        public float y;
        public float w;
        public float h;
        public float l;
        public float r;
        public float t;
        public float b;
    }

    class Point
    {
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float x;
        public float y;
    }




}