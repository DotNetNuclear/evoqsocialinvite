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
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Common.Utilities;
using DotNetNuclear.Modules.InviteRegister.Components.Common;
using System.Collections.Generic;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    public class ContentImpl : IContent
    {
        #region Constructors

        public ContentImpl()
        {
            var dataService = new DataService();

            _typeController = new ContentTypeController(dataService);
            _contentController = new ContentController(dataService);
            _termController = new TermController(dataService);
        }

        public ContentImpl(IContentController contentController, IContentTypeController typeController)
        {
            _contentController = contentController;
            _typeController = typeController;
        }

        #endregion

        #region Private members

        private readonly IContentController _contentController;
        private readonly IContentTypeController _typeController;
        private readonly ITermController _termController;

        #endregion

        #region Public methods

        /// <summary>
        /// This should only run after the Post exists in the data store. 
        /// </summary>
        /// <returns>The newly created ContentItemID from the data store.</returns>
        /// <remarks>This is for the first question in the thread. Not for replies or items with ParentID > 0.</remarks>
        public ContentItem CreateContentItem(ContentItem contentItem)
        {
            contentItem.ContentItemId = _contentController.AddContentItem(contentItem);

            // Add terms
            _termController.RemoveTermsFromContent(contentItem);

            foreach (var term in contentItem.Terms)
            {
                _termController.AddTermToContent(term, contentItem);
            }

            return contentItem;
        }

        /// <summary>
        /// This is used to update the content in the ContentItems table. Should be called when a question is updated.
        /// </summary>
        public void UpdateContentItem(ContentItem contentItem)
        {
            var terms = contentItem.Terms;

            _contentController.UpdateContentItem(contentItem);

            _termController.RemoveTermsFromContent(contentItem);

            foreach (var term in terms)
            {
                _termController.AddTermToContent(term, contentItem);
            }
        }

        /// <summary>
        /// This removes a content item associated with a question/thread from the data store. Should run every time an entire thread is deleted.
        /// </summary>
        /// <param name="contentItemId"></param>
        public void DeleteContentItem(int contentItemId)
        {
            if (contentItemId <= Null.NullInteger)
            {
                return;
            }

            var contentItem = _contentController.GetContentItem(contentItemId);
            if (contentItem == null)
            {
                return;
            }

            // remove any metadata/terms associated first (perhaps we should just rely on ContentItem cascade delete here?)
            var cntTerms = new TermsImpl();
            cntTerms.RemoveQuestionTerms(contentItem);

            _contentController.DeleteContentItem(contentItem);
        }

        /// <summary>
        /// This is used to determine the ContentTypeID (part of the Core API) based on this module's content type. If the content type doesn't exist yet for the module, it is created.
        /// </summary>
        /// <returns>The primary key value (ContentTypeID) from the core API's Content Types table.</returns>
        public int GetContentTypeId()
        {
            var contentTypes = (from t in _typeController.GetContentTypes() where t.ContentType == Constants.CONTENTTYPENAME select t);

            if (contentTypes.Any())
            {
                var contentType = contentTypes.Single();
                return contentType.ContentTypeId;
            }

            return -1;
        }

        /// <summary>
        /// </summary>
        /// <returns>List of Content Items matching the tabid</returns>
        public IEnumerable<ContentItem> GetContentItemsByTabId(int tabId)
        {
            return _contentController.GetContentItemsByTabId(tabId);
        }

        /// <summary>
        /// </summary>
        /// <returns>Content Item matching the id</returns>
        public ContentItem GetContentItem(int contentItemId)
        {
            return _contentController.GetContentItem(contentItemId);
        }

        /// <summary>
        /// Creates a Content Type (for taxonomy) in the data store, fired via IUpgradeable.
        /// </summary>
        public int AddContentType()
        {
            int id = GetContentTypeId();
            if (id < 1)
            {
                id = createContentType();
            }
            return id;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates a Content Type (for taxonomy) in the data store.
        /// </summary>
        /// <returns>The primary key value of the new ContentType.</returns>
        private int createContentType()
        {
            return _typeController.AddContentType(new ContentType
            {
                ContentType = Constants.CONTENTTYPENAME
            });
        }

        #endregion
    }
}