using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace VeraDemoNet
{
    public static class StringUtil
    {
        public static string SafeString(string s)
        {
            if (s is null) return string.Empty;
            if (s.Length > 256) s = s.Substring(0, 253) + "...";
            return Regex.Replace(s, @"[\s\p{C}]+", " ");
        }
    }
}