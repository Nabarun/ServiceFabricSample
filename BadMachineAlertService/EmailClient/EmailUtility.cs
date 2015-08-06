// //------------------------------------------------------------
// // Copyright (c) Microsoft Corporation.  All rights reserved.
// //------------------------------------------------------------

    namespace EmailClient
{
    using System.Net;
    using System.Net.Mail;
    using SendGrid;

    public class EmailUtility
    {
        public void SendMailUsingSendGrid(Email emailObj, SendGridDetails sendGridDetails)
        {
            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress(emailObj.FromRecepients);

            // Add multiple addresses to the To field.
            myMessage.AddTo(emailObj.ToRecepients);

            myMessage.Subject = emailObj.Subject;

            //Add the HTML and Text bodies
            myMessage.Html = emailObj.EmailBody;

            // Create network credentials to access your SendGrid account
            var username = sendGridDetails.UserName;
            var pswd = sendGridDetails.Password;

            var credentials = new NetworkCredential(username, pswd);


            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            // You can also use the **DeliverAsync** method, which returns an awaitable task.
            transportWeb.DeliverAsync(myMessage);
        }
    }
}
