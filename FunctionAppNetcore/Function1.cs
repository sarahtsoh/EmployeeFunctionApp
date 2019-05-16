using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionAppTest;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Function.Helper;
using System.Linq;
using System.Text;

namespace FunctionAppNetcore
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log , ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var config = GetConfig(context);
           
            //await AzureStorageHelper.UploadFileAsBlob(_azureStorageConfig.StorageConnectionString,
            //_azureStorageConfig.Container, "", $"temp_{transactionCode}.json", tempData, null);

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var employeeService = new EmployeeService();
            var list = await employeeService.GetEmployeeInformation();
            var employee = list.Where(e => e.employee_name == name).First();

            var AzureStorageConnectionString = config.GetSection("AzureStorageConfig:StorageConnectionString").Value;
            var AzureStorageContainer = config.GetSection("AzureStorageConfig:Container").Value;

            string output = JsonConvert.SerializeObject(list);
            byte[] byteArray = Encoding.UTF8.GetBytes(output);
            MemoryStream stream = new MemoryStream(byteArray);

            await AzureStorageHelper.UploadFileAsBlob(AzureStorageConnectionString, AzureStorageContainer, "", $"employeeid{employee.id}.json", stream);

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}, your employeeid is {employee.id}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
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
