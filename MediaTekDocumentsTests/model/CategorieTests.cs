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
    public class CategorieTests
    {
        private const string id = "002";
        private const string libelle = "Horreure";
        private static readonly Categorie categorie = new Genre(id, libelle);

        [TestMethod()]
        public void CategorieTest()
        {
            Assert.AreEqual(id, categorie.Id, "devrait r�ussir : id valoris�");
            Assert.AreEqual(libelle, categorie.Libelle, "devrait r�ussir : libell� valoris�");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual(libelle, categorie.ToString(), "devrait r�ussir ");
        }
    }
}