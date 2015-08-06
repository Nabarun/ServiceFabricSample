namespace CleanCloudStorage
{
    public class AzureAccountCredDetails
    {
        public string AzureStorageAccountName { get; }

        public string AzureStorageAccountKey { get; }

        public string AzureContainerName { get; }

        public string AzureStorageUri { get; }


        public AzureAccountCredDetails(string storageAccountName, string storageAccountKey, string containerName, string azureStorageUri)
        {
            this.AzureContainerName = storageAccountName;
            this.AzureStorageAccountKey = storageAccountKey;
            this.AzureStorageAccountName = containerName;
            this.AzureStorageUri = azureStorageUri;
        }

        public AzureAccountCredDetails(string storageAccountName, string storageAccountKey, string azureStorageUri)
        {
            this.AzureStorageAccountName = storageAccountName;
            this.AzureStorageAccountKey = storageAccountKey;
            this.AzureStorageUri = azureStorageUri;
        }

    }
}
