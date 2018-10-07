using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UTSMedicalSystem.FrontEnd.BusinessLogic
{
    public static class Common
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal currentUser = user;

            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal currentUser = user;

            return currentUser.FindFirst(ClaimTypes.Role).Value;
        }
    }
}
