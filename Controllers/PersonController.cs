using HEIRS.HOLDING.INTERVIEW.TEST.DataContext;
using HEIRS.HOLDING.INTERVIEW.TEST.Models.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace HEIRS.HOLDING.INTERVIEW.TEST.Controllers
{
    [RoutePrefix("api")]
    public class PersonController : ApiController
    {
        public PersonController()
        {

        }


        [HttpPost, Route("CreatePeople")]
        public async Task<IHttpActionResult> CreatePeople([FromBody] CreatePersonListDto createPerson)
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
                        int countDuplicate = 0;
                        int countEntryWithoutGrade = 0;
                        List<Person> noDuplicateRecord = new List<Person>();
                        createPerson.Persons.ForEach(c =>
                        {
                            var checkRes = ctx.People.Where(w => w.CourseId == c.CourseId || w.CourseId.ToLower().Equals(c.CourseId.ToLower())).FirstOrDefault();
                            if (checkRes != null) //count the number of record(s) that already exists in the db
                            {
                                countDuplicate += 1;
                            }
                            else
                            {
                                //create a new Course Model to store the ones that are not duplicated
                                noDuplicateRecord.Add(new Person
                                {
                                    PersonId = c.PersonId,
                                    CourseId = c.CourseId,
                                    Name = c.Name,
                                    grade = c.Grade == 0.00 ? countEntryWithoutGrade += 1 : c.Grade
                                });
                            }
                        });

                        if (countEntryWithoutGrade > 0)
                        {
                            return BadRequest("Please provide grade for all students");
                        }
                        else
                        {
                            if (noDuplicateRecord != null && noDuplicateRecord.Count() > 0 && countEntryWithoutGrade == 0)
                            {
                                //then add the list to db
                                ctx.People.AddRange(noDuplicateRecord);
                                await ctx.SaveChangesAsync();                              
                            }
                            if (countDuplicate > 0)
                                return Ok(new { responseMessage = $"{createPerson.Persons.Count() - countDuplicate} record(s) added successfully, Reason: We found {countDuplicate} duplicates, and they were ommited while creating new entries.", responseCode = "90" });
                            return Ok(new { responseMessage = $"{createPerson.Persons.Count()} record(s) created successfully.", responseCode = "00" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"An error occured, reason: {ex.Message}");
                }
            }
        }


        public class GenericPersonResponse
        {
            public object ResponseObject { get; set; }
            public double ScoreCount { get; set; }
            public double GPA { get; set; }
        }


        [HttpGet, Route("GetAllPersons")]
        public async Task<IHttpActionResult> GetAllPersons()
        {
            try
            {
                using (var ctx = new heirs_dbEntities())
                {
                    return Ok(new
                    {
                        data = await ctx.People.ToListAsync(),
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured, reason: {ex.Message}");
            }
        }



        [HttpGet, Route("GetPersonById")]
        public async Task<IHttpActionResult> GetPersonById(string personId)
        {
            try
            {
                using (var ctx = new heirs_dbEntities())
                {
                    // await ctx.StudentAssignments.Join(ctx.Assignments, stdAss => stdAss.AssignmentId, ass => ass.AssignmentId, (stdAss, ass) => new
                    var newList = new List<GenericPersonResponse>();

                    var res = await ctx.People.Where(g => g.PersonId == personId || g.PersonId.ToLower().Equals(personId.ToLower()))
                        .Join(ctx.Courses, peo => peo.CourseId, cou => cou.id, (peo, cou) => new
                        {
                            Name = peo.Name,
                            CourseDetails = ctx.Courses.Where(v => v.id == peo.CourseId)
                            .Join(ctx.People.Where(e => e.PersonId == personId), cou1 => cou1.id, peo1 => peo1.CourseId, (cou1, peo1) => new
                            {
                                cou1.name, peo1.CourseId, peo1.grade,
                            }).FirstOrDefault(),
                            //GradePointAverage = GenerateGradePointAverage(ctx.People.Where(m => m.PersonId == personId).Select(x => x.Score).ToArray())
                        }).ToListAsync();

                    var nList2 = new List<double>();
                    foreach (var item in res)
                    {
                        nList2.Add((double)item.CourseDetails.grade);
                    }

                    var gpa = (nList2.Sum() / nList2.Count());

                    newList.Add(new GenericPersonResponse()
                    {
                        GPA = gpa > 1 ? gpa : 0,
                        ResponseObject = res,
                        ScoreCount = ctx.People.Where(s => s.PersonId == personId).ToList().Count()
                    });
                    return Ok(newList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured, reason: {ex.Message}");
            }
        }

        public static double GenerateGradePointAverage(double?[] listItems)
        {
            return (double)(listItems.Sum() / listItems.Count());             
        }

    }
}
