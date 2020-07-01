using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IOverviewService
    {
        List<DepartmentNode> GenerateOverviewTree(bool getInactive, long hospitalId, Guid SecurityToken);
    }
}
