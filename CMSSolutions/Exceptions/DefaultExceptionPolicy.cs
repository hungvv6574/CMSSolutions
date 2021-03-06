﻿using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Castle.Core.Logging;
using CMSSolutions.Events;
using CMSSolutions.Localization;
using CMSSolutions.Security;
using CMSSolutions.Web.UI.Notify;

namespace CMSSolutions.Exceptions
{
    public class DefaultExceptionPolicy : IExceptionPolicy
    {
        private readonly INotifier notifier;

        public DefaultExceptionPolicy()
        {
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public DefaultExceptionPolicy(INotifier notifier)
        {
            this.notifier = notifier;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }

        public Localizer T { get; set; }

        public bool HandleException(object sender, Exception exception)
        {
            if (IsFatal(exception))
            {
                return false;
            }

            if (sender is IEventBus)
            {
                return false;
            }

            Logger.ErrorFormat(exception, "An unexpected exception was caught");

            do
            {
                RaiseNotification(exception);
                exception = exception.InnerException;
            } while (exception != null);

            return true;
        }

        private static bool IsFatal(Exception exception)
        {
            return
                exception is NotAuthorizedException ||
                exception is StackOverflowException ||
                exception is OutOfMemoryException ||
                exception is AccessViolationException ||
                exception is AppDomainUnloadedException ||
                exception is ThreadAbortException ||
                exception is SecurityException ||
                exception is SEHException;
        }

        private void RaiseNotification(Exception exception)
        {
            if (notifier == null)
            {
                return;
            }

            notifier.Error(new LocalizedString(exception.Message));
        }
    }
}