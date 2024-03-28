
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier liée à la table Service
    /// </summary>
    public class Service
    {
        public string Id { get; }
        public string Libelle { get; }

        /// <summary>
        /// Valorise les propriétés
        /// </summary>
        /// <param name="idservice"></param>
        /// <param name="nom"></param>
        public Service(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;

        }

        /// <summary>
        /// Définit l'information à afficher (juste le nom)
        /// </summary>
        /// <returns>Nom du service</returns>
        public override string ToString()
        {
            return this.Libelle;
        }
    }
}
