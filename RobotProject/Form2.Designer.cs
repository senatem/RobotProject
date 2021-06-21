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
        private static int appWidth = 1280;
        private static int appHeight = 720;

        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            ConnectionManager.KillThreads();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(appWidth, appHeight);
            this.Text = "Paletleyici Kontrolleri";
            // this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            var n = 5;
            var n2 = String.Format("{0}", n);

            systemControls.PalleteButton.ClickAction = () =>
            {
                pp.Opening();
                pp.ShowDialog();
                if (pp.Confirmed)
                {
                    // pallete openin dialog confirmed
                    // result can be taken as: pp.Text
                }
            };

            systemControls.AddProductButton.ClickAction = () =>
            {
                nbp.Opening();
                nbp.ShowDialog();
                if (nbp.confirmed)
                {
                    // new box add confirmed
                    var l = nbp.GetLines;
                    boxVisuals.AddToBoxes(new SingleBox(l[0], l[1], l[2], true, 0));
                }
            };

            systemControls.Implement(this.Controls);
            connectionIndicators.Implement(this.Controls);
            boxVisuals.Implement(this.Controls);
            ConnectionManager.BarcodeRead += barcodeUpdater;
            ConnectionManager.BarcodeConnectionChanged += barcodeIndicatorUpdater;
            ConnectionManager.PlcConnectionChanged += plcIndicatorUpdater;
            ConnectionManager.ProductIncoming += productAdd;
            ConnectionManager.CellFull += emptyCell;
            ConnectionManager.Init();
            ConnectionManager.Connect();
        }

        

        #endregion

        private void barcodeIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.BarcodeConnect(ConnectionManager.BarcodeClient.Connected);
        }

        private void plcIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.PlcConnect(ConnectionManager.PlcClient.Connected);
        }
        
        private void barcodeUpdater(object sender, EventArgs e)
        {
            //MessageBox.Show("Event Test = " + conn.Data);
        }
        private void productAdd(string o, Product p, int r)
        {
            boxVisuals.AddToBoxes(new SingleBox(o, p.GetHeight().ToString(), p.GetWidth().ToString(), false, r));
        }

        private void emptyCell(int i)
        {
            boxVisuals.EmptyPallete(i);
        }

        private SystemControls systemControls = new SystemControls(3*appWidth/4, 50, appWidth/2, 100,false);
        private ConnectionIndicators connectionIndicators = new ConnectionIndicators(appWidth/8, 50, appWidth/4, 100,false);
        private BoxVisuals boxVisuals = new BoxVisuals(appWidth/2, (appHeight-100)/2+100, 3*appWidth/4, appHeight-100,false);
        private NonBarcodePopup nbp = new NonBarcodePopup();
        private PalletePopup pp = new PalletePopup();
    }

    
    /** Outside functions
     * BoxVisuals.addToBoxes (adds a new item to the belt, the struct has id but it is not used, and order is used)
     * BoxVisuals.robotOperation (input no robot operates on the next item on the list)
     * BoxVisuals.emptyPallete (empties the pallete given as input)
     * BoxVisuals.relevantPallete (used to change the type of pallete to be used)
     * ConnectionIndicators.plcConnect (true on connect false on break)
     * ConnectionIndicators.barcodeConnect (true on connect false on break)
     * SystemControls.runButton.clickAction shold be modified (other buttons too) to fit the backend needs
     */
    
    
    

}