using System;

namespace CMSSolutions.GTools.Common.Diagnostics
{
    public enum TraceEvent
    {
        Error,
        Information,
        Debug,
        Warning
    }

    public sealed class TraceService
    {
        private static TraceService instance = new TraceService();

        public static TraceService Instance
        {
            get { return instance; }
        }

        public delegate void TraceEventHandler(TraceEventArgs e);

        public event TraceEventHandler Trace;

        public void WriteMessage(string message)
        {
            if (Trace != null)
            {
                Trace(new TraceEventArgs(message));
            }
        }

        public void WriteMessage(TraceEvent traceEvent, string message)
        {
            WriteMessage(string.Concat(traceEvent.ToString().ToUpperInvariant(), ": ", message));
        }

        public void WriteException(Exception x)
        {
            WriteException(x, string.Empty);
        }

        public void WriteException(Exception x, string additionalInfo)
        {
            string errorMessage = string.Concat(
                "ERROR:",
                System.Environment.NewLine,
                "MACHINE NAME: ", System.Environment.MachineName,
                System.Environment.NewLine,
                "TARGET SITE: ", x.TargetSite,
                System.Environment.NewLine,
                "EXCEPTION: ", x.Message,
                System.Environment.NewLine,
                "INNER EXCEPTION: ", x.InnerException == null ? string.Empty : x.InnerException.ToString(),
                System.Environment.NewLine,
                "STACK TRACE: ", x.StackTrace,
                System.Environment.NewLine,
                System.Environment.NewLine,
                "ADDITIONAL INFO: ",
                System.Environment.NewLine,
                additionalInfo);

            WriteMessage(errorMessage);
        }

        public void WriteFormat(TraceEvent traceEvent, string format, params object[] args)
        {
            WriteMessage(traceEvent, string.Format(format, args));
        }

        public void WriteConcat(TraceEvent traceEvent, params object[] args)
        {
            WriteMessage(traceEvent, string.Concat(args));
        }
    }

    public sealed class TraceEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public TraceEventArgs(string message)
            : base()
        {
            Message = message;
        }
    }
}