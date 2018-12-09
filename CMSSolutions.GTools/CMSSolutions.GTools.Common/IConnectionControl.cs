using System.Windows.Forms;
using CMSSolutions.GTools.Common.Models;
using CMSSolutions.GTools.Common.Wizard;

namespace CMSSolutions.GTools.Common
{
    public interface IConnectionControl
    {
        ConnectionDetails ConnectionDetails { get; set; }

        UserControl Content { get; }

        bool ValidateConnection();
    }
}