using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject
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
                
                RunButton = new ModifiedButton("run", "çalıştır");
                RunButton.Reorient(a[0] );
            
                PauseButton = new ModifiedButton("pause", "duraklat");
                PauseButton.Reorient(a[1]);
            
                StopButton = new ModifiedButton("stop", "durdur");
                StopButton.Reorient(a[2]);

                AddProductButton = new ModifiedButton("ap", "ürün ekle");
                AddProductButton.Reorient(a[3]);
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
                motherControlCollection.Add(RunButton!);
                motherControlCollection.Add(PauseButton!);
                motherControlCollection.Add(StopButton!);
                motherControlCollection.Add(AddProductButton!);    
            }
        }
        
        public readonly ModifiedButton? RunButton;
        public readonly ModifiedButton? PauseButton;
        public readonly ModifiedButton? StopButton;
        public readonly ModifiedButton? AddProductButton;
        private readonly Indicator? _plotIndicator;
        private readonly bool _asVisual;
    }
}