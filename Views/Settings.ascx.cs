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
using DotNetNuke.Services.Exceptions;
using System.Web.UI.WebControls;
using DotNetNuclear.Modules.InviteRegister.Components;
using DotNetNuke.Services.Localization;

namespace DotNetNuclear.Modules.InviteRegister.Views
{
    public partial class Settings : ModuleSettingsBase
    {
        #region Base Method Implementations

        private Components.SettingsRepository _settingsCtrl;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                _settingsCtrl = new Components.SettingsRepository(this.ModuleId, this.TabModuleId);
                if (!Page.IsPostBack)
                {
                    ddlControlToLoad.AppendDataBoundItems = true;
                    ddlControlToLoad.DataSource = FeatureController.GetInviteModuleViews(TemplateSourceDirectory);
                    ddlControlToLoad.DataBind();
                    ddlControlToLoad.SelectedValue = _settingsCtrl.ModuleView;

                    txtDefaultMessage.Text = _settingsCtrl.DefaultMessage;
                    txtMaxEmailInvitesPerSubmit.Text = _settingsCtrl.MaxEmailInvitesPerSubmit.ToString();
                    txtMaxEmailInvitesPerDay.Text = _settingsCtrl.MaxEmailInvitesPerDay.ToString();
                    chkShowMessage.Checked = _settingsCtrl.ShowMessageToUser;

                    txtEmailTemplate.Text = _settingsCtrl.InvitationEmailTemplate;
                    ShowHideEmailSettings();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            int tmpInt = 0;
            try
            {
                _settingsCtrl = new Components.SettingsRepository(this.ModuleId, this.TabModuleId);

                _settingsCtrl.ModuleView = ddlControlToLoad.SelectedValue;
                _settingsCtrl.DefaultMessage = txtDefaultMessage.Text;
                _settingsCtrl.ShowMessageToUser = chkShowMessage.Checked;

                Int32.TryParse(txtMaxEmailInvitesPerSubmit.Text, out tmpInt);
                if (tmpInt > 0) {
                    _settingsCtrl.MaxEmailInvitesPerSubmit = tmpInt; 
                }
                tmpInt = 0;
                Int32.TryParse(txtMaxEmailInvitesPerDay.Text, out tmpInt);
                if (tmpInt > 0)
                {
                    _settingsCtrl.MaxEmailInvitesPerDay = tmpInt;
                }
                _settingsCtrl.InvitationEmailTemplate = txtEmailTemplate.Text;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        protected void ddlControlToLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHideEmailSettings();
        }

        private void ShowHideEmailSettings()
        {
            pnlBasicEmailSettings.Visible = (ddlControlToLoad.Text.ToLower() != "invitelist.ascx");
            pnlEmailTemplate.Visible = (ddlControlToLoad.Text.ToLower() != "invitelist.ascx");
        }
    }
}