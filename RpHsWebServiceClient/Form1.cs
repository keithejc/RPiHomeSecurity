using System;
using System.Windows.Forms;

namespace HSControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonArm_Click(object sender, EventArgs e)
        {
            labelResponse.Text = ServiceClient.RunActionList(comboBoxActionLists.Text);
        }

        private void buttonGetArmed_Click(object sender, EventArgs e)
        {
            labelResponse.Text = ServiceClient.GetStatus();
        }
    }
}