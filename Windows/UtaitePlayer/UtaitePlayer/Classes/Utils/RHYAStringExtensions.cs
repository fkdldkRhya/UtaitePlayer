﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.Utils
{
    public static class RHYAStringExtensions
    {
        public static bool Contains(this string str, string substring, StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring", "substring cannot be null.");
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison", "comp");

            return str.IndexOf(substring, comp) >= 0;
        }
    }
}
