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

            
            
            // example function implementation, other system controls should be implemented similarly
            systemControls.AddProductButton.ClickAction = () =>
            {
                nbp.Opening();
                nbp.ShowDialog();
                if (nbp.confirmed)
                {
                    var  l = nbp.GetLines;
                    boxVisuals.AddToBoxes(new SingleBox("id", l[0], l[1], l[2], l[3], true, 0));
                }
            };

            systemControls.PauseButton.ClickAction = () =>
            {
                var a = new GenericWarning("Sistem duraklatıldı.");
                a.ShowDialog();
            };
            
            systemControls.RunButton.ClickAction = () =>
            {
                var a = new GenericWarning("Sistem çalıştırıldı.");
                a.ShowDialog();
            };
            
            systemControls.StopButton.ClickAction = () =>
            {
                var a = new GenericWarning("Sistem durduruldu.");
                a.ShowDialog();
            };
            
            
            systemControls.Implement(this.Controls);
            connectionIndicators.Implement(this.Controls);
            boxVisuals.Implement(this.Controls);
        }

        

        #endregion

        private SystemControls systemControls = new SystemControls(3*appWidth/4, 50, appWidth/2, 100,false);
        private ConnectionIndicators connectionIndicators = new ConnectionIndicators(appWidth/8, 50, appWidth/4, 100,false);
        private BoxVisuals boxVisuals = new BoxVisuals(appWidth/2, (appHeight-100)/2+100, 3*appWidth/4, appHeight-100,false);
        private NonBarcodePopup nbp = new NonBarcodePopup();
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