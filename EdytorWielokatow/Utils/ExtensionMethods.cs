using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Utils
{
    public static class ExtensionMethods
    {
        public static int Round(this float n)
        {
            return (int)Math.Round(n, 0);
        }
    }
}
