using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
/** Connection indicators are to indicate connection, this is achieved by calling indicator:
     * plcIndicator.paint(Color.Red); or green
     * new indicators are added to the initializer and as variables as needed
     */
internal class ConnectionIndicators
    {
        public ConnectionIndicators(int x, int y, int w, int h, bool asVisual = false)
        {
            _asVisual = asVisual;
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point(x, y ));

            if (asVisual)
            {
                _plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                _plotIndicator.PaintIndicator(Color.Fuchsia);
                Geometry.Rectangle r3 = r.SliceVertical(0f, 1f);
                _plotIndicator.Reorient(r3 );
            }
            else
            {
                // indicators
                _plcIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                Geometry.Rectangle r1 = r.SliceVertical(0.0f, 0.3f);
                _plcIndicator.Reorient(r1 );
            
                _barcodeIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                Geometry.Rectangle r2 = r.SliceVertical(0.3f, 0.6f);
                _barcodeIndicator.Reorient(r2 );
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
                motherControlCollection.Add(_plcIndicator!);
                motherControlCollection.Add(_barcodeIndicator!);                
            }
        }
        
        
        
        private readonly Indicator? _plcIndicator;
        private readonly Indicator? _barcodeIndicator;
        private readonly Indicator? _plotIndicator;
        private readonly bool _asVisual;
    }
}