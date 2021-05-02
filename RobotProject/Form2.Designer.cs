using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
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
            this.mb.clickAction = () =>
            {
                nbp.opening();
                nbp.ShowDialog();
                if (nbp.confirmed)
                {
                    mb.Text = nbp.att1.text;                    
                }
                else
                {
                    mb.Text = "hey";
                }
            };
            
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
        private NonBarcodePopup nbp = new NonBarcodePopup();
    }

    /** Connection indicators are to indicate connection, this is achieved by calling indicator:
     * plcIndicator.paint(Color.Red); or green
     * new indicators are added to the initializer and as variables as needed
     */
    class ConnectionIndicators
    {
        public ConnectionIndicators(int x, int y, int w, int h)
        {
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( (float)x,(float)y ));
            
            // indicators
            this.plcIndicator = new Indicator("plc", References.projectPath + "Images\\placeholder.jpg");
            Geometry.Rectangle r1 = r.sliceVertical(0.0f, 0.3f);
            this.plcIndicator.Reorient(r1 );
            
            this.barcodeIndicator = new Indicator("plc", References.projectPath + "Images\\placeholder.jpg");
            Geometry.Rectangle r2 = r.sliceVertical(0.3f, 0.6f);
            this.barcodeIndicator.Reorient(r2 );
            
        }
        
        public void implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(plcIndicator);
            motherControlCollection.Add(barcodeIndicator);
        }
        
        
        
        private Indicator plcIndicator;
        private Indicator barcodeIndicator;
    }
    
    /** Similar to indicators, but buttons instead
     * a function adding mechanism is not included yet, but buttons are public so it can be done like that
     */
    class SystemControls
    {
        public SystemControls(int x, int y, int w, int h)
        {
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( (float)x,(float)y ));
            this.runButton = new ModifiedButton("run", "run");
            Geometry.Rectangle r1 = r.sliceVertical(0.0f, 0.3f);
            this.runButton.Reorient(r1 );
            //this.runButton.Reorient(50,300,50,100 );
            
            this.pauseButton = new ModifiedButton("pause", "pause");
            Geometry.Rectangle r2 = r.sliceVertical(0.35f, 0.65f);
            this.pauseButton.Reorient(r2);
            
            this.stopButton = new ModifiedButton("stop", "stop");
            Geometry.Rectangle r3 = r.sliceVertical(0.7f, 1f);
            this.stopButton.Reorient(r3);
        }

        public void implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(runButton);
            motherControlCollection.Add(pauseButton);
            motherControlCollection.Add(stopButton);
        }
        
        public ModifiedButton runButton;
        public ModifiedButton pauseButton;
        public ModifiedButton stopButton;
    }


    /** This is the popup for non barcode units, necessary attributes will be added manually at initializer via adding new pairs to lines bit
     * confirm or exit can be traced by confirmed boolean
     * entered texts can be obtained by att1.text etc.
     * reset function resets inputs, it may be added to exit or opening methods, new attributes must be added manually
     * right now attributes are individual fields, they may be put into a list
     */
    class NonBarcodePopup : Form
    {
        private IContainer components = null;
        public NonBarcodePopup()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size((int)w, (int)h);
            this.Text = "Form2";
            Geometry.Rectangle v = new Geometry.Rectangle(25f, w-25f, 50f, h-50f);
            
            // lines
            var r1 = v.sliceHorizontal(0f, 0.2f);
            att1 = new TextPair("att1", "attribute 1", r1); 
            att1.implement(this.Controls);
            
            var r2 = v.sliceHorizontal(0.3f, 0.5f);
            att2 = new TextPair("att2", "attribute 2", r2); 
            att2.implement(this.Controls);
            
            
            // buttons
            var buttonsRect = v.sliceHorizontal(0.7f, 0.8f);
            var conf = new ModifiedButton("confirm","confirm");
            conf.Reorient(buttonsRect.sliceVertical(0.1f,0.4f));
            conf.clickAction = () =>
            {
                confirmed = true;
                this.Close();
            };
            
            var exit  = new ModifiedButton("exit","exit");
            exit.Reorient(buttonsRect.sliceVertical(0.6f,0.9f));
            exit.clickAction = () =>
            {
                this.Close();
            };

            Controls.Add(conf);
            Controls.Add(exit);
        }
        private float w = 400f;
        private float h = 450f;

        public void reset()
        {
            att1.text = "0";
            att2.text = "0";
        }
        public void opening()
        {
            confirmed = false;
        }
        public TextPair att1;
        public TextPair att2;
        public Boolean confirmed = false;

    }

    



}