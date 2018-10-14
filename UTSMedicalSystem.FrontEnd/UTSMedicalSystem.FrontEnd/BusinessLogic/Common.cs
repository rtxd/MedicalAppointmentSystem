using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UTSMedicalSystem.FrontEnd.Data;
using UTSMedicalSystem.FrontEnd.Models;

namespace UTSMedicalSystem.FrontEnd.BusinessLogic
{
    public static class Common
    {

        /// <summary>
        /// This returns the AspNetUserId for the currently
        /// logged in user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserAspNetId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal currentUser = user;

            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static string GetUserRole(MedicalSystemContext _context, ClaimsPrincipal loggedInUser)
        {
            var userRole = from User in _context.Users
                           where User.AspNetUserId == loggedInUser.FindFirst(ClaimTypes.NameIdentifier).Value
                           select User.Role;

            foreach (var role in userRole) return role;
            return null;
        }

        public static int CalculateUserAge(User user)
        {
            var today = DateTime.Today;
            DateTime birthdate = DateTime.Parse(user.DOB);
            var age = today.Year - birthdate.Year;
            if (birthdate > today.AddYears(-age)) age--;
            return age;
        }
        
        
    }
}
