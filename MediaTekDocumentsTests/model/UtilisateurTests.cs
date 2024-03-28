using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class UtilisateurTests
    {
        private const string id = "11111";
        private const string nom = "François ";
        private const string prenom = "Le Lionnais";
        private const string mail = "francois69Lionnais@fenouil.com";
        private const string idService = "01010";
        private const string service = "Fenouil";
        private static readonly Utilisateur utilisateur = new Utilisateur(id, nom, prenom, mail, idService, service);

        [TestMethod()]
        public void UtilisateurTest()
        {
            Assert.AreEqual(id, utilisateur.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(nom, utilisateur.Nom, "devrait réussir : nom valorisé");
            Assert.AreEqual(prenom, utilisateur.Prenom, "devrait réussir : prenom valorisé");
            Assert.AreEqual(mail, utilisateur.Mail, "devrait réussir : mail valorisé");
            Assert.AreEqual(idService, utilisateur.Service.Id, "devrait réussir : idService valorisé");
            Assert.AreEqual(service, utilisateur.Service, "devrait réussir : service valorisé");
        }
    }
}