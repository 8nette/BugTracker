using System;
using Backend.Models;

namespace Backend.Helpers
{
    public static class ExtensionMethods
    {
        public static Developer WithoutPassword(this Developer user)
        {
            if (user == null) return null;

            user.Password = null;
            return user;
        }
    }
}
