using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    
    /** Stuff for displaying boxes on belt and pallets, a bit messy unfortunately,
     * functions for outside access are given first and are explained when relevant
     */
    public class BoxVisuals
    {
        private const int BeltRow = 1;
        private const int BeltCol = 12;
        private const int RobotRow = 4;
        private const int RobotCol = 4;

        /** A box is added to the belt (or not belt depending on parameters)
         */
        public bool AddToBoxes(SingleBox s)
        {
            _boxes.Add(s);
            Refresh();
            return true;
        }

        /** Robot operated on the next item
         */
        private void RobotOperation(int n)
        {
            var b = _boxes.FindIndex(e => (e.RobotNo == n)&&(e.Belt));
            if (b == -1)
            {
            }
            else
            {
                _boxes[b] =  _boxes[b].UnBelt();
                Refresh();
            }
        }

        /** Clears the given pallete
         */
        private void EmptyPallete(int n)
        {
            _boxes.RemoveAll(e => (e.RobotNo == n) && (e.Belt == false));
            Refresh();
        }

        public bool RelevantPallete(int n, string text)
        {

            return (n < _palleteIndicators.Count);
        }
        public BoxVisuals(int x, int y, int w, int h, bool asVisual = false)
        {
            _asVisual = asVisual;
            _realSize = new Geometry.Rectangle(w, h, new Geometry.Point( x, y ));
            if (asVisual)
            {
                _plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                _plotIndicator.PaintIndicator(Color.Chartreuse);
                Geometry.Rectangle r3 = _realSize.SliceVertical(0f, 1f);
                _plotIndicator.Reorient(r3 );
            }
            else
            {
                // examples
                /*
                var sb = new SingleBox("111", "elma", "15", "152", "154",true,0);
                boxes.Add(sb);
                var sb2 = new SingleBox("111", "elma", "25", "252", "254",true,1);
                boxes.Add(sb2);
                var sb3 = new SingleBox("111", "elma3", "125", "152", "154",false,0);
                boxes.Add(sb3);
                var sb4 = new SingleBox("111", "elma1", "225", "252", "254",false,1);
                boxes.Add(sb4);
                */


                _beltIndicator = new Indicator("beltIndicator") {BackColor = Color.Black};
                _beltIndicator.Reorient(_realSize.SubRectangle(_beltRect));
                for (int i = 0; i < 3; i++)
                {
                    _palleteIndicators[i].BackColor = Color.Black;
                    _palleteIndicators[i].Reorient(_realSize.SubRectangle(_palleteRects[i]));
                    _robotIndicators[i].TextAlign = ContentAlignment.MiddleCenter;
                    _robotIndicators[i].Reorient(_realSize.SubRectangle(_palleteRects[i]).SubRectangle(new Geometry.Rectangle(0.3f,0.7f,-0.15f,-0.05f)));
                    var j = i;
                    _robotIndicators[i].ClickAction = () =>
                    {
                        
                        RobotOperation(j);
                    };
                }

                
                for (var j = 0; j < BeltCol; j++)
                {
                    for (var i = 0; i < BeltRow; i++)
                    {
                        var ml = new ModifiedLabel("aa", $"{i}, {j}", 8f)
                        {
                            ForeColor = Color.Black,
                            BackColor = Color.Goldenrod,
                            TextAlign = ContentAlignment.TopCenter
                        };
                        var otb = new Geometry.Rectangle((float) j / BeltCol,
                            (float) (j + 1) / BeltCol, (float) i / BeltRow, (float)
                            (i + 1) / BeltRow);
                        var tb = _realSize.SubRectangle(_beltRect.SubRectangle(otb)).SubRectangle(new Geometry.Rectangle(0.05f,0.95f,0.05f,0.95f));
                        ml.Reorient(tb);
                        _beltBoxLabels.Add(ml);
                    }
                }

                for (var i = 0; i < RobotRow; i++)
                {
                    for (var j = 0; j < RobotCol; j++)
                    {
                        var index = 0;
                        foreach (var e in _palleteRects)
                        {
                            var ml = new ModifiedLabel("aa", $"{i}, {j}", 8f)
                            {
                                ForeColor = Color.Black,
                                BackColor = Color.Goldenrod,
                                TextAlign = ContentAlignment.TopCenter
                            };
                            var otb = new Geometry.Rectangle((float) j / RobotCol,
                                (float) (j + 1) / RobotCol, (float) i / RobotRow, (float)
                                (i + 1) / RobotRow);
                            var tb = _realSize.SubRectangle(e.SubRectangle(otb)).SubRectangle(new Geometry.Rectangle(0.05f,0.95f,0.05f,0.95f));
                            ml.Reorient(tb);
                            _robotLabels[index].Add(ml);
                            index++;
                        }
                    }
                }



            }
            Refresh();
        }

        /** Refreshes the information displayed on the visuals
         */
        private void Refresh()
        {
            var index = 0;

            var beltBoxes = _boxes.FindAll(e => e.Belt);
            foreach (var item in _beltBoxLabels)
            {
                if (index < beltBoxes.Count)
                {
                    item.Text = beltBoxes[index].FullText;
                    item.Visible = true;
                }
                else
                {
                    item.Visible = false;
                }
                index++;
            }
            
            for(int i=0;i<3;i++)
            {
                var palleteBoxes = _boxes.FindAll(e => (e.Belt == false) && (e.RobotNo == i));
                index = 0;
                foreach (var item in _robotLabels[i])
                {
                    
                    if (index < palleteBoxes.Count)
                    {
                        item.Text = palleteBoxes[index].FullText;
                        item.Visible = true;
                    }
                    else
                    {
                        item.Visible = false;
                    }
                    index++;
                }
            }


        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            if (_asVisual)
            {
                motherControlCollection.Add(_plotIndicator);
            }
            else
            {                
                foreach (var bb in _beltBoxLabels)
                {
                    motherControlCollection.Add(bb);
                }
                foreach (var b in _robotLabels)
                {
                    foreach (var bb in b)
                    {
                        motherControlCollection.Add(bb);                        
                    }
                }
                motherControlCollection.Add(_beltIndicator);
                foreach (var pi in _palleteIndicators)
                {
                    motherControlCollection.Add(pi);
                }
                foreach (var pi in _robotIndicators)
                {
                    motherControlCollection.Add(pi);
                }
            }

            for (var i = 0; i < 3; i++)
            {
                var e = new ModifiedButton($"c{i}", $"{i + 1}. robotu temizle ");
                var j = i;
                e.ClickAction = () =>
                {
                    EmptyPallete(j);
                };
                e.Reorient(_realSize.SubRectangle(_palleteRects[i]).SubRectangle(new Geometry.Rectangle(0f,1f,1.05f,1.15f)));
                motherControlCollection.Add(e);
                
                _palleteTypes[i].Reorient(_realSize.SubRectangle(_palleteRects[i]).SubRectangle(new Geometry.Rectangle(0f,1f,1.18f,1.25f)));
                _palleteTypes[i].TextAlign = ContentAlignment.TopCenter;
                motherControlCollection.Add(_palleteTypes[i]);
            }
            
            

            //var exit = new ModifiedButton("exit", "exit");
            
        }

        

        private readonly Indicator _plotIndicator = null!;
        private readonly List<ModifiedLabel> _beltBoxLabels = new List<ModifiedLabel>();
        private readonly List<List<ModifiedLabel>> _robotLabels = new List<List<ModifiedLabel>>
        {
            new List<ModifiedLabel>(),new List<ModifiedLabel>(),new List<ModifiedLabel>()   
        };
        private readonly List<SingleBox> _boxes = new List<SingleBox>();
        private readonly Indicator _beltIndicator = null!;
        private readonly List<Indicator> _palleteIndicators = new List<Indicator> // the bg
        {
            new Indicator("r1"), new Indicator("r2"), new Indicator("r3")
        };
        private readonly List<ModifiedLabel> _robotIndicators = new List<ModifiedLabel> // the bg
        {
            new ModifiedLabel("r1","Robot 1"), new ModifiedLabel("r2","Robot 2"), new ModifiedLabel("r3","Robot 3")
        };
        private readonly List<ModifiedLabel> _palleteTypes = new List<ModifiedLabel> // the bg
        {
            new ModifiedLabel("p1","1. palet",12), new ModifiedLabel("r2","2. palet",12), new ModifiedLabel("r3","3. palet",12)
        };
        private readonly bool _asVisual;
        private readonly Geometry.Rectangle _realSize;
        private readonly Geometry.Rectangle _beltRect = new Geometry.Rectangle(0f, 1f, 0f, 0.2f);
        private readonly List<Geometry.Rectangle> _palleteRects = new List<Geometry.Rectangle>
        {
            new Geometry.Rectangle(0.01f, 0.33f, 0.3f, 0.8f),
            new Geometry.Rectangle(0.34f, 0.66f, 0.3f, 0.8f),
            new Geometry.Rectangle(0.67f, 0.99f, 0.3f, 0.8f)
        };
    }
}