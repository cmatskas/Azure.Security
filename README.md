Azure.Security
==============

An encryption provider that has been designed to work around the shortcomings of the Windows Azure platform.

The fundamental problem arises from the fact that Azure Websites run on a shared environment where managing encryption key is hard to achieve without introducing security weaknesses. Although some people may suggest that the proposed implementation is not perfect and there are still small pitfalls, at least you have a solid, pluggable and easy to configure Encryption provider for your Azure Websites. The Encryption helper uses a combination of SSL keys, AES Cryptographic keys, Blob and Table storage to create and manage the keys necessary fo encrypt and decrypt data.

In order to get the Encryption provider to work, your project will need to provide an SSL key. This doesn't have to be a commercial key that you acquire from a vendor, so even self-signed keys work just fine.

I will provide full details on how to set up and call the EncryptionHelper but in the meantime, if you cannot wait for the blog post and instructions, feel free to dive into the unit tests and have a go yourselves.

