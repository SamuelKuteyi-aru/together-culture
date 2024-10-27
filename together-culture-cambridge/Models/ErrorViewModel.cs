namespace together_culture_cambridge.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
//check for request ID
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}