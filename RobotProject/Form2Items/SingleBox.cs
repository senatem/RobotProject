namespace RobotProject.Form2Items
{
    public readonly struct SingleBox
    {
        public SingleBox(string siparis, string en, string boy, bool belt, int robotNo)
        {
            _siparis = siparis;
            _en = en;
            _boy = boy;
            Belt = belt;
            RobotNo = robotNo;
        }

        private readonly string _siparis;
        private readonly string _en;
        private readonly string _boy;
        public readonly bool Belt;
        public readonly int RobotNo;

        public SingleBox UnBelt()
        {
            return new SingleBox(_siparis, _en, _boy, false, RobotNo);
        }

        public string EbyText => $"en:{_en}, boy:{_boy}";
        public string FullText => $"{_siparis}\nen:{_en}, boy:{_boy}";
    }
}