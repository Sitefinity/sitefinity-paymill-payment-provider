using System.Net;
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
using System.Web;

namespace Telerik.Sitefinity.Samples.Ecommerce.Paymill
{
    public class PaymillProvider : PaymentProcessorProviderBase, IPaymentProcessorProvider
    {
        #region Constants
        
        public const int RESULT_SUCCESS = 0;
        public const string GATEWAY_NAME = "PAYMILL";
        public const string GATEWAY_ID = "935BA727-F7F2-4F5E-B077-036627693E55";

        /// <summary>
        /// The Gateway URL for Paymill Api used in the payment implementation
        /// </summary>
        public const string GATEWAY_URL = "https://api.paymill.com/v2/";

        /// <summary>
        /// The URL for the Paymill Module that generates tokens for the payments in Test Mode
        /// </summary>
        public const string TOKENIZATION_TEST_URL = "https://test-token.paymill.de/";

        /// <summary>
        /// The URL for the Paymill Module that generates tokens for the payments in Live Mode
        /// </summary>
        public const string TOKENIZATION_LIVE_URL = "https://token-v2.paymill.de/";

        /// <summary>
        /// Paymill Payment Provider requires to give them the name of the JavaScript function that will parse the response and they 
        /// append it in the beginning of the response. Example Response: "NONE(--JSON--)".
        /// </summary>
        public const string JSON_PARSER_FUNCTION_NAME = "NONE";

        /// <summary>
        /// The virtual path that is registered to point to the actual local path.
        /// </summary>
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

        public string TokenizationUrl
        {
            get 
            {
                return this.paymillSettings.TestMode ? TOKENIZATION_TEST_URL : TOKENIZATION_LIVE_URL;
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
                this.GetPaymentProcessorSettings(paymentMethod);
            }
        }
        
        #endregion
        
        #region IPaymentProcessorProvider Members
        
        public virtual IPaymentResponse SubmitTransaction(IPaymentRequest data)
        {
            this.paymillSettings = base.GetDeserializedSettings<PaymillSettings>(data.PaymentProcessorSettings);
            
            if (this.paymillSettings.PaymentType == "sale")
            {
                return this.Sale(data);
            }
            else if (this.paymillSettings.PaymentType == "authorize")
            {
                return this.Authorize(data);
            }
            
            this.LogMessage("Payment Transaction type is not supported, use sale.");
            return new PaymentResponse { IsSuccess = false, GatewayResponse = "Payment transaction type not supported" };
        }
        
        public virtual IPaymentResponse Sale(IPaymentRequest data)
        {
            return this.SaleAuth(data, "transactions/");
        }
        
        public virtual IPaymentResponse Authorize(IPaymentRequest data)
        {
            return this.SaleAuth(data, "preauthorizations/");
        }

        public virtual IPaymentResponse Credit(IPaymentRequest data)
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

        protected virtual IPaymentResponse SaleAuth(IPaymentRequest data, string gatewaySubDirectory)
        {
            string cardName = base.CreditCardNameFromCardType(data.CreditCard);
            if (string.IsNullOrEmpty(cardName) == true)
            {
                LogMessage("credit card type is not supported");
                return new PaymentResponse { IsSuccess = false, GatewayResponse = "Credit card type not supported" };
            }

            if (this.paymillSettings == null)
            {
                this.paymillSettings = (PaymillSettings)data.PaymentProcessorSettings;
            }

            // The first request. The response is a Transaction Object in JSON Format. 
            // for more info: https://www.paymill.com/en-gb/documentation-3/reference/paymill-bridge/
            // This replaces the Paymill Bridge
            NameValueCollection getRequestValues = this.GetRequestValues(data);
            string responseJson = this.GetWebRequest(TokenizationUrl, getRequestValues, paymillSettings.Timeout);
            
            // Parsing the response
            responseJson = responseJson.Remove(0, PaymillProvider.JSON_PARSER_FUNCTION_NAME.Length + 1);
            responseJson = responseJson.Remove(responseJson.Length - 1, 1);
            dynamic responseObj = JsonConvert.DeserializeObject(responseJson);

            // The token which is actually the transaction's uniqueId. It's saved in Misc1, because there isn't more appropirate property.
            data.Misc1= responseObj.transaction.identification.uniqueId;

            string url = string.Format("{0}{1}", GATEWAY_URL, gatewaySubDirectory);
            string response = PostWebRequestWithAuthentication(url, data, paymillSettings.Timeout);

            return this.ParsePaymillResponse(response);
        }
        
        #endregion
        
        #region Protected Methods
        
        /// <summary>
        /// Write a string message to the Sitefinity Log
        /// </summary>
        /// <param name="message">Message string to be logged</param>
        protected virtual void LogMessage(string message)
        {
            base.LogMessage(message, GATEWAY_ID.ToString());
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
        
        protected virtual string GetWebRequest(string url, NameValueCollection values, double timeout)
        {
            StringBuilder requestUrlSb = new StringBuilder();
            
            string transactionModeKey = values.GetKey(0);
            
            for (int valuesIndex = 0; valuesIndex < values.Count; valuesIndex++)
            {
                UrlEncodeAndAddItem(ref requestUrlSb, values.GetKey(valuesIndex), values[valuesIndex]);
            }
            
            string requestUrl = string.Format("{0}?{1}", url.Trim(), requestUrlSb);
            
            return HttpClient.GetRequest(requestUrl, (int)timeout);
        }
        
        protected virtual string PostWebRequestWithAuthentication(string url, IPaymentRequest data, double timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(paymillSettings.PrivateApiKey, "");
            request.ContentType = "application/x-www-form-urlencoded";
            
            var contentSb = new StringBuilder();
            contentSb.Append(string.Format("currency={0}", data.CurrencyCode));
            contentSb.Append(string.Format("&amount={0}", (int)(data.Amount * 100)));
            contentSb.Append(string.Format("&token={0}", data.Misc1));
            
            byte[] byteArray = Encoding.UTF8.GetBytes(contentSb.ToString());
            
            request.ContentLength = byteArray.Length;
            
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);   
            }

            string responseFromServer;

            using (WebResponse response = request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        responseFromServer = HttpUtility.HtmlDecode(reader.ReadToEnd());
                    }
                }
            }

            return responseFromServer;
        }

        protected virtual NameValueCollection GetRequestValues(IPaymentRequest data)
        {
            NameValueCollection requestValues = new NameValueCollection();

            requestValues.Add("transaction.mode", paymillSettings.TestMode ? "CONNECTOR_TEST" : paymillSettings.ProcessingMode);
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

        protected virtual IPaymentResponse ParsePaymillResponse(string directJsonResponse)
        {
            IPaymentResponse paymentResponse = new PaymentResponse();

            dynamic responseObj = JsonConvert.DeserializeObject(directJsonResponse);

            if (responseObj.data.status == "closed")
            {
                paymentResponse.IsSuccess = true;
                paymentResponse.GatewayTransactionType = "sale";
            }
            else if (responseObj.data.status == "preauth")
            {
                paymentResponse.IsSuccess = true;
                paymentResponse.IsAuthorizeOnlyTransaction = true;
                paymentResponse.GatewayTransactionType = "authorize";
            }
            else
            {
                paymentResponse.IsSuccess = false;
            }

            paymentResponse.FraudScore = 0;
            paymentResponse.GatewayAdditionalResponse = "";
            paymentResponse.GatewayAdditionalResult = 0;
            paymentResponse.GatewayAltTransactionID = "";
            paymentResponse.GatewayAuthCode = paymillSettings.PrivateApiKey;
            paymentResponse.GatewayAVS = "";
            paymentResponse.GatewayAVSAddress = "";
            paymentResponse.GatewayAVSAddressInternational = "";
            paymentResponse.GatewayAVSZip = "";
            paymentResponse.GatewayCorrelationID = "";
            paymentResponse.GatewayCSC = "";
            paymentResponse.GatewayCSCResponse = "";
            paymentResponse.GatewayFraudResponse = "";
            paymentResponse.GatewayName = PaymillProvider.GATEWAY_NAME;
            paymentResponse.GatewayResponse = responseObj.data.response_code;
            paymentResponse.GatewayResult = Convert.ToInt32(paymentResponse.IsSuccess);
            paymentResponse.GatewayTransactionID = responseObj.data.id;

            var paymillId = new Guid(GATEWAY_ID);

            paymentResponse.GatewayID = paymillId;
            paymentResponse.PaymentProcessorID = paymillId;

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