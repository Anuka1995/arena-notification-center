using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.TreeView
{
    public class DepartmentNode
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public List<RuleSetNode> Schedules { get; set; }
    }
}
