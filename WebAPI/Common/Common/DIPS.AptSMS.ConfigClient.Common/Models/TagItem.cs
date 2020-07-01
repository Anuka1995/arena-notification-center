using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// Object of 'Tag Item' 
    /// </summary>
    public class TagItem
    {
        public Guid? TagId { get; set; }
        public long? HospitalId { get; set; }
        public long? DepartmentId { get; set; }
        public String TagName { get; set; }
        public String Description { get; set; }
        public String TagValue { get; set; }
        public TagType TagType { get; set; }
        public TagDataType DataType	{ get; set; }
        public bool IsActive { get; set; }
      
    }
}
