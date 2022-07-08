using HEIRS.HOLDING.INTERVIEW.TEST.DataContext;
using HEIRS.HOLDING.INTERVIEW.TEST.Models;
using HEIRS.HOLDING.INTERVIEW.TEST.Models.DTO;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Course = HEIRS.HOLDING.INTERVIEW.TEST.DataContext.Course;

namespace HEIRS.HOLDING.INTERVIEW.TEST.Controllers
{
    [RoutePrefix("api")]

    public class CourseController : ApiController
    {
        
        //protected static string containerName = ConfigurationManager.AppSettings["ContainerName"].ToString();
        protected static string _connectionString = ConfigurationManager.ConnectionStrings["HeirsConnectionString"].ConnectionString.ToString();
        

        public CourseController()
        {

        }

        List<Course> courses = new List<Course>();  
        public CourseController(List<Course> courses)
        {
            this.courses = courses;
        }


        [HttpGet, Route("GetAllCourses")]
        public async Task<IHttpActionResult> GetAllCourses()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    using (var ctx = new heirs_dbEntities())
                    {
                        var res = await ctx.Courses.ToListAsync();
                        return Ok(new { data = res != null ? res : null, Message = $"{res.Count()} Record(s) Found.", responseMessage = "Command(s) Completed Successfully" });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
          
        }


        [HttpPost, Route("AddCourses")]
        public async Task<IHttpActionResult> AddCourses([FromBody] CreateBulkCourseDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    //check thru a list of courses sent for submission
                    // if a duplicate is found, alert us
                    // else add all the record to db
                    
                    //NB: another approach is to get the list of all the courses first before looping thru them

                    using (var ctx = new heirs_dbEntities())
                    {
                        //check if a course has been added before
                        //while looping, store the count of records that already exists
                        int countDuplicate = 0;
                        List<Course> noDuplicateRecord = new List<Course>();
                        requestDto.Courses.ForEach(c =>
                        { 
                            var checkRes =   ctx.Courses.Where(w => w.id == c.CourseId /*|| w.CourseId.ToLower().Equals(c.CourseId.ToLower())*/).FirstOrDefault();
                            if (checkRes != null) //count the number of record(s) that already exists in the db
                            {
                                countDuplicate += 1; 
                            }
                            else
                            {
                                //create a new Course Model to store the ones that are not duplicated
                                noDuplicateRecord.Add(new Course
                                {
                                    id = c.CourseId,
                                    name = c.CourseName,
                                });
                            }
                        });

                        if (noDuplicateRecord != null && noDuplicateRecord.Count() > 0)
                        {
                            //then add the list to db
                            ctx.Courses.AddRange(noDuplicateRecord);
                            await ctx.SaveChangesAsync();
                        }

                        if (countDuplicate > 0)
                            return Ok(new { responseMessage = $"{requestDto.Courses.Count() - countDuplicate} course(s) added successfully, Reason: We found {countDuplicate} duplicates, and they were ommited while creating new courses.", responseCode = "90" });
                        return Ok(new { responseMessage = $"{requestDto.Courses.Count() } course(s) created successfully.", responseCode = "00" });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"An error occured, reason: {ex.Message}");
                }
            }
        }


    }
}
