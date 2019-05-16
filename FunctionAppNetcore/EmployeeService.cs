using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FunctionAppNetcore;

namespace FunctionAppTest
{
    public class EmployeeService
    {
        public async Task<List<Employee>> GetEmployeeInformation()
        {
           List<Employee> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://dummy.restapiexample.com/api/v1/");
                //HTTP GET
                var responseTask = await client.GetAsync("employees");

                if (responseTask.IsSuccessStatusCode)
                {

                  string json = await responseTask.Content.ReadAsStringAsync();
                  list = JsonConvert.DeserializeObject<List<Employee>>(json);
                  return list;


                }
            }

            return list;


        }


    }
}
