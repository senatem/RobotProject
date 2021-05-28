using System;

namespace RobotProject
{
    public struct SingleBox
    {
        public SingleBox(string id, string isim, string en, string boy, string yukseklik, bool belt, int robotNo)
        {
            this.id = id;
            this.isim = isim;
            this.en = en;
            this.boy = boy;
            this.yukseklik = yukseklik;
            this.belt = belt;
            this.robotNo = robotNo;
        }

        string id;
        public string isim;
        public string en;
        public string boy;
        public string yukseklik;
        public bool belt;
        public int robotNo;

        public SingleBox unBelt()
        {
            return new SingleBox(this.id, this.isim, this.en, this.boy, this.yukseklik, false, this.robotNo);
        }

        public string ebyText => $"en:{en}, boy:{boy}, yuk:{yukseklik}";
        public string fullText => $"{isim}\nen:{en}, boy:{boy}, yuk:{yukseklik}";
    }
}