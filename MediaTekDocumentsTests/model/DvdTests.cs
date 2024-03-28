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
    public class DvdTests
    {
        private const string id = "55555";
        private const string titre = "Le Cinquième Élément";
        private const string image = "https://fr.wikipedia.org/wiki/Le_Cinqui%C3%A8me_%C3%89l%C3%A9ment";
        private const int duree = 126;
        private const string realisateur = "Luc Besson";
        private const string synopsis = "Egypte, 1914. Des extraterrestres récupèrent quatre pierres magiques," +
            " symboles des quatre éléments, jadis confiées à des prêtres. Avant de partir," +
            " les extraterrestres promettent que dans 300 ans, ils rapporteront les précieux cailloux." +
            " Au XXIIIe siècle, alors qu'ils font route vers la Terre, ils sont anéantis par la planète du Mal." +
            " Les habitants de ce monde maléfique, les Mangalores, s'emparent des pierres et foncent vers la Terre.";
        private const string idGenre = "00005";
        private const string genre = "science-fiction";
        private const string idPublic = "00500";
        private const string lePublic = "multipass";
        private const string idRayon = "cinq";
        private const string rayon = "Le Cinquième";
        private static Dvd leDvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic,
            lePublic, idRayon, rayon); 
        [TestMethod()]
        public void DvdTest()
        {
            Assert.AreEqual(id, leDvd.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(titre, leDvd.Titre, "devrait réussir : titre valorisé");
            Assert.AreEqual(image, leDvd.Image, "devrait réussir : image valorisée");
            Assert.AreEqual(duree, leDvd.Duree, "devrait réussir : duree valorisée");
            Assert.AreEqual(realisateur, leDvd.Realisateur, "devrait réussir : realisateur valorisé");
            Assert.AreEqual(synopsis, leDvd.Synopsis, "devrait réussir : synopsis valorisé");
            Assert.AreEqual(idGenre, leDvd.IdGenre, "devrait réussir : idGenre valorisé");
            Assert.AreEqual(genre, leDvd.Genre, "devrait réussir : genre valorisé");
            Assert.AreEqual(idPublic, leDvd.IdPublic, "devrait réussir : idPublic valorisé");
            Assert.AreEqual(lePublic, leDvd.Public, "devrait réussir : public valorisé");
            Assert.AreEqual(idRayon, leDvd.IdRayon, "devrait réussir : idRayon valorisé");
            Assert.AreEqual(rayon, leDvd.Rayon, "devrait réussir : rayon valorisé");
        }
    }
}