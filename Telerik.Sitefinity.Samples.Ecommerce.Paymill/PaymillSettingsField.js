Type.registerNamespace("Telerik.Sitefinity.Samples.Ecommerce.Paymill");

Telerik.Sitefinity.Samples.Ecommerce.Paymill.PaymillSettingsField = function (element) {
    Telerik.Sitefinity.Samples.Ecommerce.Paymill.PaymillSettingsField.initializeBase(this, [element]);
    this._element = element;
    this._testPrivateApiKeyControl = null;
    this._testPublicApiKeyControl = null;

    this._element = element;
    this._livePrivateApiKeyControl = null;
    this._livePublicApiKeyControl = null;

    this._timeoutControl = null;
    this._paymentTypeControl = null;
    this._processingModeControl = null;
    this._processorCreditCardsControl = null;
    this._processingModeControlDelegate = null;

    this._state = null;
    this._appLoadedDelegate = null;
    this._appLoaded = false;
};

Telerik.Sitefinity.Samples.Ecommerce.Paymill.PaymillSettingsField.prototype = {
    initialize: function () {
        Telerik.Sitefinity.Samples.Ecommerce.Paymill.PaymillSettingsField.callBaseMethod(this, "initialize");

        if (this._appLoadedDelegate === null) {
            this._appLoadedDelegate = Function.createDelegate(this, this._applicationLoaded);
        }

        if (this._processingModeControl) {
            this._processingModeControlDelegate = Function.createDelegate(this, this._processingModeControlHandler);
            this._processingModeControl.add_valueChanged(this._processingModeControlDelegate);
        }

        Sys.Application.add_load(this._appLoadedDelegate);
    },

    dispose: function () {
        Telerik.Sitefinity.Samples.Ecommerce.Paymill.PaymillSettingsField.callBaseMethod(this, "dispose");

        if (this._appLoadedDelegate) {
            Sys.Application.remove_load(this._appLoadedDelegate);
            delete this._appLoadedDelegate;
        }
    },

    /* --------------------  public methods ----------- */

    // this method is called by definitions engine just before saving. It should return true
    // if value is valid; otherwise false. As our field control is composed of several other
    // field controls, we will validate them (remember, both this control and child controls
    // inherit base FieldControl JS component.
    validate: function () {
        if (!$(this.get_element()).is(':visible')) {
            return true;
        }

        // if only one of the child field controls fails, this (parent/container) field control
        // should fail as well
        var isValid = true;

        if (!this.get_timeoutControl().validate()) {
            isValid = false;
        }

        if (!this.get_processorCreditCardsControl().validate()) {
            isValid = false;
        }

        // here you can also perform other validations that could depend
        // on the values of multiple fields, so you have a very extensible
        // validation meschanism

        if (this.get_processingModeControl().get_value() == "live") {

            if (!this.get_livePrivateApiKeyControl().validate()) {
                isValid = false;
            }

            if (!this.get_livePublicApiKeyControl().validate()) {
                isValid = false;
            }
        }
        else {

            if (!this.get_testPrivateApiKeyControl().validate()) {
                isValid = false;
            }

            if (!this.get_testPublicApiKeyControl().validate()) {
                isValid = false;
            }
        }

        // we return the aggregate result of all our validations
        return isValid;
    },

    reset: function () {
        // Setup default values
        // ProcessorCreditCards is set to undefined so updateUI handles both the 'New' and 'Edit' cases correctly.
        this._state = { 'TestPrivateApiKey': '', 'TestPublicApiKey': '', 'LivePrivateApiKey': '', 'LivePublicApiKey': '', 'Timeout': '30000', 'ProcessingMode': '', 'ProcessorCreditCards': undefined, 'PaymentType': '' };
    },

    // Gets the value of the field control.
    get_value: function () {
        return Sys.Serialization.JavaScriptSerializer.serialize(this._getSettingsObject());
    },

    // Sets the value of the text field control depending on DisplayMode.
    set_value: function (value) {
        if (value !== null && value.length > 0) {
            var settingsObject = Sys.Serialization.JavaScriptSerializer.deserialize(value);
            this._state = settingsObject;
        }

        // as application load event happens only once during the lifetime of the page, thus
        // it won't happen the next time we open the detail form for creating or updating
        // payment methods, we need to implement dual handling for updating user interface.
        // a) at the very first time we have to assume that not all components have been
        // loaded, so we are persisting data in the state and relying on the applicationLoaded
        // handler to call the _updateUI method
        // b) on subsequent usage of the dialog, we now that controls have been ready and application
        // load event wont' be fired again (as it was loaded), therefore we updated the ui from the
        // set_value method. 
        // p.s. our script combining / compressing mechanism will remove this lenghty comment for the
        // release build, so don't worry commeting scripts extensively - it does not slow down
        // the application
        if (this._appLoaded) {
            this._updateUI();
        }
    },

    // Returns true if the value of the field is changed
    isChanged: function () {
        // TODO: implement
    },

    /* --------------------------------- event handlers ---------------------------------- */

    _applicationLoaded: function (sender, agrs) {
        this._appLoaded = true;
        this._updateUI();
    },

    _processingModeControlHandler: function (e) {
        $('.sf_paymillPaymentMode').hide();
        if (this.get_processingModeControl().get_value() == 'test') {
            $('.sf_paymillTestMode').show();
        }
        else {
            $('.sf_paymillLiveMode').show();
        }
    },

    /* --------------------------------- private methods --------------------------------- */

    // this method updates the user interface with the values from the state
    _updateUI: function () {
        this.get_testPrivateApiKeyControl().set_value(this._state.TestPrivateApiKey);
        this.get_testPublicApiKeyControl().set_value(this._state.TestPublicApiKey);

        this.get_livePrivateApiKeyControl().set_value(this._state.LivePrivateApiKey);
        this.get_livePublicApiKeyControl().set_value(this._state.LivePublicApiKey);

        this.get_timeoutControl().set_value(this._state.Timeout);
        this.get_processingModeControl().set_value(this._state.ProcessingMode);

        // By default we want all credit cards to be checked. We made the reset method set the state's ProcessorCreditCards to undefined.
        var creditCardValues = this._state.ProcessorCreditCards;

        if (creditCardValues === undefined) {
            creditCardValues = $.map(this.get_processorCreditCardsControl()._choices, function (choice) {
                return choice.Value;
            });
        }
        this.get_processorCreditCardsControl().set_value(creditCardValues);

        this.get_paymentTypeControl().set_value(this._state.PaymentType);

        $('.sf_paymillPaymentMode').hide();
        if (this.get_processingModeControl().get_value() == 'test') {
            $('.sf_paymillTestMode').show();
        }
        else {
            $('.sf_paymillLiveMode').show();
        }
    },

    _getSettingsObject: function () {
        var creditCards = this.get_processorCreditCardsControl().get_value();
        if (!this._isArray(creditCards)) {
            creditCards = [creditCards];
        }

        var settings = {
            'TestPrivateApiKey': this.get_testPrivateApiKeyControl().get_value(),
            'TestPublicApiKey': this.get_testPublicApiKeyControl().get_value(),
            'LivePrivateApiKey': this.get_livePrivateApiKeyControl().get_value(),
            'LivePublicApiKey': this.get_livePublicApiKeyControl().get_value(),
            'Timeout': this.get_timeoutControl().get_value(),
            'ProcessingMode': this.get_processingModeControl().get_value(),
            "ProcessorCreditCards": creditCards,
            'PaymentType': this.get_paymentTypeControl().get_value()
        };
        return settings;
    },

    // a more compact version
    _isArray: function (obj) {
        return (obj.constructor.toString().indexOf('Array') != -1);
    },

    /* --------------------------------- properties -------------------------------------- */

    // gets the reference to the choice field which holds credit cards to be processed
    get_processorCreditCardsControl: function () {
        return this._processorCreditCardsControl;
    },
    // gets the reference to the choice field which holds credit cards to be processed
    set_processorCreditCardsControl: function (value) {
        this._processorCreditCardsControl = value;
    },

    // gets the reference to the choice field which holds available processing modes
    get_processingModeControl: function () {
        return this._processingModeControl;
    },
    // sets the reference to the choice field which holds available processing modes
    set_processingModeControl: function (value) {
        this._processingModeControl = value;
    },

    // gets the reference to the control that holds the Private Api Key in Test Mode
    get_testPrivateApiKeyControl: function () {
        return this._testPrivateApiKeyControl;
    },

    // sets the reference to the control that holds the Private Api Key in Test Mode
    set_testPrivateApiKeyControl: function (value) {
        this._testPrivateApiKeyControl = value;
    },

    // gets the reference to the control that holds the Public Api Key in Test Mode
    get_testPublicApiKeyControl: function () {
        return this._testPublicApiKeyControl;
    },

    // sets the reference to the control that holds the Public Api Key in Test Mode
    set_testPublicApiKeyControl: function (value) {
        this._testPublicApiKeyControl = value;
    },

    // gets the reference to the control that holds the Private Api Key in Live Mode
    get_livePrivateApiKeyControl: function () {
        return this._livePrivateApiKeyControl;
    },

    // sets the reference to the control that holds the Private Api Key in Live Mode
    set_livePrivateApiKeyControl: function (value) {
        this._livePrivateApiKeyControl = value;
    },

    // gets the reference to the control that holds the Public Api Key in Live Mode
    get_livePublicApiKeyControl: function () {
        return this._livePublicApiKeyControl;
    },

    // sets the reference to the control that holds the Public Api Key in Live Mode
    set_livePublicApiKeyControl: function (value) {
        this._livePublicApiKeyControl = value;
    },

    // gets the reference to the control that holds the timeout of the payment processor
    get_timeoutControl: function () {
        return this._timeoutControl;
    },
    // sets the reference to the control that holds the timeout of the payment processor
    set_timeoutControl: function (value) {
        this._timeoutControl = value;
    },

    // gets the reference to the control that holds the payment type of the payment processor
    get_paymentTypeControl: function () {
        return this._paymentTypeControl;
    },
    // sets the reference to the control that holds the payment type of the payment processor
    set_paymentTypeControl: function (value) {
        this._paymentTypeControl = value;
    }
};

Telerik.Sitefinity.Samples.Ecommerce.Paymill.PaymillSettingsField.registerClass("Telerik.Sitefinity.Samples.Ecommerce.Paymill.PaymillSettingsField", Telerik.Sitefinity.Web.UI.Fields.FieldControl);