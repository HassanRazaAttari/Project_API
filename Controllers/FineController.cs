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
        public async Task<IActionResult> GetAllFines() {

            SqlConnection db = new SqlConnection("Server=DESKTOP-NHF27C9\\SQLEXPRESS;Database=ItemsDb;Trusted_Connection=true");
            SqlCommand cmd = new SqlCommand("SELECT BorrowedItems.TimeToBeReturned, Students.StudentName, Items.Name, BorrowedItems.QuantityBorrowed from BorrowedItems INNER JOIN Items ON BorrowedItems.ItemId=Items.Id INNER JOIN Students ON Students.StudentId=BorrowedItems.StudentId", db);
            int cnt = 0;

            List<string> tmp = new List<string>();
            List<string> tmp2 = new List<string>();
            List<string> fine = new List<string>();

            string current = DateTime.Now.ToString("MM/dd/yyyy");
            db.Open();

            char[] currentDate = current.ToCharArray();

            string cmp2 = "";
            cmp2 += currentDate[0];
            cmp2 += currentDate[1];

            int currentMonth = Int32.Parse(cmp2);

            cmp2 = "";
            cmp2 += currentDate[3];
            cmp2 += currentDate[4];

            int currentDay = Int32.Parse(cmp2);

            cmp2 = "";
            cmp2 += currentDate[6];
            cmp2 += currentDate[7];
            cmp2 += currentDate[8];
            cmp2 += currentDate[9];

            int currentYear = Int32.Parse(cmp2);

            SqlDataReader reader = cmd.ExecuteReader();
            while ( reader.Read() ){
                tmp.Add( reader[0].ToString() );
                tmp2.Add(reader[1].ToString() );
            }
            db.Close();

            for(int i = 0; i<tmp.Count; i++)
            {
                char[] dbDate = tmp[i].ToCharArray();
                
                string cmp1 = "";
                cmp1 += dbDate[0];
                cmp1 += dbDate[1];

                int month = Int32.Parse(cmp1);

                cmp1 = "";
                cmp1 += dbDate[3];
                cmp1 += dbDate[4];

                int day = Int32.Parse(cmp1);

                cmp1 = "";
                cmp1 += dbDate[6];
                cmp1 += dbDate[7];
                cmp1 += dbDate[8];
                cmp1 += dbDate[9];

                int year = Int32.Parse(cmp1);

                if (currentDay > day & currentMonth > month & currentYear > year) { }
                else
                {
                    tmp2[i] = tmp2[i] + "2000";
                }
            }
            return (Ok(tmp2));
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
