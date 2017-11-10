using System;
using DotNetNuke.Web.Api;
using DotNetNuclear.Modules.InviteRegister.Components.Common;

namespace DotNetNuclear.Modules.InviteRegister.Services
{
    public class InviteServices : IServiceRouteMapper
    {
        const string moduleFolderName = Constants.DESKTOPMODULE_NAME;

        public void RegisterRoutes(IMapRoute routeManager)
        {
            routeManager.MapHttpRoute(moduleFolderName, "invite", "{controller}/{action}",
                    new[] { "DotNetNuclear.Modules.InviteRegister.Services.Controllers" });

        }
    }
}