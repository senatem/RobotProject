using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using EasyModbus;
using RobotProject.uiElements;
using S7.Net;
using System.Runtime.InteropServices;
using S7.Net.Types;
using DateTime = System.DateTime;
using Timer = System.Timers.Timer;
#pragma warning disable CS4014


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
        public readonly int OffX;
        public readonly int OffY;

        public Signal(int cell, Offsets offsets, int px, int py, int type, int count, int cellFull, int boxed, int offX,
            int offY)
        {
            Cell = cell;
            Offsets = offsets;
            Px = px;
            Py = py;
            Type = type;
            Count = count;
            CellFull = cellFull;
            Boxed = boxed;
            OffX = offX;
            OffY = offY;
        }
    }

    #region delegates

    public delegate void ProductIncoming(int r);

    public delegate void ProductDropped(int r);

    public delegate void CellFull(int i);

    public delegate void CellAssigned(int i, long orderNo, Pallet pallet, int katMax);

    public delegate void ErrorUpdate(int[] errorList);

    #endregion

    public static class ConnectionManager
    {
        #region declarations

        public static readonly ModbusClient BarcodeClient = new ModbusClient();
        public static readonly Plc Plc = new Plc(CpuType.S71500, "192.168.0.1", 0, 1);
        public static readonly Plc Plc2 = new Plc(CpuType.S71500, "192.168.0.50", 0, 1);
        public static readonly SqlCommunication Sql = new SqlCommunication();

        private static readonly Timer Timer = new Timer();
        
        private static readonly List<DataItem> Items = new List<DataItem>();

        private static readonly string File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "processTimes");
        public static bool TaperConnected;
        private static string? _receiveData;
        private static int _plcData;
        private static int _oldData;
        private static int _taken;
        private static int _droppedFirst;
        private static int _oldFirst;
        private static int _droppedSecond;
        private static int _oldSecond;
        private static int _droppedThird;
        private static int _oldThird;
        private static string? _data;
        private static bool _inProcess;
        public static bool PatternMode;
        public static Product? PatternProduct;
        private static int _patternLast;
        public static List<Cell> Cells = new List<Cell>(3);
        public static readonly OffsetCalculator Calculator = new OffsetCalculator();
        private static readonly ExcelReader Weights = new ExcelReader(References.ProjectPath + "Weights.xlsx");
        private static readonly long[] Times = new long[5];
        private static readonly int[] ErrorList = new int[9];
        private static int _productComing;
        private static CancellationTokenSource _cancelBarcodeSource = new CancellationTokenSource();
        private static CancellationToken _cancelBarcode = _cancelBarcodeSource.Token;

        public static event EventHandler BarcodeConnectionChanged = null!;
        public static event EventHandler PlcConnectionChanged = null!;
        public static event EventHandler TaperConnectionChanged = null!;
        public static event ProductIncoming ProductIncoming = null!;
        public static event ProductDropped ProductDropped = null!;
        public static event CellFull CellFull = null!;
        public static event CellAssigned CellAssigned = null!;

        public static event ErrorUpdate ErrorUpdate = null!;

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

        private static void OnCellAssigned(int i, long orderNo, Pallet pallet, int katMax)
        {
            CellAssigned.Invoke(i, orderNo, pallet, katMax);
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

        private static void OnErrorUpdate(int[] errorList)
        {
            ErrorUpdate.Invoke(errorList);
        }

        private static void UpdateReceiveData(object sender)
        {
            _receiveData = BitConverter.ToString(BarcodeClient.receiveData).Replace("-", " ") + Environment.NewLine;
            Parallel.Invoke(UpdateReceiveTextBox);
        }

        private static void ReadFromPlc()
        {
            Plc.ReadMultipleVars(Items);
            _plcData = (ushort) (Items[0].Value ?? 0);
            _taken = (ushort) (Items[1].Value ?? 0);
            _droppedFirst = (ushort) (Items[2].Value ?? 0);
            _droppedSecond = (ushort) (Items[3].Value ?? 0);
            _droppedThird = (ushort) (Items[4].Value ?? 0);
            
            for (var i = 0; i < 9; i++)
            {
                var index = 5 + i;
                ErrorList[i] = (ushort) (Items[index].Value ?? 0);
            }

            UpdatePlcData();
        }

        #endregion

        #region connections

        public static void Init()
        {
            BarcodeClient.ReceiveDataChanged += UpdateReceiveData;
            Buffer.Init();
            Timer.Enabled = false;
            Timer.Interval = 10;
            Timer.Elapsed += TimerElapsed;
            using (var sw = System.IO.File.CreateText(File))
            {
                sw.WriteLine("// read times //");
            }

         //   AllocConsole();
            
            
            Items.Add(DataItem.FromAddress("DB57.DBW32"));
            Items.Add(DataItem.FromAddress("DB57.DBW78"));
            Items.Add(DataItem.FromAddress("DB57.DBW24"));
            Items.Add(DataItem.FromAddress("DB57.DBW26"));
            Items.Add(DataItem.FromAddress("DB57.DBW28"));
            for (var i = 0; i < 5; i++)
            {
                var address = 86 + i * 2;
                Items.Add(DataItem.FromAddress("DB57.DBW" + address));
            }
            Items.Add(DataItem.FromAddress("DB57.DBW100"));
            for (var i = 0; i < 3; i++)
            {
                var address = 38 + i * 2;
                Items.Add(DataItem.FromAddress("DB57.DBW" + address));
            }
        }
/*
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
        */
        
        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Timer.Enabled = false;
            try
            {
                if (!Plc.IsConnected) ConnectPlc();
                ReadFromPlc();
            }
            catch
            {
                 //ignored
            }

            Timer.Enabled = true;
        }
        
        private static void ConnectBarcode()
        {
            if (BarcodeClient.Connected) BarcodeClient.Disconnect();

            try
            {
                _cancelBarcodeSource.Cancel();
            }
            catch (Exception)
            {
                //ignored
            }

            BarcodeClient.IPAddress = "192.168.0.100";
            BarcodeClient.Port = 51236;
            BarcodeClient.SerialPort = null;
            try
            {
                BarcodeClient.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: Barkod okuyucu." + ex.Message, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }

            _cancelBarcodeSource = new CancellationTokenSource();
            _cancelBarcode = _cancelBarcodeSource.Token;
            
            var barcodeListener = new Task(() => Listen());
            barcodeListener.ContinueWith(t =>
            {
                MessageBox.Show(@"Barkod okuyucu dinleyicisinde bilinmeyen bir hata gerçekleşti.");
            }, TaskContinuationOptions.OnlyOnFaulted);
            
            barcodeListener.Start();
            var args = EventArgs.Empty;
            OnBarcodeConnectionChanged(args);
        }

        private static void ConnectPlc()
        {
            if (Plc.IsConnected)
            {
                Plc.Close();
            }
            
            try
            {
                Plc.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Plc bağlantısı sağlanamadı. " + ex.Message);
            }
           
            var args = EventArgs.Empty;
            OnPlcConnectionChanged(args);
        }

        private static void ConnectTaper()
        {
            if (Plc2.IsConnected) Plc2.Close();

            try
            {
                Plc2.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Enine Bantlama Makinesi bağlantısı sağlanamadı. " + ex.Message, @"Bağlantı Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }


            TaperConnected = Plc2.IsConnected;
            var args = EventArgs.Empty;
            OnTaperConnectionChanged(args);
        }

        public static void Connect()
        {
            Sql.Connect();
            ConnectBarcode();
            ConnectPlc();
            ConnectTaper();
            Timer.Enabled = true;
        }

        public static void Disconnect()
        {
            Sql.Disconnect();
            BarcodeClient.Disconnect();
            Plc.Close();
            Plc2.Close();
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

        public static void SendServoAdjustments(List<int?> adj)
        {
            for (var i = 0; i < 3; i++)
            {
                var address = 80 + i * 2;
                var val = adj[i] ?? 0;
                Plc.Write("DB57.DBW" + address, (ushort) val);
            }
        }


        public static void SendFixSignal(double d)
        {
            ushort i = 0;
            if (d > 0) i = 1;

            Plc.Write("DB57.DBW96", i);
            var pulser = new Timer(500);
            pulser.AutoReset = false;
            pulser.Enabled = true;
            pulser.Elapsed += Pulsed;
        }

        private static void Pulsed(object sender, ElapsedEventArgs e)
        {
            Plc.Write("DB57.DBW96", 0);
        }

        public static void BypassTaper(bool val)
        {
            ushort i = 0;
            if (val) i = 1;
            
            Plc.Write("DB57.DBW98", i);
        }

        private static void SendSignal(int cell, Offsets offsets, int px, int py, int type, int count, int cellFull,
            int boxed, int offX, int offY)
        {
            int[] values =
            {
                cell, offsets.X + offX, offsets.Y + offY, offsets.Z, offsets.Pattern, px, py, offsets.Kat, type, count,
                cellFull,
                boxed
            };

            for (var i = 0; i < values.Length; i++)
            {
                var address = i * 2;
                Plc.Write("DB57.DBW" + address, (ushort) values[i]);
            }

            Plc.Write("DB57.DBW30", (ushort) offsets.Rotation);
            Plc.Write("DB57.DBW34", (ushort) offsets.NextRotation);
        }

        private static void SendFromBuffer(int r)
        {
            if (Buffer.Empty(r)) return;
            _inProcess = true;
            var s = Buffer.Pop(r);
            SendSignal(s.Cell, s.Offsets, s.Px, s.Py, s.Type, s.Count, s.CellFull, s.Boxed, s.OffX, s.OffY);
        }

        private static void SendPlcSignals(Signal s)
        {
          /*  using (var sw = File.AppendText(file))
            {
                sw.WriteLine("Start: " + processStart.ToString("HH:mm:ss") + " End: " + processEnd.ToString("HH:mm:ss") + " Elapsed: " + processEnd.Subtract(processStart).TotalMilliseconds);
            }
            */
            if (_inProcess)
            {
                Buffer.Add(s, 0);
            }
            else
            {
                _inProcess = true;
                SendSignal(s.Cell, s.Offsets, s.Px, s.Py, s.Type, s.Count, s.CellFull, s.Boxed, s.OffX, s.OffY);
            }
        }

        private static void ResetRobotOffsets()
        {
            for (var i = 0; i < 12; i++)
            {
                var address = i * 2;
                Plc.Write("DB57.DBW" + address, (ushort) 0);
            }

            Plc.Write("DB57.DBW30", (ushort) 0);
            Plc.Write("DB57.DBW34", (ushort) 0);
        }

        #endregion

        #region listeners
        private static async Task Listen()
        {
            while (true)
            {
                if (BarcodeClient.Available(100))
                {
                    ReadHoldingRegsBarcode(BarcodeClient);
                    if (_cancelBarcode.IsCancellationRequested)
                    {
                        return;
                    }

                    await Task.Delay(100, CancellationToken.None);
                }
                else
                {
                    ConnectBarcode();
                    var args = EventArgs.Empty;
                    OnBarcodeConnectionChanged(args);
                    return;
                }
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
            
            if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - Times[0] > 800)
            {
                if (_plcData != _oldData)
                {
                    _productComing = _plcData;
                    _oldData = _plcData;

                    if (_productComing == 1 && PatternMode)
                    {
                        Times[0] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        ProcessNonBarcode();
                    }
                }
            }

            if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - Times[1] > 300)
            {
                if (_taken == 1)
                {
                    Times[1] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
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


            int r;
            if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - Times[2] > 300)
            {
                if (_droppedFirst != _oldFirst)
                {
                    _oldFirst = _droppedFirst;
                    if (_droppedFirst == 1)
                    {
                        Times[2] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        r = 1;
                        ProductDrop(r);
                        var c = Cells.Find(cell => cell.RobotNo == r);
                        c.Drop();
                    }
                }
            }

            if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - Times[3] > 300)
            {
                if (_droppedSecond != _oldSecond)
                {
                    _oldSecond = _droppedSecond;
                    if (_droppedSecond == 1)
                    {
                        Times[3] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        r = 2;
                        ProductDrop(r);
                        var c = Cells.Find(cell => cell.RobotNo == r);
                        c.Drop();
                    }
                }
            }

            if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - Times[4] > 300)
            {
                if (_droppedThird != _oldThird)
                {
                    _oldThird = _droppedThird;
                    if (_droppedThird == 1)
                    {
                        Times[4] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        r = 3;
                        ProductDrop(r);
                        var c = Cells.Find(cell => cell.RobotNo == r);
                        c.Drop();
                    }
                }
            }

            OnErrorUpdate(ErrorList);
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
            Console.WriteLine("Barcode: " + barcode);
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
            if (product.GetProductType() == 33 && py - py % 100 >= 1800)
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
            var c = cList[_patternLast];
            _patternLast += 1;
            if (_patternLast == cList.Count)
            {
                _patternLast = 0;
            }

            if (c!.Full()) return;

            var adjWidth = 0;
            var adjHeight = 0;
            var offadjX = 0;
            var offadjY = 0;
            var boxed = 0;
            if (PatternProduct!.GetYontem() == 156 || PatternProduct.GetYontem() == 223)
            {
                boxed = 1;
                adjHeight += 80;
                adjWidth += 80;
            }

            if (PatternProduct.GetYontem() == 1)
            {
                adjHeight += 5;
                offadjY += 60;
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

            var cNo = c.GetRobotNo();

            c.AddProduct();
            //offset hesapları

            var offsets = Calculator.Calculate(PatternProduct.GetHeight() + adjHeight, PatternProduct.GetWidth() + adjWidth,
                z,
                c.GetCounter(),
                PatternProduct.GetYontem(), PatternProduct.GetProductType(), c.GetPalletHeight(),
                c.GetPalletWidth(), c.GetPalletZ());
            //gerekli sinyaller gönderilir
            var full = 0;
            if (offsets.NextKat > c.KatMax)
            {
                full = 1;
            }

            var s = new Signal(cNo, offsets, PatternProduct.GetHeight() + adjHeight,
                PatternProduct.GetWidth() + adjWidth,
                PatternProduct.GetProductType(), c.GetCounter(), full, boxed, offadjX, offadjY);
            SendPlcSignals(s);

            if (full == 1)
            {
                c.PHolding = 0;
                c.PDropped = 0;
                OnCellFull(c.RobotNo-1);
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
            
            var cList = Cells.FindAll(cell => cell.OrderNo == orderNum);


            Cell c = null;
            switch (cList.Count)
            {
                case 0 when Cells.Count < 3:
                {
                    var px = product.GetHeight();
                    var py = product.GetWidth();
                    var katMax = GetKatMax(px, py, product.GetYontem(), product.GetProductType());
                    var p = Sql.GetPallet(orderNum.ToString());
                    if (IsHeavy(product) && Cells.Find(cell => cell.RobotNo == 2) == null)
                    {
                        AssignCell(orderNum, 2, p!, katMax);
                        OnCellAssigned(2, orderNum, p!, katMax);
                    }
                    else
                    {
                        var r = 1;
                        while(Cells.Find(c => c.RobotNo == r)!=null)
                        {
                            r++;
                        }

                        if (r > 3) return;
                        AssignCell(orderNum, r, p!, katMax);
                        OnCellAssigned(r, orderNum, p!, katMax);
                    }

                    c = GetCell(orderNum)!;
                    break;
                }
                case 0 when Cells.Count == Cells.Capacity:
                    return;
                default:
                {
                    if (_patternLast < cList.Count)
                    {
                        c = cList[_patternLast];
                        _patternLast += 1;
                    }
                    else
                    {
                        _patternLast = 0;
                    }

                    if (_patternLast == cList.Count)
                    {
                        _patternLast = 0;
                    }

                    break;
                }
            }

            var adjWidth = 0;
            var adjHeight = 0;
            var offadjX = 0;
            var offadjY = 0;

            var boxed = 0;
            if (product.GetYontem() == 156 || product.GetYontem() == 223)
            {
                boxed = 1;
                adjHeight += 80;
                adjWidth += 80;
            }

            if (product.GetYontem() == 1)
            {
                adjHeight += 5;
                offadjY += 60;
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

            //offset hesapları

            var offsets = Calculator.Calculate(product.GetHeight() + adjHeight, product.GetWidth() + adjWidth, z,
                c.GetCounter(),
                product.GetYontem(), product.GetProductType(), c.GetPalletHeight(), c.GetPalletWidth(),
                c.GetPalletZ());
            
            //gerekli sinyaller gönderilir
            int full = 0;
            if (offsets.NextKat > c.KatMax)
            {
                full = 1;
            }

            var s = new Signal(cNo, offsets, product.GetHeight() + adjHeight, product.GetWidth() + adjWidth,
                product.GetProductType(), c.GetCounter(), full, boxed, offadjX, offadjY);
            SendPlcSignals(s);
            
            if (full == 1)
            {
                c.OrderSize -= c.Holding;
                c.Holding = 0;
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

            Cells.Add(new Cell(orderNo, robotNo, orderSize, pallet.GetHeight(), pallet.GetLength(), 140, katMax));
        }

        public static void AssignNonBarcodeCell(int robotNo, int height, int width, int type, int orderSize,
            string yontemKodu, int palletH, int palletL, int palletZ, int katMax)
        {
            PatternProduct = new Product(height, width, type, orderSize, yontemKodu);
            Cell c = new Cell(0, robotNo, orderSize, palletH, palletL, palletZ, katMax);
            Cells.Add(c);
        }

        public static void EmptyCell(int i)
        {
            try
            {
                var c = Cells.Find(cell => cell.GetRobotNo() == i + 1);
                if (c != null)
                {
                    Cells.Remove(c);
                    
                    if (Cells.Count == 0)
                    {
                        PatternMode = false;
                    }
                }

            }
            catch (Exception)
            {
                //ignore
            }
        }

        public static void IncrementCell(int i, int fill, int drop)
        {
            var c = Cells.Find(cell => cell.GetRobotNo() == i + 1);
            if (c == null) return;
            c.Holding += fill;
            c.Dropped += drop;

            if (c.Holding > c.OrderSize) c.Holding = c.OrderSize;
            if (c.Holding < 0) c.Holding = 0;
            
            if (c.Dropped > c.OrderSize) c.Dropped = c.OrderSize;
            if (c.Dropped < 0) c.Dropped = 0;
        }

        #endregion
    }
}