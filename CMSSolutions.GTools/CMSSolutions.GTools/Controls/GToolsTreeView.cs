using System.Drawing;
using System.Windows.Forms;
using CMSSolutions.GTools;
using CMSSolutions.GTools.Common.Configuration;
using CMSSolutions.GTools.Common.Dialogs;
using CMSSolutions.GTools.Properties;

namespace CMSSolutions.GTools.Controls
{
    public class GToolsTreeView : TreeView
    {
        private ImageList imageList = null;

        private TreeNode RootNode { get; set; }
        private TreeNode ConnectionsNode { get; set; }
        private TreeNode TemplatesNode { get; set; }
        private TreeNode SettingsNode { get; set; }

        public GToolsTreeView()
        {
            imageList = new System.Windows.Forms.ImageList();
            imageList.Images.Add(Resources.Migrate);
            imageList.Images.Add(Resources.Connection);
            imageList.Images.Add(Resources.Mapping32x32);
            imageList.Images.Add(Resources.Options32x32);
            imageList.Images.Add(Resources.Table);
            imageList.ImageSize = new Size(24, 24);
            this.ImageList = imageList;
        }

        public void LoadDefaultNodes()
        {
            RootNode = new TreeNode(Constants.TreeView.RootNodeText, 0, 0);
            ConnectionsNode = new TreeNode(Constants.TreeView.ConnectionsNodeText, 1, 1);
            TemplatesNode = new TreeNode(Constants.TreeView.TemplatesNodeText, 2, 2);
            SettingsNode = new TreeNode(Constants.TreeView.SettingsNodeText, 3, 3);

            RootNode.Nodes.Add(ConnectionsNode);
            RootNode.Nodes.Add(TemplatesNode);
            RootNode.Nodes.Add(SettingsNode);

            this.Nodes.Add(RootNode);
            RootNode.ExpandAll();
        }

        public void ClearTemplates()
        {
            TemplatesNode.Nodes.Clear();
        }
    }
}
