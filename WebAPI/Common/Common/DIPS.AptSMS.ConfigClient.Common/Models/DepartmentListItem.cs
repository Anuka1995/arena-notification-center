
using System;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
	/// <summary>
	/// Handel object of Department Display List item
	/// </summary>
	public class DepartmentListItem
	{	  
		public long DepartmentId { get; set; }
		public string DepartmentName { get; set; }
		public string DepartmentShortName { get; set; }
		public string UnitGid { get; set; }
	}
}
