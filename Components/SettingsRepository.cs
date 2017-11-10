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
using DotNetNuke.Entities.Modules;

namespace DotNetNuclear.Modules.InviteRegister.Components
{
    /// <summary>
    /// Implementation class for module settings data
    /// </summary>
    public class SettingsRepository : ISettingsRepository
    {
        /// <summary>
        /// </summary>
        public static bool SettingsChanged = false;

        private ModuleController _controller;
        private int _moduleId;
        private int _tabModuleId;

        /// <summary>
        /// </summary>
        public SettingsRepository(int moduleId)
        {
            _controller = new ModuleController();
            _moduleId = moduleId;
        }

        /// <summary>
        /// </summary>
        public SettingsRepository(int moduleId, int tabModuleId)
        {
            _controller = new ModuleController();
            _moduleId = moduleId;
            _tabModuleId = tabModuleId;
        }

        #region setting methods

        /// <summary>
        /// </summary>
        protected T ReadSetting<T>(string settingName, T defaultValue)
        {
            Hashtable settings = _controller.GetModuleSettings(_moduleId);

            T ret = default(T);

            if (settings.ContainsKey(settingName))
            {
                System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                try
                {
                    ret = (T)tc.ConvertFrom(settings[settingName]);
                }
                catch
                {
                    ret = defaultValue;
                }
            }
            else
                ret = defaultValue;

            return ret;
        }

        /// <summary>
        /// </summary>
        protected void WriteSetting(string settingName, string value)
        {
            _controller.UpdateModuleSetting(_moduleId, settingName, value);
            SettingsChanged = true;
        }

        /// <summary>
        /// </summary>
        protected T ReadTabSetting<T>(string settingName, T defaultValue)
        {
            Hashtable settings = _controller.GetTabModuleSettings(_tabModuleId);

            T ret = default(T);

            if (settings.ContainsKey(settingName))
            {
                System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                try
                {
                    ret = (T)tc.ConvertFrom(settings[settingName]);
                }
                catch
                {
                    ret = defaultValue;
                }
            }
            else
                ret = defaultValue;

            return ret;
        }

        /// <summary>
        /// </summary>
        protected void WriteTabSetting(string settingName, string value)
        {
            _controller.UpdateTabModuleSetting(_tabModuleId, settingName, value);
            SettingsChanged = true;
        }

        #endregion

        #region public properties

        /// <summary>
        /// </summary>
        public string ReplyToAddress
        {
            get { return ReadSetting<string>("ReplyToAddress", "noreply@email.com"); }
            set { WriteSetting("ReplyToAddress", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public string InviteEmailSubject
        {
            get { return ReadSetting<string>("InviteEmailSubject", "You are invited to join"); }
            set { WriteSetting("InviteEmailSubject", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public string ModuleView
        {
            get { return ReadSetting<string>("ModuleView", "Invite.ascx"); }
            set { WriteSetting("ModuleView", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public string DefaultMessage
        {
            get { return ReadSetting<string>("DefaultMessage", ""); }
            set { WriteSetting("DefaultMessage", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public int MaxEmailInvitesPerSubmit
        {
            get { return ReadSetting<int>("MaxEmailInvitesPerSubmit", 10); }
            set { WriteSetting("MaxEmailInvitesPerSubmit", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public int MaxEmailInvitesPerDay
        {
            get { return ReadSetting<int>("MaxEmailInvitesPerDay", 50); }
            set { WriteSetting("MaxEmailInvitesPerDay", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public bool ShowMessageToUser
        {
            get { return ReadSetting<bool>("ShowMessageToUser", true); }
            set { WriteSetting("ShowMessageToUser", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public string InvitationEmailTemplate
        {
            get { return ReadSetting<string>("InvitationEmailTemplate", FeatureController.GetSharedResourceString("InviteEmailTemplate")); }
            set { WriteSetting("InvitationEmailTemplate", value); }
        }
        #endregion
    }
}