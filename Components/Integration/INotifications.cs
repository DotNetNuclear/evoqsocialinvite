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

using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuclear.Modules.InviteRegister.Components.Common;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Entities.Users;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    public interface INotifications
    {

        #region Install Methods

        /// <summary>
        /// This will create a notification type associated w/ the module and also handle the actions that must be associated with it.
        /// </summary>
        void AddNotificationTypes();

        /// <summary>
        /// This will create an invitation used notification.
        /// </summary>
        void SendInvitationUsedNotification(Entities.Invitation invite);

        #endregion
    }
}