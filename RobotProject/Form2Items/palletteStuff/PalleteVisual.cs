using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items.palletteStuff
{
    public class PalletteVisual
    {
        // Dikkat!, aşağıdaki satırı bana sormadan değiştirirseniz, tüm kelimeler görünüyor mu diye de bakın
        static List<string> staticTexts = new List<string>{"Sipariş no:", "Palet Yükseklik:", "Palet Uzunluk:", "Palet Tipi:","Doluluk:"};
        public PalletteVisual(Geometry.Rectangle r, int palleteNo)
        {
            _palleteNo = palleteNo;
            _thisRectangle = r;
            var centreRect = r.SubRectangle(new Geometry.Rectangle(0f, 1f, 0.2f, 0.7f)); // centre
            var a = centreRect.SliceVertical(0.05f,0.95f).Split(staticTexts.Count, 2);
            a = new List<Geometry.Rectangle>
            {
                centreRect.SliceVertical(0.05f,0.65f),
                centreRect.SliceVertical(0.65f,0.95f)

            };
            a = new List<Geometry.Rectangle> { };
            a.AddRange(centreRect.SliceVertical(0.05f,0.7f).Split(staticTexts.Count,1));
            a.AddRange(centreRect.SliceVertical(0.7f,0.95f).Split(staticTexts.Count,1));
            
            
            _palleteBoxBg = new ModifiedLabel("bg");
            var c1 = Color.FromArgb(255,255,255,15);
            
            
            _palleteBoxBg.BackColor = c1;
            _palleteBoxBg.Reorient(centreRect);
            _palleteBoxBg.Text = "";
            for (int i = 0; i < staticTexts.Count; i++)
            {
                var staticText = new ModifiedLabel($"{i}",emSize:12f);
                staticText.BackColor = c1;
                staticText.Text = staticTexts[i];
                staticText.Reorient(a[i]);
                staticText.TextAlign = ContentAlignment.MiddleLeft;
                _staticLabels.Add(staticText);
                
                var dynamicText = new ModifiedLabel($"{i}",emSize:12f);
                dynamicText.BackColor = c1;
                dynamicText.Text = "???";
                dynamicText.Reorient(a[i+staticTexts.Count]);
                dynamicText.Parent = _palleteBoxBg;
                dynamicText.TextAlign = ContentAlignment.MiddleLeft;
                _dynamicLabels.Add(dynamicText);
            }
            
            _modifiedProgressBarDefined = new ModifiedProgressBar("h","");
            _modifiedProgressBarDefined.Reorient(r.SubRectangle(new Geometry.Rectangle(0.02f, 0.99f, 0.7f, 0.75f)));
            
            _modifiedProgressBarFilled = new ModifiedProgressBar("h2","");
            _modifiedProgressBarFilled.Reorient(r.SubRectangle(new Geometry.Rectangle(0.02f, 0.99f, 0.75f, 0.8f)));
            
            
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
            e.Reorient(_thisRectangle.SubRectangle(new Geometry.Rectangle(0.1f, 0.9f, 0.83f, 0.97f)));
            motherControlCollection.Add(e);

            var f = new ModifiedLabel("r", $"Hücre {_palleteNo+1}");
            f.Reorient(_thisRectangle.SubRectangle(new Geometry.Rectangle(0f, 1f, 0.1f, 0.201f)));
            var c2 = Color.FromArgb(255,204,204,12);
            // var c2 = Color.FromArgb(255,255,255,15); burayı uncommentlersen geri kalanıyla aynı sarı olur
            f.BackColor = c2;
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
            motherControlCollection.Add(_modifiedProgressBarFilled);
            motherControlCollection.Add(_modifiedProgressBarDefined);

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
            _modifiedProgressBarFilled.Maximum = cap;
            _modifiedProgressBarDefined.Maximum = cap;
            _prodCountFill = 0;
            _prodCountDefn = 0;
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
            if (_prodCap == _prodCountDefn)
            {
                _dynamicLabels[4].ForeColor = Color.Yellow;
                // raises warning, can be turned off
                //var gw = new GenericWarning($"Dikkat! Hücre {_palleteNo} doldu.");
                //gw.ShowDialog();
            }
            
            // this bit truns it red if overfilled also warns
            if (_prodCap == _prodCountFill)
            {
                _dynamicLabels[4].ForeColor = Color.Red;
                // raises warning, can be turned off
                //var gw = new GenericWarning($"Dikkat! Hücre {_palleteNo} doldu.");
                //gw.ShowDialog();
            }

            return $"{_prodCountDefn}/{_prodCap}";
//            return $"{_prodCountDefn}/{_prodCountFill}/{_prodCap}";
        }

        public void setCounts(int? defn=null, int? fill=null)
        {
            _prodCountFill = fill ?? _prodCountFill;
            _prodCountDefn = defn ?? _prodCountDefn;
            _modifiedProgressBarDefined.Value = _prodCountDefn;
            _modifiedProgressBarFilled.Value = _prodCountFill;
            _dynamicLabels[4].Text = filled();
        }

        public void incementCount(int incrementFill=0, int incrementDefn = 0)
        {
            _prodCountFill += incrementFill;
            _prodCountDefn += incrementDefn;
            _modifiedProgressBarDefined.Value = _prodCountDefn;
            _modifiedProgressBarFilled.Value = _prodCountFill;
            _dynamicLabels[4].Text = filled();
        }

        private ModifiedLabel _palleteBoxBg;
        private ModifiedProgressBar _modifiedProgressBarFilled;
        private ModifiedProgressBar _modifiedProgressBarDefined;
        private List<ModifiedLabel> _staticLabels = new List<ModifiedLabel>();
        private List<ModifiedLabel> _dynamicLabels = new List<ModifiedLabel>(); // prod no, height, width, type
        private Geometry.Rectangle _thisRectangle;
        private int _palleteNo;
        private int _prodCap=0;
        private int _prodCountDefn=0;
        private int _prodCountFill=0;
    }
}