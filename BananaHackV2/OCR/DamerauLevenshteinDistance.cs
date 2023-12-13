using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.OCR
{
    internal static class DamerauLevenshtein
    {
        public static int GetDistance(string s1, string s2)
        {
            if (!(string.IsNullOrEmpty(s1)) && string.IsNullOrEmpty(s2)) {
                return s1.Length;
            }

            if (string.IsNullOrEmpty(s1) && !(string.IsNullOrEmpty(s2))) {
                return s2.Length;
            }

            int len1 = s1.Length;
            int len2 = s2.Length;

            int[,] d = new int[len1 + 1, len2 + 1];

            int cost, del, ins, sub;

            for (int i = 0; i <= d.GetUpperBound(0); i++) {
                d[i, 0] = i;
            }

            for (int i = 0; i <= d.GetUpperBound(1); i++) {
                d[0, i] = i;
            }

            for (int i = 1; i <= d.GetUpperBound(0); i++)
            {
                for (int j = 1; j <= d.GetUpperBound(1); j++)
                {
                    if (s1[i-1] == s2[j-1]) {
                        cost = 0;
                    }
                    else {
                        cost = 1;
                    }

                    del = d[i - 1, j] + 1;
                    ins = d[i, j - 1] + 1;
                    sub = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(del, Math.Min(ins, sub));

                    if (i > 1 && j > 1 && s1[i-1] == s2[j-2] && s1[i-2] == s2[j - 1]) {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }
            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }
    }
}
