using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Sitefinity.Modules.Ecommerce;
using Telerik.Sitefinity.Modules.Ecommerce.Orders;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.Fields;
using Telerik.Sitefinity.Web.UI.Fields.Contracts;
using Telerik.Sitefinity.Web.UI.Fields.Enums;

namespace Telerik.Sitefinity.Samples.Ecommerce.Paymill
{
    public class PaymillSettingsField : FieldControl
    {
        #region Constructor
        public PaymillSettingsField()
        {
            this.LayoutTemplatePath = layoutTemplatePath;
        }
        #endregion

        #region Properties

        public string ProviderName
        {
            get;
            set;
        }

        protected OrdersManager Manager
        {
            get
            {
                if (this.ordersManager == null)
                    this.ordersManager = OrdersManager.GetManager(this.ProviderName);
                return this.ordersManager;
            }
        }

        /// <summary>
        /// Gets the name of the embedded layout template.
        /// </summary>
        protected override string LayoutTemplateName
        {
            get
            {
                return PaymillSettingsField.layoutTemplateName;
            }
        }

        public override object Value
        {
            get
            {
                if (base.Value == null)
                    return Guid.Empty;
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        protected Guid PaymentMethodId
        {
            get
            {
                return (this.Value == null) ? Guid.Empty : (Guid)this.Value;
            }
            set
            {
                this.Value = value;
            }
        }
        #endregion

        #region Control references


        /// <summary>
        /// Gets the reference to the control that holds the TEST PRIVATE api key of the payment processor.
        /// </summary>
        protected virtual TextField TestPrivateApiKeyControl
        {
            get
            {
                return this.Container.GetControl<TextField>("testPrivateApiKey", true);
            }
        }

        /// <summary>
        /// Gets the reference to the control that holds the TEST PUBLIC api key of the payment processor.
        /// </summary>
        protected virtual TextField TestPublicApiKeyControl
        {
            get
            {
                return this.Container.GetControl<TextField>("testPublicApiKey", true);
            }
        }

        /// <summary>
        /// Gets the reference to the control that holds the LIVE PRIVATE api key of the payment processor.
        /// </summary>
        protected virtual TextField LivePrivateApiKeyControl
        {
            get
            {
                return this.Container.GetControl<TextField>("livePrivateApiKey", true);
            }
        }

        /// <summary>
        /// Gets the reference to the control that holds the LIVE PUBLIC api key of the payment processor.
        /// </summary>
        protected virtual TextField LivePublicApiKeyControl
        {
            get
            {
                return this.Container.GetControl<TextField>("livePublicApiKey", true);
            }
        }

        /// <summary>
        /// Gets the reference to the control that holds the timeout of the payment processor.
        /// </summary>
        protected virtual TextField Timeout
        {
            get
            {
                return this.Container.GetControl<TextField>("timeout", true);
            }
        }

        /// <summary>
        /// Gets the reference to the control that holds the payment type of the payment processor.
        /// </summary>
        protected virtual ChoiceField PaymentType
        {
            get
            {
                return this.Container.GetControl<ChoiceField>("paymentType", true);
            }
        }

        /// <summary>
        /// Gets the reference to the choice field control that provides available
        /// processing modes.
        /// </summary>
        protected virtual ChoiceField ProcessingMode
        {
            get
            {
                return this.Container.GetControl<ChoiceField>("processingMode", true);
            }
        }

        /// <summary>
        /// Gets the reference to the choice field control that provides list of
        /// credit cards to be processed.
        /// </summary>
        protected virtual ChoiceField ProcessorCreditCards
        {
            get
            {
                return this.Container.GetControl<ChoiceField>("processorCreditCards", true);
            }
        }

        /// <summary>
        /// Gets the reference to the control that represents the title of the field control.
        /// Return null if no such control exists in the template.
        /// </summary>
        /// <value>
        /// The control displaying the title of the field.
        /// </value>
        protected override WebControl TitleControl
        {
            get
            {
                return this.Container.GetControl<Label>("titleLabel", true);
            }
        }

        /// <summary>
        /// Gets the reference to the control that represents the description of the field control.
        /// Return null if no such control exists in the template.
        /// </summary>
        /// <value>
        /// The control displaying the description of the field.
        /// </value>
        protected override WebControl DescriptionControl
        {
            get
            {
                return this.Container.GetControl<Label>("descriptionLabel", this.DisplayMode == FieldDisplayMode.Write);
            }
        }

        #endregion

        #region Public and overriden controls

        /// <summary>
        /// Initializes the controls.
        /// </summary>
        /// <param name="container"></param>
        protected override void InitializeControls(GenericContainer container)
        {
            //((ITextControl)this.TitleControl).Text = this.Title;
            //((ITextControl)this.ExampleControl).Text = this.Example;
            ((ITextControl)this.DescriptionControl).Text = this.Description;
        }

        ///// <summary>
        ///// Initialize properties of the field implementing <see cref="IField"/>
        ///// with default values from the configuration definition object.
        ///// </summary>
        ///// <param name="definition">The definition configuration.</param>
        public override void Configure(IFieldDefinition definition)
        {
            base.Configure(definition);
            this.definition = definition;
        }

        /// <summary>
        /// Gets a collection of script descriptors that represent ECMAScript (JavaScript) client components.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerable"/> collection of <see cref="T:System.Web.UI.ScriptDescriptor"/> objects.
        /// </returns>
        public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptors = new List<ScriptDescriptor>(base.GetScriptDescriptors());
            var descriptor = (ScriptControlDescriptor)descriptors.Last();

            descriptor.AddComponentProperty("testPrivateApiKeyControl", this.TestPrivateApiKeyControl.ClientID);
            descriptor.AddComponentProperty("testPublicApiKeyControl", this.TestPublicApiKeyControl.ClientID);

            descriptor.AddComponentProperty("livePrivateApiKeyControl", this.LivePrivateApiKeyControl.ClientID);
            descriptor.AddComponentProperty("livePublicApiKeyControl", this.LivePublicApiKeyControl.ClientID);

            descriptor.AddComponentProperty("timeoutControl", this.Timeout.ClientID);
            descriptor.AddComponentProperty("processingModeControl", this.ProcessingMode.ClientID);
            descriptor.AddComponentProperty("paymentTypeControl", this.PaymentType.ClientID);

            descriptor.AddComponentProperty("processorCreditCardsControl", this.ProcessorCreditCards.ClientID);
            return descriptors;
        }

        /// <summary>
        /// Gets a collection of <see cref="T:System.Web.UI.ScriptReference"/> objects that define script resources that the control requires.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerable"/> collection of <see cref="T:System.Web.UI.ScriptReference"/> objects.
        /// </returns>
        public override IEnumerable<ScriptReference> GetScriptReferences()
        {
            var scripts = new List<ScriptReference>(base.GetScriptReferences());
            scripts.Add(new ScriptReference(PaymillSettingsField.scriptReference, typeof(PaymillSettingsField).Assembly.FullName));
            return scripts;
        }

        #endregion

        #region Private fields and constants

        internal const string scriptReference = "$assemblyname$.Paymill.PaymillSettingsField.js";
        private const string layoutTemplateName = "$assemblyname$.Paymill.PaymillSettingsField.ascx";
        internal static readonly string layoutTemplatePath = PaymillProvider.PAYMILL_VIRTUAL_PATH + layoutTemplateName;
        private IFieldDefinition definition;
        private OrdersManager ordersManager;

        #endregion
    }
}
