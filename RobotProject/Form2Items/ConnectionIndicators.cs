using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject
{
/** Connection indicators are to indicate connection, this is achieved by calling indicator:
     * plcConnect, barcodeConnect or similar ease of access classes
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
                var a = r.Split(1, 2,0.05f,0.05f);
                this.plcIndicator = new Indicator("plc", References.projectPath + "Images\\plc_on.png");
                this.plcIndicator.Reorient(a[0].FittingSquare() );
                this.barcodeIndicator = new Indicator("plc", References.projectPath + "Images\\barcode_on.png");
                this.barcodeIndicator.Reorient(a[1].FittingSquare());
            }
        }

        /** Simple access to break and open connection for plc
         * ed: true => connected
         * ed: false => not connected
         */
        public void plcConnect(bool ed)
        {
            if (ed != plcConnected)
            {
                if (ed)
                {
                    plcIndicator.paint();
                }
                else
                {
                    plcIndicator.paint(Color.Red);
                }
                plcConnected = ed;
            }
        }
        
        /** Simple access to break and open connection for barcode
         * ed: true => connected
         * ed: false => not connected
         */
        public void barcodeConnect(bool ed)
        {
            if (ed != barcodeConnected)
            {
                if (ed)
                {
                    barcodeIndicator.paint();
                }
                else
                {
                    barcodeIndicator.paint(Color.Red);
                }
                barcodeConnected = ed;
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
        private Indicator plotIndicator; // indicator of the empty plot for design purposes
        private bool plcConnected = false;
        private bool barcodeConnected = false; 
        
        private bool asVisual = false;
    }
}