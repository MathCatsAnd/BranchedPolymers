<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="index_motion.aspx.cs" Inherits="RandomBranchedPolymer.index_motion" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" method="post" action="index_motion.aspx">
    <div>
        <label for="txtNodeCount">Enter Number of Nodes</label>
        <input type="text" id="txtNodeCount" runat="server" /><br />

        <label for="txtRadiusRatio">Enter a radial ratio between nodes. Range = [.5,2]</label>
        <input type="text" id="txtRadiusRatio" runat="server" /><br />

        <input type="radio" id="proportionalTrue" name="proportionalAnimation" value="true" runat="server" />
        <label for="proportionalTrue">Animation proportional to node radius</label><br />
        <input type="radio" id="proportionalFalse" name="proportionalAnimation" value="false" runat="server" />
        <label for="proportionalFalse">Animation proportional to node count</label><br />

        <input type="submit" />

        <hr />
        <br />

        <div id="divOutput" runat="server"></div>

    </div>
    </form>
</body>
</html>
