using System;
using Application.Seedwork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class Attendancelog
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Remarks { get; set; }
    }
}
