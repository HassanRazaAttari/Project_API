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
        [HttpGet]
        public async Task<IActionResult> GetAllBorrowedItems()
        {
            return Ok(await itemdbContext.BorrowedItems.ToListAsync());
        }

        [HttpGet]
        [Route("{ItemId:Guid}/{StudentId}")]
        public async Task<IActionResult> GetBorrowedItemById([FromRoute] Guid ItemId, string StudentId)
        {
            var item = await itemdbContext.BorrowedItems.FindAsync(ItemId, StudentId);
            if (item == null)
            {
                return NotFound();

            }
            return Ok(item);

        }

        [HttpPost]


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
                  Student = itemviewmodel.Student
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
        [Route("{id:Guid}/{StudentId}")]

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



 


            await itemdbContext.SaveChangesAsync();

            return Ok(existingItem);

        }

        [HttpDelete]
        [Route("{id:Guid}/{StudentId}")]
        //[Route("api/item/{id1}/{id2}")]
        public async Task<IActionResult> DeleteItem([FromRoute] Guid id, string StudentId)
        {
            var existingItem = await itemdbContext.BorrowedItems.FindAsync(id, StudentId);

            if (existingItem == null)
            {
                return NotFound();
            }

            var record = await itemdbContext.Items.Where(a => a.Id == id).FirstOrDefaultAsync();
            var record2 = await itemdbContext.BorrowedItems.Where(a => a.ItemId == id).FirstOrDefaultAsync();
            if (record != null)
            {
                record.Quantity = record.Quantity + record2.QuantityBorrowed;
            }


            itemdbContext.BorrowedItems.Remove(existingItem);
            await itemdbContext.SaveChangesAsync();

            return Ok();
        }


    }
}
