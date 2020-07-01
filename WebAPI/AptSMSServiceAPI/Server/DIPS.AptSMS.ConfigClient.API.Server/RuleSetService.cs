using DIPS.Infrastructure.Logging;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.API.Interface;
using System;
using System.Collections.Generic;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System.Linq;
using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;


namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class RuleSetService : IRuleSetService
    {
        private readonly IRuleSetDataService m_ruleSetDataService;
        private readonly IOrgUnitsService m_orgService;
        private static ILog s_log = LogProvider.GetLogger(typeof(RuleSetService));

        public RuleSetService(IRuleSetDataService ruleSetDataService, IOrgUnitsService orgService)
        {
            m_ruleSetDataService = ruleSetDataService;
            m_orgService = orgService;
        }

        public Guid SaveRuleSet(RuleSet ruleSet)
        {
            var ruleSetDto = MapModalToDto(ruleSet);
            return m_ruleSetDataService.SaveRuleSet(ruleSetDto);
        }
        public List<RuleSet> SearchRuleSet(long? departmentid, string searchterm, bool getInactive, bool getHospitalLevel, long hospitalId)
        {
            bool getActive = !getInactive;
            if (IsGuid(searchterm))
            {
                Guid ruleSetId = Guid.Parse(searchterm);
                var ruleSetDtoList = m_ruleSetDataService.SearchRuleSets(ruleSetId, departmentid, null, getActive, getHospitalLevel, hospitalId);
                var ruleSetList = MapDTOListToModelList(ruleSetDtoList);
                return ruleSetList;

            }
            else
            {
                var ruleSetDtoList = m_ruleSetDataService.SearchRuleSets(null, departmentid, searchterm, getActive, getHospitalLevel, hospitalId);
                var ruleSetList = MapDTOListToModelList(ruleSetDtoList);
                return ruleSetList;
            }
        }

        public List<RuleSetListItem> GetAllActiveRuleSets(long hospitalID)
        {
            var ruleSetDTOs = m_ruleSetDataService.GetAllActiveRuleSets(hospitalID);
            var ruleSetVMs = ruleSetDTOs.Select(r => MapDTOtoViewModel(r)).ToList();

            return ruleSetVMs;
        }

        public RuleSet GetRuleSetById(Guid RuleSetId)
        {
            return MapDTOtoModel(m_ruleSetDataService.GetRuleSetBy(RuleSetId));
        }

        public List<OPDListItem> GetOPDListForRuleSetId(long? departmentId, long hospitalId, Guid ruleSetId, Guid SecurityToken)
        {
            if (ruleSetId == null || ruleSetId == Guid.Empty)
            {
                throw new UserInputValidationException("RuleSetId cant't be null or empty");
            }
            if (departmentId == 0)
            {
                throw new UserInputValidationException("Departmentid is not valid.0 is not a valid department id.");
            }

            List<OPDListItem> opdList = departmentId != null
               ? m_orgService.GetOPDListByDepartmentId((long)departmentId, SecurityToken)
               : m_orgService.GetOPDListByHospitalId((long)hospitalId, SecurityToken);

            var ruleSet = GetRuleSetById((Guid)ruleSetId);
            if (ruleSet.ExcludeOrgUnits.Count == 0)
            {
                return opdList;
            }

            var newOPDList = new List<OPDListItem>();
            foreach (var opd in opdList)
            {
                bool isAvailable = false;
                foreach (var _ in ruleSet.ExcludeOrgUnits.Where(unit => opd.UnitGid == unit.UnitID).Select(unit => new { }))
                {
                    isAvailable = true;
                }
                if (!isAvailable)
                {
                    newOPDList.Add(opd);
                }
            }
            return newOPDList;
        }

        public List<SectionListItem> GetSectionListForRuleSetId(long departmentId, Guid ruleSetId, Guid SecurityToken)
        {
            if (ruleSetId == null || ruleSetId == Guid.Empty)
            {
                throw new UserInputValidationException("RuleSetId cant't be null or empty");
            }
            if (departmentId == 0)
            {
                throw new UserInputValidationException("Departmentid is not valid.0 is not a valid department id.");
            }
            var sections = m_orgService.GetSectionListByDepartmentId(departmentId, SecurityToken);

            var ruleSet = GetRuleSetById(ruleSetId);
            if (ruleSet.ExcludeOrgUnits?.Count == 0)
            {
                return sections;
            }
            var newSectionList = new List<SectionListItem>();
            foreach (var section in sections)
            {
                bool isAvailable = false;
                foreach (var _ in ruleSet.ExcludeOrgUnits.Where(unit => section.UnitGid == unit.UnitID).Select(unit => new { }))
                {
                    isAvailable = true;
                }
                if (!isAvailable)
                {
                    newSectionList.Add(section);
                }
            }
            return newSectionList;
        }

        public List<WardListItem> GetWardListForRuleSetId(long? departmentId, long? sectionId, Guid ruleSetId, Guid SecurityToken)
        {
            if (departmentId == null && sectionId == null)
            {
                throw new UserInputValidationException("Both department id and section id can't be null at same time.");
            }
            if (ruleSetId == null || ruleSetId == Guid.Empty)
            {
                throw new UserInputValidationException("RuleSetId cant't be null or empty");
            }
            if (departmentId == 0)
            {
                throw new UserInputValidationException("Departmentid is not valid.0 is not a valid department id.");
            }

            List<WardListItem> wardList = sectionId != null
                ? m_orgService.GetWardListBySectionId((long)sectionId, SecurityToken)
                : m_orgService.GetWardListByDepartmentId((long)departmentId, SecurityToken);

            var ruleSet = GetRuleSetById(ruleSetId);
            if (ruleSet.ExcludeOrgUnits?.Count == 0)
            {
                return wardList;
            }
            var newWardList = new List<WardListItem>();
            foreach (var ward in wardList)
            {
                bool isAvailable = false;
                foreach (var _ in ruleSet.ExcludeOrgUnits.Where(unit => ward.UnitGid == unit.UnitID).Select(unit => new { }))
                {
                    isAvailable = true;
                }
                if (!isAvailable)
                {
                    newWardList.Add(ward);
                }
            }
            return newWardList;
        }


        public List<LocationListItem> GetLocationListForRuleSetId(long departmentId, Guid ruleSetId, Guid SecurityToken)
        {
            if (ruleSetId == null || ruleSetId == Guid.Empty)
                throw new UserInputValidationException("RuleSetId cant't be null or empty");

            if (departmentId == 0)
                throw new UserInputValidationException("Department id is not valid.0 is not a valid department id.");

            var locationList = m_orgService.GetLocationListByDepartmentId(departmentId, SecurityToken);
            var ruleSet = GetRuleSetById(ruleSetId);
            if (ruleSet.ExcludeOrgUnits?.Count == 0)
            {
                return locationList;
            }
            var newlocationList = new List<LocationListItem>();
            foreach (var location in locationList)
            {
                bool isAvailable = false;
                foreach (var _ in ruleSet.ExcludeOrgUnits.Where(unit => location.UnitGid == unit.UnitID).Select(unit => new { }))
                {
                    isAvailable = true;
                }
                if (!isAvailable)
                {
                    newlocationList.Add(location);
                }
            }
            return newlocationList;
        }

        public List<TreeNode> GetRuleSetTreeNodes(List<RuleSet> ruleSetList)
        {
            return ruleSetList.Select(ruleSet =>
               new TreeNode()
               {
                   Title = ruleSet.RulesetName,
                   Id = ruleSet.RulesetId.ToString(),
                   ChildNodes = new List<TreeNode>()
               })
               .ToList();
        }

        public List<DepartmentListItem> GetDepartmentListForRuleSetId(long hospitalId, Guid ruleSetId, Guid SecurityToken)
        {
            if (ruleSetId == null || ruleSetId == Guid.Empty)
                throw new UserInputValidationException("RuleSetId cant't be null or empty");


            var departmentList = m_orgService.GetDepartmentListByHospitalId(hospitalId, SecurityToken);
            var ruleSet = GetRuleSetById(ruleSetId);
            if (ruleSet.ExcludeOrgUnits?.Count == 0)
            {
                return departmentList;
            }
            var newDepartmentList = new List<DepartmentListItem>();
            foreach (var dep in departmentList)
            {
                bool isAvailable = false;
                foreach (var _ in ruleSet.ExcludeOrgUnits.Where(unit => dep.UnitGid == unit.UnitID).Select(unit => new { }))
                {
                    isAvailable = true;
                }
                if (!isAvailable)
                {
                    newDepartmentList.Add(dep);
                }
            }
            return newDepartmentList;
        }


        public bool IsDepartmentExcluded(long DepartmentId, Guid ruleSetId, Guid SecurityToken)
        {
            var ruleSet = GetRuleSetById(ruleSetId);
            if (ruleSet.ExcludeOrgUnits?.Count == 0)
            {
                return false;
            }
            var department = m_orgService.GetDepartmentByDepartmentId(DepartmentId, SecurityToken);
            foreach (var _ in ruleSet.ExcludeOrgUnits.Where(excludedId => excludedId.UnitID == department.UnitGid).Select(excludedId => new { }))
            {
                return true;
            }

            return false;
        }

        #region Private Functions        

        private RuleSetListItem MapDTOtoViewModel(RuleSetDTO dto)
        {

            return new RuleSetListItem()
            {
                RulesetId = (Guid)dto.RuleSetGUID,
                RulesetName = dto.Name,
                IsActive = dto.IsActive,
                DepartmentId = dto.DepartmentID,
                HospitalId = dto.HospitalID
            };
        }

        private RuleSetDTO MapModalToDto(RuleSet ruleSet)
        {
            try
            {
                RuleSetDTO dto = new RuleSetDTO();
                if (ruleSet.ExcludeOrgUnits != null)
                {
                    var excludeIDs = (from orgUnits in ruleSet.ExcludeOrgUnits
                                      select orgUnits.UnitID).ToList();
                    dto.ExcludingOrgUnitIDs = excludeIDs;
                }
                dto.ValidFrom = ruleSet.ScheduleValidityPeriodFrom;
                dto.ValidTo = ruleSet.ScheduleValidityPeriodTo;
                dto.IsActive = ruleSet.IsActive;
                dto.SendSMSBeforeInMins = ruleSet.MinimumTimeFromMinutes;
                dto.SendSMSBeforeDays = ruleSet.SendBeforeInDays;
                dto.DaysForRetryExpiry = ruleSet.ExpireInDays;
                dto.SendingTimeWindowFrom = ruleSet.SendingTimeIntervalFrom;
                dto.SendingTimeWindowTo = ruleSet.SendingTimeIntervalTo;
                dto.IgnoreSMStoAdmittedPatient = ruleSet.IgnoreSMStoAdmittedPatient;
                dto.AptValidate_To = ruleSet.AppointmentTo;
                dto.AptValidate_From = ruleSet.AppointmentFrom;
                dto.DepartmentID = ruleSet.DepartmentId;
                dto.HospitalID = ruleSet.HospitalId;
                dto.Name = ruleSet.RulesetName;
                dto.RuleSetGUID = ruleSet.RulesetId;
                dto.isValidateAptTime = ruleSet.IsAppointmentTimeValidate;
                return dto;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Map Rule Set modal to Dto failed ", ex);
                throw;
            }
        }

        private bool IsGuid(string value)
        {
            Guid ruleSetGuid;
            return Guid.TryParse(value, out ruleSetGuid);
        }

        private List<RuleSet> MapDTOListToModelList(List<RuleSetDTO> dtoList)
        {
            try
            {
                List<RuleSet> ruleSetList = new List<RuleSet>();
                List<OrgUnit> orgUnitList = new List<OrgUnit>();
                foreach (var ruleSetDto in dtoList)
                {
                    RuleSet modle = new RuleSet();
                    modle.IsActive = ruleSetDto.IsActive;
                    modle.SendBeforeInDays = ruleSetDto.SendSMSBeforeDays;
                    modle.ExpireInDays = ruleSetDto.DaysForRetryExpiry;
                    modle.SendingTimeIntervalFrom = ruleSetDto.SendingTimeWindowFrom;
                    modle.SendingTimeIntervalTo = ruleSetDto.SendingTimeWindowTo;
                    modle.IgnoreSMStoAdmittedPatient = ruleSetDto.IgnoreSMStoAdmittedPatient;
                    modle.AppointmentTo = ruleSetDto.AptValidate_To;
                    modle.AppointmentFrom = ruleSetDto.AptValidate_From;
                    modle.DepartmentId = ruleSetDto.DepartmentID;
                    modle.HospitalId = ruleSetDto.HospitalID;
                    modle.RulesetName = ruleSetDto.Name;
                    modle.RulesetId = ruleSetDto.RuleSetGUID.Value;
                    modle.ScheduleValidityPeriodFrom = ruleSetDto.ValidFrom;
                    modle.ScheduleValidityPeriodTo = ruleSetDto.ValidTo;
                    modle.MinimumTimeFromMinutes = ruleSetDto.SendSMSBeforeInMins;
                    modle.IsAppointmentTimeValidate = ruleSetDto.isValidateAptTime;
                    if (ruleSetDto.ExcludingOrgUnitIDs != null)
                    {
                        foreach (var id in ruleSetDto.ExcludingOrgUnitIDs)
                        {
                            OrgUnit orgUnit = new OrgUnit();
                            orgUnit.UnitID = id;
                            orgUnitList.Add(orgUnit);
                        }
                        modle.ExcludeOrgUnits = orgUnitList;
                    }
                    ruleSetList.Add(modle);
                }

                return ruleSetList;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Converting RuleSet Dto to Modle is failed", ex);
                throw;
            }
        }

        private RuleSet MapDTOtoModel(RuleSetDTO ruleSetDto)
        {
            List<OrgUnit> orgUnitList = new List<OrgUnit>();
            RuleSet modle = new RuleSet();
            modle.IsActive = ruleSetDto.IsActive;
            modle.SendBeforeInDays = ruleSetDto.SendSMSBeforeDays;
            modle.ExpireInDays = ruleSetDto.DaysForRetryExpiry;
            modle.SendingTimeIntervalFrom = ruleSetDto.SendingTimeWindowFrom;
            modle.SendingTimeIntervalTo = ruleSetDto.SendingTimeWindowTo;
            modle.IgnoreSMStoAdmittedPatient = ruleSetDto.IgnoreSMStoAdmittedPatient;
            modle.AppointmentTo = ruleSetDto.AptValidate_To;
            modle.AppointmentFrom = ruleSetDto.AptValidate_From;
            modle.DepartmentId = ruleSetDto.DepartmentID;
            modle.HospitalId = ruleSetDto.HospitalID;
            modle.RulesetName = ruleSetDto.Name;
            modle.RulesetId = ruleSetDto.RuleSetGUID.Value;
            modle.ScheduleValidityPeriodFrom = ruleSetDto.ValidFrom;
            modle.ScheduleValidityPeriodTo = ruleSetDto.ValidTo;
            modle.MinimumTimeFromMinutes = ruleSetDto.SendSMSBeforeInMins;
            modle.IsAppointmentTimeValidate = ruleSetDto.isValidateAptTime;
            foreach (var id in ruleSetDto.ExcludingOrgUnitIDs)
            {
                OrgUnit orgUnit = new OrgUnit();
                orgUnit.UnitID = id;
                orgUnitList.Add(orgUnit);
            }
            modle.ExcludeOrgUnits = orgUnitList;
            return modle;
        }

        #endregion
    }
}
