using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.TreeView
{
    public class TextTemplateNode
    {
        public string Name { get; set; }
        public Guid TempalteId { get; set; }
        public bool IsSMSGenerate { get; set; }
        public string TargetOrgUnitName { get; set; }
        public string LocationName { get; set; }
        public string TargetOpdName { get; set; }
    }
}
