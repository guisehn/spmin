using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace SPMin.Features.SPMin
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("dee7d000-4b73-4b41-a820-0db75041349d")]
    public class SPMinEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var siteCollection = (SPSite)properties.Feature.Parent;
            var environment = new Environment(siteCollection);

            if (String.IsNullOrEmpty(environment.RawMode))
                environment.Update(EnvironmentMode.Development);
        }
    }
}
