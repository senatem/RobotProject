using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject
{
/** Connection indicators are to indicate connection, this is achieved by calling indicator:
     * plcIndicator.paint(Color.Red); or green
     * new indicators are added to the initializer and as variables as needed
     */
    class ConnectionIndicators
    {
        public ConnectionIndicators(int x, int y, int w, int h, bool asVisual = false)
        {
            this.asVisual = asVisual;
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( (float)x,(float)y ));

            if (asVisual)
            {
                this.plotIndicator = new Indicator("plc", References.projectPath + "Images\\placeholder.jpg");
                plotIndicator.paint(Color.Fuchsia);
                Geometry.Rectangle r3 = r.sliceVertical(0f, 1f);
                this.plotIndicator.Reorient(r3 );
            }
            else
            {
                // indicators
                this.plcIndicator = new Indicator("plc", References.projectPath + "Images\\placeholder.jpg");
                Geometry.Rectangle r1 = r.sliceVertical(0.0f, 0.3f);
                this.plcIndicator.Reorient(r1 );
            
                this.barcodeIndicator = new Indicator("plc", References.projectPath + "Images\\placeholder.jpg");
                Geometry.Rectangle r2 = r.sliceVertical(0.3f, 0.6f);
                this.barcodeIndicator.Reorient(r2 );
            }
        }
        
        public void implement(Control.ControlCollection motherControlCollection)
        {
            if (asVisual)
            {
                motherControlCollection.Add(plotIndicator);
            }
            else
            {
                motherControlCollection.Add(plcIndicator);
                motherControlCollection.Add(barcodeIndicator);                
            }
        }
        
        
        
        private Indicator plcIndicator;
        private Indicator barcodeIndicator;
        private Indicator plotIndicator;
        private bool asVisual = false;
    }
}