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
        public async Task<IActionResult> GetAllItems()
        {
            return Ok(await itemdbContext.Items.ToListAsync());
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetItemById([FromRoute]Guid id)
        {
            var item = await itemdbContext.Items.FindAsync(id);
            if(item == null)
            {
                return NotFound();

            }
            return Ok(item);

        }


        [HttpPost]

        public async Task<IActionResult> AddItem(Item item)
        {
            item.Id = Guid.NewGuid();
            await itemdbContext.Items.AddAsync(item);
            await itemdbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemById),new {id = item.Id}, item);

        }

        [HttpPut]
        [Route("{id:Guid}")]

        public async Task<IActionResult> UpdateItem([FromRoute] Guid id, [FromBody] Item updateItem)
        {
            var existingItem = await itemdbContext.Items.FindAsync(id);
            if(existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItem.Name;
            existingItem.Quantity = updateItem.Quantity;
        
            await itemdbContext.SaveChangesAsync();

            return Ok(existingItem);
        
        }

        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> DeleteItem([FromRoute] Guid id)
        {
            var existingItem = await itemdbContext.Items.FindAsync(id);

            if(existingItem == null)
            {
                return NotFound();
            }

            itemdbContext.Items.Remove(existingItem);
            await itemdbContext.SaveChangesAsync();

            return Ok();
        }

        
    }
}
