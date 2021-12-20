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

                AddProductButton = new ModifiedButton("ap", "Pattern Aç");
                AddProductButton.Reorient(a[1]);

                ReconnectButton = new ModifiedButton("recon", "Yeniden Bağlan");
                ReconnectButton.Reorient(a[2]);
                
                BypassButton = new ModifiedButton("bp", "Bantlama Bypass");
                BypassButton.Reorient(a[3]);
            }

            
        }

        public void resizeToWindowRect(Geometry.Rectangle boxRect)
        {
            var x = boxRect.L + 3 * boxRect.W / 8;
            var y = boxRect.T + boxRect.H/720*50;
            var w = boxRect.W * 3 / 4;
            var h = boxRect.H/720*100;
            resize((int)x,(int)y,(int)w,(int)h);
            //SystemControls(3*appWidthInit/8, 50, 3*appWidthInit/4, 100,false);
        }

        public void resize(int x, int y, int w, int h)
        {
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( x,y ));
            var a = r.Split(1, 4,0.05f,0.05f);
            PalleteButton.Reorient(a[0]);
            AddProductButton.Reorient(a[1]);
            ReconnectButton.Reorient(a[2]);
            BypassButton.Reorient(a[3]);
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
                motherControlCollection.Add(ReconnectButton!);
                motherControlCollection.Add(BypassButton!);
            }
        }
        public readonly ModifiedButton? PalleteButton;
        public readonly ModifiedButton? AddProductButton;
        public readonly ModifiedButton? ReconnectButton;
        public readonly ModifiedButton? BypassButton;
        private readonly Indicator? _plotIndicator;
        private readonly bool _asVisual;
    }
}