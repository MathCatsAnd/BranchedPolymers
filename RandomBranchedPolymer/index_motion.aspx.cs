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
    public partial class index_motion : System.Web.UI.Page
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

                if (inputvalue > 200)
                {
                    // can't do anything...
                    divOutput.InnerHtml = "Error: Maximum value allowed is 200 until further safety valves are implemented.";

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
            double NodeTime = 0.4;
            double NodeSize = 10;
            //collect nodes drawn as cirles and animation times
            string[] GrowthAnimation = new string[inputvalue];
            //collect paths (locations) of each node
            string[] GrowthAnimationTranslate = new string[inputvalue];
            //string history = @"";
            //string process = @"";
            Random rng = new Random();
            Dictionary<int, Node> Polymer = new Dictionary<int, Node>();
            for (int i = 1; i <= inputvalue; i++)
            {
                Polymer.Add(i, new Node(i,1));
            }
            //path names: <new node>_<branch base>_<segment>
            GrowthAnimation[0] = @"<circle id='c1' r='0' stroke='black' stroke-width='1' fill='none'>" +
                "<animate id='r1' attributeName='r' attributeType='XML' to ='" + NodeSize + "' fill='freeze' dur='" + NodeTime + "s' />" +
                "<animateMotion id='m1_1_1' dur='" + NodeTime + "' fill='freeze'><mpath xlink:href='#p1_1_1'/></animateMotion>";
            GrowthAnimationTranslate[0] = @"<path d='M0 0 0 0' id='p1_1_1'/>";
            //track number of segments for previous node attachment


            //********************************************************
            //Total segments from previous nodes (segmenttotal), begin next attachment
            //********************************************************
            
            string lastAnimation = "1_1_1";
            for (int i = 2; i <= Polymer.Count; i++)
            {
                //choose an existing node to attach next node
                int attach = rng.Next(1, i);
                //attach new node
                Polymer[attach].Children.Add(Polymer[i]);
                Polymer[i].Parent = Polymer[attach];
                //choose angle of attachment
                double angle = rng.NextDouble() * 2 * Math.PI;
                //set angle
                Polymer[i].Theta = angle;
                //compute and set x,y location
                Polymer[i].X = Polymer[attach].X + Polymer[attach].Radius * Math.Cos(angle);
                Polymer[i].Y = Polymer[attach].Y + Polymer[attach].Radius * Math.Sin(angle);
                //draw the new node as a point in the animation sequence
                GrowthAnimation[i - 1] += "<circle id='c" + i + "' r='0' stroke='black' stroke-width='1' fill='none'>";
                GrowthAnimation[i - 1] += "<animate id='r" + i + "' attributeName='r' attributeType='XML' to='" + NodeSize + "' fill='freeze' begin='r" + (i - 1) + ".end' dur='" + NodeTime + "s'/>";
                //debug
                TimeSpan timespan = CalcTimeDiff(StartTime, DateTime.Now);
                //debug
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
                int growthsegment = 1;
                double lastRadius = 0;
                double antelastRadius = 0;
                double lastX = Polymer[i].X;
                double lastY = Polymer[i].Y;
                //double[] locateX = new double[i];
                //double[] locateY = new double[i];
                //locateX[i - 1] = Polymer[i].X;
                //locateX[i - 1] = Polymer[i].Y;
                int[] openPaths = new int[i];
                double[] relativePathX = new double[i];
                double[] relativePathY = new double[i];
                string[] openAnimates = new string[i];
                for(int k=0; k < i; k++)
                {
                    openAnimates[k] = "";
                }
                string duration = "0.5m";
                while (!Graph.DetectCollision(SubPolymer, Branches, BranchLookup, Polymer[i], radius))
                {
                    //compute duration of of growth that just occurred; increase to 0.5ms if smaller so SVG element view the time as non-null
                    duration = "0.5m";
                    if ((Polymer[i].Radius - lastRadius) * NodeTime > 0.0005)
                    {
                        duration = ((Polymer[i].Radius - lastRadius) * NodeTime).ToString();
                    }
                    for (int j = 0; j < i; j++)
                    {
                        if (!(openAnimates[j] == ""))
                        {
                            GrowthAnimation[j] += duration;
                            GrowthAnimation[j] += openAnimates[j];
                            openAnimates[j] = "";
                        }
                    }
                    //GrowthAnimation[i - 1] += @"
                    //<animate attributeName='r' attributeType='XML' to='" + Polymer[i].Radius * NodeSize + "' fill='freeze' begin='m" + (i-1) + "_" + (i-1) + "_" + segmenttotal + ".end' dur='" + (Polymer[i].Radius - lastRadius)*NodeTime + "s'/>";

                    GrowthAnimation[i-1] += @"
<animateMotion id = 'm" + i + "_" + i + "_" + growthsegment + "' begin='m" + lastAnimation + ".end' dur='" + duration + "s' fill = 'freeze'><mpath xlink: href = '#p" + i + "_" + i + "_" + growthsegment + "'/></animateMotion>";
                    //path of active node
                    GrowthAnimationTranslate[i-1] += @"<path d='M"+ lastX*NodeSize + " " + lastY*NodeSize + " " + Polymer[i].X*NodeSize + " " + Polymer[i].Y*NodeSize + "' id='p" + i + "_" + i + "_" + growthsegment + "'/>";
                    lastAnimation = i + "_" + i + "_" + growthsegment;
                    //paths of branches
                    for (int j = 0; j < i; j++)
                    {
                        if (openPaths[j] == 1)
                        {
                            GrowthAnimationTranslate[j] += @" " + Polymer[j+1].X * NodeSize + " " + Polymer[j+1].Y * NodeSize + "' id='p" + i + "_" + (j+1) + "_" + (growthsegment) + "'/>";
                            openPaths[j] = 0;
                        }
                    }
                    foreach (NodeGroup branch in Branches)
                    {
                        int branchBase = branch.BranchNode.Label;
                        //GrowthAnimationTranslate[branchBase-1] += @"<path d='M" + Polymer[branchBase].X*NodeSize + " " + Polymer[branchBase].Y*NodeSize;
                        //openPaths[branchBase - 1] = 1;
                        foreach (Node member in branch.Members)
                        {
                            GrowthAnimationTranslate[member.Label-1] += @"<path d='M" + member.X * NodeSize + " " + member.Y * NodeSize;
                            openPaths[member.Label - 1] = 1;
                            //GrowthAnimation[member.Label - 1] += @"<animateMotion begin='m" + lastAnimation + ".end' dur='";
                            //openAnimates[member.Label - 1] = "s' fill = 'freeze'><mpath xlink: href = '#p" + i + "_" + branchBase + "_" + (growthsegment + 1) + "'/></animateMotion>";
                            GrowthAnimation[member.Label - 1] += @"<animateMotion begin='m" + lastAnimation + ".end' dur='";
                            openAnimates[member.Label - 1] = "s' fill = 'freeze'><mpath xlink: href = '#p" + i + "_" + member.Label + "_" + (growthsegment + 1) + "'/></animateMotion>";
                        }
                    }
                    growthsegment++;
                    antelastRadius = lastRadius;
                    lastRadius = Polymer[i].Radius;
                    lastX = Polymer[i].X;
                    lastY = Polymer[i].Y;
                    //locateX[i-1] = Polymer[i].X;
                    //locateY[i-1] = Polymer[i].Y;

                }
                //compute duration of of growth that just occurred; increase to 0.5ms if smaller so SVG element view the time as non-null
                duration = "0.5m";
                if ((Polymer[i].Radius - lastRadius) * NodeTime > 0.0005)
                {
                    duration = ((Polymer[i].Radius - lastRadius) * NodeTime).ToString();
                }
                for (int j = 0; j < i; j++)
                {
                    if (!(openAnimates[j] == ""))
                    {
                        GrowthAnimation[j] += duration;
                        GrowthAnimation[j] += openAnimates[j];
                        openAnimates[j] = "";
                    }
                }
                GrowthAnimation[i - 1] += @"
<animateMotion id = 'm" + i + "_" + i + "_" + growthsegment + "' begin='m" + lastAnimation + ".end' dur='" + duration + "s' fill = 'freeze'><mpath xlink: href = '#p" + i + "_" + i + "_" + growthsegment + "'/></animateMotion>";
                //path of active node
                GrowthAnimationTranslate[i - 1] += @"<path d='M" + lastX * NodeSize + " " + lastY * NodeSize + " " + Polymer[i].X * NodeSize + " " + Polymer[i].Y * NodeSize + "' id='p" + i + "_" + i + "_" + growthsegment + "'/>";
                lastAnimation = i + "_" + i + "_" + growthsegment;
                //paths of branches
                for (int j = 0; j <= i - 1; j++)
                {
                    if (openPaths[j] == 1)
                    {
                        GrowthAnimationTranslate[j] += @" " + Polymer[j + 1].X * NodeSize + " " + Polymer[j + 1].Y * NodeSize + "' id='p" + i + "_" + (j + 1) + "_" + (growthsegment) + "'/>";
                        openPaths[j] = 0;
                    }
                }

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
                    //process += "<svg height='" + (10 * (stop - sbottom) + 50) + "' width='" + (10 * (sright - sleft) + 50) + "' viewbox='" + (10 * sleft - 25) + " " + (10 * sbottom - 25) + " " + (10 * (sright - sleft) + 50) + " " + (10 * (stop - sbottom) + 50) + "'>";
                    //for (int j = 1; j <= SubPolymer.Count; j++)
                    //{
                    //    process += "<circle cx='" + (SubPolymer[j].X * 10) + "' cy='" + (SubPolymer[j].Y * 10) + "' r='" + SubPolymer[j].Radius * 10 + "' stroke='black' stroke-width='1' fill='none'/><text x='" + ((SubPolymer[j].X * 10) - 5) + "' y='" + ((SubPolymer[j].Y * 10) + 5) + "' fill='red'>" + j + "</text> ";
                    //}
                    //process += "</svg>";
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
            string growth = dynamic;
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

            //While input is capped at 500, hide step-wise images
            //divOutput.InnerHtml += process;
            divOutput.InnerHtml += "<br/>";
            divOutput.InnerHtml += "Other Static String: " + inputvalue.ToString();
            divOutput.InnerHtml += "<br/>";

            divOutput.InnerHtml += growth;
            foreach (string path in GrowthAnimationTranslate)
            {
                divOutput.InnerHtml += path;
            }
            foreach (string circles in GrowthAnimation)
            {
                divOutput.InnerHtml += circles + "</circle>";
            }
            divOutput.InnerHtml += @"</svg>";


            // End Log File
            //System.Diagnostics.Trace.Flush();
        }

        public static TimeSpan CalcTimeDiff(DateTime StartTime, DateTime EndTime)
        {
            return (EndTime - StartTime);
        }
    }
}