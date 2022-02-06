<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CountSnakes.aspx.cs" Inherits="RandomBranchedPolymer.CountSnakes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" method="post" action="CountSnakes.aspx">
    <div>
        <label for="txtInputValue">Enter Number</label>
        <input type="text" id="txtInputValue" runat="server" />
        <input type="submit" />

        <hr />
        <br />

        <div id="divOutput" runat="server"></div>

    </div>
    </form>
</body>
</html>
