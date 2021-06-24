using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items.palletteStuff
{
    public class PalletteVisual
    {
        static List<string> staticTexts = new List<string>{"Sipariş no:", "Palet Yükseklik:", "Palet Uzunluk:", "Palet Tipi:","Doluluk:"};
        public PalletteVisual(Geometry.Rectangle r, int palleteNo)
        {
            _palleteNo = palleteNo;
            _thisRectangle = r;
            var centreRect = r.SubRectangle(new Geometry.Rectangle(0f, 1f, 0.2f, 0.7f)); // centre
            var a = centreRect.SliceVertical(0.05f,0.95f).Split(staticTexts.Count, 2);
            _palleteBoxBg = new ModifiedLabel("bg");
            _palleteBoxBg.BackColor = Color.Khaki;
            _palleteBoxBg.Reorient(centreRect);
            _palleteBoxBg.Text = "";
            for (int i = 0; i < staticTexts.Count; i++)
            {
                var staticText = new ModifiedLabel($"{i}");
                staticText.BackColor = Color.Khaki;
                staticText.Text = staticTexts[i];
                staticText.Reorient(a[i]);
                staticText.TextAlign = ContentAlignment.MiddleLeft;
                _staticLabels.Add(staticText);
                
                var dynamicText = new ModifiedLabel($"{i}");
                dynamicText.BackColor = Color.Khaki;
                dynamicText.Text = "???";
                dynamicText.Reorient(a[i+staticTexts.Count]);
                dynamicText.Parent = _palleteBoxBg;
                dynamicText.TextAlign = ContentAlignment.MiddleLeft;
                _dynamicLabels.Add(dynamicText);
            }
            
            _modifiedProgressBar = new ModifiedProgressBar("h","");
            _modifiedProgressBar.Reorient(r.SubRectangle(new Geometry.Rectangle(0f, 1f, 0.7f, 0.8f)));
            
            
        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            for (int i=0; i < staticTexts.Count; i++)
            {
                motherControlCollection.Add(_staticLabels[i]);
                motherControlCollection.Add(_dynamicLabels[i]);
            }
            motherControlCollection.Add(_palleteBoxBg);
            
            var e = new ModifiedButton($"c{_palleteNo}", $"{_palleteNo+1}. Hücreyi Boşalt",12f);
            e.ClickAction = EmptyPallette;
            e.Reorient(_thisRectangle.SubRectangle(new Geometry.Rectangle(0.2f, 0.8f, 0.82f, 0.92f)));
            motherControlCollection.Add(e);

            var f = new ModifiedLabel("r", $"Hücre {_palleteNo+1}");
            f.Reorient(_thisRectangle.SubRectangle(new Geometry.Rectangle(0f, 1f, 0.1f, 0.201f)));
            f.BackColor = Color.Goldenrod;
            f.TextAlign = ContentAlignment.MiddleCenter;
            /*
             fast testing
            f.ClickAction = () =>
            {
                _prodCap = 5;                
                _modifiedProgressBar.Maximum = 5;                
                incementCount(1);
            };
            */
            
            motherControlCollection.Add(f);
            motherControlCollection.Add(_modifiedProgressBar);

        }

        /** Sets info to the pallete box
         * also 
         */
        public void setInfo(string no= "???" , string en= "???", string boy="???", string type="???", int cap=0)
        {
            _dynamicLabels[0].Text = no;
            _dynamicLabels[1].Text = en;
            _dynamicLabels[2].Text = boy;
            _dynamicLabels[3].Text = type;
            _prodCap = cap;
            _modifiedProgressBar.Maximum = cap;
            _prodCount = 0;
            _dynamicLabels[4].ForeColor = Color.Black;
            _dynamicLabels[4].Text = filled();
        }

        /** Clears the given pallete
         */
        public void EmptyPallette()
        {
            ConnectionManager.EmptyCell(_palleteNo);
            setInfo();
        }
        
        private string filled()
        {
            if (_prodCap == 0)
            {
                return "???";
            }

            // this bit truns it red if overfilled also warns
            if (_prodCap == _prodCount)
            {
                _dynamicLabels[4].ForeColor = Color.Red;
                // raises warning, can be turned off
                //var gw = new GenericWarning($"Dikkat! Hücre {_palleteNo} doldu.");
                //gw.ShowDialog();
            }


            return $"{_prodCount}/{_prodCap}";
        }

        public void setCount(int c)
        {
            _prodCount = c;
            _modifiedProgressBar.Value = _prodCount;
            _dynamicLabels[4].Text = filled();
        }

        public void incementCount(int increment=1)
        {
            _prodCount += increment;
            _modifiedProgressBar.Value = _prodCount;
            _dynamicLabels[4].Text = filled();
        }

        private ModifiedLabel _palleteBoxBg;
        private ModifiedProgressBar _modifiedProgressBar;
        private List<ModifiedLabel> _staticLabels = new List<ModifiedLabel>();
        private List<ModifiedLabel> _dynamicLabels = new List<ModifiedLabel>(); // prod no, height, width, type
        private Geometry.Rectangle _thisRectangle;
        private int _palleteNo;
        private int _prodCap=0;
        private int _prodCount=0;
    }
}