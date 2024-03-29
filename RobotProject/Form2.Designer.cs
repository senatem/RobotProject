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
using RobotProject.Form2Items;
using RobotProject.uiElements;
using RobotProject.Form2Items.palletteStuff;

namespace RobotProject
{
    partial class Form2
    {
        private static int appWidthInit = 1920;
        private static int appHeightInit = 1080;
        private static float appRatio = (float)(appWidthInit) / (float)appHeightInit;
        private int appHeight = appHeightInit;
        private int appWidth = appWidthInit;

        private bool bypassing = false;
        private List<double> activeErrors = new List<double>();

        #region error texts

        private List<List<string>>errorList = new List<List<string>>();

        private void SetErrorTexts()
        {
            var r1Errors = new List<string>();
            r1Errors.Add("");
            r1Errors.Add("Robot 1 Servo 1 Hatada");
            r1Errors.Add("Robot 1 Servo 2 Hatada");
            r1Errors.Add("Robot 1 Servo 1-2 Hatada");
            r1Errors.Add("Robot 1 Servo 3 Hatada");
            r1Errors.Add("Robot 1 Servo 1-3 Hatada");
            r1Errors.Add("Robot 1 Servo 2-3 Hatada");
            r1Errors.Add("Robot 1 Servo 1-2-3 Hatada");
            errorList.Add(r1Errors);
            
            var r2Errors = new List<string>();
            r2Errors.Add("");
            r2Errors.Add("Robot 2 Servo 1 Hatada");
            r2Errors.Add("Robot 2 Servo 2 Hatada");
            r2Errors.Add("Robot 2 Servo 1-2 Hatada");
            r2Errors.Add("Robot 2 Servo 3 Hatada");
            r2Errors.Add("Robot 2 Servo 1-3 Hatada");
            r2Errors.Add("Robot 2 Servo 2-3 Hatada");
            r2Errors.Add("Robot 2 Servo 1-2-3 Hatada");
            errorList.Add(r2Errors);
            
            var r3Errors = new List<string>();
            r3Errors.Add("");
            r3Errors.Add("Robot 3 Servo 1 Hatada");
            r3Errors.Add("Robot 3 Servo 2 Hatada");
            r3Errors.Add("Robot 3 Servo 1-2 Hatada");
            r3Errors.Add("Robot 3 Servo 3 Hatada");
            r3Errors.Add("Robot 3 Servo 1-3 Hatada");
            r3Errors.Add("Robot 3 Servo 2-3 Hatada");
            r3Errors.Add("Robot 3 Servo 1-2-3 Hatada");
            errorList.Add(r3Errors);

            var tableErrors = new List<string>();
            tableErrors.Add("");
            tableErrors.Add("Döner Tabla 1 Hatada");
            tableErrors.Add("Döner Tabla 2 Hatada");
            tableErrors.Add("Döner Tabla 1-2 Hatada");
            tableErrors.Add("Döner Tabla 3 Hatada");
            tableErrors.Add("Döner Tabla 1-3 Hatada");
            tableErrors.Add("Döner Tabla 2-3 Hatada");
            tableErrors.Add("Döner Tabla 1-2-3 Hatada");
            errorList.Add(tableErrors);
            
            var taper1Errors = new List<string>();
            taper1Errors.Add("");
            taper1Errors.Add("Bantlama 1 Servo Hatada");
            taper1Errors.Add("Bantlama 1 Hizalama Motoru Hatada");
            taper1Errors.Add("Bantlama 1 Servo-Hizalama Motoru Hatada");
            taper1Errors.Add("Bantlama 1 Bant Bitti");
            taper1Errors.Add("Bantlama 1 Servo Hatada-Bant Bitti");
            taper1Errors.Add("Bantlama 1 Servo Hatada-Hizalama Motoru Hatada-Bant Bitti");
            
            
            var taper2Errors = new List<string>();
            taper2Errors.Add("");
            taper2Errors.Add("Bantlama 2 Servo Hatada");
            taper2Errors.Add("Bantlama 2 Hizalama Motoru Hatada");
            taper2Errors.Add("Bantlama 2 Servo-Hizalama Motoru Hatada");
            taper2Errors.Add("Bantlama 2 Bant Bitti");
            taper2Errors.Add("Bantlama 2 Servo Hatada-Bant Bitti");
            taper2Errors.Add("Bantlama 2 Servo Hatada-Hizalama Motoru Hatada-Bant Bitti");

            errorList.Add(taper1Errors);
            errorList.Add(taper2Errors);

            var barrierErrors = new List<string>();
            barrierErrors.Add("");
            barrierErrors.Add("Hücre 1 Işık Bariyeri İhlalde");
            barrierErrors.Add("Hücre 2 Işık Bariyeri İhlalde");
            barrierErrors.Add("Hücre 1-2 Işık Bariyeri İhlalde");
            barrierErrors.Add("Hücre 3 Işık Bariyeri İhlalde");
            barrierErrors.Add("Hücre 1-3 Işık Bariyeri İhlalde");
            barrierErrors.Add("Hücre 2-3 Işık Bariyeri İhlalde");
            barrierErrors.Add("Hücre 1-2-3 Işık Bariyeri İhlalde");
            errorList.Add(barrierErrors);

            var emergency = new List<string>();
            emergency.Add("");
            emergency.Add("Hücre 1-2 Acil Butonu Basılı");
            emergency.Add("Hücre 3 Acil Butonu Basılı");
            emergency.Add("Hücre 1-2-3 Acil Butonu Basılı");
            errorList.Add(emergency);

            var misc = new List<string>();
            misc.Add("");
            misc.Add("Bantlama Tarafı Işık Bariyeri İhlalde");
            misc.Add("Kapı Açık");
            misc.Add("Bantlama Tarafı Işık Bariyeri İhlalde - Kapı Açık");
            errorList.Add(misc);
        }

        #endregion

        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            ConnectionManager.Disconnect();
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
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Icon = new System.Drawing.Icon(References.ProjectPath + "Images\\t-ara128.ico");
            
            var n = 5;
            var n2 = String.Format("{0}", n);
            
            SetErrorTexts();

            systemControls.PalleteButton.ClickAction = () =>
            {
                // input is the list of options, change options here
                List<string> orders = ConnectionManager.Sql.GetOrders();
                List<string> zeroless = new List<string>();
                foreach (string order in orders)
                {
                    zeroless.Add(order.Substring(5));
                }
                pp.Opening(zeroless.ToArray());
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
                    int k = ConnectionManager.GetKatMax(product.GetHeight(), product.GetWidth(), product.GetYontem(),
                        product.GetProductType());
                    int l = ConnectionManager.GetPalletMax(product.GetHeight(), product.GetWidth(), product.GetYontem(),
                        product.GetProductType());
                    assignCell(no + 1, long.Parse(orders[pp.SelectedIndex]), p, l);
                    ConnectionManager.AssignCell(long.Parse(order), no+1, p, k, product);
                }
            };


            systemControls.ReconnectButton.ClickAction = () =>
            {
               ConnectionManager.Connect();
            };
            
            systemControls.AddProductButton.ClickAction = () =>
            {
                nbp.Opening();
                nbp.ShowDialog();
                if (nbp.confirmed)
                {
                    var info = nbp.GetLines;
                    ConnectionManager.EmptyCell(nbp.RobotNo-1);
                    emptyCell(nbp.RobotNo-1);
                    int k = ConnectionManager.GetPalletMax(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[4]), int.Parse(info[2]));
                    int l = ConnectionManager.GetKatMax(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[4]), int.Parse(info[2]));

                    ConnectionManager.AssignNonBarcodeCell(nbp.RobotNo, int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]), int.Parse(info[3]), info[4], int.Parse(info[5]),
                        int.Parse(info[6]), int.Parse(info[7]), l);
                    assignCell(nbp.RobotNo, 0, new Pallet(int.Parse(info[5]), int.Parse(info[6]), int.Parse(info[2]), int.Parse(info[3])), k);
                    ConnectionManager.PatternMode = true;
                }
            };

            systemControls.BypassButton.ClickAction = () =>
            {
                ConnectionManager.BypassTaper(!bypassing);
                bypassing = !bypassing;
            };

            servoControls.Implement(this.Controls);
            servoControls.applyPressed = () =>
            {
                ConnectionManager.SendServoAdjustments(servoControls.getValues());
                servoControls.ResetNumbers();
            };
            
            errorBox.Implement(this.Controls);

            errorBox._fixButton.ClickAction = () =>
            {
                var x = 0.0;
                foreach (var i in activeErrors)
                {
                    x = x + Math.Pow(2, i);
                }

                ConnectionManager.SendFixSignal(x);
            };

            systemControls.Implement(this.Controls);
            palleteVisuals.Implement(this.Controls);

            ConnectionManager.BarcodeConnectionChanged += barcodeIndicatorUpdater;
            ConnectionManager.PlcConnectionChanged += plcIndicatorUpdater;
            ConnectionManager.TaperConnectionChanged += taperIndicatorUpdater;
            ConnectionManager.ProductIncoming += productAdd;
            ConnectionManager.ProductDropped += productFill;
            ConnectionManager.CellFull += resetKat;
            ConnectionManager.CellAssigned += assignCell;
            ConnectionManager.ErrorUpdate += updateErrors;
            ConnectionManager.Init();
            LoadData();
            ConnectionManager.Connect();
            connectionIndicators.Implement(this.Controls);
            connectionIndicators.BarcodeConnect(ConnectionManager.BarcodeClient.Connected);
            connectionIndicators.PlcConnect(ConnectionManager.Plc.IsConnected);
            connectionIndicators.TaperConnect(ConnectionManager.Plc2.IsConnected);
            
            Resize += new EventHandler(Form2_Resize);
            
            bg.Reorient(0,0,appWidth,appHeight);
            Controls.Add(bg);
            
            var appRect = new Geometry.Rectangle(0f, ClientSize.Width, 0f, ClientSize.Height);
            
            var boxRect = appRect.RatedRectangle(appRatio);
            systemControls.resizeToWindowRect(boxRect);
            connectionIndicators.resizeToWindowRect(boxRect);
            palleteVisuals.resizeToWindowRect(boxRect);
            servoControls.resizeToWindowRect(boxRect);
            errorBox.resizeToWindowRect(boxRect);
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
        }

        private void barcodeIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.BarcodeConnect(ConnectionManager.BarcodeClient.Available(50));
        }

        private void plcIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.PlcConnect(ConnectionManager.Plc.IsConnected);
        }

        private void taperIndicatorUpdater(object sender, EventArgs e)
        {
            connectionIndicators.TaperConnect(ConnectionManager.TaperConnected);
        }
        private void productAdd(int r)
        {
            palleteVisuals.increaseProdCount(r, 1);
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
        }

        private void assignCell(int i, long orderNo, Pallet p, int katMax = 0)
        {
            var no = i-1;
            try
            {
                palleteVisuals.setPallette(no, orderNo.ToString(),p.GetHeight().ToString(),p.GetLength().ToString(),p.GetMax(), katMax);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void updateErrors(int[] errors)
        {
            var l = new List<String>();
            activeErrors.Clear();
            
            for (var i = 0; i < 9; i++)
            {
                var e = errorList[i][errors[i]];
                if (e!="") l.Add(e);
                activeErrors.Add(i);
            }
            errorBox.setErrorText(l);
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
                    var m = ConnectionManager.GetPalletMax(cell.Product.Height, cell.Product.Width, cell.Product.YontemKodu,
                        cell.Product.Type);
                    palleteVisuals.setPallette(cell.RobotNo-1, cell.OrderNo.ToString(), cell.PalletHeight.ToString(),cell.PalletWidth.ToString(),cell.OrderSize, m);
                    palleteVisuals.setProdCount(cell.RobotNo-1, valueDefn: cell.Holding, valueFill: cell.Dropped, pDef: cell.PHolding, pFill: cell.PDropped);
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
        
        private SystemControls systemControls = new SystemControls(3*appWidthInit/8, 50, 3*appWidthInit/4, 100,false);
        private ConnectionIndicators connectionIndicators = new ConnectionIndicators(7*appWidthInit/8, 50, appWidthInit/4, 100,false);
        private PalleteVisuals palleteVisuals = new PalleteVisuals(new Geometry.Rectangle(0f,appWidthInit,0f,appHeightInit),false);
        private ServoControls servoControls = new ServoControls(new Geometry.Rectangle(0f,appWidthInit,0f,appHeightInit),asVisual: false);
        private ErrorBox errorBox= new ErrorBox(new Geometry.Rectangle(0f,appWidthInit,0f,appHeightInit),asVisual: false);
        private Indicator bg  = new Indicator("bg", References.ProjectPath + "Images\\bg.png");
        private NonBarcodePopup nbp = new NonBarcodePopup();
        private PalletePopup pp = new PalletePopup();
    }
}