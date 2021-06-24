using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;

namespace RobotProject.Form2Items
{
    #region delegates

    public delegate void ProductIncoming(int r);

    public delegate void CellFull(int i);

    #endregion

    public static class ConnectionManager
    {
        #region declarations

        public static readonly ModbusClient BarcodeClient = new ModbusClient();
        public static readonly ModbusClient PlcClient = new ModbusClient();
        public static readonly ModbusClient PlcClient2 = new ModbusClient();
        public static readonly SqlCommunication Sql = new SqlCommunication();

        private static string? _receiveData;
        private static string? _data;
        private static readonly List<Cell> Cells = new List<Cell>(3);
        private static readonly OffsetCalculator Calculator = new OffsetCalculator();

        public static event EventHandler BarcodeRead = null!;
        public static event EventHandler BarcodeConnectionChanged = null!;
        public static event EventHandler PlcConnectionChanged = null!;
        public static event ProductIncoming ProductIncoming = null!;
        public static event CellFull CellFull = null!;

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

        private static void OnBarcodeConnectionChanged(EventArgs e)
        {
            EventHandler handler = BarcodeConnectionChanged;
            handler.Invoke(null, e);
        }

        private static void OnPlcConnectionChanged(EventArgs e)
        {
            EventHandler handler = PlcConnectionChanged;
            handler.Invoke(null, e);
        }

        private static void UpdateReceiveData(object sender)
        {
            _receiveData = BitConverter.ToString(BarcodeClient.receiveData).Replace("-", " ") + Environment.NewLine;
            Parallel.Invoke(UpdateReceiveTextBox);
        }

        private static void UpdateBarcodeConnectedChanged(object sender)
        {
            EventArgs args = new EventArgs();
            OnBarcodeConnectionChanged(args);
        }

        private static void UpdatePlcConnectedChanged(object sender)
        {
            EventArgs args = new EventArgs();
            OnPlcConnectionChanged(args);
        }

        private static void OnBarcodeRead()
        {
            BarcodeRead(null, EventArgs.Empty);
        }

        #endregion

        #region connections

        public static void Init()
        {
            BarcodeClient.ReceiveDataChanged += UpdateReceiveData;
            BarcodeClient.ConnectedChanged += UpdateBarcodeConnectedChanged;
            PlcClient.ConnectedChanged += UpdatePlcConnectedChanged;
            PlcClient2.ReceiveDataChanged += ReadFromPlc;
        }

        public static void Connect()
        {
            Sql.Connect();

            try
            {
                if (BarcodeClient.Connected) BarcodeClient.Disconnect();

                BarcodeClient.IPAddress = "192.168.0.100";
                BarcodeClient.Port = 51236;
                BarcodeClient.SerialPort = null;
                BarcodeClient.Connect();

                Task.Run(Listen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: Barkod okuyucu." + ex.Message, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }

            try
            {
                if (PlcClient.Connected) PlcClient.Disconnect();

                PlcClient.IPAddress = "192.168.0.1";
                PlcClient.Port = 502;
                PlcClient.SerialPort = null;
                PlcClient.Connect();

                if (PlcClient2.Connected) PlcClient2.Disconnect();

                PlcClient2.IPAddress = "192.168.0.50";
                PlcClient2.Port = 502;
                PlcClient2.SerialPort = null;
                PlcClient2.Connect();

                //int[] v = {15};
                //PlcClient2.WriteMultipleRegisters(2,v);

                Task.Run(ListenPlc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: PLC." + ex.Message, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }
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
                var t = client.ReadHoldingRegisters(0, 1);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        private static void ReadFromPlc(object sender)
        {
            _receiveData = BitConverter.ToString(PlcClient.receiveData).Replace("-", " ");
            var orderNo = long.Parse(_receiveData);
            //MessageBox.Show(_receiveData);
            ProcessOrder(orderNo);
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
        }

        #endregion

        #region listeners

        private static void ListenPlc()
        {
            while (PlcClient2.Connected)
            {
                ReadHoldingRegsPlc(PlcClient2);
                Task.Delay(1000, CancellationToken.None);
            }
        }

        private static void Listen()
        {
            while (BarcodeClient.Connected)
            {
                ReadHoldingRegsBarcode(BarcodeClient);
                Task.Delay(1000, CancellationToken.None);
            }
        }

        #endregion

        #region barcode

        private static void UpdateReceiveTextBox()
        {
            _data = ConvertFromHex(_receiveData!.Trim());
            Interpret(_data);
            OnBarcodeRead();
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

        private static void ProcessOrder(long orderNum)
        {
            var product = Sql.Select("Siparis_No", orderNum.ToString());
            
            if (product == null) return;
            var c = GetCell(orderNum);

            if (c == null) return;
            c.AddProduct();

            var boxed = 0;
            if (product.GetYontem() == 156)
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
                OnCellFull(cNo-1);
            }

            //offset hesapları
            Offsets offsets = Calculator.Calculate(product.GetHeight(), product.GetWidth(), z, c.GetCounter(),
                product.GetYontem(), product.GetProductType(), c.GetPalletHeight());

            //gerekli sinyaller gönderilir
            SendPlcSignals(cNo, offsets, product.GetHeight(), product.GetWidth(),
                product.GetProductType(), c.GetCounter(), c.Full(), boxed);
            //cell, (x,y,z) offsets, dizilim şekli, en, boy, kat, tip, sayı, hücredolu, kutulu?
            ProductAdd(cNo);
        }

        #endregion

        #region control

        private static Cell? GetCell(long type)
        {
            return Cells.FirstOrDefault(cell => cell.GetCellType() == type);
        }

        public static void AssignCell(long orderNo, int robotNo)
        {
            var orderSize = Sql.GetOrderSize(orderNo);
            Cells.Add(new Cell(orderNo, robotNo, orderSize, 140));
        }

        public static void EmptyCell(int i)
        {
            var c = Cells.Find(cell => cell.GetRobotNo() == i);
            Cells.Remove(c);
        }

        #endregion
    }
}