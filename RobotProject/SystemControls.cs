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
                this.plotIndicator = new Indicator("plc", References.projectPath + "Images\\placeholder.jpg");
                plotIndicator.paint(Color.Aquamarine);
                Geometry.Rectangle r3 = r.sliceVertical(0f, 1f);
                this.plotIndicator.Reorient(r3 );
            }
            else
            {
                
                this.runButton = new ModifiedButton("run", "run");
                Geometry.Rectangle r1 = r.sliceVertical(0.0f, 0.3f);
                this.runButton.Reorient(r1 );
                //this.runButton.Reorient(50,300,50,100 );
            
                this.pauseButton = new ModifiedButton("pause", "pause");
                Geometry.Rectangle r2 = r.sliceVertical(0.35f, 0.65f);
                this.pauseButton.Reorient(r2);
            
                this.stopButton = new ModifiedButton("stop", "stop");
                Geometry.Rectangle r3 = r.sliceVertical(0.7f, 1f);
                this.stopButton.Reorient(r3);
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
            }
        }
        
        public ModifiedButton runButton;
        public ModifiedButton pauseButton;
        public ModifiedButton stopButton;
        private Indicator plotIndicator;
        private bool asVisual = false;
    }
}