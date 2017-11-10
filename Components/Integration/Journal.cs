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

using System.Linq;
using DotNetNuclear.Modules.InviteRegister.Components.Common;
using DotNetNuclear.Modules.InviteRegister.Components.Entities;
using DotNetNuke.Services.Journal;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    public class Journal
    {

        #region Internal Methods

        /// <summary>
        /// Informs the core journal that the user has added a new task.
        /// </summary>
        /// <param name="objCard"></param>
        /// <param name="title"></param>
        /// <param name="summary"></param>
        /// <param name="portalId"></param>
        /// <param name="journalUserId"></param>
        /// <param name="url"></param>
        internal void AddTaskToJournal(int profileid, int userid, int contentitemid,
                                        string objectKey, string title, string summary, string url,
                                        int portalid, int tabid, JournalSecurity security)
        {
            var ji = JournalController.Instance.GetJournalItemByKey(portalid, objectKey);

            if ((ji != null))
            {
                JournalController.Instance.DeleteJournalItemByKey(portalid, objectKey);
            }

            ji = new JournalItem
            {
                PortalId = portalid,
                ProfileId = profileid,
                UserId = userid,
                ContentItemId = contentitemid,
                Title = title,
                ItemData = new ItemData { Url = url },
                Summary = summary,
                Body = null,
                JournalTypeId = getTaskAddJournalTypeId(portalid, "taskadd"),
                ObjectKey = objectKey,
                SecuritySet = security.ToString().ToUpper().Substring(0, 1)
            };

            JournalController.Instance.SaveJournalItem(ji, tabid);
        }

        /// <summary>
        /// Informs the core journal that we have to delete an item (task). 
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="moduleId"></param>
        /// <param name="portalId"></param>
        internal void RemoveTaskFromJournal(string objectKey, int portalId)
        {
            JournalController.Instance.DeleteJournalItemByKey(portalId, objectKey);
        }

        /// <summary>
        /// Informs the core journal that the user has updated a task (assigned it to someone, edited description or other associated data not available on creation, etc.)
        /// </summary>
        /// <param name="objCard"></param>
        /// <param name="actionKey"></param>
        /// <param name="title"></param>
        /// <param name="portalId"></param>
        /// <param name="journalUserId"></param>
        /// <param name="url"></param>
        /// <param name="summary"> </param>
        internal void AddTaskUpdateToJournal(string objectKey, int actionKey, string title, int portalId, int contentitemid, int journalUserId, string url, int tabid, string summary)
        {
            var ji = JournalController.Instance.GetJournalItemByKey(portalId, objectKey);

            if ((ji != null))
            {
                JournalController.Instance.DeleteJournalItemByKey(portalId, objectKey);
            }

            ji = new JournalItem
            {
                PortalId = portalId,
                ProfileId = journalUserId,
                UserId = journalUserId,
                ContentItemId = contentitemid,
                Title = title,
                ItemData = new ItemData { Url = url },
                Summary = summary,
                Body = null,
                JournalTypeId = getTaskUpdateJournalTypeId(portalId),
                ObjectKey = objectKey,
                SecuritySet = "E,"
            };

            JournalController.Instance.SaveJournalItem(ji, tabid);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns a journal type associated with adding a task.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        private static int getTaskAddJournalTypeId(int portalid, string journaltypename)
        {
            var colJournalTypes = (from t in JournalController.Instance.GetJournalTypes(portalid) where t.JournalType == journaltypename select t);
            int journalTypeId;

            if (colJournalTypes.Count() > 0)
            {
                var journalType = colJournalTypes.Single();
                journalTypeId = journalType.JournalTypeId;
            }
            else
            {
                // taskadd
                journalTypeId = 28;
            }

            return journalTypeId;
        }

        /// <summary>
        /// Returns a journal type associated with commenting (using one of the core built in journal types)
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        private static int getTaskUpdateJournalTypeId(int portalId)
        {
            var colJournalTypes = (from t in JournalController.Instance.GetJournalTypes(portalId) where t.JournalType == Constants.JOURNALTASK_UPDATENAME select t);
            int journalTypeId;

            if (colJournalTypes.Any())
            {
                var journalType = colJournalTypes.Single();
                journalTypeId = journalType.JournalTypeId;
            }
            else
            {
                journalTypeId = 29;
            }

            return journalTypeId;
        }

        internal enum JournalSecurity
        {
            Everyone,
            Friend,
            User,
            Role
        }
        #endregion

    }
}