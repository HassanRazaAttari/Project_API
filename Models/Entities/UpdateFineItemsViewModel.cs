namespace Project_API.Models.Entities
{
    public class UpdateFineItemsViewModel
    {
        public Guid ItemId { get; set; }
        public string StudentId { get; set; }

        public int FineAmount { get; set; }
        public string ReturnedTime { get; set; }
    }
}
