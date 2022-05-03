namespace SqlCredFreeMvcApp001.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class CountryCapitalInfo
    {
        public string? Country { get; set; }

        public string? Capital { get; set; }
    }
}