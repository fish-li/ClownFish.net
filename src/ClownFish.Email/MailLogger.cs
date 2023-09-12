using ClownFish.EMail;

namespace ClownFish.Email;

/// <summary>
/// EMail日志工具类
/// </summary>
public static class MailLogger
{
    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        MailClientEvent.OnSendMail += MailClientEventOnSendMail;
    }

    private static void MailClientEventOnSendMail(object sender, SendMailEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        StepItem step = StepItem.CreateNew(e.StartTime);
        step.StepKind = StepKinds.Mail;
        step.StepName = "SendMail";
        step.SetException(e.Exception);

        MailClient client = (MailClient)sender;
        step.Detail = $"subject: {client.EmailData.Subject}\nto: {string.Join(';', client.EmailData.Receivers)}\nsize: {client.EmailData.GetSize()}";

        step.End(e.EndTime);

        scope.AddStep(step);
    }

}
