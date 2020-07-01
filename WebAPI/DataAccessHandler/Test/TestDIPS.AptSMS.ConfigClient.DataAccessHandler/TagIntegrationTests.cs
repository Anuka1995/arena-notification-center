using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;

namespace TestDIPS.AptSMS.ConfigClient.DAH.IntegrationTest
{
    [TestClass]
    public class TagIntegrationTests : TestBase
    {
        private readonly long HospitalId = 1;
        private readonly long DepartmentId = 25;

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_CreateStringTag_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var guid = tagDatatService.SaveTag(getATagDTO());

            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SaveAndGetTagByGuid_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var guid = tagDatatService.SaveTag(getATagDTO());

            var tagDTO = tagDatatService.GetTagBy(guid);

            Assert.IsNotNull(tagDTO);
            Assert.AreEqual(tagDTO.TagGUID, guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SaveTagName_DUPLICATE_FAIL()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var guidOne = tagDatatService.SaveTag(getATagDTO(title: "MY_TAG1"));

            Assert.IsNotNull(guidOne);

            try
            {
                var guidTwo = tagDatatService.SaveTag(getATagDTO(title: "MY_TAG1", value: "different tag value"));
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
                Assert.IsTrue(e is DBOperationException);
                e = (DBOperationException)e;
            }
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_GetTagByGuid_NOTEXISITS_EmptyResultSet()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var tagDTO = tagDatatService.GetTagBy(new Guid());

            Assert.IsNull(tagDTO);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_EditStringTag_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var firstGuid = tagDatatService.SaveTag(getATagDTO());

            var firstSavedTag = tagDatatService.GetTagBy(firstGuid);

            firstSavedTag.Value = "new tag value";

            var newguid = tagDatatService.SaveTag(firstSavedTag);

            Assert.IsNotNull(newguid);
            Assert.AreNotEqual(firstGuid, newguid);

            var oldTag = tagDatatService.GetTagBy(firstGuid);
            var updatedTag = tagDatatService.GetTagBy(newguid);

            Assert.AreEqual(oldTag.ReplacedByTagGUID, updatedTag.TagGUID);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_CreateXPathTag_SHOULD_FAIL()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var newtag =
                new TagDTO
                {
                    Name = "TITLE",
                    Value = "Hi Patient",
                    Description = "This is a tag for title",
                    TagType = (int)TagType.XPATH,
                    DataType = (int)TagDataType.STRING,
                    HospitalID = 1,
                    DepartmentID = 25,
                    IsActive = true
                };

            try
            {
                var guid = tagDatatService.SaveTag(getATagDTO(type: (int)TagType.XPATH));
            }
            catch (Exception ex)
            {
                // should give an excption from db
                Assert.IsNotNull(ex);
            }
        }

        [Ignore]
        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_EditXPathTag_SHOULD_FAIL()
        {
            var tagDatatService = GetInstance<ITagDataService>();
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsBySearchTerm_InNameFULL_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TITLE";

            var guid = tagDatatService.SaveTag(getATagDTO(title: tagname));

            var tagresult = tagDatatService.SearchTags(null, tagname, true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsBySearchTerm_InNamePart_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TAG1245";

            var guid = tagDatatService.SaveTag(getATagDTO(title: tagname));

            var tagresult = tagDatatService.SearchTags(null, "TAG", true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsBySearchTerm_InName_NORESULT_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var guid = tagDatatService.SaveTag(getATagDTO());

            var tagresult = tagDatatService.SearchTags(null, "WRONGTAGNAME", true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count == 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsBySearchTerm_InValue_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var guid = tagDatatService.SaveTag(getATagDTO(value: "Tag for title"));

            var tagresult = tagDatatService.SearchTags(null, "for", true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsBySearchTerm_InValueNORESULT_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();

            var guid = tagDatatService.SaveTag(getATagDTO());

            var tagresult = tagDatatService.SearchTags(null, "nothing", true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count == 0);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsByDepartment_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var guid = tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId));

            var tagresult = tagDatatService.SearchTags(DepartmentId, string.Empty, true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsByDepartmentAndTerm_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagtitle = "MYTAG";
            var guid = tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId, title: tagtitle));

            var tagresult = tagDatatService.SearchTags(DepartmentId, tagtitle, true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
            Assert.IsTrue(tagresult.First().DepartmentID == DepartmentId);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsAllActiveHospitalLevel_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: "SearchForTagsByHospitalLevel_1", value: "hospital tag 1"));
            tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId, title: "SearchForTagsByHospitalLevel_2", value: "not hospital tag 1"));

            var tagresult = tagDatatService.SearchTags(null, "", true, true, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
            foreach (var tag in tagresult)
            {
                Assert.IsTrue(tag.DepartmentID == null);
                Assert.IsTrue(tag.IsActive == true);
            }
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsByHospitalLevelTags_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: "SearchForTagsByHospitalLevel_1", value: "hospital tag 1"));
            tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId, title: "SearchForTagsByHospitalLevel_2", value: "not hospital tag 1"));

            var tagresult = tagDatatService.SearchTags(null, "hospital tag", true, true, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
            foreach (var tag in tagresult)
            {
                Assert.IsTrue(tag.DepartmentID == null);
            }
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsAllInActiveHospitalLevel_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagDto = getATagDTO(HospitalId, null, title: "ActiveTagOne", value: "hospital tag 1");
            tagDto.IsActive = false;
            var tagDto1 = getATagDTO(HospitalId, DepartmentId, title: "ActiveTagTwo", value: "not hospital tag 1");
            tagDto1.IsActive = false;

            tagDatatService.SaveTag(tagDto);
            tagDatatService.SaveTag(tagDto1);

            var tagresult = tagDatatService.SearchTags(null, "", false, true, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
            foreach (var tag in tagresult)
            {
                Assert.IsTrue(tag.DepartmentID == null);
            }
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchTags_GetTagsOnlyForGivenHospitalId_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var guid = tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId));

            var tagresult = tagDatatService.SearchTags(null, string.Empty, true, false, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Any(c => c.TagGUID == guid));

            var tagresult1 = tagDatatService.SearchTags(null, string.Empty, true, false, TestHospitalId2);
            Assert.IsNotNull(tagresult1);
            Assert.IsFalse(tagresult1.Any(c => c.TagGUID == guid));
        }
        
        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsInModalBySearchTerm_InNameEmpty_ForHospital_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TITLE";

            var guid1 = tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: tagname));

            var tagresult = tagDatatService.SearchTags(null, null, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsInModalBySearchTerm_InNameEmpty_ForDepartment_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TITLE";

            var guid1 = tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: tagname));
            var guid2 = tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId, title: tagname));

            var tagresult = tagDatatService.SearchTags(DepartmentId, null, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsInModalBySearchTerm_InNameFULL_ForHospital_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TITLE";
            
            var guid1 = tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: tagname));

            var tagresult = tagDatatService.SearchTags(null, tagname, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsInModalBySearchTerm_InNameFULL_ForDepartment_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TITLE";

            var guid1 = tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: tagname));
            var guid2 = tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId, title: tagname));

            var tagresult = tagDatatService.SearchTags(DepartmentId, null, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsInModalBySearchTerm_InNamePart_ForHospital_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TAG1245";
            var searchTerm = "TAG";

            var guid = tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: tagname));

            var tagresult = tagDatatService.SearchTags(null, searchTerm, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SearchForTagsInModalBySearchTerm_InNamePart_ForDepartment_SUCCESS()
        {
            var tagDatatService = GetInstance<ITagDataService>();
            var tagname = "TAG1234";
            var searchTerm = "TAG";

            var guid1 = tagDatatService.SaveTag(getATagDTO(HospitalId, null, title: tagname));
            var guid2 = tagDatatService.SaveTag(getATagDTO(HospitalId, DepartmentId, title: tagname));

            var tagresult = tagDatatService.SearchTags(DepartmentId, searchTerm, HospitalId);

            Assert.IsNotNull(tagresult);
            Assert.IsTrue(tagresult.Count > 0);
        }

        #region Utils
        private TagDTO getATagDTO(string title = "TITLE", string value = "tag value", int type = 1, int datatype = 0)
        {
            return new TagDTO
            {
                Name = title,
                Value = value,
                Description = "This is a tag for title",
                TagType = type,
                DataType = datatype,
                HospitalID = HospitalId,
                DepartmentID = DepartmentId,
                IsActive = true
            };
        }

        private TagDTO getATagDTO(long? hospitalId, long? department, string title = "TITLE", string value = "tag value", int type = 1, int datatype = 0)
        {
            return new TagDTO
            {
                Name = title,
                Value = value,
                Description = "This is a tag for title",
                TagType = type,
                DataType = datatype,
                HospitalID = hospitalId,
                DepartmentID = department,
                IsActive = true
            };
        }
        #endregion
    }
}
