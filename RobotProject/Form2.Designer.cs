using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject
{
    partial class Form2
    {
        private static int appWidth = 1920;
        private static int appHeight = 1080;
        
        
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
            this.ClientSize = new System.Drawing.Size(appWidth, appHeight);
            this.Text = "Form2";

            var n = 5;
            var n2 = String.Format("{0}", n);

            this.tl = new ModifiedLabel("tl1","hey");
            this.mb = new ModifiedButton("mb1","button");
            this.mb.clickAction = () =>
            {
                nbp.opening();
                nbp.ShowDialog();
                if (nbp.confirmed)
                {
                    var  l = nbp.getLines;
                    boxVisuals.addToBoxes(new SingleBox("id", l[0], l[1], l[2], l[3], true, 0));
                    //mb.Text = nbp.att1.text;
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
            boxVisuals.implement(this.Controls);
            
            
            

        }

        

        #endregion

        private int counter = 0; 
        private SystemControls systemControls = new SystemControls(3*appWidth/4, 50, appWidth/2, 100,false);
        private ConnectionIndicators connectionIndicators = new ConnectionIndicators(appWidth/4, 50, appWidth/2, 100,false);
        private BoxVisuals boxVisuals = new BoxVisuals(3*appWidth/4, (appHeight-100)/2+100, appWidth/2, appHeight-100,false);
        private ModifiedLabel tl;
        private ModifiedButton mb;
        private NonBarcodePopup nbp = new NonBarcodePopup();
    }


}