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


        [HttpGet]
        //public async Task<IEnumerable<BorrowedItem>> GetAllFineItems()
        public async Task<IActionResult> GetAllFineItems()

        {


            DateTime now = DateTime.Now;
            var dateTime = DateTime.Now;
            /*
                        var query = from da in itemdbContext.BorrowedItems
                                    join st in itemdbContext.Items
                                    on da.ItemId equals st.Id
                                    join ab in itemdbContext.Students
                                    on da.StudentId equals ab.StudentId
                                    // where DateTime.Compare(Convert.ToDateTime(da.TimeToBeReturned), DateTime.Now) < 0
                                    // where DateTime.Compare(DateTime.ParseExact(da.TimeToBeReturned, "MM-dd-yyyy", CultureInfo.InvariantCulture), DateTime.Now) < 0
                                    //where DateTime.Compare(DateTime.ParseExact(da.TimeToBeReturned, "MM-dd-yyyy", CultureInfo.InvariantCulture), DateTime.Now) < 0
                                    select new
                                    {
                                        FinedAmount = (st.PricePerItem * da.QuantityBorrowed) / 10,
                                        ItemName = st.Name,
                                        StudentId = ab.StudentId,
                                        ItemId = st.Id,
                                        StudentName = ab.StudentName,
                                        coondition = DateTime.Compare(DateTime.ParseExact(da.TimeToBeReturned, "MM-dd-yyyy", CultureInfo.InvariantCulture), DateTime.Now) < 0,
                                        //  = DateTime.ParseExact(da.TimeToBeReturned, "MM-dd-yyyy", CultureInfo.InvariantCulture),
                                        todaytime = DateTime.Now,
                                        // Timeabc = DateTime.ParseExact(da.TimeToBeReturned, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                                        TimeToBeReturned = typeof()
                                    };
            */

            List<Fine> finees = new List<Fine>();

            foreach (var item in itemdbContext.BorrowedItems)
            {
                if (DateTime.Compare(DateTime.ParseExact(item.TimeToBeReturned, "MM-dd-yyyy", CultureInfo.InvariantCulture), DateTime.Now) < 0)
                {
                    var studid = item.StudentId;
                    var query = from da in itemdbContext.BorrowedItems
                                join st in itemdbContext.Items
                                on da.ItemId equals st.Id
                                join ab in itemdbContext.Students
                                on da.StudentId equals ab.StudentId
                                where da.StudentId == studid & da.ItemId == item.ItemId

                                select new
                                {
                                    FinedAmount = (st.PricePerItem * da.QuantityBorrowed) / 10,
                                    ItemName = st.Name,
                                    StudentId = ab.StudentId,
                                    ItemId = st.Id,
                                    StudentName = ab.StudentName
                                };

                    Fine myInt = (Fine)query;
                    finees.Add(myInt);



                    //itemdbContext.Fines.Remove(item);
                }
                //itemdbContext.SaveChanges();


                /* IQueryable<BorrowedItem> query1 = itemdbContext.BorrowedItems;


                 query1 = query1.ToList().Where(e => DateTime.Compare(DateTime.ParseExact(e.TimeToBeReturned, "MM-dd-yyyy", CultureInfo.InvariantCulture), DateTime.Now) < 0);


                     return await query1.ToListAsync();
                 */
                /*       var innerJoin = from s in itemdbContext.Items // outer sequence
                                       join st in itemdbContext.Fines //inner sequence 
                                       on s.Id equals st.ItemId
                                       join ab in itemdbContext.Students
                                       on st.StudentId equals ab.StudentId
                                       join cd in itemdbContext.BorrowedItems
                                       on s.Id equals cd.ItemId// key selector 
                                       select new
                                       { // result selector 
                                           ItemId = s.Id,
                                           StudentId = ab.StudentId,
                                           ItemName = s.Name,
                                           StudentName = ab.StudentName,
                                           FineAmount = st.FineAmount,
                                           ReturnedTime = st.ReturnedTime,
                                           TimeToBeReturned = cd.TimeToBeReturned

                                       };
                */
               
            }
            return Ok(finees);
        }

        [HttpGet]
        [Route("{StudentId}")]
        public async Task<IActionResult> GetFinedItemByStudentId(string StudentId)
        {

            


            var innerJoin = from s in itemdbContext.Items // outer sequence
                            join st in itemdbContext.Fines //inner sequence 
                            on s.Id equals st.ItemId
                            join ab in itemdbContext.Students
                            on st.StudentId equals ab.StudentId
                            join cd in itemdbContext.BorrowedItems
                            on s.Id equals cd.ItemId
                            where ab.StudentId == StudentId// key selector 
                            select new
                            { // result selector 
                                ItemId = s.Id,
                                StudentId = ab.StudentId,
                                ItemName = s.Name,
                                StudentName = ab.StudentName,
                                FineAmount = st.FineAmount,
                                ReturnedTime = st.ReturnedTime,
                                TimeToBeReturned = cd.TimeToBeReturned

                            };

            return Ok(innerJoin);

        }


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


    }
}
