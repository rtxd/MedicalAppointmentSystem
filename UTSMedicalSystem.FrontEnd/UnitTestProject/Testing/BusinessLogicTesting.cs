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
            var result = Common.CalculateUserAge("01/01/2000");

            //assert 
            Assert.AreEqual(18, result,"error calculating the age");
        }
        [TestMethod]
        public void TestingCalculateUserAgeNumber2()
        {
            var result = Common.CalculateUserAge("12/12/2000");

            //Assert
            Assert.AreEqual(17, result, "error calculating the age");
        }
    }
}
