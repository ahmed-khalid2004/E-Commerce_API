namespace ServiceAbstracion
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string otp);
    }
}