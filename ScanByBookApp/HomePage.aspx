<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="HomePage.aspx.cs" Inherits="ScanByBookApp.HomePage" %>

<!DOCTYPE html>
<meta name="viewport" content="width=device-width, initial-scale=1">
<link href="Style/style.css" rel="stylesheet" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ScanBook - A Place for all Book Worms</title>
</head>
<script src="Script/GoogleSignIn.js"></script>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script src="Script/Validate.js"></script>
<body>
    <form id="frmHome" runat="server">
        <table width="100%">
            <tr>
                <td colspan="2">
                    <img alt="" class="auto-style1" src="Style/scan.001.png" />

                </td>
                <td colspan="2">
                    <div style="text-align: right; vertical-align: middle">
                        <div id='uName'></div>
                        <label id='loginId' hidden="true" runat="server"></label>
                        <img src="Images/google.png" onclick='login();' runat="server" id="loginText" />
                        <asp:Button runat="server" class="button-secondary" Style="display: none" ID="logoutText" Text="Logout" target='myIFrame' OnClick="logoutText_Click" OnClientClick="myIFrame.location='https://www.google.com/accounts/Logout'; startLogoutPolling();return false;" />
                        <iframe name='myIFrame' id="myIFrame" style='display: none'></iframe>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <legend style="font-family: Verdana, Geneva, Tahoma, sans-serif; font-weight: bold; font-style: normal; font-variant: normal; color: #333333">Search Criteria</legend>
                    <input name="rbSearch" id="rbISBN" type="radio" checked="true" value="ISBN" runat="server" />
                    <label>
                        ISBN
                    </label>
                    <input name="rbSearch" id="rbTitle" type="radio" value="Title" runat="server" />
                    <label>
                        Book Title
                    </label>
                    <input name="rbSearch" id="rbAuthor" type="radio" value="Author" runat="server" />
                    <label>
                        Author
                    </label>
                    <input name="rbSearch" id="rbText" type="radio" value="Search" runat="server" />
                    <label>
                        General Search
                    </label>

                </td>
                <td colspan="2">
                    <asp:ScriptManager ID="smHomePage" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="upnlSearch" runat="server">
                        <ContentTemplate>
                            <legend style="font-family: Verdana, Geneva, Tahoma, sans-serif; font-weight: bold; font-style: normal; font-variant: normal; color: #333333">Keyword Search</legend>
                            <input name="nSearch" type="search" id="txtSearch" runat="server" />
                            <asp:Button runat="server" ID="btnSearch" class="button-secondary" Text="Search" OnClick="btnSearch_Click" OnClientClick="return valLogin();" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:UpdatePanel runat="server" ID="upGrid">
                        <ContentTemplate>
                            <asp:GridView ID="gvBookDetails" CssClass="mGrid" RowStyle-CssClass="td" HeaderStyle-CssClass="th" HeaderStyle-HorizontalAlign="Center"
                                runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="5"
                                OnPageIndexChanging="gvBookDetails_PageIndexChanging" Width="100%" OnRowDataBound="OnRowDataBound" DataKeyNames="ISBNNo">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                        <HeaderTemplate>Add Note</HeaderTemplate>
                                        <ItemTemplate>
                                            <img alt="" style="cursor: pointer" src="Images/Note1.png" />
                                            <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <label>Your Comments:</label></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox TextMode="MultiLine" Width="95%" runat="server" ID="txtNotes"></asp:TextBox></td>
                                                        <td style="vertical-align: middle">
                                                            <asp:Button class="button-secondary" runat="server" ID="btnGridNote" Text="Save Note" OnClick="btnGridNote_Click" CommandArgument='<%# Container.DataItemIndex %>' />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:GridView ID="gvOrders" GridLines="None" RowStyle-CssClass="td" HeaderStyle-CssClass="th" runat="server" AutoGenerateColumns="true" CssClass="ChildGrid" Width="100%" EnableTheming="True">
                                                </asp:GridView>
                                            </asp:Panel>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Book Cover</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Image ID="lblimgBook" runat="server" ImageUrl='<%# Bind("Image") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>ISBN Type</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblISDNType" runat="server" Text='<%# Bind("ISBNType") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>ISBN No</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblISDNNo" runat="server" Text='<%# Bind("ISBNNo") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Title</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:HyperLink Font-Underline="false" ID="lblTitle" runat="server" Text='<%# Bind("title") %>' NavigateUrl='<%# Bind("preview") %>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Authors</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblauthors" runat="server" Text='<%# Bind("authors") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Published Date</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblpublishedDate" runat="server" Text='<%# Bind("publishedDate") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Page Count</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblpageCount" runat="server" Text='<%# Bind("pageCount") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Mark Read</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Button class="button-secondary" ID="btnRead" Text='<%# Bind("UserRead") %>' runat="server" OnClientClick="return valLogin();" OnClick="btnRead_Click" CommandArgument='<%# Container.DataItemIndex %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="th" HorizontalAlign="Center" VerticalAlign="Middle" />
                                <PagerStyle BackColor="#939393" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                                <RowStyle CssClass="td" HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hvfEmail" runat="server" />
    </form>
</body>
</html>
