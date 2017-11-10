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
using DotNetNuke.ComponentModel.DataAnnotations;

namespace DotNetNuclear.Modules.InviteRegister.Components.Entities
{
    [TableName("DNNuclear_InviteRegister_Invites")]
    //setup the primary key for table
    [PrimaryKey("InviteId", AutoIncrement = false)]
    public class InviteLookup
    {
        ///<summary>
        ///</summary>
        public int InviteId { get; set; }

        ///<summary>
        ///</summary>
        public string InviteKey { get; set; }

        ///<summary>
        ///</summary>
        public string RegisterCode { get; set; }
    }
}