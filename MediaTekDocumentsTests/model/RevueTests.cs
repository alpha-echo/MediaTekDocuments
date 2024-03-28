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
    public class RevueTests
    {
        private const string id = "00001";
        private const string titre = "Fluide Glacial";
        private const string image = "https://upload.wikimedia.org/wikipedia/fr/f/f7/Fluide_glacial_logo.PNG";
        private const string idGenre = "44444";
        private const string genre = "humour";
        private const string idPublic = "00333";
        private const string lePublic = "adulte";
        private const string idRayon = "7777";
        private const string rayon = "Livre|Littérature|Science-fiction";
        private const string periodicite = "périodique ";
        private const int delaiMiseADispo = 2;
        private static readonly Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic,
            idRayon, rayon, periodicite, delaiMiseADispo);


        [TestMethod()]
        public void RevueTest()
        {
            Assert.AreEqual(id, revue.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(titre, revue.Titre, "devrait réussir : titre valorisé");
            Assert.AreEqual(image, revue.Image, "devrait réussir : image valorisée");
            Assert.AreEqual(idGenre, revue.IdGenre, "devrait réussir : idGenre valorisé");
            Assert.AreEqual(genre, revue.Genre, "devrait réussir : genre valorisé");
            Assert.AreEqual(idPublic, revue.IdPublic, "devrait réussir : idPublic valorisé");
            Assert.AreEqual(lePublic, revue.Public, "devrait réussir : public valorisé");
            Assert.AreEqual(idRayon, revue.IdRayon, "devrait réussir : idRayon valorisé");
            Assert.AreEqual(rayon, revue.Rayon, "devrait réussir : rayon valorisé");
            Assert.AreEqual(periodicite, revue.Periodicite, "devrait réussir : periodicite valorisée");
            Assert.AreEqual(delaiMiseADispo, revue.DelaiMiseADispo, "devrait réussir : delaiMiseADispo valorisé");
        }
    }
}