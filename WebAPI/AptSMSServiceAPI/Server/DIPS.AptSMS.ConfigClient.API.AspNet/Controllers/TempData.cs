using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    /// <summary>
    /// This is temporary class which returns the dummy date for front ent 
    /// </summary>
    public static class TempData
    {
        public static List<OPDListItem> GetOpds()
        {
            var opdListItems = new List<OPDListItem>()
            {
                new OPDListItem()
                {
                    OPDDisplayName = "OPD 1",
                    OPDID = 1
                },
                new OPDListItem()
                {
                    OPDDisplayName = "OPD 2",
                    OPDID = 2
                }
            };
            return opdListItems;
        }

        
        public static List<WardListItem> GetWardsByDepartment()
        {
            var opdListItems = new List<WardListItem>()
            {
                new WardListItem()
                {
                    WardDisplayName = "ward 1",
                    WardId = 1
                },
                new WardListItem()
                {
                    WardDisplayName = "ward 2",
                    WardId = 2
                }
            };
            return opdListItems;
        }

        public static TagItem GetTestDataTag()
        {
            var tag = new TagItem()
            {
                TagId = Guid.NewGuid(),
                DepartmentId = 25,
                DataType = TagDataType.INT,
                Description = "my tag description",
                IsActive = true,
                TagName = "MYTAG",
                TagType = TagType.STATIC_VALUE,
                TagValue = "My Value"
            };
            return tag;
        }
    }
}
