<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Invite.ascx.cs" Inherits="DotNetNuclear.Modules.InviteRegister.Views.Invite" %>

<script type="text/javascript">
    var _inviteOptions = {
        ModuleId: <%=base.ModuleContext.ModuleId%>,
        ModuleName: '<%=ModuleName%>',
        FormPanelId: '#<%=inviteModuleWrap.ClientID%>',
        AlertPanelId: '#<%=alertsDiv.ClientID%>',
        LoadingImageId: '#<%=imgContentLoading.ClientID%>',
        DefaultMessage: '<%=DefaultMessage%>',
        MaxEmailInputs: <%=MaxEmailInputs%>,
        MaxDailyInvites: <%=MaxDailyInvites%>,
        DailyInviteCount: <%=DailyInviteCount%>
    };

    $(document).ready(function () {
        var inviteForm = new dnnuclear.InviteForm(_inviteOptions);
    });
</script>

<asp:Panel id="alertsDiv" class="dnnFormMessage" style="display: none" runat="server">
    <div></div>
</asp:Panel>

<asp:Panel ID="inviteModuleWrap" CssClass="inviteModuleWrap" runat="server">
    <img id="imgContentLoading" src="~/images/loading.gif" class="loading-img" alt="Searching..." runat="server" />
    <h1><%= LocalizeString("InviteTitle") %></h1>
    <asp:Panel ID="inviteFormWrapper" data-bind="visible: enableForm" CssClass="inviteFormWrapper ten columns alpha" runat="server">
        <fieldset class="validationGroup">
            <legend style="visibility: hidden"></legend>
            <div id="emailInviteList" data-bind="template: { foreach: emailModel, beforeRemove: removeInvite, afterRender: initValidationRules }">
                <div class="container">
                    <div class="ten columns alpha inviteEmailFields">
                        <h3><label data-bind="attr: { 'for': 'email_' + $index() }">Email address</label></h3>
                        <asp:TextBox runat="server" ID="FirstName" CssClass="emailInput nine columns alpha required" placeholder="Email address" data-bind="attr: {id:'email_' + $index(), name: 'data[emails][' + $index() + ']' }, value: trimmed" />
                        <div class="addEmailContainer" data-bind="if: ($index() == 0)">
                            <a id="addInviteLink" class="addAddressField" href="#" data-bind="click: $parent.addEmail" tabindex="-1">add email address field</a>
                        </div>
                        <div class="addEmailContainer" data-bind="if: $index() > 0">
                            <a href="#" class="removeInput removeAddressField" data-bind="click: $parent.removeEmail" tabindex="-1">remove email address field</a>
                        </div>
                    </div>
                </div>
            </div>
            <div id="pnlUserMessage" class="container" runat="server">
                <div class="nine columns alpha">
                    <h3><label class="visuallyhidden" for="note"><%= LocalizeString("MessageTitle") %></label></h3>
                    <textarea id="note" class="textareaNote nine columns" name="data[note]" placeholder="Add a personal note" data-bind="value: note"></textarea>
                </div>
            </div>
            <div id="submitButtonWrapper">
                <a class="causesValidation dnnPrimaryAction" href="#" data-bind="click: sendInvite">Submit</a>
                <a id="lnkCloseModal" class="dnnSecondaryAction" href="javascript: window.dnnModal.closePopUp(true, '');" runat="server">Close</a>
            </div>
        </fieldset>
    </asp:Panel>
    <div data-bind="visible: !enableForm()" class="dnnFormMessage">
        <%= LocalizeString("CannotEnableForm") %>
    </div>
</asp:Panel>