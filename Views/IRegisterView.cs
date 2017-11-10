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
using DotNetNuke.Web.Mvp;
using DotNetNuclear.Modules.InviteRegister.Components.Entities;
using DotNetNuke.Web.UI.WebControls;
using System.Collections.Generic;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Security.Membership;
using DotNetNuke.Entities.Users;

namespace DotNetNuclear.Modules.InviteRegister.Views
{

    /// <summary>
    /// IItemView - interface for the item detail view
    /// </summary>
    public interface IRegisterView : IModuleView<RegisterInfo>
    {
        string InviteCode { get; }
        bool UserNameInUse { get; set; }
        bool IsValid { get; set; }
        int UserPortalID { get; }
        string AuthenticationType { get; set; }
        UserInfo User { get; set; }
        UserCreateStatus CreateStatus { get; set; }
        DnnFormEditor RegistrationForm { get; }

        event System.EventHandler<System.EventArgs> ModuleInit;
        event System.EventHandler<ViewLoadEventArgs> ModuleLoad;
        event System.EventHandler<System.EventArgs> RegisterUser;
        event System.EventHandler<System.EventArgs> LoginUser;
        //event System.EventHandler<ValidateUsernameEventArgs> ValidateUsername;
        event System.EventHandler<System.EventArgs> Validate;
    }

    public class ValidateUsernameEventArgs : System.EventArgs
    {
        public string Username;
    }

    public class ViewLoadEventArgs : System.EventArgs
    {
        public bool IsPostBack;
    }
}
