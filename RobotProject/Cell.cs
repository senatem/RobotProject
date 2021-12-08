namespace RobotProject
{
    public class Cell
    {
        public long OrderNo { get; set; }
        public int RobotNo { get; set; }
        public int OrderSize { get; set; }
        public int Holding { get; set; }
        
        public int Dropped { get; set; }
        public int PalletHeight { get; set; }
        public int PalletWidth { get; set; }
        
        public int PalletZ { get; set; }
        
        public int KatMax { get; set; }

        public Cell(long orderNo, int robotNo, int orderSize, int palletHeight, int palletWidth, int palletZ, int katMax, int dropped)
        {
            OrderNo = orderNo;
            RobotNo = robotNo;
            OrderSize = orderSize;
            PalletHeight = palletHeight;
            PalletWidth = palletWidth;
            PalletZ = palletZ;
            KatMax = katMax;
            Dropped = dropped;
        }
        
        public void AddProduct()
        {
            Holding++;
        }

        public void Drop()
        {
            Dropped++;
        }

        public int GetCounter()
        {
            return Holding;
        }
        public bool Full()
        {
            return Holding == OrderSize;
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