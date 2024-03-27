using System;


namespace MediaTekDocuments.model
{
    public class Commande
    {
        public string Id { get; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; }

        public Commande(string id, DateTime dateCommande, double montant)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
        }
    }
}
