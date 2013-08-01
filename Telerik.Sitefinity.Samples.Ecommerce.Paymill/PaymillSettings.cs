using Telerik.Sitefinity.Ecommerce.Payment.Model;

namespace Telerik.Sitefinity.Samples.Ecommerce.Paymill
{
    public class PaymillSettings : IPaymentSettings
    {
        /// <summary>
        /// Returns the Private Api Key depending on the value of TestMode
        /// </summary>
        public string PrivateApiKey
        {
            get
            {
                return this.TestMode ? this.TestPrivateApiKey : this.LivePrivateApiKey;
            }

            set
            { 
                // The empty set method on some properties are because of the WCF serializer ( it needs them to work properly )
            }
        }

        /// <summary>
        /// Returns the Public Api Key depending on the value of TestMode
        /// </summary>
        public string PublicApiKey
        {
            get
            {
                return this.TestMode ? this.TestPublicApiKey : this.LivePublicApiKey;
            }

            set
            {
            }
        }

        /// <summary>
        /// The Private Test API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string TestPrivateApiKey { get; set; }

        /// <summary>
        /// The Public Test API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string TestPublicApiKey { get; set; }

        /// <summary>
        /// The Private Live API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string LivePrivateApiKey { get; set; }

        /// <summary>
        /// The Public Live API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string LivePublicApiKey { get; set; }

        /// <summary>
        /// Returns the Mode of the payment provider.
        /// </summary>
        public bool TestMode
        {
            get
            {
                return this.ProcessingMode.ToLower() == "live" ? false : true;
            }

            set
            { 
            }
        }

        /// <summary>
        /// Processing Mode ("test", "live")
        /// </summary>
        public string ProcessingMode { get; set; }

        /// <summary>
        /// Timeout (in milliseconds) for post to payment processor
        /// </summary>
        public double Timeout { get; set; }

        /// <summary>
        /// Array of strings that are names of credit cards (lower case) processed by the payment processor
        /// </summary>
        public string[] ProcessorCreditCards { get; set; }

        /// <summary>
        /// Payment type (sale, auth, capture, auth and capture, etc)
        /// </summary>
        public string PaymentType { get; set; }
    }
}
