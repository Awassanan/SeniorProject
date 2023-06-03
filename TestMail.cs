using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace SeniorProject
{
    class Mail
    {
        public static void SendMail(string receiver, string subject, string text /*, string password*/)
        {

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Chatchawit Aporntewan", "chatchawit.a@chula.ac.th"));
            email.To.Add(MailboxAddress.Parse(receiver));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = text };

            var smtp = new SmtpClient();

            smtp.Connect("email-smtp.ap-southeast-1.amazonaws.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("AKIA2MYSA4BNV37CMUXO", "BDYpo8WMZM411m71iOjzBTI6Utwd6Lb3AqURBu2pxlBR");

            //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);        
            //smtp.Authenticate("chatchawit.a@chula.ac.th", password);

            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public static void Main(String[] arg){
            string receiver = "6234472123@student.chula.ac.th";
            string subject = "ทดสอบใช้ API ส่งอีเมล";
            string text = "นี่คือข้อความที่ใช้ทดสอบการส่งอีเมลด้วย API";
            SendMail(receiver,subject,text);
        }
    }
}