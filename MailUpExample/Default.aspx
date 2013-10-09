<%@ Page Title="Homepage" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="MailUpExample._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        MailUp Demo Client
    </h2>

    <p>
        <asp:Button OnClick="LogOn_ServerClick" ID="LogOn" runat="server" Text="Sign in to MailUp"/>
    </p>
    
    <p>
    Username: <asp:TextBox ID="txtUsr" Columns="80" Value="type your MailUp username" runat="server" /><br />
    Password: <asp:TextBox ID="txtPwd" Columns="80" Value="type your MailUp password" runat="server" /><br />
        <asp:Button OnClick="LogOnWithUsernamePassword_ServerClick" ID="Button1" runat="server" Text="Sign in to MailUp with username and password."/>
    </p>

    <p id="pAuthorization" runat="server"></p><br /><br />

    <p><b>Custom method call</b></p>
    <table>
    <thead>
    <td>Verb</td>
    <td>Content-Type</td>
    <td>Endpoint</td>
    <td>Path</td>
    </thead>
    <tr>
    <td><asp:DropDownList ID="lstVerb" runat="server"  AutoPostBack="true"/></td>
    <td style="width:100px;"><asp:DropDownList ID="lstContentType" runat="server" AutoPostBack="true" Width="100%" /></td>
    <td><asp:DropDownList ID="lstEndpoint" runat="server"  AutoPostBack="true"/></td>
    <td><asp:TextBox ID="txtPath" Columns="80" Value="/Console/Authentication/Info" runat="server" /></td>
    </tr>
    </table>

    <p>Body</p><p><asp:TextBox ID="txtBody" Textmode="MultiLine" Rows="5" Columns="60" runat="server" /></p>
    <p>
        <asp:Button OnClick="CallMethod_ServerClick" ID="CallMethod" runat="server" Text="Call Method"/>
    </p>

    <p id="pResultString" runat="server"></p><br /><br />

    <p><b>Run example set of calls</b></p>
    
    <p id="pExampleResultString" runat="server"></p>
    <p>
        <asp:Button OnClick="RunExample1_ServerClick" ID="RunExample1" runat="server" Text="Run example code 1 - Import recipients"/>
    </p>
    <p>
        <asp:Button OnClick="RunExample2_ServerClick" ID="RunExample2" runat="server" Text="Run example code 2 - Unsubscripe a recipient"/>
    </p>
    <p>
        <asp:Button OnClick="RunExample3_ServerClick" ID="RunExample3" runat="server" Text="Run example code 3 - Update a recipient"/>
    </p>
    <p>
        <asp:Button OnClick="RunExample4_ServerClick" ID="RunExample4" runat="server" Text="Run example code 4 - Create a message from template"/>
    </p>
    <p>
        <asp:Button OnClick="RunExample5_ServerClick" ID="RunExample5" runat="server" Text="Run example code 5 - Create a message from scratch"/>
    </p>
    <p>
        <asp:Button OnClick="RunExample6_ServerClick" ID="RunExample6" runat="server" Text="Run example code 6 - Tag a message"/>
    </p>
    <p>
        <asp:Button OnClick="RunExample7_ServerClick" ID="RunExample7" runat="server" Text="Run example code 7 - Send a message"/>
    </p>
    <p>
        <asp:Button OnClick="RunExample8_ServerClick" ID="RunExample8" runat="server" Text="Run example code 8 - Retreive statistics"/>
    </p>

</asp:Content>
