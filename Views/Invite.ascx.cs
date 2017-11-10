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
    public partial class Invite : PortalModuleBase
    {
        private ISettingsRepository _settings;

        protected string ModuleName;

        public int UserProfileTabId { get; set; }

        private bool _isModal = false;
        public bool IsModal 
        {
            get
            {
                bool _isModal = false;
                try
                {
                    Boolean.TryParse(Request.QueryString["mdl"].ToString(), out _isModal);
                }
                catch { }
                return _isModal;
            }
        }

        public string DefaultMessage { get; set; }
        public int MaxEmailInputs { get; set; }
        public int MaxDailyInvites { get; set; }
        public int DailyInviteCount { get; set; }


        /// <summary>
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _settings = new SettingsRepository(base.ModuleContext.ModuleId, base.ModuleContext.TabModuleId);
                DefaultMessage = System.Web.HttpUtility.JavaScriptStringEncode(_settings.DefaultMessage);
                MaxEmailInputs = _settings.MaxEmailInvitesPerSubmit;
                MaxDailyInvites = _settings.MaxEmailInvitesPerDay;
                pnlUserMessage.Visible = _settings.ShowMessageToUser;

                if (!IsPostBack)
                {
                    DotNetNuke.Entities.Tabs.TabController tCtrl = new DotNetNuke.Entities.Tabs.TabController();
                    var activityTab = tCtrl.GetTabByName("Activity Feed", PortalId, -1);
                    if (activityTab != null) { UserProfileTabId = activityTab.TabID; }

                    var mCtrl = new DotNetNuke.Entities.Modules.ModuleController();
                    ModuleName = mCtrl.GetModule(base.ModuleId).DesktopModule.ModuleName;

                    lnkCloseModal.Visible = IsModal;

                    IInviteRepository inviteRepo = new InviteRepository();
                    DailyInviteCount = inviteRepo.GetUserInvites(base.UserId, DateTime.Today).Count();
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

            ClientResourceManager.RegisterScript(this.Page, "/Resources/Shared/Scripts/knockout.js", 1);
            ClientResourceManager.RegisterScript(this.Page, this.ControlPath + "../Resources/js/jquery.validate.js", 2);
            ClientResourceManager.RegisterScript(this.Page, this.ControlPath + "../Resources/js/view.js", 3);
        }

    }
}