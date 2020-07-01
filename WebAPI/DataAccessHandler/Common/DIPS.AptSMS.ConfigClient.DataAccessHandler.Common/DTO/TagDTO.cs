using System;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO
{
    public class TagDTO
    {
        public Guid? TagGUID { get; set; }

        public long? HospitalID { get; set; } //HospitalID can be null for XPath

        public long? DepartmentID { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public int TagType { get; set; }

        public int DataType { get; set; }

        public bool IsActive { get; set; }

        public Guid? ReplacedByTagGUID { get; set; }
    }
}
