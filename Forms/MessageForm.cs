﻿using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookUI
{
    public partial class MessageForm : Form
    {
        private int userID, viewID;
        public MessageForm(int userID, int viewID)
        {
            InitializeComponent();
            this.userID = userID;
            this.viewID = viewID;
            populateData();
        }

        private void populateData()
        {
            Task.Run(async () =>
            {
                return await DataBase.getMessages(this.userID, this.viewID);
            }).ContinueWith((result) => 
            { 
                Util.SetControlPropertyThreadSafe(messageDGV, "DataSource", result.Result);
            });
        }

        private void messageDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            messageDGV.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            messageDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            string text = msgTxtBox.Text;
            msgTxtBox.Text = "";
            if (msgTxtBox.TextLength > 255)
            {
                MessageBox.Show("Message cannot be longer than 255 characters!");
            }
            else
            {
                Task.Run(async () =>
                {
                    return await DataBase.sendMessage(text, this.userID, this.viewID);
                }).ContinueWith(async (reult) =>
                {
                    if (reult.Result)
                    {
                        populateData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to send message");
                    }
                });
            }
        }

        private void msgTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
            {
                sendBtn_Click(this, EventArgs.Empty);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            charLbl.Text = msgTxtBox.TextLength.ToString() + " / 255";
            if(msgTxtBox.TextLength > 255)
            {
                msgTxtBox.BackColor = Color.FromArgb(255, 183, 183);
                charLbl.ForeColor = Color.FromArgb(186, 49, 49);
            }
            else
            {
                msgTxtBox.BackColor = SystemColors.ControlLightLight;
                charLbl.ForeColor = SystemColors.ControlDarkDark;
            }
        }
    }
}
