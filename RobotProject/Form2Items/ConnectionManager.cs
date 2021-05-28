using System;
using System.Threading;
using System.Windows.Forms;
using EasyModbus;

namespace RobotProject
{
    public class ConnectionManager
    {
        private readonly ModbusClient _barcodeClient = new ModbusClient();
        private readonly ModbusClient _plcClient = new ModbusClient();

        private Thread? _barcodeListener;

        private string? _receiveData;
        private string? _data;

//        private readonly ModbusClient _bantClient = new ModbusClient(); //TODO enine bantlama bağlantısını koy

        private void Init()
        {
            _barcodeClient.ReceiveDataChanged += UpdateReceiveData;
            _barcodeClient.ConnectedChanged += UpdateBarcodeConnectedChanged;
            _plcClient.ConnectedChanged += UpdatePlcConnectedChanged;
        }

        private void Connect()
        {
            try
            {
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
                MessageBox.Show(ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand); //TODO barkod ve plc bağlantısı için hataları ayır, mesaj daha iyi olsun
            }
        }

        private void Disconnect()
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
            _data = ConvertFromHex(_receiveData.Trim());
            Interpret(_data);
        }

        private string ConvertFromHex(string hexString)
        {
            char[] output = new char[15];
            string[] toClean = hexString.Split(' ');

            for (int i = 0; i < toClean.Length; i++)
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
                //TODO connection indicator green
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

        private void ReadHoldingRegs(ModbusClient client)
        {
            try
            {
                client.ReadHoldingRegisters(0, 0);
            }
            catch (Exception)
            {
                
            }
        }

        private void Interpret(string barcode)
        {
            
        }
    }
}