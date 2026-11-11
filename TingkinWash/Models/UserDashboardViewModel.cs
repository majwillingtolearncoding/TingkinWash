namespace TingkinWash.ViewModels
{
    public class UserDashboardViewModel
    {
        public string FullName { get; set; } = "";
        public string Username { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CompletedBookings { get; set; }
    }
}