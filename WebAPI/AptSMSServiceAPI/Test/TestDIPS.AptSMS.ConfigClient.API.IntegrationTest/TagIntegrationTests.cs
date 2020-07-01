using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TestDIPS.AptSMS.ConfigClient.API.IntegrationTest
{
    [TestClass]
    public class TagIntegrationTests : TestBase
    {
        private TService GetInstance<TService>()
        {
            return m_serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        [Ignore]
        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTests_SaveTag_SUCCESS()
        {

            var m_tagService = GetInstance<ITagService>();
            //Save Tags
            var tagItem = CreateDummyTag(TestdepId_1,TestHospitalId);
            var guid = m_tagService.SaveTag(tagItem);
            Assert.IsNotNull(guid);

        }
        [Ignore]
        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTests_GetAllTags_NullTerm_and_NullDepId_RETURNALL()
        {

            var m_tagService = GetInstance<ITagService>();
            //Save Tags
            var tagItem = CreateDummyTag(TestHospitalId, null);
            var guid = m_tagService.SaveTag(tagItem);
            var tagList = m_tagService.GetAllTags(TestHospitalId);
            Assert.IsNotNull(guid);
            Assert.IsTrue(tagList.Count == 1);
        }
        //TODO
        [Ignore]
        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTests_GetTagByTagGuid_SUCCESS()
        {

            var m_tagService = GetInstance<ITagService>();
            //Save Tags
            var tagItem = CreateDummyTag(TestHospitalId, 25);
            var guid = m_tagService.SaveTag(tagItem);
            var returntagItme = m_tagService.GetTagBy(guid);
            Assert.IsNotNull(guid);
            Assert.IsNotNull(returntagItme);
        }

        [Ignore]
        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTests_GetTagBySearchTerm_SUCCESS()
        {

            var m_tagService = GetInstance<ITagService>();
            //Save Tags
            var tagItem = CreateDummyTag(TestHospitalId, null);
            var guid = m_tagService.SaveTag(tagItem);
            var returntagItme = m_tagService.GetTagBy(tagItem.Description, TestHospitalId);
            Assert.IsNotNull(guid);
            Assert.IsNotNull(returntagItme);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTests_SearchTags_OnlyForGivenHospitalId_SUCCESS()
        {

            var m_tagService = GetInstance<ITagService>();
            //Save Tags
            var tagItem = CreateDummyTag(TestHospitalId, null);
            var guid = m_tagService.SaveTag(tagItem);

            var returnList = m_tagService.GetTagsBy(null, null, false, false, TestHospitalId);
            Assert.IsNotNull(guid);
            Assert.IsTrue(returnList.Any(c => c.TagId == guid));

            var returnList2 = m_tagService.GetTagsBy(null, null, false, false, TestHospitalId2);
            Assert.IsNotNull(returnList2);
            Assert.IsFalse(returnList2.Any(c => c.TagId == guid));
        }

        private TagItem CreateDummyTag(long hospitalId, long? departmentId)
        {
            var tag = new TagItem()
            {
                //TagId = Guid.NewGuid(),
                DepartmentId = departmentId,
                DataType = TagDataType.INT,
                Description = "my tag description",
                IsActive = true,
                TagName = "MYTAG",
                TagType = TagType.STATIC_VALUE,
                TagValue = "My Value",
                HospitalId = hospitalId
            };
            return tag;
        }
    }
}
