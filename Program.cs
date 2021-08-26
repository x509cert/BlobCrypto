using System;
using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

string keyName = "blobcrypto";
string kvUri = "<full URI to AKV>" + "/keys/" + keyName;

string storageConnStr = "BlobEndpoint=<full URI to blob store>";
string containerName = "crypto";
string encryptBlob = "encrypt.txt";
string localblobPath = @"C:\temp\123.txt";
string localblobPath2 = @"C:\temp\123-decrypt.txt";

var creds = new DefaultAzureCredential();

CryptographyClient cryptoClient = new (new Uri(kvUri), creds);
KeyResolver keyResolver = new (creds);

ClientSideEncryptionOptions encryptionOptions = new (ClientSideEncryptionVersion.V1_0)
{
    KeyEncryptionKey = cryptoClient,
    KeyResolver = keyResolver,
    KeyWrapAlgorithm = "RSA-OAEP"
};

BlobClientOptions options = new SpecializedBlobClientOptions() { ClientSideEncryption = encryptionOptions };
var blobContainerClient = new BlobServiceClient(storageConnStr, options).GetBlobContainerClient(containerName);
var blobClient = blobContainerClient.GetBlobClient(encryptBlob);

blobClient.Upload(localblobPath, true); // Encrypt on upload
blobClient.DownloadTo(localblobPath2);  // Decrypt on download
