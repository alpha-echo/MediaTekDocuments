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
    public class GenreTests
    {
        private const string id = "002";
        private const string libelle = "libelle";
        private static readonly Genre genre = new Genre(id, libelle);

        [TestMethod()]
        public void GenreTest()
        {
            Assert.AreEqual(id, genre.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, genre.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}