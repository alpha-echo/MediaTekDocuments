using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TechTalk.SpecFlow;
using MediaTekDocuments.view;
using MediaTekDocuments.model;
using FluentAssertions;
using System.Threading;

namespace SpecFlowProject.Steps
{
    [Binding]
    public sealed class StepDefinitionLivres
    {
        private static readonly FrmMediatek frmMediatek = new FrmMediatek(new Utilisateur("01", "pat", "pat", "pat", "0001", "accueil"));
        private static readonly DataGridView dgvLivres = (DataGridView)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["dgvLivresListe"];


        [Given(@"je saisie la valleur dans le champs de recherche de l'id : ""([^""]*)""")]
        public void RemplirChampId(string id)
        {
            TabControl tabonglet = (TabControl)frmMediatek.Controls["tabControl"];
            frmMediatek.Visible = true;
            tabonglet.SelectedTab = (TabPage)tabonglet.Controls["tabLivres"];
            TextBox txbLivresNumRecherche = (TextBox)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresNumRecherche"];

            txbLivresNumRecherche.Text = id;
        }

        [When("je clique sur le bouton de recherche")]
        public void ClickRecherche()
        {
            TabControl tabonglet = (TabControl)frmMediatek.Controls["tabControl"];
            tabonglet.SelectedTab = (TabPage)tabonglet.Controls["tabLivres"];
            Console.WriteLine(((TextBox)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresTitreRecherche"]).Text);
            Button btnLivresNumRecherche = (Button)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["btnLivresNumRecherche"];

            frmMediatek.Visible = true;
            btnLivresNumRecherche.PerformClick();
        }

        [Then(@"la vue affiche le livre possédant l'id : ""([^""]*)""")]
        public void ComparatifResultatId(string number)
        {
            bool result = true;

            foreach (DataGridViewRow row in dgvLivres.Rows)
            {
                Livre livre = (Livre)row.DataBoundItem;
                if (livre == null || !livre.Id.Contains(number))
                {
                    result = false;
                }

                result.Should().BeTrue();
            }
        }

        [Given(@"je saisie la valleur dans le champs de recherche des titres: ""([^""]*)""")]
        public void RemplirRechercheTitre(string titre)
        {
            TabControl tabonglet = (TabControl)frmMediatek.Controls["tabControl"];
            frmMediatek.Visible = true;
            tabonglet.SelectedTab = (TabPage)tabonglet.Controls["tabLivres"];
            TextBox txbLivresTitreRecherche = (TextBox)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresTitreRecherche"];

            txbLivresTitreRecherche.Text = titre;
        }

        [Then(@"la vue affiche le livre possédant le titre  : ""([^""]*)""")]
        public void ComparatifResultatTitre(string titre)
        {
            bool result = true;

            foreach (DataGridViewRow row in dgvLivres.Rows)
            {
                Livre livre = (Livre)row.DataBoundItem;
                if (livre == null || !livre.Titre.Contains(titre))
                {
                    result = false;
                }

                result.Should().BeTrue();
            }
        }

        [Given(@"je selectionne le rayon : ""([^""]*)""")]
        public void SelectionnerRayon(string titre)
        {
            TabControl tabonglet = (TabControl)frmMediatek.Controls["tabControl"];
            frmMediatek.Visible = true;
            tabonglet.SelectedTab = (TabPage)tabonglet.Controls["tabLivres"];
            ComboBox cbxLivresRayons = (ComboBox)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["cbxLivresRayons"];
            int indiceLigne = cbxLivresRayons.FindStringExact(titre);
            Console.WriteLine(indiceLigne);
            Console.WriteLine(titre);
            cbxLivresRayons.SelectedIndex = indiceLigne;
        }

        [Then(@"la vue affiche les livres du rayon : ""([^""]*)""")]
        public void ComparatifResultatRayon(string titre)
        {
            bool result = true;

            foreach (DataGridViewRow row in dgvLivres.Rows)
            {
                Livre livre = (Livre)row.DataBoundItem;
                if (livre == null || !livre.Rayon.Contains(titre))
                {
                    result = false;
                }

                result.Should().BeTrue();
            }
        }
    }
}
