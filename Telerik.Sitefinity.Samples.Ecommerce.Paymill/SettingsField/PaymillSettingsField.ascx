<%@ Control %>
<%@ Register Namespace="Telerik.Sitefinity.Web.UI.Fields" Assembly="Telerik.Sitefinity" TagPrefix="sitefinity" %>

<asp:Label id="titleLabel" runat="server" CssClass="sfTxtLbl"></asp:Label>
<asp:Label id="exampleLabel" runat="server" CssClass="sfExample"></asp:Label>

<div class="sfForm">
    <div class="sfFormIn">

    <sitefinity:ChoiceField ID="processorCreditCards" DisplayMode="Write" RenderChoicesAs="CheckBoxes" runat="server" CssClass="sfCheckListBox" Title='<%$Resources:OrdersResources, PaymentMethodCreditCards %>'>
        <Choices>
            <sitefinity:ChoiceItem Text="American Express" Value="americanexpress" />
            <sitefinity:ChoiceItem Text="Mastercard" Value="mastercard" />
            <sitefinity:ChoiceItem Text="Visa" Value="visa" />
            <sitefinity:ChoiceItem Text="Maestro" Value="maestro" />
            <sitefinity:ChoiceItem Text="Diners Club" Value="dinersclub" />
            <sitefinity:ChoiceItem Text="JCB" Value="jcb" />
        </Choices>
        <ValidatorDefinition Required="true"
                                RequiredViolationMessage="<%$Resources:OrdersResources, PaymentMethodCreditCardsRequired %>"
                                MessageCssClass="sfError" />
    </sitefinity:ChoiceField>

        <sitefinity:ChoiceField id="processingMode" DisplayMode="Write" RenderChoicesAs="RadioButtons" runat="server" CssClass="sfRadioList" Title="<%$Resources:OrdersResources, Mode %>">            
            <Choices>
                <sitefinity:ChoiceItem Text="<%$Resources:OrdersResources, PaymentMethodTestMode %>" Selected="true" Value="test"  />
                <sitefinity:ChoiceItem Text="<%$Resources:OrdersResources, PaymentMethodLiveMode %>" Value="live" />
            </Choices>        
        </sitefinity:ChoiceField>

        <div class="sf_paymillPaymentMode sf_paymillLiveMode">
            <sitefinity:TextField ID="liveGatewayUrl" runat="server" CssClass="sfShortField350" DisplayMode="Write" Title='Live Gateway URL'>
                <ValidatorDefinition Required="True"
                                        RequiredViolationMessage="The Live Gateway Url is Required"
                                        ExpectedFormat="Custom" RegularExpression = "^((((ht|f)tp(s?))\://){1}\S+)$"
                                        RegularExpressionViolationMessage="<%$Resources:OrdersResources, InternetUrlViolationMessage %>"
                                        MessageCssClass="sfError" />
            </sitefinity:TextField>
            

            <sitefinity:TextField ID="livePrivateApiKey" runat="server" CssClass="sfShortField180" DisplayMode="Write" Title='Live Private API Key'>
                <ValidatorDefinition Required="True" 
                                        RequiredViolationMessage="The Live Private API Key is Required"
                                        MessageCssClass="sfError" />
            </sitefinity:TextField>

            <sitefinity:TextField ID="livePublicApiKey" runat="server" CssClass="sfShortField180" DisplayMode="Write" Title='Live Public API Key'>
                <ValidatorDefinition Required="True" 
                                        RequiredViolationMessage="The Live Public API Key is Required"
                                        MessageCssClass="sfError" />
            </sitefinity:TextField>
        </div>  

        <div class="sf_paymillPaymentMode sf_paymillTestMode">
            <sitefinity:TextField ID="testGatewayUrl" runat="server" CssClass="sfShortField350" DisplayMode="Write" Title='Test Gateway URL'>
                <ValidatorDefinition Required="True"
                                        RequiredViolationMessage="The Test Gateway Url is Required"
                                        ExpectedFormat="Custom" RegularExpression = "^((((ht|f)tp(s?))\://){1}\S+)$"
                                        RegularExpressionViolationMessage="<%$Resources:OrdersResources, InternetUrlViolationMessage %>"
                                        MessageCssClass="sfError" />
            </sitefinity:TextField>

            <sitefinity:TextField ID="testPrivateApiKey" runat="server" CssClass="sfShortField180" DisplayMode="Write" Title='Test Private API Key'>
                <ValidatorDefinition Required="True" 
                                        RequiredViolationMessage="The Test Private API Key is Required"
                                        MessageCssClass="sfError" />
            </sitefinity:TextField>

            <sitefinity:TextField ID="testPublicApiKey" runat="server" CssClass="sfShortField180" DisplayMode="Write" Title='Test Public API Key'>
                <ValidatorDefinition Required="True" 
                                        RequiredViolationMessage="The Test Public API Key is Required"
                                        MessageCssClass="sfError" />
            </sitefinity:TextField>
        </div>
    </div>
</div>

<div class="sfForm">
    <ul class="sfFormIn">
        <sitefinity:TextField ID="timeout" runat="server" DisplayMode="Write" Title="<%$Resources:OrdersResources, PaymentMethodTimeout %>" CssClass="sfShortField80" WrapperTag="li">
            <ValidatorDefinition Required="false" ExpectedFormat="Numeric" 
                                    NumericViolationMessage="<%$Resources:OrdersResources, PaymentMethodTimeoutViolationMessage %>"
                                    MessageCssClass="sfError" />
        </sitefinity:TextField>
        <sitefinity:ChoiceField ID="paymentType" DisplayMode="Write" RenderChoicesAs="DropDown" runat="server" Title='<%$Resources:OrdersResources, PaymentMethodPaymentType %>' CssClass="sfRadioList" WrapperTag="li">            
            <Choices>
                <sitefinity:ChoiceItem Text="<%$Resources:OrdersResources, PaymentTypeSale %>" Selected="true" Value="sale"  />
            </Choices>      
        </sitefinity:ChoiceField>
    </ul>
</div>
<asp:Label id="descriptionLabel" runat="server" CssClass="sfDescription"></asp:Label>
    
<asp:HiddenField ID="paymentMethodId" runat="server" />