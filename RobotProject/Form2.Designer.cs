using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using RobotProject.Form2Items;
using RobotProject.uiElements;

namespace RobotProject
{
    partial class Form2
    {
        private static int appWidth = 1920;
        private static int appHeight = 1080;

        private static ConnectionManager conn = new ConnectionManager();

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
            // this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            var n = 5;
            var n2 = String.Format("{0}", n);

            this.tl = new ModifiedLabel("tl1","hey");
            this.mb = new ModifiedButton("mb1","ürün ekle");
            this.mb.ClickAction = () =>
            {
                nbp.Opening();
                nbp.ShowDialog();
                if (nbp.Confirmed)
                {
                    var  l = nbp.GetLines;
                    boxVisuals.AddToBoxes(new SingleBox("id", l[0], l[1], l[2], l[3], true, 0));
                    //mb.Text = nbp.att1.text;
                }
                else
                {
                    mb.Text = "hey";
                }
            };
            
            mb.Reorient(new Geometry.Rectangle(0,appWidth/8,100, 200));
            
            tl.Reorient(w:500);
            //tl.ClickFunction = (object sender, EventArgs e) => { };
            tl.ClickAction = () =>
            {
                //tl.Text = conn.ConvertFromHex("12345");
            };

            // this.Controls.Add(tl);
            this.Controls.Add(mb);
            
            //NN nn = new NN();
            systemControls.Implement(this.Controls);
            connectionIndicators.Implement(this.Controls);
            boxVisuals.Implement(this.Controls);
            conn.BarcodeRead += barcodeUpdater;
            conn.Init();
            conn.Connect();
        }

        

        #endregion

        private void barcodeUpdater(object sender, EventArgs e)
        {
            tl.Text = "Event Test = " + conn.Data;
            Console.WriteLine("Event Test = " + conn.Data);
        }

        private SystemControls systemControls = new SystemControls(3*appWidth/4, 50, appWidth/2, 100,false);
        private ConnectionIndicators connectionIndicators = new ConnectionIndicators(appWidth/4, 50, appWidth/2, 100,false);
        private BoxVisuals boxVisuals = new BoxVisuals(appWidth/2, (appHeight-100)/2+100, 3*appWidth/4, appHeight-100,false);
        private ModifiedLabel tl;
        private ModifiedButton mb;
        private NonBarcodePopup nbp = new NonBarcodePopup();
    }

    /** Outside functions
     * BoxVisuals.addToBoxes (adds a new item to the belt, the struct has id but it is not used, and order is used)
     * BoxVisuals.robotOperation (input no robot operates on the next item on the list)
     * BoxVisuals.emptyPallete (empties the pallete given as input)
     * BoxVisuals.relevantPallete (used to change the type of pallete to be used)
     * 
     */

}