using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    public class Signal
    {
        public readonly int Cell;
        public readonly Offsets Offsets;
        public readonly int Px;
        public readonly int Py;
        public readonly int Type;
        public readonly int Count;
        public readonly int CellFull;
        public readonly int Boxed;

        public Signal(int cell, Offsets offsets, int px, int py, int type, int count, int cellFull, int boxed)
        {
            Cell = cell;
            Offsets = offsets;
            Px = px;
            Py = py;
            Type = type;
            Count = count;
            CellFull = cellFull;
            Boxed = boxed;
        }
    }

    #region delegates

    public delegate void ProductIncoming(int r);

    public delegate void ProductDropped(int r);

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
        private static string? _taken;
        private static string? _droppedFirst;
        private static string? _droppedSecond;
        private static string? _droppedThird;
        private static string? _data;
        private static bool _inProcess;
        public static bool PatternMode;
        public static Product? PatternProduct;
        private static int patternLast;
        public static List<Cell> Cells = new List<Cell>(3);
        private static readonly OffsetCalculator Calculator = new OffsetCalculator();
        private static readonly ExcelReader Weights = new ExcelReader(References.ProjectPath + "Weights.xlsx");
        private static long[] _times = new long[5];
        private static int _productComing;
        private static CancellationTokenSource _cancelPlcSource = new CancellationTokenSource();
        private static CancellationToken _cancelPlc = _cancelPlcSource.Token;
        private static CancellationTokenSource _cancelBarcodeSource = new CancellationTokenSource();
        private static CancellationToken _cancelBarcode = _cancelBarcodeSource.Token;
        private static string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string fileName = "plcLog" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

        public static event EventHandler BarcodeConnectionChanged = null!;
        public static event EventHandler PlcConnectionChanged = null!;
        public static event EventHandler TaperConnectionChanged = null!;
        public static event ProductIncoming ProductIncoming = null!;
        public static event ProductDropped ProductDropped = null!;
        public static event CellFull CellFull = null!;
        public static event CellAssigned CellAssigned = null!;

        #endregion

        #region events

        private static void ProductAdd(int r)
        {
            ProductIncoming.Invoke(r);
        }

        private static void ProductDrop(int r)
        {
            ProductDropped.Invoke(r);
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
            _plcData = PlcClient.receiveData[18].ToString();
            _taken = PlcClient.receiveData[64].ToString();
            _droppedFirst = PlcClient.receiveData[10].ToString();
            _droppedSecond = PlcClient.receiveData[12].ToString();
            _droppedThird = PlcClient.receiveData[14].ToString();

            File.AppendAllText(Path.Combine(docPath, fileName),
                "Time: " + DateTime.Now + " PlcData: " + _plcData + "\n");

            Parallel.Invoke(UpdatePlcData);
        }

        #endregion

        #region connections

        public static void Init()
        {
            BarcodeClient.ReceiveDataChanged += UpdateReceiveData;
            PlcClient.ReceiveDataChanged += ReadFromPlc;
            Buffer.Init();
            File.WriteAllText(Path.Combine(docPath, fileName), "");
        }

        private static void ConnectBarcode()
        {
            if (BarcodeClient.Connected) BarcodeClient.Disconnect();

            try
            {
                _cancelBarcodeSource.Cancel();
            }
            catch (Exception e)
            {
                //ignored
            }

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

            _cancelBarcodeSource = new CancellationTokenSource();
            _cancelBarcode = _cancelBarcodeSource.Token;
            Task.Run(Listen, _cancelBarcode);
            EventArgs args = new EventArgs();
            OnBarcodeConnectionChanged(args);
        }

        private static void ConnectPlc()
        {
            if (PlcClient.Connected) PlcClient.Disconnect();

            try
            {
                _cancelPlcSource.Cancel();
            }
            catch (Exception e)
            {
                //ignored
            }

            PlcClient.IPAddress = "192.168.0.1";
            PlcClient.Port = 502;
            PlcClient.SerialPort = null;
            PlcConnected = true;
            PlcClient.ConnectionTimeout = 1000000000;

            try
            {
                PlcClient.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: Plc. " + ex.Message + "\n" + ex.StackTrace, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
                PlcConnected = false;
            }


            _cancelPlcSource = new CancellationTokenSource();
            _cancelPlc = _cancelPlcSource.Token;
            Task.Run(ListenPlc, _cancelPlc);
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
                client.ReadHoldingRegisters(12, 28);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static async void SendSignal(int cell, Offsets offsets, int px, int py, int type, int count,
            int cellFull,
            int boxed)
        {
            int[] values =
            {
                cell, offsets.X, offsets.Y, offsets.Z, offsets.Pattern, px, py, offsets.Kat, type, count, cellFull,
                boxed
            };
            try
            {
                PlcClient.Connect();
                await Task.Delay(100, CancellationToken.None);
                PlcClient.WriteMultipleRegisters(0, values);
                PlcClient.WriteSingleRegister(15, offsets.Rotation);
                PlcClient.WriteSingleRegister(17, offsets.NextRotation);
                await Task.Delay(100, CancellationToken.None);
                PlcClient.Disconnect();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private static void SendFromBuffer(int r)
        {
            if (Buffer.Empty(r)) return;
            _inProcess = true;
            var s = Buffer.Pop(r);
            SendSignal(s.Cell, s.Offsets, s.Px, s.Py, s.Type, s.Count, s.CellFull, s.Boxed);
        }

        private static void SendPlcSignals(Signal s)
        {
            if (_inProcess)
            {
                Buffer.Add(s, 0);
            }
            else
            {
                _inProcess = true;
                SendSignal(s.Cell, s.Offsets, s.Px, s.Py, s.Type, s.Count, s.CellFull, s.Boxed);
            }
        }

        private static async void ResetRobotOffsets()
        {
            try
            {
                PlcClient.Connect();
                await Task.Delay(100, CancellationToken.None);
                PlcClient.WriteMultipleRegisters(0, new int[12]);
                PlcClient.WriteSingleRegister(15, 0);
                PlcClient.WriteSingleRegister(17, 0);
                await Task.Delay(100, CancellationToken.None);
                PlcClient.Disconnect();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        #endregion

        #region listeners

        private static async Task ListenPlc()
        {
            while (true)
            {
                PlcClient.Disconnect();
                PlcClient.Connect();
                if (PlcClient.Available(50))
                {
                    await Task.Delay(100, CancellationToken.None);
                    ReadHoldingRegsPlc(PlcClient);
                    await Task.Delay(100, CancellationToken.None);
                    PlcClient.Disconnect();
                    if (_cancelPlc.IsCancellationRequested)
                    {
                        return;
                    }
                }
                else
                {
                    ConnectPlc();

                    EventArgs args = new EventArgs();
                    OnPlcConnectionChanged(args);
                    return;
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static async Task Listen()
        {
            while (true)
            {
                BarcodeClient.Disconnect();
                BarcodeClient.Connect();
                await Task.Delay(100, CancellationToken.None);
                //      if (BarcodeClient.Available(50))
                //    {
                ReadHoldingRegsBarcode(BarcodeClient);
                await Task.Delay(100, CancellationToken.None);
                BarcodeClient.Disconnect();
                if (_cancelBarcode.IsCancellationRequested)
                {
                    return;
                }

                /*         }
                         else
                         {
                             ConnectBarcode();
                             EventArgs args = new EventArgs();
                             OnBarcodeConnectionChanged(args);
                             return;
                         }
                         */
            }
            // ReSharper disable once FunctionNeverReturns
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
            if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _times[0]) > 1000)
            {
                _productComing = int.Parse(_plcData!);

                if (_productComing == 1 && PatternMode)
                {
                    _times[0] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    ProcessNonBarcode();
                }
            }

            if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _times[1]) > 800)
            {
                if (int.Parse(_taken ?? "0") == 1)
                {
                    _times[1] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    try
                    {
                        ResetRobotOffsets();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                    _inProcess = false;
                    SendFromBuffer(0);
                }
            }


            var r = 0;
            if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _times[2]) > 1000)
            {
                if (int.Parse(_droppedFirst ?? "0") == 1)
                {
                    _times[2] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    r = 1;
                    ProductDrop(r);
                    var c = Cells.Find(cell => cell.RobotNo == r);
                    c.Drop();
                }
            }

            if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _times[3]) > 1000)
            {
                if (int.Parse(_droppedSecond ?? "0") == 1)
                {
                    _times[3] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    r = 2;
                    ProductDrop(r);
                    var c = Cells.Find(cell => cell.RobotNo == r);
                    c.Drop();
                }
            }

            if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _times[4]) > 1000)
            {
                if (int.Parse(_droppedThird ?? "0") == 1)
                {
                    _times[4] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    r = 3;
                    ProductDrop(r);
                    var c = Cells.Find(cell => cell.RobotNo == r);
                    c.Drop();
                }
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

        private static bool IsHeavy(Product product)
        {
            string[] fields = {"Tip", "Yukseklik", "Uzunluk"};
            var px = product.GetHeight();
            var py = product.GetWidth();
            if (product.GetProductType() == 33 && (py - py % 100) >= 1800)
            {
                return true;
            }

            int[] values = {product.GetProductType(), px - px % 100, py - py % 100};
            var weight = (double) Weights.Find(fields, values).Rows[0]["Brut"];

            return weight >= 50;
        }

        private static void ProcessNonBarcode()
        {
            var cList = GetNonBarcodeCells();
            var c = cList[patternLast];
            patternLast += 1;
            if (patternLast == cList.Count)
            {
                patternLast = 0;
            }

            if (c!.Full()) return;

            var boxed = 0;
            if (PatternProduct!.GetYontem() == 156 || PatternProduct.GetYontem() == 223)
            {
                boxed = 1;
            }

            var a = PatternProduct.GetProductType();
            var z = 0;
            if (boxed == 1)
            {
                z = a switch
                {
                    11 => 96,
                    21 => 96,
                    22 => 130,
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

            var cNo = c!.GetRobotNo();

            c.AddProduct();
            Offsets offsets;
            //offset hesapları
            if (boxed == 1)
            {
                offsets = Calculator.Calculate(PatternProduct.GetHeight() + 80, PatternProduct.GetWidth() + 80, z,
                    c.GetCounter(),
                    PatternProduct.GetYontem(), PatternProduct.GetProductType(), c.GetPalletHeight(),
                    c.GetPalletWidth(), c.GetPalletZ());
                //gerekli sinyaller gönderilir
                int full = 0;
                if (offsets.NextKat > c.KatMax)
                {
                    full = 1;
                }

                var s = new Signal(cNo, offsets, PatternProduct.GetHeight() + 80, PatternProduct.GetWidth() + 80,
                    PatternProduct.GetProductType(), c.GetCounter(), full, boxed);
                SendPlcSignals(s);

                if (full == 1)
                {
                    c.OrderSize = c.OrderSize - c.Holding;
                    c.Holding = 0;
                }
            }
            else
            {
                offsets = Calculator.Calculate(PatternProduct.GetHeight(), PatternProduct.GetWidth(), z, c.GetCounter(),
                    PatternProduct.GetYontem(), PatternProduct.GetProductType(), c.GetPalletHeight(),
                    c.GetPalletWidth(), c.GetPalletZ());
                //gerekli sinyaller gönderilir
                int full = 0;
                if (offsets.NextKat > c.KatMax)
                {
                    full = 1;
                }

                var s = new Signal(cNo, offsets, PatternProduct.GetHeight(), PatternProduct.GetWidth(),
                    PatternProduct.GetProductType(), c.GetCounter(), full, boxed);
                SendPlcSignals(s);
                if (full == 1)
                {
                    c.OrderSize = c.OrderSize - c.Holding;
                    c.Holding = 0;
                }
            }

            if (offsets.NextKat > c.KatMax)
            {
                //OnCellFull(cNo - 1);
                /*if (cNo == 3)
                {
                    cNo = 0;
                }
                

                AssignNonBarcodeCell(cNo + 1, PatternProduct.GetHeight(), PatternProduct.GetWidth(),
                    PatternProduct.GetProductType(), PatternProduct.GetOrderSize(),
                    PatternProduct.GetYontem().ToString(), c.GetPalletHeight(), c.GetPalletWidth(), c.GetPalletZ(),
                    c.KatMax);
                var p = new Pallet(c.PalletHeight, c.PalletWidth, PatternProduct.GetProductType(), c.OrderSize);
                OnCellAssigned(cNo + 1, 0, p);
                */
            }

            ProductAdd(cNo);

            //cell, (x,y,z) offsets, dizilim şekli, en, boy, kat, tip, sayı, hücredolu, kutulu?
        }

        public static int GetKatMax(int px, int py, int yontem, int type)
        {
            string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk"};
            int[] values = {yontem, type, px - px % 100, py - py % 100};
            try
            {
                return (int) (double) Calculator.Er.Find(fields, values).Rows[0]["KatYuksekligi"];
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static void ProcessOrder(long orderNum)
        {
            var product = Sql.Select("Siparis_No", orderNum.ToString());

            if (product == null) return;


            var c = GetCell(orderNum);

            if (c == null && Cells.Count < 3)
            {
                int px = product.GetHeight();
                int py = product.GetWidth();
                int katMax = GetKatMax(px, py, product.GetYontem(), product.GetProductType());
                var p = Sql.GetPallet(orderNum.ToString());
                if (IsHeavy(product))
                {
                    AssignCell(orderNum, 2, p!, katMax);
                    OnCellAssigned(2, orderNum, p);
                }
                else
                {
                    AssignCell(orderNum, Cells.Count + 1, p!, katMax);
                    OnCellAssigned(Cells.Count, orderNum, p);
                }
            }
            else if (c == null && Cells.Count == Cells.Capacity)
            {
                return;
            }

            c = GetCell(orderNum);
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

            var cNo = c!.GetRobotNo();
            c.AddProduct();

            Offsets offsets;

            //offset hesapları
            // if offsets == 0 write offsets to plc; else buffer it
            if (boxed == 1)
            {
                offsets = Calculator.Calculate(product.GetHeight() + 80, product.GetWidth() + 80, z,
                    c.GetCounter(),
                    product.GetYontem(), product.GetProductType(), c.GetPalletHeight(), c.GetPalletWidth(),
                    c.GetPalletZ());
                //gerekli sinyaller gönderilir
                int full = 0;
                if (offsets.NextKat > c.KatMax)
                {
                    full = 1;
                }

                var s = new Signal(cNo, offsets, product.GetHeight() + 80, product.GetWidth() + 80,
                    product.GetProductType(), c.GetCounter(), full, boxed);
                SendPlcSignals(s);

                if (full == 1)
                {
                    c.OrderSize = c.OrderSize - c.Holding;
                    c.Holding = 0;
                }
            }
            else
            {
                offsets = Calculator.Calculate(product.GetHeight(), product.GetWidth(), z, c.GetCounter(),
                    product.GetYontem(), product.GetProductType(), c.GetPalletHeight(), c.GetPalletWidth(),
                    c.GetPalletZ());
                //gerekli sinyaller gönderilir
                int full = 0;
                if (offsets.NextKat > c.KatMax)
                {
                    full = 1;
                }

                var s = new Signal(cNo, offsets, product.GetHeight(), product.GetWidth(),
                    product.GetProductType(), c.GetCounter(), full, boxed);
                SendPlcSignals(s);

                if (full == 1)
                {
                    c.OrderSize = c.OrderSize - c.Holding;
                    c.Holding = 0;
                }
            }

            if (offsets.NextKat > c.KatMax)
            {
                OnCellFull(cNo - 1);
            }
            //cell, (x,y,z) offsets, dizilim şekli, en, boy, kat, tip, sayı, hücredolu, kutulu?

            ProductAdd(cNo);
        }

        #endregion

        #region control

        private static List<Cell> GetNonBarcodeCells()
        {
            return Cells.FindAll(cell => cell.GetCellType() == 0);
        }

        private static Cell? GetCell(long type)
        {
            return Cells.FirstOrDefault(cell => cell.GetCellType() == type);
        }

        public static void AssignCell(long orderNo, int robotNo, Pallet pallet, int katMax)
        {
            var orderSize = Sql.GetOrderSize(orderNo);
            if (katMax == 0) katMax = orderSize;

            Cells.Add(new Cell(orderNo, robotNo, orderSize, pallet.GetHeight(), pallet.GetLength(), 140, katMax, 0));
        }

        public static void AssignNonBarcodeCell(int robotNo, int height, int width, int type, int orderSize,
            string yontemKodu, int palletH, int palletL, int palletZ, int katMax)
        {
            PatternProduct = new Product(height, width, type, orderSize, yontemKodu);
            Cell c = new Cell(0, robotNo, orderSize, palletH, palletL, palletZ, katMax, 0);
            Cells.Add(c);
        }

        public static void EmptyCell(int i)
        {
            if (i == 1)
            {
                PatternMode = false;
            }

            try
            {
                var c = Cells.Find(cell => cell.GetRobotNo() == i + 1);
                if (c != null)
                {
                    Cells.Remove(c);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        #endregion
    }
}