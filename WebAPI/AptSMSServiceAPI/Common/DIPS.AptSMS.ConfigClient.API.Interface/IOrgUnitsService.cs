using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.Common;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IOrgUnitsService
    {
        List<DepartmentListItem> GetDepartmentListByHospitalId(long hospitalId, Guid SecurityToken);
        List<SectionListItem> GetSectionListByDepartmentId(long departmentId, Guid SecurityToken);
        List<WardListItem> GetWardListByDepartmentId(long departmentId, Guid SecurityToken);
        List<LocationListItem> GetLocationListByDepartmentId(long departmentId, Guid SecurityToken);
        List<WardListItem> GetWardListBySectionId(long sectionId, Guid SecurityToken);
        List<OPDListItem> GetOPDListByDepartmentId(long departmentId, Guid SecurityToken);
        List<OPDListItem> GetOPDListByHospitalId(long hospitalId, Guid SecurityToken);
        Department GetOrgUnitByDepartmentId(long departmentId, Guid SecurityToken);
        DepartmentListItem GetDepartmentByDepartmentId(long departmentId, Guid SecurityToken);
        List<DepartmentListItem> GetDepartmentListByDepartmentIdList(List<long?> departmentIdList, Guid SecurityToken);
        OrganizationalUnit GetFullOrgUnitStructure(long hospitalId, Guid securityToken);
        List<TreeNode> GetFullOrgUnitStructureTreeNodeList(long hospitalId, Guid securityToken);
        List<TreeNode> GetOrgUnitStructureForDepartment(long hospitalId, long departmentId, Guid securityToken);        
        String FindNameForOrgUnitId(long hospitalId, Guid securityToken, string orgUnitType, long orgUnitId);
    }
}
