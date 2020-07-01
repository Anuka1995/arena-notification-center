using System;


namespace DIPS.AptSMS.ConfigClient.Common.Models
{
	/// <summary>
	/// information of Tag List item
	/// </summary>
	public class TagListItem
	{
		public Guid TagId { get; set; }
		public string TagDisplayName { get; set; }
	}
}
