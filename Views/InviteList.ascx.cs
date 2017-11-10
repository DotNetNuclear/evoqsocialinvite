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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Mvp;
using DotNetNuke.Web.Client.ClientResourceManagement;
using WebFormsMvp;

using DotNetNuclear.Modules.InviteRegister.Components;
using DotNetNuclear.Modules.InviteRegister.Components.Entities;

namespace DotNetNuclear.Modules.InviteRegister.Views
{
    public partial class InviteList : PortalModuleBase
    {
        protected string ModuleName;

        private List<Invitation> _invitations = null;

        protected string InvitationsJSON 
        {
            get
            {
                var jsSer = new JavaScriptSerializer();
                InvitationList lst = new InvitationList();
                if (_invitations != null && _invitations.Any())
                {
                    lst.Invitations = _invitations;
                }
                return jsSer.Serialize(lst);
            }
        }
        protected string MissingUserAvatar
        {
            get
            {
                return this.ControlPath + @"../Resources/images/unknown_avatar.png";
            }
        }

        public int UserProfileTabId { get; set; }

        /// <summary>
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    IInviteRepository _repository = new InviteRepository();
                    _invitations = _repository.GetUserInvites(UserId, DateTime.MinValue).ToList();

                    DotNetNuke.Entities.Tabs.TabController tCtrl = new DotNetNuke.Entities.Tabs.TabController();
                    var activityTab = tCtrl.GetTabByName("Activity Feed", PortalId, -1);
                    if (activityTab != null) { UserProfileTabId = activityTab.TabID; }

                    var mCtrl = new DotNetNuke.Entities.Modules.ModuleController();
                    ModuleName = mCtrl.GetModule(base.ModuleId).DesktopModule.ModuleName;
                }
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxScriptSupport();
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }
            catch (Exception exc) //Module failed to load
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            ClientResourceManager.RegisterStyleSheet(this.Page, this.ControlPath + "../Resources/css/view.css");

            ClientResourceManager.RegisterScript(this.Page, this.ControlPath + "../Resources/js/knockout-2.2.1.js", 1);
            ClientResourceManager.RegisterScript(this.Page, this.ControlPath + "../Resources/js/view.js", 3);
        }

    }
}