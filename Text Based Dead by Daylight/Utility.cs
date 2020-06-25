using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Text_Based_Dead_by_Daylight
{
    public class Utility
    {
        public static int GetDistance(Point a, Point b)
        {
            var hor = Math.Abs(b.X - a.X);
            var ver = Math.Abs(a.Y - b.Y);
            return hor + ver;
        }
    }
}
