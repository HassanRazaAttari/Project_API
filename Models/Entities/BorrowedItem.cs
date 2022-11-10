using System.ComponentModel.DataAnnotations.Schema;

namespace Project_API.Models.Entities
{
    public class BorrowedItem
    {
    
        
        public int QuantityBorrowed { get; set; }

        public string TimeBorrowed { get; set; }

        public  string TimeToBeReturned { get; set; }
        
        [ForeignKey("Item")]
        public Guid ItemId { get; set; }
        public virtual Item Item { get; set; }

        
        [ForeignKey("Student")]
        public string StudentId { get; set; }

        public virtual Student Student { get; set; }

    }
}
