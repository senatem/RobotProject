using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EasyModbus;

namespace RobotProject.Form2Items
{
    public sealed class ConnectionManager
    {
        public readonly ModbusClient BarcodeClient = new ModbusClient();
        public readonly ModbusClient PlcClient = new ModbusClient();
        private readonly SqlCommunication _sql = new SqlCommunication();

        private Thread? _barcodeListener;

        private string? _receiveData;
        public string? Data;

        private List<Cell> _cells = new List<Cell>(3);
        private readonly OffsetCalculator _calculator = new OffsetCalculator();
        public event EventHandler BarcodeRead = null!;
        public event EventHandler BarcodeConnectionChanged = null!;
        public event EventHandler PlcConnectionChanged = null!;


        private void OnBarcodeRead(EventArgs e)
        {
            EventHandler handler = BarcodeRead;
            handler.Invoke(this, e);
        }

        private void OnBarcodeConnectionChanged(EventArgs e)
        {
            EventHandler handler = BarcodeConnectionChanged;
            handler.Invoke(this, e);
        }

        private void OnPlcConnectionChanged(EventArgs e)
        {
            EventHandler handler = PlcConnectionChanged;
            handler.Invoke(this, e);
        }

        public void Init()
        {
            BarcodeClient.ReceiveDataChanged += UpdateReceiveData;
            BarcodeClient.ConnectedChanged += UpdateBarcodeConnectedChanged;
            PlcClient.ConnectedChanged += UpdatePlcConnectedChanged;
        }

        public void Connect()
        {
            try
            {
                _sql.Connect();
                if (BarcodeClient.Connected) BarcodeClient.Disconnect();

                BarcodeClient.IPAddress = "192.168.0.100";
                BarcodeClient.Port = 51236;
                BarcodeClient.SerialPort = null;
                BarcodeClient.Connect();

                if (PlcClient.Connected) PlcClient.Disconnect();

                PlcClient.IPAddress = "192.168.0.1";
                PlcClient.Port = 502;
                PlcClient.SerialPort = null;
                PlcClient.Connect();

                _barcodeListener = new Thread(Listen);
                _barcodeListener.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand); //TODO barkod ve plc bağlantısı için hataları ayır, mesaj daha iyi olsun
            }
        }

        public void Disconnect()
        {
            BarcodeClient.Disconnect();
            PlcClient.Disconnect();
            _barcodeListener!.Abort();
        }

        private void Listen()
        {
            while (BarcodeClient.Connected)
            {
                ReadHoldingRegs(BarcodeClient);
                Thread.Sleep((1000));
            }
        }

        private void UpdateReceiveData(object sender)
        {
            _receiveData = BitConverter.ToString(BarcodeClient.receiveData).Replace("-", " ") + Environment.NewLine;
            new Thread(UpdateReceiveTextBox).Start();
        }

        private void UpdateReceiveTextBox()
        {
            Data = ConvertFromHex(_receiveData!.Trim());
            Interpret(Data);
            EventArgs args = new EventArgs();
            OnBarcodeRead(args);
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

        private void UpdateBarcodeConnectedChanged(object sender)
        {
            EventArgs args = new EventArgs();
            OnBarcodeConnectionChanged(args);
            Console.WriteLine(BarcodeClient.Connected
                ? @"Connected to the barcode reader!"
                : @"Disconnected from the barcode reader!");
        }

        private void UpdatePlcConnectedChanged(object sender)
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

        private Cell? GetCell(long type)
        {
            foreach (Cell cell in _cells)
            {
                if (cell.GetCellType() == type)
                {
                    return cell;
                }
            }

            return null;
        }

        private bool NotAssigned(long orderNo)
        {
            return GetCell(orderNo) == null;
        }

        private void AssignCell(long orderNo, int orderSize)
        {
            _cells.Add(new Cell(orderNo, orderSize, 0));
        }

        private void Interpret(string barcode)
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
                var product = _sql.Select("Siparis_No", orderNo);
                var orderNum = long.Parse(orderNo);
                if (product == null) return;
                if (NotAssigned(orderNum))
                {
                    var orderSize = product.GetOrderSize();
                    AssignCell(orderNum, orderSize);
                }

                var c = GetCell(orderNum)!;
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
                Offsets offsets = _calculator.Calculate(product.GetHeight(), product.GetWidth(), z, c.GetCounter(),
                    product.GetYontem(), product.GetProductType(), c.GetPalletHeight());

                //gerekli sinyaller gönderilir
                SendPlcSignals(_cells.IndexOf(c), offsets, product.GetHeight(), product.GetWidth(),
                    product.GetProductType(), c.GetCounter(), c.Full(), boxed);
                //cell, (x,y,z) offsets, dizilim şekli, en, boy, kat, tip, sayı, hücredolu, kutulu?
            }
        }

        private void SendPlcSignals(int cell, Offsets offsets, int px, int py, int type, int count, int cellFull,
            int boxed)
        {
            //PlcClient.Connect();
            int[] values =
            {
                cell, offsets.X, offsets.Y, offsets.Z, offsets.Pattern, px, py, offsets.Kat, type, count, cellFull,
                boxed
            };
            PlcClient.WriteMultipleRegisters(0, values);
        }
    }
}