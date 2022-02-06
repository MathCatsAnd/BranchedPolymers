using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomBranchedPolymer
{
    public class Helper
    {
        public static Node GetNode(Dictionary<int, Node> Polymer, int key)
        {
            Node tempnode;
            Polymer.TryGetValue(key, out tempnode);
            return tempnode;
        }
        public static double FindAngle(double X0, double Y0, double X1, double Y1)
        {
            if (X0 == X1)
            {
                if (Y0 > Y1)
                {
                    return 3 * Math.PI / 2;
                }
                else
                {
                    return Math.PI / 2;
                }
            }
            else
            {
                double arctan = Math.Atan2(Y1 - Y0, X1 - X0);
                if (Y1 - Y0 < 0)
                {
                    return arctan + Math.PI;
                }
                else
                {
                    if (arctan < 0)
                    {
                        return arctan + 2 * Math.PI;
                    }
                    else
                    {
                        return arctan;
                    }
                }
            }
        }
        public static int[] CycleArray(int[] nodeorder, int target)
        {
            int i = 0;
            while (nodeorder[i] != target)
            {
                i++;
            }
            int[] neworder = new int[nodeorder.Length];
            for (int j = 0; j < nodeorder.Length; j++)
            {
                neworder[j] = nodeorder[(i+j)%nodeorder.Length];
            }
            return neworder;
        }
    }
}