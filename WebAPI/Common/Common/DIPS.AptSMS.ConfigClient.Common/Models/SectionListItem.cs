using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
	/// <summary>
	/// List item for Section 
	/// </summary>
	public class SectionListItem
	{
		public long SectionId { get; set; }
		public string SectionDisplayName { get; set; }
		public string UnitGid { get; set; }
		public List<OPDListItem> OPDList { get; set; }
		public List<WardListItem> WardList { get; set; }
		public List<LocationListItem> LocationList { get; set; }
	}
}
