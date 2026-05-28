namespace SmtpEmailSend
{
    public interface ICustomEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }
}
