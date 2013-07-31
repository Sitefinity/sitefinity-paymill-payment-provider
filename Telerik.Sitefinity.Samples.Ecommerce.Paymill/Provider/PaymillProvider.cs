using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Ecommerce.Orders.Model;
using Telerik.Sitefinity.Ecommerce.Payment.Model;
using Telerik.Sitefinity.Modules.Ecommerce.Payment;
using Telerik.Sitefinity.Modules.Ecommerce.Utilities;

namespace Telerik.Sitefinity.Samples.Ecommerce.Paymill
{
    public class PaymillProvider : PaymentProcessorProviderBase, IPaymentProcessorProvider
    {
        #region Constants
        
        public const int RESULT_SUCCESS = 0;
        public const string GATEWAY_NAME = "PAYMILL";
        public const string PaymillProcessorId = "935BA727-F7F2-4F5E-B077-036627693E55";
        /// <summary>
        /// Paymill Payment Provider requires to give them the name of the JavaScript function that will parse the response and they 
        /// append it in the beginning of the response. Example Response: "NONE(--JSON--)".
        /// </summary>
        public const string JSON_PARSER_FUNCTION_NAME = "NONE";
        public const string PAYMILL_VIRTUAL_PATH = "~/SFPAYMILL/";

        #endregion
        
        #region Fields
        
        protected PaymillSettings paymillSettings;
        
        #endregion
        
        #region Properties
            
        public int Timeout
        {
            get
            {
                return this.paymillSettings.Timeout > 0 ? int.Parse(this.paymillSettings.Timeout.ToString()) : PaymentProcessorProviderBase.DefaultWebRequestTimeout;
            }
        }
        
        #endregion
        
        #region Constructor
        
        public PaymillProvider()
        {
        }
                
        public PaymillProvider(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
            {
                throw new Exception("Paymill payment method cannot be null");
            }
            else
            {
                GetPaymentProcessorSettings(paymentMethod);
            }
        }
        
        #endregion
            
        #region IPaymentProcessorProvider Members
            
        public virtual IPaymentResponse SubmitTransaction(IPaymentRequest data)
        {
            this.paymillSettings = base.GetDeserializedSettings<PaymillSettings>(data.PaymentProcessorSettings);

            if (paymillSettings.PaymentType == "sale")
            {
                return Sale(data);
            }
        
            LogMessage("Payment Transaction type is not supported, use sale.");
            return new PaymentResponse { IsSuccess = false, GatewayResponse = "Payment transaction type not supported" };
        }

        public virtual IPaymentResponse Credit(IPaymentRequest data)
        {
            throw new NotImplementedException();
        }
            
        public virtual IPaymentResponse Sale(IPaymentRequest data)
        {
            string cardName = base.CreditCardNameFromCardType(data.CreditCard);
            if (String.IsNullOrEmpty(cardName) == true)
            {
                LogMessage("credit card type is not supported");
                return new PaymentResponse { IsSuccess = false, GatewayResponse = "Credit card type not supported" };
            }

            if (this.paymillSettings == null)
            {
                this.paymillSettings = (PaymillSettings)data.PaymentProcessorSettings;
            }
            
            NameValueCollection getRequestValues = GetRequestValues(data);
            
            string responseJson = this.GetWebRequest(paymillSettings.GatewayUrl, getRequestValues, paymillSettings.Timeout);

            IPaymentResponse payResponse = this.ParsePaymillResponse(responseJson);
        
            return payResponse;
        }

        public virtual IPaymentResponse Authorize(IPaymentRequest data)
        {
            throw new NotImplementedException();
        }

        public virtual IPaymentResponse Capture(IPaymentRequest data, string originalTransactionID)
        {
            throw new NotImplementedException();
        }

        public virtual IPaymentResponse Void(IPaymentRequest data, string originalTransactionID)
        {
            throw new NotImplementedException();
        }

        public virtual IPaymentResponse AuthVoidCapture(IPaymentRequest data)
        {
            throw new NotImplementedException();
        }

        public virtual IPaymentResponse PerformValidations(IPaymentRequest data, bool avsAddress, bool avsZip, bool csc)
        {
            throw new NotImplementedException();
        }

        public virtual IPaymentResponse TestConnection(IPaymentRequest data)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
        #region Protected Methods
        
        /// <summary>
        /// Write a string message to the Sitefinity Log
        /// </summary>
        /// <param name="message">Message string to be logged</param>
        protected virtual void LogMessage(string message)
        {
            base.LogMessage(message, PaymillProcessorId);
        }
        
        /// <summary>
        /// Deserialize the payment settings from the PaymentMethod object into a module level variable
        /// </summary>
        /// <param name="paymentMethod">PaymentMethod object</param>
        protected virtual void GetPaymentProcessorSettings(PaymentMethod paymentMethod)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            paymillSettings = (PaymillSettings)serializer.Deserialize(paymentMethod.PaymentProcessorSettings, typeof(PaymillSettings));
        }
            
        protected virtual NameValueCollection GetRequestValues(IPaymentRequest data)
        {
            NameValueCollection requestValues = new NameValueCollection();
            
            //transaction mode should awlays be the first argument!!
            requestValues.Add("transaction.mode", paymillSettings.ProcessingMode);
            requestValues.Add("channel.id", paymillSettings.TestMode ? paymillSettings.TestPublicApiKey : paymillSettings.LivePublicApiKey);
            requestValues.Add("jsonPFunction", PaymillProvider.JSON_PARSER_FUNCTION_NAME);
            requestValues.Add("account.number", data.CreditCardNumber);
            requestValues.Add("account.expiry.month", data.CreditCardExpireMonth.ToString("00"));
            requestValues.Add("account.expiry.year", data.CreditCardExpireYear.ToString());
            requestValues.Add("account.verification", data.CVV);
            requestValues.Add("account.holder", data.CardHolderName);
            
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberGroupSeparator = "";

            // Through Paymill its charged for the whole amount (products + tax + shipping).
            decimal finalAmount = data.Amount + data.TaxAmount + data.ShippingAmount;
            requestValues.Add("presentation.amount3D", finalAmount.ToString("N2", nfi));
            requestValues.Add("presentation.currency3D", data.CurrencyCode);
        
            return requestValues;
        }
            
        protected virtual string GetWebRequest(string url, NameValueCollection values, double timeout)
        {
            StringBuilder requestURL = new StringBuilder();
                
            string transactionModeKey = values.GetKey(0);
            
            if (transactionModeKey == "transaction.mode")
            {
                requestURL.Append(string.Format("{0}?{1}={2}", url.Trim(), transactionModeKey.Trim(), values[0].Trim()));
            }
            else
            {
                throw new ArgumentException("Request values aren't in valid format!");
            }

            for (int valuesIndex = 1; valuesIndex < values.Count; valuesIndex++)
            {
                UrlEncodeAndAddItem(ref requestURL, values.GetKey(valuesIndex), values[valuesIndex]);
            }
        
            return HttpClient.GetRequest(requestURL.ToString(), (int)timeout);
        }
            
        protected virtual IPaymentResponse ParsePaymillResponse(string directJsonResponse)
        {
            IPaymentResponse paymentResponse = new PaymentResponse();
            
            directJsonResponse = directJsonResponse.Remove(0, PaymillProvider.JSON_PARSER_FUNCTION_NAME.Length + 1);
            directJsonResponse = directJsonResponse.Remove(directJsonResponse.Length - 1, 1);
            dynamic responseObj = JsonConvert.DeserializeObject(directJsonResponse);
            
            paymentResponse.IsSuccess = (responseObj.transaction.processing.result == "ACK");
            
            paymentResponse.FraudScore = 0;
            paymentResponse.GatewayAdditionalResponse = "";
            paymentResponse.GatewayAdditionalResult = 0;
            paymentResponse.GatewayAltTransactionID = "";
            paymentResponse.GatewayAuthCode = paymillSettings.PublicApiKey;
            paymentResponse.GatewayAVS = "";
            paymentResponse.GatewayAVSAddress = "";
            paymentResponse.GatewayAVSAddressInternational = "";
            paymentResponse.GatewayAVSZip = "";
            paymentResponse.GatewayCorrelationID = "";
            paymentResponse.GatewayCSC = "";
            paymentResponse.GatewayCSCResponse = "";
            paymentResponse.GatewayFraudResponse = "";
            paymentResponse.GatewayID = Guid.Empty; // processorId;
            paymentResponse.GatewayName = PaymillProvider.GATEWAY_NAME;
            paymentResponse.GatewayResponse = responseObj.transaction.response;
            paymentResponse.GatewayResult = Convert.ToInt32(paymentResponse.IsSuccess);
            paymentResponse.GatewayToken = responseObj.transaction.channel;
            paymentResponse.GatewayTransactionID = responseObj.transaction.identification.uniqueId;
            paymentResponse.GatewayTransactionType = "sale";
            paymentResponse.PaymentProcessorID = Guid.Empty; //processorId;
        
            return paymentResponse;
        }
        
        /// <summary>
        /// JSON Serialization
        /// </summary>
        protected static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }
        
        /// <summary>
        /// JSON Deserialization
        /// </summary>
        protected static T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        #endregion
    }
}