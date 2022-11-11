using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_API.Data;
using Project_API.Models.Entities;

namespace Project_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly ItemdbContext itemdbContext;

        public ItemsController(ItemdbContext itemdbContext)
        {
            this.itemdbContext = itemdbContext;

        }

        [HttpGet]
        [Route("/api/Items/GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            return Ok(await itemdbContext.Items.ToListAsync());
        }

        
        [HttpGet]
        [Route("/api/Items/GetItemByItemName/{ItemName}")]
        public async Task<IActionResult> GetItemByName(string ItemName)
        {
            var Itemss = from m in itemdbContext.Items
                         select m;

            if (!String.IsNullOrEmpty(ItemName))
            {
                Itemss = Itemss.Where(s => s.Name!.Contains(ItemName));
            }

            return Ok(await Itemss.ToListAsync());

            

        }


        [HttpGet]
        [Route("/api/Items/GetItemByItemId/{id:Guid}")]
        public async Task<IActionResult> GetItemById([FromRoute] Guid id)
        {
            var item = await itemdbContext.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();

            }
            return Ok(item);

        }

        [HttpPost]
        [Route("/api/Items/AddItem")]
        public async Task<IActionResult> AddItem(Item item)
        {
            item.Id = Guid.NewGuid();
            ApiResponse response = new ApiResponse();
            await itemdbContext.Items.AddAsync(item);
            await itemdbContext.SaveChangesAsync();

            response.isSuccess = true;
            response.statusCode = StatusCodes.Status200OK.ToString();
            response.message = "Record Saved";

            return Ok(response);
            //return CreatedAtAction(nameof(GetItemById),new {id = item.Id}, item);

        }

        [HttpPut]
        [Route("/api/Items/UpdateItemByItemId/{id:Guid}")]

        public async Task<IActionResult> UpdateItem([FromRoute] Guid id, [FromBody] Item updateItem)
        {
            ApiResponse response = new ApiResponse();
            var existingItem = await itemdbContext.Items.FindAsync(id);
            if(existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItem.Name;
            existingItem.Quantity = updateItem.Quantity;
            existingItem.PricePerItem = updateItem.PricePerItem;
        
            await itemdbContext.SaveChangesAsync();

            response.isSuccess = true;
            response.statusCode = StatusCodes.Status200OK.ToString();
            response.message = "Record Updated";

            return Ok(response);

            //return Ok(existingItem);
        
        }

        [HttpDelete]
        [Route("/api/Items/DeleteItemByItemId/{id:Guid}")]

        public async Task<IActionResult> DeleteItem([FromRoute] Guid id)
        {
            ApiResponse response = new ApiResponse();
            var existingItem = await itemdbContext.Items.FindAsync(id);

            if(existingItem == null)
            {
                return NotFound();
            }

            itemdbContext.Items.Remove(existingItem);
            await itemdbContext.SaveChangesAsync();


            response.isSuccess = true;
            response.statusCode = StatusCodes.Status200OK.ToString();
            response.message = "Record Deleted";

            return Ok(response);

            //return Ok();
        }

        
    }
}
