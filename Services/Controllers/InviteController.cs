using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using DotNetNuclear.Modules.InviteRegister.Components;
using DotNetNuclear.Modules.InviteRegister.Components.Common;
using DotNetNuclear.Modules.InviteRegister.Components.Entities;
using DotNetNuclear.Modules.InviteRegister.Components.Integration;

namespace DotNetNuclear.Modules.InviteRegister.Services.Controllers
{
    [SupportedModules(Constants.DESKTOPMODULE_NAME)]
    public class InviteController : DnnApiController
    {
        ISettingsRepository _settings;
        IInviteRepository _inviteRepo;

        /// <summary>
        /// Anonymous REST API that emails invitations to a list of recipients
        /// </summary>
        /// <param name="emailList">email address list</param>
        /// <returns></returns>
        [HttpPost]  //[baseURL]/invite/send
        [ValidateAntiForgeryToken]
        [ActionName("send")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public HttpResponseMessage SendInvitations(EmailList emailList)
        {
            Dictionary<string, string> EmailTokens = new Dictionary<string, string>();
            int inviteCount = 0;
            SendInvitationResponse resp = new SendInvitationResponse();
            if (ActiveModule != null && UserInfo != null)
            {
                _settings = new SettingsRepository(ActiveModule.ModuleID);
                _inviteRepo = new InviteRepository();

                int dailycount = _inviteRepo.GetUserInvites(UserInfo.UserID, DateTime.Today).Count();
                int dailymax = _settings.MaxEmailInvitesPerDay;
                if (dailymax < 1) { dailymax = 999999; }

                PortalController pCtlr = new PortalController();
                PortalInfo pSettings = pCtlr.GetPortal(ActiveModule.PortalID);

                string appPath = HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
                if (emailList != null && emailList.tolist.Length > 0)
                {
                    foreach (string inviteEmail in emailList.tolist)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(inviteEmail))
                            {
                                string fmEmail = inviteEmail.Trim().ToLower();

                                // Check if email already exists
                                var dupInv = _inviteRepo.CheckDuplicateInvite(UserInfo.UserID, fmEmail);
                                if (dupInv != null)
                                {
                                    resp.Warnings++;
                                    resp.Messages.Add(String.Format("Email '{0}' has already been invited.", dupInv.RecipientEmailAddress));
                                }
                                else
                                {
                                    if ((dailycount + inviteCount) < dailymax)
                                    {
                                        string regCode = System.Guid.NewGuid().ToString();
                                        string registerLink;
                                        if (pSettings.RegisterTabId > 0)
                                        {
                                            registerLink = DotNetNuke.Common.Globals.NavigateURL(pSettings.RegisterTabId);
                                        }
                                        else
                                        {
                                            registerLink = String.Format("{0}://{1}{2}",
                                                (HttpContext.Current.Request.IsSecureConnection ? "https" : "http"), appPath, "Register.aspx"); ;
                                        }
                                        registerLink += "?regcode=" + regCode;
                                        if (EmailTokens.ContainsKey("RegisterLinkUrl"))
                                            EmailTokens["RegisterLinkUrl"] = registerLink;
                                        else
                                            EmailTokens.Add("RegisterLinkUrl", registerLink);
                                        if (!EmailTokens.ContainsKey("Message")) { EmailTokens.Add("Message", FeatureController.ConvertTextAreaLineBreaks(emailList.note)); }
                                        string body = HttpUtility.HtmlDecode(FeatureController.ReplaceTokens(_settings.InvitationEmailTemplate, EmailTokens));

                                        // Send user the invite email
                                        DotNetNuke.Services.Mail.Mail.SendEmail(
                                                    _settings.ReplyToAddress,
                                                    fmEmail,
                                                    _settings.InviteEmailSubject,
                                                    body);

                                        // Add invite record to db
                                        var t = _inviteRepo.CreateInvite(new Invitation
                                                {
                                                    PortalId = ActiveModule.PortalID,
                                                    RecipientEmailAddress = fmEmail,
                                                    InvitedByUserId = UserInfo.UserID,
                                                    RecipientRegCode = regCode,
                                                    RecipientUserId = 0,
                                                    CreatedOnDate = DateTime.Now
                                                });
                                        resp.Messages.Add(String.Format("Email '{0}' was invited.", fmEmail));
                                        inviteCount++;

                                        // Add user scoring
                                        resp.PointsEarned = 0;
                                        int logid = Mechanics.Instance.LogUserActivity(Constants.GAMING_MECHANICS_ACTION_INVITATIONSENT,
                                                            UserInfo.UserID, UserInfo.PortalID, t.InviteId, "", "");
                                        if (logid > 0)
                                        {
                                            var scoreAction = Mechanics.Instance.GetScoringAction(Constants.GAMING_MECHANICS_ACTION_INVITATIONSENT, UserInfo.PortalID);
                                            if (scoreAction != null)
                                                resp.PointsEarned = scoreAction.ReputationPoints;
                                        }

                                        // Add content item to Journal
                                        Journal journal = new Journal();
                                        journal.AddTaskToJournal(t.InvitedByUserId, t.InvitedByUserId,
                                                            0, String.Format("Invite:{0}:{1}", t.PortalId, t.InviteId), "Invitation Sent",
                                                            t.getJournalDescription(resp.PointsEarned), "", UserInfo.PortalID, 0,
                                                            Journal.JournalSecurity.Friend);
                                    }
                                    else
                                    {
                                        resp.Warnings++;
                                        resp.Messages.Add("You have reached the daily invite limit.  Not all invites were sent.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            resp.Errors++;
                            resp.Messages.Add("An error has occurred trying to send your invitations");
                            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                        }
                    }
                }

                resp.Invitations = _inviteRepo.GetUserInvites(UserInfo.UserID, DateTime.MinValue).ToList();
            }
            else
            {
                resp.Errors++;
                resp.Messages.Add("You do not have access to this feature.");
            }

            if (resp.Messages.Count == 0)
            {
                resp.Errors++;
                resp.Messages.Add("No messages were sent");
            }

            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

    }

    public class SendInvitationResponse
    {
        public SendInvitationResponse()
        { 
            Messages = new List<string>(); 
        }
        public int Errors { get; set; }
        public int Warnings { get; set; }
        public int PointsEarned { get; set; }
        public List<string> Messages { get; set; }
        public List<Invitation> Invitations { get; set; }
    }

    public class EmailList
    {
        public String[] tolist { get; set; }
        public string note { get; set; }
    }
}