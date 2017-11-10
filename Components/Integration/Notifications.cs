﻿/*
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
using DotNetNuke.Framework;

namespace DotNetNuclear.Modules.InviteRegister.Components.Integration
{
    public class Notifications : ServiceLocator<INotifications, Notifications>
    {
        #region Overrides of ServiceLocator<INotifications,Notifications>

        protected override Func<INotifications> GetFactory()
        {
            return () => new NotificationsImpl();
        }

        #endregion
    }
}