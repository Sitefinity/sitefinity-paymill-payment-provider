========================
Paymill Payment Provider
========================

Ivan Petrov, Sitefinity Team 1, 08/2013

# Overview
Sitefinity's Paymill Payment Provider is an implementation of an online payment processor. It's purpose is to ease the European clients of Sitefinity CMS, because the PaymentProcessors already implemented are mainly for the US ( they sell in USD and not in EUR. Also they require the merchants to be in the US). This document will show you how to install and use Paymill in Sitefinity. For more information about the Payment Processors in Sitefinity, please refer to Sitefinity's documentation at [this link.](http://www.sitefinity.com/documentation/documentationarticles/developers-guide/sitefinity-essentials/modules/ecommerce/payment-methods/payment-processors) For additional information about Paymill check out their official documentation on their website [here.](https://www.paymill.com/en-gb/)

## Setting up Paymill Payment Provider with NuGet
It is really fast and easy to set up Paymill using the NuGet package that is uploaded to NuGet.org. Here is the link to the project: [https://www.nuget.org/packages/SitefinityPaymillPaymentProcessor/](click!) The package is called "SitefinityPaymillPaymentProcessor". To set it up just follow these steps:
1. Download it and install it to your SitefinityWebApp or whatever WebApp that uses Sitefinity CMS from the NuGet Package Manager in Visual Studio. ( It is mandatory that you have saved the *.sln file that contains the WebApp )
2. Build/Rebuild the project.
3. Now you should have new Payment Processor in Settings -> Advanced -> PaymentProcessor -> PaymentProcessorProviders and you can use it to create a new Payment Method in the Ecommerce Module.

![Paymill Payment Processor](http://s14.postimg.org/6ddinsa3l/Payment_Method.png)

NB: To use Paymill you need to have an account in the Paymill Website: https://www.paymill.com/en-gb/ and use the API Keys that they provide for authentication.

![Paymill Payment Method](http://s14.postimg.org/6ddinsa3l/Payment_Method.png)

## Setting up Paymill Payment Provider Manually

Setting up Paymill from this project takse a little longer and a few more steps:

1. Download the project from GitHub
2. Place it in the same folder as your WebApp.
3. Add the project the the solution of the WebApp.
4. Add to the Paymill Project references to these 5 assemblies:
 * Newtonsoft.Json
 * Telerik.OpenAccess
 * Telerik.Sitefinity
 * Telerik.Sitefinity.Ecommerce
 * Telerik.Sitefinity.Model
5. Reference the Paymill Project from the WebApp.
6. Build/Rebuild the WebApp and the Paymill Project.
7. Now the static method InstallPaymillConfiguration() in the ConfigurationInstaller class should have installed you the new PaymentProcessorProvider in the Sitefinity Configuration Files.

## What does creating a new Payment Method with Paymill Payment Processor Provider requires:
To use this implementation you need to create ( if you don't already have ) account in Paymill's Official Website [https://www.paymill.com/en-gb/](here). When you do that you will have an TEST account and in order to make real transaction with it you will need to switch to LIVE mode. (To do that you'll have to provide them with information about you and your bank account and etc.) What will they give you is are two API Keys ( two for each transaction mode: test and live. TEST mode simulates LIVE transactions, but doesn't actually make them ). You would have to provide these API Keys to Sitefinity CMS, so that it can authenticate you when it sends request to Paymill's web server. 
* The Public API Key is used in the first request to Paymill for each transaction is made through your Sitefinity Website. It gives your clients information to Paymill ( Card number, CVC etc. ). 
* And the Private API Key is used in the second web request made to Paymill, which aims to close the transaction.