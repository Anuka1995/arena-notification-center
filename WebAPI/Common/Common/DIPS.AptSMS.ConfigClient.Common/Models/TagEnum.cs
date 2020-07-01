using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// Enumeration to handle the data types of tag template & Tag types
    /// </summary>
    public enum TagDataType
    {
        STRING = 0,
        INT =1,
        DATETIME=2
    }

    public enum TagType {
        XPATH = 0,
        STATIC_VALUE=1
    }
}
