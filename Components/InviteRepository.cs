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
using System.Web.Script.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuclear.Modules.InviteRegister.Components.Entities;
using DotNetNuclear.Modules.InviteRegister.Components.Integration;

namespace DotNetNuclear.Modules.InviteRegister.Components
{
    public class InviteRepository : IInviteRepository
    {
        private int _itemContentTypeId;
        
        #region cstor
        public InviteRepository()
        {
            _itemContentTypeId = Content.Instance.GetContentTypeId();
            if (_itemContentTypeId < 1)
               _itemContentTypeId = Content.Instance.AddContentType();
        }
        #endregion

        public Invitation CheckDuplicateInvite(int userid, string email)
        {
            string invKey = String.Format("{0}|{1}", userid, email);
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<InviteLookup>();
                var dup = rep.Find("WHERE InviteKey = @0", invKey).FirstOrDefault();
                if (dup != null)
                {
                    return GetInvite(dup.InviteId);
                }
                else
                {
                    return null;
                }
            }
        }

        public Invitation CreateInvite(Invitation t)
        {
            try
            {
                // Add content item
                
                var newCI = Content.Instance.CreateContentItem(convertModeltoContentItem((Invitation)t));
                t.InviteId = newCI.ContentItemId;
                t.RecipientUser = new InviteRecipientUser(UserController.GetUserById(t.PortalId, t.RecipientUserId), t.RecipientUserId);
                // Add lookup record
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<InviteLookup>();
                    rep.Insert(new InviteLookup
                    {
                        InviteId = newCI.ContentItemId,
                        InviteKey = String.Format("{0}|{1}", t.InvitedByUserId, t.RecipientEmailAddress),
                        RegisterCode = t.RecipientRegCode
                    });
                }
                DataCache.RemoveCache(itemCacheKey(t.InvitedByUserId));
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            return t;
        }

        public void DeleteInvite(int inviteId, int userid)
        {
            // Delete contentitem invite
            Content.Instance.DeleteContentItem(inviteId);
            DataCache.RemoveCache(itemCacheKey(userid));

            // Delete invite lookup
            var l = getInviteLookup(inviteId);
            deleteInviteLookup(l);
        }

        public IEnumerable<Invitation> GetUserInvites(int userid, DateTime after)
        {
            List<Invitation> items = new List<Invitation>();
            var contentItems = Content.Instance.GetContentItemsByTabId(userid)
                                .Where(c => c.ContentTypeId == _itemContentTypeId && c.CreatedOnDate >= after).ToList();
            foreach (ContentItem ci in contentItems)
            {
                items.Add(convertContentItemtoModel(ci));
            }
            return items;
        }

        public Invitation GetInvite(int inviteId)
        {
            ContentItem ci = Content.Instance.GetContentItem(inviteId);
            if (ci != null)
            {
                return convertContentItemtoModel(ci);
            }
            else
            {
                return null;
            }
        }

        public Invitation GetInviteByCode(string inviteCode)
        {
            Invitation t = null;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<InviteLookup>();
                var luInv = rep.Find("WHERE RegisterCode = @0", inviteCode).FirstOrDefault();
                if (luInv != null)
                {
                    t = GetInvite(luInv.InviteId);
                }
            }
            return t;
        }

        public void UpdateInvite(Invitation t)
        {
            Content.Instance.UpdateContentItem(convertModeltoContentItem((Invitation)t));
            DataCache.RemoveCache(itemCacheKey(t.RecipientUserId));
        }

        #region private methods

        private void deleteInviteLookup(InviteLookup l)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<InviteLookup>();
                rep.Delete(l);
            }
        }

        private string itemCacheKey(int moduleId)
        {
            return "SL_Invitation_" + moduleId.ToString();
        }

        private Invitation convertContentItemtoModel(ContentItem ci)
        {
            Invitation t = new Invitation();

            try
            {
                t = new JavaScriptSerializer().Deserialize<Invitation>(ci.Content);
                if (t != null) 
                { 
                    t.InviteId = ci.ContentItemId;
                    t.RecipientUser = new InviteRecipientUser(UserController.GetUserById(t.PortalId, t.RecipientUserId), t.RecipientUserId);
                    t.CreatedOnDate = ci.CreatedOnDate;
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }

            return t;
        }

        private ContentItem convertModeltoContentItem(Invitation t)
        {
            ContentItem ci = new ContentItem();
            ci = new ContentItem
            {
                Content = new JavaScriptSerializer().Serialize(t),
                ContentKey = String.Format("{0}|{1}", t.InvitedByUserId, t.RecipientEmailAddress),
                ContentTitle = String.Format("Invitation sent to {0}", t.RecipientEmailAddress),
                ContentTypeId = _itemContentTypeId,
                //Put userid in tab id to retrieve using API (no FK references will be broken)
                TabID = t.InvitedByUserId, 
                ModuleID = -1, 
                ContentItemId = t.InviteId
            };

            return ci;
        }

        private InviteLookup getInviteLookup(int inviteId)
        {
            InviteLookup t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<InviteLookup>();
                t = rep.GetById(inviteId);
            }
            return t;
        }

        #endregion
    }
}
