using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TechTalk.SpecFlow;
using MediaTekDocuments.view;
using MediaTekDocuments.model;
using FluentAssertions;

namespace SpecFlowProject1.Steps
{
    [Binding]
    public sealed class testsLivresStepDefinition
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _scenarioContext;

        private static readonly FrmMediatek form = new FrmMediatek(new Utilisateur( "01", "Paul", "pat", "pat", "0001", "accueil")) ;

        private static readonly DataGridView dgvLivres = (DataGridView)form.Controls["tabControlFrmMediatek"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["dgvLivresListe"];

        public static FrmMediatek Form => form;

        public testsLivresStepDefinition(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"je saisie la valleur ""([^""]*)"" dans le champs de recherche de l'id")]
        public void GivenTheFirstNumberIs(string number)
        {
            TextBox txbLivresTitreRecherche = (TextBox)form.Controls["tabControlFrmMediatek"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresTitreRecherche"];

            txbLivresTitreRecherche.Text = number;
        }

        [When(@"je clique sur le bouton de recherche")]
        public void GivenTheSecondNumberIs()
        {
            Button btnLivresRecherche = (Button)form.Controls["tabControlFrmMediatek"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["btnLivresNumRecherche"];

            btnLivresRecherche.PerformClick();
        }

        [Then(@"Le datagridview affiche le livre possédant l'id ""([^""]*)""")]
        public void ThenTheResultShouldBe(string number)
        {
            bool result = true;


            foreach (DataGridViewRow row in dgvLivres.Rows)
            {
                Livre livre = (Livre)row.DataBoundItem;


                if (!livre.Titre.Contains(number))
                {
                    result = false;
                    break;
                }

                result.Should().BeTrue();
            }
        }

    }
}
