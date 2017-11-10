#region Usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Internal;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Users.Membership;
using DotNetNuclear.Modules.InviteRegister.Components;

#endregion

namespace DotNetNuclear.Modules.InviteRegister.Views
{
    public partial class Register : UserUserControlBase
    {
        private readonly List<AuthenticationLoginBase> _loginControls = new List<AuthenticationLoginBase>();

        private DnnMembershipUtilities membershipUtils;

        #region Protected Properties

        protected string AuthenticationType
        {
            get
            {
                return ViewState.GetValue("AuthenticationType", Null.NullString);
            }
            set
            {
                ViewState.SetValue("AuthenticationType", value, Null.NullString);
            }
        }

        public string InviteCode
        {
            get
            {
                if (ViewState["regcode"] == null)
                    ViewState["regcode"] = String.Empty;
                return ViewState["regcode"].ToString();
            }
        }

        protected string ReturnURL
        {
            get
            {
                return (Request.QueryString["returnurl"] == null ? "" : Request.QueryString["returnurl"].ToString());
            }
        }

        protected bool UseAuthProviders
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Registration_UseAuthProviders"));
            }
        }

        protected bool UseCaptcha
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_CaptchaRegister"));
            }
        }

        protected string UserToken
        {
            get
            {
                return ViewState.GetValue("UserToken", string.Empty);
            }
            set
            {
                ViewState.SetValue("UserToken", value, string.Empty);
            }
        }

        #endregion

        private string GetSettingValue(string key)
        {
            var value = String.Empty;
            var setting = GetSetting(UserPortalID, key);
            if ((setting != null) && (!String.IsNullOrEmpty(Convert.ToString(setting))))
            {
                value = Convert.ToString(setting);
            }
            return value;

        }



        #region Form Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            membershipUtils = new DnnMembershipUtilities(base.PortalId);

            jQuery.RequestDnnPluginsRegistration();

            ClientResourceManager.RegisterStyleSheet(Page, "~/DesktopModules/Admin/Security/module.css");
            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.extensions.js");
            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.tooltip.js");
            ClientResourceManager.RegisterScript(Page, "~/DesktopModules/Admin/Security/Scripts/dnn.PasswordComparer.js");

            membershipUtils.InitialiseUserFormEditor(ref userForm, User);

            //Verify that the current user has access to this page
            if (PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.NoRegistration && Request.IsAuthenticated == false)
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }

            cancelButton.Click += cancelButton_Click;
            registerButton.Click += registerButton_Click;

            if (UseAuthProviders)
            {
                List<AuthenticationInfo> authSystems = AuthenticationController.GetEnabledAuthenticationServices();
                foreach (AuthenticationInfo authSystem in authSystems)
                {
                    try
                    {
                        var authLoginControl = (AuthenticationLoginBase)LoadControl("~/" + authSystem.LoginControlSrc);
                        if (authSystem.AuthenticationType != "DNN")
                        {
                            BindLoginControl(authLoginControl, authSystem);
                            //Check if AuthSystem is Enabled
                            if (authLoginControl.Enabled && authLoginControl.SupportsRegistration)
                            {
                                authLoginControl.Mode = AuthMode.Register;

                                //Add Login Control to List
                                _loginControls.Add(authLoginControl);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Exceptions.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Request.IsAuthenticated)
            {
                //if a Login Page has not been specified for the portal
                if (Globals.IsAdminControl())
                {
                    //redirect to current page 
                    Response.Redirect(Globals.NavigateURL(), true);
                }
                else //make module container invisible if user is not a page admin
                {
                    if (!TabPermissionController.CanAdminPage())
                    {
                        ContainerControl.Visible = false;
                    }
                }
            }

            if (UseCaptcha)
            {
                captchaRow.Visible = true;
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", LocalResourceFile);
                ctlCaptcha.Text = Localization.GetString("CaptchaText", LocalResourceFile);
            }

            if (UseAuthProviders && String.IsNullOrEmpty(AuthenticationType))
            {
                foreach (AuthenticationLoginBase authLoginControl in _loginControls)
                {
                    socialLoginControls.Controls.Add(authLoginControl);
                }
            }

            //Display relevant message
            userHelpLabel.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS");
            switch (PortalSettings.UserRegistration)
            {
                case (int)Globals.PortalRegistrationType.PrivateRegistration:
                    userHelpLabel.Text += Localization.GetString("PrivateMembership", Localization.SharedResourceFile);
                    break;
                case (int)Globals.PortalRegistrationType.PublicRegistration:
                    userHelpLabel.Text += Localization.GetString("PublicMembership", Localization.SharedResourceFile);
                    break;
                case (int)Globals.PortalRegistrationType.VerifiedRegistration:
                    userHelpLabel.Text += Localization.GetString("VerifiedMembership", Localization.SharedResourceFile);
                    break;
            }
            userHelpLabel.Text += Localization.GetString("Required", LocalResourceFile);
            userHelpLabel.Text += Localization.GetString("RegisterWarning", LocalResourceFile);

            userForm.DataSource = User;
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["regcode"] != null)
                {
                    ViewState["regcode"] = Request.QueryString["regcode"].ToString();
                }
                userForm.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var confirmPasswordOptions = new DnnConfirmPasswordOptions()
            {
                FirstElementSelector = "." + DnnMembershipUtilities.PasswordStrengthTextBoxCssClass,
                SecondElementSelector = "." + DnnMembershipUtilities.ConfirmPasswordTextBoxCssClass,
                ContainerSelector = ".dnnRegistrationForm",
                UnmatchedCssClass = "unmatched",
                MatchedCssClass = "matched"
            };

            var optionsAsJsonString = Json.Serialize(confirmPasswordOptions);
            var script = string.Format("dnn.initializePasswordComparer({0});{1}", optionsAsJsonString, Environment.NewLine);

            if (ScriptManager.GetCurrent(Page) != null)
            {
                // respect MS AJAX
                ScriptManager.RegisterStartupScript(Page, GetType(), "ConfirmPassword", script, true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "ConfirmPassword", script, true);
            }
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(membershipUtils.GetRedirectURL(base.PortalSettings, ReturnURL), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerButton_Click(object sender, EventArgs e)
        {
            if ((UseCaptcha && ctlCaptcha.IsValid) || !UseCaptcha)
            {
                UserCreateStatus rc = membershipUtils.Validate(base.PortalId, AuthenticationType, User);
                if (rc == UserCreateStatus.AddUser)
                {
                    CreateUser();
                }
                else
                {
                    AddLocalizedModuleMessage(UserController.GetUserCreateStatus(rc), ModuleMessage.ModuleMessageType.RedError, true);
                }
            }
        }

        #endregion

        #region Private Methods

        private void CreateUser()
        {
            try
            {
                UserInfo user = User;
                UserCreateStatus rc = membershipUtils.CreateUser(PortalId, PortalSettings, ref user);
                if (rc == UserCreateStatus.Success)
                {
                    //award points to inviter user
                    FeatureController.AwardRegisterInvitePoints(InviteCode, PortalId, user.UserID);

                    //hide the successful captcha
                    captchaRow.Visible = false;

                    //Associate alternate Login with User and proceed with Login
                    if (!String.IsNullOrEmpty(AuthenticationType))
                    {
                        AuthenticationController.AddUserAuthentication(User.UserID, AuthenticationType, UserToken);
                    }

                    string strMessage = CompleteUserCreation(rc, user, true, IsRegister);
                    if ((string.IsNullOrEmpty(strMessage)))
                    {
                        Response.Redirect(membershipUtils.GetRedirectURL(base.PortalSettings, ReturnURL), true);
                    }
                }
                else
                {
                    AddLocalizedModuleMessage(UserController.GetUserCreateStatus(rc), ModuleMessage.ModuleMessageType.RedError, true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authLoginControl"></param>
        /// <param name="authSystem"></param>
        private void BindLoginControl(AuthenticationLoginBase authLoginControl, AuthenticationInfo authSystem)
        {
            //set the control ID to the resource file name ( ie. controlname.ascx = controlname )
            //this is necessary for the Localization in PageBase
            authLoginControl.AuthenticationType = authSystem.AuthenticationType;
            authLoginControl.ID = String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc), authSystem.AuthenticationType);
            authLoginControl.LocalResourceFile = authLoginControl.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" +
                                                 Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc);
            authLoginControl.RedirectURL = membershipUtils.GetRedirectURL(base.PortalSettings, ReturnURL);
            authLoginControl.ModuleConfiguration = ModuleConfiguration;

            authLoginControl.UserAuthenticated += UserAuthenticated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAuthenticated(object sender, UserAuthenticatedEventArgs e)
        {
            NameValueCollection profileProperties = e.Profile;

            User.Username = e.UserToken;
            AuthenticationType = e.AuthenticationType;
            UserToken = e.UserToken;

            foreach (string key in profileProperties)
            {
                switch (key)
                {
                    case "FirstName":
                        User.FirstName = profileProperties[key];
                        break;
                    case "LastName":
                        User.LastName = profileProperties[key];
                        break;
                    case "Email":
                        User.Email = profileProperties[key];
                        break;
                    case "DisplayName":
                        User.DisplayName = profileProperties[key];
                        break;
                    default:
                        User.Profile.SetProfileProperty(key, profileProperties[key]);
                        break;
                }
            }

            //Generate a random password for the user
            User.Membership.Password = UserController.GeneratePassword();

            if (!String.IsNullOrEmpty(User.Email))
            {
                CreateUser();
            }
            else
            {
                AddLocalizedModuleMessage(LocalizeString("NoEmail"), ModuleMessage.ModuleMessageType.RedError, true);
                foreach (DnnFormItemBase formItem in userForm.Items)
                {
                    formItem.Visible = formItem.DataField == "Email";
                }
                userForm.DataBind();
            }
        }

        #endregion
    }
}