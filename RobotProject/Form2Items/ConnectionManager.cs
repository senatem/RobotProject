using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EasyModbus;

namespace RobotProject.Form2Items
{
    public static class ConnectionManager
    {
        public static readonly ModbusClient BarcodeClient = new ModbusClient();
        public static readonly ModbusClient PlcClient = new ModbusClient();
        private static readonly SqlCommunication Sql = new SqlCommunication();

        private static Thread? _barcodeListener;

        private static string? _receiveData;
        private static string? _data;
        public static BoxVisuals? Bv = null;

        private static readonly List<Cell> Cells = new List<Cell>(3);
        private static readonly OffsetCalculator Calculator = new OffsetCalculator();
        public static event EventHandler BarcodeRead = null!;
        public static event EventHandler BarcodeConnectionChanged = null!;
        public static event EventHandler PlcConnectionChanged = null!;


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

        public static void Init()
        {
            BarcodeClient.ReceiveDataChanged += UpdateReceiveData;
            BarcodeClient.ConnectedChanged += UpdateBarcodeConnectedChanged;
            PlcClient.ConnectedChanged += UpdatePlcConnectedChanged;
        }

        public static void Connect()
        {
            try
            {
                Sql.Connect();
                if (BarcodeClient.Connected) BarcodeClient.Disconnect();

                BarcodeClient.IPAddress = "192.168.0.100";
                BarcodeClient.Port = 51236;
                BarcodeClient.SerialPort = null;
                BarcodeClient.Connect();

                _barcodeListener = new Thread(Listen);
                _barcodeListener.Start();
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
            if(_barcodeListener!=null) _barcodeListener!.Abort();
        }

        private static void Listen()
        {
            while (BarcodeClient.Connected)
            {
                ReadHoldingRegs(BarcodeClient);
                Thread.Sleep((1000));
            }
        }

        private static void UpdateReceiveData(object sender)
        {
            _receiveData = BitConverter.ToString(BarcodeClient.receiveData).Replace("-", " ") + Environment.NewLine;
            new Thread(UpdateReceiveTextBox).Start();
        }

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

        private static void UpdateBarcodeConnectedChanged(object sender)
        {
            EventArgs args = new EventArgs();
            OnBarcodeConnectionChanged(args);
            Console.WriteLine(BarcodeClient.Connected
                ? @"Connected to the barcode reader!"
                : @"Disconnected from the barcode reader!");
        }

        private static void UpdatePlcConnectedChanged(object sender)
        {
            EventArgs args = new EventArgs();
            OnPlcConnectionChanged(args);
            Console.WriteLine(PlcClient.Connected
                ? @"Connected to the plc reader!"
                : @"Disconnected from the plc reader!");
        }

        private static void ReadHoldingRegs(ModbusClient client)
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

        private static Cell? GetCell(long type)
        {
            return Cells.FirstOrDefault(cell => cell.GetCellType() == type);
        }

        public static void AssignCell(int orderNo, int robotNo)
        {
            var orderSize = Sql.GetOrderSize(orderNo);
            Cells[robotNo] = new Cell(orderNo, orderSize, 0);
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
                if (b.Length > 1)
                {
                    foreach (var sub in b)
                    {
                        if (!sub.Contains('S')) continue;
                        orderNo = sub.Split('S')[1];
                        break;
                    }
                }

                var product = Sql.Select("Siparis_No", orderNo);
                var orderNum = long.Parse(orderNo);
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


                //offset hesapları
                Offsets offsets = Calculator.Calculate(product.GetHeight(), product.GetWidth(), z, c.GetCounter(),
                    product.GetYontem(), product.GetProductType(), c.GetPalletHeight());

                //gerekli sinyaller gönderilir
                SendPlcSignals(Cells.IndexOf(c), offsets, product.GetHeight(), product.GetWidth(),
                    product.GetProductType(), c.GetCounter(), c.Full(), boxed);
                //cell, (x,y,z) offsets, dizilim şekli, en, boy, kat, tip, sayı, hücredolu, kutulu?
                Bv!.AddToBoxes(new SingleBox(orderNo, product.GetHeight().ToString(), product.GetWidth().ToString(),
                   false, Cells.IndexOf(c)));
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
        }

        private static void OnBarcodeRead()
        {
            BarcodeRead(null, EventArgs.Empty);
        }

        public static void EmptyCell(int i)
        {
            Cells.Remove(Cells[i]);
        }
    }
}