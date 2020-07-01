using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// Handling the contact type of patient 
    /// </summary>
    public class ContactType
    {
        public long ContactTypeId { get; set; }
        public string ContactTypeName { get; set; }
    }
}
