using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

// string connectionString = ConnectionString;
// string containerName = Randomize("sample-container");
// string blobName = Randomize("sample-file");
// string filePath = CreateTempFile(SampleFileContent);
#region Snippet:SampleSnippetsBlob_Upload
// Get a connection string to our Azure Storage account.  You can
// obtain your connection string from the Azure Portal (click
// Access Keys under Settings in the Portal Storage account blade)
// or using the Azure CLI with:
//
//     az storage account show-connection-string --name <account_name> --resource-group <resource_group>
//
// And you can provide the connection string to your application
// using an environment variable.

//@@ string connectionString = "<connection_string>";
//@@ string containerName = "sample-container";
//@@ string blobName = "sample-blob";
//@@ string filePath = "sample-file";

// Get a reference to a container named "sample-container" and then create it
BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
container.Create();

// Get a reference to a blob named "sample-file" in a container named "sample-container"
BlobClient blob = container.GetBlobClient(blobName);

// Upload local file
blob.Upload(filePath);
#endregion

// Assert.AreEqual(1, container.GetBlobs().Count());
BlobProperties properties = blob.GetProperties();
// Assert.AreEqual(SampleFileContent.Length, properties.ContentLength);