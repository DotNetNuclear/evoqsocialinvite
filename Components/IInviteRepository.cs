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

using DotNetNuclear.Modules.InviteRegister.Components.Entities;

namespace DotNetNuclear.Modules.InviteRegister.Components
{
    public interface IInviteRepository
    {
        ///<summary>
        /// CheckDuplicateInvite
        ///</summary>
        Invitation CheckDuplicateInvite(int userid, string email);

        ///<summary>
        /// CreateInvite
        ///</summary>
        Invitation CreateInvite(Invitation t);

        ///<summary>
        /// DeleteInvite
        ///</summary>
        void DeleteInvite(int inviteId, int userid);

        ///<summary>
        /// GetUserInvites
        ///</summary>
        IEnumerable<Invitation> GetUserInvites(int userid, DateTime after);

        ///<summary>
        /// GetInvite
        ///</summary>
        Invitation GetInvite(int inviteId);

        ///<summary>
        /// GetInviteByCode
        ///</summary>
        Invitation GetInviteByCode(string inviteCode);

        ///<summary>
        /// UpdateInvite
        ///</summary>
        void UpdateInvite(Invitation t);

    }
}
