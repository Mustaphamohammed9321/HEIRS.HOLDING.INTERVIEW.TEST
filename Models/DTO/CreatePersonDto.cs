using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HEIRS.HOLDING.INTERVIEW.TEST.Models.DTO
{
    public class CreatePersonDto
    {
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string CourseId { get; set; }
        public double Grade { get; set; }

    }


    public class CreatePersonListDto
    {
        public List<CreatePersonDto> Persons { get; set; }
    }


}