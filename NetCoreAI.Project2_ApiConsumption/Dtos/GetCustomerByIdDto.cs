namespace NetCoreAI.Project2_ApiConsumption.Dtos
{
    public class GetCustomerByIdDto
    {
        public int CustomerId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public decimal CustomerBalance { get; set; }
    }
}
