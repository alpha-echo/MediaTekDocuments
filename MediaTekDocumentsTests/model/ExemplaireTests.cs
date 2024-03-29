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
    public class ExemplaireTests
    {
        private const int numero = 2005;
        private const string photo = "https://www.legorafi.fr/wp-content/uploads/2020/01/2005.jpg";
        private static readonly DateTime dateAchat = DateTime.Now.AddYears(-4).AddMonths(-1);
        private const string idEtat = "00001";
        private const string id = "10005";
        private static Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, id);

        [TestMethod()]
        public void ExemplaireTest()
        {
            Assert.AreEqual(numero, exemplaire.Numero, "devrait réussir : numero valorisé");
            Assert.AreEqual(photo, exemplaire.Photo, "devrait réussir : photo valorisé");
            Assert.AreEqual(dateAchat, exemplaire.DateAchat, "devrait réussir : date d'achat valorisée");
            Assert.AreEqual(idEtat, exemplaire.IdEtat, "devrait réussir : idEtat valorisé");
            Assert.AreEqual(id, exemplaire.Id, "devrait réussir : id valorisé");
        }
    }
}