using CrudOnAzureTableStorage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrudOnAzureTableStorage.Controllers
{
    public class HomeController : Controller
    {
        public readonly IConfiguration Configuration;

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            var table = GetTable();
            TableQuery<EmployeeEntity> tableQuery = new TableQuery<EmployeeEntity>().Take(1);
            TableContinuationToken token = null;
            do
            {
                var res = await table.ExecuteQuerySegmentedAsync(tableQuery, token);
                token = res.ContinuationToken;
                //employees.AddRange(res.Results);
                var entity = new EmployeeModel()
                {
                    Department = res.Results[0].PartitionKey,
                    EmpId = res.Results[0].RowKey,
                    Name = res.Results[0].Name,
                    Email = res.Results[0].Email,
                    Domain = res.Results[0].Domain
                };
                employees.Add(entity);
            }
            while (token != null);
            
            return View(employees);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeModel employee)
        {
            if (ModelState.IsValid)
            {
                var table = GetTable();
                var entity = new EmployeeEntity()
                {
                    PartitionKey = employee.Department,
                    RowKey = employee.EmpId,
                    Name = employee.Name,
                    Email = employee.Email,
                    Domain = employee.Domain
                };

                TableOperation insertOp = TableOperation.InsertOrReplace(entity);
                await table.ExecuteAsync(insertOp);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id, string partition)
        {
            var table = GetTable();
            TableOperation tableOperation = TableOperation.Retrieve<EmployeeEntity>(partition, id);
            var res = await table.ExecuteAsync(tableOperation);
            EmployeeEntity employeeEntity = (EmployeeEntity)res.Result;
            var entity = new EmployeeModel()
            {
                Department = employeeEntity.PartitionKey,
                EmpId =  employeeEntity.RowKey,
                Name =   employeeEntity.Name,
                Email =  employeeEntity.Email,
                Domain = employeeEntity.Domain
            };
            return View("Create", entity);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeModel employee)
        {
            if (ModelState.IsValid)
            {
                var table = GetTable();
                TableOperation tableOperation = TableOperation.Retrieve<EmployeeEntity>(employee.Department, employee.EmpId);
                var res = await table.ExecuteAsync(tableOperation);
                EmployeeEntity employeeEntity = (EmployeeEntity)res.Result;

                employeeEntity.Name = employee.Name;
                employeeEntity.Email = employee.Email;
                employeeEntity.Domain = employee.Domain;


                TableOperation insertOp = TableOperation.Replace(employeeEntity);
                await table.ExecuteAsync(insertOp);
                return RedirectToAction(nameof(Index));
            }
            return View("Create", employee);
        }

        public async Task<IActionResult> Delete(string id, string partition)
        {
            // Retrieve the object to Delete
            var table = GetTable();
            TableOperation tableOperation = TableOperation.Retrieve<EmployeeEntity>(partition, id);
            var res = await table.ExecuteAsync(tableOperation);
            EmployeeEntity employeeEntity = (EmployeeEntity)res.Result;
            var name = employeeEntity.Name;
            var tr = await table.ExecuteAsync(TableOperation.Delete(employeeEntity));

            return RedirectToAction(nameof(Index));
        }

        #region private method
        private CloudTable GetTable()
        {
            string accessKey = Configuration.GetValue<string>("AzureTableStorage:AccessKey");
            string tableName = Configuration.GetValue<string>("AzureTableStorage:TableName");

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudTableClient cloudtableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable table = cloudtableClient.GetTableReference(tableName);
            table.CreateIfNotExistsAsync();
            return table;
        }
        #endregion
    }
}
