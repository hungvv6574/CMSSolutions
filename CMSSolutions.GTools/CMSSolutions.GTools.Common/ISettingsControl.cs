using System.Windows.Forms;

namespace CMSSolutions.GTools.Common
{
    public interface ISettingsControl
    {
        UserControl ControlContent { get; }

        void Save();
    }
}