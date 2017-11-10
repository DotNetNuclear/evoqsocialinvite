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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Mechanics.Components.Controllers;
using DotNetNuke.Mechanics.Entities;
using DotNetNuclear.Modules.InviteRegister.Components.Common;


namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public class MechanicsImpl : IMechanics
    {
        #region Private members

        /// <summary>
        /// Returns an instance of the Boards desktopModule for installation purposes.
        /// </summary>
        /// <remarks>This can only be used during initial install (friendly name can be changed by hosts). </remarks>
        private static DesktopModuleInfo DesktopModule
        {
            get
            {
                return DesktopModuleController.GetDesktopModuleByFriendlyName(Constants.DESKTOPMODULE_FRIENDLYNAME);
            }
        }

        #endregion

        #region Public methods

        ///<summary>
        /// Returns a scoring activity (aka action) via the mechanics API.
        ///</summary>
        /// <param name="actionName"></param>
        /// <param name="portalid"></param>
        public ScoringAction GetScoringAction(string actionName, int portalid)
        {
            var desktopModuleId = DesktopModuleController.GetDesktopModuleByModuleName(Constants.DESKTOPMODULE_NAME, portalid).DesktopModuleID;
            ScoringAction ret = null;
            var smCtrl = MechanicsController.Instance;
            ScoringActionDefinition adef = smCtrl.GetScoringActionDefinition(actionName, desktopModuleId);
            if (adef != null)
            {
                var ctlr = new DotNetNuke.Mechanics.Components.Controllers.Internal.InternalMechanicsController();
                ret = ctlr.GetScoringAction(adef.ScoringActionDefId, portalid);
            }
            return ret;
        }

        /// <summary>
        /// Logs a user's current activity (aka action) via the mechanics API.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="userid"></param>
        /// <param name="portalid"></param>
        /// <param name="contentItemId"></param>
        /// <param name="context"></param>
        /// <param name="notes"></param>
        public int LogUserActivity(string actionName,
                                    int userid,
                                    int portalid,
                                    int contentItemId,
                                    string context,
                                    string notes)
        {
            var desktopModuleId = DesktopModuleController.GetDesktopModuleByModuleName(Constants.DESKTOPMODULE_NAME, portalid).DesktopModuleID;
            int ret = -1;
            if (desktopModuleId > 0)
            {
                var smCtrl = MechanicsController.Instance;
                ScoringActionDefinition adef = smCtrl.GetScoringActionDefinition(actionName, DesktopModule.DesktopModuleID);
                if (adef == null)
                {
                    AddScoringDefinitions();
                }

                if (adef != null)
                {
                    ret = smCtrl.LogUserActivity(adef.ActionName, adef.DesktopModuleId, new UserScoreLog
                    {
                        ContentItemId = contentItemId,
                        Context = context,
                        Notes = notes,
                        CreatedOnDate = DateTime.Now,
                        ScoringActionDefId = adef.ScoringActionDefId,
                        PortalId = portalid,
                        UserId = userid
                    });
                }
            }
            return ret;
        }

        /// <summary>
        /// Determines if the user has a privilege to do something via the mechanics API.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        public bool HasPrivilege(UserInfo user, Constants.SocialInvitePrivileges privilege)
        {
            var desktopModuleId = DesktopModuleController.GetDesktopModuleByModuleName(Constants.DESKTOPMODULE_NAME, user.PortalID).DesktopModuleID; 
            return MechanicsController.Instance.UserHasPrivilege(user, privilege.ToString(), desktopModuleId);
        }

        #region Installation Methods

        /// <summary>
        /// Add scoring action definitions for the Boards module.
        /// </summary>
        public void AddScoringDefinitions()
        {
            AddScoringAction(Constants.SocialInviteScoringActions.InvitationSent.ToString(), 0, 0, ActionTypes.Created);
            AddScoringAction(Constants.SocialInviteScoringActions.InvitationUsed.ToString(), 0, 0, ActionTypes.Edited);
        }

        /// <summary>
        /// Add privilege definitions for Boards.
        /// </summary>
        public void AddPrivileges()
        {
            AddPrivilege(Constants.SocialInvitePrivileges.Placeholder.ToString(), 0);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Add a scoring action definition if it does not already exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exp"></param>
        /// <param name="rep"></param>
        /// <param name="actionType"></param>
        /// <param name="maxCount"></param>
        /// <param name="interval"></param>
        private static void AddScoringAction(string name, int rep, int exp, ActionTypes actionType, int maxCount = 0, ScoringIntervals interval = ScoringIntervals.Undefined)
        {
            var scoringActionDef = MechanicsController.Instance.GetScoringActionDefinition(name, DesktopModule.DesktopModuleID);

            if (scoringActionDef != null) return;

            scoringActionDef = new ScoringActionDefinition
                {
                    ActionName = name,
                    DefaultExperiencePoints = exp,
                    DefaultReputationPoints = rep,
                    DefaultMaxCount = maxCount,
                    DefaultInterval = interval,
                    DesktopModuleId = DesktopModule.DesktopModuleID,
                    ActionType = actionType
                };

            MechanicsController.Instance.AddScoringActionDefinition(scoringActionDef);
        }

        /// <summary>
        /// Add a privilege definition if it does not already exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rep"></param>
        private static void AddPrivilege(string name, int rep)
        {
            var privilegeDef = MechanicsController.Instance.GetPrivilegeDefinition(name, DesktopModule.DesktopModuleID);

            if (privilegeDef != null) return;
            privilegeDef = new PrivilegeDefinition { PrivilegeName = name, DefaultReputationPoints = rep, DesktopModuleId = DesktopModule.DesktopModuleID };

            MechanicsController.Instance.AddPrivilegeDefinition(privilegeDef);
        }

        #endregion
    }
}