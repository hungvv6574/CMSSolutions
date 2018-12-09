using System.Windows.Forms;

namespace CMSSolutions.GTools.Common.Wizard
{
    public interface IWizardPage
    {
        UserControl Content { get; }

        void Load();

        void Save();

        bool PageValid { get; }

        string ValidationMessage { get; }
    }

    public interface IAsyncWizardPage : IWizardPage
    {
        void Cancel();

        bool IsBusy { get; }
    }
}