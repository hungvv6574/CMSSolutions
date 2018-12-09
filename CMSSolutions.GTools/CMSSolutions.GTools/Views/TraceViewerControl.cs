using System;
using System.Windows.Forms;
using CMSSolutions.GTools.Common.Diagnostics;

namespace CMSSolutions.GTools.Views
{
    public partial class TraceViewerControl : UserControl
    {
        public TraceViewerControl()
        {
            InitializeComponent();

            TraceService.Instance.Trace += new TraceService.TraceEventHandler(Instance_Trace);
        }

        void Instance_Trace(TraceEventArgs e)
        {
            if (txtTrace.InvokeRequired)
            {
                Invoke(new MethodInvoker(()=>
                    {
                        txtTrace.AppendText(e.Message);
                        txtTrace.AppendText(System.Environment.NewLine);
                    }));
            }
            else
            {
                txtTrace.AppendText(e.Message);
                txtTrace.AppendText(System.Environment.NewLine);
            }
            Application.DoEvents();
        }
    }
}
