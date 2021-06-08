using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudOnAzureTableStorage.Models
{
    public class EmployeeModel
    {
        public string Department { get; set; }
        public string EmpId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }

    }
}
