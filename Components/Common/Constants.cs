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

namespace DotNetNuclear.Modules.InviteRegister.Components.Common
{
    public class Constants
    {
        #region Misc.

        public const string DESKTOPMODULE_NAME = "SocialInvite";
        public const string DESKTOPMODULE_FRIENDLYNAME = "Social Invite";
        public const string DESKTOPMODULE_FRIENDLYNAME_REG = "Social Register";
        public const string GAMING_MECHANICS_ACTION_INVITATIONSENT = "InvitationSent";
        public const string GAMING_MECHANICS_ACTION_INVITATIONUSED = "InvitationUsed";

        /// <summary>
        /// Content Types
        /// </summary>
        public const string CONTENTTYPENAME = "DNNuclear_SocialInvite";
        /// <summary>
        /// Journal types
        /// </summary>
        public const string JOURNALTASK_ADDNAME = "taskadd";
        public const string JOURNALTASK_UPDATENAME = "taskupdate";
        public const string JOURNALPROJECT_ADDNAME = "projectcreated";
        /// <summary>
        /// Notification types for the various actions
        /// </summary>
        public const string SUBSCRIPTIONTYPENAME = "DNNuclear_SocialInvite";
        public const string NOTIFICATION_INVITATIONUSED = "DNNuclear_SocialInvitationUsed";

        public const string PERMISSIONCODE = "DNNUCLEARINVITES";
        public const string PERMISSIONKEY = "MODERATORS";

        #endregion

        #region Enums

        /// <summary>
        /// 
        /// </summary>
        public enum SocialInvitePrivileges
        {
            Placeholder
        }

        /// <summary>
        /// 
        /// </summary>
        public enum SocialInviteScoringActions
        {
            InvitationSent, 
            InvitationUsed 
        }

        #endregion
    }
}