﻿using System;
using System.Collections.Generic;
using System.Linq;
using VortexSoft.MvcCornerStone.Plugins;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    /// <summary>
    /// Payment service
    /// </summary>
    public class PaymentService : IPaymentService
    {
        #region Fields

        private readonly PaymentSettings paymentSettings;
        private readonly IPluginFinder pluginFinder;

        #endregion Fields

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        public PaymentService(PaymentSettings paymentSettings, IPluginFinder pluginFinder)
        {
            this.paymentSettings = paymentSettings;
            this.pluginFinder = pluginFinder;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Load active payment methods
        /// </summary>
        /// <param name="filterByCustomerId">Filter payment methods by customer; null to load all records</param>
        /// <returns>Payment methods</returns>
        public virtual IList<IPaymentMethod> LoadActivePaymentMethods(int? filterByCustomerId = null)
        {
            return LoadAllPaymentMethods()
                   .Where(provider => paymentSettings.ActivePaymentMethodNames.Contains(provider.PluginDescriptor.Name, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
        }

        /// <summary>
        /// Load payment provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found payment provider</returns>
        public virtual IPaymentMethod LoadPaymentMethodByName(string systemName)
        {
            var descriptor = pluginFinder.GetPluginDescriptorByName<IPaymentMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IPaymentMethod>();

            return null;
        }

        /// <summary>
        /// Load all payment providers
        /// </summary>
        /// <returns>Payment providers</returns>
        public virtual IList<IPaymentMethod> LoadAllPaymentMethods()
        {
            return pluginFinder.GetPlugins<IPaymentMethod>().ToList();
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public virtual ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResult
                                 {
                                     NewPaymentStatus = PaymentStatus.Paid
                                 };
                return result;
            }

            var paymentMethod = LoadPaymentMethodByName(processPaymentRequest.PaymentMethodName);
            if (paymentMethod == null)
                throw new FrameworkException("Payment method couldn't be loaded");
            return paymentMethod.ProcessPayment(processPaymentRequest);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public virtual void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //already paid or order.OrderTotal == decimal.Zero
            if (postProcessPaymentRequest.Order.PaymentStatus == PaymentStatus.Paid)
                return;

            var paymentMethod = LoadPaymentMethodByName(postProcessPaymentRequest.Order.PaymentMethodName);
            if (paymentMethod == null)
                throw new FrameworkException("Payment method couldn't be loaded");
            paymentMethod.PostProcessPayment(postProcessPaymentRequest);
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public virtual bool CanRePostProcessPayment(IOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!paymentSettings.AllowRePostingPayments)
                return false;

            var paymentMethod = LoadPaymentMethodByName(order.PaymentMethodName);
            if (paymentMethod == null)
                return false; //Payment method couldn't be loaded (for example, was uninstalled)

            if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                return false;   //this option is available only for redirection payment methods

            return paymentMethod.CanRePostProcessPayment(order);
        }

        /// <summary>
        /// Gets an additional handling fee of a payment method
        /// </summary>
        /// <param name="paymentMethodName">Payment method system name</param>
        /// <returns>Additional handling fee</returns>
        public virtual decimal GetAdditionalHandlingFee(string paymentMethodName)
        {
            var paymentMethod = LoadPaymentMethodByName(paymentMethodName);
            if (paymentMethod == null)
                return decimal.Zero;

            decimal result = paymentMethod.GetAdditionalHandlingFee();
            if (result < decimal.Zero)
                result = decimal.Zero;
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported by payment method
        /// </summary>
        /// <param name="paymentMethodName">Payment method system name</param>
        /// <returns>A value indicating whether capture is supported</returns>
        public virtual bool SupportCapture(string paymentMethodName)
        {
            var paymentMethod = LoadPaymentMethodByName(paymentMethodName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportCapture;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public virtual CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var paymentMethod = LoadPaymentMethodByName(capturePaymentRequest.Order.PaymentMethodName);
            if (paymentMethod == null)
                throw new FrameworkException("Payment method couldn't be loaded");
            return paymentMethod.Capture(capturePaymentRequest);
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodName">Payment method system name</param>
        /// <returns>A value indicating whether partial refund is supported</returns>
        public virtual bool SupportPartiallyRefund(string paymentMethodName)
        {
            var paymentMethod = LoadPaymentMethodByName(paymentMethodName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportPartiallyRefund;
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodName">Payment method system name</param>
        /// <returns>A value indicating whether refund is supported</returns>
        public virtual bool SupportRefund(string paymentMethodName)
        {
            var paymentMethod = LoadPaymentMethodByName(paymentMethodName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportRefund;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public virtual RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var paymentMethod = LoadPaymentMethodByName(refundPaymentRequest.Order.PaymentMethodName);
            if (paymentMethod == null)
                throw new FrameworkException("Payment method couldn't be loaded");
            return paymentMethod.Refund(refundPaymentRequest);
        }

        /// <summary>
        /// Gets a value indicating whether void is supported by payment method
        /// </summary>
        /// <param name="paymentMethodName">Payment method system name</param>
        /// <returns>A value indicating whether void is supported</returns>
        public virtual bool SupportVoid(string paymentMethodName)
        {
            var paymentMethod = LoadPaymentMethodByName(paymentMethodName);
            if (paymentMethod == null)
                return false;
            return paymentMethod.SupportVoid;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public virtual VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var paymentMethod = LoadPaymentMethodByName(voidPaymentRequest.Order.PaymentMethodName);
            if (paymentMethod == null)
                throw new FrameworkException("Payment method couldn't be loaded");
            return paymentMethod.Void(voidPaymentRequest);
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <param name="paymentMethodName">Payment method system name</param>
        /// <returns>A recurring payment type of payment method</returns>
        public virtual RecurringPaymentType GetRecurringPaymentType(string paymentMethodName)
        {
            var paymentMethod = LoadPaymentMethodByName(paymentMethodName);
            if (paymentMethod == null)
                return RecurringPaymentType.NotSupported;
            return paymentMethod.RecurringPaymentType;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public virtual ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResult
                                 {
                                     NewPaymentStatus = PaymentStatus.Paid
                                 };
                return result;
            }

            var paymentMethod = LoadPaymentMethodByName(processPaymentRequest.PaymentMethodName);
            if (paymentMethod == null)
                throw new FrameworkException("Payment method couldn't be loaded");
            return paymentMethod.ProcessRecurringPayment(processPaymentRequest);
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public virtual CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            if (cancelPaymentRequest.Order.OrderTotal == decimal.Zero)
                return new CancelRecurringPaymentResult();

            var paymentMethod = LoadPaymentMethodByName(cancelPaymentRequest.Order.PaymentMethodName);
            if (paymentMethod == null)
                throw new FrameworkException("Payment method couldn't be loaded");
            return paymentMethod.CancelRecurringPayment(cancelPaymentRequest);
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <param name="paymentMethodName">Payment method system name</param>
        /// <returns>A payment method type</returns>
        public virtual PaymentMethodType GetPaymentMethodType(string paymentMethodName)
        {
            var paymentMethod = LoadPaymentMethodByName(paymentMethodName);
            if (paymentMethod == null)
                return PaymentMethodType.Unknown;
            return paymentMethod.PaymentMethodType;
        }

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        public virtual string GetMaskedCreditCardNumber(string creditCardNumber)
        {
            if (String.IsNullOrEmpty(creditCardNumber))
                return string.Empty;

            if (creditCardNumber.Length <= 4)
                return creditCardNumber;

            string last4 = creditCardNumber.Substring(creditCardNumber.Length - 4, 4);
            string maskedChars = string.Empty;
            for (int i = 0; i < creditCardNumber.Length - 4; i++)
            {
                maskedChars += "*";
            }
            return maskedChars + last4;
        }

        #endregion Methods
    }
}