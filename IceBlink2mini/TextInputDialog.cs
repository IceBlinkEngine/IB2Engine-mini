using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2mini
{
    public partial class TextInputDialog : Form
    {
        GameView gv;
        public string HeaderText = "";
        public string textInput = "";

        public TextInputDialog(GameView g, string headertxt)
        {
            InitializeComponent();            
            gv = g;
            btnReturn.Text = "RETURN";
            HeaderText = headertxt;
            this.label1.Text = headertxt;
        }
                        
        private void btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            textInput = txtInput.Text;
        }
    }
}
