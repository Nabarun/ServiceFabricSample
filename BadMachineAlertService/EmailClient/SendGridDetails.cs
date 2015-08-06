// //------------------------------------------------------------
// // Copyright (c) Microsoft Corporation.  All rights reserved.
// //------------------------------------------------------------
namespace EmailClient
{
    public class SendGridDetails
    {
        public SendGridDetails(string userName, string passWord)
        {
            this.UserName = userName;
            this.Password = passWord;
        }

        public string UserName { get; }
        public string Password { get; }
    }
}
