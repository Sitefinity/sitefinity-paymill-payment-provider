using Telerik.Sitefinity.Ecommerce.Payment.Model;

namespace Telerik.Sitefinity.Samples.Ecommerce.Paymill
{
    public class PaymillSettings : IPaymentSettings
    {
        /// <summary>
        /// Returns test or live URL depending on value of TestMode property
        /// </summary>
        public string GatewayUrl
        {
            get
            {
                return TestMode ? TestGatewayUrl : LiveGatewayUrl;
            }
        }

        public string PrivateApiKey
        {
            get
            {
                return TestMode ? TestPrivateApiKey : LivePrivateApiKey;
            }
        }

        public string PublicApiKey
        {
            get
            {
                return TestMode ? TestPublicApiKey : LivePublicApiKey;
            }
        }

        /// <summary>
        /// The Gateway URL for Test mode
        /// </summary>
        public string TestGatewayUrl { get; set; }

        /// <summary>
        /// The Private Test API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string TestPrivateApiKey { get; set; }

        /// <summary>
        /// The Public Test API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string TestPublicApiKey { get; set; }

        /// <summary>
        /// The Gateway URL for Live mode
        /// </summary>
        public string LiveGatewayUrl { get; set; }

        /// <summary>
        /// The Private Live API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string LivePrivateApiKey { get; set; }

        /// <summary>
        /// The Public Live API Key provided by the Payment Processor ( Paymill ). 
        /// </summary>
        public string LivePublicApiKey { get; set; }

        /// <summary>
        /// Returns test or live URL depending on value of TestMode property
        /// </summary>
        public bool TestMode
        {
            get
            {
                return ProcessingMode.ToLower() == "live" ? false : true;
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
