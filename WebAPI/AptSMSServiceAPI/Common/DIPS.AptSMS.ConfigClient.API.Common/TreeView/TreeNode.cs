using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.TreeView
{
    public class TreeNode
    {
        public string ParentNode { get; set; }
        public string Type { get; set; }
        public List<TreeNode> ChildNodes { get; set; } = new List<TreeNode>();
        public string Title { get; set; }
        public string Id { get; set; } // we have different types of ids long/guid and we pass json.
        public object Tag { get; set; } // actual object of the node should be here (in case you can cast it and get the data)
     
    }
}
