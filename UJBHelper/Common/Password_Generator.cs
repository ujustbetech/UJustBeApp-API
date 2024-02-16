using System;
using System.Linq;

namespace UJBHelper.Common
{
    public static class Password_Generator
    {
        public static string Generate_New_Password()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());   
        }
    }
}
