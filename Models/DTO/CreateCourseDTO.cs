using HEIRS.HOLDING.INTERVIEW.TEST.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HEIRS.HOLDING.INTERVIEW.TEST.Models.DTO
{
    public class CreateCourseDTO
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
    }


    public class CreateBulkCourseDto
    {
        public List<CreateCourseDTO> Courses { get; set; }
    }


}