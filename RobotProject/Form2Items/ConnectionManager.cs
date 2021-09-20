using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    #region delegates

    public delegate void ProductIncoming(int r);

    public delegate void CellFull(int i);

    public delegate void CellAssigned(int i, long orderNo, Pallet pallet);

    #endregion

    public static class ConnectionManager
    {
        #region declarations

        public static readonly ModbusClient BarcodeClient = new ModbusClient();
        public static readonly ModbusClient PlcClient = new ModbusClient();
        public static readonly ModbusClient PlcClient2 = new ModbusClient();
        public static readonly SqlCommunication Sql = new SqlCommunication();

        public static bool PlcConnected;
        public static bool BarcodeConnected;
        public static bool TaperConnected;
        private static string? _receiveData;
        private static string? _plcData;
        private static string? _data;
        public static List<Cell> Cells = new List<Cell>(3);
        private static readonly OffsetCalculator Calculator = new OffsetCalculator();
        private static readonly ExcelReader _weights = new ExcelReader(References.ProjectPath + "Weights.xlsx");

        public static event EventHandler BarcodeConnectionChanged = null!;
        public static event EventHandler PlcConnectionChanged = null!;
        public static event EventHandler TaperConnectionChanged = null!;
        public static event ProductIncoming ProductIncoming = null!;
        public static event CellFull CellFull = null!;
        public static event CellAssigned CellAssigned = null!;

        #endregion

        #region events

        private static void ProductAdd(int r)
        {
            ProductIncoming.Invoke(r);
        }

        private static void OnCellFull(int i)
        {
            CellFull.Invoke(i);
        }

        private static void OnCellAssigned(int i, long orderNo, Pallet pallet)
        {
            CellAssigned.Invoke(i, orderNo, pallet);
        }

        private static void OnPlcConnectionChanged(EventArgs e)
        {
            EventHandler handler = PlcConnectionChanged;
            handler.Invoke(null, e);
        }

        private static void OnBarcodeConnectionChanged(EventArgs e)
        {
            EventHandler handler = BarcodeConnectionChanged;
            handler.Invoke(null, e);
        }

        private static void OnTaperConnectionChanged(EventArgs e)
        {
            EventHandler handler = TaperConnectionChanged;
            handler.Invoke(null, e);
        }

        private static void UpdateReceiveData(object sender)
        {
            _receiveData = BitConverter.ToString(BarcodeClient.receiveData).Replace("-", " ") + Environment.NewLine;
            Parallel.Invoke(UpdateReceiveTextBox);
        }

        private static void ReadFromPlc(object sender)
        {
            _plcData = PlcClient.receiveData[10].ToString();
            Parallel.Invoke(UpdatePlcData);
        }

        #endregion

        #region connections

        public static void Init()
        {
            BarcodeClient.ReceiveDataChanged += UpdateReceiveData;
            PlcClient.ReceiveDataChanged += ReadFromPlc;
        }

        private static void ConnectBarcode()
        {
            if (BarcodeClient.Connected) BarcodeClient.Disconnect();

            BarcodeClient.IPAddress = "192.168.0.100";
            BarcodeClient.Port = 51236;
            BarcodeClient.SerialPort = null;
            BarcodeConnected = true;
            try
            {
                BarcodeClient.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: Barkod okuyucu." + ex.Message, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
                BarcodeConnected = false;
            }

            Task.Run(Listen);
            EventArgs args = new EventArgs();
            OnBarcodeConnectionChanged(args);
        }

        private static void ConnectPlc()
        {
            if (PlcClient.Connected) PlcClient.Disconnect();

            PlcClient.IPAddress = "192.168.0.1";
            PlcClient.Port = 502;
            PlcClient.SerialPort = null;

            try
            {
                PlcClient.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: Plc." + ex.Message, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
                PlcConnected = false;
            }

            Task.Run(ListenPlc);
            EventArgs args = new EventArgs();
            OnPlcConnectionChanged(args);
        }

        private static void ConnectTaper()
        {
            if (PlcClient2.Connected) PlcClient2.Disconnect();
            PlcClient2.IPAddress = "192.168.0.50";
            PlcClient2.Port = 502;
            PlcClient2.SerialPort = null;
            try
            {
                PlcClient2.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: Enine Bantlama Makinesi." + ex.Message, @"Bağlantı Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }

            TaperConnected = PlcClient2.Available(100);
            Task.Run(ListenTaper);
            EventArgs args = new EventArgs();
            OnTaperConnectionChanged(args);
        }

        public static void Connect()
        {
            Sql.Connect();

            ConnectBarcode();
            ConnectPlc();
            ConnectTaper();
        }

        public static void Disconnect()
        {
            BarcodeClient.Disconnect();
            PlcClient.Disconnect();
        }

        private static void ReadHoldingRegsBarcode(ModbusClient client)
        {
            try
            {
                client.ReadHoldingRegisters(0, 0);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void ReadHoldingRegsPlc(ModbusClient client)
        {
            try
            {
                client.ReadHoldingRegisters(16, 1);
            }
            catch (Exception)
            {
                // ignored
            }
        }


        private static void SendPlcSignals(int cell, Offsets offsets, int px, int py, int type, int count, int cellFull,
            int boxed)
        {
            int[] values =
            {
                cell, offsets.X, offsets.Y, offsets.Z, offsets.Pattern, px, py, offsets.Kat, type, count, cellFull,
                boxed
            };
            PlcClient.WriteMultipleRegisters(0, values);
            PlcClient.WriteSingleRegister(15, offsets.Rotation);
        }

        #endregion

        #region listeners

        private static async Task ListenPlc()
        {
            while (true)
            {
                ReadHoldingRegsPlc(PlcClient);
                await Task.Delay(1000, CancellationToken.None);
            }
        }

        private static async Task ListenTaper()
        {
            while (true)
            { await Task.Delay(1000, CancellationToken.None);
            }
        }

        private static async Task Listen()
        {
            while (true)
            {
                ReadHoldingRegsBarcode(BarcodeClient);
                await Task.Delay(1000, CancellationToken.None);
            }
        }

        #endregion

        #region barcode

        private static void UpdateReceiveTextBox()
        {
            _data = ConvertFromHex(_receiveData!.Trim());
            Interpret(_data);
        }

        private static void UpdatePlcData()
        {
            var productComing = int.Parse(_plcData);

            if (productComing == 1)
            {
                ProcessOrder(GetCellTwoOrder());
            }
        }

        private static string ConvertFromHex(string hexString)
        {
            char[] output = new char[30];
            string[] toClean = hexString.Split(' ');

            for (var i = 0; i < toClean.Length; i++)
            {
                if (i < 30)
                    output[i] = (char) Convert.ToByte(toClean[i], 16);
            }

            return new string(output);
        }

        private static void Interpret(string barcode)
        {
            if (barcode.IndexOf('S') == -1)
            {
                //barkod okunamadı
            }
            else
            {
                var orderNo = "";
                var b = barcode.Split(';');

                foreach (var sub in b)
                {
                    if (!sub.Contains('S')) continue;
                    orderNo = sub.Split('S')[1].Substring(0, 7);
                    break;
                }

                var orderNum = long.Parse(orderNo);
                ProcessOrder(orderNum);
            }
        }
        
        private static bool IsHeavy (Product product)
        {
            string[] fields = {"Tip", "Yukseklik", "Uzunluk"};
            var px = product.GetHeight();
            var py = product.GetWidth();
            if (product.GetProductType() == 33 && (py - py % 100) >= 1800)
            {
                return true;
            }
            int[] values = {product.GetProductType(), px - px % 100, py - py % 100};
            var weight = (double) _weights.Find(fields, values).Rows[0]["Brut"];

            return weight >= 50;
        }

        private static void ProcessOrder(long orderNum)
        {
            var product = Sql.Select("Siparis_No", orderNum.ToString());

            if (product == null) return;
            
            
            var c = GetCell(orderNum);

            if (c == null && Cells.Count < 3)
            {
                var p = Sql.GetPallet(orderNum.ToString());
                if (IsHeavy(product))
                {
                    AssignCell(orderNum, 2, p);
                    OnCellAssigned(2, orderNum, p);
                }
                else
                {
                    AssignCell(orderNum, Cells.Count+1, p);
                    OnCellAssigned(Cells.Count, orderNum, p);
                }
            } else if (c == null && Cells.Count == Cells.Capacity)
            {
                return;
            }
            
            c = GetCell(orderNum);
            c.AddProduct();

            var boxed = 0;
            if (product.GetYontem() == 156 || product.GetYontem() == 223)
            {
                boxed = 1;
            }

            var a = product.GetProductType();
            var z = 0;
            if (boxed == 1)
            {
                z = a switch
                {
                    11 => 90,
                    21 => 90,
                    22 => 124,
                    33 => 180,
                    _ => z
                };
            }
            else
            {
                z = a switch
                {
                    11 => 74,
                    21 => 76,
                    22 => 108,
                    33 => 164,
                    _ => z
                };
            }

            var cNo = c.GetRobotNo();

            if (c.Full() == 1)
            {
                OnCellFull(cNo - 1);
            }

            //offset hesapları
            if (boxed == 1)
            {
                Offsets offsets = Calculator.Calculate(product.GetHeight() + 80, product.GetWidth() + 80, z,
                    c.GetCounter(),
                    product.GetYontem(), product.GetProductType(), c.GetPalletHeight(), c.GetPalletWidth());
                //gerekli sinyaller gönderilir
                SendPlcSignals(cNo, offsets, product.GetHeight(), product.GetWidth(),
                    product.GetProductType(), c.GetCounter(), c.Full(), boxed);
            }
            else
            {
                Offsets offsets = Calculator.Calculate(product.GetHeight(), product.GetWidth(), z, c.GetCounter(),
                    product.GetYontem(), product.GetProductType(), c.GetPalletHeight(), c.GetPalletWidth());
                //gerekli sinyaller gönderilir
                SendPlcSignals(cNo, offsets, product.GetHeight(), product.GetWidth(),
                    product.GetProductType(), c.GetCounter(), c.Full(), boxed);
            }

            //cell, (x,y,z) offsets, dizilim şekli, en, boy, kat, tip, sayı, hücredolu, kutulu?
            ProductAdd(cNo);
        }

        #endregion

        #region control

        private static Cell? GetCell(long type)
        {
            return Cells.FirstOrDefault(cell => cell.GetCellType() == type);
        }

        private static long GetCellTwoOrder()
        {
            return Cells.Find(cell => cell.GetRobotNo() == 2).OrderNo;
        }

        public static void AssignCell(long orderNo, int robotNo,Pallet pallet)
        {
            var orderSize = Sql.GetOrderSize(orderNo);
            Cells.Add(new Cell(orderNo, robotNo, orderSize, pallet.GetHeight(), pallet.GetLength()));
        }

        public static void EmptyCell(int i)
        {
            var c = Cells.Find(cell => cell.GetRobotNo() == i + 1);
            Cells.Remove(c);
        }

        #endregion
    }
}