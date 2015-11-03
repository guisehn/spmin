using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin
{
    public class SPMinLoggingService : SPDiagnosticsServiceBase
    {
        private static string AreaName = "Mavention";
        private static SPMinLoggingService _instance;

        public static SPMinLoggingService Current
        {
            get
            {
                if (_instance == null)
                    _instance = new SPMinLoggingService();

                return _instance;
            }
        }

        private SPMinLoggingService() : base("SPMin Logging Service", SPFarm.Local) { }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(AreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory("SPMin", TraceSeverity.Unexpected, EventSeverity.Error)
                })
            };

            return areas;
        }

        public static void LogError(string errorMessage, TraceSeverity traceSeverity)
        {
            SPDiagnosticsCategory category = Current.Areas[AreaName].Categories["SPMin"];
            Current.WriteTrace(0, category, traceSeverity, errorMessage);
        }
    }
}
