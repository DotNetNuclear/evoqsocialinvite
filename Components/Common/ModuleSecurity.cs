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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;

namespace DotNetNuclear.Modules.InviteRegister.Components.Common
{
    public class ModuleSecurity
    {
        #region Constructors

        public ModuleSecurity(int moduleId, int tabId)
        {
            var mc = new ModuleController();

            var module = mc.GetModule(moduleId, tabId, false);
            if (module == null)
            {
                return;
            }

            _hasEdit = ModulePermissionController.CanEditModuleContent(module);

            _hasModerator = ModulePermissionController.HasModulePermission(module.ModulePermissions, Constants.PERMISSIONKEY);
        }

        #endregion

        #region Private members

        private readonly bool _hasEdit;

        private readonly bool _hasModerator;

        #endregion

        #region Public methods

        /// <summary>
        /// Determines if the user has moderator level permissions for the module.
        /// </summary>
        /// <returns>True if the user has moderator permissions on the module instance, false otherwise.</returns>
        public bool CanModerate()
        {
            return _hasEdit || _hasModerator;
        }

        #endregion
    }
} 