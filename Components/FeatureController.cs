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
using DotNetNuke.Services.Search;
using DotNetNuclear.Modules.InviteRegister.Components.Integration;
using DotNetNuclear.Modules.InviteRegister.Components.Common;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins.Controls;
using System.Text.RegularExpressions;

namespace DotNetNuclear.Modules.InviteRegister.Components
{

    public class FeatureController : IUpgradeable
    {
        public static string SharedResourceFile 
        {
            get
            {
                return DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/DotNetNuclear.InviteRegister/App_LocalResources/SharedResources.resx");
            }
        }

        #region Upgradeable

        //-----------------------------------------------------------------------------
        //<summary>
        //UpgradeModule implements the IUpgradeable Interface
        //</summary>
        //<param name="Version">The current version of the module</param>
        //-----------------------------------------------------------------------------
        public string UpgradeModule(string version)
        {
            var message = String.Format("Social Invite Upgradeable actions for verion {0}.)", version);

            switch (version)
            {
                case "01.00.00":
                    Integration.Mechanics.Instance.AddScoringDefinitions();
                    message += "Added scoring definitions for Social Invite. " + Environment.NewLine;

                    Integration.Notifications.Instance.AddNotificationTypes();
                    message += "Added notification types for Social Invite. " + Environment.NewLine;

                    Integration.Content.Instance.AddContentType();
                    message += "Added content types for Social Invite. " + Environment.NewLine;

                    DotNetNuke.SocialLibrary.Components.Common.Utilities.CategorizeSocialModule(DesktopModuleController.GetDesktopModuleByFriendlyName(Constants.DESKTOPMODULE_FRIENDLYNAME));
                    DotNetNuke.SocialLibrary.Components.Common.Utilities.CategorizeSocialModule(DesktopModuleController.GetDesktopModuleByFriendlyName(Constants.DESKTOPMODULE_FRIENDLYNAME_REG));
                    message += "Added Social Invite to Social module category. " + Environment.NewLine;

                    break;
            }

            return message;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inviteCode"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        public static void AwardRegisterInvitePoints(string inviteCode, int portalId, int invitedUserId)
        {
            IInviteRepository _repository = new InviteRepository();
            // Lookup invite code and reward points
            if (inviteCode != String.Empty)
            {
                int pointsEarned = 0;
                var invite = _repository.GetInviteByCode(inviteCode);
                if (invite != null)
                {
                    // Add user scoring
                    int logid = Mechanics.Instance.LogUserActivity(Constants.GAMING_MECHANICS_ACTION_INVITATIONUSED,
                                                        invite.InvitedByUserId, portalId, invite.InviteId, "", "");
                    if (logid > 0)
                    {
                        var scoreAction = Mechanics.Instance.GetScoringAction(Constants.GAMING_MECHANICS_ACTION_INVITATIONUSED, portalId);
                        if (scoreAction != null)
                            pointsEarned = scoreAction.ReputationPoints;
                    }

                    // Send Notification to invite user
                    Notifications.Instance.SendInvitationUsedNotification(invite);

                    // Add Journal entry
                    Journal journal = new Journal();
                    journal.AddTaskToJournal(invite.InvitedByUserId, invite.InvitedByUserId,
                                        0, String.Format("InviteReg:{0}:{1}", invite.PortalId, invite.InviteId), "Invitation Used",
                                        invite.getJournalRegisterDescription(pointsEarned), "", invite.PortalId, 0,
                                        Journal.JournalSecurity.Friend);

                    // Update the invite with the recipient's userid
                    invite.RecipientUserId = invitedUserId;
                    _repository.UpdateInvite(invite);
                }
            }
        }

        public static string GetSharedResourceString(string resourceKey)
        {
            return DotNetNuke.Services.Localization.Localization.GetString(resourceKey, SharedResourceFile); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <returns></returns>
        public static List<FileInfo> GetInviteModuleViews(string sourceDirectory)
        {
            List<FileInfo> allowedViews = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(sourceDirectory));
            FileInfo[] views = dir.GetFiles("*.ascx", SearchOption.TopDirectoryOnly);
            foreach (FileInfo view in views)
            {
                if (view.Name.Contains("Invite"))
                {
                    allowedViews.Add(view);
                }
            }
            return allowedViews;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisModule"></param>
        /// <param name="moduleContainer"></param>
        /// <returns></returns>
        public static ModuleMessage.ModuleMessageType LoadModuleControl(PortalModuleBase thisModule, PlaceHolder moduleContainer)
        {
            ModuleMessage.ModuleMessageType result = ModuleMessage.ModuleMessageType.RedError;
            try
            {
                Components.SettingsRepository _settingsCtrl = new Components.SettingsRepository(thisModule.ModuleContext.ModuleId, thisModule.ModuleContext.TabModuleId);

                if (!String.IsNullOrEmpty(_settingsCtrl.ModuleView))
                {
                    string modulePath = String.Format("{0}/{1}", thisModule.TemplateSourceDirectory, _settingsCtrl.ModuleView);
                    var objControl = thisModule.LoadControl(modulePath) as PortalModuleBase;
                    if (objControl != null)
                    {
                        moduleContainer.Controls.Clear();
                        objControl.ModuleContext.Configuration = thisModule.ModuleContext.Configuration;
                        objControl.LocalResourceFile = thisModule.LocalResourceFile.Replace("Dispatch", _settingsCtrl.ModuleView.Replace(".ascx", ""));
                        objControl.ID = System.IO.Path.GetFileNameWithoutExtension(modulePath);
                        moduleContainer.Controls.Add(objControl);
                        result = ModuleMessage.ModuleMessageType.GreenSuccess;
                    }
                }
                if (result != ModuleMessage.ModuleMessageType.GreenSuccess)
                {
                    result = ModuleMessage.ModuleMessageType.YellowWarning;
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                result = ModuleMessage.ModuleMessageType.RedError;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string ReplaceTokens(string template, Dictionary<string, string> replacements)
        {
            var rex = new Regex(@"\${([^}]+)}");
            return (rex.Replace(template, delegate(Match m)
            {
                string key = m.Groups[1].Value;
                string rep = replacements.ContainsKey(key) ? replacements[key] : m.Value;
                return (rep);
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertTextAreaLines(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            else
            {
                return text.Replace("\n", "&#013;&#010;");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertTextAreaLineBreaks(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            else
            {
                return text.Replace("\n", "&lt;br&gt;");
            }
        }
    }
}
