using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin
{
    public class CheckInTypeDetector
    {
        private int beforeLevel;
        private int afterLevel;

        public CheckInTypeDetector(SPItemEventProperties properties)
        {
            this.beforeLevel = Int32.TryParse(properties.BeforeProperties["vti_level"].ToString(), out beforeLevel) ? beforeLevel : -1;
            this.afterLevel = Int32.TryParse(properties.AfterProperties["vti_level"].ToString(), out afterLevel) ? afterLevel : -1;
        }

        public CheckInTypeDetector(int beforeLevel, int afterLevel)
        {
            this.beforeLevel = beforeLevel;
            this.afterLevel = afterLevel;
        }

        public CheckInType Detect()
        {
            if (beforeLevel == 255 && afterLevel == 255)
                return CheckInType.MinorVersion;

            if (beforeLevel == 255 && afterLevel == 1)
                return CheckInType.MajorVersion;

            return CheckInType.None;
        }
    }
}
