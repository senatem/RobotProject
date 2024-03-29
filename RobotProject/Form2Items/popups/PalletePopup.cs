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

            
            var helpfulText= new ModifiedLabel("warn", "*Hücre no seçimi yapın.");
            helpfulText.Reorient(v.SliceHorizontal(0.5f, 0.75f));
            helpfulText.ForeColor = Color.Red;
            helpfulText.ImageAlign = ContentAlignment.TopLeft;
            helpfulText.Visible = false;
            Controls.Add(helpfulText);


            
            // buttons
            var buttonsRect = v.SliceHorizontal(0.75f, 0.9f);
            _confButton = new ModifiedButton("onay", "Onay");
            _confButton.Reorient(buttonsRect.SliceVertical(0.1f, 0.4f));
            _confButton.ClickAction = () =>
            {
                if (cb.SelectedIndex != -1)
                {
                    Confirmed = true;
                    Close();
                }
                else
                {
                    helpfulText.Visible = true;
                }
            };
            _confButton.Enabled = false;
            

            var exit = new ModifiedButton("çık", "Çık");
            exit.Reorient(buttonsRect.SliceVertical(0.6f, 0.9f));
            exit.ClickAction = Close;

            Controls.Add(_confButton);
            Controls.Add(exit);
            
            
            for (float i = 0; i < 3; i++)
            {
                var mrb = new ModifiedRadioButton("radio");
                mrb.Reorient(v.SliceHorizontal(1f / 8f, 2f / 8f).SliceVertical(i/3f,(i+1)/3f));
                mrb.Text = $"Hücre {i+1}";
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

            
            
            
            cb.Reorient(v.SliceHorizontal(2.5f / 8f, 4f / 8f));
            //cb.IntegralHeight = false;
            cb.MaxDropDownItems = 5;
            cb.DropDownStyle = ComboBoxStyle.Simple;
            //cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.AutoCompleteMode = AutoCompleteMode.Append;
            cb.TabIndex = 0;
            Controls.Add(cb);
            
            
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private string _productNo = "";

        private float w = 600f;
        private float h = 350f;

        public void Reset()
        {
            _prodNo.Text = "";
            _productNo = "";
            _confButton.Enabled = false;
        }
        
        

        public void Opening(string[] s)
        {
            Confirmed = false;
            foreach (Control control in Controls)
            {
                if (control is ModifiedRadioButton)
                {
                    (control as ModifiedRadioButton).Checked = false;
                }
            }
            cb.Items.Clear();
            cb.Items.AddRange(s);
        }


        public int SelectedIndex => cb.SelectedIndex;

        public bool Confirmed { get; private set; }

        private readonly TextPair _prodNo;

        public int RobotNo { get; set; }

        private readonly ModifiedButton _confButton;
        private ModifiedComboBox cb = new ModifiedComboBox("mcb");


    }
}