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
    public class SuiviTests
    {
        private const int id = 2;
        private const string etat = "en cours";
        private static readonly Suivi suivi = new Suivi(id, etat);

        [TestMethod()]
        public void SuiviTest()
        {
            Assert.AreEqual(id, suivi.Id, "devrait réussir : id valorisé");
            Assert.AreEqual(etat, suivi.Etat, "devrait réussir : etat valorisé");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual(etat, etat.ToString(), "devrait réussir ");
        }
    }
}