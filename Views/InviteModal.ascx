<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InviteModal.ascx.cs" Inherits="DotNetNuclear.Modules.InviteRegister.Views.InviteModal" %>

<a id="lnkOpenInvite" class="invite-link dnnSecondaryAction" runat="server"><%= LocalizeString("lnkOpenInvite.Text") %></a>

<script type="text/javascript">
    $(document).ready(function () {
        if (<%= LocalizeString("MoveProfileButton.Text") %>) {
            var inviteLink = $('#<%=lnkOpenInvite.ClientID %>').detach();
            if ($(".profile-social-links:first").length > 0) {
                $('.profile-social-links:first').append(inviteLink);
            } else if ($("div[id$='ViewProfile_buttonPanel']").length > 0) {
                $("div[id$='ViewProfile_buttonPanel']").append($('<li/>')).append(inviteLink);
            }
        }
    });
</script>