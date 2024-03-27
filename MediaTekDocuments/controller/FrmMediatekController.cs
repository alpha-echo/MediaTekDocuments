using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;
using System.Threading;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        #region Commun
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// Getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// Getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// Getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// Getter sur les etats
        /// </summary>
        /// <returns></returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }
        #endregion


        #region Onglet Livres
        /// <summary>
        /// Getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// Creer un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerLivre(Livre livre)
        {
            return access.CreerEntite("livre", JsonConvert.SerializeObject(livre));
        }

        /// <summary>
        /// Modifie un livre dans la bddd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool UpdateLivre(Livre livre)
        {
            return access.UpdateEntite("livre", livre.Id, JsonConvert.SerializeObject(livre));
        }

        /// <summary>
        /// Supprime un livre dans la bdd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerLivre(Livre livre)
        {
            return access.SupprimerEntite("livre", JsonConvert.SerializeObject(livre));
        }
        #endregion

        #region Onglet DvD
        /// <summary>
        /// Getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// Creer un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerDvd(Dvd dvd)
        {
            return access.CreerEntite("dvd", JsonConvert.SerializeObject(dvd));
        }

        /// <summary>
        /// Modifie un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool UpdateDvd(Dvd dvd)
        {
            return access.UpdateEntite("dvd", dvd.Id, JsonConvert.SerializeObject(dvd));
        }

        /// <summary>
        /// Supprime un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerDvd(Dvd dvd)
        {
            return access.SupprimerEntite("dvd", JsonConvert.SerializeObject(dvd)); ;
        }
        #endregion

        #region Onglet Revues
        /// <summary>
        /// Getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// Creer une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerRevue(Revue revue)
        {
            return access.CreerEntite("revue", JsonConvert.SerializeObject(revue));
        }

        /// <summary>
        /// Modifie une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool UpdateRevue(Revue revue)
        {
            return access.UpdateEntite("revue", revue.Id, JsonConvert.SerializeObject(revue));
        }

        /// <summary>
        /// Supprime une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerRevue(Revue revue)
        {
            return access.SupprimerEntite("revue", JsonConvert.SerializeObject(revue));
        }
        #endregion

        #region Onglet Parutions
        /// <summary>
        /// Récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }
        #endregion

        #region Commandes de livres et Dvd
        /// <summary>
        /// Récupère les commandes d'une livre
        /// </summary>
        /// <param name="idLivre">id du livre concernée</param>
        /// <returns></returns>
        public List<CommandeDocument> GetCommandesLivres(string idLivre)
        {
            return access.GetCommandesLivres(idLivre);
        }

        /// <summary>
        /// Retourne l'id max des commandes
        /// </summary>
        /// <returns></returns>
        public string getNbCommandeMax()
        {
            return access.getMaxIndex("maxcommande");
        }

        /// <summary>
        /// Retourne l'id max des livres
        /// </summary>
        /// <returns></returns>
        public string getNbLivreMax()
        {
            return access.getMaxIndex("maxlivre");
        }

        /// <summary>
        /// Retourne l'id max des Dvd
        /// </summary>
        /// <returns></returns>
        public string getNbDvdMax()
        {
            return access.getMaxIndex("maxdvd");
        }

        /// <summary>
        /// Retourne l'id max des revues
        /// </summary>
        /// <returns></returns>
        public string getNbRevueMax()
        {
            return access.getMaxIndex("maxrevue");
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// une commande de livre ou dvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nbExemplaire"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="idsuivi"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilCommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaire,
            string idLivreDvd, int idSuivi, string etat, string verbose)
        {
            Dictionary<string, object> uneCommandeDocument = new Dictionary<string, object>();
            uneCommandeDocument.Add("Id", id);
            uneCommandeDocument.Add("DateCommande", dateCommande.ToString("yyyy-MM-dd"));
            uneCommandeDocument.Add("Montant", montant);
            uneCommandeDocument.Add("NbExemplaire", nbExemplaire);
            uneCommandeDocument.Add("IdLivreDvd", idLivreDvd);
            uneCommandeDocument.Add("IdSuivi", idSuivi);
            uneCommandeDocument.Add("Etat", etat);

            if (verbose == "post")
                return access.CreerEntite("commandedocument", JsonConvert.SerializeObject(uneCommandeDocument));
            if (verbose == "update")
                return access.UpdateEntite("commandedocument", id, JsonConvert.SerializeObject(uneCommandeDocument));
            if (verbose == "delete")
                return access.SupprimerEntite("commandedocument", JsonConvert.SerializeObject(uneCommandeDocument));
            return false;
        }

        /// <summary>
        /// Creer une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool CreerLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return utilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                    commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "post");
        }
        
        /// <summary>
        /// Modifie une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool UpdateLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return utilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                   commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "update");
        }

        /// <summary>
        /// Supprime une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool SupprimerLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return utilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                   commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "delete");
        }
        #endregion

        #region abonnements

        /// <summary>
        /// Retourne tous les abonnements d'une revue
        /// </summary>
        /// <param name="idRevue"></param>
        /// <returns></returns>
        public List<Abonnement> GetAbonnements(string idRevue)
        {
            return access.GetAbonnements(idRevue);
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// un abonnement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="idRevue"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilAbonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue, string verbose)
        {
            Dictionary<string, object> unAbonnement = new Dictionary<string, object>();
            unAbonnement.Add("Id", id);
            unAbonnement.Add("DateCommande", dateCommande.ToString("yyyy-MM-dd"));
            unAbonnement.Add("Montant", montant);
            unAbonnement.Add("DateFinAbonnement", dateFinAbonnement.ToString("yyyy-MM-dd"));
            unAbonnement.Add("IdRevue", idRevue);

            if (verbose == "post")
                return access.CreerEntite("abonnement", JsonConvert.SerializeObject(unAbonnement));
            if (verbose == "update")
                return access.UpdateEntite("abonnement", id, JsonConvert.SerializeObject(unAbonnement));
            if (verbose == "delete")
                return access.SupprimerEntite("abonnement", JsonConvert.SerializeObject(unAbonnement));
            return false;
        }

        /// <summary>
        /// Creer un abonnement dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return utilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "post");
        }

        /// <summary>
        /// Modifie un abonnement dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool UpdateAbonnement(Abonnement abonnement)
        {
            return utilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "update");
        }

        /// <summary>
        /// Supprime un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            return utilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "delete");
        }

        #endregion
    }
}
