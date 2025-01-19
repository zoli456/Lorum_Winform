using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lórum_Winform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lórum_Winform.Tests
{
    [TestClass()]
    public class MainTests
    {
        [TestMethod()]
        public void Teszt1()
        {
            string kert  = Main.KártyaNév(1);
            string vart = "Piros Hetes";
            Assert.AreEqual(kert, vart);
        }
        [TestMethod()]
        public void Teszt2()
        {
            string kert = Main.KártyaNév(2);
            string vart = "Piros Nyolcas";
            Assert.AreEqual(kert, vart);
        }
        [TestMethod()]
        public void Teszt3()
        {
            string kert = Main.KártyaNév(3);
            string vart = "Piros Kilences";
            Assert.AreEqual(kert, vart);
        }
    }
}