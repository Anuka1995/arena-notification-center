using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.TreeView
{
    public class RuleSetNode
    {
        public string Name { get; set; }
        public Guid RuleSetId { get; set; }
        public bool IsGlobal { get; set; } = false;
        public int DaysCountBeforeApt { get; set; }
        public List<string> ExcludedOrgUnitNames { get; set; }
        public List<TextTemplateNode> TextTemplates { get; set; }
    }
}
