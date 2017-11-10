<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DotNetNuclear.Modules.InviteRegister.Views.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagName="texteditor" TagPrefix="dnn" Src="~/controls/texteditor.ascx" %>

<div class="invite-settings">
    <h2 id="dnnInvite-BasicSettings" class="dnnFormSectionHead"><a href="javascript:void(0);" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
    <fieldset><legend></legend>
        <div class="dnnFormItem">
            <dnn:Label ID="lblModuleView" runat="server" ResourceKey="lblModuleView"/> 
            <asp:DropDownList ID="ddlControlToLoad" runat="server" DataValueField="name" DataTextField="name" AutoPostBack="true" OnSelectedIndexChanged="ddlControlToLoad_SelectedIndexChanged"></asp:DropDownList>
        </div>

        <asp:Panel ID="pnlBasicEmailSettings" runat="server">
        <div class="dnnFormItem">
            <dnn:Label ID="lblMaxEmailInvitesPerSubmit" runat="server"/> 
            <asp:TextBox ID="txtMaxEmailInvitesPerSubmit" runat="server" Width="80" MaxLength="8" Columns="4"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblMaxEmailInvitesPerDay" runat="server"/> 
            <asp:TextBox ID="txtMaxEmailInvitesPerDay" runat="server" Width="80" MaxLength="8" Columns="4"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowMessage" runat="server" ResourceKey="lblShowMessage"/> 
            <asp:Checkbox ID="chkShowMessage" runat="server"></asp:Checkbox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDefaultMessage" runat="server" ResourceKey="lblDefaultMessage"/> 
            <asp:TextBox ID="txtDefaultMessage" runat="server" MaxLength="200" TextMode="MultiLine" Columns="20" Rows="3"></asp:TextBox>
        </div>
        </asp:Panel>
    </fieldset>

    <asp:Panel ID="pnlEmailTemplate" runat="server">
    <h2 id="dnnInvite-EmailTemplate" class="dnnFormSectionHead"><a href="javascript:void(0);" class="dnnSectionExpanded"><%=LocalizeString("EmailTemplate")%></a></h2>
    <fieldset><legend></legend>
        <dnn:TextEditor ID="txtEmailTemplate" runat="server"></dnn:TextEditor>
        <span><%=LocalizeString("EmailTemplate.Help") %></span>
    </fieldset>
    </asp:Panel>
</div>