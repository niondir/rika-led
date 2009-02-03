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
    public partial class ucAdvertisement : UserControl
    {
        AdvertisementData[] ads;

        public ucAdvertisement()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            RefreshData();
        }

        private void RefreshData()
        {
            ads = Connection.GetInstance().GetAds();
            gridAds.Rows.Clear();
            foreach (AdvertisementData i in ads)
            {

                int rowNr = gridAds.Rows.Add(new object[] { i.Name, i.Region.Name,
                                                i.Text[0]+" ...",
                                                i.StartDate.ToShortDateString(),
                                                i.StopDate.ToShortDateString(),
                                                i.StartTime.ToShortTimeString(),
                                                i.StopTime.ToShortTimeString()});

                gridAds.Rows[rowNr].Tag = i;
            }
        }

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            FormAddAd addAd = new FormAddAd();
            if (addAd.ShowDialog() == DialogResult.OK)
            {
                Connection.GetInstance().Add(addAd.Value);
                RefreshData();
            }
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            if (gridAds.SelectedRows.Count == 0)
                return;
            FormAddAd editAdd = new FormAddAd((AdvertisementData)gridAds.SelectedRows[0].Tag);
            if (editAdd.ShowDialog() == DialogResult.OK)
            {
                Connection.GetInstance().EditAd(((AdvertisementData)gridAds.SelectedRows[0].Tag).Id, editAdd.Value);
                RefreshData();
            }
        }

        private void gridAds_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripButtonEdit_Click(null, null);
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow i in gridAds.SelectedRows)
            {
                Connection.GetInstance().DeleteAd((AdvertisementData)i.Tag);
            }
            RefreshData();
        }

        private void gridAds_SelectionChanged(object sender, EventArgs e)
        {
            if (gridAds.SelectedRows.Count > 0)
                toolStripButtonDelete.Enabled = toolStripButtonEdit.Enabled = true;
            else
                toolStripButtonDelete.Enabled = toolStripButtonEdit.Enabled = false;
        }
    }
}
