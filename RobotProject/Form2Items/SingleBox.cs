namespace RobotProject.Form2Items
{
    public readonly struct SingleBox
    {
        public SingleBox(string id, string isim, string en, string boy, string yukseklik, bool belt, int robotNo)
        {
            _id = id;
            _isim = isim;
            _en = en;
            _boy = boy;
            _yukseklik = yukseklik;
            Belt = belt;
            RobotNo = robotNo;
        }

        private readonly string _id;
        private readonly string _isim;
        private readonly string _en;
        private readonly string _boy;
        private readonly string _yukseklik;
        public readonly bool Belt;
        public readonly int RobotNo;

        public SingleBox UnBelt()
        {
            return new SingleBox(_id, _isim, _en, _boy, _yukseklik, false, RobotNo);
        }

        public string EbyText => $"en:{_en}, boy:{_boy}, yuk:{_yukseklik}";
        public string FullText => $"{_isim}\nen:{_en}, boy:{_boy}, yuk:{_yukseklik}";
    }
}