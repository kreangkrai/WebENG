using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebENG.LeaveInterfaces;

namespace WebENG.LeaveServices
{
    public class MailService : IMail
    {

        public string Requester(List<string> to, string status,string name,string leave_type, string leave_date , string leave_time)
        {
            try
            {

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient();

                string password = "P@ssw0rd";
                string sender = "e-leave@contrologic.co.th";
                mail.From = new MailAddress(sender);
                for (int i = 0; i < to.Count; i++)
                {
                    mail.To.Add(to[i]);
                }

                mail.Subject = "แจ้งเตือนระบบวันลา";
                string body = $@"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <link href=""https://fonts.googleapis.com/css2?family=Kanit:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"" rel=""stylesheet"">
                    <style>
                        body {{ font-family: 'Kanit', sans-serif; background:#f4f4f4; margin:0; padding:0; -webkit-text-size-adjust:100%; }}
                        .email-wrapper {{ max-width: 620px; margin: 20px auto; background: #fff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.08); }}
                        .header {{ background: darkorange; color: #fff; padding: 30px 10px; text-align: center; }}
                        .header h1 {{ margin: 0; font-size: 26px; font-weight: 600; }}
                        .content {{ padding: 40px 30px; background:#fff; }}
                        .info-card {{ background: #f8f9fa; border-radius: 12px; padding: 25px; margin-bottom: 35px; border-left: 5px solid #fb8c00; }}
                        .btn-single {{
                            display: block; width: 85%; max-width: 420px; margin: 30px auto; padding: 18px 20px; text-align: center;
                            background: linear-gradient(135deg, #6bc97b, #4fb05f); color: #fff !important; font-size: 20px; font-weight: 600;
                            text-decoration: none; border-radius: 50px; box-shadow: 0 8px 25px rgba(40,167,69,0.4);
                        }}
                        .footer {{ background: #1e88e5; color: #fff; text-align: center; padding: 20px; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class=""email-wrapper"">
                        <div class=""header"">
                            <h1>{status}</h1>
                        </div>

                        <div class=""content"">
                            <p class=""kanit-bold"" style=""font-size:18px;"">แจ้งเตือนใบลา</p>

                            <div style=""background:#fff3e0; border-left:4px solid #ff9100; padding:15px; margin:20px 0; border-radius:8px;"">
                                <strong>สถานะใบลา:</strong> <span style=""color:#e65100;font-weight:600;"">{status}</span>
                            </div>

                            <div class=""info-card"">
                                <table style=""width:100%; border-collapse:collapse;"">
                                    <tr><th width=""140"" align=""left"" style=""padding:8px 0;"">ประเภทการลา</th><td>{leave_type}</td></tr>
                                    <tr><th align=""left"" style=""padding:8px 0;"">ชื่อ</th><td>{name}</td></tr>
                                    <tr><th align=""left"" style=""padding:8px 0;"">วันที่ลา</th><td>{leave_date}</td></tr>
                                    <tr><th align=""left"" style=""padding:8px 0;"">เวลาที่ลา</th><td>{leave_time}</td></tr>
                                </table>
                            </div>

                            <div style=""text-align:center;"">
                                    <a href = ""http://ctracking.contrologic.co.th/eng/ManagementLeave""
                                    style = ""text-decoration:none; color:inherit; display:block; width:100%; height:100%;border:none;font-size:18px;background:#fff3e0;"" >
                                    <i class=""fas fa-edit fa-fw kanit-bold""></i> คลิ๊กเพื่อไปที่หน้าเว็บ
                                    </a>
                            </div>

                            <div style=""background:#e3f2fd; padding:15px; border-radius:8px; text-align:center; margin-top:20px;"">
                                คลิกปุ่มด้านบนเพื่อเข้าไปดำเนินการ<br>
                            </div>
                        </div>

                        <div class=""footer"">
                            <p>ระบบการลาออนไลน์ • บริษัท คอนโทรลอจิค จำกัด</p>
                            <small>© 2025 บริษัท คอนโทรลอจิค จำกัด สงวนลิขสิทธิ์</small>
                        </div>
                    </div>
                </body>
                </html>";


                mail.IsBodyHtml = true;
                string html = body;

                mail.Body = html;
                mail.IsBodyHtml = true;
                SmtpServer.EnableSsl = false;
                SmtpServer.Host = "192.168.15.3";
                SmtpServer.Port = 25;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(sender, password);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Success";
        }
        public string Approver(string to, string status, string leave_type, string leave_date, string leave_time,string approver,string comment)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient();

                string password = "P@ssw0rd";
                string sender = "e-leave@contrologic.co.th";
                mail.From = new MailAddress(sender);
                mail.To.Add(to);
                

                mail.Subject = "แจ้งเตือนระบบวันลา";
                string body = $@"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <link href=""https://fonts.googleapis.com/css2?family=Kanit:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"" rel=""stylesheet"">
                    <style>
                        body {{ font-family: 'Kanit', sans-serif; background:#f4f4f4; margin:0; padding:0; -webkit-text-size-adjust:100%; }}
                        .email-wrapper {{ max-width: 620px; margin: 20px auto; background: #fff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.08); }}
                        .header {{ background: darkorange; color: #fff; padding: 30px 10px; text-align: center; }}
                        .header h1 {{ margin: 0; font-size: 26px; font-weight: 600; }}
                        .content {{ padding: 40px 30px; background:#fff; }}
                        .info-card {{ background: #f8f9fa; border-radius: 12px; padding: 25px; margin-bottom: 35px; border-left: 5px solid #fb8c00; }}
                        .btn-single {{
                            display: block; width: 85%; max-width: 420px; margin: 30px auto; padding: 18px 20px; text-align: center;
                            background: linear-gradient(135deg, #6bc97b, #4fb05f); color: #fff !important; font-size: 20px; font-weight: 600;
                            text-decoration: none; border-radius: 50px; box-shadow: 0 8px 25px rgba(40,167,69,0.4);
                        }}
                        .footer {{ background: #1e88e5; color: #fff; text-align: center; padding: 20px; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class=""email-wrapper"">
                        <div class=""header"">
                            <h1>{status}</h1>
                        </div>

                        <div class=""content"">
                            <p>แจ้งเตือนใบลา</p>

                            <div style=""background:#fff3e0; border-left:4px solid #ff9100; padding:15px; margin:20px 0; border-radius:8px;"">
                                <strong>สถานะใบลา:</strong> <span style=""color:#e65100;font-weight:600;"">{status}</span>
                            </div>

                            <div class=""info-card"">
                                <table style=""width:100%; border-collapse:collapse;"">
                                    <tr><th width=""140"" align=""left"" style=""padding:8px 0;"">ประเภทการลา</th><td>{leave_type}</td></tr>
                                    <tr><th align=""left"" style=""padding:8px 0;"">วันที่ลา</th><td>{leave_date}</td></tr>
                                    <tr><th align=""left"" style=""padding:8px 0;"">เวลาที่ลา</th><td>{leave_time}</td></tr>
                                    <tr><th align=""left"" style=""padding:8px 0;"">ผู้พิจารณา</th><td>{approver}</td></tr>
                                </table>
                            </div>

                            <div style=""background:#fff8e1; border-radius:8px; padding:18px; margin:25px 0; line-height:1.6;"">
                                <strong class=""kanit-bold"">ความเห็นจากผู้อนุมัติ:</strong><br><br>
                                {comment}
                            </div>

                            <div style=""text-align:center;"">
                                    <a href = ""http://ctracking.contrologic.co.th/eng/StatusLeave""
                                    style = ""text-decoration:none; color:inherit; display:block; width:100%; height:100%;border:none;font-size:18px;background:#fff3e0;"" >
                                    <i class=""fas fa-edit fa-fw kanit-bold""></i> คลิ๊กเพื่อไปที่หน้าเว็บ
                                    </a>
                            </div>

                            <div style=""background:#e3f2fd; padding:15px; border-radius:8px; text-align:center; margin-top:20px;"">
                                คลิกปุ่มด้านบนเพื่อเข้าไปดำเนินการ<br>
                            </div>
                        </div>

                        <div class=""footer"">
                            <p>ระบบการลาออนไลน์ • บริษัท คอนโทรลอจิค จำกัด</p>
                            <small>© 2025 บริษัท คอนโทรลอจิค จำกัด สงวนลิขสิทธิ์</small>
                        </div>
                    </div>
                </body>
                </html>";


                mail.IsBodyHtml = true;
                string html = body;

                mail.Body = html;
                mail.IsBodyHtml = true;
                SmtpServer.EnableSsl = false;
                SmtpServer.Host = "192.168.15.3";
                SmtpServer.Port = 25;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(sender, password);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Success";
        }

    }
}
