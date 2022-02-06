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
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Gets values from Form, if available
            NameValueCollection nvc = Request.Form;

            string InputValueString = "";
            int inputvalue = 4;

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

            string ConnectionTracker = "";
            //string[] GrowthAnimation = new string[inputvalue];
            //string[] GrowthAnimationTranslate = new string[inputvalue];
            //string history = @"";
            string process = @"";
            Random rng = new Random();
            Dictionary<int, Node> Polymer = new Dictionary<int, Node>();
            for (int i = 1; i <= inputvalue; i++)
            {
                Polymer.Add(i, new Node(i,1));
            }
//            GrowthAnimation[0] = @"<circle cx='0' cy='0' r='0' stroke='black' stroke-width='1' fill='none'>
//<animate attributeName='r' attributeType='XML' to ='10' fill='freeze' begin='0s' dur='1s' />";
            for (int i = 2; i <= Polymer.Count; i++)
            {
                int attach = rng.Next(1, i);
                Polymer[attach].Children.Add(Polymer[i]);
                Polymer[i].Parent = Polymer[attach];
                double angle = rng.NextDouble() * 2 * Math.PI;
                Polymer[i].Theta = angle;
                Polymer[i].X = Polymer[attach].X + Polymer[attach].Radius * Math.Cos(angle);
                Polymer[i].Y = Polymer[attach].Y + Polymer[attach].Radius * Math.Sin(angle);
//                GrowthAnimation[i - 1] = "<circle cx='" + Polymer[i].X*10 + "' cy='" + Polymer[i].Y*10 + @"' r='0' stroke='black' stroke-width='1' fill='none' style='visibility:hidden;'>
//<set attributeName='visibility' attributeType='CSS' to='visible' begin='" + (i - 1) + "' fill='freeze' /> ";
                         TimeSpan timespan = CalcTimeDiff(StartTime, DateTime.Now);
                ConnectionTracker = "Node: " +i+ ", attach = " + attach + ", angle = " + angle;
                //history += "attach = " + attach + ", angle = " + angle + ".<br>";
                Debug.WriteLine(ConnectionTracker + ", " + timespan.TotalMinutes);
                //System.Diagnostics.Trace.WriteLine(ConnectionTracker + ", " + timespan.TotalMinutes);

                Dictionary<int, Node> SubPolymer = new Dictionary<int, Node>();
                List<Node> BaseBranchMembers = new List<Node>();
                NodeGroup BaseBranch = new NodeGroup(1, Polymer[attach], BaseBranchMembers);
                Dictionary<Node, NodeGroup> BranchLookup = new Dictionary<Node, NodeGroup>();
                for (int j = 1; j < i; j++)
                {
                    SubPolymer.Add(j, Polymer[j]);
                    BaseBranchMembers.Add(Polymer[j]);
                    BranchLookup.Add(Polymer[j], BaseBranch);

                }
                SubPolymer.Add(i, Polymer[i]);
                double radius = Polymer[i].Radius;
                Polymer[i].Radius = 0;
                //try { Graph.DetectCollision(SubPolymer, new List<NodeGroup>() { BaseBranch }, BranchLookup, Polymer[i], radius); }
                //catch (Exception ex)
                //{
                //    process += ex.Message;
                //}
                List<NodeGroup> Branches = new List<NodeGroup>() { BaseBranch };
                double lastRadius = 0;
                double lastX = Polymer[i].X;
                double lastY = Polymer[i].Y;
                while (!Graph.DetectCollision(SubPolymer, Branches, BranchLookup, Polymer[i], radius))
                {
//                    GrowthAnimation[i - 1] += @"
//<animate attributeName='r' attributeType='XML' to='" + Polymer[i].Radius * 10 + "' fill='freeze' begin='" + ((double)(i - 1) + lastRadius) + "s' dur='" + (Polymer[i].Radius - lastRadius) + @"s'/>";
//                    GrowthAnimationTranslate[i-1] += @"
//<animateTransform attributeName='transform' attributeType='XML' type='translate' to='" + (Polymer[i].X- lastX)*10 + "," + (Polymer[i].Y- lastY) *10 + "' fill='freeze' begin='" + ((double)(i-1)+lastRadius) + "s' dur='" + (Polymer[i].Radius - lastRadius) + @"s'>";
//                    lastRadius = Polymer[i].Radius;
//                    lastX = Polymer[i].X;
//                    lastY = Polymer[i].Y;
                    //Debug Output
                }
//                GrowthAnimation[i - 1] += @"
//<animate attributeName='r' attributeType='XML' to='" + Polymer[i].Radius * 10 + "' fill='freeze' begin='" + ((double)(i - 1) + lastRadius) + "s' dur='" + (Polymer[i].Radius - lastRadius) + @"s'/>";
//                GrowthAnimationTranslate[i - 1] += @"
//<animateTransform attributeName='transform' attributeType='XML' type='translate' to='" + (Polymer[i].X - lastX) * 10 + "," + (Polymer[i].Y - lastY) * 10 + "' fill='freeze' begin='" + ((double)(i - 1) + lastRadius) + "s' dur='" + (Polymer[i].Radius - lastRadius) + @"s'>";
                if (i % 1 == 0)
                {
                    double sleft = 0;
                    double sright = 0;
                    double stop = 0;
                    double sbottom = 0;
                    foreach (Node node in SubPolymer.Values)
                    {
                        sleft = Math.Min(sleft, node.X);
                        sright = Math.Max(sright, node.X);
                        stop = Math.Max(stop, node.Y);
                        sbottom = Math.Min(sbottom, node.Y);
                    }
                    process += "<svg height='" + (10 * (stop - sbottom) + 50) + "' width='" + (10 * (sright - sleft) + 50) + "' viewbox='" + (10 * sleft - 25) + " " + (10 * sbottom - 25) + " " + (10 * (sright - sleft) + 50) + " " + (10 * (stop - sbottom) + 50) + "'>";
                    for (int j = 1; j <= SubPolymer.Count; j++)
                    {
                        process += "<circle cx='" + (SubPolymer[j].X * 10) + "' cy='" + (SubPolymer[j].Y * 10) + "' r='" + SubPolymer[j].Radius * 10 + "' stroke='black' stroke-width='1' fill='none'/><text x='" + ((SubPolymer[j].X * 10) - 5) + "' y='" + ((SubPolymer[j].Y * 10) + 5) + "' fill='red'>" + j + "</text> ";
                    }
                    process += "</svg>";
                }
            }


            //string dynamic = @"
            //     <svg height='" + (10 * Polymer.Count+200) + "' width='" + (10 * Polymer.Count+200) + "' viewbox='" + (-5*Polymer.Count-100) + " " + (-5*Polymer.Count-100) + " " + (10 * Polymer.Count+200) + " " + (10 * Polymer.Count+200) + "'>";
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
            string dynamic = @"
                <svg height='" + (10 * (top - bottom) + 50) + "' width='" + (10 * (right - left) + 50) + "' viewbox='" + (10 * left - 25) + " " + (10 * bottom - 25) + " " + (10 * (right - left) + 50) + " " + (10 * (top - bottom) + 50) + "'>";

            for (int i = 1; i <= Polymer.Count; i++)
            {
                dynamic += "<circle cx='" + (Polymer[i].X*10) + "' cy='" + (Polymer[i].Y*10) + "' r='" + Polymer[i].Radius*10 + "' stroke='black' stroke-width='1' fill='none'/>";
                dynamic += "<text x='" + ((Polymer[i].X * 10)-5) + "' y='" + ((Polymer[i].Y * 10)+5) + "' fill='red' font-size='10'>" + i + "</text> ";
                if (Polymer[i].Parent != null)
                {
                    dynamic += "<line x1='" + (10*Polymer[i].Parent.X) + "' y1='" + (10*Polymer[i].Parent.Y) + "' x2='" + (10*Polymer[i].X) + "' y2='" + (10*Polymer[i].Y) + "' stroke='blue' stroke-width = '1'/>";
                }
            } 
            dynamic +="</svg>";

//            string dynamic2 = @"
//                <svg height='" + (10 * (top - bottom) + 50) + "' width='" + (10 * (right - left) + 50) + "' viewbox='" + (10 * left - 25) + " " + (10 * bottom - 25) + " " + (10 * (right - left) + 50) + " " + (10 * (top - bottom) + 50) + "'>";
//            for (int i = 1; i <= Polymer.Count; i++)
//            {
//                dynamic2 += "<g>" + GrowthAnimation[i - 1] + @"
//</circle>
//" + GrowthAnimationTranslate[i-1] + @"
//</g>
//";
//            }
//            dynamic2 += @"
//</svg>";

                // output html...
                divOutput.InnerHtml = "Input Value: " + inputvalue.ToString();
            divOutput.InnerHtml += "<br/>";
            //divOutput.InnerHtml += history;
            //divOutput.InnerHtml += "<br/>";
            divOutput.InnerHtml += dynamic;
            divOutput.InnerHtml += "<br/>";
            divOutput.InnerHtml += process;
            divOutput.InnerHtml += "<br/>";
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