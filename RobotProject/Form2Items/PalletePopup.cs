using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    internal class PalletePopup: Form
    {
        
        public PalletePopup()
        {
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size((int) w, (int) h);
            Text = @"Palet Ekleme";
            Geometry.Rectangle v = new Geometry.Rectangle(w * 0.1f, w * 0.9f, 0f, h);

            var helpfulText= new ModifiedLabel("warn", "Palet numarasını sayıyla girin.");
            helpfulText.Reorient(v.SliceHorizontal(1f / 8f, 3f / 8f));
            Controls.Add(helpfulText);


            
            // buttons
            var buttonsRect = v.SliceHorizontal(0.75f, 0.9f);
            _confButton = new ModifiedButton("onay", "onay");
            _confButton.Reorient(buttonsRect.SliceVertical(0.1f, 0.4f));
            _confButton.ClickAction = () =>
            {
                Confirmed = true;
                ConnectionManager.AssignCell(int.Parse(_productNo), RobotNo);
                Close();
            };
            _confButton.Enabled = false;

            var exit = new ModifiedButton("çık", "çık");
            exit.Reorient(buttonsRect.SliceVertical(0.6f, 0.9f));
            exit.ClickAction = Close;

            Controls.Add(_confButton);
            Controls.Add(exit);
            
            
            for (float i = 0; i < 3; i++)
            {
                var mrb = new ModifiedRadioButton("radio");
                mrb.Reorient(v.SliceHorizontal(4.5f / 8f, 5.5f / 8f).SliceVertical(i/3f,(i+1)/3f));
                var i1 = i;
                mrb.ClickAction = () =>
                {
                    if (RobotNo == 0)
                    {
                        _confButton.Enabled = true;
                    }
                    RobotNo = (int) i1  + 1;
                    
                };
                Controls.Add(mrb);
            }

            
            
            // text can be changed here
            _prodNo = new TextPair("prod no", "palet no:", v.SliceHorizontal(3f / 8f, 4f / 8f));
            
            _prodNo.KeyPressed = () =>
            {
                var ss = _prodNo.SelectionStart;
                if (System.Text.RegularExpressions.Regex.IsMatch(_prodNo.Text, "[^0-9]"))
                {
                    // prodNo.Text = prodNo.Text.Remove(prodNo.Text.Length - 1);
                    ss = ss - 1;
                }
                else
                {
                    _productNo = _prodNo.Text;
                }

                _prodNo.Text = _productNo;
                //prodNo.CursorToEnd();
                _prodNo.SelectionStart = ss;
            };
            _prodNo.Text = "";
            _prodNo.Implement(Controls);
            
            




            
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private string _productNo = "";

        private float w = 400f;
        private float h = 350f;

        public void Reset()
        {
            _prodNo.Text = "";
            _productNo = "";
            _confButton.Enabled = false;

        }
        
        

        public void Opening()
        {
            Confirmed = false;
        }


        public bool Confirmed { get; private set; }

        private readonly TextPair _prodNo;

        private int RobotNo { get; set; }

        private readonly ModifiedButton _confButton;


    }
}