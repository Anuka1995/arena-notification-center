
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
	 
	/// <summary>
	/// Handel object of Department 
	/// </summary>
	public class Department
	{	  
		public long DepartmentId { get; set; }
		public string DepartmentUnitGid { get; set; }
		public string DepartmentName { get; set; }
		public List<OPDListItem> OPDList { get; set; }
		public List<SectionListItem> SectionList { get; set; }
		public List<WardListItem> WardList { get; set; }
		public List<LocationListItem> LocationList { get; set; }
	}
}
