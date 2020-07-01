using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.Infrastructure.Logging;
using DIPS.OrganizationalUnits.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class OrgUnitsService : IOrgUnitsService
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(OrgUnitsService));
        private readonly IWcfClient m_wcfClient;
        private readonly IWcfConfigurationProvider m_configurationProvider;
        private IMemoryCache m_cache;

        //hospital based cache keys-insert hospital id front
        private readonly string OrgUnitCacheKey = "FullOrgUnit";
        private readonly string DepartmentListCacheKey = "DepartmentList";
        private readonly string OPDListCacheKey = "OPDList";
        //Department based cache keys-insert department id front
        private readonly string DepartmentCacheKey = "DepartmentOrgUnit";
        private readonly string DepartmentLocationCacheKey = "DepartmentLocationOrgUnit";
        private readonly string DepartmentSectionCacheKey = "DepartmentSectionOrgUnit";
        private readonly string DepartmentWardCacheKey = "DepartmentWardOrgUnit";
        private readonly string DepartmentOPDCacheKey = "DepartmentOPDOrgUnit";

        public OrgUnitsService(IWcfClient client, IWcfConfigurationProvider config, IMemoryCache cache)
        {
            m_wcfClient = client;
            m_configurationProvider = config;
            m_cache = cache;
        }

        public Department GetOrgUnitByDepartmentId(long departmentId, Guid SecurityToken)
        {
            if (!m_cache.TryGetValue(departmentId + DepartmentCacheKey, out Department cachedDepartment))
            {
                var department = new Department();
                var depUrl = m_configurationProvider.FullDepartmentServiceUrl;
                var departmentsResponse = m_wcfClient.CallServiceAndReturnResult<IDepartmentService, OrganizationalUnits.Interface.DTOs.DepartmentResponse>(t => t.GetDepartment(departmentId), depUrl, SecurityToken);
                if (departmentsResponse == null)
                {
                    m_log.Error($"Null returned for GetDepartmentId WCF call that used department id - {department.DepartmentId}");
                    throw new Exception("Failed to load organizational unit");
                }

                var departmentList = departmentsResponse.Departments;
                var departmentItem = departmentList?.FirstOrDefault();
                if (departmentList.Count == 0)
                {
                    m_log.Error($"No departments returned for the department id - {departmentId} , please validate this department id.");
                    throw new Exception("No units found for the department");
                }

                department.DepartmentId = departmentId;
                department.DepartmentName = departmentItem.DepartmentName;
                department.DepartmentUnitGid = departmentItem.Unitgid;

                var locationList = new List<LocationListItem>();
                m_log.Debug($"Number of locations returned for department id {departmentId} = {departmentItem.DepartmentLocation.Count}");
                locationList.AddRange(departmentItem.DepartmentLocation.Select(loc => new LocationListItem { LocationId = loc.LocalizationId, LocationDisplayName = loc.LocalizationName, UnitGid = loc.Unitgid }));
                department.LocationList = locationList;

                var sectionList = GetSectionListByDepartmentId(departmentId, SecurityToken);

                foreach (var section in sectionList)
                {
                    section.WardList = GetWardListBySectionId(section.SectionId, SecurityToken);
                }
                department.SectionList = sectionList;

                department.WardList = GetWardListByDepartmentId(departmentId, SecurityToken);
                department.OPDList = GetOPDListByDepartmentId(departmentId, SecurityToken);
                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(departmentId + DepartmentCacheKey, department, entryOptions);
                return department;
            }
            return cachedDepartment;
        }

        public List<DepartmentListItem> GetDepartmentListByHospitalId(long hospitalId, Guid SecurityToken)
        {
            if (!m_cache.TryGetValue(hospitalId + DepartmentListCacheKey, out List<DepartmentListItem> cachedDepList))
            {
                var asd = hospitalId + DepartmentListCacheKey;
                m_log.Info($"Trying to get departments Id list fot hospital ID - {hospitalId}");
                var m_url = m_configurationProvider.FullDepartmentServiceUrl;
                var departmentsResponse = m_wcfClient.CallServiceAndReturnResult<IDepartmentService, OrganizationalUnits.Interface.DTOs.DepartmentResponse>(t => t.GetDepartmentByHospitalId(hospitalId), m_url, SecurityToken);
                var departmentList = new List<DepartmentListItem>();
                if (departmentsResponse?.Departments.Count == 0)
                {
                    m_log.Info($"Zero departments returned for hospital ID - {hospitalId}");
                    return departmentList;
                }
                foreach (var dep in departmentsResponse.Departments)
                {
                    departmentList.Add(new DepartmentListItem { DepartmentId = dep.DepartmentId, DepartmentName = dep.DepartmentName, UnitGid = dep.Unitgid, DepartmentShortName = dep.DepartmentShortName });
                }
                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(hospitalId + DepartmentListCacheKey, departmentList, entryOptions);

                m_log.Info($"Recieved {departmentList.Count} Departments for hospital ID - {hospitalId}");
                return departmentList;
            }
            return cachedDepList;
        }

        public List<LocationListItem> GetLocationListByDepartmentId(long departmentId, Guid SecurityToken)
        {
            if (!m_cache.TryGetValue(departmentId + DepartmentLocationCacheKey, out List<LocationListItem> cachedLocationList))
            {
                var m_url = m_configurationProvider.FullDepartmentServiceUrl;
                var departmentsResponse = m_wcfClient.CallServiceAndReturnResult<IDepartmentService, OrganizationalUnits.Interface.DTOs.DepartmentResponse>(t => t.GetDepartment(departmentId), m_url, SecurityToken);
                var locationList = new List<LocationListItem>();
                var departmentLocation = departmentsResponse?.Departments?.FirstOrDefault();
                if (departmentLocation == null)
                {
                    m_log.Info($"Zero locations returned for department ID - {departmentId}");
                    return locationList;
                }

                if (departmentLocation.DepartmentLocation?.Count != 0)
                {
                    locationList.AddRange(departmentLocation.DepartmentLocation.Select(loc => new LocationListItem { LocationId = loc.LocalizationId, LocationDisplayName = loc.LocalizationName, UnitGid = loc.Unitgid }));
                    m_log.Info($"Recieved {locationList.Count} locations for department ID - {departmentId}");
                }
                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(departmentId + DepartmentLocationCacheKey, locationList, entryOptions);
                return locationList;
            }
            return cachedLocationList;
        }

        public List<OPDListItem> GetOPDListByDepartmentId(long departmentId, Guid SecurityToken)
        {
            if (!m_cache.TryGetValue(departmentId + DepartmentOPDCacheKey, out List<OPDListItem> cachedOPDList))
            {
                var location_url = m_configurationProvider.FullLocationServiceUrl;
                var locationResponse = m_wcfClient.CallServiceAndReturnResult<ILocationService, OrganizationalUnits.Interface.DTOs.LocationResponse>(t => t.GetOpdByDepartmentId(departmentId), location_url, SecurityToken);
                var opdList = new List<OPDListItem>();
                if (locationResponse?.Locations.Count == 0)
                {
                    m_log.Info($"Zero OPDs returned for department ID - {departmentId}");
                    return opdList;
                }
                opdList.AddRange(locationResponse.Locations.Select(opd => new OPDListItem { OPDDisplayName = opd.LocationName, OPDID = opd.LocationId, UnitGid = opd.Unitgid }));
                m_log.Info($"Recieved {opdList.Count} OPDs for department ID - {departmentId}");

                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(departmentId + DepartmentOPDCacheKey, opdList, entryOptions);
                return opdList;
            }
            return cachedOPDList;
        }

        public List<SectionListItem> GetSectionListByDepartmentId(long departmentId, Guid SecurityToken)
        {
            if (!m_cache.TryGetValue(departmentId + DepartmentSectionCacheKey, out List<SectionListItem> cachedSectionList))
            {
                var section_url = m_configurationProvider.FullSectionServiceUrl;
                var sectionResponse = m_wcfClient.CallServiceAndReturnResult<ISectionService, OrganizationalUnits.Interface.DTOs.SectionResponse>(t => t.GetSectionByDepartmentId(departmentId), section_url, SecurityToken);
                var sectionList = new List<SectionListItem>();
                if (sectionResponse?.Sections.Count == 0)
                {
                    m_log.Info($"Zero Sections returned for department ID - {departmentId}");
                    return sectionList;
                }
                sectionList.AddRange(sectionResponse.Sections.Select(section => new SectionListItem { SectionDisplayName = section.SectionName, SectionId = section.SectionId, UnitGid = section.Unitgid }));
                m_log.Info($"Recieved {sectionList.Count} Sections for department ID - {departmentId}");

                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(departmentId + DepartmentSectionCacheKey, sectionList, entryOptions);
                return sectionList;

            }
            return cachedSectionList;
        }

        public List<WardListItem> GetWardListByDepartmentId(long departmentId, Guid SecurityToken)
        {
            if (!m_cache.TryGetValue(departmentId + DepartmentWardCacheKey, out List<WardListItem> cachedWardList))
            {
                var ward_url = m_configurationProvider.FullWardServiceUrl;
                var wardResponse = m_wcfClient.CallServiceAndReturnResult<IWardService, OrganizationalUnits.Interface.DTOs.WardResponse>(t => t.GetWardByDepartmentId(departmentId), ward_url, SecurityToken);
                var wardList = new List<WardListItem>();
                if (wardResponse?.Wards.Count == 0)
                {
                    m_log.Info($"Zero Wards returned for department ID - {departmentId}");
                    return wardList;
                }
                wardList.AddRange(wardResponse.Wards.Select(ward => new WardListItem { WardDisplayName = ward.WardName, WardId = ward.WardId, UnitGid = ward.Unitgid }));
                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(departmentId + DepartmentWardCacheKey, wardList, entryOptions);

                m_log.Info($"Recieved {wardList.Count} wards for department ID - {departmentId}");
                return wardList;
            }
            return cachedWardList;
        }

        public List<WardListItem> GetWardListBySectionId(long sectionId, Guid SecurityToken)
        {
            var ward_url = m_configurationProvider.FullWardServiceUrl;
            var wardResponse = m_wcfClient.CallServiceAndReturnResult<IWardService, OrganizationalUnits.Interface.DTOs.WardResponse>(t => t.GetWardBySectionId(sectionId), ward_url, SecurityToken);
            var wardList = new List<WardListItem>();
            if (wardResponse?.Wards.Count == 0)
            {
                m_log.Info($"Zero Wards returned for section ID - {sectionId}");
                return wardList;
            }
            wardList.AddRange(wardResponse.Wards.Select(ward => new WardListItem { WardDisplayName = ward.WardName, WardId = ward.WardId, UnitGid = ward.Unitgid }));
            m_log.Info($"Recieved {wardList.Count} wards for department ID - {sectionId}");
            return wardList;
        }

        public DepartmentListItem GetDepartmentByDepartmentId(long departmentId, Guid SecurityToken)
        {
            m_log.Info($"Trying to get department for department ID - {departmentId}");
            var m_url = m_configurationProvider.FullDepartmentServiceUrl;
            var departmentsResponse = m_wcfClient.CallServiceAndReturnResult<IDepartmentService, OrganizationalUnits.Interface.DTOs.DepartmentResponse>(t => t.GetDepartment(departmentId), m_url, SecurityToken);
            if (departmentsResponse == null)
            {
                m_log.Error($"Null returned for GetDepartmentId WCF call that used department id - {departmentId}");
                throw new Exception("Failed to load department");
            }
            var departmentList = departmentsResponse.Departments;
            var departmentItem = departmentList?.FirstOrDefault();
            if (departmentList.Count == 0)
            {
                m_log.Info($"Zero departments returned for department ID - {departmentId}");
                throw new Exception("Failed to load department");
            }
            return new DepartmentListItem { DepartmentId = departmentItem.DepartmentId, DepartmentName = departmentItem.DepartmentName, DepartmentShortName = departmentItem.DepartmentShortName, UnitGid = departmentItem.Unitgid };
        }

        public List<DepartmentListItem> GetDepartmentListByDepartmentIdList(List<long?> departmentIdList, Guid SecurityToken)
        {
            var departmentList = new List<DepartmentListItem>();
            foreach (var department in departmentIdList)
            {
                if (department == null)
                    continue;
                departmentList.Add(GetDepartmentByDepartmentId((long)department, SecurityToken));
            }
            return departmentList;
        }

        public List<OPDListItem> GetOPDListByHospitalId(long hospitalId, Guid SecurityToken)
        {
            if (!m_cache.TryGetValue(hospitalId + OPDListCacheKey, out List<OPDListItem> cachedOPDs))
            {
                m_log.Info($"Trying to get OPD Id list fot hospital ID - {hospitalId}");
                var m_url = m_configurationProvider.FullLocationServiceUrl;
                var locationResponse = m_wcfClient.CallServiceAndReturnResult<ILocationService, OrganizationalUnits.Interface.DTOs.LocationResponse>(t => t.GetOpdByHospitalId(hospitalId), m_url, SecurityToken);
                var opdList = new List<OPDListItem>();
                if (locationResponse?.Locations.Count == 0)
                {
                    m_log.Info($"Zero OPDs returned for hospital ID - {hospitalId}");
                    return opdList;
                }
                foreach (var opd in locationResponse.Locations)
                {
                    opdList.Add(new OPDListItem { OPDID = opd.LocationId, OPDDisplayName = opd.LocationName, UnitGid = opd.Unitgid });
                }
                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(hospitalId + OPDListCacheKey, opdList, entryOptions);
                m_log.Info($"Recieved {opdList.Count} OPDs for hospital ID - {hospitalId}");
                return opdList;
            }
            return cachedOPDs;
        }

        public List<TreeNode> GetFullOrgUnitStructureTreeNodeList(long hospitalId, Guid securityToken)
        {
            if (!m_cache.TryGetValue(hospitalId, out List<TreeNode> org))
            {
                var orgUnit = GetFullOrgUnitStructure(hospitalId, securityToken);
                var tree = GenerateTreeNodesForFullOrgUnit(orgUnit);
                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(hospitalId, tree, entryOptions);
                return tree;
            }
            return org;
        }

        public List<TreeNode> GetOrgUnitStructureForDepartment(long hospitalId, long departmentId, Guid securityToken)
        {
            var department = GetOrgUnitByDepartmentId(departmentId, securityToken);
            var orgUnit = new OrganizationalUnit
            {
                Department = new List<Department> { department },
                HospitalId = hospitalId,
                IsHospitalLevel = false
            };
            return GenerateTreeNodesForFullOrgUnit(orgUnit);
        }

        public OrganizationalUnit GetFullOrgUnitStructure(long hospitalId, Guid securityToken)
        {
            if (!m_cache.TryGetValue(hospitalId.ToString() + OrgUnitCacheKey, out OrganizationalUnit org))
            {

                var departmentItemList = GetDepartmentListByHospitalId(hospitalId, securityToken);
                var newdepartmentList = (from department in departmentItemList
                                         let orgUnitForDepartment = GetOrgUnitByDepartmentId(department.DepartmentId, securityToken)
                                         select orgUnitForDepartment).ToList();
                var opdListForHospital = GetOPDListByHospitalId(hospitalId, securityToken);
                var orgUnit = new OrganizationalUnit
                {
                    Department = newdepartmentList,
                    OPDList = opdListForHospital,
                    HospitalId = hospitalId,
                    IsHospitalLevel = true
                };
                var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                m_cache.Set(hospitalId.ToString() + OrgUnitCacheKey, orgUnit, entryOptions);
                return orgUnit;
            }
            return org;
        }

        public String FindNameForOrgUnitId(long hospitalId, Guid securityToken, string orgUnitType, long orgUnitId)
        {
            var orgUnit = m_cache.Get<OrganizationalUnit>(hospitalId.ToString() + OrgUnitCacheKey);
            if (orgUnit == null)
            {
                orgUnit = GetFullOrgUnitStructure(hospitalId, securityToken);
            }

            if (orgUnit.OPDList.Count != 0)
            {
                if (orgUnitType == OrganizationalUnitType.OPD.ToString())
                {
                    foreach (var opd in orgUnit.OPDList.Where(opd => opd.OPDID == orgUnitId).Select(opd => opd))
                    {
                        return opd.OPDDisplayName;
                    }
                }
            }

            foreach (var dep in orgUnit.Department)
            {
                if (orgUnitType == OrganizationalUnitType.Department.ToString())
                {
                    if (dep.DepartmentId == orgUnitId)
                        return dep.DepartmentName;

                }
                if (orgUnitType == OrganizationalUnitType.Section.ToString())
                {
                    foreach (var section in dep.SectionList.Where(section => section.SectionId == orgUnitId).Select(section => section))
                    {
                        return section.SectionDisplayName;
                    }
                }

                if (orgUnitType == OrganizationalUnitType.Ward.ToString())
                {
                    foreach (var ward in dep.SectionList.SelectMany(section => section.WardList.Where(ward => ward.WardId == orgUnitId).Select(ward => ward)))
                    {
                        return ward.WardDisplayName;
                    }

                    foreach (var ward in dep.WardList.Where(ward => ward.WardId == orgUnitId).Select(ward => ward))
                    {
                        return ward.WardDisplayName;
                    }
                }

                if (orgUnitType == OrganizationalUnitType.OPD.ToString())
                {
                    foreach (var opd in dep.OPDList.Where(opd => opd.OPDID == orgUnitId).Select(opd => opd))
                    {
                        return opd.OPDDisplayName;
                    }
                }

                if (orgUnitType == OrganizationalUnitType.Location.ToString())
                {
                    foreach (var loc in dep.LocationList.Where(loc => loc.LocationId == orgUnitId).Select(loc => loc))
                    {
                        return loc.LocationDisplayName;
                    }
                }
            }
            return null;
        }

        #region private methods 
        private List<TreeNode> GenerateTreeNodesForFullOrgUnit(OrganizationalUnit orgUnit)
        {
            var hospitalDepartmentNodeId = "hosdep";
            var hospitalOPDNodeId = "hosopd";
            var DepartmentSectioNodeId = "depsec";
            var DepartmentWardNodeId = "depward";
            var DepartmentOPDNodeId = "depopd";

            var departmentList = new List<TreeNode>();
            foreach (var department in orgUnit.Department)
            {
                //section tree
                var sectionTreeList = new List<TreeNode>();
                foreach (var section in department.SectionList)
                {
                    var wardList = (from ward in section.WardList
                                    let wardTree = new TreeNode()
                                    {
                                        Id = ward.UnitGid,
                                        Title = ward.WardDisplayName,
                                        ParentNode = section.UnitGid
                                    }
                                    select wardTree).ToList();
                    var sectionTree = new TreeNode()
                    {
                        Id = section.UnitGid,
                        Title = section.SectionDisplayName,
                        ChildNodes = wardList,
                        ParentNode = DepartmentSectioNodeId + department.DepartmentUnitGid
                    };

                    sectionTreeList.Add(sectionTree);
                }

                //ward tree
                var wardTreeList = (from ward in department.WardList
                                    let wardTree = new TreeNode()
                                    {
                                        Id = ward.UnitGid,
                                        Title = ward.WardDisplayName,
                                        ParentNode = DepartmentWardNodeId + department.DepartmentUnitGid
                                    }
                                    select wardTree).ToList();

                //opd tree
                var opdTreeList = (from opd in department.OPDList
                                   let opdTree = new TreeNode()
                                   {
                                       Id = opd.UnitGid,
                                       Title = opd.OPDDisplayName,
                                       ParentNode = DepartmentOPDNodeId + department.DepartmentUnitGid
                                   }
                                   select opdTree).ToList();

                ////location tree
                //var locationTreeList = (from location in department.LocationList
                //                        let locationTree = new TreeNode()
                //                        {
                //                            Id = location.UnitGid,
                //                            Title = location.LocationDisplayName,
                //                            ParentNode = DepartmentLocationNodeId//department.DepartmentUnitGid
                //                        }
                //                        select locationTree).ToList();

                var departmentTree = new TreeNode()
                {
                    Id = department.DepartmentUnitGid,
                    Title = department.DepartmentName,
                    ChildNodes = new List<TreeNode>(),
                    ParentNode = hospitalDepartmentNodeId + orgUnit.HospitalId.ToString()
                };

                if (sectionTreeList.Count != 0)
                {
                    var sectionNode = new TreeNode
                    {
                        Id = DepartmentSectioNodeId + department.DepartmentUnitGid,
                        ChildNodes = sectionTreeList,
                        ParentNode = department.DepartmentUnitGid,
                        Title = "Sections"
                    };
                    departmentTree.ChildNodes.Add(sectionNode);
                }

                if (wardTreeList.Count != 0)
                {
                    var wardNode = new TreeNode
                    {
                        Id = DepartmentWardNodeId + department.DepartmentUnitGid,
                        ChildNodes = wardTreeList,
                        ParentNode = department.DepartmentUnitGid,
                        Title = "Wards"
                    };
                    departmentTree.ChildNodes.Add(wardNode);
                }

                if (opdTreeList.Count != 0)
                {
                    var opdNode = new TreeNode
                    {
                        Id = DepartmentOPDNodeId + department.DepartmentUnitGid,
                        ChildNodes = opdTreeList,
                        ParentNode = department.DepartmentUnitGid,
                        Title = "OPDs"
                    };
                    departmentTree.ChildNodes.Add(opdNode);
                }

                //var locationNode = new TreeNode
                //{
                //    Id = DepartmentLocationNodeId,
                //    ChildNodes = locationTreeList,
                //    ParentNode = department.DepartmentUnitGid
                //};
                departmentList.Add(departmentTree);

            }

            if (!orgUnit.IsHospitalLevel)
            {
                return departmentList;
            }

            var departmentNodeForHospital = new TreeNode
            {
                Id = hospitalDepartmentNodeId + orgUnit.HospitalId.ToString(),
                ChildNodes = departmentList,
                ParentNode = null, //orgUnit.HospitalId.ToString(),
                Title = "Departments"
            };

            var opdList = (from opd in orgUnit.OPDList
                           let opdTree = new TreeNode()
                           {
                               Id = opd.UnitGid,
                               Title = opd.OPDDisplayName,
                               ParentNode = hospitalOPDNodeId + orgUnit.HospitalId.ToString()
                           }
                           select opdTree).ToList();

            var opdNodeForHospital = new TreeNode
            {
                Id = hospitalOPDNodeId + orgUnit.HospitalId.ToString(),
                ChildNodes = opdList,
                ParentNode = null, //orgUnit.HospitalId.ToString(),
                Title = "OPDs"
            };

            return new List<TreeNode> { departmentNodeForHospital, opdNodeForHospital };
        }

        #endregion
    }
}
