using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthServer.Infrastructure.Common
{
    public class WebHelper
    {
        public static string GenerateCode()
        {
            var pCode = "";

            //pCode = RadomCode();

            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            pCode = new string(Enumerable.Repeat(chars, 6)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());

            return pCode;
        }
    }
}
