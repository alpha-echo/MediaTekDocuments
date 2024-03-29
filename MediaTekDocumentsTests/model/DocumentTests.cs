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
    public class DocumentTests
    {
        private const string id = "55555";
        private const string titre = "Le Cinquième Élément";
        private const string image = "c:\\local";
        private const string idGenre = "00005";
        private const string genre = "science-fiction";
        private const string idPublic = "00500";
        private const string lePublic = "multipass";
        private const string idRayon = "cinq";
        private const string rayon = "Le Cinquième";
        private static readonly Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
        [TestMethod()]
        public void DocumentTest()
        {
            Assert.AreEqual(id, document.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(titre, document.Titre, "devrait réussir : titre valorisé");
            Assert.AreEqual(image, document.Image, "devrait réussir : image valorisée");
            Assert.AreEqual(idGenre, document.IdGenre, "devrait réussir : idGenre valorisé");
            Assert.AreEqual(genre, document.Genre, "devrait réussir : genre valorisé");
            Assert.AreEqual(idPublic, document.IdPublic, "devrait réussir : idPublic valorisé");
            Assert.AreEqual(lePublic, document.Public, "devrait réussir : public valorisé");
            Assert.AreEqual(idRayon, document.IdRayon, "devrait réussir : idRayon valorisé");
            Assert.AreEqual(rayon, document.Rayon, "devrait réussir : rayon valorisé");
        }
    }
}