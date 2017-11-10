using System.Collections.Generic;
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
using DotNetNuke.Entities.Content;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    public interface IContent
    {
        /// <summary>
        /// This should only run after the Post exists in the data store. 
        /// </summary>
        /// <returns>The newly created ContentItemID from the data store.</returns>
        /// <remarks>This is for the first question in the thread. Not for replies or items with ParentID > 0.</remarks>
        ContentItem CreateContentItem(ContentItem contentItem);

        /// <summary>
        /// This is used to update the content in the ContentItems table. Should be called when a question is updated.
        /// </summary>
        void UpdateContentItem(ContentItem contentItem);

        /// <summary>
        /// This removes a content item associated with a question/thread from the data store. Should run every time an entire thread is deleted.
        /// </summary>
        /// <param name="contentItemId"></param>
        void DeleteContentItem(int contentItemId);

        /// <summary>
        /// This is used to determine the ContentTypeID (part of the Core API) based on this module's content type. If the content type doesn't exist yet for the module, it is created.
        /// </summary>
        /// <returns>The primary key value (ContentTypeID) from the core API's Content Types table.</returns>
        int GetContentTypeId();

        /// <summary>
        /// </summary>
        /// <returns>List of Content Items matching the tabid</returns>
        IEnumerable<ContentItem> GetContentItemsByTabId(int tabId);

        /// <summary>
        /// </summary>
        /// <returns>Content Item matching the id</returns>
        ContentItem GetContentItem(int contentItemId);

        /// <summary>
        /// Creates a Content Type (for taxonomy) in the data store, fired via IUpgradeable.
        /// </summary>
        int AddContentType();
    }
}