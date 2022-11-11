using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_API.Data;
using Project_API.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowedItemController : Controller
    {
        private readonly ItemdbContext itemdbContext;

        public BorrowedItemController(ItemdbContext itemdbContext)
        {
            this.itemdbContext = itemdbContext;

        }

        [Route("/api/BorrowedItem/GetAllBorrowedItems")]
        [HttpGet]
        public async Task<IActionResult> GetAllBorrowedItems()
        {
            
            var innerJoin = from s in itemdbContext.Items // outer sequence
                            join st in itemdbContext.BorrowedItems //inner sequence 
                            on s.Id equals st.ItemId
                            join ab in itemdbContext.Students
                            on st.StudentId equals ab.StudentId// key selector 
                            select new
                            { // result selector 
                                ItemId = s.Id,
                                StudentId = ab.StudentId,
                                ItemName = s.Name,
                                StudentName = ab.StudentName,
                                QuantityBorrowed = st.QuantityBorrowed,
                                TimeBorrowed = st.TimeBorrowed,
                                Timetobereturned = st.TimeToBeReturned

                            };
            return Ok(innerJoin);
        }



        [HttpGet]
        [Route("/api/BorrowedItem/GetBorrowedItemByStudentId/{StudentId}")]
        //[Route("{StudentName}")]
        public async Task<IActionResult> GetBorrowedItemByStudentId( string StudentId)
        {
            var innerJoin = from s in itemdbContext.Items // outer sequence
                            join st in itemdbContext.BorrowedItems //inner sequence 
                            on s.Id equals st.ItemId
                            join ab in itemdbContext.Students
                            on st.StudentId equals ab.StudentId
                            where ab.StudentId == StudentId // key selector 
                            select new
                            { // result selector
                                ItemId = s.Id,
                                StudentId = ab.StudentId,
                                ItemName = s.Name,
                                StudentName = ab.StudentName,
                                QuantityBorrowed = st.QuantityBorrowed,
                                TimeBorrowed = st.TimeBorrowed,
                                Timetobereturned = st.TimeToBeReturned

                            };

            return Ok(innerJoin);

        }

        [HttpPost]
        [Route("/api/BorrowedItem/AddBorrowedItem")]

        public async Task<ApiResponse> AddBorrowedItem(BorrowedItemsViewModel itemviewmodel)
        {
            ApiResponse response = new ApiResponse();
            var item = new  BorrowedItem
                {
                  QuantityBorrowed = itemviewmodel.QuantityBorrowed,
                  TimeBorrowed= itemviewmodel.TimeBorrowed,
                  TimeToBeReturned = itemviewmodel.TimeToBeReturned,    
                  ItemId = itemviewmodel.ItemId,      
                  StudentId = itemviewmodel.StudentId,
                  
                };


            await itemdbContext.BorrowedItems.AddAsync(item);
            await itemdbContext.SaveChangesAsync();
            // For get record through item id
            //aise hoga 
            var record = await itemdbContext.Items.Where(a => a.Id == item.ItemId).FirstOrDefaultAsync();
            if(record != null)
            {
                record.Quantity = record.Quantity - item.QuantityBorrowed;
            } 
            await itemdbContext.SaveChangesAsync(); 

            response.isSuccess = true;
            response.statusCode = StatusCodes.Status200OK.ToString();
            response.message = "Record Saved";
            //response.data = record;
            return response;
            //return //CreatedAtAction(nameof(GetBorrowedItemById), new { id = item.ItemId }, item);
          

        }


        [HttpPut]
        [Route("/api/BorrowedItem/UpdateBorrowedItemByItemIdAndStudentId/{id:Guid}/{StudentId}")]

        public async Task<IActionResult> UpdateItem([FromRoute] Guid id, string StudentId,[FromBody] updateborrowitemsviewmodel itemviewmodel)
        {
            

            ApiResponse response = new ApiResponse();
            var item = new BorrowedItem
            {
                QuantityBorrowed = itemviewmodel.QuantityBorrowed,
                TimeBorrowed = itemviewmodel.TimeBorrowed,
                TimeToBeReturned = itemviewmodel.TimeToBeReturned,
                ItemId = itemviewmodel.ItemId,
                StudentId = itemviewmodel.StudentId,
                
            };

            var existingItem = await itemdbContext.BorrowedItems.FindAsync(id,StudentId);
            if (existingItem == null)
            {
                return NotFound();
            }

            var record = await itemdbContext.Items.Where(a => a.Id == item.ItemId).FirstOrDefaultAsync();
            if (record != null)
            {
                int abc = itemviewmodel.QuantityBorrowed - existingItem.QuantityBorrowed;
                record.Quantity = record.Quantity - abc;
            }

            existingItem.QuantityBorrowed = itemviewmodel.QuantityBorrowed;
            existingItem.TimeBorrowed = itemviewmodel.TimeBorrowed;
            existingItem.TimeToBeReturned = itemviewmodel.TimeToBeReturned;


            response.isSuccess = true;
            response.statusCode = StatusCodes.Status200OK.ToString();
            response.message = "Record Updated";


            await itemdbContext.SaveChangesAsync();

            return Ok(response);

        }

        [HttpDelete]
        [Route("/api/BorrowedItem/DeleteBorrowedItemByItemIdAndStudentId/{id:Guid}/{StudentId}")]
        //[Route("api/item/{id1}/{id2}")]
        public async Task<IActionResult> DeleteItem([FromRoute] Guid id, string StudentId, int ReturnedQuantity)
        {
            var existingItem = await itemdbContext.BorrowedItems.FindAsync(id, StudentId);
            ApiResponse response = new ApiResponse();


            if (existingItem == null)
            {
                return NotFound();
            }

            var record = await itemdbContext.Items.Where(a => a.Id == id).FirstOrDefaultAsync();
            var record2 = await itemdbContext.BorrowedItems.Where(a => a.ItemId == id).FirstOrDefaultAsync();
            if (record != null)
            {
                record.Quantity = record.Quantity + ReturnedQuantity;
                record2.QuantityBorrowed = record2.QuantityBorrowed - ReturnedQuantity;
            }

            if(record2.QuantityBorrowed == 0)
            {
                itemdbContext.BorrowedItems.Remove(existingItem);

            }



            await itemdbContext.SaveChangesAsync();

            response.isSuccess = true;
            response.statusCode = StatusCodes.Status200OK.ToString();
            response.message = "Record Deleted";
            //response.data = record;
           

            return Ok(response);
        }


    }
}
