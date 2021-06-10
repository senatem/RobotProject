using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject
{
    /** Similar to indicators, but buttons instead
     * a function adding mechanism is not included yet, but buttons are public so it can be done like that
     */
    internal class SystemControls
    {
        public SystemControls(int x, int y, int w, int h,bool asVisual = false)
        {
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point(x, y));
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
                
                _runButton = new ModifiedButton("run", "run");
                Geometry.Rectangle r1 = r.SliceVertical(0.0f, 0.3f);
                _runButton.Reorient(r1 );
                //this.runButton.Reorient(50,300,50,100 );
            
                _pauseButton = new ModifiedButton("pause", "pause");
                Geometry.Rectangle r2 = r.SliceVertical(0.35f, 0.65f);
                _pauseButton.Reorient(r2);
            
                _stopButton = new ModifiedButton("stop", "stop");
                Geometry.Rectangle r3 = r.SliceVertical(0.7f, 1f);
                _stopButton.Reorient(r3);
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
                motherControlCollection.Add(_runButton!);
                motherControlCollection.Add(_pauseButton!);
                motherControlCollection.Add(_stopButton!);                
            }
        }

        private readonly ModifiedButton? _runButton;
        private readonly ModifiedButton? _pauseButton;
        private readonly ModifiedButton? _stopButton;
        private readonly Indicator? _plotIndicator;
        private readonly bool _asVisual;
    }
}