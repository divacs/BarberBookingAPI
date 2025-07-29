namespace BarberBookingAPI.Helppers
{
    public class QueryObject
    {
        // We can add here different filters and pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
