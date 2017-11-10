
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Common.Lists;
using DotNetNuke.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Host;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using System.Web;
using DotNetNuke.Services.Localization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Security;
using DotNetNuke.Entities.Users.Membership;
using System.Globalization;
using DotNetNuke.Entities.Users.Internal;

namespace DotNetNuclear.Modules.InviteRegister.Components
{
    public class DnnMembershipUtilities
    {
        public const string PasswordStrengthTextBoxCssClass = "password-strength";
        public const string ConfirmPasswordTextBoxCssClass = "password-confirm";

        private int _portalId;

        #region Protected Properties

        public string DisplayNameFormat
        {
            get
            {
                return GetSettingValue("Security_DisplayNameFormat");
            }
        }

        public string EmailValidator
        {
            get
            {
                return GetSettingValue("Security_EmailValidation");
            }
        }


        public string ExcludeTerms
        {
            get
            {
                string excludeTerms = GetSettingValue("Registration_ExcludeTerms");
                string regex = String.Empty;
                if (!String.IsNullOrEmpty(excludeTerms))
                {
                    regex = @"^(?:(?!" + excludeTerms.Replace(" ", "").Replace(",", "|") + @").)*$\r?\n?";
                }
                return regex;
            }
        }

        public bool RandomPassword
        {
            get
            {
                return Convert.ToBoolean(UserModuleBase.GetSetting(_portalId, "Registration_RandomPassword"));
            }
        }

        public string RegistrationFields
        {
            get
            {
                return GetSettingValue("Registration_RegistrationFields");
            }
        }


        public int RegistrationFormType
        {
            get
            {
                return Convert.ToInt32(GetSettingValue("Registration_RegistrationFormType"));
            }
        }

        protected bool RequireUniqueDisplayName
        {
            get
            {
                return Convert.ToBoolean(UserModuleBase.GetSetting(_portalId, "Registration_RequireUniqueDisplayName"));
            }
        }

        public bool RequireValidProfile
        {
            get
            {
                return Convert.ToBoolean(UserModuleBase.GetSetting(_portalId, "Security_RequireValidProfile"));
            }
        }

        public bool RequirePasswordConfirm
        {
            get
            {
                return Convert.ToBoolean(UserModuleBase.GetSetting(_portalId, "Registration_RequireConfirmPassword"));
            }
        }

        protected bool UseProfanityFilter
        {
            get
            {
                return Convert.ToBoolean(UserModuleBase.GetSetting(_portalId, "Registration_UseProfanityFilter"));
            }
        }

        public string UserNameValidator
        {
            get
            {
                return GetSettingValue("Security_UserNameValidation");
            }
        }

        public bool UseEmailAsUserName
        {
            get
            {
                return Convert.ToBoolean(UserModuleBase.GetSetting(_portalId, "Registration_UseEmailAsUserName"));
            }
        }

        #endregion

        /// <summary>
        /// CSTOR
        /// </summary>
        /// <param name="portalid"></param>
        public DnnMembershipUtilities(int portalid)
        {
            _portalId = portalid;
        }

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegistrationForm"></param>
        /// <param name="User"></param>
        public void InitialiseUserFormEditor(ref DnnFormEditor RegistrationForm, UserInfo User)
        {
            if (RegistrationFormType == 0)
            {
                //UserName
                if (!UseEmailAsUserName)
                {

                    AddFieldToForm(ref RegistrationForm, "Username", String.Empty, true,
                            String.IsNullOrEmpty(UserNameValidator) ? ExcludeTerms : UserNameValidator,
                            TextBoxMode.SingleLine);
                }

                //Password
                if (!RandomPassword)
                {
                    AddPasswordStrengthFieldToForm(ref RegistrationForm, "Password", "Membership", true);

                    if (RequirePasswordConfirm)
                    {
                        AddPasswordConfirmFieldToForm(ref RegistrationForm, "PasswordConfirm", "Membership", true);
                    }
                }

                //Password Q&A
                if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                {
                    AddFieldToForm(ref RegistrationForm, "PasswordQuestion", "Membership", true, String.Empty, TextBoxMode.SingleLine);
                    AddFieldToForm(ref RegistrationForm, "PasswordAnswer", "Membership", true, String.Empty, TextBoxMode.SingleLine);
                }

                //DisplayName
                if (String.IsNullOrEmpty(DisplayNameFormat))
                {
                    AddFieldToForm(ref RegistrationForm, "DisplayName", String.Empty, true, String.Empty, TextBoxMode.SingleLine);
                }
                else
                {
                    AddFieldToForm(ref RegistrationForm, "FirstName", String.Empty, true, String.Empty, TextBoxMode.SingleLine);
                    AddFieldToForm(ref RegistrationForm, "LastName", String.Empty, true, String.Empty, TextBoxMode.SingleLine);
                }

                //Email
                AddFieldToForm(ref RegistrationForm, "Email", String.Empty, true, EmailValidator, TextBoxMode.SingleLine);

                if (RequireValidProfile)
                {
                    foreach (ProfilePropertyDefinition property in User.Profile.ProfileProperties)
                    {
                        if (property.Required)
                        {
                            AddPropertyToForm(ref RegistrationForm, property);
                        }
                    }
                }
            }
            else
            {
                var fields = RegistrationFields.Split(',').ToList();
                //append question/answer field when RequiresQuestionAndAnswer is enabled in config.
                if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                {
                    if (!fields.Contains("PasswordQuestion"))
                    {
                        fields.Add("PasswordQuestion");
                    }
                    if (!fields.Contains("PasswordAnswer"))
                    {
                        fields.Add("PasswordAnswer");
                    }
                }

                foreach (string field in fields)
                {
                    var trimmedField = field.Trim();
                    switch (trimmedField)
                    {
                        case "Username":
                            AddFieldToForm(ref RegistrationForm, "Username", String.Empty, true, String.IsNullOrEmpty(UserNameValidator)
                                                                        ? ExcludeTerms : UserNameValidator,
                                                                        TextBoxMode.SingleLine);
                            break;
                        case "Email":
                            AddFieldToForm(ref RegistrationForm, "Email", String.Empty, true, EmailValidator, TextBoxMode.SingleLine);
                            break;
                        case "Password":
                            AddPasswordStrengthFieldToForm(ref RegistrationForm, trimmedField, "Membership", true);
                            break;
                        case "PasswordConfirm":
                            AddPasswordConfirmFieldToForm(ref RegistrationForm, trimmedField, "Membership", true);
                            break;
                        case "PasswordQuestion":
                        case "PasswordAnswer":
                            AddFieldToForm(ref RegistrationForm, trimmedField, "Membership", true, String.Empty, TextBoxMode.SingleLine);
                            break;
                        case "DisplayName":
                            AddFieldToForm(ref RegistrationForm, trimmedField, String.Empty, true, ExcludeTerms, TextBoxMode.SingleLine);
                            break;
                        default:
                            ProfilePropertyDefinition property = User.Profile.GetProperty(trimmedField);
                            if (property != null)
                            {
                                AddPropertyToForm(ref RegistrationForm, property);
                            }
                            break;
                    }
                }
            }

        }

        //public UserInfo CreateNewUser(string username, string password, string email, string firstname, string lastname, string displayname, int portalId, string ipaddress)
        //{
        //    UserInfo oUser = new UserInfo();
        //    oUser.PortalID = portalId;
        //    oUser.IsSuperUser = false;
        //    oUser.FirstName = firstname;
        //    oUser.LastName = lastname;  
        //    oUser.Email = email;     
        //    oUser.Username = username;
        //    oUser.DisplayName = displayname;
        //    oUser.LastIPAddress = ipaddress;

        //    var pCt = new PortalController();
        //    var portalInf = pCt.GetPortal(portalId);

        //    //Fill MINIMUM Profile Items (KEY PIECE)
        //    oUser.Profile.PreferredLocale = portalInf.DefaultLanguage;
        //    oUser.Profile.FirstName = oUser.FirstName;
        //    oUser.Profile.LastName = oUser.LastName;

        //    // OPTIONAL - ADD custom user profile values            
        //    //oUser.Profile.SetProfileProperty("PayrollID", payrollId.ToString());

        //    //Set Membership
        //    UserMembership oNewMembership = new UserMembership(oUser);
        //    oNewMembership.Password = password;

        //    //Bind membership to user
        //    oUser.Membership = oNewMembership;

        //    //Add the user, ensure it was successful
        //    if (UserController.CreateUser(ref oUser) == UserCreateStatus.Success)
        //    {
        //        var objRoles = new RoleController();
        //        RoleInfo objRole;

        //        // autoassign user to portal roles
        //        var arrRoles = objRoles.GetPortalRoles(oUser.PortalID);
        //        for (int i = 0; i < arrRoles.Count - 1; i++)
        //        {
        //            objRole = (RoleInfo)arrRoles[i];
        //            if (objRole.AutoAssignment)
        //            {
        //                objRoles.AddUserRole(oUser.PortalID, oUser.UserID, objRole.RoleID, DateTime.MinValue, DateTime.MaxValue);
        //            }
        //        }

        //        UserLoginStatus loginStatus = ValidateDNNUser(portalId, oUser.Username, password, ipaddress, ref oUser);
        //        return oUser;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="portalSettings"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserCreateStatus CreateUser(int portalId, PortalSettings portalSettings, ref UserInfo user)
        {
            //Update DisplayName to conform to Format
            UpdateDisplayName(portalId, ref user);

            user.Membership.Approved = portalSettings.UserRegistration == (int)Globals.PortalRegistrationType.PublicRegistration;
            UserCreateStatus CreateStatus = UserController.CreateUser(ref user);

            if (CreateStatus == UserCreateStatus.Success)
            {
                var objRoles = new RoleController();

                // autoassign user to portal roles
                var arrRoles = objRoles.GetRoles(user.PortalID);
                foreach (RoleInfo r in arrRoles)
                {
                    if (r.AutoAssignment)
                    {
                        objRoles.AddUserRole(user.PortalID, user.UserID, r.RoleID, DateTime.MinValue, DateTime.MaxValue);
                    }
                }
            }

            DataCache.ClearPortalCache(portalId, true);

            return CreateStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="AuthenticationType"></param>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public UserCreateStatus Validate(int portalId, string AuthenticationType, UserInfo newUser)
        {
            var membUtils = new DnnMembershipUtilities(portalId);

            UserCreateStatus CreateStatus = UserCreateStatus.AddUser;
            var portalSecurity = new PortalSecurity();

            //Check User Editor
            //bool _IsValid = base.View.RegistrationForm.IsValid;

            if (membUtils.RegistrationFormType == 0)
            {
                //Update UserName
                if (membUtils.UseEmailAsUserName)
                {
                    newUser.Username = newUser.Email;
                    if (String.IsNullOrEmpty(newUser.DisplayName))
                    {
                        newUser.DisplayName = newUser.Email.Substring(0, newUser.Email.IndexOf("@", StringComparison.Ordinal));
                    }
                }

                //Check Password is valid
                if (!membUtils.RandomPassword)
                {
                    //Check Password is Valid
                    if (CreateStatus == UserCreateStatus.AddUser && !UserController.ValidatePassword(newUser.Membership.Password))
                    {
                        CreateStatus = UserCreateStatus.InvalidPassword;
                    }

                    if (membUtils.RequirePasswordConfirm && String.IsNullOrEmpty(AuthenticationType))
                    {
                        if (newUser.Membership.Password != newUser.Membership.PasswordConfirm)
                        {
                            CreateStatus = UserCreateStatus.PasswordMismatch;
                        }
                    }
                }
                else
                {
                    //Generate a random password for the user
                    newUser.Membership.Password = UserController.GeneratePassword();
                    newUser.Membership.PasswordConfirm = newUser.Membership.Password;
                }

            }
            else
            {
                //Set Username to Email
                if (String.IsNullOrEmpty(newUser.Username))
                {
                    newUser.Username = newUser.Email;
                }

                //Set DisplayName
                if (String.IsNullOrEmpty(newUser.DisplayName))
                {
                    newUser.DisplayName = String.IsNullOrEmpty(String.Format("{0} {1}", newUser.FirstName, newUser.LastName))
                                           ? newUser.Email.Substring(0, newUser.Email.IndexOf("@", StringComparison.Ordinal))
                                           : String.Format("{0} {1}", newUser.FirstName, newUser.LastName);
                }

                //Random Password
                if (String.IsNullOrEmpty(newUser.Membership.Password))
                {
                    //Generate a random password for the user
                    newUser.Membership.Password = UserController.GeneratePassword();
                }

                //Password Confirm
                if (!String.IsNullOrEmpty(newUser.Membership.PasswordConfirm))
                {
                    if (newUser.Membership.Password != newUser.Membership.PasswordConfirm)
                    {
                        CreateStatus = UserCreateStatus.PasswordMismatch;
                    }
                }
            }

            //Validate banned password
            var settings = new MembershipPasswordSettings(newUser.PortalID);

            if (settings.EnableBannedList)
            {
                var m = new MembershipPasswordController();
                if (m.FoundBannedPassword(newUser.Membership.Password) || newUser.Username == newUser.Membership.Password)
                {
                    CreateStatus = UserCreateStatus.BannedPasswordUsed;
                }

            }
            //Validate Profanity
            if (UseProfanityFilter)
            {
                if (!portalSecurity.ValidateInput(newUser.Username, PortalSecurity.FilterFlag.NoProfanity))
                {
                    CreateStatus = UserCreateStatus.InvalidUserName;
                }
                if (!String.IsNullOrEmpty(newUser.DisplayName))
                {
                    if (!portalSecurity.ValidateInput(newUser.DisplayName, PortalSecurity.FilterFlag.NoProfanity))
                    {
                        CreateStatus = UserCreateStatus.InvalidDisplayName;
                    }
                }
            }

            //Validate Unique User Name
            UserInfo user = UserController.GetUserByName(portalId, newUser.Username);
            if (user != null)
            {
                if (membUtils.UseEmailAsUserName)
                {
                    CreateStatus = UserCreateStatus.DuplicateEmail;
                }
                else
                {
                    CreateStatus = UserCreateStatus.DuplicateUserName;
                    int i = 1;
                    string userName = null;
                    while (user != null)
                    {
                        userName = String.Format("{0}0{1}", newUser.Username, i.ToString(CultureInfo.InvariantCulture));
                        user = UserController.GetUserByName(portalId, userName);
                        i++;
                    }
                    newUser.Username = userName;
                }
            }

            //Validate Unique Display Name
            if (CreateStatus == UserCreateStatus.AddUser && RequireUniqueDisplayName)
            {
                user = TestableUserController.Instance.GetUserByDisplayname(portalId, newUser.DisplayName);
                if (user != null)
                {
                    CreateStatus = UserCreateStatus.DuplicateDisplayName;
                    int i = 1;
                    string displayName = null;
                    while (user != null)
                    {
                        displayName = String.Format("{0} 0{1}", newUser.DisplayName, i.ToString(CultureInfo.InvariantCulture));
                        user = TestableUserController.Instance.GetUserByDisplayname(portalId, displayName);
                        i++;
                    }
                    newUser.DisplayName = displayName;
                }
            }

            //Check Question/Answer
            if (CreateStatus == UserCreateStatus.AddUser && MembershipProviderConfig.RequiresQuestionAndAnswer)
            {
                if (string.IsNullOrEmpty(newUser.Membership.PasswordQuestion))
                {
                    //Invalid Question
                    CreateStatus = UserCreateStatus.InvalidQuestion;
                }
                if (CreateStatus == UserCreateStatus.AddUser)
                {
                    if (string.IsNullOrEmpty(newUser.Membership.PasswordAnswer))
                    {
                        //Invalid Question
                        CreateStatus = UserCreateStatus.InvalidAnswer;
                    }
                }
            }

            return CreateStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PortalSettings"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public string GetRedirectURL(PortalSettings PortalSettings, string ReturnUrl)
        {
            string _RedirectURL = "";

            object setting = UserModuleBase.GetSetting(PortalSettings.PortalId, "Redirect_AfterRegistration");

            if (Convert.ToInt32(setting) > 0) //redirect to after registration page
            {
                _RedirectURL = DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(setting));
            }
            else
            {

                if (Convert.ToInt32(setting) <= 0)
                {
                    if (ReturnUrl != null)
                    {
                        //return to the url passed to register
                        _RedirectURL = HttpUtility.UrlDecode(ReturnUrl);
                        //redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
                        if (_RedirectURL.Contains("://") &&
                            !_RedirectURL.StartsWith(DotNetNuke.Common.Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias),
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            _RedirectURL = "";
                        }
                        if (_RedirectURL.Contains("?returnurl"))
                        {
                            string baseURL = _RedirectURL.Substring(0,
                                _RedirectURL.IndexOf("?returnurl", StringComparison.Ordinal));
                            string returnURL =
                                _RedirectURL.Substring(_RedirectURL.IndexOf("?returnurl", StringComparison.Ordinal) + 11);

                            _RedirectURL = string.Concat(baseURL, "?returnurl", HttpUtility.UrlEncode(returnURL));
                        }
                    }
                    if (String.IsNullOrEmpty(_RedirectURL))
                    {
                        //redirect to current page 
                        _RedirectURL = Globals.NavigateURL();
                    }
                }
                else //redirect to after registration page
                {
                    _RedirectURL = Globals.NavigateURL(Convert.ToInt32(setting));
                }
            }

            return _RedirectURL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portalid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ipaddress"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserLoginStatus ValidateDNNUser(int portalid, string username, string password, string ipaddress, ref UserInfo user)
        {
            //Try and Log User into DNN
            UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
            user = UserController.ValidateUser(portalid, username, password, "", "", ipaddress, ref loginStatus);

            return loginStatus;
        }

        #endregion

        #region Private Methods

        private void AddFieldToForm(ref DnnFormEditor RegistrationForm, string dataField, string dataMember, bool required, string regexValidator, TextBoxMode textMode)
        {
            var formItem = new DnnFormTextBoxItem
            {
                ID = dataField,
                DataField = dataField,
                DataMember = dataMember,
                Visible = true,
                Required = required,
                TextMode = textMode
            };
            if (!String.IsNullOrEmpty(regexValidator))
            {
                formItem.ValidationExpression = regexValidator;
            }
            RegistrationForm.Items.Add(formItem);
        }

        private void AddPasswordStrengthFieldToForm(ref DnnFormEditor RegistrationForm, string dataField, string dataMember, bool required)
        {
            DnnFormItemBase formItem;

            if (Host.EnableStrengthMeter)
            {
                formItem = new DnnFormPasswordItem
                {
                    TextBoxCssClass = DnnMembershipUtilities.PasswordStrengthTextBoxCssClass,
                    ContainerCssClass = "password-strength-container"
                };
            }
            else
            {
                formItem = new DnnFormTextBoxItem
                {
                    TextMode = TextBoxMode.Password,
                    TextBoxCssClass = DnnMembershipUtilities.PasswordStrengthTextBoxCssClass,
                };
            }

            formItem.ID = dataField;
            formItem.DataField = dataField;
            formItem.DataMember = dataMember;
            formItem.Visible = true;
            formItem.Required = required;

            RegistrationForm.Items.Add(formItem);
        }

        private void AddPasswordConfirmFieldToForm(ref DnnFormEditor RegistrationForm, string dataField, string dataMember, bool required)
        {
            var formItem = new DnnFormTextBoxItem
            {
                ID = dataField,
                DataField = dataField,
                DataMember = dataMember,
                Visible = true,
                Required = required,
                TextMode = TextBoxMode.Password,
                TextBoxCssClass = DnnMembershipUtilities.ConfirmPasswordTextBoxCssClass,
            };
            RegistrationForm.Items.Add(formItem);
        }

        private void AddPropertyToForm(ref DnnFormEditor RegistrationForm, ProfilePropertyDefinition property)
        {
            var controller = new ListController();
            ListEntryInfo imageType = controller.GetListEntryInfo("DataType", "Image");
            if (property.DataType != imageType.EntryID)
            {
                DnnFormEditControlItem formItem = new DnnFormEditControlItem
                {
                    ID = property.PropertyName,
                    ResourceKey = String.Format("ProfileProperties_{0}", property.PropertyName),
                    LocalResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx.resx",
                    ValidationMessageSuffix = ".Validation",
                    ControlType = EditorInfo.GetEditor(property.DataType),
                    DataMember = "Profile",
                    DataField = property.PropertyName,
                    Visible = property.Visible,
                    Required = property.Required
                };
                //To check if the property has a default value
                if (!String.IsNullOrEmpty(property.DefaultValue))
                {
                    formItem.Value = property.DefaultValue;
                }
                if (!String.IsNullOrEmpty(property.ValidationExpression))
                {
                    formItem.ValidationExpression = property.ValidationExpression;
                }
                RegistrationForm.Items.Add(formItem);
            }
        }

        private void UpdateDisplayName(int portalId, ref UserInfo user)
        {
            //Update DisplayName to conform to Format
            object setting = UserModuleBase.GetSetting(portalId, "Security_DisplayNameFormat");
            if ((setting != null) && (!string.IsNullOrEmpty(Convert.ToString(setting))))
            {
                user.UpdateDisplayName(Convert.ToString(setting));
            }
        }

        private string GetSettingValue(string key)
        {
            var value = String.Empty;
            var setting = UserModuleBase.GetSetting(_portalId, key);
            if ((setting != null) && (!String.IsNullOrEmpty(Convert.ToString(setting))))
            {
                value = Convert.ToString(setting);
            }
            return value;
        }

        #endregion
    }
}
