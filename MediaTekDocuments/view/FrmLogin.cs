using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaTekDocuments.controller;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class FrmLogin : Form
    {
        private FrmLoginController controller;
        public FrmLogin()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// Initialisations :
        /// Création du controleur
        /// </summary>
        private void Init()
        {
            txbLogin.Text = "";
            txbPwd.Text = "";
            controller = new FrmLoginController();
        }

        /// <summary>
        /// Demande de connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnec_Click(object sender, EventArgs e)
        {
            if (controller.GetLogin(txbLogin.Text, txbPwd.Text))
                this.Visible = false;
            else
            {
                MessageBox.Show("Mauvais mot de passe ou login utilisateur");
                txbLogin.Text = "";
                txbPwd.Text = "";
            }

        }
    }
}
