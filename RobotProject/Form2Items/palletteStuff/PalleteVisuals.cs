using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items.palletteStuff
{
    public class PalleteVisuals
    {
        const int palleteNumber = 3;
        public PalleteVisuals(int x, int y, int w, int h, bool asVisual = false)
        {
            _asVisual = asVisual;
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point(x, y));

            if (asVisual)
            {
                _plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                _plotIndicator.PaintIndicator(Color.Fuchsia);
                Geometry.Rectangle r3 = r.SliceVertical(0f, 1f);
                _plotIndicator.Reorient(r3);
            }
            else
            {
                // indicators
                var a = r.Split(1, 3, 0.05f, 0.05f);

                for (int i = 0; i < 3; i++)
                {
                    //var centreRect = a[i].SubRectangle(new Geometry.Rectangle(0f, 1f, 0.2f, 0.6f)); // centre
                    var pv = new PalletteVisual(a[i],i);
                    
                    _pallettes.Add(pv);
                }

            }    
        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            for (int i=0; i < 3; i++)
            {
                _pallettes[i].Implement(motherControlCollection);
            }
        }
        
        public void EmptyPallette(int n)
        {
            _pallettes[n].EmptyPallette();
        }

        public void setPallette(int n, string no, string en, string boy, string type, int cap)
        {
            _pallettes[n].setInfo(no,en,boy,type,cap);
        }

        public void setProdCount(int n, int value)
        {
            _pallettes[n].setCount(value);
        }

        public void increaseProdCount(int n, int increment = 1)
        {
            _pallettes[n].incementCount(increment);
        }



        private List<PalletteVisual> _pallettes = new List<PalletteVisual>();
        private readonly Indicator _plotIndicator = null!; // indicator of the empty plot for design purposes
        private readonly bool _asVisual;
    }
}