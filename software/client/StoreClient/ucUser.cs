﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommunicationAPI.DataTypes;

namespace StoreClient
{
    /// <summary>
    /// Stellt die Informationen über bestehende Benutzer und Benutzergruppen dar und bietet die Möglichkeit sie zu manipulieren
    /// </summary>
    public partial class ucUser : UserControl
    {
        UserData[] users;
        RoleData[] roles;

        /// <summary>
        /// Bewirkt das (De)Aktivieren aller Eingabeelemente, die der Benutzergruppensteuerung angehören
        /// </summary>
        private bool GroupGroupEnabled
        {
            get
            {
                return checkBoxUser.Enabled;
            }
            set
            {
                checkBoxAds.Enabled = value;
                checkBoxProducts.Enabled = value;
                checkBoxUser.Enabled = value;
                checkBoxNetwork.Enabled = value;
                checkBoxRegions.Enabled = value;
                checkBoxStats.Enabled = value;
                buttonSaveGroup.Enabled = value;
                toolStripButtonDeleteGroup.Enabled = value;
                toolStripButtonSaveGroup.Enabled = value;
                textBoxGroupName.Enabled = value;
                label7.Enabled = value;
                label6.Enabled = value;

                if (!value)
                {
                    checkBoxAds.Checked = value;
                    checkBoxProducts.Checked = value;
                    checkBoxUser.Checked = value;
                    checkBoxNetwork.Checked = value;
                    checkBoxRegions.Checked = value;
                    checkBoxStats.Checked = value;
                    textBoxGroupName.Text = "";
                }
            }
        }

        /// <summary>
        /// Initialisiert das Control und füllt es mit aktuellen Daten
        /// </summary>
        public ucUser()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            refreshContent(null, null);
        }

        /// <summary>
        /// Behandelt die Auswahl eines anderen Benutzer in der Liste.
        /// Die Benutzerdaten werden geladen und angezeigt und die Erreichbarkeit von Delete und Edit Buttons
        /// </summary>
        private void listBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxUsers.SelectedIndex >= 0)
            {
                tableLayoutPanel1.Enabled = false;
                toolStripButtonSave.Enabled = false;

                textBoxName.Text = users[listBoxUsers.SelectedIndex].Username;
                comboBoxGroup.Text = users[listBoxUsers.SelectedIndex].Role.Name;

                listBoxUsers.Tag = users[listBoxUsers.SelectedIndex];

                toolStripButtonDelete.Enabled = true;
                toolStripButtonEdit.Enabled = true;
            }
            else
            {
                toolStripButtonDelete.Enabled = false;
                toolStripButtonEdit.Enabled = false;
                toolStripButtonSave.Enabled = false;
            }
        }

        /// <summary>
        /// Behandelt die Auswahl einer anderen Benutzergruppe in der Liste.
        /// Die Benutzergruppen Daten werden geladen und angezeigt und die Erreichbarkeit von Delete und Edit Buttons
        /// </summary>
        private void listBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0)
            {
                GroupGroupEnabled = false;
                toolStripButtonEditGroup.Enabled = false;
                toolStripButtonDeleteGroup.Enabled = false;
                checkBoxAds.Checked = false;
                checkBoxUser.Checked = false;
                checkBoxProducts.Checked = false;
                textBoxName.Text = "";
                return;
            }
            listBox2.Tag = roles[listBox2.SelectedIndex];
            toolStripButtonEditGroup.Enabled = true;
            toolStripButtonDeleteGroup.Enabled = true;

            if ((roles[listBox2.SelectedIndex].Flags & (int)CommunicationAPI.AccessFlags.Ads) != 0)
                checkBoxAds.Checked = true;
            else
                checkBoxAds.Checked = false;
            
            if ((roles[listBox2.SelectedIndex].Flags & (int)CommunicationAPI.AccessFlags.Product) != 0)
                checkBoxProducts.Checked = true;
            else
                checkBoxProducts.Checked = false;
            
            if ((roles[listBox2.SelectedIndex].Flags & (int)CommunicationAPI.AccessFlags.User) != 0)
                checkBoxUser.Checked = true;
            else
                checkBoxUser.Checked = false;

            if ((roles[listBox2.SelectedIndex].Flags & (int)CommunicationAPI.AccessFlags.Traces) != 0)
                checkBoxStats.Checked = true;
            else
                checkBoxStats.Checked = false;

            if ((roles[listBox2.SelectedIndex].Flags & (int)CommunicationAPI.AccessFlags.Regions) != 0)
                checkBoxRegions.Checked = true;
            else
                checkBoxRegions.Checked = false;

            if ((roles[listBox2.SelectedIndex].Flags & (int)CommunicationAPI.AccessFlags.Network) != 0)
                checkBoxNetwork.Checked = true;
            else
                checkBoxNetwork.Checked = false;

            textBoxGroupName.Text = roles[listBox2.SelectedIndex].Name;
        }

        /// <summary>
        /// Die Eingabeelemente werden für den Benutzer freigegeben, so dass er diesen Verändern kann
        /// </summary>
        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            bool setVal = !tableLayoutPanel1.Enabled;

            tableLayoutPanel1.Enabled = setVal;
            toolStripButtonSave.Enabled = setVal;
        }

        /// <summary>
        /// Speichert die Eingaben der Benutzergruppe
        /// </summary>
        private void buttonSaveGroup_Click(object sender, EventArgs e)
        {
            bool newGroup = false;
            if (listBox2.SelectedIndex < 0)
                newGroup = true;

            int flags = 0;
            if(checkBoxAds.Checked)
                flags |= (int)CommunicationAPI.AccessFlags.Ads;
            if(checkBoxProducts.Checked)
                flags |= (int)CommunicationAPI.AccessFlags.Product;
            if(checkBoxUser.Checked)
                flags |= (int)CommunicationAPI.AccessFlags.User;
            if (checkBoxNetwork.Checked)
                flags |= (int)CommunicationAPI.AccessFlags.Network;
            if (checkBoxRegions.Checked)
                flags |= (int)CommunicationAPI.AccessFlags.Regions;
            if (checkBoxStats.Checked)
                flags |= (int)CommunicationAPI.AccessFlags.Traces;


            if (textBoxGroupName.Text.Length == 0)
            {
                MessageBox.Show("Bitte geben Sie eine Beizeichnung der Gruppe an", "Gruppenname", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            RoleData role = new RoleData(textBoxGroupName.Text, flags);
            if (newGroup)
                Connection.GetInstance().Add(role);
            else
                Connection.GetInstance().EditRole(roles[listBox2.SelectedIndex], role);

            refreshContent(null, null);

            GroupGroupEnabled = false;
        }

        /// <summary>
        /// Aktualisiert die Daten, indem der Server abgefragt wird und stellt sie dar
        /// </summary>
        private void refreshContent(object sender, EventArgs e)
        {
            listBoxUsers.Items.Clear();
            users = Connection.GetInstance().GetUsers();
            foreach (UserData i in users)
            {
                listBoxUsers.Items.Add(i.Username);
            }

            comboBoxGroup.Items.Clear();
            listBox2.Items.Clear();
            roles = Connection.GetInstance().GetRoles();
            foreach (RoleData i in roles)
            {
                comboBoxGroup.Items.Add(i.Name);
                listBox2.Items.Add(i.Name);
            }

            listBoxUsers_SelectedIndexChanged(null, null);
            listBoxGroup_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// Speichert die angegebenen Daten des Benutzers.
        /// Die Angaben werden überprüft und der Benutzer wird gegebenenfalls darauf hingewiesen, seine Angaben zu ergänzen
        /// </summary>
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text.Length == 0)
            {
                MessageBox.Show("Bitte geben Sie einen Benutzernamen ein", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                clearPWBoxes();
                return;
            }
            if (comboBoxGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Ein Benutzer muss einer Gruppe angehören, die bereits exisitert.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                clearPWBoxes();
                return;
            }
            if (textBoxNewPW.Text != textBoxNewPWagain.Text || textBoxNewPW.Text.Length == 0)
            {
                MessageBox.Show("Die Eingabe der beiden neuen Passwörter stimmen nicht überein", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                clearPWBoxes();
                return;
            }
            UserData newUser = new UserData(textBoxName.Text, textBoxNewPW.Text);
            newUser.Role = new RoleData(comboBoxGroup.Text);
            if (listBoxUsers.SelectedIndex >= 0)
            {
                UserData oldUser = new UserData(((UserData)listBoxUsers.Tag).Username, textBoxOldPW.Text);
                Connection.GetInstance().EditUser(oldUser, newUser);
            }
            else
                Connection.GetInstance().Add(newUser);
            

            tableLayoutPanel1.Enabled = false;
            toolStripButtonSave.Enabled = false;

            refreshContent(null, null);

            clearAllBoxes();
        }

        /// <summary>
        /// Löscht die Eingabefelder der Passwörter
        /// </summary>
        private void clearPWBoxes()
        {
            textBoxNewPW.Text = textBoxNewPWagain.Text = textBoxOldPW.Text = "";
        }

        /// <summary>
        /// Löscht alle Eingabefelder, die für die Benutzersteuerund zuständig sind
        /// </summary>
        private void clearAllBoxes()
        {
            textBoxName.Text = "";
            comboBoxGroup.Text = "";
            clearPWBoxes();
        }

        /// <summary>
        /// Gibt die Eingabeelemente frei für die Eingabe eines neuen Benutzers
        /// </summary>
        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            listBoxUsers.ClearSelected();
            
            tableLayoutPanel1.Enabled = true;
            toolStripButtonSave.Enabled = true;
            toolStripButtonDelete.Enabled = true;

            clearAllBoxes();
        }

        /// <summary>
        /// Eingabeerleichterung. Durch die Enter Taste wird das Speichern des Benutzers ausgelöst
        /// </summary>
        private void textBoxOldPW_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                toolStripButtonSave_Click(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gibt die Eingabeelemente frei für die Eingabe einer neuen Benutzergruppe
        /// </summary>
        private void toolStripButtonNewGroup_Click(object sender, EventArgs e)
        {
            listBox2.ClearSelected();
            GroupGroupEnabled = false;
            GroupGroupEnabled = true;
        }

        /// <summary>
        /// Löscht die ausgewählte Benutzergruppe lokal udn vom Server.
        /// Achtung, die zugehörigen Benutzer werden nicht gelöscht und behalten alle ihre Rechte.
        /// Sie sollten also einer neuen/anderen Gruppe zugeordnet werden
        /// Die Löschung wird durch eine Abfrage von unabsichtlichen Löschungen geschützt
        /// </summary>
        private void toolStripButtonDeleteGroup_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1)
            {
                if(MessageBox.Show("Soll die Gruppe \""+listBox2.SelectedItem.ToString()+"\" wirklich gelöscht werden?", "Sicherheitsfrage", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                Connection.GetInstance().DeleteRole((RoleData)listBox2.Tag);
                refreshContent(null, null);
            }
            
            GroupGroupEnabled = false;
        }

        /// <summary>
        /// Löscht den ausgewählten Benutzer lokal udn vom Server.
        /// Die Löschung wird durch eine Abfrage von unabsichtlichen Löschungen geschützt
        /// </summary>
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxUsers.SelectedIndex < 0)
            {
                tableLayoutPanel1.Enabled = false;
                toolStripButtonDelete.Enabled = false;
                toolStripButtonSave.Enabled = false;
                clearAllBoxes();
            }
            else
            {
                if(MessageBox.Show("Soll der Benutzer \""+listBoxUsers.SelectedItem.ToString()+"\" wirklich gelöscht werden?", "Sicherheitsfrage", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                Connection.GetInstance().DeleteUser((UserData)listBoxUsers.Tag);
                refreshContent(null, null);
            }
        }

        /// <summary>
        /// Die Eingabeelemente werden für die Benutzergruppe freigegeben, so dass er diese Verändern kann
        /// </summary>
        private void toolStripButtonEditGroup_Click(object sender, EventArgs e)
        {
            GroupGroupEnabled = !GroupGroupEnabled;
        }
    }
}
