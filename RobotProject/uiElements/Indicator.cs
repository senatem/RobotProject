using System.Drawing;
using System.Windows.Forms;

namespace RobotProject.uiElements
{
    public class Indicator: ModifiedLabel
    {
        public Indicator(string id, string? path=null, string text = "", ImageLayout il = ImageLayout.Stretch) : base(id, text)
        {
            if (path == null) return;
            _fig = new BetterBitmap(new Bitmap(path))
            {
                Tint = _tint
            };
            BackgroundImageLayout = il;
            BackgroundImage = _fig.Bitmap();


        }

        public sealed override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        public sealed override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        /** Recolours the tint of indicator
         * null input resets
         * 
         */
        public void PaintIndicator(Color? c = null)
        {
            _tint = c?? Color.White;
            if (_fig != null)
            {
                _fig.Tint = _tint;
                BackgroundImage = _fig.Bitmap();
            }

            
            BackColor = _tint;
        }

        private readonly BetterBitmap? _fig;
        private Color _tint = Color.White;
    }

    internal class BetterBitmap
    {
        public BetterBitmap(Bitmap bitmap)
        {
            _baseBitmap = bitmap;
        }


        public Bitmap Bitmap()
        {
            var bm = new Bitmap(_baseBitmap);

            // var m = (byte) (_baseBitmap.GetPixel(5, 5).R * (Tint.R / 255f));
            
            for (var i = 0; i<_baseBitmap.Width; i++)
            {
                for (var j = 1; j < _baseBitmap.Height; j++)
                {
                    // var c = _baseBitmap.GetPixel(i, j);
                    var a = (byte) (_baseBitmap.GetPixel(i, j).A * (Tint.A / 255f));
                    var r = (byte) (_baseBitmap.GetPixel(i, j).R * (Tint.R / 255f));
                    var g = (byte) (_baseBitmap.GetPixel(i, j).G * (Tint.G / 255f));
                    var b = (byte) (_baseBitmap.GetPixel(i, j).B * (Tint.B / 255f));
                    var c1 = Color.FromArgb(a,r,g,b);
                    bm.SetPixel(i,j,c1);
                }
            }
            return bm;
        }

        private readonly Bitmap _baseBitmap;
        public Color Tint = Color.White;
    }
}