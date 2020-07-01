using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
	/// <summary>
	/// information of ward list item
	/// </summary>
	public class WardListItem
	{
		public long WardId { get; set; }
		public string WardDisplayName { get; set; }
		public string UnitGid { get; set; }
		public  List<OPDListItem> OPDList { get; set; }
		public List<LocationListItem> LocationList { get; set; }
	}
}