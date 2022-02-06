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
    public partial class Order5Data : System.Web.UI.Page
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

            Dictionary<string, int> Tally = new Dictionary<string, int>();

            string SampleTracker = "";
            //string history = @"";
            string dynamic = @"";
            Random rng = new Random();
            int size = 5;
            for (int i = 1; i <= inputvalue; i++)
            {
                TimeSpan timespan = CalcTimeDiff(StartTime, DateTime.Now);
                SampleTracker = "Sample: " + i;
                Debug.WriteLine(SampleTracker + ", " + timespan.TotalMinutes);
                
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

                //double left = 0;
                //double right = 0;
                //double top = 0;
                //double bottom = 0;
                //foreach (Node node in Polymer.Values)
                //{
                //    left = Math.Min(left, node.X);
                //    right = Math.Max(right, node.X);
                //    top = Math.Max(top, node.Y);
                //    bottom = Math.Min(bottom, node.Y);
                //}
                //dynamic += "<svg height='" + (10 * (top - bottom) + 50) + "' width='" + (10 * (right - left) + 50) + "' viewbox='" + (10 * left - 25) + " " + (10 * bottom - 25) + " " + (10 * (right - left) + 50) + " " + (10 * (top - bottom) + 50) + "'>";
                //for (int j = 1; j <= Polymer.Count; j++)
                //{
                //    dynamic += "<circle cx='" + (Polymer[j].X * 10) + "' cy='" + (Polymer[j].Y * 10) + "' r='" + Polymer[j].Radius * 10 + "' stroke='black' stroke-width='1' fill='none'/><text x='" + ((Polymer[j].X * 10) - 5) + "' y='" + ((Polymer[j].Y * 10) + 5) + "' fill='red'>" + j + "</text> ";
                //    if (Polymer[j].Parent != null)
                //    {
                //        dynamic += "<line x1='" + (10 * Polymer[j].Parent.X) + "' y1='" + (10 * Polymer[j].Parent.Y) + "' x2='" + (10 * Polymer[j].X) + "' y2='" + (10 * Polymer[j].Y) + "' stroke='blue' stroke-width = '1'/>";
                //    }
                //}
                //dynamic += @"</svg>
                
                //";
                if (Polymer[1].Children.Count == 4)
                {
                    double[] angles = new double[4];
                    int[] nodes = new int[4];
                    int n = 0;
                    foreach (Node child in Polymer[1].Children)
                    {
                        angles[n]= Graph.Angle(child);
                        nodes[n] = child.Label;
                        n++;
                    }
                    Array.Sort(angles, nodes);
                    int[] nodeorder = Helper.CycleArray(nodes, nodes.Min());
                    string label = "X1-" + nodeorder[0] + nodeorder[1] + nodeorder[2] + nodeorder[3];
                    if (Tally.ContainsKey(label))
                    {
                        Tally[label]++;
                    }
                    else
                    {
                        Tally.Add(label, 1);
                    }
                }
                else if (Polymer[1].Children.Count == 3)
                {
                    double[] angles = new double[3];
                    int[] nodes = new int[3];
                    int n = 0;
                    int branch = 0;
                    foreach (Node child in Polymer[1].Children)
                    {
                        angles[n] = Graph.Angle(child);
                        nodes[n] = child.Label;
                        if (child.Children.Count == 1)
                        {
                            branch = child.Label;
                        }
                        n++;
                    }
                    Array.Sort(angles, nodes);
                    int[] nodeorder = Helper.CycleArray(nodes, branch);
                    string label = "T" + Polymer[branch].Children[0].Label + nodeorder[0] + "1-" + nodeorder[1] + nodeorder[2];
                    if (Tally.ContainsKey(label))
                    {
                        Tally[label]++;
                    }
                    else
                    {
                        Tally.Add(label, 1);
                    }
                }
                else if (Polymer[1].Children.Count == 2)
                {
                    int hub = 0;
                    int stub = 0;
                    foreach (Node child in Polymer[1].Children)
                    {
                        if (child.Children.Count == 2)
                        {
                            hub = child.Label;
                        }
                        if (child.Children.Count == 0)
                        {
                            stub = child.Label;
                        }
                    }
                    if (hub > 0)
                    {
                        int other = 0;
                        if (Polymer[1].Children[0].Label == hub)
                        {
                            other = Polymer[1].Children[1].Label;
                        }
                        else
                        {
                            other = Polymer[1].Children[0].Label;
                        }
                        double[] angles = new double[3];
                        int[] nodes = new int[3];
                        int n = 0;
                        foreach (Node child in Polymer[hub].Children)
                        {
                            angles[n] = Graph.Angle(child);
                            nodes[n] = child.Label;
                            n++;
                        }
                        angles[n] = Graph.AntiAngle(Polymer[hub]);
                        nodes[n] = 1;
                        Array.Sort(angles, nodes);
                        int[] nodeorder = Helper.CycleArray(nodes, 1);
                        string label = "T" + other + "1" + hub + "-" + nodeorder[1] + nodeorder[2];
                        if (Tally.ContainsKey(label))
                        {
                            Tally[label]++;
                        }
                        else
                        {
                            Tally.Add(label, 1);
                        }
                    }
                    else
                    {
                        string label = "";
                        if (stub > 0)
                        {
                            int other = 0;
                            if (Polymer[1].Children[0].Label == stub)
                            {
                                other = Polymer[1].Children[1].Label;
                            }
                            else
                            {
                                other = Polymer[1].Children[0].Label;
                            }
                            label = "C" + stub + "1" + other + Polymer[other].Children[0].Label + Polymer[other].Children[0].Children[0].Label;
                        }
                        else
                        {
                            if (Polymer[1].Children[0].Label < Polymer[1].Children[1].Label)
                            {
                                label = "C" + Polymer[1].Children[0].Children[0].Label + Polymer[1].Children[0].Label + "1" + Polymer[1].Children[1].Label + Polymer[1].Children[1].Children[0].Label;
                            }
                            else
                            {
                                label = "C" + Polymer[1].Children[1].Children[0].Label + Polymer[1].Children[1].Label + "1" + Polymer[1].Children[0].Label + Polymer[1].Children[0].Children[0].Label;
                            }
                        }
                        if (Tally.ContainsKey(label))
                        {
                            Tally[label]++;
                        }
                        else
                        {
                            Tally.Add(label, 1);
                        }
                    }
                }
                else //Polymer[1].Children.Count == 1
                {
                    string label = "";
                    if (Polymer[1].Children[0].Children.Count == 3)
                    {
                        double[] angles = new double[4];
                        int[] nodes = new int[4];
                        int n = 0;
                        foreach (Node child in Polymer[1].Children[0].Children)
                        {
                            angles[n] = Graph.Angle(child);
                            nodes[n] = child.Label;
                            n++;
                        }
                        angles[n] = Graph.AntiAngle(Polymer[1].Children[0]);
                        nodes[n] = 1;
                        Array.Sort(angles, nodes);
                        int[] nodeorder = Helper.CycleArray(nodes, 1);
                        label = "X" + Polymer[1].Children[0].Label + "-1" + nodeorder[1] + nodeorder[2] + nodeorder[3];
                    }
                    else if (Polymer[1].Children[0].Children.Count == 2)
                    {
                        int branch = 0;
                        if (Polymer[1].Children[0].Children[0].Children.Count == 1)
                        {
                            branch = Polymer[1].Children[0].Children[0].Label;
                        }
                        else
                        {
                            branch = Polymer[1].Children[0].Children[1].Label;
                        }
                        double[] angles = new double[3];
                        int[] nodes = new int[3];
                        int n = 0;
                        foreach (Node child in Polymer[1].Children[0].Children)
                        {
                            angles[n] = Graph.Angle(child);
                            nodes[n] = child.Label;
                            n++;
                        }
                        angles[n] = Graph.AntiAngle(Polymer[1].Children[0]);
                        nodes[n] = 1;
                        Array.Sort(angles, nodes);
                        int[] nodeorder = Helper.CycleArray(nodes, branch);
                        label = "T" + Polymer[branch].Children[0].Label + branch + Polymer[1].Children[0].Label + "-" + nodeorder[1] + nodeorder[2];
                    }
                    else //Polymer[1].Children[0].Children.Count == 1
                    {
                        if (Polymer[1].Children[0].Children[0].Children.Count == 1)
                        {
                            label = "C1" + Polymer[1].Children[0].Label + Polymer[1].Children[0].Children[0].Label + Polymer[1].Children[0].Children[0].Children[0].Label + Polymer[1].Children[0].Children[0].Children[0].Children[0].Label;
                        }
                        else
                        {
                            double[] angles = new double[3];
                            int[] nodes = new int[3];
                            int n = 0;
                            foreach (Node child in Polymer[1].Children[0].Children[0].Children)
                            {
                                angles[n] = Graph.Angle(child);
                                nodes[n] = child.Label;
                                n++;
                            }
                            angles[n] = Graph.AntiAngle(Polymer[1].Children[0].Children[0]);
                            nodes[n] = Polymer[1].Children[0].Label;
                            Array.Sort(angles, nodes);
                            int[] nodeorder = Helper.CycleArray(nodes, Polymer[1].Children[0].Label);
                            label = "T1" + Polymer[1].Children[0].Label + Polymer[1].Children[0].Children[0].Label + "-" + nodeorder[1] + nodeorder[2];
                        }
                    }
                    if (Tally.ContainsKey(label))
                    {
                        Tally[label]++;
                    }
                    else
                    {
                        Tally.Add(label, 1);
                    }
                }
            }

            // output html...
            divOutput.InnerHtml = "Input Value: " + inputvalue.ToString();
            divOutput.InnerHtml += "<br/>";
            string path = @"C:\Users\juggl\Documents\Math\Research\" + DateTime.Now.ToString("yyyyMMddHHmm") + "_OutputOrder5.txt";
            string datalist = @"";
            foreach (KeyValuePair<string, int> entry in Tally)
            {
                datalist += entry.Key + ": " + entry.Value + @"
";
            }
            File.WriteAllText(path, datalist);


            foreach (KeyValuePair<string, int> entry in Tally)
            {
                divOutput.InnerHtml += entry.Key + ": " + entry.Value + @"
                <br>
                ";
            }


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