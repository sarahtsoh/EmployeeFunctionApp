using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.IO;
using Function.Helper;

namespace FunctionAppNetcore
{
    static class Helper
    {
        public static async Task UploadToAzure(IConfigurationRoot config, Employee employee)
        {
            var AzureStorageConnectionString = config.GetSection("AzureStorageConfig:StorageConnectionString").Value;
            var AzureStorageContainer = config.GetSection("AzureStorageConfig:Container").Value;

            string output = JsonConvert.SerializeObject(employee);
            byte[] byteArray = Encoding.UTF8.GetBytes(output);
            MemoryStream stream = new MemoryStream(byteArray);

            await AzureStorageHelper.UploadFileAsBlob(AzureStorageConnectionString, AzureStorageContainer, "", $"employeeid{employee.id}.json", stream);


        }


        static IConfigurationRoot GetConfig(ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
            return config;
        }


    }
}
