using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using RobotProject.uiElements;


namespace RobotProject
{
    /** This is the popup for non barcode units, necessary attributes will be added manually at initializer via adding new pairs to lines bit
     * confirm or exit can be traced by confirmed boolean
     * entered texts can be obtained by att1.text etc.
     * reset function resets inputs, it may be added to exit or opening methods, new attributes must be added manually
     * right now attributes are individual fields, they may be put into a list
     */
    class NonBarcodePopup : Form
    {
        private IContainer components = null;

        public NonBarcodePopup()
        {
            
            
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size((int) w, (int) h);
            this.Text = "Ürün Ekleme";
            Geometry.Rectangle v = new Geometry.Rectangle(w * 0.1f, w * 0.9f, 0f, h);


            for (int i = 0; i < 4; i++)
            {
                var r = v.sliceHorizontal((i + 1) / 8f, (i + 2) / 8f);
                var att = new TextPair(lineStrings[i], lineStrings[i], r);
                lines.Add(att);
                att.implement(this.Controls);
            }




            // buttons
            var buttonsRect = v.sliceHorizontal(0.7f, 0.8f);
            var conf = new ModifiedButton("onay", "onay");
            conf.Reorient(buttonsRect.sliceVertical(0.1f, 0.4f));
            conf.clickAction = () =>
            {
                _confirmed = true;
                this.Close();
            };

            var exit = new ModifiedButton("çık", "çık");
            exit.Reorient(buttonsRect.sliceVertical(0.6f, 0.9f));
            exit.clickAction = () =>
            {
                this.Close();
            };

            Controls.Add(conf);
            Controls.Add(exit);
        }

        private float w = 400f;
        private float h = 450f;

        public void reset()
        {
            foreach (var tp in lines)
            {
                tp.text = "";
            }
        }

        public void opening()
        {
            _confirmed = false;
        }

        private Boolean _confirmed = false;

        public Boolean confirmed {
            get
            {
                // additional invalid conditions
                return _confirmed;
            }
        }

        public List<string> getLines
        {
            get
            {
                return lines.Select(textPair => textPair.text).ToList();
            }
        }

        public List<TextPair> lines = new List<TextPair>();
        public List<string> lineStrings = new List<string>()
        {
            "isim","en","boy","yükseklik"
        };

    }
}