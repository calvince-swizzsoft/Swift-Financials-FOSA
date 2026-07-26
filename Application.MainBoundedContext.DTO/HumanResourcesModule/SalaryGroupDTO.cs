using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class SalaryGroupDTO : BindingModelBase<SalaryGroupDTO>
    {
        public SalaryGroupDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public string salaryGroups { get; set; }

        //HashSet<SalaryGroupEntryDTO> _salaryGroupEntries;
        //[DataMember]
        //[Display(Name = "Salary Group Entries")]
        //public virtual ICollection<SalaryGroupEntryDTO> SalaryGroupEntries
        //{
        //    get
        //    {
        //        if (_salaryGroupEntries == null)
        //        {
        //            _salaryGroupEntries = new HashSet<SalaryGroupEntryDTO>();
        //        }
        //        return _salaryGroupEntries;
        //    }
        //    private set
        //    {
        //        _salaryGroupEntries = new HashSet<SalaryGroupEntryDTO>(value);
        //    }
        //}

        public List<SalaryGroupEntryDTO> SalaryGroupEntries { get; set; }
        public SalaryGroupEntryDTO SalaryGroupEntry { get; set; }
        



        //public List<SalaryGroupEntryDTO> sGroupEntries
        //{
        //    get
        //    {
        //        if (SalaryGroupEntries != null)
        //        {
        //            SalaryGroupEntryDTO salaryGroupEntryDTO = new SalaryGroupEntryDTO();

        //            List<ICollection> collections = new List<ICollection>((IEnumerable<ICollection>)SalaryGroupEntries);
        //        }

        //        return sGroupEntries
        //    }
        //    set
        //    {

        //    }
        //}

        //public SalaryGroupEntryDTO sGroupEntry { get; set; }
    }
}
