using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    /** This is the popup for non barcode units, necessary attributes will be added manually at initializer via adding new pairs to lines bit
     * confirm or exit can be traced by confirmed boolean
     * entered texts can be obtained by att1.text etc.
     * reset function resets inputs, it may be added to exit or opening methods, new attributes must be added manually
     * right now attributes are individual fields, they may be put into a list
     */
    internal class NonBarcodePopup : Form
    {
        public NonBarcodePopup()
        {
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size((int) w, (int) h);
            Text = @"Pattern Aç";
            Geometry.Rectangle v = new Geometry.Rectangle(w * 0.1f, w * 0.9f, 0f, h);


            for (var i = 0; i < _lineStrings.Count; i++)
            {
                var r = v.SliceHorizontal((i + 1) / 12f, (i + 2) / 12f);
                var att = new TextPair(_lineStrings[i], _lineStrings[i], r);
                _lines.Add(att);
                att.Implement(Controls);
            }
            

            // buttons
            var buttonsRect = v.SliceHorizontal(0.9f, 0.95f);
            var conf = new ModifiedButton("onay", "Onay");
            conf.Reorient(buttonsRect.SliceVertical(0.1f, 0.4f));
            conf.ClickAction = () =>
            {
                _confirmed = true;
                Close();
            };

            var exit = new ModifiedButton("çık", "Çık");
            exit.Reorient(buttonsRect.SliceVertical(0.6f, 0.9f));
            exit.ClickAction = Close;

            Controls.Add(conf);
            Controls.Add(exit);

            for (float i = 0; i < 3; i++)
            {
                var mrb = new ModifiedRadioButton("radio");
                mrb.Reorient(v.SliceHorizontal(0.8f, 0.9f).SliceVertical(i / 3f, (i + 1) / 3f));
                mrb.Text = $"Hücre {i + 1}";
                var i1 = i;
                mrb.ClickAction = () =>
                {
                    if (RobotNo == 0)
                    {
                        conf.Enabled = true;
                    }

                    RobotNo = (int) i1 + 1;

                };


                Controls.Add(mrb);
            }
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private float w = 400f;
        private float h = 700f;

        public void Reset()
        {
            foreach (var tp in _lines)
            {
                tp.Text = "";
            }
        }

        public void Opening()
        {
            _confirmed = false;
            foreach (Control control in Controls)
            {
                if (control is ModifiedRadioButton)
                {
                    (control as ModifiedRadioButton).Checked = false;
                }
            }
        }

        private Boolean _confirmed;

        public Boolean confirmed =>
            // additional invalid conditions
            _confirmed;

        public List<string> GetLines
        {
            get
            {
                return _lines.Select(textPair => textPair.Text).ToList();
            }
        }

        public int RobotNo { get; set; }
        private readonly List<TextPair> _lines = new List<TextPair>();

        private readonly List<string> _lineStrings = new List<string>
        {
            "Ürün Yükseklik", "Ürün Uzunluk", "Ürün Tip", "Sipariş Miktarı", "Yöntem Kodu", "Palet Yükseklik", "Palet Uzunluk", "Palet Yerden Yükseklik"
        };
    }
}