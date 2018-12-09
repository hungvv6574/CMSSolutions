using System.Drawing;
using System.Windows.Forms;
using CMSSolutions.GTools.Properties;
using CMSSolutions.GTools.Common;

namespace CMSSolutions.GTools.Controls
{
    public class SettingsTreeView : TreeView
    {
        private ImageList imageList;
        public SettingsTreeView()
        {
            imageList = new ImageList {ImageSize = new Size(24, 24)};
            imageList.Images.Add(Resources.TreeNode);
            ImageList = imageList;
        }

        public TreeNode AddSettingsNode(string providerName, ISettingsControl tag)
        {
            if (tag != null)
            {
                var treeNode = new TreeNode(providerName);
                treeNode.Tag = tag;
                Nodes.Add(treeNode);
                return treeNode;
            }

            return null;
        }
    }
}