
namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        /// <summary>D
        /// Valorise les propriétés
        /// </summary>
        /// <param name="idpersonnel"></param>
        /// <param name="nom"></param>
        /// <param name="prenom"></param>
        /// <param name="mail"></param>
        /// <param name="idService"></param>
        /// <param name="service"></param>
        public Utilisateur(string id, string nom, string prenom, string mail, string idService, string service)
        {
            this.Id = id;
            this.Nom = nom;
            this.Prenom = prenom;
            this.Mail = mail;
            this.Service = new Service(idService, service);
        }
        public string Id { get; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Mail { get; set; }
        public Service Service { get; }
    }
}
