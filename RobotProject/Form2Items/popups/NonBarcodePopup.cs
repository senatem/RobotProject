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
                var r = v.SliceHorizontal((i + 1) / 8f, (i + 2) / 8f);
                var att = new TextPair(_lineStrings[i], _lineStrings[i], r);
                _lines.Add(att);
                att.Implement(Controls);
            }
            

            // buttons
            var buttonsRect = v.SliceHorizontal(0.7f, 0.8f);
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
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private float w = 400f;
        private float h = 450f;

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

        
        private readonly List<TextPair> _lines = new List<TextPair>();

        private readonly List<string> _lineStrings = new List<string>
        {
            "Sipariş No",
        };
    }
}