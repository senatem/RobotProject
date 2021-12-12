using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using MySql.Data.Types;
using RobotProject.Form2Items;
using RobotProject.uiElements;
using RobotProject.Form2Items.palletteStuff;

namespace RobotProject
{
    partial class Form2
    {
        private static int appWidthInit = 1280;
        private static int appHeightInit = 720;
        private static float appRatio = (float)(appWidthInit) / (float)appHeightInit;
        private int appHeight = appHeightInit;
        private int appWidth = appWidthInit;

        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            SaveData();
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
            this.ClientSize = new System.Drawing.Size(appWidthInit, appHeightInit);
            this.Text = "Paletleyici Kontrolleri";
            this.BackColor = Color.DimGray;

            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            
            //this.AutoScaleMode = AutoScaleMode.Font;

            //this.MaximizeBox = false;
            //this.MinimizeBox = false;

            this.Icon = new System.Drawing.Icon(References.ProjectPath + "Images\\t-ara128.ico");
            

            var n = 5;
            var n2 = String.Format("{0}", n);

            systemControls.PalleteButton.ClickAction = () =>
            {
                // input is the list of options, change options here
                
                
                List<string> orders = ConnectionManager.Sql.GetOrders();
                pp.Opening(orders.ToArray());
                pp.ShowDialog();
                if (pp.Confirmed)
                {
                    var no = pp.RobotNo-1;
                    ConnectionManager.EmptyCell(no);
                    emptyCell(no);
                    var order = orders[pp.SelectedIndex];
                    var p = ConnectionManager.Sql.GetPallet(order);
                    
                    // pp.SelectedIndex returns the selected index
                    
                    // adjust the following line according to the returned index
                    var product = ConnectionManager.Sql.Select("Siparis_No", order);
                    palleteVisuals.setPallette(no, orders[pp.SelectedIndex],p.GetHeight().ToString(),p.GetLength().ToString(),p.GetMax());
                    int k = ConnectionManager.GetKatMax(p.GetHeight(), p.GetLength(), product.GetYontem(),
                        product.GetProductType());
                    ConnectionManager.AssignCell(long.Parse(order), no+1, p, k);
                    

                    // pallete openin dialog confirmed
                    // result can be taken as: pp.Text
                }
            };


            systemControls.ReconnectButton.ClickAction = () =>
            {
               ConnectionManager.Connect();
               Console.Write("Enter your name: ");
            
            };
            

            
            
            systemControls.AddProductButton.ClickAction = () =>
            {
                nbp.Opening();
                nbp.ShowDialog();
                if (nbp.confirmed)
                {
                    // new box add confirmed
                    var info = nbp.GetLines;
                    ConnectionManager.EmptyCell(nbp.RobotNo-1);
                    emptyCell(nbp.RobotNo-1);
                    ConnectionManager.AssignNonBarcodeCell(nbp.RobotNo, int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]), int.Parse(info[3]), info[4], int.Parse(info[5]),
                        int.Parse(info[6]), int.Parse(info[7]), int.Parse(info[8]));
                    assignCell(nbp.RobotNo, 0, new Pallet(int.Parse(info[5]), int.Parse(info[6]), int.Parse(info[2]), int.Parse(info[3])));
                    ConnectionManager.PatternMode = true;
                }
            };
            
            servoControls.Implement(this.Controls);
            servoControls.applyPressed = () =>
            {
                // buraya apply düğmesinde isteidğin şeyi yaz
                servoControls.getValues(); // kayıtlı değerler için
            };
            
            errorBox.Implement(this.Controls);

            systemControls.Implement(this.Controls);
            palleteVisuals.Implement(this.Controls);
            
             
            
            //boxVisuals.Implement(this.Controls);
            
            ConnectionManager.BarcodeConnectionChanged += barcodeIndicatorUpdater;
            ConnectionManager.PlcConnectionChanged += plcIndicatorUpdater;
            ConnectionManager.TaperConnectionChanged += taperIndicatorUpdater;
            ConnectionManager.ProductIncoming += productAdd;
            ConnectionManager.ProductDropped += productFill;
            ConnectionManager.CellFull += resetKat;
            ConnectionManager.CellAssigned += assignCell;
            ConnectionManager.Init();
            LoadData();
            //ConnectionManager.Connect();
            connectionIndicators.Implement(this.Controls);
            connectionIndicators.BarcodeConnect(ConnectionManager.BarcodeClient.Connected);
            connectionIndicators.PlcConnect(ConnectionManager.PlcClient.Connected);
            connectionIndicators.TaperConnect(ConnectionManager.PlcClient2.Connected);
            
            Resize += new EventHandler(Form2_Resize);
            
            bg.Reorient(0,0,appWidth,appHeight);
            Controls.Add(bg);
            
            var appRect = new Geometry.Rectangle(0f, ClientSize.Width, 0f, ClientSize.Height);
            
            var boxRect = appRect.RatedRectangle(appRatio);
            systemControls.resizeToWindowRect(boxRect);
            connectionIndicators.resizeToWindowRect(boxRect);
            palleteVisuals.resizeToWindowRect(boxRect);
            //servoControls.resizeToWindowRect(boxRect);
            errorBox.resizeToWindowRect(boxRect);
            var l = new List<String>();
            l.Add("some error");
            l.Add("Bantlama 1-2 Hizalama Motorları Hatada");
            errorBox.setErrorText(l);
            
            bg.Reorient((int)boxRect.L,(int)boxRect.T,(int)boxRect.W,(int)boxRect.H);
            
        }

        

        #endregion
        
        private void Form2_Resize(object sender, System.EventArgs e)
        {
            
            Console.Write("Enter your name: ");
            
            Control control = (Control)sender;
        
            // Ensure the Form remains square (Height = Width).
            //if(control.Size.Height != control.Size.Width)
            //{
            //    control.Size = new Size(control.Size.Width, control.Size.Width);
            //}
            
            var appRect = new Geometry.Rectangle(0f, Size.Width-10, 0f, Size.Height);
            var boxRect = appRect.RatedRectangle(appRatio);
            systemControls.resizeToWindowRect(boxRect);
            connectionIndicators.resizeToWindowRect(boxRect);
            palleteVisuals.resizeToWindowRect(boxRect);
            servoControls.resizeToWindowRect(boxRect);
            errorBox.resizeToWindowRect(boxRect);
            bg.Reorient((int)boxRect.L,(int)boxRect.T,(int)boxRect.W+2,(int)boxRect.H);
            
            
            
            //systemControls = new SystemControls(3*appWidthInit/8, 50, 3*appWidthInit/4, 100,false);
        }

        private void barcodeIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.BarcodeConnect(ConnectionManager.BarcodeClient.Available(50));
        }

        private void plcIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.PlcConnect(ConnectionManager.PlcClient.Connected);
        }

        private void taperIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.TaperConnect(ConnectionManager.TaperConnected);
        }
        private void productAdd(int r)
        {
            palleteVisuals.increaseProdCount(r, 1);
            //boxVisuals.AddToBoxes(new SingleBox(o, p.GetHeight().ToString(), p.GetWidth().ToString(), false, r));
        }

        private void productFill(int r)
        {
            palleteVisuals.increaseFillCount(r, 1);
        }

        private void resetKat(int i)
        {
            palleteVisuals.resetKat(i);
        }

        private void emptyCell(int i)
        {
            palleteVisuals.EmptyPallette(i);
           // boxVisuals.EmptyPallete(i);
        }

        private void assignCell(int i, long orderNo, Pallet p)
        {
            var no = i-1;
            try
            {
                palleteVisuals.setPallette(no, orderNo.ToString(),p.GetHeight().ToString(),p.GetLength().ToString(),p.GetMax());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void SaveData()
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(ConnectionManager.Cells);
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            File.WriteAllText(Path.Combine(docPath, "cells"), jsonString);
            jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(ConnectionManager.PatternMode);
            File.WriteAllText(Path.Combine(docPath, "config"), jsonString);
            jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(ConnectionManager.PatternProduct);
            File.WriteAllText(Path.Combine(docPath, "pattern"), jsonString);
        }

        private void LoadData()
        {
            try
            {
                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                using (var sr = new StreamReader(Path.Combine(docPath, "cells")))
                {
                    var jsonString = sr.ReadToEnd();
                    var cells = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Cell>>(jsonString);
                    if (cells != null)
                    {
                        ConnectionManager.Cells = cells;
                    }
                }

                foreach (var cell in ConnectionManager.Cells)
                {
                    palleteVisuals.setPallette(cell.RobotNo-1, cell.OrderNo.ToString(), cell.PalletHeight.ToString(),cell.PalletWidth.ToString(),cell.OrderSize);
                    // adjust here to adjust prodcuts, setProdCount has two inputs nullable first for defined, second for filled
                    palleteVisuals.setProdCount(cell.RobotNo-1, cell.Holding);
                    palleteVisuals.setProdCount(cell.RobotNo-1, valueFill: cell.Dropped);
                }
                
                using (var sr = new StreamReader(Path.Combine(docPath, "config")))
                {
                    var jsonString = sr.ReadToEnd();
                    ConnectionManager.PatternMode = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(jsonString);
                }
                using (var sr = new StreamReader(Path.Combine(docPath, "pattern")))
                {
                    var jsonString = sr.ReadToEnd();
                    ConnectionManager.PatternProduct = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(jsonString);
                }
            }
            catch
            {
                
            }

        }
        
        
        
        /*
        private void Form2_ResizeEnd(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
        
            // Ensure the Form remains square (Height = Width).
            if(control.Size.Height != control.Size.Width)
            {
                control.Size = new Size(control.Size.Width, control.Size.Width);
            }
        }
        */

        private SystemControls systemControls = new SystemControls(3*appWidthInit/8, 50, 3*appWidthInit/4, 100,false);
        private ConnectionIndicators connectionIndicators = new ConnectionIndicators(7*appWidthInit/8, 50, appWidthInit/4, 100,false);
        // private BoxVisuals boxVisuals = new BoxVisuals(appWidth/2, (appHeight-100)/2+100, 3*appWidth/4, appHeight-100,false);
        
        //private PalleteVisuals palleteVisuals = new PalleteVisuals(appWidthInit/2, (appHeightInit-100)/2+100, appWidthInit, appHeightInit-100,false);
        private PalleteVisuals palleteVisuals = new PalleteVisuals(new Geometry.Rectangle(0f,appWidthInit,0f,appHeightInit),false);
        private ServoControls servoControls = new ServoControls(new Geometry.Rectangle(0f,appWidthInit,0f,appHeightInit),asVisual: false);
        private ErrorBox errorBox= new ErrorBox(new Geometry.Rectangle(0f,appWidthInit,0f,appHeightInit),asVisual: false);
        private Indicator bg  = new Indicator("bg", References.ProjectPath + "Images\\bg.png");
        private NonBarcodePopup nbp = new NonBarcodePopup();
        private PalletePopup pp = new PalletePopup();
    }

    
    /** Outside functions
     * BoxVisuals.addToBoxes (adds a new item to the belt, the struct has id but it is not used, and order is used)
     * BoxVisuals.robotOperation (input no robot operates on the next item on the list)
     * BoxVisuals.emptyPallete (empties the pallete given as input)
     * BoxVisuals.relevantPallete (used to change the type of pallete to be used)
     * PalleteVisuals.emptyPallette (empties the pallete given as input)
     * PalleteVisuals.setProdCount/increaseProdCount (adjusts the product count as required)
     * ConnectionIndicators.plcConnect (true on connect false on break)
     * ConnectionIndicators.barcodeConnect (true on connect false on break)
     * SystemControls.runButton.clickAction shold be modified (other buttons too) to fit the backend needs
     */
}