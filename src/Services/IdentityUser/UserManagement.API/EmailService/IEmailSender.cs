namespace UserManagement.API.EmailService;

public interface IEmailSender
{
    void SendEmail(Message message);
    Task SendEmailAsync(Message message);
}
