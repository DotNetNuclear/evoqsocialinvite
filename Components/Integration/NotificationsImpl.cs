/*
' Copyright (c) 2013 DotNetNuclear
' http://www.dotnetnuclear.com
' All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuclear.Modules.InviteRegister.Components.Common;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    public class NotificationsImpl : INotifications
    {
 
        #region Install Methods

        /// <summary>
        /// This will create a notification type associated w/ the module and also handle the actions that must be associated with it.
        /// </summary>
        public void AddNotificationTypes()
        {
            var actions = new List<NotificationTypeAction>();
            var deskModuleId = DesktopModuleController.GetDesktopModuleByFriendlyName(Constants.DESKTOPMODULE_FRIENDLYNAME).DesktopModuleID;

            var objNotificationType = new NotificationType
            {
                Name = Constants.NOTIFICATION_INVITATIONUSED,
                Description = "Social Invites: Invitation Used",
                DesktopModuleId = deskModuleId
            };

            if (NotificationsController.Instance.GetNotificationType(objNotificationType.Name) == null)
            {
                var objAction = new NotificationTypeAction
                {
                    NameResourceKey = "RequestFriend",
                    DescriptionResourceKey = "RequestFriend_Desc",
                    APICall = "DesktopModules/SocialInvite/API/Notification/friend",
                    Order = 1
                };
                actions.Add(objAction);

                NotificationsController.Instance.CreateNotificationType(objNotificationType);
                NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId);
            }
        }

        /// <summary>
        /// This will create an invitation used notification.
        /// </summary>
        public void SendInvitationUsedNotification(Entities.Invitation invite)
        {
            var invUsedNType = NotificationsController.Instance.GetNotificationType(Constants.NOTIFICATION_INVITATIONUSED);

            var invitedUser = UserController.GetUserById(invite.PortalId, invite.InvitedByUserId);

            if (invUsedNType != null && invitedUser != null)
            {
                Notification msg = new Notification
                {
                    NotificationTypeID = invUsedNType.NotificationTypeId,
                    To = invitedUser.DisplayName,
                    Subject = "Your invited guest has joined!",
                    Body = String.Format("{0} {1} has joined the community that you invited! You can now friend this user to add them to your inner circle.", invite.RecipientUser.FirstName, invite.RecipientUser.LastName),
                    IncludeDismissAction = true,
                    Context = invite.InviteId.ToString()
                };

                List<UserInfo> sendUsers = new List<UserInfo>();
                sendUsers.Add(invitedUser);

                NotificationsController.Instance.SendNotification(msg, invite.PortalId, null, sendUsers);
            }
        }

        #endregion
    }
}