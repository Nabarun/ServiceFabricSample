using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace CleanCloudStorage
{
    public class DeleteContainer : IDeleteStorage
    {
        /// <summary>
        /// Delete a container by its Name. Normally this method is invoked from client.
        /// </summary>
        /// <param name="credentials">Gets the Azure Credential Details object</param>
        /// <param name="containerName">Gets the container Name</param>
        /// <returns></returns>
        public bool DeleteByName(AzureAccountCredDetails credentials, string containerName)
        {
            try
            {
                var account = new CloudStorageAccount(
                    new StorageCredentials(credentials.AzureStorageAccountName, credentials.AzureStorageAccountKey),
                    false);
                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(credentials.AzureContainerName);
                container.DeleteIfExists();

                return true;
            }
            catch (NullReferenceException nullException)
                when (credentials != null && (string.IsNullOrEmpty(credentials?.AzureStorageAccountName) &&
                                              string.IsNullOrEmpty(credentials.AzureStorageAccountName) &&
                                              string.IsNullOrEmpty(credentials.AzureStorageAccountKey)))
            {
                Trace.WriteLine("Null value in Azure Credential object or datetime limit. " +
                                $"Exception Message:{nullException.InnerException.Message}");
                throw;
            }
            catch (Exception exception)
            {
                Trace.WriteLine($"Exception see in DeleteByName section. Message: {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes the table entity based on datetime limit. This will normally be called from the service.
        /// </summary>
        /// <param name="credentials">Gets the Azure credential details object</param>
        /// <param name="limit">Gets the limit since when the user wants the service to delete entities</param>
        /// <returns></returns>
        public bool DeleteByTime(AzureAccountCredDetails credentials, DateTime limit)
        {
            try
            {
                var account = new CloudStorageAccount(
                    new StorageCredentials(credentials.AzureStorageAccountName, credentials.AzureStorageAccountKey),
                    false);
                var blobClient = account.CreateCloudBlobClient();
                var containers = blobClient.ListContainers();

                foreach (var container in containers)
                {
                    if (container != null)
                    {
                        if (container.Properties.LastModified < limit)
                        {
                            //Delete the container
                            container.DeleteAsync();
                        }
                    }
                }
                return true;
            }
            //For all null or empty values of credentials the code needs to trace the error properly
            catch (NullReferenceException nullException)
                when (credentials == null || limit == null)
            {
                Trace.WriteLine("Null value in Azure Credential object or datetime limit. " +
                                $"Exception Message:{nullException.InnerException.Message}");
                throw new ArgumentNullException($"{nameof(credentials)} or {nameof(limit)} is null");
            }
            catch (Exception exception)
            {
                Trace.WriteLine($"Exception see in DeleteByName section. Message: {exception.Message}");
                throw new Exception("Exception occue in DeleteByName");
            }
        }
    }
}
