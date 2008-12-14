﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommunicationAPI.DataTypes;

namespace StoreClient
{
    public partial class FormAddRegion : Form
    {
        public FormAddRegion()
        {
            InitializeComponent();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            if (Valid())
            {
                int res;
                int.TryParse(textBox2.Text, out res);
                Connection.GetInstance().Add(new RegionData(res, textBox1.Text));
                this.Close();
            }
        }

        private bool Valid()
        {
            string errorMsg = string.Empty;
            int i = 1;
            if (textBox1.Text.Length < 1)
            {
                textBox1.BackColor = Color.OrangeRed;
                errorMsg += i++.ToString() + ". Bitte geben Sie eine gütige Produktgruppen Nummer ein" + Environment.NewLine;
            }
            int res;
            if (!int.TryParse(textBox2.Text, out res))
            {
                textBox2.BackColor = Color.OrangeRed;
                errorMsg += i++.ToString() + ". Bitte geben Sie eine gütige Identifikationsnummer ein" + Environment.NewLine;
            }
            if (errorMsg.Length > 0)
            {
                MessageBox.Show(errorMsg);
                return false;
            }
            return true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ChangeBackToWhiteBackCol(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = SystemColors.Window;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            richTextBox1.Text = "Bitte geben Sie eine für die neue Produktgruppe einen Namen ein";
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            richTextBox1.Text = "Bitte geben Sie die Identifikationsnummer der zugehörigen Lampe ein. Diese finden Sie auf der Rückseite der Steuerschaltung";
        }
    }
}
