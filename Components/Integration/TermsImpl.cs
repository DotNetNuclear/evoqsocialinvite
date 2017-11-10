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
using DotNetNuke.Entities.Content.Taxonomy;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    public class TermsImpl : ITerms
    {
        #region Constructors

        public TermsImpl()
        {
            _termController = new TermController();
            _vocabularyController = new VocabularyController();
        }

        public TermsImpl(ITermController termController)
        {
            _termController = termController;
            _vocabularyController = new VocabularyController();
        }

        #endregion

        #region Private members

        private readonly ITermController _termController;

        private readonly IVocabularyController _vocabularyController;

        #endregion

        #region ITerms Implementation

        ///// <summary>
        ///// This should run only after the post has been added/updated in data store and the ContentItem exists.
        ///// </summary>
        ///// <param name="objPost">The content item we are associating categories with. In this module, it will always be a question (first post).</param>
        ///// <param name="objContent"></param>
        //public void ManageQuestionTerms(PostInfo objPost, ContentItem objContent)
        //{
        //    RemoveQuestionTerms(objContent);

        //    foreach (var term in objPost.Tags)
        //    {
        //        _termController.AddTermToContent(term, objContent);
        //    }
        //}

        /// <summary>
        /// Removes terms associated w/ a specific ContentItem.
        /// </summary>
        /// <param name="objContent"></param>
        public void RemoveQuestionTerms(ContentItem objContent)
        {
            _termController.RemoveTermsFromContent(objContent);
        }

        /// <summary>
        /// This method will check the core taxonomy to ensure that a term exists, if not it will create.
        /// </summary>
        public Term CreateAndReturnTerm(string name, int vocabularyId)
        {
            var existantTerm = _termController.GetTermsByVocabulary(vocabularyId).FirstOrDefault(t => t.Name.ToLower() == name.ToLower());
            if (existantTerm != null)
            {
                return existantTerm;
            }

            var termId = _termController.AddTerm(
                new Term(vocabularyId)
                    {
                        Name = name
                    });

            return new Term
                {
                    Name = name,
                    TermId = termId
                };
        }

        public Term GetTermById(int id, int vocabularyId)
        {
            return _termController.GetTermsByVocabulary(vocabularyId).FirstOrDefault(t => t.TermId == id);
        }

        /// <summary>
        /// Looks up a Taxonomy tag, and if it does not exist, then creates it.
        /// </summary>
        public Term ToTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return null;
            }

            var collection = _vocabularyController.GetVocabularies();
            if (collection == null)
            {
                return null;
            }

            var vocabulary = collection.Single(v => v.Name == "Tags");
            var vocabularyId = vocabulary.VocabularyId;

            return CreateAndReturnTerm(tag, vocabularyId);
        }

        #endregion
    }
}