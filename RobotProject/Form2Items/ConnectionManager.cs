using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EasyModbus;

namespace RobotProject.Form2Items
{
    public class BarcodeReadEventArgs : EventArgs
    {
        public BarcodeReadEventArgs(string barcode)
        {
            Barcode = barcode;
        }

        public string Barcode { get; set; }
    }
    public class ConnectionManager
    {
        private const int Z = 0;
        private readonly ModbusClient _barcodeClient = new ModbusClient();
        private readonly ModbusClient _plcClient = new ModbusClient();
        private readonly SqlCommunication _sql = new SqlCommunication();

        private Thread? _barcodeListener;

        private string? _receiveData;
        private string? _data;

        private List<Cell> _cells = new List<Cell>(3);
        private readonly OffsetCalculator _calculator = new OffsetCalculator();
        public event EventHandler BarcodeRead;

        public delegate void BarcodeReadEventHandler(object sender, BarcodeReadEventArgs e);


        protected virtual void OnBarcodeRead(BarcodeReadEventArgs e)
        {
            EventHandler handler = BarcodeRead;
            handler.Invoke(this, e);
        }

        public void Init()
        {
            _barcodeClient.ReceiveDataChanged += UpdateReceiveData;
            _barcodeClient.ConnectedChanged += UpdateBarcodeConnectedChanged;
            _plcClient.ConnectedChanged += UpdatePlcConnectedChanged;
        }

        public void Connect()
        {
            try
            {
                _sql.Connect();
                if (_barcodeClient.Connected) _barcodeClient.Disconnect();

                _barcodeClient.IPAddress = "192.168.0.100"; //TODO barcode ip ve portunu al
                _barcodeClient.Port = 51236;
                _barcodeClient.SerialPort = null;
                _barcodeClient.Connect();

                if (_plcClient.Connected) _plcClient.Disconnect();

                _plcClient.IPAddress = "tbd"; //TODO plc ip ve portunu al
                _plcClient.Port = 0;
                _plcClient.SerialPort = null;
             //   _plcClient.Connect();

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
            _barcodeClient.Disconnect();
            _plcClient.Disconnect();
        }

        private void Listen()
        {
            while (_barcodeClient.Connected)
            {
                ReadHoldingRegs(_barcodeClient);
                Thread.Sleep((1000));
            }
        }

        private void UpdateReceiveData(object sender)
        {
            _receiveData = BitConverter.ToString(_barcodeClient.receiveData).Replace("-", " ") + Environment.NewLine;
            new Thread(UpdateReceiveTextBox).Start();
        }

        private void UpdateReceiveTextBox()
        {
            _data = ConvertFromHex(_receiveData!.Trim());
            Interpret(_data);
            BarcodeReadEventArgs args = new BarcodeReadEventArgs(_data);
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
            Console.WriteLine(_barcodeClient.Connected
                ? @"Connected to the barcode reader!"
                : @"Disconnected from the barcode reader!");
        }

        private void UpdatePlcConnectedChanged(object sender)
        {
            if (_plcClient.Connected)
            {
                //TODO connection indicator green
            }
            else
            {
                //todo connection indicator red
            }
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

        public void SendDataToPlc(int address, int data)
        {
            try
            {
                _plcClient.Connect();
                _plcClient.WriteSingleRegister(address, data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private Cell? GetCell(int type)
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

        private bool NotAssigned(int type)
        {
            return GetCell(type) == null;
        }

        private IEnumerable<Cell> AssignCell(int type, int orderSize)
        {
            return _cells.Append(new Cell(type, orderSize));
        }
        
        private void Interpret(string barcode)
        {
            if (barcode.IndexOf('?') != -1) {
                //barkod okunamadı
            }
            else
            {
                var orderNo = barcode.Split(',')[0].Split('S')[1];
                Product product = _sql.Select("Siparis_No", orderNo);
                var type = product.GetProductType();
                if (NotAssigned(type))
                {
                    var orderSize = product.GetOrderSize();
                    _cells = (List<Cell>) AssignCell(type, orderSize);
                }

                var c = GetCell(product.GetProductType())!;
                c.AddProduct();
                
                
                //offset hesapları
                Offsets offsets = _calculator.Calculate(product.GetHeight(), product.GetWidth(), Z, c.GetCounter(), product.GetYontem(), product.GetProductType());

                var boxed = 0;
                if (product.GetYontem() == 156)
                {
                    boxed = 1;
                }
                //gerekli sinyaller gönderilir
                SendPlcSignals(_cells.IndexOf(c), offsets, product.GetHeight(), product.GetWidth(), product.GetProductType(), c.GetCounter(), c.Full(), boxed);
                //cell, (x,y,z) offsets, dizilim şekli, en, boy, kat, tip, sayı, hücredolu, kutulu?
            }
        }

        private void SendPlcSignals(int cell, Offsets offsets, int px, int py, int type, int count, int cellFull, int boxed)
        {
            _plcClient.Connect();
            int[] values = {cell, offsets._x, offsets._y, offsets._z, offsets._pattern, px, py, offsets._kat, type, count, cellFull, boxed};
            _plcClient.WriteMultipleRegisters(0, values);
            
            
        }
    }
}