using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.TreeView
{
    public static class TreeViewUtils
    {
        // All the tree node operations should go there
        public static TreeNode AddNodeToParent(TreeNode parentNode, List<TreeNode> childNodes)
        {
            parentNode.ChildNodes.AddRange(childNodes);
            
            return parentNode;
        }
    }
}
