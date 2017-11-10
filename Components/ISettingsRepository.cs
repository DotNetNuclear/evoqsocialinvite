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

namespace DotNetNuclear.Modules.InviteRegister.Components
{
    /// <summary>
    /// Interface for the DNN module settings values
    /// </summary>
    public interface ISettingsRepository
    {
        /// <summary>
        /// </summary>
        string ReplyToAddress { get; set; }

        /// <summary>
        /// </summary>
        string InviteEmailSubject { get; set; }

        /// <summary>
        /// </summary>
        string ModuleView { get; set; }

        /// <summary>
        /// </summary>
        string DefaultMessage { get; set; }

        /// <summary>
        /// </summary>
        int MaxEmailInvitesPerSubmit { get; set; }

        /// <summary>
        /// </summary>
        int MaxEmailInvitesPerDay { get; set; }

        /// <summary>
        /// </summary>
        bool ShowMessageToUser { get; set; }

        /// <summary>
        /// </summary>
        string InvitationEmailTemplate { get; set; }
    }
}
