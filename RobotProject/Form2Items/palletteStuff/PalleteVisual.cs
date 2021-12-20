using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items.palletteStuff
{
    public class PalletteVisual
    {
        // Dikkat!, aşağıdaki satırı bana sormadan değiştirirseniz, tüm kelimeler görünüyor mu diye de bakın
        static List<string> staticTexts = new List<string>{"Sipariş no:", "Palet Yükseklik:", "Palet Uzunluk:","Doluluk:"};
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
        
        public void resize(Geometry.Rectangle r)
        {
            //Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( x,y ));
            //var a = r.Split(1, 4,0.05f,0.05f);
            
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
            
            
            
            _palleteBoxBg.Reorient(centreRect);
            
            for (int i = 0; i < staticTexts.Count; i++)
            {
                _staticLabels[i].Reorient(a[i]);
                _dynamicLabels[i].Reorient(a[i+staticTexts.Count]);
            }
            
            _modifiedProgressBarDefined.Reorient(r.SubRectangle(new Geometry.Rectangle(0.02f, 0.99f, 0.7f, 0.75f)));
            _modifiedProgressBarFilled.Reorient(r.SubRectangle(new Geometry.Rectangle(0.02f, 0.99f, 0.75f, 0.8f)));
            _palleteName.Reorient(r.SubRectangle(new Geometry.Rectangle(0f, 1f, 0.1f, 0.201f)));
            _palleteButton.Reorient(r.SubRectangle(new Geometry.Rectangle(0.15f, 0.85f, 0.83f, 0.97f)));
            _plusButton.Reorient(r.SubRectangle(new Geometry.Rectangle(0.85f, 0.95f, 0.83f, 0.97f)));
            _minusButton.Reorient(r.SubRectangle(new Geometry.Rectangle(0.05f, 0.15f, 0.83f, 0.97f)));
            
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
            _palleteButton = e;
            
            var e2 = new ModifiedButton($"c{_palleteNo}", $"+",12f);
            e2.ClickAction = manuelPlus;
            e2.Reorient(_thisRectangle.SubRectangle(new Geometry.Rectangle(0.85f, 0.95f, 0.83f, 0.97f)));
            motherControlCollection.Add(e2);
            _plusButton = e2;
            
            var e3 = new ModifiedButton($"c{_palleteNo}", $"-",12f);
            e3.ClickAction = manuelMinus;
            e3.Reorient(_thisRectangle.SubRectangle(new Geometry.Rectangle(0.05f, 0.15f, 0.83f, 0.97f)));
            motherControlCollection.Add(e3);
            _minusButton = e3;

            var f = new ModifiedLabel("r", $"Hücre {_palleteNo+1}");
            f.Reorient(_thisRectangle.SubRectangle(new Geometry.Rectangle(0f, 1f, 0.1f, 0.201f)));
            var c2 = Color.FromArgb(255,204,204,12);
            // var c2 = Color.FromArgb(255,255,255,15); burayı uncommentlersen geri kalanıyla aynı sarı olur
            f.BackColor = c2;
            f.TextAlign = ContentAlignment.MiddleCenter;
            _palleteName = f;
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
        public void setInfo(string no= "???" , string en= "???", string boy="???", int cap=0)
        {
            _dynamicLabels[0].Text = no;
            _dynamicLabels[1].Text = en;
            _dynamicLabels[2].Text = boy;
            _prodCap = cap;
            _modifiedProgressBarFilled.Maximum = cap;
            _modifiedProgressBarDefined.Maximum = cap;
            _prodCountFill = 0;
            _prodCountDefn = 0;
            _dynamicLabels[3].ForeColor = Color.Black;
            _dynamicLabels[3].Text = filled();
        }

        /** this function manually increases the amount stored in bars
         * any byfunction can be added here
         */
        public void manuelEntry(int n)
        {
            incementCount(n);
            // TODO additional functionality
        }
        
        /** this function manually increases the amount stored in bars
         * any byfunction can be added here
         */
        private void manuelPlus()
        {
            manuelEntry(+1);
        }
        
        /** this function manually increases the amount stored in bars
         * any byfunction can be added here
         */
        private void manuelMinus()
        {
            manuelEntry(-1);
        }

        /** Clears the given pallete
         */
        public void EmptyPallette()
        {
            ConnectionManager.EmptyCell(_palleteNo);
            setInfo();
        }

        public void Increment()
        {
            ConnectionManager.IncrementCell(_palleteNo, 1, 1);
            incementCount(1, 1);
        }
        
        public void Decrement()
        {
            ConnectionManager.IncrementCell(_palleteNo, -1, -1);
            incementCount(-1, -1);
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
                _dynamicLabels[3].ForeColor = Color.Yellow;
                // raises warning, can be turned off
                //var gw = new GenericWarning($"Dikkat! Hücre {_palleteNo} doldu.");
                //gw.ShowDialog();
            }
            
            // this bit truns it red if overfilled also warns
            if (_prodCap == _prodCountFill)
            {
                _dynamicLabels[3].ForeColor = Color.Red;
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
            _dynamicLabels[3].Text = filled();
        }

        public void incementCount(int incrementFill=0, int incrementDefn = 0)
        {
            _prodCountFill += incrementFill;
            _prodCountDefn += incrementDefn;
            _dynamicLabels[3].Text = filled();
            _modifiedProgressBarDefined.Value = _prodCountDefn;
            _modifiedProgressBarFilled.Value = _prodCountFill;
        }

        private ModifiedLabel _palleteBoxBg;
        private ModifiedLabel _palleteName;
        private ModifiedButton _palleteButton;
        private ModifiedButton _plusButton;
        private ModifiedButton _minusButton;
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