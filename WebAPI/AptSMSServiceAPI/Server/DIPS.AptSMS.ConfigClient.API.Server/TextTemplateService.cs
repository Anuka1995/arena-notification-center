using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.Infrastructure.Logging;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using TagData = System.Collections.Generic.KeyValuePair<string, string>;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.Common.FormatFunction;
using DIPS.AptSMS.ConfigClient.Common.TextTemplateService;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class TextTemplateService : ITextTemplateService
    {
        private static ILog s_log = LogProvider.GetLogger(typeof(TextTemplateService));
        private readonly ITextTemplateDataService m_TextTemplateDataService;
        private readonly IGroupedTextService m_GroupedTextService;
        private readonly ITagService m_TagService;
        private readonly IOrgUnitsService m_orgUnitsService;
        private readonly IPSSLinkService m_PSSLinkDataService;
        private readonly string tagStart = @"\[\$\$";
        private readonly string tagEnd = @"\$\$\]";
        private readonly string tagStartdStr = "[$$";
        private readonly string tagEndStr = "$$]";
        XmlDocument xdoc;
        bool isSampalXpathAvailable = false;
        private IConfiguration m_config;


        public TextTemplateService(ITextTemplateDataService textTemplateDataService, ITagService tagService, IConfiguration config, IGroupedTextService GroupedTextService, IOrgUnitsService orgUnitService, IPSSLinkService PSSLinkDataService)
        {
            m_TextTemplateDataService = textTemplateDataService;
            m_TagService = tagService;
            m_config = config;
            m_GroupedTextService = GroupedTextService;
            m_orgUnitsService = orgUnitService;
            m_PSSLinkDataService = PSSLinkDataService;
        }

        public string GetPreview(string templateTextStr, bool isPathPSSRequred, long hospitalId)
        {
            LoadSampleXml();
            var tagList = m_TagService.GetAllTags(hospitalId);
            var pssLinkObject = m_PSSLinkDataService.GetPSSLinkByHospital(hospitalId).First();
            return TextTemplateCreate.GetCompleteSmsText(xdoc, templateTextStr, tagList, pssLinkObject.Link, isPathPSSRequred);
        }

        public List<TagData> GetTagsList(string templateStr)
        {
            if (templateStr == null || templateStr.Length < 5)
                return null;
            List<TagData> tagDictionary = new List<TagData>();
            foreach (string value in Regex.Split(templateStr, tagStart))
            {
                if (value.Contains(tagEndStr))
                {
                    var tagPredict = Regex.Split(value, tagEnd);
                    if (tagPredict != null && tagPredict.Length > 1)
                    {
                        if (tagPredict[0].Trim().Length > 0)
                        {
                            var tagValue = GetTagWithFunction(tagPredict[0].Trim());
                            tagDictionary.Add(new TagData(tagValue.Key, tagValue.Value));
                        }
                    }
                }
            }
            return tagDictionary;
        }

        public KeyValuePair<string, string> GetTagWithFunction(string value)
        {
            if (value != null && value.Length > 1)
            {
                if (value.Contains(":"))
                {
                    var tagPredict = Regex.Split(value, ":");
                    if (tagPredict[0].Trim().Length > 0)
                    {
                        if (tagPredict.Length > 2)
                        {
                            var replaceValue = tagPredict[0].Trim() + ":";
                            return new KeyValuePair<string, string>(tagPredict[0].Trim(), value.Replace(replaceValue, ""));
                        }
                        return new KeyValuePair<string, string>(tagPredict[0].Trim(), tagPredict[1].Trim());
                    }
                }
            }
            return new KeyValuePair<string, string>(value, "");
        }


        public IDictionary<string, string> GetTagValues(List<TagData> tagValueDictonary, long hospitalId)
        {
            IDictionary<string, string> resulyDictonary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in tagValueDictonary)
            {
                if (entry.Key.Length > 0)
                {
                    var tagName = entry.Key;
                    var tagObject = m_TagService.GetTagBy(tagName, hospitalId);
                    if (tagObject != null)
                    {
                        if (tagObject.TagType == ConfigClient.Common.Models.TagType.XPATH)
                        {
                            var value = GetXpathValue(tagObject.TagValue);
                            if (!resulyDictonary.ContainsKey(entry.Key))
                                resulyDictonary.Add(entry.Key, value);
                        }
                        else
                        {
                            if (!resulyDictonary.ContainsKey(entry.Key))
                                resulyDictonary.Add(entry.Key, tagObject.TagValue);
                        }
                    }
                }
            }
            return resulyDictonary;
        }

        public string GetXpathValue(string xPath)
        {
            if (isSampalXpathAvailable)
            {
                try
                {
                    XmlNode dataAttribute = xdoc.SelectSingleNode(xPath);
                    return dataAttribute.InnerText.Trim();
                }
                catch (Exception ex)
                {
                    var errormessage = "Failed to load GetXpathValue for tag Xpath " + xPath;
                    s_log.Error(errormessage, ex);
                    return "";
                }
            }
            return "";
        }

        public Guid SaveSMSTextTemplate(SMSText textTemplate)
        {
            return m_TextTemplateDataService.SaveTextTemplate(MapModelToDataDTO(textTemplate));
        }

        public List<SMSText> SearchSMSTextTemplate(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly, long hospitalId)
        {
            var smsTextDtoList = m_TextTemplateDataService.SearchTextTemplate(departmetnID, opdID, sectionID, wardID, searchTerm, isActive, isHospitalOnly, hospitalId);
            return (smsTextDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
        }

        public SMSText GetSMSTextTemplateById(Guid SMSTextId)
        {
            var smsTextDto = m_TextTemplateDataService.GetTextTemplateById(SMSTextId);
            if (smsTextDto != null)
            {
                var smsText = MapDataDtoToModel(smsTextDto);
                return smsText;
            }
            s_log.Info($"SMS text template is not exited for SMSTextId - {SMSTextId}");
            throw new UserInputValidationException($"SMSTextId is not a valid id,No entry exist for this id - {SMSTextId}");
        }

        public List<TreeNode> GetSMSTextTemplateTreeNodes(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly, long hospitalId)
        {
            var smsTextDtoList = m_TextTemplateDataService.SearchTextTemplate(departmetnID, opdID, sectionID, wardID, searchTerm, isActive, isHospitalOnly, hospitalId);
            var smsTextList = (smsTextDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            var ruleSetTreeNodeList = new List<TreeNode>();

            foreach (var smsTextObj in smsTextList)
            {
                var isRuleSetExit = ruleSetTreeNodeList.Any(c => c.Id == smsTextObj.RulesetId.ToString());
                if (!isRuleSetExit)
                {
                    var ruleSetTreeNode = new TreeNode
                    {
                        Id = smsTextObj.RulesetId.ToString(),
                        Title = smsTextObj.RuleSetName,
                        ChildNodes = new List<TreeNode>(),
                        Type ="schedule"
                       
                    };
                    var smsTextTree = new TreeNode
                    {
                        Id = smsTextObj.TextTemplateId.ToString(),
                        Title = smsTextObj.TextTemplateName,
                        ChildNodes = new List<TreeNode>(),
                        Tag = new { smsTextObj.IsGenerateSMS},
                        Type="template"

                    };
                    ruleSetTreeNode.ChildNodes.Add(smsTextTree);
                    ruleSetTreeNodeList.Add(ruleSetTreeNode);
                }
                else
                {
                    var smsTextTree = new TreeNode
                    {
                        Id = smsTextObj.TextTemplateId.ToString(),
                        Title = smsTextObj.TextTemplateName,
                        ChildNodes = new List<TreeNode>(),
                        Tag = new { smsTextObj.IsGenerateSMS},
                        Type = "template"
                    };
                    var selectedRuleSetTree = ruleSetTreeNodeList.Find(c => c.Id == smsTextObj.RulesetId.ToString());
                    selectedRuleSetTree.ChildNodes.Add(smsTextTree);
                }
            }
            return ruleSetTreeNodeList;
        }
        public List<SMSText> GetEnhancedSearchByWard(Guid scheduleId, long departmentID, long wardId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            var texttemplatExact = GetTextTemplatByWard(scheduleId, departmentID, wardId, hospitalId, locationId, contactTypes, officialLevelofcare);
            if (texttemplatExact.Count > 0) { return texttemplatExact; }

            else
            {
                //one level back tracked and search --> search on DepartmentLevel
                var textTemplateListByDept = GetTextTemplatByDepartment(scheduleId, departmentID, hospitalId, locationId, contactTypes, officialLevelofcare);
                if (textTemplateListByDept.Count > 0)
                {
                    return textTemplateListByDept;
                }
                else
                {

                    return GetTextTemplateByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
                }



            }
        }
        public List<SMSText> GetEnhancedSearchByOPDId(Guid scheduleId, long departmentID, long opdId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            var texttemplatExact = GetTextTemplatByOPD(scheduleId, departmentID, opdId, hospitalId, locationId, contactTypes, officialLevelofcare);
            if (texttemplatExact.Count > 0) { return texttemplatExact; }

            else
            {
                var textTemplateListByDept = GetTextTemplatByDepartment(scheduleId, departmentID, hospitalId, locationId, contactTypes, officialLevelofcare);
                if (textTemplateListByDept.Count > 0)
                {
                    return textTemplateListByDept;
                }
                else
                {
                    //check on hospital level
                    return GetTextTemplateByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
                }

            }
        }
        public List<SMSText> GetEnhancedSearchByWard_BySection(Guid scheduleId, long depId, long wardId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            var texttemplatExact = GetTextTemplatByWard_BySections(scheduleId, depId, wardId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
            if (texttemplatExact.Count > 0)
            {
                return texttemplatExact;
            }
            else
            {
                var texttemplateBySection = GetTextTemplatBySection(scheduleId, depId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
                if (texttemplateBySection.Count > 0)
                {
                    return texttemplateBySection;
                }
                else
                {
                    var textTemplateListByDept = GetTextTemplatByDepartment(scheduleId, depId, hospitalId, locationId, contactTypes, officialLevelofcare);
                    if (textTemplateListByDept.Count > 0)
                    {
                        return textTemplateListByDept;
                    }
                    else
                    {
                        //check on hospital level
                        return GetTextTemplateByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
                    }
                }
            }

        }

        public List<SMSText> GetEnhancedSearchBySection(Guid scheduleId, long depId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            var texttemplatExact = GetTextTemplatBySection(scheduleId, depId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
            if (texttemplatExact.Count > 0)
            {
                return texttemplatExact;
            }
            else
            {
                var textTemplateListByDept = GetTextTemplatByDepartment(scheduleId, depId, hospitalId, locationId, contactTypes, officialLevelofcare);
                if (textTemplateListByDept.Count > 0)
                {
                    return textTemplateListByDept;
                }
                else
                {
                    //check on hospital level
                    return GetTextTemplateByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
                }


            }
        }
        public List<SMSText> GetEnhancedSearchByDep(Guid scheduleId, long depId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            var texttemplatExact = GetTextTemplatByDep(scheduleId, depId, hospitalId, locationId, contactTypes, officialLevelofcare);
            if (texttemplatExact.Count > 0)
            {
                return texttemplatExact;
            }
            else
            {
                //can expand to  get by HospitalId if needed.
                return GetTextTemplateByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
            }
        }
        public List<SMSText> GetEnhancedSearchOnHospitalLevel(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            var texttemplatExact = GetTextTemplateByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
            return texttemplatExact;
        }
        public List<SMSText> GetEnhancedSearchOnHospitalLevel_OPD(Guid scheduleId, long hospitalId, long opdId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            //Seach on Hospital + OPD
            var texttemplatDtoList = m_TextTemplateDataService.GetGetTextTemplateByHospitalLevel_OPD(scheduleId, hospitalId, opdId, locationId, contactTypes, officialLevelofcare);
            var smsTextList = (texttemplatDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            if (smsTextList.Count > 0)
            {
                return smsTextList;
            }
            else
            {
                //seacrh on Hospital Level
                var textTemplateByHospital = GetTextTemplateByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
                return textTemplateByHospital;
            }
        }

        public List<SMSText> GetFullSMSTextTemplatesFor(bool isActive, long hospitalId)
        {
            var smsTextDtoList = m_TextTemplateDataService.GetTemplatesOverviewBy(isActive, hospitalId);
            return (smsTextDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
        }

        #region private
        private SMSText MapDataDtoToModel(TextTemplateDTO dto)
        {
            var smsText = new SMSText
            {
                TextTemplateId = dto.TemplateGUID,
                DepartmentId = dto.DepartmentID,
                TemplateDepartmentId = dto.TemplateDepartmentID,
                HospitalId = dto.HospitalID,
                SectionId = dto.SectionID,
                WardId = dto.WardID,
                OPDId = dto.OPDID,
                LocationId = dto.LocationID,
                isPSSLinkInclude = dto.AttachPSSLink,
                IsVideoAppoinment = dto.IsVideoAppoinment,
                IsGenerateSMS = dto.IsGenerateSMS,
                isActive = dto.IsActive,
                TextTemplateName = dto.Name,
                ValidTo = dto.ValidTo,
                ValidFrom = dto.ValidFrom,
                OfficialLevelOfCare = dto.OfficialLevelOfCare,
                ContactType = dto.ContactType,
                RulesetId = (dto.RuleSetGUID == null || dto.RuleSetGUID == Guid.Empty) ? null : dto.RuleSetGUID,
                RuleSetName = dto.RuleSetName,
                SendSMSBeforeDays = dto.SendSMSBeforeDays,
                EcludedOrgIds = dto.ExcludedOrgUnits,
                SMSTextTemplate = dto.SMSText
            };

            if (dto.GroupedTextGUID != null && dto.GroupedTextGUID != Guid.Empty)
                smsText.GroupTemplateId = dto.GroupedTextGUID;

            return smsText;
        }

        private TextTemplateDTO MapModelToDataDTO(SMSText textTemplate)
        {
            var textTemplateDataDto = new TextTemplateDTO()
            {
                IsActive = textTemplate.isActive,
                DepartmentID = textTemplate.DepartmentId,
                SectionID = textTemplate.SectionId,
                WardID = textTemplate.WardId,
                OPDID = textTemplate.OPDId,
                LocationID = textTemplate.LocationId,
                AttachPSSLink = textTemplate.isPSSLinkInclude,
                IsVideoAppoinment = textTemplate.IsVideoAppoinment,
                HospitalID = textTemplate.HospitalId,
                Name = textTemplate.TextTemplateName,
                ValidTo = textTemplate.ValidTo,
                ValidFrom = textTemplate.ValidFrom,
                OfficialLevelOfCare = textTemplate.OfficialLevelOfCare,
                ContactType = textTemplate.ContactType,
                TemplateGUID = (textTemplate.TextTemplateId == null || textTemplate.TextTemplateId == Guid.Empty) ? null : textTemplate.TextTemplateId,
                RuleSetGUID = (textTemplate.RulesetId == null || textTemplate.RulesetId == Guid.Empty) ? null : textTemplate.RulesetId
            };

            if (textTemplate.GroupTemplateId != null && textTemplate.GroupTemplateId != Guid.Empty)
                textTemplateDataDto.GroupedTextGUID = textTemplate.GroupTemplateId;

            if (!string.IsNullOrEmpty(textTemplate.SMSTextTemplate))
                textTemplateDataDto.SMSText = textTemplate.SMSTextTemplate;

            return textTemplateDataDto;
        }

        private void LoadSampleXml()
        {
            var sampleFilePath = "";
            try
            {
                xdoc = new XmlDocument();
                sampleFilePath = m_config.GetValue<string>("SampleXMLfilePath");
                var owners = System.IO.File.ReadAllText(sampleFilePath);
                xdoc.LoadXml(owners);
                isSampalXpathAvailable = true;
            }
            catch (Exception ex)
            {
                var errormessage = "Failed to load SampleXml from " + sampleFilePath;
                s_log.Error(errormessage, ex);
                isSampalXpathAvailable = false;
            }
        }

        public string ApplyFunction(string value, string functionCode)
        {
            return TagFormater.GetFormatString(functionCode, value);
        }

        public List<SMSText> GetTextTemplatByWard(Guid scheduleId, long? depId, long? wardId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            var textTemplateDtoList = m_TextTemplateDataService.GetTextTemplatByWard(scheduleId, depId, wardId, hospitalId, null, null, null);
            var smsTextList = (textTemplateDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            return smsTextList;
        }
        private List<SMSText> GetTextTemplatByOPD(Guid scheduleId, long? depId, long? opdId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            var textTemplateDtoList = m_TextTemplateDataService.GetTextTemplatByOPD(scheduleId, depId, opdId, hospitalId, null, null, null);
            var smsTextList = (textTemplateDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            return smsTextList;
        }


        private List<SectionListItem> GetSectionIdByDepartment(long? departmentId, Guid securityToken)
        {

            var orgUnits = m_orgUnitsService.GetSectionListByDepartmentId(departmentId.Value, securityToken);
            return orgUnits;

        }
        private List<SMSText> GetTextTemplatBySection(Guid scheduleId, long depId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            var textTemplateDtoList = m_TextTemplateDataService.GetTextTemplateBySection(scheduleId, depId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
            var smsTextList = (textTemplateDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            return smsTextList;
        }
        private List<SMSText> GetTextTemplatByDepartment(Guid scheduleId, long departmentID, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            var textTemplateDtoList = m_TextTemplateDataService.GetTextTemplatByDepartment(scheduleId, departmentID, hospitalId, locationId, contactTypes, officialLevelofcare);
            var smsTextList = (textTemplateDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            return smsTextList;
        }

        private List<SMSText> GetTextTemplatByWard_BySections(Guid scheduleId, long depId, long wardId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            var textTemplateDtoList = m_TextTemplateDataService.GetTextTemplatByWard_BySections(scheduleId, depId, wardId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
            var smsTextList = (textTemplateDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            return smsTextList;
        }

        private List<SMSText> GetTextTemplatByDep(Guid scheduleId, long depId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            var textTemplateDtoList = m_TextTemplateDataService.GetTextTemplatByDepartment(scheduleId, depId, hospitalId, locationId, contactTypes, officialLevelofcare);
            var smsTextList = (textTemplateDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            return smsTextList;
        }



        public List<SMSText> GetAllSMSTextTemplates(bool isActive, long hospitalId)
        {
            var smsTextDtoList = m_TextTemplateDataService.SearchTextTemplate(null, null, null, null, null, isActive, false, hospitalId);
            return (smsTextDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
        }

        private List<SMSText> GetTextTemplateByHospital(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            var textTemplateDtoList = m_TextTemplateDataService.GetTextTemplatByHospitalLevel(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
            var smsTextList = (textTemplateDtoList.Select(dto => MapDataDtoToModel(dto))).ToList();
            return smsTextList;
        }

        #endregion
    }
}
