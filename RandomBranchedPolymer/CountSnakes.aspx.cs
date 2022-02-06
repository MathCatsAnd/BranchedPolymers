using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RandomBranchedPolymer;

namespace RandomBranchedPolymer
{
    public partial class CountSnakes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Gets values from Form, if available
            NameValueCollection nvc = Request.Form;

            string InputValueString = "";
            int inputvalue = 3;

            // see if we have a value from the text box
            if (!string.IsNullOrEmpty(nvc["txtInputValue"]))
            {
                // store in string...
                InputValueString = nvc["txtInputValue"];

                if (!int.TryParse(InputValueString, out inputvalue))
                {
                    // can't do anything...
                    divOutput.InnerHtml = "Error: Invalid Input - Input must be an integer.";

                    // exit without continuing
                    return;
                }
            }

            DateTime StartTime = DateTime.Now;

            // start LogFile
            //string filename = "C:\\Users\\Public\\OutputLog_" + StartTime.ToString("yyyyMMddHHmmss") + ".log";
            //FileStream traceLog = new FileStream(filename, FileMode.OpenOrCreate);
            //System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(traceLog));
            //System.Diagnostics.Trace.AutoFlush = true;

            List<int[]> Neighbors = new List<int[]>();
            Neighbors.Add(new int[2] { 0, 1 });
            Neighbors.Add(new int[2] { 1, 1 });
            Neighbors.Add(new int[2] { 1, 0 });
            Neighbors.Add(new int[2] { 0, -1 });
            Neighbors.Add(new int[2] { -1, -1 });
            Neighbors.Add(new int[2] { -1, 0 });

            Dictionary<int, int[]> Polymer = new Dictionary<int, int[]>();
            for (int i = 1; i <= inputvalue; i++)
            {
                Polymer.Add(i, new int[2] { 0, 0 });
            }

            if (inputvalue == 1)
            {
                divOutput.InnerHtml += 1;
            }
            else if (inputvalue == 2)
            {
                divOutput.InnerHtml += 6;
            }
            else if (inputvalue == 3)
            {
                divOutput.InnerHtml += 30;
            }
            else
            {
                Polymer[4][0] = 1;
                Polymer[4][1] = 1;

                Polymer[1][0] = 0;
                Polymer[1][1] = 1;
                Polymer[2][0] = -1;
                Polymer[2][1] = 0;
                int total = 2*Tally(Neighbors, Polymer, 4, inputvalue);

                Polymer[1][0] = 0;
                Polymer[1][1] = 1;
                Polymer[2][0] = -1;
                Polymer[2][1] = -1;
                total = total + 2 * Tally(Neighbors, Polymer, 4, inputvalue);

                Polymer[1][0] = 0;
                Polymer[1][1] = 1;
                Polymer[2][0] = 0;
                Polymer[2][1] = -1;
                total = total + 2 * Tally(Neighbors, Polymer, 4, inputvalue);

                Polymer[1][0] = 0;
                Polymer[1][1] = 1;
                Polymer[2][0] = 1;
                Polymer[2][1] = 0;
                total = total + Tally(Neighbors, Polymer, 4, inputvalue);

                Polymer[1][0] = -1;
                Polymer[1][1] = 0;
                Polymer[2][0] = -1;
                Polymer[2][1] = -1;
                total = total + 2 * Tally(Neighbors, Polymer, 4, inputvalue);

                Polymer[1][0] = -1;
                Polymer[1][1] = 0;
                Polymer[2][0] = 0;
                Polymer[2][1] = -1;
                total = total + Tally(Neighbors, Polymer, 4, inputvalue);


                string path = @"C:\Users\juggl\Documents\Math\Research\" + "CountSnakes.txt";
                string datalist = "Total: " + 6 * total + " = 6 * " + total;
                File.WriteAllText(path, datalist);
                divOutput.InnerHtml += "Total: " + 6 * total + " = 6*" + total;
            }


            // End Log File
            //System.Diagnostics.Trace.Flush();
        }

        public static int Tally(List<int[]> Neighbors, Dictionary<int,int[]> Polymer, int Last, int n)
        {
            if (Last == n)
            {
                return 1;
            }
            else
            {
                int subtotal = 0;
                foreach (int[] Coordinate in Neighbors)
                {
                    int[] Next = new int[2] { Polymer[Last][0] + Coordinate[0], Polymer[Last][1] + Coordinate[1] };
                    bool match = false;
                    for (int i=1; i <= Last; i++)
                    {
                        if (Polymer[i][0] == Next[0] && Polymer[i][1] == Next[1])
                        {
                            match = true;
                        }
                    }
                    if (match == false)
                    {
                        Polymer[Last+1][0] = Next[0];
                        Polymer[Last + 1][1] = Next[1];
                        subtotal = subtotal + Tally(Neighbors, Polymer, Last+1, n);
                        Polymer[Last + 1][0] = 0;
                        Polymer[Last + 1][1] = 0;
                    }
                }
                return subtotal;
            }
        }

        public static TimeSpan CalcTimeDiff(DateTime StartTime, DateTime EndTime)
        {
            return (EndTime - StartTime);
        }
    }
}