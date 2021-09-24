using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Aaina.Common
{
    public static class Common
    {
        public static string DateFormate()
        {
            return "dd/MM/yyyy";
        }
        public static string ServiceResultGetMethod(string serviceUrl)
        {
            string target;
            HttpWebRequest HttpWReq;
            HttpWebResponse HttpWResp;
            HttpWReq = (HttpWebRequest)WebRequest.Create(serviceUrl);
            HttpWReq.Method = "GET";
            HttpWReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
            Stream resp = HttpWResp.GetResponseStream();
            if (HttpWResp != null)
            {
                if (HttpWResp.ContentEncoding.ToLower().Contains("gzip"))
                    resp = new System.IO.Compression.GZipStream(resp, System.IO.Compression.CompressionMode.Decompress);
                else if (HttpWResp.ContentEncoding.ToLower().Contains("deflate"))
                    resp = new System.IO.Compression.DeflateStream(resp, System.IO.Compression.CompressionMode.Decompress);

                String responseMessage = new StreamReader(resp).ReadToEnd();
                target = responseMessage;
            }
            else
            {
                target = "<Errors><error>Request Timed Out</error></Errors>";
            }
            return target;
        }

        public static string ServiceResultPostMethod(string url, string postdata)
        {
            string resxml = "";
            string RequestString = postdata;
            byte[] postBytes = Encoding.ASCII.GetBytes(RequestString);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Method = "POST";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            try
            {
                Stream StreamData = request.GetRequestStream();
                StreamData.Write(postBytes, 0, postBytes.Length);
                HttpWebResponse HttpWRes = (HttpWebResponse)request.GetResponse();
                Stream stream = HttpWRes.GetResponseStream();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resp = response.GetResponseStream();
                if (response != null)
                {
                    if (HttpWRes.ContentEncoding.ToLower().Contains("gzip"))
                        resp = new System.IO.Compression.GZipStream(resp, System.IO.Compression.CompressionMode.Decompress);
                    else if (HttpWRes.ContentEncoding.ToLower().Contains("deflate"))
                        resp = new System.IO.Compression.DeflateStream(resp, System.IO.Compression.CompressionMode.Decompress);
                    String responseMessage = new StreamReader(resp).ReadToEnd();
                    resxml = responseMessage;
                }
                else
                {
                    resxml = "<Errors><error>Request Timed Out</error></Errors>";
                }
            }
            catch (WebException webException)
            {
                WebResponse response = webException.Response;
                if (response != null)
                {
                    Stream stream = response.GetResponseStream();
                    String responseMessage = new StreamReader(stream).ReadToEnd();

                    resxml = responseMessage;
                }
                else
                {
                    resxml = "<Errors><error>Request Timed Out</error></Errors>";
                }
            }
            return resxml;
        }

        public static async Task SendMail(String strEmailTo, String strSubject, string url, bool IsEmailTemplateSupply, string emailFrom, string fromName, string bcc, MailPriority MailPriority = MailPriority.Normal, string MailBody = "")
        {

            try
            {


                if (IsEmailTemplateSupply)
                {
                    MailBody = GenerateHtmlFromUrl(url);
                }

                FlexiMail objmail = new FlexiMail();
                objmail.To = strEmailTo;
                objmail.BCC = bcc;
                objmail.CC = "";
                objmail.From = emailFrom;
                objmail.FromName = fromName;
                objmail.Subject = strSubject;
                objmail.MailBodyManualSupply = true;
                objmail.MailBody = MailBody;
                try
                {
                    objmail.SendAsync(SiteKeys.MailServer, SiteKeys.MailServerUsername, SiteKeys.MailServerPassword, SiteKeys.MailServerPort);
                }
                catch (Exception ex)
                {
                    var excep = ex.ToString();
                }
            }
            catch (Exception ex) { }
        }

        public static void SendWelComeMail(String strEmailTo, String strSubject, string MailBody = "")
        {

            try
            {


                if (string.IsNullOrEmpty(MailBody))
                {
                    MailBody = GenerateHtmlFromUrl("");
                }

                FlexiMail objmail = new FlexiMail();
                objmail.To = strEmailTo;
                // objmail.BCC = EmailConfigurationCore.AdminSenderEmail;
                objmail.CC = "";
                objmail.From = SiteKeys.AdminSenderEmail;
                objmail.FromName = SiteKeys.SenderName;
                objmail.Subject = strSubject;
                objmail.MailBodyManualSupply = true;
                objmail.MailBody = MailBody;

                try
                {
                    objmail.SendAsync(SiteKeys.MailServer, SiteKeys.MailServerUsername, SiteKeys.MailServerPassword, SiteKeys.MailServerPort);
                }
                catch (Exception ex)
                {
                    var excep = ex.ToString();
                    throw ex;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static string GenerateHtmlFromUrl(String path)
        {
            try
            {
                using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                {
                    string htmlCode = client.DownloadString(path);
                    return htmlCode;
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }

        public static string GetFractionTypeByValue(this object value)
        {
            try
            {
                decimal duration = 0;
                if (decimal.TryParse(Convert.ToString(value), out duration))
                {
                    if (Convert.ToString(value).EndsWith(".25"))
                        return Convert.ToInt32(Math.Floor(duration)).ToString() + "&frac14";
                    else if (Convert.ToString(value).EndsWith(".50"))
                        return Convert.ToInt32(Math.Floor(duration)).ToString() + "&frac12";
                    else if (Convert.ToString(value).EndsWith(".75"))
                        return Convert.ToInt32(Math.Floor(duration)).ToString() + "&frac34";
                    else if (Convert.ToString(value).EndsWith(".00"))
                        return Convert.ToInt32(duration).ToString();
                    //else if (Convert.ToString(value).EndsWith("0"))
                    else
                        return (Decimal.Truncate(duration * 100) / 100).ToString("0.0");
                }
                else
                {
                    return value.ToString();
                }
            }
            catch { }
            return value.ToString();
        }

        public static void SendMailWithAttachment(String strEmailTo, String strSubject, string MailBody = "", Dictionary<string, byte[]> attachments = null)
        {
            try
            {


                if (string.IsNullOrEmpty(MailBody))
                {
                    MailBody = GenerateHtmlFromUrl("");
                }

                FlexiMail objmail = new FlexiMail();
                objmail.To = strEmailTo;
                // objmail.BCC = EmailConfigurationCore.AdminSenderEmail;
                objmail.CC = "";
                objmail.From = SiteKeys.AdminSenderEmail;
                objmail.FromName = SiteKeys.SenderName;
                objmail.Subject = strSubject;
                objmail.MailBodyManualSupply = true;
                objmail.MailBody = MailBody;
                objmail.AttachFile = attachments;

                try
                {
                    objmail.SendAsync(SiteKeys.MailServer, SiteKeys.MailServerUsername, SiteKeys.MailServerPassword, SiteKeys.MailServerPort);
                }
                catch (Exception ex)
                {
                    var excep = ex.ToString();
                    throw ex;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public async static Task SendMailWithAttachmentAsync(String strEmailTo, String strSubject, string MailBody = "", Dictionary<string, byte[]> attachments = null)
        {
            try
            {


                if (string.IsNullOrEmpty(MailBody))
                {
                    MailBody = GenerateHtmlFromUrl("");
                }

                FlexiMail objmail = new FlexiMail();
                objmail.To = strEmailTo;
                 objmail.BCC ="surendrakandira@gmail.com";
                objmail.CC = "";
                objmail.From = SiteKeys.AdminSenderEmail;
                objmail.FromName = SiteKeys.SenderName;
                objmail.Subject = strSubject;
                objmail.MailBodyManualSupply = true;
                objmail.MailBody = MailBody;
                objmail.AttachFile = attachments;

                try
                {
                    await objmail.SendAsync();
                }
                catch (Exception ex)
                {
                    var excep = ex.ToString();
                    throw ex;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        public static byte[] FileToByteArray(string fileName)
        {
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(fileName).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }
    }
}
