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
    public partial class Order3Data : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Gets values from Form, if available
            NameValueCollection nvc = Request.Form;

            string InputValueString = "";
            int inputvalue = 10;

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
            int w123 = 0;
            int w132 = 0;
            int w213 = 0;
            //string ConnectionTracker = "";
            //string history = @"";
            string dynamic = @"";
            Random rng = new Random();
            int size = 3;
            for (int i = 1; i <= inputvalue; i++)
            {
                Dictionary<int, Node> Polymer = new Dictionary<int, Node>();
                Polymer.Add(1, new Node(1, 1));
                for (int k = 2; k <= size; k++)
                {
                    Polymer.Add(k, new Node(k, 1));
                    int attach = rng.Next(1, k);
                    Polymer[attach].Children.Add(Polymer[k]);
                    Polymer[k].Parent = Polymer[attach];
                    double angle = rng.NextDouble() * 2 * Math.PI;
                    Polymer[k].X = Polymer[attach].X + Polymer[attach].Radius * Math.Cos(angle);
                    Polymer[k].Y = Polymer[attach].Y + Polymer[attach].Radius * Math.Sin(angle);

                    List<Node> BaseBranchMembers = new List<Node>();
                    NodeGroup BaseBranch = new NodeGroup(1, Polymer[attach], BaseBranchMembers);
                    Dictionary<Node, NodeGroup> BranchLookup = new Dictionary<Node, NodeGroup>();
                    for (int j = 1; j < k; j++)
                    {
                        BaseBranchMembers.Add(Polymer[j]);
                        BranchLookup.Add(Polymer[j], BaseBranch);
                    }
                    double radius = Polymer[k].Radius;
                    Polymer[k].Radius = 0;
                    List<NodeGroup> Branches = new List<NodeGroup>() { BaseBranch };
                    while (!Graph.DetectCollision(Polymer, Branches, BranchLookup, Polymer[k], radius))
                    {
                        //Debug Output
                    }
                }

                double left = 0;
                double right = 0;
                double top = 0;
                double bottom = 0;
                foreach (Node node in Polymer.Values)
                {
                    left = Math.Min(left, node.X);
                    right = Math.Max(right, node.X);
                    top = Math.Max(top, node.Y);
                    bottom = Math.Min(bottom, node.Y);
                }
                dynamic += "<svg height='" + (10 * (top - bottom) + 50) + "' width='" + (10 * (right - left) + 50) + "' viewbox='" + (10 * left - 25) + " " + (10 * bottom - 25) + " " + (10 * (right - left) + 50) + " " + (10 * (top - bottom) + 50) + "'>";
                for (int j = 1; j <= Polymer.Count; j++)
                {
                    dynamic += "<circle cx='" + (Polymer[j].X * 10) + "' cy='" + (Polymer[j].Y * 10) + "' r='" + Polymer[j].Radius * 10 + "' stroke='black' stroke-width='1' fill='none'/><text x='" + ((Polymer[j].X * 10) - 5) + "' y='" + ((Polymer[j].Y * 10) + 5) + "' fill='red'>" + j + "</text> ";
                    if (Polymer[j].Parent != null)
                    {
                        dynamic += "<line x1='" + (10 * Polymer[j].Parent.X) + "' y1='" + (10 * Polymer[j].Parent.Y) + "' x2='" + (10 * Polymer[j].X) + "' y2='" + (10 * Polymer[j].Y) + "' stroke='blue' stroke-width = '1'/>";
                    }
                }
                dynamic += @"</svg>
                
                ";
                if (Polymer[1].Children.Count == 2)
                {
                    w213++;
                }
                else if (Polymer[2].Children.Count == 1)
                {
                    w123++;
                }
                else
                {
                    w132++;
                }
            }

            // output html...
            divOutput.InnerHtml = "Input Value: " + inputvalue.ToString();
            divOutput.InnerHtml += "<br/>";
            divOutput.InnerHtml += "123: " + w123 + ", 132: " + w132 + ", 213: " + w213;
            divOutput.InnerHtml += "<br/>";
            divOutput.InnerHtml += dynamic;
            divOutput.InnerHtml += "<br/>";
            //divOutput.InnerHtml += process;
            //divOutput.InnerHtml += "<br/>";
            divOutput.InnerHtml += "Other Static String: " + inputvalue.ToString();


            // End Log File
            //System.Diagnostics.Trace.Flush();
        }

        public static TimeSpan CalcTimeDiff(DateTime StartTime, DateTime EndTime)
        {
            return (EndTime - StartTime);
        }
    }
}