using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace CleanCloudStorage
{
    public class DeleteTable : IDeleteStorage
    {
        /// <summary>
        /// Delete a Table with its name. This is method is exposed for clients to connect to the service and delete specific table
        /// </summary>
        /// <param name="credentials">Gets the Azure Accoutn Credential Details Object</param>
        /// <param name="tableName">Gets the Table Name</param>
        /// <returns></returns>
        public bool DeleteByName(AzureAccountCredDetails credentials, string tableName)
        {
            try
            {
                var account = new CloudStorageAccount(
                    new StorageCredentials(credentials.AzureStorageAccountName, credentials.AzureStorageAccountKey),
                    false);
                var tableClient = account.CreateCloudTableClient();

                var table = tableClient.GetTableReference(tableName);
                table.DeleteIfExists();
                return true;
            }
            catch (ArgumentNullException nullException) when (tableName == null && credentials == null)
            {
                Trace.WriteLine($"Null Exception with message {nullException.Message} thrown because of either table name or " +
                                $"credentials object equal to null");
                throw new ArgumentNullException($"{nameof(credentials)} or {nameof(tableName)} is null");
            }
            catch (Exception exception)
            {
                Trace.WriteLine($"Exception thrown with message {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// This method will be invoked whenever service is running.This willdo the cleanup of old entities
        /// </summary>
        /// <param name="credentials">Gets the Azure credentials</param>
        /// <param name="limit">Gets the limit since when you want the service to do cleanup</param>
        /// <returns>Returns if the delete operation was succesful or not</returns>
        public bool DeleteByTime(AzureAccountCredDetails credentials, DateTime limit)
        {
            try
            {
                var account = new CloudStorageAccount(
                    new StorageCredentials(credentials.AzureStorageAccountName, credentials.AzureStorageAccountKey),
                    false);
                var tableClient = account.CreateCloudTableClient();
                var tables = tableClient.ListTables();

                var query =
                    new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterConditionForDate("Timestamp",
                        QueryComparisons.LessThanOrEqual, limit));

                foreach (var cloudTable in tables)
                {
                    foreach (var entity in cloudTable.ExecuteQuery(query))
                    {
                        //Delete the entity
                        cloudTable.Execute(TableOperation.Delete(entity));
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Trace.WriteLine($"Exception thrown with message {exception.Message}");
                throw;
            }

        }
    }
}
