using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin
{
    class EnvironmentDetector
    {
        public static Environment Detect(SPSite site)
        {
            var environment = Environment.Development;
            string property = Convert.ToString(site.RootWeb.AllProperties["SPMinEnvironment"]);

            if (!String.IsNullOrEmpty(property))
                environment = ParseEnum<Environment>(property);

            return environment;
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
