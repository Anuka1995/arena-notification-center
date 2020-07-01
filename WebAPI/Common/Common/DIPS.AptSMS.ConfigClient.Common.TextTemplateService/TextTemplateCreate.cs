using DIPS.AptSMS.ConfigClient.Common.FormatFunction;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
//using DIPS.Infrastructure.Logging;
//using TagData = System.Collections.Generic.KeyValuePair<string, string>;


namespace DIPS.AptSMS.ConfigClient.Common.TextTemplateService
{
    public class TextTemplateCreate
    {
        static readonly string m_tagStart = @"\[\$\$";
        static readonly string m_tagEnd = @"\$\$\]";
        static readonly string m_tagStartdString = "[$$";
        static readonly string m_tagEndString = "$$]";
        static readonly string m_PssHospitalCode = "<HID>";
        static readonly string m_hospitalCodeTag = "SYKEHUSID";
        static readonly string m_smptySpace = "          ";//This is basically for keep seperate PSS-URL from basic information of SMS
        /// <summary>
        /// This method will return complete 'SMS' Text according to the template and given Booking XML/Planned contact  XML
        /// </summary>
        /// <param name="bookingXml">Planned Contact XML</param>
        /// <param name="smsTextTemplate">Template Text selected for SMS</param>
        /// <param name="smsTemplateTagList">List of Tags for SMS template</param>
        /// <param name="pssLink">Link to download PSS </param>
        /// <param name="isPssAtached">bool is need to attached PSS link</param>
        /// <returns></returns>
        public static string GetCompleteSmsText(XmlDocument bookingXml, string smsTextTemplate, List<TagItem> smsTemplateTagList, string pssLink, bool isPssAtached)
        {
            var tagDictonary = LoadTextTemplateIncludedTagList(smsTextTemplate);
            var tagResultDictonary = LoadTagWithValue(tagDictonary, smsTemplateTagList, bookingXml);
            var templateTextStr = smsTextTemplate;
            foreach (var tagItem in tagDictonary)
            {
                var value = tagResultDictonary.FirstOrDefault(t => t.Key == tagItem.Name);
                var result = value.Value;
                var fulTag = m_tagStartdString + tagItem.Name + m_tagEndString;
                if (tagItem.value.Length > 0)
                {
                    //result = ApplyFunction(result, tagItem.Value);
                    result = TagFormater.GetFormatString(tagItem.value, result);
                    fulTag = m_tagStartdString + tagItem.Name + ":" + tagItem.value + m_tagEndString;
                }
                templateTextStr = templateTextStr.Replace(fulTag, result);
            }
            if (isPssAtached)
            {
                templateTextStr = templateTextStr + m_smptySpace + GetPSSLinkAttachedwithHospitalID(pssLink, smsTemplateTagList, bookingXml);
            }
            return templateTextStr;
        }

        #region Private

        static string GetPSSLinkAttachedwithHospitalID(string basePssLink, List<TagItem> smsTemplateTagList, XmlDocument bookingXml)
        {
            string hospitalCode = "0";
            var value = smsTemplateTagList.FirstOrDefault(t => t.TagName == m_hospitalCodeTag);
            if(value != null)
                hospitalCode =  GetXpathValue(value.TagValue, bookingXml);
            return basePssLink.Replace(m_PssHospitalCode, hospitalCode);
        }
 
        static List<TagDataItem> LoadTextTemplateIncludedTagList(string textTemplateString)
        {
            if (textTemplateString == null || textTemplateString.Length < 5)
                return null;
            List<TagDataItem> tagDictionary = new List<TagDataItem>();
            foreach (string tagCandidate in Regex.Split(textTemplateString, m_tagStart))
            {
                if (tagCandidate.Contains(m_tagEndString))
                {
                    var tagPredict = Regex.Split(tagCandidate, m_tagEnd);
                    if (tagPredict != null && tagPredict.Length > 1)
                    {
                        if (tagPredict[0].Trim().Length > 0)
                        {
                            var tagValue = GetTagWithFormatFunction(tagPredict[0].Trim());
                            tagDictionary.Add(new TagDataItem {Name  = tagValue.Key, value = tagValue.Value });
                        }
                    }
                }
            }
            return tagDictionary;
        }

        static KeyValuePair<string, string> GetTagWithFormatFunction(string potentialTag)
        {
            if (potentialTag != null && potentialTag.Length > 1)
            {
                if (potentialTag.Contains(":"))
                {
                    var potentialTagArray = Regex.Split(potentialTag, ":");
                    if (potentialTagArray[0].Trim().Length > 0)
                    {
                        if (potentialTagArray.Length > 2)
                        {
                            var tagWithoutFormat = potentialTagArray[0].Trim() + ":";
                            return new KeyValuePair<string, string>(potentialTagArray[0].Trim(), potentialTag.Replace(tagWithoutFormat, string.Empty));
                        }
                        return new KeyValuePair<string, string>(potentialTagArray[0].Trim(), potentialTagArray[1].Trim());
                    }
                }
            }
            return new KeyValuePair<string, string>(potentialTag, "");
        }

        static Dictionary<string, string> LoadTagWithValue(List<TagDataItem> tagValueDictonary, List<TagItem> smsTemplateTagList, XmlDocument bookingXml)
        {
            var tagDictionary = new Dictionary<string, string>();
            foreach (var tagItem in tagValueDictonary)
            {
                if (tagItem.Name.Length > 0)
                {
                    var tagName = tagItem.Name.Trim();
                    var tagValue = tagItem.value.Trim();
                    var tagObject = smsTemplateTagList.FirstOrDefault(o => o.TagName == tagName); 
                    if (tagObject != null)
                    {
                        tagValue = tagObject.TagValue.Trim();
                        if (tagObject.TagType == ConfigClient.Common.Models.TagType.XPATH)
                        {
                            tagValue = GetXpathValue(tagObject.TagValue,  bookingXml);                            
                        }
                        if (!tagDictionary.ContainsKey(tagName))
                        {
                            tagDictionary.Add(tagName, tagValue);
                        }
                    }
                }
            }
            return tagDictionary;
        }
        
        static string GetXpathValue(string xPathString, XmlDocument bookingXml)
        {           
            try
            {
                XmlNode dataAttribute = bookingXml.SelectSingleNode(xPathString);
                return dataAttribute.InnerText.Trim();
            }
            catch (Exception ex)
            {
                var errormessage = "Failed to load GetXpathValue for tag Xpath " + bookingXml;
                return "";
            }            
        }

        #endregion Private
    }

    internal class TagDataItem
    {
        public string Name { get; set; }
        public string value { get; set; }
    }
}
