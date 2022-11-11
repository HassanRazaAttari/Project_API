using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_API.Data;
using Project_API.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;

namespace Project_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FineController : Controller
    {
        private readonly ItemdbContext itemdbContext;

        public FineController(ItemdbContext itemdbContext)
        {
            this.itemdbContext = itemdbContext;

        }




        static bool compareDates(string datetime1, DateTime datetime2)
        {
            string datetimee = datetime2.ToString();
            int date1 = Convert.ToInt32(datetime1.Substring(0, 2));
            int month1 = Convert.ToInt32(datetime1.Substring(3, 2));
            int year1 = Convert.ToInt32(datetime1.Substring(6, 4));

            int date2 = Convert.ToInt32(datetimee.Substring(0, 2));
            int month2 = Convert.ToInt32(datetimee.Substring(3, 2));
            int year2 = Convert.ToInt32(datetimee.Substring(6, 4));

            return (date1 + (month1 * 12) + (year1 * 365)) > (date2 + (month2 * 12) + (year2 * 365));
        }





        [Route("/api/Fine/GetAllFines")]
        [HttpGet]

        //public async Task<IEnumerable<BorrowedItem>> GetAllFineItems()
        public async Task<IActionResult> GetAllFineItems()

        {

            foreach (var item in itemdbContext.Fines)
            {
                itemdbContext.Fines.Remove(item);
            }
            itemdbContext.SaveChanges();

            DateTime now = DateTime.Now;
            var dateTime = DateTime.Now;

                        var query = from da in itemdbContext.BorrowedItems
                                    join st in itemdbContext.Items
                                    on da.ItemId equals st.Id
                                    join ab in itemdbContext.Students
                                    on da.StudentId equals ab.StudentId
                                   
                                    select new
                                    {
                                        FinedAmount = (st.PricePerItem * da.QuantityBorrowed) / 10,
                                        ItemName = st.Name,
                                        StudentId = ab.StudentId,
                                        ItemId = st.Id,
                                        StudentName = ab.StudentName,
                                        Condition = DateTime.Compare(Convert.ToDateTime(da.TimeToBeReturned), DateTime.Now) < 0


                                    };
            List<Fine> listobj = new List<Fine>(); 
            string zyx = "";
            foreach (var abc in query)
            {
                if (abc.Condition == true)
                {
                    
                   
                    
                    
                    Fine objfine= new Fine();
                    objfine.ItemId = abc.ItemId;
                    objfine.StudentId = abc.StudentId;
                    objfine.FineAmount = abc.FinedAmount;
                    objfine.ReturnedTime = "";
                    listobj.Add(objfine);
                    await itemdbContext.Fines.AddAsync(objfine);



                }
                

            }
            await itemdbContext.SaveChangesAsync();

            var innerJoin = from s in itemdbContext.Items // outer sequence
                            join st in itemdbContext.Fines //inner sequence 
                            on s.Id equals st.ItemId
                            join ab in itemdbContext.Students
                            on st.StudentId equals ab.StudentId
                             // key selector 
                            select new
                            { // result selector
                                ItemId = s.Id,
                                StudentId = ab.StudentId,
                                ItemName = s.Name,
                                StudentName = ab.StudentName,
                                FinedAmount = st.FineAmount,
                                TimeBorrowed = st.ReturnedTime
                            };

            return Ok(innerJoin);


        }

            
            
           

        [HttpGet]
        [Route("/api/Fine/GetFineByStudentId/{StudentId}")]
        public async Task<IActionResult> GetFinedItemByStudentId(string StudentId)
        {



            foreach (var item in itemdbContext.Fines)
            {
                itemdbContext.Fines.Remove(item);
            }
            itemdbContext.SaveChanges();

            DateTime now = DateTime.Now;
            var dateTime = DateTime.Now;

            var query = from da in itemdbContext.BorrowedItems
                        join st in itemdbContext.Items
                        on da.ItemId equals st.Id
                        join ab in itemdbContext.Students
                        on da.StudentId equals ab.StudentId

                        select new
                        {
                            FinedAmount = (st.PricePerItem * da.QuantityBorrowed) / 10,
                            ItemName = st.Name,
                            StudentId = ab.StudentId,
                            ItemId = st.Id,
                            StudentName = ab.StudentName,
                            Condition = DateTime.Compare(Convert.ToDateTime(da.TimeToBeReturned), DateTime.Now) < 0


                        };
            List<Fine> listobj = new List<Fine>();
            string zyx = "";
            foreach (var abc in query)
            {
                if (abc.Condition == true)
                {




                    Fine objfine = new Fine();
                    objfine.ItemId = abc.ItemId;
                    objfine.StudentId = abc.StudentId;
                    objfine.FineAmount = abc.FinedAmount;
                    objfine.ReturnedTime = "";
                    listobj.Add(objfine);
                    await itemdbContext.Fines.AddAsync(objfine);



                }


            }
            await itemdbContext.SaveChangesAsync();

            var innerJoin = from s in itemdbContext.Items // outer sequence
                            join st in itemdbContext.Fines //inner sequence 
                            on s.Id equals st.ItemId
                            join ab in itemdbContext.Students
                            on st.StudentId equals ab.StudentId
                            where st.StudentId == StudentId
                            // key selector 
                            select new
                            { // result selector
                                ItemId = s.Id,
                                StudentId = ab.StudentId,
                                ItemName = s.Name,
                                StudentName = ab.StudentName,
                                FinedAmount = st.FineAmount,
                                TimeBorrowed = st.ReturnedTime
                            };

            return Ok(innerJoin);


        }

        /*
                [HttpDelete]
                [Route("{id:Guid}/{StudentId}")]
                public async Task<IActionResult> DeleteFineItem([FromRoute] Guid id, string StudentId)
                {
                    var existingItem = await itemdbContext.Fines.FindAsync(id, StudentId);
                    ApiResponse response = new ApiResponse();


                    if (existingItem == null)
                    {
                        return NotFound();
                    }



                    itemdbContext.Fines.Remove(existingItem);



                    await itemdbContext.SaveChangesAsync();

                    response.isSuccess = true;
                    response.statusCode = StatusCodes.Status200OK.ToString();
                    response.message = "Record Deleted";
                    //response.data = record;


                    return Ok(response);
                }
        */
        /*
        [HttpPut]
        [Route("{id:Guid}/{StudentId}")]
        public async Task<IActionResult> UpdateFineItem([FromRoute] Guid id, string StudentId, [FromBody] UpdateFineItemsViewModel itemviewmodel)
        {


            ApiResponse response = new ApiResponse();
            var item = new Fine
            {
                FineAmount = itemviewmodel.FineAmount,
                ReturnedTime = itemviewmodel.ReturnedTime,
                ItemId = itemviewmodel.ItemId,
                StudentId = itemviewmodel.StudentId,

            };

            var existingItem = await itemdbContext.Fines.FindAsync(id, StudentId);
            if (existingItem == null)
            {
                return NotFound();
            }



            existingItem.ReturnedTime = itemviewmodel.ReturnedTime ;
            existingItem.FineAmount = itemviewmodel.FineAmount;
            

            response.isSuccess = true;
            response.statusCode = StatusCodes.Status200OK.ToString();
            response.message = "Record Updated";


            await itemdbContext.SaveChangesAsync();

            return Ok(response);

        }

        */
    }

}
        