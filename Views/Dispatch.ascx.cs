using DotNetNuclear.Modules.InviteRegister.Components;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Web.Mvp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuclear.Modules.InviteRegister.Views
{
    public partial class Dispatch : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ModuleMessage.ModuleMessageType result = FeatureController.LoadModuleControl(this, phModuleControl);

            if (result == ModuleMessage.ModuleMessageType.YellowWarning)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, LocalizeString("LoadControlWarning"), result);
            }
            else if (result == ModuleMessage.ModuleMessageType.RedError)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, LocalizeString("LoadControlError"), result);
            }
        }
    }
}