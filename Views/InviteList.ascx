<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InviteList.ascx.cs" Inherits="DotNetNuclear.Modules.InviteRegister.Views.InviteList" %>

<script type="text/javascript">
    $(document).ready(function () {
        var inviteData = <%=InvitationsJSON%>;
        var initeListingPanel = '#<%=pnlInviteListing.ClientID %>';
        var __InviteListing = new dnnuclear.InviteList(inviteData, $(initeListingPanel).get(0));
    });
</script>


<asp:Panel id="pnlInviteListing" class="sentIntitations" runat="server">
    <h3>Sent Invitations</h3>
    <div class="twelve columns alpha omega" data-bind="template: {foreach: invitesModel}" id="inviteList">
        <div class="six columns alpha myConnections" id="searchResult">
            <div class="six columns alpha myConnection" style="background-color: #fff; margin: 10px;" id="profile">
                <!-- If the user has a profile id -->
                <span data-bind="if: RecipientUser &amp;&amp; RecipientUser.IsVisible" class="dataBindSpan">
                    <a data-bind="attr: { href: '/ActivityFeed/tabid/<%=UserProfileTabId%>/userId/'+ RecipientUser.UserId }" title="View profile" class="profileAvatar">
                        <span class="dataBindSpan">
                            <span class="identityImg">
                                <img data-bind="attr: {src: '/profilepic.ashx?userId='+ RecipientUser.UserId +'&h=50&w=50'}" />
                            </span>
                        </span>
                    </a>
                    <a class="profileFriendlyName" data-bind="attr: { href: '/user/'+ RecipientUser.UserId  },text: RecipientUser.DisplayName"></a><br>
                </span>
                <span data-bind="if: !RecipientUser || !RecipientUser.IsVisible" class="dataBindSpan">
                    <a href="#" title="View profile" class="profileAvatar">
                        <span class="dataBindSpan">
                            <span class="identityImg">
                                <img src="<%=MissingUserAvatar%>" />
                            </span>
                        </span>
                    </a>
                </span>

                <span data-bind="text: RecipientEmailAddress" class="mcLocations"></span>

                <div class="clpActions clearAll">
                    <span>Invited: <span data-bind="text: fixJsonDate(CreatedOnDate)" class="dataBindSpan"></span></span>
                    <span data-bind="if: RecipientUser &amp;&amp; RecipientUser.IsVisible">
                        &nbsp;|&nbsp; 
                        <span>Joined: <span data-bind="text: fixJsonDate(RecipientUser.JoinedDate)" class="dataBindSpan"></span></span>
                        &nbsp;|&nbsp; 
                        <span>Relationship: <span data-bind="text: RecipientUser.Status" class="dataBindSpan"></span></span>
                    </span>
                    </div>
            </div>
        </div>             
    </div>
</asp:Panel>
