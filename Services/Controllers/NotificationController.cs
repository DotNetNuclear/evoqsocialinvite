using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Web.Api;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuclear.Modules.InviteRegister.Components;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Social;

namespace DotNetNuclear.Modules.InviteRegister.Services.Controllers
{
    public class NotificationController : DnnApiController
    {
        private IInviteRepository _inviteRepo;
        private int _inviteid = -1;

        public class NotificationDto
        {
            public int NotificationId { get; set; }
        }

        [ValidateAntiForgeryToken]
        [DnnAuthorize]
        [ActionName("friend")]
        [HttpPost]
        public HttpResponseMessage RequestFriend(NotificationDto postData)
        {
            var success = false;
            try
            {
                var notify = NotificationsController.Instance.GetNotification(postData.NotificationId);
                ParseInviteNotificationKey(notify.Context);

                _inviteRepo = new InviteRepository();
                var oInvitation = _inviteRepo.GetInvite(_inviteid);

                if (oInvitation != null)
                {
                    // Add friend of new user to invite user
                    UserInfo inviteUser = UserController.GetUserById(oInvitation.PortalId, oInvitation.InvitedByUserId);
                    if (inviteUser != null)
                    {
                        UserInfo recipientUser = UserController.GetUserById(oInvitation.PortalId, oInvitation.RecipientUserId);
                        FriendsController.Instance.AddFriend(inviteUser, recipientUser);
                    }
                    success = true;
                    NotificationsController.Instance.DeleteNotification(postData.NotificationId);
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }

            return success ? Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" }) : Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "unable to process notification");
        }

        #region Private Members

        private void ParseInviteNotificationKey(string key)
        {
            Int32.TryParse(key, out _inviteid);
        }

        #endregion
    }

}