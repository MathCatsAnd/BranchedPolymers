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
    public partial class Order4Data : System.Web.UI.Page
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
            int w1234 = 0;
            int w1243 = 0;
            int w1324 = 0;
            int w1342 = 0;
            int w1423 = 0;
            int w1432 = 0;
            int w2134 = 0;
            int w2143 = 0;
            int w3124 = 0;
            int w3142 = 0;
            int w4123 = 0;
            int w4132 = 0;
            int t1c234 = 0;
            int t1c243 = 0;
            int t2c134 = 0;
            int t2c143 = 0;
            int t3c124 = 0;
            int t3c142 = 0;
            int t4c123 = 0;
            int t4c132 = 0;
            string SampleTracker = "";
            //string history = @"";
            string dynamic = @"";
            Random rng = new Random();
            int size = 4;
            for (int i = 1; i <= inputvalue; i++)
            {
                if (i%10000 == 0)
                {
                    TimeSpan timespan = CalcTimeDiff(StartTime, DateTime.Now);
                    SampleTracker = "Sample: " + i;
                    Debug.WriteLine(SampleTracker + ", " + timespan.TotalMinutes);
                }

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
                if (Polymer[1].Children.Count == 3)
                {
                    double angle2 = Graph.Angle(Polymer[2]);
                    double angle3 = Graph.Angle(Polymer[3]);
                    double angle4 = Graph.Angle(Polymer[4]);
                    bool b234 = (angle2 < angle3 && angle3 < angle4);
                    bool b342 = (angle3 < angle4 && angle4 < angle2);
                    bool b423 = (angle4 < angle2 && angle2 < angle3);
                    if (b234 | b342 | b423)
                    {
                        t1c234++;
                    }
                    else
                    {
                        t1c243++;
                    }
                }
                else if (Polymer[1].Children.Count == 2)
                {
                    if (Polymer[2].Children.Count == 1)
                    {
                        if (Polymer[2].Children.Contains(Polymer[3]))
                        {
                            w4123++;
                        }
                        else
                        {
                            w3124++;
                        }
                    }
                    else if (Polymer[3].Children.Count == 1)
                    {
                        if (Polymer[3].Children.Contains(Polymer[2]))
                        {
                            w4132++;
                        }
                        else
                        {
                            w2134++;
                        }
                    }
                    else
                    {
                        if (Polymer[4].Children.Contains(Polymer[2]))
                        {
                            w3142++;
                        }
                        else
                        {
                            w2143++;
                        }
                    }
                }
                else
                {
                    if (Polymer[2].Children.Count == 2)
                    {
                        double angle1 = Graph.AntiAngle(Polymer[2]);
                        double angle3 = Graph.Angle(Polymer[3]);
                        double angle4 = Graph.Angle(Polymer[4]);
                        bool b134 = (angle1 < angle3 && angle3 < angle4);
                        bool b341 = (angle3 < angle4 && angle4 < angle1);
                        bool b413 = (angle4 < angle1 && angle1 < angle3);
                        if (b134 | b341 | b413)
                        {
                            t2c134++;
                        }
                        else
                        {
                            t2c143++;
                        }
                    }
                    else if (Polymer[3].Children.Count == 2)
                    {
                        double angle1 = Graph.AntiAngle(Polymer[3]);
                        double angle2 = Graph.Angle(Polymer[2]);
                        double angle4 = Graph.Angle(Polymer[4]);
                        bool b124 = (angle1 < angle2 && angle2 < angle4);
                        bool b241 = (angle2 < angle4 && angle4 < angle1);
                        bool b412 = (angle4 < angle1 && angle1 < angle2);
                        if (b124 | b241 | b412)
                        {
                            t3c124++;
                        }
                        else
                        {
                            t3c142++;
                        }
                    }
                    else if (Polymer[4].Children.Count == 2)
                    {
                        double angle1 = Graph.AntiAngle(Polymer[4]);
                        double angle2 = Graph.Angle(Polymer[2]);
                        double angle3 = Graph.Angle(Polymer[3]);
                        bool b123 = (angle1 < angle2 && angle2 < angle3);
                        bool b231 = (angle2 < angle3 && angle3 < angle1);
                        bool b312 = (angle3 < angle1 && angle1 < angle2);
                        if (b123 | b231 | b312)
                        {
                            t4c123++;
                        }
                        else
                        {
                            t4c132++;
                        }
                    }
                    else if (Polymer[1].Children.Contains(Polymer[2]))
                    {
                        if (Polymer[4].Children.Count == 0)
                        {
                            w1234++;
                        }
                        else
                        {
                            w1243++;
                        }
                    }
                    else if (Polymer[1].Children.Contains(Polymer[3]))
                    {
                        if (Polymer[4].Children.Count == 0)
                        {
                            w1324++;
                        }
                        else
                        {
                            w1342++;
                        }
                    }
                    else if (Polymer[1].Children.Contains(Polymer[4]))
                    {
                        if (Polymer[3].Children.Count == 0)
                        {
                            w1423++;
                        }
                        else
                        {
                            w1432++;
                        }
                    }
                }
            }

            // output html...
            divOutput.InnerHtml = "Input Value: " + inputvalue.ToString();
            divOutput.InnerHtml += "<br/>";
            divOutput.InnerHtml += "1234: " + w1234 + "<br> 1243: " + w1243 + "<br> 1324: " + w1324 + "<br> 1342: " + w1342 + "<br> 1423: " + w1423 + "<br> 1432: " + w1432;
            divOutput.InnerHtml += "<br> 2134: " + w2134 + "<br> 2143: " + w2143 + "<br> 3124: " + w3124 + "<br> 3142: " + w3142 + "<br> 4123: " + w4123 + "<br> 4132: " + w4132;
            divOutput.InnerHtml += "<br> 4-123: " + t4c123 + "<br> 4-132: " + t4c132 + "<br> 3-124: " + t3c124 + "<br> 3-142: " + t3c142 + "<br> 2-134: " + t2c134 + "<br> 2-143: " + t2c143 + "<br> 1-234: " + t1c234 + "<br> 1-243: " + t1c243;
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