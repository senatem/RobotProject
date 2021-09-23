namespace RobotProject
{
    public class Cell
    {
        public long OrderNo { get; set; }
        public int RobotNo { get; set; }
        public int OrderSize { get; set; }
        public int Holding { get; set; }
        public int PalletHeight { get; set; }
        public int PalletWidth { get; set; }
        
        public int PalletZ { get; set; }

        public Cell(long orderNo, int robotNo, int orderSize, int palletHeight, int palletWidth, int palletZ)
        {
            OrderNo = orderNo;
            RobotNo = robotNo;
            OrderSize = orderSize;
            PalletHeight = palletHeight;
            PalletWidth = palletWidth;
            PalletZ = palletZ;
        }
        
        public void AddProduct()
        {
            Holding++;
        }

        public int GetCounter()
        {
            return Holding;
        }
        public int Full()
        {
            return Holding == OrderSize ? 1 : 0;
        }

        public long GetCellType()
        {
            return OrderNo;
        }

        public int GetPalletHeight()
        {
            return PalletHeight;
        }

        public int GetPalletZ()
        {
            return PalletZ;
        }
        
        public int GetRobotNo()
        {
            return RobotNo;
        }

        public int GetPalletWidth()
        {
            return PalletWidth;
        }
    }
}