<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Order5Data.aspx.cs" Inherits="RandomBranchedPolymer.Order5Data" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" method="post" action="Order5Data.aspx">
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
