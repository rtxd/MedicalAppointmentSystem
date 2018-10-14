using UTSMedicalSystem.FrontEnd.Controllers;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UTSMedicalSystem.FrontEnd.BusinessLogic;

namespace UnitTestProject.Testing
{
    [TestClass]
    public class BusinessLogicTesting
    {
        //Testing BusinessLogic
        [TestMethod]
        public void TestingCalculateUserAgeType()
        {
            var result = Common.CalculateUserAge("01/01/2000");
            //assert 
            Assert.IsInstanceOfType(result, typeof(int));
        }
        [TestMethod]
        public void TestingCalculateUserAgeNumber()
        {
            var result1 = Common.CalculateUserAge("01/01/2000");
            var result2 = Common.CalculateUserAge("12/01/2000");
            //assert 
            Assert.AreEqual<int>(18, result1,"error calculating the age");
            Assert.AreEqual<int>(17, result2, "error calculating the age");
        }
    }
}
