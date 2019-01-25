<%@ Page Title="MailUp Demo Client" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="MailUpExample._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="bd-pageheader text-center text-sm-left">
        <h1><strong>MailUp Demo Client</strong></h1>
    </div>
    
    <h3><strong>Authentication</strong></h3>
    <div class="row">
        <div class="col-sm-3">
            <div class="panel panel-default auth-panel">
                <div class="panel-heading">Authorization code grant</div>
                <div class="panel-body">
                    <div class="auth-panel-sign">
                        <asp:Button class="btn btn-default" OnClick="LogOn_ServerClick" ID="LogOn" runat="server" Text="Sign in to MailUp"/>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-sm-9">
            <div class="panel panel-default auth-panel">
                <div class="panel-heading">Password grant</div>
                <div class="panel-body">
                    <div class="form-group">
                        <label for="txtUsr">Username:</label>
                        <asp:TextBox type="text" ID="txtUsr" name="txtUsr" Columns="80" class="form-control" placeholder="type your MailUp username" onkeyup="checkSignInButton();" runat="server" />
                    </div>
                    <div class="form-group">
                        <label for="txtPwd">Password:</label>
                        <asp:TextBox type="text" ID="txtPwd" name="txtPwd" Columns="80" class="form-control" placeholder="type your MailUp password" onkeyup="checkSignInButton();" runat="server" />
                    </div>
                    <asp:Button ID="Button1" class="btn btn-success" OnClick="LogOnWithUsernamePassword_ServerClick" Text="Sign in to MailUp with username and password" disabled runat="server"/>
                </div>
            </div>
        </div>
    </div>
    
    <div class="panel panel-default token-panel">
	    <div class="panel-heading">Authorization token grant</div>
	    <div class="panel-body-token">
		    <div class="token-panel-sign">
                <div>
                    <div>
                        <strong><div id="pAuthorizationStatus" class="example-body" runat="server"></div></strong>
                        <div id="pAuthorizationToken" class="example-body" runat="server"></div>
                        <div id="showExpiresInTimer">
                            <span id="expires-label" style="font-weight: bold">&nbsp;</span>
                            <span id="expires-in"></span>
                            <span id="pExpirationTime" runat="server" hidden="true"></span>
                            <br/>
                        </div>
                    </div>
                    <br/>
                    <div>
                        <asp:Button ID="RefreshButton" class="btn btn-success" OnClick="RefreshToken_ServerClick" Text="Refresh token" disabled runat="server"/>
                    </div>
                </div>
		    </div>
	    </div>
    </div>

    <h3><strong>Custom method call</strong></h3>

    <div class = "panel panel-default">
        <div class="panel-body">
            <div class="form-group row">
                <div class="col-xs-2">
                    <label for="lstVerb">Verb</label>
                    <asp:DropDownList ID="lstVerb" class="form-control" runat="server"  AutoPostBack="true"/>
                </div>
                <div class="col-xs-2">
                    <label for="lstContentType">Content-Type</label>
                    <asp:DropDownList ID="lstContentType" class="form-control" runat="server" AutoPostBack="true" Width="100%" />
                </div>
                <div class="col-xs-2">
                    <label for="lstEndpoint">Endpoint</label>
                    <asp:DropDownList ID="lstEndpoint" class="form-control" runat="server"  AutoPostBack="true"/>
                </div>
                <div class="col-xs-6">
                    <label for="txtPath">Path</label>
                    <asp:TextBox ID="txtPath" class="form-control" Columns="80" Value="/Console/Authentication/Info" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <label for="txtBody">Body</label>
                <asp:TextBox ID="txtBody" class="form-control" Textmode="MultiLine" Rows="5" Columns="60" runat="server" />
            </div>
            
            <asp:Button ID="CallMethod" class="btn btn-success" OnClick="CallMethod_ServerClick"  runat="server" Text="Call Method"/>

            <div class="response-panel">
                <div class="well">
                    <div class="form-group response-body">
                        <label>Response</label>
                        <p id="pResultString" runat="server"></p>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <h3><strong>Run example set of calls</strong></h3>
    <div class="panel-group" id="runExample1Block1">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample1">Run example code 1 - Import recipients</a>
                </h4>
            </div>
            <div id="collapseRunExample1" class="panel-collapse collapse">
                <div class="example1">
                     <asp:Button OnClick="RunExample1_ServerClick" onclientclick="showImage('load1', false);" ID="RunExample1" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 1 - Import recipients"/>
                    <img src="Images/loading.gif" id="load1" class="img" alt="" hidden/>
                    <div id="pExampleResultBlock1String" runat="server"></div>
                 </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="panel-group" id="runExample1Block2">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample2">Run example code 2 - Unsubscribe a recipient</a>
                </h4>
            </div>
            <div id="collapseRunExample2" class="panel-collapse collapse">
                <div class="example2">
                    <asp:Button OnClick="RunExample2_ServerClick" onclientclick="showImage('load2', false);" ID="Button2" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 2 - Unsubscripe a recipient"/>
					<img src="Images/loading.gif" id="load2" class="img" alt="" hidden/>
                    <div id="pExampleResultBlock2String" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="panel-group" id="runExample1Block3">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample3">Run example code 3 - Update a recipient</a>
                </h4>
            </div>
            <div id="collapseRunExample3" class="panel-collapse collapse ">
                <div class="example3">
                    <asp:Button OnClick="RunExample3_ServerClick" onclientclick="showImage('load3', false);" ID="Button3" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 3 - Update a recipient"/>
                    <img src="Images/loading.gif" id="load3" class="img" alt="" hidden/>
                    <div id="pExampleResultBlock3String" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="panel-group" id="runExample1Block4">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample4">Run example code 4 - Create a message from template</a>
                </h4>
            </div>
            <div id="collapseRunExample4" class="panel-collapse collapse ">
                <div class="example4">
                    <asp:Button OnClick="RunExample4_ServerClick" onclientclick="showImage('load4', false);" ID="Button4" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 4 - Create a message from template"/>
					<img src="Images/loading.gif" id="load4" class="img" alt="" hidden/>
                    <div id="pExampleResultBlock4String" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="panel-group" id="runExample1Block5">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample5" aria-expanded="true" aria-controls="collapseRunExample5">Run example code 5 - Create a message from scratch</a>
                </h4>
            </div>
            <div id="collapseRunExample5" class="panel-collapse collapse ">
                <div class="example5">
                    <asp:Button OnClick="RunExample5_ServerClick" onclientclick="showImage('load5', false);" ID="Button5" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 5 - Create a message from scratch"/>
					<img src="Images/loading.gif" id="load5" class="img" alt="" hidden/>
                    <div id="pExampleResultBlock5String" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="panel-group" id="runExample1Block6">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample6">Run example code 6 - Tag a message</a>
                </h4>
            </div>
            <div id="collapseRunExample6" class="panel-collapse collapse ">
                <div class="example6">
                    <asp:Button OnClick="RunExample6_ServerClick" onclientclick="showImage('load6', false);" ID="Button6" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 6 - Tag a message"/>
                    <img src="Images/loading.gif" id="load6" class="img" alt="" hidden/>
					<div id="pExampleResultBlock6String" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="panel-group" id="runExample1Block7">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample7">Run example code 7 - Send a message</a>
                </h4>
            </div>
            <div id="collapseRunExample7" class="panel-collapse collapse ">
                <div class="example7">
                    <asp:Button OnClick="RunExample7_ServerClick" onclientclick="showImage('load7', false);" ID="Button7" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 7 - Send a message"/>
					<img src="Images/loading.gif" id="load7" class="img" alt="" hidden/>
                    <div id="pExampleResultBlock7String" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="panel-group" id="runExample1Block8">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#collapseRunExample8">Run example code 8 - Retreive statistics</a>
                </h4>
            </div>
            <div id="collapseRunExample8" class="panel-collapse collapse ">
                <div class="example8">
                    <asp:Button OnClick="RunExample8_ServerClick" onclientclick="showImage('load8', false);" ID="Button8" class="btn btn-success" style="margin-top: 10px; margin-left:15px; margin-bottom:10px;" runat="server" Text="Run example code 8 - Retreive statistics"/>
                    <img src="Images/loading.gif" id="load8" class="img" alt="" hidden/>
                    <div id="pExampleResultBlock8String" runat="server"></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
