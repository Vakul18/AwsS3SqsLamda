using System.Text;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.SQSEvents;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaDemo;

public class Function
{
    // Replace sender@example.com with your "From" address.
    // This address must be verified with Amazon SES.
    static readonly string senderAddress = "vakul.18@gmail.com";

    // Replace recipient@example.com with a "To" address. If your account
    // is still in the sandbox, this address must be verified.
    static readonly string receiverAddress = "vakul.18@gmail.com";

    // The configuration set to use for this email. If you do not want to use a
    // configuration set, comment out the following property and the
    // ConfigurationSetName = configSet argument below. 
    static readonly string configSet = "ConfigSet";

    // The subject line for the email.
    static readonly string subject = "Amazon SES test (AWS SDK for .NET)";

    // The email body for recipients with non-HTML email clients.
    static readonly string textBody = "Amazon SES Test (.NET)\r\n"
                                    + "This email was sent through Amazon SES "
                                    + "using the AWS SDK for .NET.";

    // The HTML body of the email.
    static readonly string htmlBody = @"<html>
<head></head>
<body>
  <h1>Amazon SES Test (AWS SDK for .NET)</h1>
  <p>This email was sent with
    <a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
    <a href='https://aws.amazon.com/sdk-for-net/'>
      AWS SDK for .NET</a>.</p>
</body>
</html>";

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    async public Task FunctionHandler(SQSEvent event1, ILambdaContext context)
    {
        StringBuilder sb = new StringBuilder();
        foreach(var rec in event1.Records)
        {
            sb.Append("Rec :" + Environment.NewLine);
            sb.Append("EventSource : "+rec.EventSource + Environment.NewLine);
            sb.Append("EventSourceArn : "+rec.EventSourceArn + Environment.NewLine);
            sb.Append("ReceiptHandle : "+rec.ReceiptHandle + Environment.NewLine);
            sb.Append("Attributes : " + Environment.NewLine);
            foreach(var dict in rec.Attributes)
            {
                sb.Append(dict.Key + " : "+ dict.Value + Environment.NewLine);
            }
            sb.Append("MessageAttributes : " + Environment.NewLine);
            foreach(var dict in rec.MessageAttributes)
            {
                sb.Append(dict.Key + " : "+ dict.Value + Environment.NewLine);
            }
            sb.Append("Body :" + Environment.NewLine);
            sb.Append(rec.Body + Environment.NewLine);
        }
        context.Logger.Log("MEssage : " + sb.ToString());
        using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
        {
            var sendRequest = new SendEmailRequest
            {
                Source = senderAddress,
                Destination = new Destination
                {
                    ToAddresses =
                    new List<string> { receiverAddress }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlBody + Environment.NewLine + sb.ToString()
                        },
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = textBody 
                        }
                    }
                },
                // If you are not using a configuration set, comment
                // or remove the following line 
               // ConfigurationSetName = configSet
            };
            try
            {
                context.Logger.Log("Sending email using Amazon SES...");
                var response =await client.SendEmailAsync(sendRequest);
                context.Logger.Log("The email was sent successfully.");
            }
            catch (Exception ex)
            {
                context.Logger.Log("The email was not sent.");
                context.Logger.Log("Error message: " + ex.Message);

            }
        }
    }
}
