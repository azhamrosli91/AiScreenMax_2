namespace MaxSys.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class ErrorViewModel_Page
    {
        public string? Message { get; set; }
    }
}
