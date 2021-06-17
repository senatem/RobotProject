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
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( (float)x,(float)y ));
            this.asVisual = asVisual; 
            if (asVisual)
            {
                this.plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                plotIndicator.PaintIndicator(Color.Aquamarine);
                Geometry.Rectangle r3 = r.SliceVertical(0f, 1f);
                this.plotIndicator.Reorient(r3 );
            }
            else
            {
                var a = r.Split(1, 4,0.05f,0.05f);
                
                this.runButton = new ModifiedButton("run", "çalıştır");
                this.runButton.Reorient(a[0] );
            
                this.pauseButton = new ModifiedButton("pause", "duraklat");
                this.pauseButton.Reorient(a[1]);
            
                this.stopButton = new ModifiedButton("stop", "durdur");
                this.stopButton.Reorient(a[2]);

                addProductButton = new ModifiedButton("ap", "ürün ekle");
                this.addProductButton.Reorient(a[3]);
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
                motherControlCollection.Add(runButton);
                motherControlCollection.Add(pauseButton);
                motherControlCollection.Add(stopButton);
                motherControlCollection.Add(addProductButton);    
            }
        }
        
        public ModifiedButton runButton;
        public ModifiedButton pauseButton;
        public ModifiedButton stopButton;
        public ModifiedButton addProductButton;
        private Indicator plotIndicator;
        private bool asVisual = false;
    }
}