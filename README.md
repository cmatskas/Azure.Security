Azure.Security
==============

[![Join the chat at https://gitter.im/cmatskas/Azure.Security](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cmatskas/Azure.Security?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

**Build Status** : [![Build status](https://ci.appveyor.com/api/projects/status/fyb9bs6e2d8w8xtn)](https://ci.appveyor.com/project/cmatskas/azure-security)

A C# encryption provider designed specifically to work around the shortcomings of the Windows Azure platform.

The fundamental problem arises from the fact that Azure Websites run on a shared environment where managing encryption keys is hard to achieve without introducing security weaknesses. Although some people may suggest that the proposed implementation is not perfect and there are still small pitfalls, at least you have a solid, pluggable and easy to configure Encryption provider for your Azure Websites. The Encryption helper uses a combination of SSL keys, AES Cryptographic keys, Blob and Table storage to create and manage the keys necessary to encrypt and decrypt data.

In order to get the Encryption provider to work, your project will need to provide an SSL key. This doesn't have to be a commercially acquired SSL key so even self-signed keys can do the job.

For full details about the project and how to install and use Azure.Security you can go [here](https://cmatskas.com/a-c-encryption-provider-for-azure-websites/)

## Quick Guide

1. Add the SSL certificate file to your solution
2. Create a storage account in Azure, if you don't have one already
3. Install the Azure.Security Nuget package
4. Configure the web.config variables accordingly

In your code, instantiate an instance of EncryptionHelper in your startup routine in order to generate the necessary table and blob container.

Example in Global.asax
```
protected void Application_Start()
{
 	//other code omitted
 
 	var certificatePath = Server.MapPath("~/App_Data");
 	var encryptionHelper = new EncryptionHelper(certificatePath);
 	encryptionHelper.CreateNewCryptoKeyIfNotExists();
}
```

Then, whenever you want to encrypt or decrypt data, just instantiate an EncryptionHelper class and call the appropriate methods.

Example
```
// ... other code omitted
var encryptionHelper = new EncryptionHelper("pathToCertificateFile");
var encryptedString = encryptionHelper.EncryptAndBase64("test string");

var decryptedString = encryptionHelper.DecryptFromBase64(encryptedString);
```

The EncryptionHelper can encrypt/decrypt strings and byte[] (i.e. streams)

For more details, you can either visit my blog post or dive into the unit tests.

