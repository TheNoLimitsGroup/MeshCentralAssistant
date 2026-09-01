/*
Copyright 2009-2022 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MeshAssistant
{
    public partial class ConsentForm : Form
    {
        private MainForm parent;
        private string orgtitle;
        private string message = "";
        public static Dictionary<string, DateTime> autoConsent = new Dictionary<string, DateTime>();

        public ConsentForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            Translate.TranslateControl(this);
            this.orgtitle = this.Text;
            ApplyNoLimitsConsentSurface();
        }

        public string userid;
        public MeshCentralTunnel tunnel;

        public string Message { set { message = value; updateInfo(); } }
        public string UserName { set { nameLabel.Text = value; updateInfo(); } }
        public string Title { set { this.Text = string.Format(Translate.T(Properties.Resources.TitleMerge), orgtitle, value); } }
        public Image UserImage { set { if (value == null) { mainPictureBox.Image = mainPictureBox.InitialImage; } else { mainPictureBox.Image = value; } } }

        public bool AutoAccept { get { return autoConsentCheckBox.Checked; } }

        // Consent is a security decision, not a notification. Keep the existing
        // protocol callbacks and optional five-minute preference intact, but make
        // the decision, its consequence and the refusal path equally obvious.
        private void ApplyNoLimitsConsentSurface()
        {
            Color canvas = Color.FromArgb(13, 11, 19);
            Color card = Color.FromArgb(18, 16, 28);
            Color ink = Color.FromArgb(222, 233, 254);
            Color muted = Color.FromArgb(154, 147, 173);
            Color accent = Color.FromArgb(159, 115, 196);

            BackColor = canvas;
            ClientSize = new Size(500, 362);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            mainPictureBox.Visible = false;

            mainLabel.Location = new Point(28, 34);
            mainLabel.Size = new Size(440, 52);
            mainLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            mainLabel.ForeColor = ink;

            nameLabel.Location = new Point(28, 96);
            nameLabel.Size = new Size(440, 26);
            nameLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            nameLabel.ForeColor = muted;

            Panel details = new Panel();
            details.BackColor = card;
            details.Location = new Point(28, 138);
            details.Size = new Size(444, 82);
            Label copy = new Label();
            copy.Text = "They will be able to view and control your mouse and keyboard. You can end support at any time.";
            copy.Location = new Point(16, 16);
            copy.Size = new Size(412, 52);
            copy.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            copy.ForeColor = ink;
            details.Controls.Add(copy);
            Controls.Add(details);

            autoConsentCheckBox.Location = new Point(28, 237);
            autoConsentCheckBox.Size = new Size(444, 24);
            autoConsentCheckBox.ForeColor = muted;
            autoConsentCheckBox.BackColor = canvas;

            cancelButton.Text = "Decline";
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(83, 76, 96);
            cancelButton.BackColor = canvas;
            cancelButton.ForeColor = ink;
            cancelButton.Location = new Point(28, 290);
            cancelButton.Size = new Size(206, 44);

            okButton.Text = "Allow support";
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.FlatAppearance.BorderColor = accent;
            okButton.BackColor = accent;
            okButton.ForeColor = Color.White;
            okButton.Location = new Point(266, 290);
            okButton.Size = new Size(206, 44);
            AcceptButton = okButton;
        }

        private void updateInfo()
        {
            mainLabel.Text = string.Format(message, nameLabel.Text);
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            tunnel.ConsentRejected();
            Close();
        }

        private void ConsentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.consentForm = null;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            if ((userid != null) && (autoConsentCheckBox.Checked == true)) { autoConsent.Add(userid, DateTime.Now.AddMinutes(5)); }
            tunnel.ConsentAccepted();
            Close();
        }

        private void ConsentForm_Load(object sender, System.EventArgs e)
        {
            autoConsentCheckBox.Visible = (userid != null);
        }
    }
}
