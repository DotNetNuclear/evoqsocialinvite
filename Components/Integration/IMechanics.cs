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

using DotNetNuke.Entities.Users;
using DotNetNuclear.Modules.InviteRegister.Components.Common;
using DotNetNuke.Mechanics.Entities;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMechanics
    {
        /// <summary>
        /// Logs a user's current activity (aka action) via the mechanics API.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="user"></param>
        /// <param name="contentItemId"></param>
        int LogUserActivity(string actionName, int userid, int portalid, int contentItemId, string context, string notes);

        ///<summary>
        /// Returns a scoring activity (aka action) via the mechanics API.
        ///</summary>
        /// <param name="actionName"></param>
        /// <param name="portalid"></param>
        ScoringAction GetScoringAction(string actionName, int portalid);

        /// <summary>
        /// Determines if the user has a privilege to do something via the mechanics API.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        bool HasPrivilege(UserInfo user, Constants.SocialInvitePrivileges privilege);

        #region Installation Methods

        /// <summary>
        /// Add scoring action definitions for the Boards module.
        /// </summary>
        void AddScoringDefinitions();

        /// <summary>
        /// Add privilege definitions for Boards.
        /// </summary>
        void AddPrivileges();

        #endregion
    }
}