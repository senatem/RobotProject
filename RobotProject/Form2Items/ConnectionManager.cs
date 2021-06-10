using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EasyModbus;

namespace RobotProject.Form2Items
{
    public class ConnectionManager
    {
        private readonly ModbusClient _barcodeClient = new ModbusClient();
        private readonly ModbusClient _plcClient = new ModbusClient();
        private readonly SqlCommunication _sql = new SqlCommunication();

        private Thread? _barcodeListener;

        private string? _receiveData;
        private string? _data;

        private List<Cell> _cells = new List<Cell>(3);

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

                _barcodeClient.IPAddress = "tbd"; //TODO barcode ip ve portunu al
                _barcodeClient.Port = 0;
                _barcodeClient.SerialPort = null;
                _barcodeClient.Connect();

                if (_plcClient.Connected) _plcClient.Disconnect();

                _plcClient.IPAddress = "tbd"; //TODO plc ip ve portunu al
                _plcClient.Port = 0;
                _plcClient.SerialPort = null;
                _plcClient.Connect();

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
            Console.WriteLine("Barcode data = " + _data);
            //Interpret(_data);
        }

        private static string ConvertFromHex(string hexString)
        {
            char[] output = new char[15];
            string[] toClean = hexString.Split(' ');

            for (var i = 0; i < toClean.Length; i++)
            {
                if (i < 15)
                    output[i] = (char) (Convert.ToInt32(toClean[i], 16));
            }

            return new string(output);
        }

        private void UpdateBarcodeConnectedChanged(object sender)
        {
            if (_barcodeClient.Connected)
            {
                //TODO connection indicator green (raise event)
            }
            else
            {
                //todo connection indicator red
            }
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
                Product product = _sql.Select("id", barcode);
                var type = product.GetProductType();
                if (NotAssigned(type))
                {
                    var orderSize = product.GetOrderSize();
                    _cells = (List<Cell>) AssignCell(type, orderSize);
                }
                GetCell(product.GetProductType())!.AddProduct();
                //gerekli sinyaller gönderilir
                
            }
        }

        private void SendPlcSignals()
        {
            _plcClient.Connect();
            
        }
    }
}