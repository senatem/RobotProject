using System.Drawing;
using System.Windows.Forms;

namespace RobotProject.uiElements
{
    public class Indicator: ModifiedLabel
    {
        public Indicator(string id, string? path=null, string text = "") : base(id, text)
        {
            if (path != null)
            {
                fig = new BetterBitmap(new Bitmap(path));
                fig.tint = tint;
                BackgroundImage = fig.bitmap();
            }
            
            
        }

        /** Recolours the tint of indicator
         * null input resets
         * 
         */
        public void paint(Color? c = null)
        {
            tint = c?? Color.White;
            if (fig != null)
            {
                fig.tint = tint;
                BackgroundImage = fig.bitmap();
            }

            
            BackColor = tint;
        }

        private BetterBitmap fig;
        
        
        private Color tint = Color.White;
        

    }

    class BetterBitmap
    {
        public BetterBitmap(Bitmap bitmap)
        {
            this.baseBitmap = bitmap;
        }


        public Bitmap bitmap()
        {
            var b = new Bitmap(baseBitmap);

            var m = (byte) (baseBitmap.GetPixel(5, 5).R * (float) (tint.R / 255f));
            
            for (int i = 0; i<baseBitmap.Width; i++)
            {
                for (int j = 1; j < baseBitmap.Height; j++)
                {
                    var c = baseBitmap.GetPixel(i, j);
                    var A = (byte) (baseBitmap.GetPixel(i, j).A * (float) (tint.A / 255f));
                    var R = (byte) (baseBitmap.GetPixel(i, j).R * (float) (tint.R / 255f));
                    var G = (byte) (baseBitmap.GetPixel(i, j).G * (float) (tint.G / 255f));
                    var B = (byte) (baseBitmap.GetPixel(i, j).B * (float) (tint.B / 255f));
                    var c1 = Color.FromArgb(A,R,G,B);
                    b.SetPixel(i,j,c1);
                }
            }
            
            
            return b;
        }

        private  Bitmap baseBitmap;
        public Color tint = Color.White;
    }
}