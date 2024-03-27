using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgEtats = new BindingSource();
        private List<Etat> lesEtatsEx = new List<Etat>();
        private bool ajouterBool = false;
        private bool modifEtat = false;


        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            afficherAlerteAbo();
        }
            
        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Rempli un combo de suivi
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboSuivi(List<Suivi> lesSuivis, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesSuivis;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Rempli un combo d'etat
        /// </summary>
        /// <param name="lesEtats"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboEtat(List<Etat> lesEtats, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesEtats;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Augemente un index de type string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string plusUnIdString(string id)
        {
            int taille = id.Length;
            int idnum = int.Parse(id) + 1;
            id = idnum.ToString();
            if (id.Length > taille)
                MessageBox.Show("Taille du registre arrivé a saturation");
            while (id.Length != taille)
            {
                id = "0" + id;
            }
            return id;
        }

        /// <summary>
        /// Ouvre une MessageBox au lancement de FrmMediatek.cs
        /// si des abonnements sont proches de se terminer
        /// </summary>
        private void afficherAlerteAbo()
        {
            bool interupteur = false;
            List<Revue> revues = controller.GetAllRevues();
            string alerteRevues = "Revues dont l'abonnement se termine dans moins de 30 jours : \n";
            foreach (Revue revue in revues)
            {
                List<Abonnement> abonnements = controller.GetAbonnements(revue.Id);
                if (abonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddMonths(1))
                            && (o.DateFinAbonnement >= DateTime.Now)).Count > 0)
                {
                    alerteRevues += "  -" + revue.Titre + "\n";
                    interupteur = true;
                }

            }

            if(interupteur)
                MessageBox.Show(alerteRevues);
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();  
        private readonly BindingSource bdgGenresInfo = new BindingSource();
        private readonly BindingSource bdgPublicsInfo = new BindingSource();
        private readonly BindingSource bdgRayonsInfo = new BindingSource();
        private readonly BindingSource bdgLivresListeEx = new BindingSource();
        private List<Exemplaire> lesExemplairesLivres = new List<Exemplaire>();
        

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenresInfo, cbxLivresGenresInfo);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublicsInfo, cbxLivresPublicInfo);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayonsInfo, cbxLivresRayonInfo);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxLivresExEtat);
            lesEtatsEx = controller.GetAllEtats();
            modifEtat = false;
            enCoursModifLivres(false);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires"></param>
        private void RemplirLivresListeExemplaire(List<Exemplaire> exemplaires)
        {
            if( exemplaires.Count > 0)
            {
                bdgLivresListeEx.DataSource = exemplaires;
                dgvLivresListeEx.DataSource = bdgLivresListeEx;
                dgvLivresListeEx.Columns["photo"].Visible = false;
                dgvLivresListeEx.Columns["id"].Visible = false;
                dgvLivresListeEx.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLivresListeEx.Columns["id"].DisplayIndex = 0;
                dgvLivresListeEx.Columns["numero"].DisplayIndex = 1;
                dgvLivresListeEx.Columns["dateAchat"].DefaultCellStyle.Format = "d/M/yyyy";
                dgvLivresListeEx.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //change les idEtats en la valeur de leur libelle 
                for (int i = 0 ; i < dgvLivresListeEx.RowCount; i++)
                {
                    if (int.TryParse(dgvLivresListeEx.Rows[i].Cells["idEtat"].Value.ToString(), out _))
                        dgvLivresListeEx.Rows[i].Cells["idEtat"].Value = lesEtatsEx.First(o => o.Id == dgvLivresListeEx.Rows[i].Cells["idEtat"].Value.ToString());
                }
            }
            else
            {
                dgvLivresListeEx.Columns.Clear();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            VideLivresEx();
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            cbxLivresGenresInfo.SelectedIndex = cbxLivresGenresInfo.FindString(livre.Genre);
            cbxLivresPublicInfo.SelectedIndex = cbxLivresPublicInfo.FindString(livre.Public);
            cbxLivresRayonInfo.SelectedIndex = cbxLivresRayonInfo.FindString(livre.Rayon);
            txbLivresTitre.Text = livre.Titre;
            lesExemplairesLivres = controller.GetExemplairesRevue(livre.Id);
            gbxLivresEx.Text = "Les exemplaire de " + livre.Titre;
            RemplirLivresListeExemplaire(lesExemplairesLivres);
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Affichage des informations de l'exemplaire sélectionné
        /// </summary>
        /// <param name="exemplaire"></param>
        private void AfficheLivresExemplaire(Exemplaire exemplaire)
        {
            txbLivresNbEx.Text = exemplaire.Numero.ToString();
            txbLivresPhotoEx.Text = exemplaire.Photo;
            dtpLivresDateAchatEx.Value = exemplaire.DateAchat;
            if (int.TryParse(exemplaire.IdEtat, out _))
            {
                cbxLivresExEtat.SelectedIndex = int.Parse(exemplaire.IdEtat) - 1;
            }
            else
            {
                cbxLivresExEtat.SelectedIndex = cbxLivresExEtat.FindString(exemplaire.IdEtat);
            }
            txbLivresExId.Text = exemplaire.Id;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            cbxLivresGenresInfo.SelectedIndex = -1;
            cbxLivresPublicInfo.SelectedIndex = -1;
            cbxLivresRayonInfo.SelectedIndex = -1;
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
            gbxLivresEx.Text = "";
            grpLivresInfos.Text = "";
        }

        /// <summary>
        /// Vide les zones d'affichage d'un exemplaire du livre
        /// </summary>
        private void VideLivresEx()
        {
            txbLivresNbEx.Text = "";
            dtpLivresDateAchatEx.Value = DateTime.Now;
            txbLivresPhotoEx.Text = "";
            cbxLivresExEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresEx();
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresEx();
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// Vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Vide l'affichage du detail d'un exemplaire
        /// </summary>
        private void VideLivresExemplaire()
        {
            txbLivresNbEx.Text = "";
            dtpLivresDateAchatEx.Value = DateTime.Now;
            cbxLivresExEtat.SelectedIndex = -1;
            txbLivresPhotoEx.Text = "";
            txbLivresExId.Text = "";
        }

        /// <summary>
        /// Applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void enCoursModifLivres(bool modif)
        {
            btnAjouterLivres.Enabled = !modif;
            btnSupprimerLivres.Enabled = !modif;
            btnModifierLivres.Enabled = !modif;
            btnAnnulerLivres.Enabled = modif;
            btnValiderLivres.Enabled = modif;
            txbLivresTitre.ReadOnly = !modif;
            txbLivresAuteur.ReadOnly = !modif;
            cbxLivresPublicInfo.Enabled = modif;
            txbLivresIsbn.ReadOnly = !modif;
            txbLivresCollection.ReadOnly = !modif;
            cbxLivresGenresInfo.Enabled = modif;
            cbxLivresRayonInfo.Enabled = modif;
            txbLivresImage.ReadOnly = !modif;
            txbLivresNumero.ReadOnly = true;
            dgvLivresListe.Enabled = !modif;
            cbxLivresGenres.Enabled = !modif;
            cbxLivresPublics.Enabled = !modif;
            cbxLivresRayons.Enabled = !modif;
            btnLivresNumRecherche.Enabled = !modif;
            txbLivresTitreRecherche.Enabled = !modif;
            btnLivresAnnulRayons.Enabled = !modif;
            btnLivresAnnulGenres.Enabled = !modif;
            btnLivresAnnulPublics.Enabled = !modif;
            ajouterBool = false;
            txbLivresNbEx.Enabled = false;
            dtpLivresDateAchatEx.Enabled = false;
            cbxLivresExEtat.Enabled = false;
            btnSupprimerLivresEx.Enabled = !modif;
            btnModifierLivresEx.Enabled = !modif;
            dgvLivresListeEx.Enabled = !modif;
        }

        /// <summary>
        /// démarre la procédure d'ajout de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterLivres_Click(object sender, EventArgs e)
        {
            enCoursModifLivres(true);
            ajouterBool = true;
            string id = plusUnIdString(controller.getNbLivreMax());
            if (id == "1")
                id = "00001";
            txbLivresNumero.Text = id;
            txbLivresTitre.Text = "";
            txbLivresAuteur.Text = "";
            cbxLivresPublicInfo.SelectedIndex = -1;
            txbLivresCollection.Text = "";
            cbxLivresGenresInfo.SelectedIndex = -1;
            cbxLivresRayonInfo.SelectedIndex = -1;
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
        }

        /// <summary>
        /// démarre la procédure de modification de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierLivres_Click(object sender, EventArgs e)
        {
            enCoursModifLivres(true);
        }

        /// <summary>
        /// démarre la procédure de suppresion de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerLivres_Click(object sender, EventArgs e)
        {
            Livre leLivre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            if (MessageBox.Show("Etes vous sur de vouloir supprimer" + leLivre.Titre + " de " + leLivre.Auteur + " ?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // fonction a modifier pour prendre en charge le faite que l'on ne pourra pas supprimer un livre tant que des examplaire de se livre existe
                if (controller.SupprimerLivre(leLivre))
                {
                    Thread.Sleep(100);
                    lesLivres = controller.GetAllLivres();
                    RemplirLivresListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// annule les modification ou ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerLivres_Click(object sender, EventArgs e)
        {
            enCoursModifLivres(false);
            Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            AfficheLivresInfos(livre);
        }

        /// <summary>
        /// valide dans la bdd les changements en cours ( ajout / modification)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderLivres_Click(object sender, EventArgs e)
        {
            bool checkValid;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbLivresNumero.Text;
                Genre unGenre = (Genre)cbxLivresGenresInfo.SelectedItem;
                Public unPublic = (Public)cbxLivresPublicInfo.SelectedItem;
                Rayon unRayon = (Rayon)cbxLivresRayonInfo.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = txbLivresTitre.Text;
                string image = txbLivresImage.Text;
                string isbn = txbLivresIsbn.Text;
                string auteur = txbLivresAuteur.Text;
                string collection = txbLivresCollection.Text;
                string idGenre = (unGenre == null) ? null : unGenre.Id;
                string genre = (unGenre == null) ? null : unGenre.Libelle;
                string idPublic = (unPublic == null) ? null : unPublic.Id;
                string lePublic = (unPublic == null) ? null : unPublic.Libelle;
                string idRayon = (unRayon == null) ? null : unRayon.Id;
                string rayon = (unRayon == null) ? null : unRayon.Libelle;
                if (titre != "" && auteur != "" && genre != null && unPublic != null)
                {
                    Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    if (!ajouterBool)  // si on est en  modification
                        checkValid = controller.UpdateLivre(livre);
                    else      // si on est en creation
                        checkValid = controller.CreerLivre(livre);
                    if (checkValid)
                    {
                        enCoursModifLivres(false);
                        Thread.Sleep(100);
                        lesLivres = controller.GetAllLivres();
                        RemplirLivresListeComplete();
                    }
                    else
                    {
                        if (txbLivresNumero.ReadOnly)
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Supprime un exemplaire ou Annule la modification en cours
        /// en fonction de modif etat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerLivresEx_Click(object sender, EventArgs e)
        {
            if(modifEtat)
            {
                modifEtat = false;
                btnSupprimerLivresEx.Text = "Supprimer";
                btnModifierLivresEx.Text = "Modifier";
                cbxLivresExEtat.Enabled = false;
                enCoursModifLivres(false);
                grpLivresInfos.Enabled = true;

            }
            else // si on est pas en modif 
            {
                if(dgvLivresListeEx.CurrentCell != null)
                {
                    if (MessageBox.Show("Etes vous de supprimer l'exemplaire " + txbLivresNbEx.Text + " ? ", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int numero = int.Parse(txbLivresNbEx.Text);
                        DateTime dateAchat = (DateTime)dtpLivresDateAchatEx.Value;
                        string photo = txbLivresPhotoEx.Text;
                        string idEtat = ((Etat)cbxLivresExEtat.SelectedItem).Id;
                        string iDocument = txbLivresExId.Text;
                        {
                            Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, iDocument);
                            controller.SupprimerExemplaire(exemplaire);
                            lesExemplairesLivres = controller.GetExemplairesRevue(iDocument);
                            RemplirLivresListeExemplaire(lesExemplairesLivres);
                            Thread.Sleep(20);
                        }
                    }
                    
                }
                else
                {
                    MessageBox.Show("Aucun exemplaire selectionné");
                }
            }
        }

        /// <summary>
        /// Modifie ou valide la modification en cours
        /// en fonction de modif etat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierLivresEx_Click(object sender, EventArgs e)
        {
            if (modifEtat)
            {
                if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    modifEtat = false;
                    btnSupprimerLivresEx.Text = "Supprimer";
                    btnModifierLivresEx.Text = "Modifier";
                    int? a = null;
                    int numero = 0;
                    try
                    {
                        numero = int.Parse(txbLivresNbEx.Text);
                        a = numero;
                    }
                    catch
                    {
                        MessageBox.Show("Le numéro doit etre un entier");
                    }
                    DateTime dateAchat = (DateTime)dtpLivresDateAchatEx.Value;
                    string photo = txbLivresPhotoEx.Text;
                    string idEtat = "";
                    try
                    {
                        idEtat = ((Etat)cbxLivresExEtat.SelectedItem).Id;
                    }
                    catch
                    {
                        MessageBox.Show("Aucun etat selectionné");
                    }
                    string iDocument = txbLivresExId.Text;
                    if (a != null && idEtat != "" && iDocument != "")
                    {
                        Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, iDocument);
                        controller.UpdateExemplaire(exemplaire);
                        enCoursModifLivres(false);
                        lesExemplairesLivres = controller.GetExemplairesRevue(iDocument);
                        RemplirLivresListeExemplaire(lesExemplairesLivres);
                        Thread.Sleep(20);
                        grpLivresInfos.Enabled = true;
                    }
                }
            }
            else // si on est pas en modif
            {
                if (dgvLivresListeEx.CurrentCell != null)
                {
                    btnSupprimerLivresEx.Text = "Annuler";
                    btnModifierLivresEx.Text = "Valider";
                    modifEtat = true;
                    enCoursModifLivres(true);
                    btnAnnulerLivres.Enabled = false;
                    btnValiderLivres.Enabled = false;
                    btnSupprimerLivresEx.Enabled = true;
                    btnModifierLivresEx.Enabled = true;
                    cbxLivresExEtat.Enabled = true;
                    grpLivresInfos.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Aucun exemplaire selectionné");
                }    
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du detaille de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresListeEx_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListeEx.CurrentCell != null)
            {
                try
                {
                    Exemplaire exemplaire = (Exemplaire)bdgLivresListeEx[bdgLivresListeEx.Position];
                    AfficheLivresExemplaire(exemplaire);
                }
                catch
                {
                    VideLivresExemplaire();
                }
            }
            else
            {
                VideLivresExemplaire();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresListeEx_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresExemplaire();
            Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            string titreColonne = dgvLivresListeEx.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = controller.GetExemplairesRevue(livre.Id);

            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "IdEtat":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.IdEtat).ToList();
                    break;
            }
            RemplirLivresListeExemplaire(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private readonly BindingSource bdgGenresInfoDvd = new BindingSource();
        private readonly BindingSource bdgPublicsInfoDvD = new BindingSource();
        private readonly BindingSource bdgRayonsInfoDvD = new BindingSource();
        private readonly BindingSource bdgDvdListeEx = new BindingSource();
        private List<Exemplaire> lesExemplairesDvd = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenresInfoDvd, cbxDvdGenresInfo);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublicsInfoDvD, cbxDvdPublicInfo);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayonsInfoDvD, cbxDvdRayonInfo);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxDvdExEtat);
            lesEtatsEx = controller.GetAllEtats();
            modifEtat = false;
            enCoursModifDvd(false);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires"></param>
        private void RemplirDvdListeExemplaire(List<Exemplaire> exemplaires)
        {
            if (exemplaires.Count > 0)
            {
                bdgDvdListeEx.DataSource = exemplaires;
                dgvDvdListeEx.DataSource = bdgDvdListeEx;
                dgvDvdListeEx.Columns["photo"].Visible = false;
                dgvDvdListeEx.Columns["id"].Visible = false;
                dgvDvdListeEx.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvDvdListeEx.Columns["id"].DisplayIndex = 0;
                dgvDvdListeEx.Columns["numero"].DisplayIndex = 1;
                dgvDvdListeEx.Columns["dateAchat"].DefaultCellStyle.Format = "d/M/yyyy";
                dgvDvdListeEx.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //change les idEtats en la valeur de leur libelle 
                for (int i = 0; i < dgvDvdListeEx.RowCount; i++)
                {
                    if (int.TryParse(dgvDvdListeEx.Rows[i].Cells["idEtat"].Value.ToString(), out _))
                        dgvDvdListeEx.Rows[i].Cells["idEtat"].Value = lesEtatsEx.First(o => o.Id == dgvDvdListeEx.Rows[i].Cells["idEtat"].Value.ToString());
                }
            }
            else
            {
                dgvDvdListeEx.Columns.Clear();
            }

        }
        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            cbxDvdGenresInfo.SelectedIndex = cbxDvdGenresInfo.FindString(dvd.Genre);
            cbxDvdPublicInfo.SelectedIndex = cbxDvdPublicInfo.FindString(dvd.Public);
            cbxDvdRayonInfo.SelectedIndex = cbxDvdRayonInfo.FindString(dvd.Rayon);
            txbDvdTitre.Text = dvd.Titre;
            lesExemplairesDvd = controller.GetExemplairesRevue(dvd.Id);
            gbxDvdEx.Text = "Les exemplaire de " + dvd.Titre;
            RemplirDvdListeExemplaire(lesExemplairesDvd);
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Affichage des informations de l'exemplaire sélectionné
        /// </summary>
        /// <param name="exemplaire"></param>
        private void AfficheDvdExemplaire(Exemplaire exemplaire)
        {
            txbDvdNbEx.Text = exemplaire.Numero.ToString();
            txbDvdPhotoEx.Text = exemplaire.Photo;
            dtpDvdDateAchatEx.Value = exemplaire.DateAchat;
            if (int.TryParse(exemplaire.IdEtat, out _))
            {
                cbxDvdExEtat.SelectedIndex = int.Parse(exemplaire.IdEtat) - 1;
            }
            else
            {
                cbxDvdExEtat.SelectedIndex = cbxDvdExEtat.FindString(exemplaire.IdEtat);
            }
            txbDvdExId.Text = exemplaire.Id;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            cbxDvdGenresInfo.SelectedIndex = -1;
            cbxDvdPublicInfo.SelectedIndex = -1;
            cbxDvdRayonInfo.SelectedIndex = -1;
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
            gbxDvdEx.Text = "";
            grpDvdInfos.Text = "";
        }

        /// <summary>
        /// Vide les zones d'affichage d'un exemplaire du livre
        /// </summary>
        private void VideDvdEx()
        {
            txbDvdNbEx.Text = "";
            dtpDvdDateAchatEx.Value = DateTime.Now;
            txbDvdPhotoEx.Text = "";
            cbxDvdExEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdEx();
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdEx();
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Vide l'affichage du detail d'un exemplaire
        /// </summary>
        private void VideDvdExemplaire()
        {
            txbDvdNbEx.Text = "";
            dtpDvdDateAchatEx.Value = DateTime.Now;
            cbxDvdExEtat.SelectedIndex = -1;
            txbDvdPhotoEx.Text = "";
            txbDvdExId.Text = "";
        }

        /// <summary>
        /// configure l'interface en fonction de la procédure événementielle requise
        /// </summary>
        /// <param name="modif"></param>
        private void enCoursModifDvd(bool modif)
        {
            btnAjouterDvd.Enabled = !modif;
            btnSupprimerDvd.Enabled = !modif;
            btnModifierDvd.Enabled = !modif;
            btnAnnulerDvd.Enabled = modif;
            btnValiderDvd.Enabled = modif;
            txbDvdTitre.ReadOnly = !modif;
            txbDvdRealisateur.ReadOnly = !modif;
            txbDvdSynopsis.ReadOnly = !modif;
            cbxDvdPublicInfo.Enabled = modif;
            txbDvdDuree.ReadOnly = !modif;
            cbxDvdGenresInfo.Enabled = modif;
            cbxDvdRayonInfo.Enabled = modif;
            txbDvdImage.ReadOnly = !modif;
            dgvDvdListe.Enabled = !modif;
            txbDvdNumero.ReadOnly = true;
            cbxDvdGenres.Enabled = !modif;
            cbxDvdPublics.Enabled = !modif;
            cbxDvdRayons.Enabled = !modif;
            btnDvdNumRecherche.Enabled = !modif;
            txbDvdTitreRecherche.Enabled = !modif;
            btnDvdAnnulRayons.Enabled = !modif;
            btnDvdAnnulGenres.Enabled = !modif;
            btnDvdAnnulPublics.Enabled = !modif;
            ajouterBool = false;
            txbDvdNbEx.Enabled = false;
            dtpDvdDateAchatEx.Enabled = false;
            cbxDvdExEtat.Enabled = false;
            btnSupprimerDvdEx.Enabled = !modif;
            btnModifierDvdEx.Enabled = !modif;
            dgvDvdListeEx.Enabled = !modif;
        }

        /// <summary>
        /// lance la procédure d'ajout d'un nouveau dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterDvd_Click(object sender, EventArgs e)
        {
            enCoursModifDvd(true);
            ajouterBool = true;
            string id = plusUnIdString(controller.getNbDvdMax());
            if (id == "1")
                id = "20001";
            txbDvdNumero.Text = id;
            txbDvdTitre.Text = "";
            txbDvdRealisateur.Text = "";
            cbxDvdPublicInfo.SelectedIndex = -1;
            txbDvdSynopsis.Text = "";
            cbxDvdGenresInfo.SelectedIndex = -1;
            cbxDvdRayonInfo.SelectedIndex = -1;
            txbDvdDuree.Text = "";
        }

        /// <summary>
        /// lance la procédure de modification du DVD sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierDvd_Click(object sender, EventArgs e)
        {
            enCoursModifDvd(true);
        }

        /// <summary>
        /// lance la procédure de suppresion du DVD sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerDvd_Click(object sender, EventArgs e)
        {
            Dvd leDvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            if (MessageBox.Show("Etes vous sur de vouloir supprimer" + leDvd.Titre + " de " + leDvd.Realisateur + " ?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // fonction a modifier pour prendre en charge le faite que l'on ne pourra pas supprimer un livre tant que des examplaire de se livre existe
                if (controller.SupprimerDvd(leDvd))
                {
                    Thread.Sleep(100);
                    lesDvd = controller.GetAllDvd();
                    RemplirDvdListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// Annule les modification en cours (ajouter / supprimer )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerDvd_Click(object sender, EventArgs e)
        {
            enCoursModifDvd(false);
            Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            AfficheDvdInfos(dvd);
        }

        /// <summary>
        /// Valide les modification en cours dans la BDD ( ajouter / supprimer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderDvd_Click(object sender, EventArgs e)
        {
            bool checkValid = true;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbDvdNumero.Text; ;
                Genre unGenre = (Genre)cbxDvdGenresInfo.SelectedItem;
                Public unPublic = (Public)cbxDvdPublicInfo.SelectedItem;
                Rayon unRayon = (Rayon)cbxDvdRayonInfo.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = txbDvdTitre.Text;
                string image = txbDvdImage.Text;
                int duree = (txbDvdDuree.Text == "") ? 0 : int.Parse(txbDvdDuree.Text);
                string realisateur = txbDvdRealisateur.Text;
                string synopsis = txbDvdSynopsis.Text;
                string idGenre = (unGenre == null) ? null : unGenre.Id;
                string genre = (unGenre == null) ? null : unGenre.Libelle;
                string idPublic = (unPublic == null) ? null : unPublic.Id;
                string lePublic = (unPublic == null) ? null : unPublic.Libelle;
                string idRayon = (unRayon == null) ? null : unRayon.Id;
                string rayon = (unRayon == null) ? null : unRayon.Libelle;
                if (titre != "" && realisateur != "" && genre != null && unPublic != null)
                {
                    Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    if (!ajouterBool)
                        checkValid = controller.UpdateDvd(dvd);
                    else
                        checkValid = controller.CreerDvd(dvd);
                    if (checkValid)
                    {
                        enCoursModifDvd(false);
                        Thread.Sleep(100);
                        lesDvd = controller.GetAllDvd();
                        RemplirDvdListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Supprime un exemplaire ou Annule la modification en cours
        /// en fonction de modif etat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerDvdEx_Click(object sender, EventArgs e)
        {
            if (modifEtat)
            {
                modifEtat = false;
                btnSupprimerDvdEx.Text = "Supprimer";
                btnModifierDvdEx.Text = "Modifier";
                cbxDvdExEtat.Enabled = false;
                enCoursModifDvd(false);
                grpDvdInfos.Enabled = true;

            }
            else // si on est pas en modif 
            {
                if (dgvDvdListeEx.CurrentCell != null)
                {
                    if (MessageBox.Show("Etes vous de supprimer l'exemplaire " + txbDvdNbEx.Text + " ? ", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int numero = int.Parse(txbDvdNbEx.Text);
                        DateTime dateAchat = (DateTime)dtpDvdDateAchatEx.Value;
                        string photo = txbDvdPhotoEx.Text;
                        string idEtat = ((Etat)cbxDvdExEtat.SelectedItem).Id;
                        string iDocument = txbDvdExId.Text;
                        {
                            Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, iDocument);
                            controller.SupprimerExemplaire(exemplaire);
                            lesExemplairesDvd = controller.GetExemplairesRevue(iDocument);
                            RemplirDvdListeExemplaire(lesExemplairesDvd);
                            Thread.Sleep(20);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Aucun exemplaire selectionné");
                }
            }
        }

        /// <summary>
        /// Modifie ou valide la modification en cours
        /// en fonction de modif etat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierDvdEx_Click(object sender, EventArgs e)
        {
            if (modifEtat)
            {
                if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    modifEtat = false;
                    btnSupprimerDvdEx.Text = "Supprimer";
                    btnModifierDvdEx.Text = "Modifier";
                    int? a = null;
                    int numero = 0;
                    try
                    {
                        numero = int.Parse(txbDvdNbEx.Text);
                        a = numero;
                    }
                    catch
                    {
                        MessageBox.Show("Le numéro doit etre un entier");
                    }
                    DateTime dateAchat = (DateTime)dtpDvdDateAchatEx.Value;
                    string photo = txbDvdPhotoEx.Text;
                    string idEtat = "";
                    try
                    {
                        idEtat = ((Etat)cbxDvdExEtat.SelectedItem).Id;
                    }
                    catch
                    {
                        MessageBox.Show("Aucun etat selectionné");
                    }
                    string iDocument = txbDvdExId.Text;
                    if (a != null && idEtat != "" && iDocument != "")
                    {
                        Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, iDocument);
                        controller.UpdateExemplaire(exemplaire);
                        enCoursModifDvd(false);
                        lesExemplairesDvd = controller.GetExemplairesRevue(iDocument);
                        RemplirDvdListeExemplaire(lesExemplairesDvd);
                        grpDvdInfos.Enabled = true;
                        Thread.Sleep(20);
                    }
                }
            }
            else // si on est pas en modif
            {
                if (dgvDvdListeEx.CurrentCell != null)
                {
                    btnSupprimerDvdEx.Text = "Annuler";
                    btnModifierDvdEx.Text = "Valider";
                    modifEtat = true;
                    enCoursModifDvd(true);
                    btnAnnulerDvd.Enabled = false;
                    btnValiderDvd.Enabled = false;
                    btnSupprimerDvdEx.Enabled = true;
                    btnModifierDvdEx.Enabled = true;
                    cbxDvdExEtat.Enabled = true;
                    grpDvdInfos.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Aucun exemplaire selectionné");
                }
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du detaille de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListeEx_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListeEx.CurrentCell != null)
            {
                try
                {
                    Exemplaire exemplaire = (Exemplaire)bdgDvdListeEx[bdgDvdListeEx.Position];
                    AfficheDvdExemplaire(exemplaire);
                }
                catch
                {
                    VideDvdExemplaire();
                }
            }
            else
            {
                VideDvdExemplaire();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListeEx_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdExemplaire();
            Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            string titreColonne = dgvDvdListeEx.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = controller.GetExemplairesRevue(dvd.Id);

            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "IdEtat":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.IdEtat).ToList();
                    break;
            }
            RemplirDvdListeExemplaire(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();
        private readonly BindingSource bdgGenresInfoRevues = new BindingSource();
        private readonly BindingSource bdgPublicsInfoRevues = new BindingSource();
        private readonly BindingSource bdgRayonsInfoRevues = new BindingSource();
        private readonly BindingSource bdgRevuesListeEx = new BindingSource();
        private List<Exemplaire> lesExemplairesRevues = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenresInfoRevues, cbxRevuesGenresInfo);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublicsInfoRevues, cbxRevuesPublicInfo);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayonsInfoRevues, cbxRevuesRayonInfo);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxRevuesExEtat);
            lesEtatsEx = controller.GetAllEtats();
            modifEtat = false;
            enCoursModifRevues(false);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires"></param>
        private void RemplirRevuesListeExemplaire(List<Exemplaire> exemplaires)
        {
            if (exemplaires.Count > 0)
            {
                bdgRevuesListeEx.DataSource = exemplaires;
                dgvRevuesListeEx.DataSource = bdgRevuesListeEx;
                dgvRevuesListeEx.Columns["photo"].Visible = false;
                dgvRevuesListeEx.Columns["id"].Visible = false;
                dgvRevuesListeEx.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvRevuesListeEx.Columns["id"].DisplayIndex = 0;
                dgvRevuesListeEx.Columns["numero"].DisplayIndex = 1;
                dgvRevuesListeEx.Columns["dateAchat"].DefaultCellStyle.Format = "d/M/yyyy";
                dgvRevuesListeEx.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //change les idEtats en la valeur de leur libelle 
                for (int i = 0; i < dgvRevuesListeEx.RowCount; i++)
                {
                    if (int.TryParse(dgvRevuesListeEx.Rows[i].Cells["idEtat"].Value.ToString(), out _))
                        dgvRevuesListeEx.Rows[i].Cells["idEtat"].Value = lesEtatsEx.First(o => o.Id == dgvRevuesListeEx.Rows[i].Cells["idEtat"].Value.ToString());
                }
            }
            else
            {
                dgvRevuesListeEx.Columns.Clear();
            }
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            cbxRevuesGenresInfo.SelectedIndex = cbxRevuesGenresInfo.FindString(revue.Genre);
            cbxRevuesPublicInfo.SelectedIndex = cbxRevuesPublicInfo.FindString(revue.Public);
            cbxRevuesRayonInfo.SelectedIndex = cbxRevuesRayonInfo.FindString(revue.Rayon);
            txbRevuesTitre.Text = revue.Titre;
            lesExemplairesRevues = controller.GetExemplairesRevue(revue.Id);
            gbxRevuesEx.Text = "Les exemplaire de " + revue.Titre;
            RemplirRevuesListeExemplaire(lesExemplairesRevues);
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Affichage des informations de l'exemplaire sélectionné
        /// </summary>
        /// <param name="exemplaire"></param>
        private void AfficheRevuesExemplaire(Exemplaire exemplaire)
        {
            txbRevuesNbEx.Text = exemplaire.Numero.ToString();
            txbRevuesPhotoEx.Text = exemplaire.Photo;
            dtpRevuesDateAchatEx.Value = exemplaire.DateAchat;
            if (int.TryParse(exemplaire.IdEtat, out _))
            {
                cbxRevuesExEtat.SelectedIndex = int.Parse(exemplaire.IdEtat) - 1;
            }
            else
            {
                cbxRevuesExEtat.SelectedIndex = cbxRevuesExEtat.FindString(exemplaire.IdEtat);
            }
            txbRevuesExId.Text = exemplaire.Id;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            cbxRevuesGenresInfo.SelectedIndex = -1;
            cbxRevuesPublicInfo.SelectedIndex = -1;
            cbxRevuesRayonInfo.SelectedIndex = -1;
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
            gbxRevuesEx.Text = "";
            grpRevuesInfos.Text = "";
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage d'un exemplaire du livre
        /// </summary>
        private void VideRevuesEx()
        {
            txbRevuesNbEx.Text = "";
            dtpRevuesDateAchatEx.Value = DateTime.Now;
            txbRevuesPhotoEx.Text = "";
            cbxRevuesExEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesEx();
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesEx();
                VideRevuesZones();
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Vide l'affichage du detail d'un exemplaire
        /// </summary>
        private void VideRevuesExemplaire()
        {
            txbRevuesNbEx.Text = "";
            dtpRevuesDateAchatEx.Value = DateTime.Now;
            cbxRevuesExEtat.SelectedIndex = -1;
            txbRevuesPhotoEx.Text = "";
            txbRevuesExId.Text = "";
        }

        /// <summary>
        /// configure l'interface en fonction de la procédure événementielle requise
        /// </summary>
        /// <param name="modif"></param>
        private void enCoursModifRevues(bool modif)
        {
            btnAjouterRevues.Enabled = !modif;
            btnSupprimerRevues.Enabled = !modif;
            btnModifierRevues.Enabled = !modif;
            btnAnnulerRevues.Enabled = modif;
            btnValiderRevues.Enabled = modif;
            txbRevuesTitre.ReadOnly = !modif;
            txbRevuesPeriodicite.ReadOnly = !modif;
            cbxRevuesPublicInfo.Enabled = modif;
            txbRevuesDateMiseADispo.ReadOnly = !modif;
            cbxRevuesGenresInfo.Enabled = modif;
            cbxRevuesRayonInfo.Enabled = modif;
            txbRevuesImage.ReadOnly = !modif;
            txbRevuesNumero.ReadOnly = true;
            dgvRevuesListe.Enabled = !modif;
            cbxRevuesGenres.Enabled = !modif;
            cbxRevuesPublics.Enabled = !modif;
            cbxRevuesRayons.Enabled = !modif;
            btnRevuesNumRecherche.Enabled = !modif;
            txbRevuesTitreRecherche.Enabled = !modif;
            btnRevuesAnnulRayons.Enabled = !modif;
            btnRevuesAnnulGenres.Enabled = !modif;
            btnRevuesAnnulPublics.Enabled = !modif;
            ajouterBool = false;
            txbRevuesNbEx.Enabled = false;
            dtpRevuesDateAchatEx.Enabled = false;
            cbxRevuesExEtat.Enabled = false;
            btnSupprimerRevuesEx.Enabled = !modif;
            btnModifierRevuesEx.Enabled = !modif;
            dgvRevuesListeEx.Enabled = !modif;
        }

        /// <summary>
        /// lance la procédure d'ajout d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterRevues_Click(object sender, EventArgs e)
        {
            enCoursModifRevues(true);
            ajouterBool = true;
            string id = plusUnIdString(controller.getNbRevueMax());
            if (id == "1")
                id = "10001";
            txbRevuesNumero.Text = id;
            txbRevuesTitre.Text = "";
            txbRevuesPeriodicite.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesImage.Text = "";
            cbxRevuesPublicInfo.SelectedIndex = -1;
            cbxRevuesGenresInfo.SelectedIndex = -1;
            cbxRevuesRayonInfo.SelectedIndex = -1;
        }

        /// <summary>
        /// lance la procédure de modification d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierRevues_Click(object sender, EventArgs e)
        {
            enCoursModifRevues(true);
        }

        /// <summary>
        /// lance la procédure de suppresion d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerRevues_Click(object sender, EventArgs e)
        {
            Revue laRevue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            if (MessageBox.Show("Etes vous sur de vouloir supprimer" + laRevue.Titre + " ?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (controller.GetExemplairesRevue(laRevue.Id).Count == 0)
                {

                    if (controller.SupprimerRevue(laRevue))
                    {
                        Thread.Sleep(100);
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Des parutions sont rattachées à cette revue, vous ne pouvez pas la supprimer");
                }

            }
        }

        /// <summary>
        /// annule les modifications en cours (ajout / suppresion)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerRevues_Click(object sender, EventArgs e)
        {
            enCoursModifRevues(false);
            Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            AfficheRevuesInfos(revue);
        }

        /// <summary>
        /// valide les modifications en cours dans la bdd ( ajout / suppresion)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderRevues_Click(object sender, EventArgs e)
        {
            bool checkValid;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbRevuesNumero.Text; ;
                int delaiMiseADispo = 0;
                int? b = null;
                try
                {
                    delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);
                    b = delaiMiseADispo;
                }
                catch
                {
                    MessageBox.Show("Le Numéro de document et le delai de mise a dispo doivent etre des entiers");
                }
                Genre unGenre = (Genre)cbxRevuesGenresInfo.SelectedItem;
                Public unPublic = (Public)cbxRevuesPublicInfo.SelectedItem;
                Rayon unRayon = (Rayon)cbxRevuesRayonInfo.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                string idGenre = (unGenre == null) ? null : unGenre.Id;
                string genre = (unGenre == null) ? null : unGenre.Libelle;
                string idPublic = (unPublic == null) ? null : unPublic.Id;
                string lePublic = (unPublic == null) ? null : unPublic.Libelle;
                string idRayon = (unRayon == null) ? null : unRayon.Id;
                string rayon = (unRayon == null) ? null : unRayon.Libelle;
                string periodicite = txbRevuesPeriodicite.Text;
                if (b != null && titre != "" && genre != null && unPublic != null)
                {
                    Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);
                    if (!ajouterBool)  // si on est en  modification
                        checkValid = controller.UpdateRevue(revue);
                    else      // si on est en creation
                        checkValid = controller.CreerRevue(revue);
                    if (checkValid)
                    {
                        enCoursModifRevues(false);
                        Thread.Sleep(100);
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Supprime un exemplaire ou Annule la modification en cours
        /// en fonction de modif etat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerRevuesEx_Click(object sender, EventArgs e)
        {
            if (modifEtat)
            {
                modifEtat = false;
                btnSupprimerRevuesEx.Text = "Supprimer";
                btnModifierRevuesEx.Text = "Modifier";
                cbxRevuesExEtat.Enabled = false;
                enCoursModifRevues(false);
                grpRevuesInfos.Enabled = true;

            }
            else // si on est pas en modif 
            {
                if (dgvRevuesListeEx.CurrentCell != null)
                {
                    if (MessageBox.Show("Etes vous de supprimer l'exemplaire " + txbRevuesNbEx.Text + " ? ", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int numero = int.Parse(txbRevuesNbEx.Text);
                        DateTime dateAchat = (DateTime)dtpRevuesDateAchatEx.Value;
                        string photo = txbRevuesPhotoEx.Text;
                        string idEtat = ((Etat)cbxRevuesExEtat.SelectedItem).Id;
                        string iDocument = txbRevuesExId.Text;
                        {
                            Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, iDocument);
                            controller.SupprimerExemplaire(exemplaire);
                            lesExemplairesRevues = controller.GetExemplairesRevue(iDocument);
                            RemplirRevuesListeExemplaire(lesExemplairesRevues);
                            Thread.Sleep(20);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Aucun exemplaire selectionné");
                }
            }
        }

        /// <summary>
        /// Modifie ou valide la modification en cours
        /// en fonction de modif etat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierRevuesEx_Click(object sender, EventArgs e)
        {
            if (modifEtat)
            {
                if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    modifEtat = false;
                    btnSupprimerRevuesEx.Text = "Supprimer";
                    btnModifierRevuesEx.Text = "Modifier";
                    int? a = null;
                    int numero = 0;
                    try
                    {
                        numero = int.Parse(txbRevuesNbEx.Text);
                        a = numero;
                    }
                    catch
                    {
                        MessageBox.Show("Le numéro doit etre un entier");
                    }
                    DateTime dateAchat = (DateTime)dtpRevuesDateAchatEx.Value;
                    string photo = txbRevuesPhotoEx.Text;
                    string idEtat = "";
                    try
                    {
                        idEtat = ((Etat)cbxRevuesExEtat.SelectedItem).Id;
                    }
                    catch
                    {
                        MessageBox.Show("Aucun etat selectionné");
                    }
                    string iDocument = txbRevuesExId.Text;
                    if (a != null && idEtat != "" && iDocument != "")
                    {
                        Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, iDocument);
                        controller.UpdateExemplaire(exemplaire);
                        enCoursModifRevues(false);
                        lesExemplairesRevues = controller.GetExemplairesRevue(iDocument);
                        RemplirRevuesListeExemplaire(lesExemplairesRevues);
                        grpRevuesInfos.Enabled = true;
                        Thread.Sleep(20);
                    }
                }
            }
            else // si on est pas en modif
            {
                if (dgvRevuesListeEx.CurrentCell != null)
                {
                    btnSupprimerRevuesEx.Text = "Annuler";
                    btnModifierRevuesEx.Text = "Valider";
                    modifEtat = true;
                    enCoursModifRevues(true);
                    btnAnnulerRevues.Enabled = false;
                    btnValiderRevues.Enabled = false;
                    btnSupprimerRevuesEx.Enabled = true;
                    btnModifierRevuesEx.Enabled = true;
                    cbxRevuesExEtat.Enabled = true;
                    grpRevuesInfos.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Aucun exemplaire selectionné");
                }
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du detaille de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListeEx_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListeEx.CurrentCell != null)
            {
                try
                {
                    Exemplaire exemplaire = (Exemplaire)bdgRevuesListeEx[bdgRevuesListeEx.Position];
                    AfficheRevuesExemplaire(exemplaire);
                }
                catch
                {
                    VideRevuesExemplaire();
                }
            }
            else
            {
                VideRevuesExemplaire();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListeEx_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesExemplaire();
            Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            string titreColonne = dgvRevuesListeEx.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = controller.GetExemplairesRevue(revue.Id);

            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesExemplairesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplairesRevues.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "IdEtat":
                    sortedList = lesExemplairesRevues.OrderBy(o => o.IdEtat).ToList();
                    break;
            }
            RemplirRevuesListeExemplaire(sortedList);
        }

        #endregion

        #region Onglet Parutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cxbReceptionRevueEtatEx);
            lesEtatsEx = controller.GetAllEtats();
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals("") && cxbReceptionRevueEtatEx.SelectedIndex != -1)
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ((Etat)cxbReceptionRevueEtatEx.SelectedItem).Id;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution et selection de l'etat obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        #endregion

        #region Onglet Commandes de livres
        private readonly BindingSource bdgLivresComListe = new BindingSource();
        private readonly BindingSource bdgLivresComListeCommande = new BindingSource();
        private readonly BindingSource bdgLivresComEtat = new BindingSource();
        private List<Livre> lesLivresCom = new List<Livre>();
        private List<CommandeDocument> lesCommandes = new List<CommandeDocument>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivresCom_Enter(object sender, EventArgs e)
        {
            lesLivresCom = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresComGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresComPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresComRayons);
            RemplirComboSuivi(controller.GetAllSuivis(), bdgLivresComEtat, cbxLivresComEtat);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxLivresExEtat);
            enCoursModifLivresCom(false);
            RemplirLivresComListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresComListe(List<Livre> livres)
        {
            bdgLivresComListe.DataSource = livres;
            dgvLivresComListe.DataSource = bdgLivresComListe;
            dgvLivresComListe.Columns["isbn"].Visible = false;
            dgvLivresComListe.Columns["idRayon"].Visible = false;
            dgvLivresComListe.Columns["idGenre"].Visible = false;
            dgvLivresComListe.Columns["idPublic"].Visible = false;
            dgvLivresComListe.Columns["image"].Visible = false;
            dgvLivresComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresComListe.Columns["id"].DisplayIndex = 0;
            dgvLivresComListe.Columns["titre"].DisplayIndex = 1;
            dgvLivresComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresComListeCommandes(List<CommandeDocument> LesCommandes)
        {
            if (LesCommandes.Count > 0)
            {
                bdgLivresComListeCommande.DataSource = LesCommandes;
                dgvLivresComListeCom.DataSource = bdgLivresComListeCommande;
                dgvLivresComListeCom.Columns["idLivreDvd"].Visible = false;
                dgvLivresComListeCom.Columns["idSuivi"].Visible = false;
                dgvLivresComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLivresComListeCom.Columns["id"].DisplayIndex = 0;
                dgvLivresComListeCom.Columns["dateCommande"].DisplayIndex = 1;
                dgvLivresComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvLivresComListeCom.Columns.Clear();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComNbRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresComNumRecherche.Text.Equals(""))
            {
                txbLivresComTitreRecherche.Text = "";
                cbxLivresComGenres.SelectedIndex = -1;
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                Livre livre = lesLivresCom.Find(x => x.Id.Equals(txbLivresComNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresComListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresComListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresComTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresComTitreRecherche.Text.Equals(""))
            {
                cbxLivresComGenres.SelectedIndex = -1;
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                txbLivresComNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivresCom.FindAll(x => x.Titre.ToLower().Contains(txbLivresComTitreRecherche.Text.ToLower()));
                RemplirLivresComListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresComGenres.SelectedIndex < 0 && cbxLivresComPublics.SelectedIndex < 0 && cbxLivresComRayons.SelectedIndex < 0
                    && txbLivresComNumRecherche.Text.Equals(""))
                {
                    RemplirLivresComListeComplete();
                }
            }
        }


        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresComGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresComGenres.SelectedIndex >= 0)
            {
                txbLivresComTitreRecherche.Text = "";
                txbLivresComNumRecherche.Text = "";
                dgvLivresComListe.ClearSelection();
                Genre genre = (Genre)cbxLivresComGenres.SelectedItem;
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                List<Livre> livres = lesLivresCom.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresComListe(livres);

            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresComPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresComPublics.SelectedIndex >= 0)
            {
                txbLivresComTitreRecherche.Text = "";
                txbLivresComNumRecherche.Text = "";
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComGenres.SelectedIndex = -1;
                dgvLivresComListe.ClearSelection();
                Public lePublic = (Public)cbxLivresComPublics.SelectedItem;
                List<Livre> livres = lesLivresCom.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresComListe(livres);
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresComRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresComRayons.SelectedIndex >= 0)
            {
                txbLivresComTitreRecherche.Text = "";
                txbLivresComNumRecherche.Text = "";
                cbxLivresComGenres.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                dgvLivresComListe.ClearSelection();
                Rayon rayon = (Rayon)cbxLivresComRayons.SelectedItem;
                List<Livre> livres = lesLivresCom.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresComListe(livres);
            }
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresCommandeInfos(Livre livre)
        {
            string idLivre = livre.Id;
            VideLivresComInfos();
            Console.WriteLine("coucou AfficheLivresCommandeInfos?");
            lesCommandes = controller.GetCommandesLivres(idLivre);
            grpLivresCommandes.Text = livre.Titre + " de " + livre.Auteur;
            Console.WriteLine("lesCommandes.count = " + lesCommandes.Count.ToString());
            if (lesCommandes.Count == 0)
                VideLivresComInfos();
            RemplirLivresComListeCommandes(lesCommandes);
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresComListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnlivresComAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresComListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresComListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresComListeComplete()
        {
            RemplirLivresComListe(lesLivresCom);
            VideLivresComZones();
        }

        // <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresComZones()
        {
            cbxLivresComGenres.SelectedIndex = -1;
            cbxLivresComRayons.SelectedIndex = -1;
            cbxLivresComPublics.SelectedIndex = -1;
            txbLivresComNumRecherche.Text = "";
            txbLivresComTitreRecherche.Text = "";
            grpLivresCommandes.Text = "";
        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideLivresComInfos()
        {
            txbLivresComNbCommande.Text = "";
            dtpLivresComDateCommande.Value = DateTime.Now.Date;
            txbLivresComMontant.Text = "";
            txbLivresComNbExemplaires.Text = "";
            cbxLivresComEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void enCoursModifLivresCom(bool modif)
        {
            btnLivresComAjouter.Enabled = !modif;
            btnLivresComSupprimer.Enabled = !modif;
            btnLivresComModifier.Enabled = !modif;
            btnLivresComAnnuler.Enabled = modif;
            btnLivresComValider.Enabled = modif;
            txbLivresComNbCommande.ReadOnly = true;
            dtpLivresComDateCommande.Enabled = modif;
            txbLivresComNbExemplaires.ReadOnly = !modif;
            txbLivresComNumLivre.ReadOnly = true;
            txbLivresComMontant.ReadOnly = !modif;
            cbxLivresComEtat.Enabled = modif;
            dgvLivresComListe.Enabled = !modif;
            dgvLivresComListeCom.Enabled = !modif;
            cbxLivresComGenres.Enabled = !modif;
            cbxLivresComPublics.Enabled = !modif;
            cbxLivresComRayons.Enabled = !modif;
            btnLivresComNbRecherche.Enabled = !modif;
            txbLivresComTitreRecherche.Enabled = !modif;
            btnLivresComAnnulRayons.Enabled = !modif;
            btnLivresComAnnulGenres.Enabled = !modif;
            btnlivresComAnnulPublics.Enabled = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// affiche les détailles d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheLivresComInfo(CommandeDocument laCommande)
        {
            txbLivresComNbCommande.Text = laCommande.Id;
            txbLivresComNumLivre.Text = laCommande.IdLivreDvd;
            dtpLivresComDateCommande.Value = laCommande.DateCommande;
            txbLivresComMontant.Text = laCommande.Montant.ToString();
            txbLivresComNbExemplaires.Text = laCommande.NbExemplaire.ToString();
            txbLivresComNumLivre.Text = laCommande.IdLivreDvd;
            cbxLivresComEtat.SelectedIndex = cbxLivresComEtat.FindString(laCommande.Etat);
        }

        /// <summary>
        /// démarre la procédure d'ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAjouter_Click(object sender, EventArgs e)
        {
            enCoursModifLivresCom(true);
            txbLivresComNumLivre.ReadOnly = true;
            ajouterBool = true;
            string id = plusUnIdString(controller.getNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideLivresComInfos();
            cbxLivresComEtat.SelectedIndex = 0;
            txbLivresComNbCommande.Text = id;
            cbxLivresComEtat.Enabled = false;
        }

        /// <summary>
        /// démarre la procédure d'ajout de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivresComListeCom.CurrentCell != null && txbLivresComNbCommande.Text != "")
            {
                List<Suivi> lesSuivi = controller.GetAllSuivis().FindAll(o => o.Id >= ((Suivi)cbxLivresComEtat.SelectedItem).Id).ToList();
                if (lesSuivi.Count > 2)
                    lesSuivi = lesSuivi.FindAll(o => o.Id < 4).ToList();
                enCoursModifLivresCom(true);
                RemplirComboSuivi(lesSuivi, bdgLivresComEtat, cbxLivresComEtat);
                cbxLivresComEtat.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Aucune commande sélectionné");
            }
        }

        /// <summary>
        /// annule les modifications en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComSupprimer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgLivresComListeCommande[bdgLivresComListeCommande.Position];
            if (dgvLivresComListeCom.CurrentCell != null && txbLivresComNbCommande.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                    MessageBox.Show("Une commande livrée ou réglée ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sur de vouloir supprimer la commande n°" + commandeDocument.Id +
                    " concernant " + lesLivresCom.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre + " ?",
                    "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {
                        Thread.Sleep(50);
                        try
                        {
                            Livre livre = (Livre)bdgLivresComListe.List[bdgLivresComListe.Position];
                            AfficheLivresCommandeInfos(livre);
                            txbLivresComNumLivre.Text = livre.Id;
                        }
                        catch
                        {
                            VideLivresComZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une commande");
            }

        }

        /// <summary>
        /// annule la modifications ou l'ajouts en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgLivresComEtat, cbxLivresComEtat);
            enCoursModifLivresCom(false);
            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgLivresComListeCommande[bdgLivresComListeCommande.Position];
                AfficheLivresComInfo(commandeDocument);
            }
            catch
            {
                VideLivresComInfos();
            }
        }


        /// <summary>
        /// valide la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComValider_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbLivresComNbCommande.Text;
                bool checkValid = false;
                DateTime dateCommande = dtpLivresComDateCommande.Value;
                float montant = -1;
                int nbExemplaire = -1;
                try
                {
                    montant = float.Parse(txbLivresComMontant.Text);
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un nombre a virgule");
                }
                try
                {
                    nbExemplaire = int.Parse(txbLivresComNbExemplaires.Text);
                }
                catch
                {
                    MessageBox.Show("Le nombre d'exemplaire doit etre un nombre a entier");
                }
                string idLivreDvd = txbLivresComNumLivre.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxLivresComEtat.SelectedItem;
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                else
                    MessageBox.Show("Veuillez selectionner un etat");
                if (montant != -1 && nbExemplaire != -1 && etat != "")
                {
                    CommandeDocument commandeLivre = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                    if (!ajouterBool)
                        checkValid = controller.UpdateLivreDvdCom(commandeLivre);
                    else
                        checkValid = controller.CreerLivreDvdCom(commandeLivre);
                    if (checkValid)
                    {
                        if (!ajouterBool)
                            RemplirComboSuivi(controller.GetAllSuivis(), bdgLivresComEtat, cbxLivresComEtat);
                        enCoursModifLivresCom(false);
                        try
                        {
                            Livre livre = (Livre)bdgLivresComListe.List[bdgLivresComListe.Position];
                            AfficheLivresCommandeInfos(livre);
                            txbLivresComNumLivre.Text = livre.Id;
                        }
                        catch
                        {
                            VideLivresComZones();
                        }

                    }
                    else
                        MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des commandes du livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresComListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresComListe.List[bdgLivresComListe.Position];
                    AfficheLivresCommandeInfos(livre);
                    txbLivresComNumLivre.Text = livre.Id;
                }
                catch
                {
                    VideLivresComZones();
                }
            }
            else
            {
                txbLivresComNumLivre.Text = "";
                VideLivresComInfos();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresComZones();
            string titreColonne = dgvLivresComListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivresCom.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivresCom.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivresCom.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivresCom.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivresCom.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivresCom.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivresCom.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresComListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du detaille de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListeCom_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresComListeCom.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgLivresComListeCommande[bdgLivresComListeCommande.Position];
                    AfficheLivresComInfo(commandeDocument);
                }
                catch
                {
                    VideLivresComInfos();
                }
            }
            else
            {
                VideLivresComInfos();
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListeCom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandes.Count > 0 && dgvLivresComListeCom != null)
            {
                VideLivresComInfos();
                string titreColonne = dgvLivresComListeCom.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = lesCommandes.OrderBy(o => o.Id).ToList();
                        break;
                    case "DateCommande":
                        sortedList = lesCommandes.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandes.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandes.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandes.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirLivresComListeCommandes(sortedList);
            }
        }


        #endregion
 
        #region Onglet Commandes de Dvd

        private readonly BindingSource bdgDvdComListe = new BindingSource();
        private readonly BindingSource bdgDvdComListeCommande = new BindingSource();
        private readonly BindingSource bdgDvdComEtat = new BindingSource();
        private List<Dvd> lesDvdCom = new List<Dvd>();
        private List<CommandeDocument> lesCommandesDvd = new List<CommandeDocument>();


        /// <summary>
        /// Ouverture de l'onglet Dvd : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabDvdCom_Enter(object sender, EventArgs e)
        {
            lesDvdCom = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdComGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdComPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdComRayons);
            RemplirComboSuivi(controller.GetAllSuivis(), bdgDvdComEtat, cbxDvdComEtat);
            enCoursModifDvdCom(false);
            RemplirDvdComListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirDvdComListe(List<Dvd> livres)
        {
            bdgDvdComListe.DataSource = livres;
            dgvDvdComListe.DataSource = bdgDvdComListe;
            dgvDvdComListe.Columns["synopsis"].Visible = false;
            dgvDvdComListe.Columns["idRayon"].Visible = false;
            dgvDvdComListe.Columns["idGenre"].Visible = false;
            dgvDvdComListe.Columns["idPublic"].Visible = false;
            dgvDvdComListe.Columns["image"].Visible = false;
            dgvDvdComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdComListe.Columns["id"].DisplayIndex = 0;
            dgvDvdComListe.Columns["titre"].DisplayIndex = 1;
            dgvDvdComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirDvdComListeCommandes(List<CommandeDocument> LesCommandes)
        {
            if (LesCommandes.Count > 0)
            {
                bdgDvdComListeCommande.DataSource = LesCommandes;
                dgvDvdComListeCom.DataSource = bdgDvdComListeCommande;
                dgvDvdComListeCom.Columns["idLivreDvd"].Visible = false;
                dgvDvdComListeCom.Columns["idSuivi"].Visible = false;
                dgvDvdComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvDvdComListeCom.Columns["id"].DisplayIndex = 0;
                dgvDvdComListeCom.Columns["dateCommande"].DisplayIndex = 1;
                dgvDvdComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvDvdComListeCom.Columns.Clear();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComNbRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdComNumRecherche.Text.Equals(""))
            {
                txbDvdComTitreRecherche.Text = "";
                cbxDvdComGenres.SelectedIndex = -1;
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                Dvd dvd = lesDvdCom.Find(x => x.Id.Equals(txbDvdComNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> lesDvd = new List<Dvd>() { dvd };
                    RemplirDvdComListe(lesDvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdComListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }
        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbDvdComTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdComTitreRecherche.Text.Equals(""))
            {
                cbxDvdComGenres.SelectedIndex = -1;
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                txbDvdComNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvdCom.FindAll(x => x.Titre.ToLower().Contains(txbDvdComTitreRecherche.Text.ToLower()));
                RemplirDvdComListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdComGenres.SelectedIndex < 0 && cbxDvdComPublics.SelectedIndex < 0 && cbxDvdComRayons.SelectedIndex < 0
                    && txbDvdComNumRecherche.Text.Equals(""))
                {
                    RemplirDvdComListeComplete();
                }
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxDvdComGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdComGenres.SelectedIndex >= 0)
            {
                txbDvdComTitreRecherche.Text = "";
                txbDvdComNumRecherche.Text = "";
                dgvDvdComListe.ClearSelection();
                Genre genre = (Genre)cbxDvdComGenres.SelectedItem;
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                List<Dvd> lesDvd = lesDvdCom.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdComListe(lesDvd);

            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxDvdComPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdComPublics.SelectedIndex >= 0)
            {
                txbDvdComTitreRecherche.Text = "";
                txbDvdComNumRecherche.Text = "";
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComGenres.SelectedIndex = -1;
                dgvDvdComListe.ClearSelection();
                Public lePublic = (Public)cbxDvdComPublics.SelectedItem;
                List<Dvd> lesDvd = lesDvdCom.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdComListe(lesDvd);
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxDvdComRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdComRayons.SelectedIndex >= 0)
            {
                txbDvdComTitreRecherche.Text = "";
                txbDvdComNumRecherche.Text = "";
                cbxDvdComGenres.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                dgvDvdComListe.ClearSelection();
                Rayon rayon = (Rayon)cbxDvdComRayons.SelectedItem;
                List<Dvd> LesDvd = lesDvdCom.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdComListe(LesDvd);
            }
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheDvdCommandeInfos(Dvd dvd)
        {
            string idDvd = dvd.Id;
            VideDvdComInfos();
            lesCommandes = controller.GetCommandesLivres(idDvd);
            grpDvdCommandes.Text = dvd.Titre + " de " + dvd.Realisateur;
            if (lesCommandes.Count == 0)
                VideDvdComInfos();
            RemplirDvdComListeCommandes(lesCommandes);
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdComListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdComListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdComListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdComListeComplete()
        {
            RemplirDvdComListe(lesDvdCom);
            VideDvdComZones();
        }

        // <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdComZones()
        {
            cbxDvdComGenres.SelectedIndex = -1;
            cbxDvdComRayons.SelectedIndex = -1;
            cbxDvdComPublics.SelectedIndex = -1;
            txbDvdComNumRecherche.Text = "";
            txbDvdComTitreRecherche.Text = "";
            grpDvdCommandes.Text = "";
        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideDvdComInfos()
        {
            txbDvdComNbCommande.Text = "";
            dtpDvdComDateCommande.Value = DateTime.Now.Date;
            txbDvdComMontant.Text = "";
            txbDvdComNbExemplaires.Text = "";
            cbxDvdComEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void enCoursModifDvdCom(bool modif)
        {
            btnDvdComAjouter.Enabled = !modif;
            btnDvdComSupprimer.Enabled = !modif;
            btnDvdComModifier.Enabled = !modif;
            btnDvdComAnnuler.Enabled = modif;
            btnDvdComValider.Enabled = modif;
            txbDvdComNbCommande.ReadOnly = true;
            dtpDvdComDateCommande.Enabled = modif;
            txbDvdComNbExemplaires.ReadOnly = !modif;
            txbDvdComNumLivre.ReadOnly = true;
            txbDvdComMontant.ReadOnly = !modif;
            cbxDvdComEtat.Enabled = modif;
            dgvDvdComListe.Enabled = !modif;
            dgvDvdComListeCom.Enabled = !modif;
            cbxDvdComGenres.Enabled = !modif;
            cbxDvdComPublics.Enabled = !modif;
            cbxDvdComRayons.Enabled = !modif;
            btnDvdComNbRecherche.Enabled = !modif;
            txbDvdComTitreRecherche.Enabled = !modif;
            btnDvdComAnnulRayons.Enabled = !modif;
            btnDvdComAnnulGenres.Enabled = !modif;
            btnlivresComAnnulPublics.Enabled = !modif;
            ajouterBool = false;
                
        }

        /// <summary>
        /// affiche les détailles d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheDvdComInfo(CommandeDocument laCommande)
        {
            txbDvdComNbCommande.Text = laCommande.Id;
            txbDvdComNumLivre.Text = laCommande.IdLivreDvd;
            dtpDvdComDateCommande.Value = laCommande.DateCommande;
            txbDvdComMontant.Text = laCommande.Montant.ToString();
            txbDvdComNbExemplaires.Text = laCommande.NbExemplaire.ToString();
            txbDvdComNumLivre.Text = laCommande.IdLivreDvd;
            cbxDvdComEtat.SelectedIndex = cbxDvdComEtat.FindString(laCommande.Etat);
        }

        /// <summary>
        /// démarre la procédure d'ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAjouter_Click(object sender, EventArgs e)
        {
            enCoursModifDvdCom(true);
            txbDvdComNumLivre.ReadOnly = true;
            ajouterBool = true;
            string id = plusUnIdString(controller.getNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideDvdComInfos();
            cbxDvdComEtat.SelectedIndex = 0;
            txbDvdComNbCommande.Text = id;
            cbxDvdComEtat.Enabled = false;
        }

        /// <summary>
        /// démarre la procédure d'ajout de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComModifier_Click(object sender, EventArgs e)
        {
            if (dgvDvdComListeCom.CurrentCell != null && txbDvdComNbCommande.Text != "")
            {
                List<Suivi> lesSuivi = controller.GetAllSuivis().FindAll(o => o.Id >= ((Suivi)cbxDvdComEtat.SelectedItem).Id).ToList();
                if (lesSuivi.Count > 2)
                    lesSuivi = lesSuivi.FindAll(o => o.Id < 4).ToList();
                enCoursModifDvdCom(true);
                RemplirComboSuivi(lesSuivi, bdgDvdComEtat, cbxDvdComEtat);
                cbxDvdComEtat.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Aucune commande sélectionné");
            }
        }

        /// <summary>
        /// annule les modifications en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComSupprimer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgDvdComListeCommande[bdgDvdComListeCommande.Position];
            if (dgvDvdComListeCom.CurrentCell != null && txbDvdComNbCommande.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                    MessageBox.Show("Une commande livrée ou réglée ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sur de vouloir supprimer la commande n°" + commandeDocument.Id +
                    " concernant " + lesDvdCom.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre + " ?",
                    "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {
                        Thread.Sleep(50);
                        try
                        {
                            Dvd dvd = (Dvd)bdgDvdComListe.List[bdgDvdComListe.Position];
                            AfficheDvdCommandeInfos(dvd);
                            txbDvdComNumLivre.Text = dvd.Id;
                        }
                        catch
                        {
                            VideDvdComZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une commande");
            }

        }

        /// <summary>
        /// annule la modifications ou l'ajouts en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgDvdComEtat, cbxDvdComEtat);
            enCoursModifDvdCom(false);
            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgDvdComListeCommande[bdgDvdComListeCommande.Position];
                AfficheDvdComInfo(commandeDocument);
            }
            catch
            {
                VideDvdComInfos();
            }
        }


        /// <summary>
        /// valide la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComValider_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbDvdComNbCommande.Text;
                bool checkValid = false;
                DateTime dateCommande = dtpDvdComDateCommande.Value;
                float montant = -1;
                int nbExemplaire = -1;
                try
                {
                    montant = float.Parse(txbDvdComMontant.Text);
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un nombre a virgule");
                }
                try
                {
                    nbExemplaire = int.Parse(txbDvdComNbExemplaires.Text);
                }
                catch
                {
                    MessageBox.Show("Le nombre d'exemplaire doit etre un nombre a entier");
                }
                string idLivreDvd = txbDvdComNumLivre.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxDvdComEtat.SelectedItem;
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                else
                    MessageBox.Show("Veuillez selectionner un etat");
                if (montant != -1 && nbExemplaire != -1 && etat != "")
                {
                    CommandeDocument commandeLivre = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                    if (!ajouterBool)
                        checkValid = controller.UpdateLivreDvdCom(commandeLivre);
                    else
                        checkValid = controller.CreerLivreDvdCom(commandeLivre);
                    if (checkValid)
                    {
                        if (!ajouterBool)
                            RemplirComboSuivi(controller.GetAllSuivis(), bdgDvdComEtat, cbxDvdComEtat);
                        enCoursModifDvdCom(false);
                        try
                        {
                            Dvd dvd = (Dvd)bdgDvdComListe.List[bdgDvdComListe.Position];
                            AfficheDvdCommandeInfos(dvd);
                            txbDvdComNumLivre.Text = dvd.Id;
                        }
                        catch
                        {
                            VideDvdComZones();
                        }

                    }
                    else
                        MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des commandes du livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdComListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdComListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdComListe.List[bdgDvdComListe.Position];
                    AfficheDvdCommandeInfos(dvd);
                    txbDvdComNumLivre.Text = dvd.Id;
                }
                catch
                {
                    VideDvdComZones();
                }
            }
            else
            {
                txbDvdComNumLivre.Text = "";
                VideDvdComInfos();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdComListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdComZones();
            string titreColonne = dgvDvdComListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvdCom.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvdCom.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvdCom.OrderBy(o => o.Duree).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvdCom.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvdCom.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvdCom.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdComListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du detaille de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdComListeCom_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdComListeCom.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgDvdComListeCommande[bdgDvdComListeCommande.Position];
                    AfficheDvdComInfo(commandeDocument);
                }
                catch
                {
                    VideDvdComInfos();
                }
            }
            else
            {
                VideDvdComInfos();
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdComListeCom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandesDvd.Count > 0 && dgvDvdComListeCom != null)
            {
                VideDvdComInfos();
                string titreColonne = dgvDvdComListeCom.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Id).ToList();
                        break;
                    case "DateCommande":
                        sortedList = lesCommandesDvd.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandesDvd.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandesDvd.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirDvdComListeCommandes(sortedList);
            }
        }
        #endregion

        #region Onglet Gestion des abonnements
        private readonly BindingSource bdgAboListe = new BindingSource();
        private readonly BindingSource bdgAboListeCommande = new BindingSource();
        private List<Revue> lesRevuesAbo = new List<Revue>();
        private List<Abonnement> lesAbonnements = new List<Abonnement>();
        bool filtre;


        /// <summary>
        /// Ouverture de l'onglet Dvd : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabAbo_Enter(object sender, EventArgs e)
        {
            lesRevuesAbo = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxAboGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxAboPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxAboRayons);
            enCoursModifAbo(false);
            RemplirAboListeComplete();
            filtre = false;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirAboListe(List<Revue> revues)
        {
            bdgAboListe.DataSource = revues;
            dgvAboListe.DataSource = bdgAboListe;
            dgvAboListe.Columns["idRayon"].Visible = false;
            dgvAboListe.Columns["idGenre"].Visible = false;
            dgvAboListe.Columns["idPublic"].Visible = false;
            dgvAboListe.Columns["image"].Visible = false;
            dgvAboListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvAboListe.Columns["id"].DisplayIndex = 0;
            dgvAboListe.Columns["titre"].DisplayIndex = 1;
            dgvAboListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirAboListeCommandes(List<Abonnement> LesAbonnements)
        {
            if (LesAbonnements.Count > 0)
            {
                bdgAboListeCommande.DataSource = LesAbonnements;
                dgvAboListeCom.DataSource = bdgAboListeCommande;
                dgvAboListeCom.Columns["idRevue"].Visible = false;
                dgvAboListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvAboListeCom.Columns["id"].DisplayIndex = 0;
                dgvAboListeCom.Columns["dateCommande"].DisplayIndex = 1;
                dgvAboListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvAboListeCom.Columns.Clear();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboNbRecherche_Click(object sender, EventArgs e)
        {
            if (!txbAboNumRecherche.Text.Equals(""))
            {
                txbAboTitreRecherche.Text = "";
                cbxAboGenres.SelectedIndex = -1;
                cbxAboRayons.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                Revue revue = lesRevuesAbo.Find(x => x.Id.Equals(txbAboNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirAboListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirAboListeComplete();
                }
            }
            else
            {
                RemplirAboListeComplete();
            }
        }
        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbAboTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbAboTitreRecherche.Text.Equals(""))
            {
                cbxAboGenres.SelectedIndex = -1;
                cbxAboRayons.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                txbAboNumRecherche.Text = "";
                List<Revue> lesRevueParTitre;
                lesRevueParTitre = lesRevuesAbo.FindAll(x => x.Titre.ToLower().Contains(txbAboTitreRecherche.Text.ToLower()));
                RemplirAboListe(lesRevueParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxAboGenres.SelectedIndex < 0 && cbxAboPublics.SelectedIndex < 0 && cbxAboRayons.SelectedIndex < 0
                    && txbAboNumRecherche.Text.Equals(""))
                {
                    RemplirAboListeComplete();
                }
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxAboGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboGenres.SelectedIndex >= 0)
            {
                txbAboTitreRecherche.Text = "";
                txbAboNumRecherche.Text = "";
                dgvAboListe.ClearSelection();
                Genre genre = (Genre)cbxAboGenres.SelectedItem;
                cbxAboRayons.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                List<Revue> lesRevues = lesRevuesAbo.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirAboListe(lesRevues);

            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxAboPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboPublics.SelectedIndex >= 0)
            {
                txbAboTitreRecherche.Text = "";
                txbAboNumRecherche.Text = "";
                cbxAboRayons.SelectedIndex = -1;
                cbxAboGenres.SelectedIndex = -1;
                dgvAboListe.ClearSelection();
                Public lePublic = (Public)cbxAboPublics.SelectedItem;
                List<Revue> lesRevues = lesRevuesAbo.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirAboListe(lesRevues);
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxAboRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboRayons.SelectedIndex >= 0)
            {
                txbAboTitreRecherche.Text = "";
                txbAboNumRecherche.Text = "";
                cbxAboGenres.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                dgvAboListe.ClearSelection();
                Rayon rayon = (Rayon)cbxAboRayons.SelectedItem;
                List<Revue> lesRevues = lesRevuesAbo.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirAboListe(lesRevues);
            }
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheAboInfos(Revue revue)
        {
            string idRevue = revue.Id;
            VideAboInfos();
            lesAbonnements = controller.GetAbonnements(idRevue);
            grpAboCommandes.Text = revue.Titre ;
            if (lesAbonnements.Count == 0)
                VideAboInfos();
            RemplirAboListeCommandes(lesAbonnements);
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirAboListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirAboListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirAboListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirAboListeComplete()
        {
            RemplirAboListe(lesRevuesAbo);
            VideAboZones();
        }

        // <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideAboZones()
        {
            cbxAboGenres.SelectedIndex = -1;
            cbxAboRayons.SelectedIndex = -1;
            cbxAboPublics.SelectedIndex = -1;
            txbAboNumRecherche.Text = "";
            txbAboTitreRecherche.Text = "";
            grpAboCommandes.Text = "";
        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideAboInfos()
        {
            txbAboNbCommande.Text = "";
            dtpAboDateCommande.Value = DateTime.Now.Date;
            txbAboMontant.Text = "";
            dtpAboDateFin.Value = DateTime.Now.Date.AddMonths(6);
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void enCoursModifAbo(bool modif)
        {
            btnAboAjouter.Enabled = !modif;
            btnAboSupprimer.Enabled = !modif;
            btnAboModifier.Enabled = !modif;
            btnAboAnnuler.Enabled = modif;
            btnAboValider.Enabled = modif;
            txbAboNbCommande.ReadOnly = true;
            dtpAboDateCommande.Enabled = modif;
            txbAboNumRevue.ReadOnly = true;
            txbAboMontant.ReadOnly = !modif;
            dtpAboDateCommande.Enabled = modif;
            dtpAboDateFin.Enabled = modif;
            dgvAboListe.Enabled = !modif;
            dgvAboListeCom.Enabled = !modif;
            cbxAboGenres.Enabled = !modif;
            cbxAboPublics.Enabled = !modif;
            cbxAboRayons.Enabled = !modif;
            btnAboNbRecherche.Enabled = !modif;
            txbAboTitreRecherche.Enabled = !modif;
            btnAboAnnulRayons.Enabled = !modif;
            btnAboAnnulGenres.Enabled = !modif;
            btnAboAnnulPublics.Enabled = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// affiche les détailles d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheAboInfo(Abonnement labonnement)
        {
            txbAboNbCommande.Text = labonnement.Id;
            txbAboNumRevue.Text = labonnement.IdRevue;
            dtpAboDateCommande.Value = labonnement.DateCommande;
            txbAboMontant.Text = labonnement.Montant.ToString();
            dtpAboDateFin.Value = labonnement.DateFinAbonnement;
        }

        /// <summary>
        /// démarre la procédure d'ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAjouter_Click(object sender, EventArgs e)
        {
            enCoursModifAbo(true);
            ajouterBool = true;
            string id = plusUnIdString(controller.getNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideAboInfos();
            dtpAboDateCommande.Value = DateTime.Now;
            dtpAboDateFin.Value = DateTime.Now.AddMonths(2);
            txbAboNbCommande.Text = id;
            Console.WriteLine("btnAboAjouter_Click  " + id);
        }

        /// <summary>
        /// démarre la procédure d'ajout de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboModifier_Click(object sender, EventArgs e)
        {
            if (dgvAboListeCom.CurrentCell != null && txbAboNbCommande.Text != "")
            {
                enCoursModifAbo(true);
            }
            else
            {
                MessageBox.Show("Aucune revue sélectionnée");
            }
        }

        /// <summary>
        /// renvoie vrai si un exemplaire a été aquis pendant la perdiode de l'abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        private bool verrifExemplaireAbo(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplaires = controller.GetExemplairesRevue(abonnement.IdRevue);
            return lesExemplaires.FindAll(o => (o.DateAchat >= abonnement.DateCommande) && (o.DateAchat <= abonnement.DateCommande)).Count > 0;
        }

        /// <summary>
        /// annule les modifications en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboSupprimer_Click(object sender, EventArgs e)
        {
            Abonnement abonnement = (Abonnement)bdgAboListeCommande[bdgAboListeCommande.Position];
            if (dgvAboListeCom.CurrentCell != null && txbAboNbCommande.Text != "")
            {
                if (verrifExemplaireAbo(abonnement))
                    MessageBox.Show("Une revue a été livrée le temps de cet abonnement, il ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sur de vouloir supprimer la commande n°" + abonnement.Id +
                    " concernant " + lesRevuesAbo.Find(o => o.Id == abonnement.IdRevue).Titre + " ?",
                    "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerAbonnement(abonnement))
                    {
                        Thread.Sleep(50);
                        try
                        {
                            Revue Revue = (Revue)bdgAboListe.List[bdgAboListe.Position];
                            AfficheAboInfos(Revue);
                            txbAboNumRevue.Text = Revue.Id;
                        }
                        catch
                        {
                            VideAboZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une abonnement");
            }

        }

        /// <summary>
        /// Filtre les revues dont un abonnement se termine
        /// dans moins de 30 jours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntAboFiltreRevue_Click(object sender, EventArgs e)
        {
            if (filtre)
            {
                lesRevuesAbo = controller.GetAllRevues();
                btnAboFiltreRevue.Text = "Filtrer";
                filtre = false;
            }
            else
            {
                List<Revue> listeTrie = new List<Revue>();
                foreach (Revue revue in lesRevuesAbo)
                {
                    List<Abonnement> abonnements = controller.GetAbonnements(revue.Id);
                    if (abonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddMonths(1))
                            && (o.DateFinAbonnement >= DateTime.Now)).Count > 0)
                        listeTrie.Add(revue);
                }
                lesRevuesAbo = listeTrie;
                btnAboFiltreRevue.Text = "X";
                filtre = true;
            }
            RemplirAboListeComplete();
        }  

        /// <summary>
        /// Filtre les abonnement dont la fin est
        /// dans moins de 30 jours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntAboFiltreAbo_Click(object sender, EventArgs e)
        {
            lesAbonnements = lesAbonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddMonths(1))
                && (o.DateFinAbonnement >= DateTime.Now));
            if (lesAbonnements.Count == 0)
                VideAboInfos();
            RemplirAboListeCommandes(lesAbonnements);
        }

        /// <summary>
        /// annule la modifications ou l'ajouts en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            enCoursModifAbo(false);
            try
            {
                Abonnement abonnement = (Abonnement)bdgAboListeCommande[bdgAboListeCommande.Position];
                AfficheAboInfo(abonnement);
            }
            catch
            {
                VideAboInfos();
            }
        }


        /// <summary>
        /// valide la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboValider_Click(object sender, EventArgs e)
        {
            bool checkValid = false;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbAboNbCommande.Text;
                double montant = 0;
                double? b = null;
                try
                {
                    montant = double.Parse(txbAboMontant.Text);
                    b = montant;
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un chiffre a virgule");
                }
                DateTime dateDeCommande = dtpAboDateCommande.Value;
                DateTime dateFin = dtpAboDateFin.Value;
                string idRevue = txbAboNumRevue.Text;
                if (b != null && dateDeCommande <= dateFin && idRevue != "")
                {
                    Abonnement abonnement = new Abonnement(id, dateDeCommande, montant, dateFin, idRevue);
                    if (!ajouterBool)
                        checkValid = controller.UpdateAbonnement(abonnement);
                    else
                        checkValid = controller.CreerAbonnement(abonnement);
                    if( checkValid)
                    {
                        enCoursModifAbo(false);
                        Thread.Sleep(100);
                        lesRevuesAbo = controller.GetAllRevues();
                        RemplirAboListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des commandes du livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAboListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgAboListe.List[bdgAboListe.Position];
                    AfficheAboInfos(revue);
                    txbAboNumRevue.Text = revue.Id;
                }
                catch
                {
                    VideAboZones();
                }
            }
            else
            {
                txbAboNumRevue.Text = "";
                VideAboInfos();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideAboZones();
            string titreColonne = dgvAboListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevuesAbo.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirAboListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du detaille de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListeCom_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAboListeCom.CurrentCell != null)
            {
                try
                {
                    Abonnement abonnement = (Abonnement)bdgAboListeCommande[bdgAboListeCommande.Position];
                    AfficheAboInfo(abonnement);
                }
                catch
                {
                    VideAboInfos();
                }
            }
            else
            {
                VideAboInfos();
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListeCom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesAbonnements.Count > 0 && dgvAboListeCom != null)
            {
                VideAboInfos();
                string titreColonne = dgvAboListeCom.Columns[e.ColumnIndex].HeaderText;
                List<Abonnement> sortedList = new List<Abonnement>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = lesAbonnements.OrderBy(o => o.Id).ToList();
                        break;
                    case "DateCommande":
                        sortedList = lesAbonnements.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "Montant":
                        sortedList = lesAbonnements.OrderBy(o => o.Montant).ToList();
                        break;
                    case "DateFinAbonnement":
                        sortedList = lesAbonnements.OrderBy(o => o.DateFinAbonnement).ToList();
                        break;
                }
                RemplirAboListeCommandes(sortedList);
            }
        }

        #endregion
    }
}




