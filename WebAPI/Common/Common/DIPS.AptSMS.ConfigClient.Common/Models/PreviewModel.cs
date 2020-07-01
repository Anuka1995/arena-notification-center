using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// SMS template inefomation 
    /// </summary>
    public class PreviewModel
    {
        public string SMSTextTemplate { get; set; }
        public bool isPathRequired { get; set; }

    }
}
