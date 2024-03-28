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
    public class PublicTests
    {
        private const string id = "002";
        private const string libelle = "libelle";
        private static readonly Public lepublic = new Public(id, libelle);

        [TestMethod()]
        public void PublicTest()
        {
            Assert.AreEqual(id, lepublic.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(libelle, lepublic.Libelle, "devrait réussir : libellé valorisé");
        }
    }
}