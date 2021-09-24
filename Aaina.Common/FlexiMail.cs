using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Generic;
using MimeKit.Text;
using System.Net;

namespace Aaina.Common
{
    public class FlexiMail
    {
        #region Constructors-Destructors
        public FlexiMail()
        {
            //set defaults 
            myEmail = new MimeMessage();
            _MailBodyManualSupply = false;
        }

        //private string SmtpServer => SiteKeys.SmtpServer;
        //private string SMTPUserName => SiteKeys.SMTPUserName;
        //private string SMTPUserPassword => SiteKeys.SMTPUserPassword;
        //private string SmtpPort => SiteKeys.SmtpPort;
        #endregion

        #region  Class Data
        private string _From;
        private string _FromName;
        private string _To;
        private string _ToList;
        private string _Subject;
        private string _CC;
        private string _CCList;
        private string _BCC;
        private string _ReplyTo;
        private string _TemplateDoc;
        private string[] _ArrValues;
        private string _BCCList;
        private bool _MailBodyManualSupply;
        private string _MailBody;
        //private string _Attachment;
        private Dictionary<string, byte[]> _Attachment;
        private MimeMessage myEmail;

        #endregion

        #region Properties
        public string From
        {
            set { _From = value; }
        }
        public string FromName
        {
            set { _FromName = value; }
        }
        public string To
        {
            set { _To = value; }
        }

        public string Subject
        {
            set { _Subject = value; }
        }
        public string CC
        {
            set { _CC = value; }
        }
        public string BCC
        {

            set { _BCC = value; }
        }
        public string ReplyTo
        {
            set { _ReplyTo = value; }
        }
        public bool MailBodyManualSupply
        {

            set { _MailBodyManualSupply = value; }
        }
        public string MailBody
        {
            set { _MailBody = value; }
        }
        public string EmailTemplateFileName
        {
            //FILE NAME OF TEMPLATE ( MUST RESIDE IN ../EMAILTEMPLAES/ FOLDER ) 
            set { _TemplateDoc = value; }
        }
        public string[] ValueArray
        {
            //ARRAY OF VALUES TO REPLACE VARS IN TEMPLATE 
            set { _ArrValues = value; }
        }

        public Dictionary<string, byte[]> AttachFile
        {
            set { _Attachment = value; }
        }

        #endregion

        #region SEND EMAIL

        public void SendAsync(string SmtpServer, string SMTPUserName, string SMTPUserPassword, int SMTPPort)
        {

            List<InternetAddress> _toList = new List<InternetAddress>();
            List<InternetAddress> _fromList = new List<InternetAddress>();
            List<InternetAddress> _ccList = new List<InternetAddress>();
            List<InternetAddress> _bccList = new List<InternetAddress>();
            //set mandatory properties 
            if (_FromName == "")
                _FromName = _From;
            _fromList.Add(InternetAddress.Parse(_From));
            //myEmail.From = new MimeMessage(_From, _FromName);

            myEmail.Subject = _Subject;
            //---Set recipients in To List 
            _ToList = _To.Replace(";", ",");
            if (_ToList != "")
            {
                string[] arr = _ToList.Split(',');
                myEmail.To.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        _toList.Add(InternetAddress.Parse(address));
                    }
                }
                else
                {
                    _toList.Add(InternetAddress.Parse(_ToList));
                }
            }

            //---Set recipients in CC List 
            if (!String.IsNullOrWhiteSpace(_CC))
            {
                _CCList = _CC.Replace(";", ",");
                if (_CCList != "")
                {
                    string[] arr = _CCList.Split(',');
                    //myEmail.CC.Clear();
                    if (arr.Length > 0)
                    {
                        foreach (string address in arr)
                        {
                            _ccList.Add(InternetAddress.Parse(address));
                        }
                    }
                    else
                    {
                        _ccList.Add(InternetAddress.Parse(_CCList));
                    }
                }
            }

            //---Set recipients in BCC List 
            if (!String.IsNullOrWhiteSpace(_BCC))
            {
                _BCCList = _BCC.Replace(";", ",");
                if (_BCCList != "")
                {
                    string[] arr = _BCCList.Split(',');
                    myEmail.Bcc.Clear();
                    if (arr.Length > 0)
                    {
                        foreach (string address in arr)
                        {
                            _ccList.Add(InternetAddress.Parse(address));
                        }
                    }
                    else
                    {
                        _ccList.Add(InternetAddress.Parse(_BCCList));
                    }
                }
            }
            var builder = new BodyBuilder();
            //set mail body 
            if (_MailBodyManualSupply)
            {
                builder.HtmlBody = _MailBody;
            }
            else
            {
                builder.HtmlBody = GetHtml(_TemplateDoc);
            }

            // set attachment 
            if (_Attachment != null)
            {
                foreach (var attachment in _Attachment)
                {
                    builder.Attachments.Add(attachment.Key, attachment.Value);
                }

            }
            myEmail.Body = builder.ToMessageBody();
            myEmail = new MimeMessage(_fromList, _toList, _Subject, myEmail.Body);
            //Send mail 
            //SmtpClient client = new SmtpClient();
            //client.Host = SmtpServer;
            //client.Port = SMTPPort;
            //client.Credentials = new NetworkCredential(SMTPUserName, SMTPUserPassword);
            //client.Timeout = 0;
            ////client.EnableSsl = EmailConfigurationCore.EnableSsl;
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;            
            //await client.SendMailAsync(myEmail);

            using var smtp = new SmtpClient();
            smtp.Connect(SmtpServer, SMTPPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(SMTPUserName, SMTPUserPassword);
            smtp.Send(myEmail);
            smtp.Disconnect(true);
        }

        public async Task SendAsync()
        {

            List<InternetAddress> _toList = new List<InternetAddress>();
            List<InternetAddress> _fromList = new List<InternetAddress>();
            List<InternetAddress> _ccList = new List<InternetAddress>();
            List<InternetAddress> _bccList = new List<InternetAddress>();
            //set mandatory properties 
            if (_FromName == "")
                _FromName = _From;
            _fromList.Add(InternetAddress.Parse(_From));
            //myEmail.From = new MimeMessage(_From, _FromName);

            myEmail.Subject = _Subject;
            //---Set recipients in To List 
            _ToList = _To.Replace(";", ",");
            if (_ToList != "")
            {
                string[] arr = _ToList.Split(',');
                myEmail.To.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        _toList.Add(InternetAddress.Parse(address));
                    }
                }
                else
                {
                    _toList.Add(InternetAddress.Parse(_ToList));
                }
            }

            //---Set recipients in CC List 
            if (!String.IsNullOrWhiteSpace(_CC))
            {
                _CCList = _CC.Replace(";", ",");
                if (_CCList != "")
                {
                    string[] arr = _CCList.Split(',');
                    //myEmail.CC.Clear();
                    if (arr.Length > 0)
                    {
                        foreach (string address in arr)
                        {
                            _ccList.Add(InternetAddress.Parse(address));
                        }
                    }
                    else
                    {
                        _ccList.Add(InternetAddress.Parse(_CCList));
                    }
                }
            }

            //---Set recipients in BCC List 
            if (!String.IsNullOrWhiteSpace(_BCC))
            {
                _BCCList = _BCC.Replace(";", ",");
                if (_BCCList != "")
                {
                    string[] arr = _BCCList.Split(',');
                    myEmail.Bcc.Clear();
                    if (arr.Length > 0)
                    {
                        foreach (string address in arr)
                        {
                            _ccList.Add(InternetAddress.Parse(address));
                        }
                    }
                    else
                    {
                        _ccList.Add(InternetAddress.Parse(_BCCList));
                    }
                }
            }
            var builder = new BodyBuilder();
            //set mail body 
            if (_MailBodyManualSupply)
            {
                builder.TextBody = _MailBody;
            }
            else
            {
                builder.HtmlBody = GetHtml(_TemplateDoc);
            }

            // set attachment 
            if (_Attachment != null)
            {
                foreach (var attachment in _Attachment)
                {
                    builder.Attachments.Add(attachment.Key, attachment.Value);
                }

            }
            myEmail.Body = builder.ToMessageBody();
            myEmail = new MimeMessage(_fromList, _toList, _Subject, myEmail.Body);

            await SendAsync(myEmail);
            
        }
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(SiteKeys.MailServer, SiteKeys.MailServerPort, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(SiteKeys.MailServerUsername, SiteKeys.MailServerPassword);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public async Task<bool> MailSendVerify(string host, short port, string userName, string password, bool isSSLRequired, MimeMessage mail)
        {
            try
            {
                //SmtpClient client = new SmtpClient();
                //client.Host = host;
                //client.Port = port;
                //client.Credentials = new NetworkCredential(userName, password);
                //client.Timeout = 0;
                //client.EnableSsl = isSSLRequired;
                //client.UseDefaultCredentials = true;
                //await client.SendMailAsync(mail);

                using var smtp = new SmtpClient();
                smtp.Connect(host, port, SecureSocketOptions.StartTls);
                smtp.Authenticate(userName, password);
                smtp.Send(mail);
                smtp.Disconnect(true);

                return true;
            }
            catch
            {

                return false;
            }

        }

        #endregion

        #region GetHtml
        public string GetHtml(string argTemplateDocument)
        {
            int i;
            StreamReader filePtr;
            string fileData = argTemplateDocument;

            //filePtr = File.OpenText(HttpContext.Current.Server.MapPath("~/EmailTemplate/") + argTemplateDocument);

            filePtr = File.OpenText("~/EmailTemplate/" + argTemplateDocument);

            //filePtr = File.OpenText(ConfigurationSettings.AppSettings["EMLPath"] + argTemplateDocument);
            fileData = filePtr.ReadToEnd();


            filePtr.Close();
            filePtr = null;
            if ((_ArrValues == null))
            {

                return fileData;
            }
            else
            {
                //fileData = fileData.Replace("##user##", _ArrValues[0].ToString());
                //fileData = fileData.Replace("##question##", _ArrValues[1].ToString());

                for (i = _ArrValues.GetLowerBound(0); i <= _ArrValues.GetUpperBound(0); i++)
                {

                    fileData = fileData.Replace("@v" + i.ToString() + "@", (string)_ArrValues[i]);
                }
                return fileData;
            }


        }


        /// <summary>
        /// Reads contents of a URL
        /// </summary>
        /// <param name="url"></param>
        public static String GetHtmlFromURL(String url)
        {
            HttpWebRequest myWebRequest = null;
            HttpWebResponse myWebResponse = null;
            Stream receiveStream = null;
            Encoding encode = null;
            StreamReader readStream = null;
            string text = null;

            try
            {
                myWebRequest = HttpWebRequest.Create(url) as HttpWebRequest;

                // myWebRequest.Timeout = TIMEOUT;
                // myWebRequest.ReadWriteTimeout = TIMEOUT;

                myWebResponse = myWebRequest.GetResponse() as HttpWebResponse;
                receiveStream = myWebResponse.GetResponseStream();
                encode = System.Text.Encoding.GetEncoding("utf-8");
                readStream = new StreamReader(receiveStream, encode);
                text = readStream.ReadToEnd(); //.ToLower();
                if (readStream != null) readStream.Close();
                if (receiveStream != null) receiveStream.Close();
                if (myWebResponse != null) myWebResponse.Close();
            }
            catch (Exception)
            {
                //Do Something
            }
            finally
            {
                readStream = null;
                receiveStream = null;
                myWebResponse = null;
                myWebRequest = null;
            }
            return text;
        }

        #endregion

    }
}
