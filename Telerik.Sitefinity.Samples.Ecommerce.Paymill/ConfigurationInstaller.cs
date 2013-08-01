using System;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Modules.Ecommerce.Orders.Configuration;

namespace Telerik.Sitefinity.Samples.Ecommerce.Paymill
{
    public class ConfigurationInstaller
    {

        /// <summary>
        /// Installs Paymill as a PaymentProcessor in the Sitefinity Configuration and adds a virtual path to it's local path.
        /// </summary>
        public static void PreApplicationStart()
        {
            Bootstrapper.Initialized += new EventHandler<ExecutedEventArgs>(ConfigurationInstaller.Bootstrapper_Initialized);
        }

        private static void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName != "RegisterRoutes" || !Bootstrapper.IsDataInitialized)
            {
                return;
            }

            InstallPaymillVirtualPath();
            InstallPaymillConfiguration();
        }

        private static void InstallPaymillVirtualPath()
        {
            var virtualPathConfig = Config.Get<VirtualPathSettingsConfig>();
            var key = PaymillProvider.PAYMILL_VIRTUAL_PATH + "*";
            if (!virtualPathConfig.VirtualPaths.ContainsKey(key))
            {
                var configManager = ConfigManager.GetManager();
                virtualPathConfig = configManager.GetSection<VirtualPathSettingsConfig>();
                virtualPathConfig.VirtualPaths.Add(new VirtualPathElement(virtualPathConfig.VirtualPaths)
                {
                    VirtualPath = key,
                    ResolverName = "EmbeddedResourceResolver",
                    ResourceLocation = "Telerik.Sitefinity.Samples.Ecommerce.Paymill"
                });
                configManager.SaveSection(virtualPathConfig);
            }
        }

        private static void InstallPaymillConfiguration()
        {
            var config = Config.Get<PaymentProcessorConfig>();
            var key = PaymillProvider.GATEWAY_NAME;
            if (!config.PaymentProcessorProviders.ContainsKey(key))
            {
                var configManager = ConfigManager.GetManager();
                config = configManager.GetSection<PaymentProcessorConfig>();
                config.PaymentProcessorProviders.Add(new PaymentProcessorProviderSettings(config.PaymentProcessorProviders)
                {
                    Id = PaymillProvider.GATEWAY_ID.ToString(),
                    Name = key,
                    Title = "Paymill",
                    IsActive = true,
                    EnableLogging = false,
                    Description = "Paymill Payment Proiveder",
                    SectionName = "PaymillSettingsSection",
                    SectionCssClass = "sf_paymentSettingsField sf_paymillPaymentSettingsField sfSectionInSection sfDisplayNone",
                    SettingsType = typeof(PaymillSettings),
                    ViewProviderType = typeof(PaymillSettingsField),
                    ProviderType = typeof(PaymillProvider)
                });
                configManager.SaveSection(config);
            }
        }
    }
}
