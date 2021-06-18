using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    /** Connection indicators are to indicate connection, this is achieved by calling indicator:
     * plcConnect, barcodeConnect or similar ease of access classes
     * new indicators are added to the initializer and as variables as needed
     */
    internal class ConnectionIndicators
    {
        public ConnectionIndicators(int x, int y, int w, int h, bool asVisual = false)
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
                var a = r.Split(1, 2, 0.05f, 0.05f);
                _plcIndicator = new Indicator("plc", References.ProjectPath + "Images\\plc_on.png");
                _plcIndicator.Reorient(a[0].FittingSquare());
                _barcodeIndicator = new Indicator("plc", References.ProjectPath + "Images\\barcode_on.png");
                _barcodeIndicator.Reorient(a[1].FittingSquare());
            }
        }

        /** Simple access to break and open connection for plc
         * ed: true => connected
         * ed: false => not connected
         */
        public void PlcConnect(bool ed)
        {
            if (ed)
            {
                _plcIndicator.PaintIndicator();
            }
            else
            {
                _plcIndicator.PaintIndicator(Color.Red);
            }
        }

        /** Simple access to break and open connection for barcode
         * ed: true => connected
         * ed: false => not connected
         */
        public void BarcodeConnect(bool ed)
        {
            if (ed)
            {
                _barcodeIndicator.PaintIndicator();
            }
            else
            {
                _barcodeIndicator.PaintIndicator(Color.Red);
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
                motherControlCollection.Add(_plcIndicator);
                motherControlCollection.Add(_barcodeIndicator);
            }
        }


        private readonly Indicator _plcIndicator = null!;
        private readonly Indicator _barcodeIndicator = null!;
        private readonly Indicator _plotIndicator = null!; // indicator of the empty plot for design purposes

        private readonly bool _asVisual;
    }
}