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
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;

namespace DotNetNuclear.Modules.InviteRegister.Components.Entities
{
    public class Invitation
    {
        ///<summary>
        ///</summary>
        public int InviteId { get; set; }

        ///<summary>
        /// The PortalID of where the object was created and gets displayed
        ///</summary>
        public int PortalId { get; set; }

        ///<summary>
        ///</summary>
        public int InvitedByUserId { get; set; }

        ///<summary>
        ///</summary>
        public string RecipientEmailAddress { get; set; }

        ///<summary>
        ///</summary>
        public int RecipientUserId { get; set; }

        ///<summary>
        ///</summary>
        public string RecipientRegCode { get; set; }

        ///<summary>
        ///</summary>
        //[IgnoreColumn]
        public InviteRecipientUser RecipientUser { get; set; }

        ///<summary>
        /// The date the object was created
        ///</summary>
        public DateTime CreatedOnDate { get; set; }

        public string getJournalDescription(int pointsEarned)
        {
            string msg = String.Format("You send an invitation to '{0}'.", RecipientEmailAddress);
            if (pointsEarned > 0)
                msg += String.Format("You earned {0} points!", pointsEarned);

            return msg;
        }

        public string getJournalRegisterDescription(int pointsEarned)
        {
            string msg = String.Format("A new user joined from the invitation you sent to {0} on {1:d}.", RecipientEmailAddress, CreatedOnDate);
            if (pointsEarned > 0)
                msg += String.Format("You earned {0} points!", pointsEarned);

            return msg;
        }
    }

    public class InvitationList
    {
        public List<Invitation> Invitations { get; set; }

        public InvitationList()
        {
            Invitations = new List<Invitation>();
        }
    }

    public class InviteRecipientUser
    {
        public int UserId { get; set; }
        public bool IsVisible { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public DateTime JoinedDate { get; set; }

        public InviteRecipientUser() { }

        public InviteRecipientUser(DotNetNuke.Entities.Users.UserInfo dnnUser, int inviteUserId) 
        {
            if (dnnUser != null)
            {
                UserId = dnnUser.UserID;
                FirstName = dnnUser.FirstName;
                LastName = dnnUser.LastName;
                DisplayName = dnnUser.DisplayName;
                JoinedDate = dnnUser.CreatedOnDate;
                IsVisible = !dnnUser.IsDeleted;
                Status = "Not Connected";
                var rel = dnnUser.Social.UserRelationships.Where(x => x.UserId == inviteUserId).FirstOrDefault();
                if (rel != null)
                {
                    switch (rel.Status)
                    {
                        case DotNetNuke.Entities.Users.Social.RelationshipStatus.Accepted:
                            Status = "Friends";
                            break;
                        case DotNetNuke.Entities.Users.Social.RelationshipStatus.Pending:
                            Status = "Pending Friend Request";
                            break;
                    }
                }
            }
        }
    }
}