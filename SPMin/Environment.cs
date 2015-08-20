using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin
{
    class Environment
    {
        private const string PropertyName = "SPMinEnvironment";
        private const EnvironmentMode DefaultMode = EnvironmentMode.Development;

        private SPWeb rootWeb;

        public Environment(SPSite site)
        {
            this.rootWeb = site.RootWeb;
        }

        public EnvironmentMode Mode
        {
            get
            {
                var environment = DefaultMode;

                if (!String.IsNullOrEmpty(RawMode))
                    environment = ParseEnum<EnvironmentMode>(RawMode);

                return environment;
            }
        }

        public string RawMode
        {
            get
            {
                return Convert.ToString(rootWeb.AllProperties[PropertyName]);
            }
        }

        public void Update(EnvironmentMode mode)
        {
            rootWeb.AllProperties[PropertyName] = mode.ToString();
            rootWeb.Update();
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
