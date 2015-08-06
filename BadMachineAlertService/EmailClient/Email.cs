// //------------------------------------------------------------
// // Copyright (c) Microsoft Corporation.  All rights reserved.
// //------------------------------------------------------------
namespace EmailClient
{
    public class Email
    {
        public Email(string toRecepients, string fromRecepients, string subject, string body)
        {
            this.ToRecepients = toRecepients;
            this.FromRecepients = fromRecepients;
            this.EmailBody = body;
            this.Subject = subject;
        }

        public string ToRecepients { get; }
        public string FromRecepients { get; }
        public string Subject { get; }
        public string EmailBody { get; }
    }
}
