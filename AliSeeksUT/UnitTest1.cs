using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aliseeks.Domain;

namespace AliSeeksUT
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SearchProducts()
        {
            AliexpressAPI api = new AliexpressAPI();
            api.GetItems();
        }
    }
}
