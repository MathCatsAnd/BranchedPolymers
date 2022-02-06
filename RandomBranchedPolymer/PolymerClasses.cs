using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomBranchedPolymer
{
    public class Node
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
        public double Theta { get; set; }
        public int Label { get; set;  }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }

        public Node(int label, double radius)
        {
            this.Label = label;
            this.Radius = radius;
            this.Parent = null;
            this.Theta = 0;
            this.X = 0;
            this.Y = 0;
            this.Children = new List<Node>();
        }

        public Node(int label, double radius, Node parent)
        {
            this.Label = label;
            this.Radius = radius;
            this.Parent = parent;
            this.Theta = 0;
            this.X = 0;
            this.Y = 0;
            this.Children = new List<Node>();
        }

        public Node(int label, double radius, Node parent, double theta)
        {
            this.Label = label;
            this.Radius = radius;
            this.Parent = parent;
            this.Theta = theta;
            this.X = parent.X + (parent.Radius + radius) * Math.Cos(theta);
            this.Y = parent.Y + (parent.Radius + radius) * Math.Sin(theta);
            this.Children = new List<Node>();
        }

        public int Height()
        {
            if (this.Parent == null)
            {
                return 0;
            }
            else
            {
                return this.Parent.Height() + 1;
            }
        }

    }
    class NodeGroup
    {
        public int Origin { get; set; }
        public Node BranchNode { get; set; }
        public List<Node> Members  { get; set; }
        public NodeGroup(int origin, Node branch, List<Node> members)
        {
            this.Origin = origin;
            this.BranchNode = branch;
            this.Members = members;
        }
    }
    class Graph
    {
        public static Random rng = new Random();
        public static double Angle (Node node)
        {
            double XP = node.Parent.X;
            double YP = node.Parent.Y;
            double XC = node.X;
            double YC = node.Y;

            return Math.Atan2(YC - YP, XC - XP);

            #region old angle
            //if (XP == XC)
            //{
            //    if (YP > YC)
            //    {
            //        return 3 * Math.PI / 2;
            //    }
            //    else
            //    {
            //        return Math.PI / 2;
            //    }
            //}
            //else
            //{
            //    double arctan = Math.Atan2(YC - YP, XC - XP);
            //    if (XC > XP)
            //    {
            //        if (arctan < 0)
            //        {
            //            arctan = arctan + 2 * Math.PI;
            //        }
            //        return arctan;
            //    }
            //    else
            //    {
            //        arctan = arctan + Math.PI;
            //        return arctan;
            //    }
            //}
            #endregion

        }
        public static double AntiAngle(Node node)
        {
            double XP = node.Parent.X;
            double YP = node.Parent.Y;
            double XC = node.X;
            double YC = node.Y;

            return Math.Atan2(YP - YC, XP - XC);
        }

        public static Node Break (Dictionary<int, Node> Polymer, List<NodeGroup> Branches, Dictionary<Node, NodeGroup> BranchLookup, Node New, Node Impact1, Node Impact2)
        {
            if (Impact2 == New)
            {
                Node temp = Impact1;
                Impact1 = Impact2;
                Impact2 = temp;
            }
            else if (Impact1 != New && BranchLookup[Impact1].Origin == 1)
            {
                Node temp = Impact1;
                Impact1 = Impact2;
                Impact2 = temp;
            }
            Node I1 = Impact1;
            Node I2 = Impact2;
            int height1 = Impact1.Height();
            int height2 = Impact2.Height();

            int h1 = height1;
            int h2 = height2;
            while (h1 > h2)
            {
                I1 = I1.Parent;
                h1--;
            }
            while (h2 > h1)
            {
                I2 = I2.Parent;
                h2--;
            }
            while (I1 != I2)
            {
                I1 = I1.Parent;
                I2 = I2.Parent;
                h1--;
                h2--;
            }
            Node Top = I1;
            double growthx = New.X - BranchLookup[Impact2].BranchNode.X;
            double growthy = New.Y - BranchLookup[Impact2].BranchNode.Y;
            double collidex = Impact2.X - Impact1.X;
            double collidey = Impact2.Y - Impact1.Y;
            if (Impact1 != New)
            {
                collidex = BranchLookup[Impact1].BranchNode.X - New.X;
                collidey = BranchLookup[Impact1].BranchNode.Y - New.Y;
            }
            double bisectx = (growthx + collidex) / 2;
            double bisecty = (growthy + collidey) / 2;
            h1 = height1;
            h2 = height2;
            I1 = Impact1;
            I2 = Impact2;
            List<Node> BreakList = new List<Node>();
            List<double> BreakWeight = new List<double>();
            List<int> BreakSide = new List<int>();
            //check impact in case rounding error misses one
            double dotprod = (I2.X - I1.X) * bisectx + (I2.Y - I1.Y) * bisecty;
            if (dotprod < 0)
            {
                BreakList.Add(New);
                BreakWeight.Add(-dotprod);
                BreakSide.Add(0);
            }
            while (h1 > h2)
            {
                dotprod = (I1.X - I1.Parent.X) * bisectx + (I1.Y - I1.Parent.Y) * bisecty;
                if (dotprod < 0)
                {
                    BreakList.Add(I1);
                    BreakWeight.Add(-dotprod);
                    BreakSide.Add(1);
                }
                I1 = I1.Parent;
                h1--;
            }
            while (h2 > h1)
            {
                dotprod = (I2.Parent.X - I2.X) * bisectx + (I2.Parent.Y - I2.Y) * bisecty;
                if (dotprod < 0)
                {
                    BreakList.Add(I2);
                    BreakWeight.Add(-dotprod);
                    BreakSide.Add(2);
                }
                I2 = I2.Parent;
                h2--;
            }
            while (I1 != I2)
            {
                dotprod = (I1.X - I1.Parent.X) * bisectx + (I1.Y - I1.Parent.Y) * bisecty;
                if (dotprod < 0)
                {
                    BreakList.Add(I1);
                    BreakWeight.Add(-dotprod);
                    BreakSide.Add(1);
                }
                I1 = I1.Parent;
                h1--;
                dotprod = (I2.Parent.X - I2.X) * bisectx + (I2.Parent.Y - I2.Y) * bisecty;
                if (dotprod < 0)
                {
                    BreakList.Add(I2);
                    BreakWeight.Add(-dotprod);
                    BreakSide.Add(2);
                }
                I2 = I2.Parent;
                h2--;
            }
            double probability = BreakWeight.Sum();
            double find = probability * rng.NextDouble();
            int i = 0;
            double breaksum = 0;
            if (find == probability)
            {
                i = BreakWeight.Count - 1;
            }
            else
            {
                while (find >= breaksum)
                {
                    breaksum = breaksum + BreakWeight[i];
                    i++;
                }
                i--;
            }
            if (BreakSide[i] == 0)
            {
                return New;
            }
            //Create new branch in case the growing node is part of the impact
            List<Node> NewGroupList = new List<Node>();
            NodeGroup group1 = new NodeGroup(0, Impact2, NewGroupList);            
            Node branch1 = Impact2;
            if (Impact1 != New)
            {
                group1 = BranchLookup[Impact1];
                branch1 = group1.BranchNode;
            }
            NodeGroup group2 = BranchLookup[Impact2];
            Node branch2 = group2.BranchNode;
            //Add new branch to the list if the growing node is part of the impact
            if (Impact1 == New)
            {
                Branches.Add(group1);
            }

            if (BreakSide[i] == 2)
            {
                Node Stitch1 = Impact1;
                Node Stitch2 = Impact2;
                while (Stitch1 != BreakList[i])
                {
                    if (Stitch1 == New)
                    {
                        //BranchLookup[Stitch2].Members.Remove(Stitch2);
                        //BranchLookup[Stitch2] = group1;
                        //group1.Members.Add(Stitch2);
                        Regroup(BranchLookup, BranchLookup[Stitch2], group1, Stitch2);
                        //group1.BranchNode = branch1;
                    }
                    else
                    {
                        //BranchLookup[Stitch2].Members.Remove(Stitch2);
                        //BranchLookup[Stitch2] = BranchLookup[Stitch1];
                        //BranchLookup[Stitch1].Members.Add(Stitch2);
                        Regroup(BranchLookup, BranchLookup[Stitch2], BranchLookup[Stitch1], Stitch2);
                    }
                    Node temp = Stitch2.Parent;
                    Stitch1.Children.Add(Stitch2);
                    Stitch2.Parent.Children.Remove(Stitch2);
                    Stitch2.Parent = Stitch1;
                    Stitch1 = Stitch2;
                    Stitch2 = temp;
                }
            }
            else
            { 
                Node Stitch1 = Impact1;
                Node Stitch2 = Impact2;
                while (Stitch2 != BreakList[i])
                {
                    if (Stitch1 == New)
                    {
                        group2.BranchNode = branch1;
                    }
                    else if (Stitch1 == branch2)
                    {
                        //passgrow = true;
                        group1.BranchNode = branch2;
                        //BranchLookup[branch2].Members.Remove(branch2);
                        //group1.Members.Add(branch2);
                        //BranchLookup[branch2] = group1;
                        Regroup(BranchLookup, BranchLookup[branch2], group1, branch2);
                    }
                    else
                    {
                        //BranchLookup[Stitch1].Members.Remove(Stitch1);
                        //BranchLookup[Stitch1] = BranchLookup[Stitch2];
                        //BranchLookup[Stitch2].Members.Add(Stitch1);
                        Regroup(BranchLookup, BranchLookup[Stitch1], BranchLookup[Stitch2], Stitch1);
                    }
                    Node temp = Stitch1.Parent;
                    Stitch2.Children.Add(Stitch1);
                    Stitch1.Parent.Children.Remove(Stitch1);
                    Stitch1.Parent = Stitch2;
                    Stitch2 = Stitch1;
                    Stitch1 = temp;
                }
            }
                return BreakList[i];
        }

        //public static Node Break(Node GrowingNode, NodeGroup ImpactGroup1, NodeGroup ImpactGroup2, Node Group1ImpactNode, Node Group2ImpactNode)
        //{
        //    int group1height = Group1ImpactNode.Height()-GrowingNode.Height();
        //    int group2height = Group2ImpactNode.Height()-GrowingNode.Height();
        //    double angle1 = (ImpactGroup1.BranchNode.Theta + ImpactGroup2.BranchNode.Theta) / 2;
        //    double angle2 = angle1 + Math.PI;
        //    if (angle2 >= 2 * Math.PI)
        //    {
        //        angle2 = angle2 - 2 * Math.PI;
        //    }
        //    double maxangle = Math.Max(angle1, angle2);
        //    double minangle = Math.Min(angle1, angle2);
        //    //set bool parameters to check angles up the chain.  growside and collisionside are set to desired values for a break
        //    bool check1 = false;
        //    bool check2 = false;
        //    if (ImpactGroup1.BranchNode.Theta < maxangle && ImpactGroup1.BranchNode.Theta > minangle)
        //    {
        //        check2 = true;
        //    }
        //    else
        //    {
        //        check1 = true;
        //    }
        //    List<Node> breakpoints = new List<Node>();
        //    Node group1node = Group1ImpactNode;
        //    Node group2node = Group2ImpactNode;

        //    while (group1height > group2height)
        //    {
        //        bool check = group1node.Theta < maxangle && group1node.Theta > minangle;
        //        if (check == check1)
        //        {
        //            breakpoints.Add(group1node);
        //        }
        //        group1node = group1node.Parent;
        //        group1height--;
        //    }
        //    while (group2height > group1height)
        //    {
        //        bool check = group2node.Theta < maxangle && group2node.Theta > minangle;
        //        if (check == check2)
        //        {
        //            breakpoints.Add(group2node);
        //        }
        //        group2node = group2node.Parent;
        //        group2height--;
        //    }
        //    while (group2node != GrowingNode)
        //    {
                
        //        bool check = group1node.Theta < maxangle && group1node.Theta > minangle;
        //        if (check == check1)
        //        {
        //            breakpoints.Add(group1node);
        //        }
        //        group1node = group1node.Parent;
        //        group1height--;
        //        check = group2node.Theta < maxangle && group2node.Theta > minangle;
        //        if (check == check2)
        //        {
        //            breakpoints.Add(group2node);
        //        }
        //        group2node = group2node.Parent;
        //        group2height--;
        //    }
        //    int choose = Graph.rng.Next(0, breakpoints.Count);

        //    return breakpoints[choose];
        //}
        /// <summary>
        /// Returns a breakpoint when the growing node hits a node from the stable group
        /// </summary>
        /// <param name="GrowingNode"></param>
        /// <param name="CollisionNode"></param>
        /// <returns></returns>
        //public static Node Break(Node GrowingNode, Node CollisionNode)
        //{
        //    int growingheight = GrowingNode.Height();
        //    int collisionheight = CollisionNode.Height();
        //    int gh = growingheight;
        //    int ch = collisionheight;
        //    Node GTop = GrowingNode;
        //    Node CTop = CollisionNode;
        //    while (gh > ch)
        //    {
        //        GTop = GTop.Parent;
        //        gh--;
        //    }
        //    while (ch > gh)
        //    {
        //        CTop = CTop.Parent;
        //        ch--;
        //    }
        //    while (CTop != GTop)
        //    {
        //        CTop = CTop.Parent;
        //        GTop = GTop.Parent;
        //        gh--;
        //        ch--;
        //    }

        //    double impactangle = 0;
        //    if (GrowingNode.X == CollisionNode.X && GrowingNode.Y < CollisionNode.Y)
        //    {
        //        impactangle = Math.PI / 2;
        //    }
        //    else if (GrowingNode.X == CollisionNode.X && CollisionNode.Y < GrowingNode.Y)
        //    {
        //        impactangle = 3 * Math.PI / 2;
        //    }
        //    else if (GrowingNode.X < CollisionNode.X)
        //    {
        //        impactangle = Math.Atan2(CollisionNode.Y - GrowingNode.Y, CollisionNode.X - GrowingNode.X);
        //        if (impactangle < 0)
        //        {
        //            impactangle = impactangle + Math.PI;
        //        }
        //    }
        //    else if (CollisionNode.X < GrowingNode.X)
        //    {
        //        impactangle = Math.PI + Math.Atan2(CollisionNode.Y - GrowingNode.Y, CollisionNode.X - GrowingNode.X);
        //    }

        //    double growthangle;
        //    if (CTop == GrowingNode)
        //    {
        //        Node temp = CollisionNode;
        //        while (temp.Parent != GrowingNode)
        //        {
        //            temp = temp.Parent;
        //        }
        //        growthangle = temp.Theta;
        //    }
        //    else
        //    {
        //        growthangle = GrowingNode.Theta + Math.PI;
        //    }
        //    if (growthangle >= 2 * Math.PI)
        //    {
        //        growthangle = growthangle - 2 * Math.PI;
        //    }
        //    double angle1 = (growthangle + impactangle) / 2;
        //    double angle2 = angle1 + Math.PI;
        //    if (angle2 >= 2 * Math.PI)
        //    {
        //        angle2 = angle2 - 2 * Math.PI;
        //    }
        //    double maxangle = Math.Max(angle1, angle2);
        //    double minangle = Math.Min(angle1, angle2);
        //    //set bool parameters to check angles up the chain.  growside and collisionside are set to desired values for a break
        //    bool growcheck = false;
        //    bool collisioncheck = false;
        //    if (growthangle < maxangle && growthangle > minangle)
        //    {
        //        collisioncheck = true;
        //    }
        //    else
        //    {
        //        growcheck = true;
        //    }

        //    List<Node> breakpoints = new List<Node>();
        //    Node growingsidenode = GrowingNode;
        //    Node collisionsidenode = CollisionNode;
        //    while (growingheight > collisionheight)
        //    {
        //        bool check = growingsidenode.Theta < maxangle && growingsidenode.Theta > minangle;
        //        if (check == growcheck)
        //        {
        //            breakpoints.Add(growingsidenode);
        //        }
        //        growingsidenode = growingsidenode.Parent;
        //        growingheight--;
        //    }
        //    while (collisionheight > growingheight)
        //    {
        //        bool check = collisionsidenode.Theta < maxangle && collisionsidenode.Theta > minangle;
        //        if (check == collisioncheck)
        //        {
        //            breakpoints.Add(collisionsidenode);
        //        }
        //        collisionsidenode = collisionsidenode.Parent;
        //        collisionheight--;
        //    }
        //    while (collisionsidenode != growingsidenode)
        //    {
        //        bool check = growingsidenode.Theta < maxangle && growingsidenode.Theta > minangle;
        //        if (check == growcheck)
        //        {
        //            breakpoints.Add(growingsidenode);
        //        }
        //        growingsidenode = growingsidenode.Parent;
        //        growingheight--;
        //        check = collisionsidenode.Theta < maxangle && collisionsidenode.Theta > minangle;
        //        if (check == collisioncheck)
        //        {
        //            breakpoints.Add(collisionsidenode);
        //        }
        //        collisionsidenode = collisionsidenode.Parent;
        //        collisionheight--;
        //    }
        //    int choose = Graph.rng.Next(0, breakpoints.Count);
        //    return breakpoints[choose];
        //}
        //public static void Regraph(Dictionary<Node, NodeGroup> BranchLookup, NodeGroup Donor, NodeGroup Receiver, Node TreeNode, Node OrnamentNode, Node BreakNode)
        //{
        //    Node NewTree = TreeNode;
        //    Node NewOrnament = OrnamentNode;
        //    while (NewOrnament != BreakNode.Parent)
        //    {
        //        Node Storage = NewOrnament.Parent;
        //        NewTree.Children.Add(NewOrnament);
        //        NewOrnament.Parent = NewTree;
        //        NewOrnament.Theta = Angle(NewOrnament);
        //        Storage.Children.Remove(NewOrnament);
        //        Regroup(BranchLookup, Donor, Receiver, NewOrnament);
        //        NewTree = NewOrnament;
        //        NewOrnament = Storage;
        //    }
        //    return;
        //}
        public static void Regroup(Dictionary<Node, NodeGroup> BranchLookup, NodeGroup Donor, NodeGroup Receiver, Node MovedNode)
        {
            Donor.Members.Remove(MovedNode);
            Receiver.Members.Add(MovedNode);
            BranchLookup[MovedNode] = Receiver;
            foreach (Node successors in MovedNode.Children)
            {
                Regroup(BranchLookup, Donor, Receiver, successors);
            }
            return;
        }
        public static void Translate(List<NodeGroup> Branches, Dictionary<NodeGroup, double> translationx, Dictionary<NodeGroup, double> translationy)
        {
            foreach (NodeGroup nodegroup in Branches)
            {
                foreach (Node node in nodegroup.Members)
                {
                    node.X = node.X + translationx[nodegroup];
                    node.Y = node.Y + translationy[nodegroup];
                }
            }
            return;
        }
        public static void Translate(List<NodeGroup> Branches, Dictionary<NodeGroup, double> translationx, Dictionary<NodeGroup, double> translationy, double proportion)
        {
            foreach (NodeGroup nodegroup in Branches)
            {
                foreach (Node node in nodegroup.Members)
                {
                    node.X = node.X + proportion * translationx[nodegroup];
                    node.Y = node.Y + proportion * translationy[nodegroup];
                }
            }
            return;
        }
        public static bool DetectCollision(Dictionary<int,Node> Polymer, List<NodeGroup> Branches, Dictionary<Node,NodeGroup> BranchLookup, Node New, double radius)
        {
            //int attach = Graph.rng.Next(0, Polymer.Count);
            //Node attachingpoint = Polymer.Values[attach];
            Dictionary<NodeGroup, double> translation = new Dictionary<NodeGroup, double>();
            Dictionary<NodeGroup, double> translationx = new Dictionary<NodeGroup, double>();
            Dictionary<NodeGroup, double> translationy = new Dictionary<NodeGroup, double>();
            List<Node[]> WatchList = new List<Node[]>();
            //calculate movement of each node group
            //double additivedx = (radius - New.Radius) * Math.Cos(New.Theta);
            //double additivedy = (radius - New.Radius) * Math.Sin(New.Theta);
            Node Branch1 = BranchLookup[Polymer[1]].BranchNode;
            double additivedx = (New.X - Branch1.X) * ((Branch1.Radius + radius) / (Branch1.Radius + New.Radius) - 1);
            double additivedy = (New.Y - Branch1.Y) * ((Branch1.Radius + radius) / (Branch1.Radius + New.Radius) - 1);
            foreach (NodeGroup nodegroup in Branches)
            {
                if (nodegroup.Origin == 1)
                {
                    translation.Add(nodegroup, 0);
                    translationx.Add(nodegroup, 0);
                    translationy.Add(nodegroup, 0);
                    //check if growing node intersects
                    double newlocationx = New.X + additivedx;
                    double newlocationy = New.Y + additivedy;
                    foreach (Node node in nodegroup.Members)
                    {
                        double dxdx = (newlocationx - node.X) * (newlocationx - node.X);
                        double dydy = (newlocationy - node.Y) * (newlocationy - node.Y);
                        double distance = Math.Sqrt(dxdx + dydy);
                        if (distance < node.Radius + radius && node != nodegroup.BranchNode)
                        {
                            WatchList.Add(new Node[] { New, node });
                        }
                    }
                }
                else
                {
                    //double dx = additivedx + (radius - New.Radius) * Math.Cos(nodegroup.BranchNode.Theta);
                    //double dy = additivedy + (radius - New.Radius) * Math.Sin(nodegroup.BranchNode.Theta);
                    //double hypotenuse = Math.Sqrt(Math.Pow((nodegroup.BranchNode.X - New.X),2)+ Math.Pow((nodegroup.BranchNode.Y - New.Y),2));
                    //double dx = additivedx + (radius - New.Radius) * (nodegroup.BranchNode.X - New.X) / hypotenuse;
                    //double dy = additivedy + (radius - New.Radius) * (nodegroup.BranchNode.Y - New.Y) / hypotenuse;
                    //double dxdx = Math.Pow(dx, 2);
                    //double dydy = Math.Pow(dy, 2);
                    double branchadditivedx = (New.X - nodegroup.BranchNode.X) * ((nodegroup.BranchNode.Radius + radius) / (nodegroup.BranchNode.Radius + New.Radius) - 1);
                    double branchadditivedy = (New.Y - nodegroup.BranchNode.Y) * ((nodegroup.BranchNode.Radius + radius) / (nodegroup.BranchNode.Radius + New.Radius) - 1);
                    double dx = additivedx - branchadditivedx;
                    double dy = additivedy - branchadditivedy;
                    translation.Add(nodegroup, Math.Sqrt(dx*dx + dy*dy));
                    translationx.Add(nodegroup, dx);
                    translationy.Add(nodegroup, dy);
                    //check if growing node intersects
                    //double targetx = New.X + (radius - New.Radius) * Math.Cos(nodegroup.BranchNode.Theta + Math.PI);
                    //double targety = New.Y + (radius - New.Radius) * Math.Sin(nodegroup.BranchNode.Theta + Math.PI);
                    //double targetx = New.X - (radius - New.Radius) * (nodegroup.BranchNode.X - New.X) / hypotenuse;
                    //double targety = New.Y - (radius - New.Radius) * (nodegroup.BranchNode.Y - New.Y) / hypotenuse;
                    double targetx = New.X + branchadditivedx;
                    double targety = New.Y + branchadditivedy;

                    foreach (Node node in nodegroup.Members)
                    {
                        double dxdx = (targetx - node.X) * (targetx - node.X);
                        double dydy = (targety - node.Y) * (targety - node.Y);
                        double distance = Math.Sqrt(dxdx + dydy);
                        if (distance < node.Radius + radius && node != nodegroup.BranchNode)
                        {
                            WatchList.Add(new Node[] { New, node });
                        }
                    }
                }
            }
            //check for branches hitting each other
            for (int i = 0; i < Branches.Count; i++)
            {
                for (int j = i + 1; j < Branches.Count; j++)
                {
                    foreach (Node nodei in Branches[i].Members)
                    {
                        foreach (Node nodej in Branches[j].Members)
                        {
                            double distance = Math.Sqrt(Math.Pow((nodei.X - nodej.X), 2) + Math.Pow((nodei.Y - nodej.Y), 2));
                            if (distance < nodei.Radius + translation[Branches[i]] + nodej.Radius + translation[Branches[j]])
                            {
                                WatchList.Add(new Node[] { nodei, nodej });
                            }
                        }
                    }
                }
            }
            if (WatchList.Count == 0)
            {
                New.Radius = radius;
                New.X = New.X + additivedx;
                New.Y = New.Y + additivedy;
                Translate(Branches, translationx, translationy);
                return true;
            }
            else
            {
                //double r = 0;
                double sfirst = 2;
                int indexfirst = -1;
                for (int i = 0; i < WatchList.Count; i++)
                {
                    double dx0;
                    double dy0;
                    double dx1;
                    double dy1;
                    if (WatchList[i][0] == New)
                    {
                        dx0 = additivedx;
                        dy0 = additivedy;
                        //r = radius - New.Radius;
                    }
                    else
                    {
                        dx0 = translationx[BranchLookup[WatchList[i][0]]];
                        dy0 = translationy[BranchLookup[WatchList[i][0]]];
                        //r = 0;
                    }
                    dx1 = translationx[BranchLookup[WatchList[i][1]]];
                    dy1 = translationy[BranchLookup[WatchList[i][1]]];
                    double x0 = WatchList[i][0].X;
                    double y0 = WatchList[i][0].Y;
                    double r0 = WatchList[i][0].Radius;
                    //dr only used for Impact1 = New
                    double dr = radius - r0;
                    double x1 = WatchList[i][1].X;
                    double y1 = WatchList[i][1].Y;
                    double r1 = WatchList[i][1].Radius;
                    //double s = 0;
                    //double s1 = 0;
                    //double s2 = 0;
                    //calculate precise impact time if growing node is involved
                    if (WatchList[i][0] == New)
                    {
                        double s = .5;
                        bool proceed = true;
                        for (int k = 2; k <= 50 && proceed == true; k++)
                        {
                            double x0s = x0 + s * dx0;
                            double y0s = y0 + s * dy0;
                            double x1s = x1 + s * dx1;
                            double y1s = y1 + s * dy1;
                            double distance = Math.Sqrt((x0s - x1s) * (x0s - x1s) + (y0s - y1s) * (y0s - y1s));
                            if (distance == (r0 + s * dr + r1))
                            {
                                proceed = false;
                            }
                            else if (distance < (r0 + s * dr + r1))
                            {
                                s = s - Math.Pow(2, -k);
                            }
                            else
                            {
                                s = s + Math.Pow(2, -k);
                            }
                        }
                        if (s > 0 && s < sfirst)
                        {
                            sfirst = s;
                            indexfirst = i;
                        }
                    }
                    //calculate preciseimpact time if only branches collide
                    else
                    {
                        //solve the quadratic equation 0 = ax ^ 2 + bx + c
                        double a = (Math.Pow((dx0 - dx1), 2) + Math.Pow((dy0 - dy1), 2));
                        double b = 2 * (dx0 - dx1) * (x0 - x1) + 2 * (dy0 - dy1) * (y0 - y1);
                        double c = (Math.Pow((x0 - x1), 2) + Math.Pow((y0 - y1), 2)) - Math.Pow(r0 + r1, 2);
                        double descriminant = Math.Pow(b, 2) - 4 * a * c;
                        if (descriminant > 0)
                        {
                            double s1 = (-b + Math.Sqrt(descriminant)) / (2 * a);
                            double s2 = (-b - Math.Sqrt(descriminant)) / (2 * a);
                            if (Math.Min(s1, s2) < 1 && Math.Max(s1, s2) > 0)
                            {
                                if (Math.Min(s1, s2) > 0)
                                {
                                    double s = Math.Min(s1, s2);
                                    if (s < sfirst)
                                    {
                                        sfirst = s;
                                        indexfirst = i;
                                    }
                                }
                                //TO DO: Correct for re-detecting collsion of branches.  1/2^45 approximation
                                // && Math.Max(s1, s2)>Math.Pow(2,-50)
                                else if (Math.Max(s1, s2) < 1)
                                {
                                    Node B1 = BranchLookup[WatchList[i][0]].BranchNode;
                                    Node B2 = BranchLookup[WatchList[i][1]].BranchNode;
                                    Node I1 = WatchList[i][0];
                                    Node I2 = WatchList[i][1];
                                    double NewtoB1X = B1.X - New.X;
                                    double NewtoB1Y = B1.Y - New.Y;
                                    double B2toNewX = New.X - B2.X;
                                    double B2toNewY = New.Y - B2.Y;
                                    double averageX = (NewtoB1X + B2toNewX) / 2;
                                    double averageY = (NewtoB1Y + B2toNewY) / 2;
                                    double I1toI2X = I2.X - I1.X;
                                    double I1toI2Y = I2.Y - I1.Y;
                                    double dotprod = (I1toI2X * averageX + I1toI2Y * averageY);
                                    if (dotprod > 0)
                                    {
                                        double s = Math.Max(s1, s2);
                                        if (s < sfirst)
                                        {
                                            sfirst = s;
                                            indexfirst = i;
                                        }
                                    }
                                    else
                                    {
                                        //false collision detection
                                    }
                                }
                                else // when the times straddle the interval of interest
                                {
                                    //s = 2;
                                }
                            }
                            else //when both times are before the start or both times are after the start
                            {
                                //s = 2;
                            }
                        }
                        else //when descriminant is not positive
                        {
                            //s = 2;
                        }
                    }
                }
                if (sfirst > 1)
                {
                    New.Radius = radius;
                    New.X = New.X + additivedx;
                    New.Y = New.Y + additivedy;
                    Translate(Branches, translationx, translationy);
                    return true;
                }
                else
                {
                    New.Radius = New.Radius + sfirst * (radius - New.Radius);
                    New.X = New.X + sfirst * additivedx;
                    New.Y = New.Y + sfirst * additivedy;
                    Translate(Branches, translationx, translationy, sfirst);
                    Break(Polymer, Branches, BranchLookup, New, WatchList[indexfirst][0], WatchList[indexfirst][1]);
                    //DetectCollision(Polymer, Branches, BranchLookup, New, radius);
                    if (New.Radius >= radius)
                    {
                        return true;
                    }
                    else
                    {
                        //throw new Exception("Node " + Polymer.Count + " not full size.");
                        return false;
                    }
                }
            }
        }
    }
}
