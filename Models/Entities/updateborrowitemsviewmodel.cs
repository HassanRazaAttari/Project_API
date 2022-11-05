namespace Project_API.Models.Entities
{
    public class updateborrowitemsviewmodel
    {
        public Guid ItemId { get; set; }
        public string StudentId { get; set; }

        public int QuantityBorrowed { get; set; }

        public string TimeBorrowed { get; set; }

        public string TimeToBeReturned { get; set; }

    }
}
