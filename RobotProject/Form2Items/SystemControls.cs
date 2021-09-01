using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    /** Similar to indicators, but buttons instead
     * a function adding mechanism is not included yet, but buttons are public so it can be done like that
     */
    class SystemControls
    {
        public SystemControls(int x, int y, int w, int h,bool asVisual = false)
        {
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( x,y ));
            this._asVisual = asVisual; 
            if (asVisual)
            {
                _plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                _plotIndicator.PaintIndicator(Color.Aquamarine);
                Geometry.Rectangle r3 = r.SliceVertical(0f, 1f);
                _plotIndicator.Reorient(r3 );
            }
            else
            {
                var a = r.Split(1, 4,0.05f,0.05f);
                
                PalleteButton = new ModifiedButton("pallete", "Palet Aç");
                PalleteButton.Reorient(a[0]);

                AddProductButton = new ModifiedButton("ap", "Ürün Ekle");
                AddProductButton.Reorient(a[1]);
            }

            
        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            if (_asVisual)
            {
                motherControlCollection.Add(_plotIndicator!);
            }
            else
            {
                motherControlCollection.Add(PalleteButton!);
                motherControlCollection.Add(AddProductButton!);    
            }
        }
        public readonly ModifiedButton? PalleteButton;
        public readonly ModifiedButton? AddProductButton;
        private readonly Indicator? _plotIndicator;
        private readonly bool _asVisual;
    }
}