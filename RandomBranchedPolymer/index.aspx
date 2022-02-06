<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="index.aspx.cs" Inherits="RandomBranchedPolymer.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" method="post" action="index.aspx">
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
