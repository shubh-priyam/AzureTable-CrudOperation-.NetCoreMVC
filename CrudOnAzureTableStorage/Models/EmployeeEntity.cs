using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudOnAzureTableStorage.Models
{
    public class EmployeeEntity : TableEntity
    {
        public EmployeeEntity()
        {
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }

    }
}
