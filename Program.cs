using log4net;
using MailSender.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MailSender
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string InputFolder => Path.Combine(Settings.Default.WorkPath, Settings.Default.InputFolder);

        public static string SendedFolder => Path.Combine(Settings.Default.WorkPath, Settings.Default.SendedFolder);

        public static string ErrorsFolder => Path.Combine(Settings.Default.WorkPath, Settings.Default.ErrorsFolder);

        private static void Main(string[] args)
        {
            try
            {
                Log.Info("Starting...");
                if (!Directory.Exists(InputFolder))
                {
                    Log.Warn("Input folder not found");
                    Log.Info("Stop.");
                }
                else
                {
                    if (!Directory.Exists(SendedFolder))
                        Directory.CreateDirectory(SendedFolder);
                    foreach (string filePath in GetMailsForSend())
                        ProcessFile(filePath);
                    Log.Info("Stop.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Log.Info("Stop.");
            }
        }

        private static IEnumerable<string> GetMailsForSend() => new DirectoryInfo(InputFolder).GetFiles("*.xml").OrderByDescending(fi => fi.LastWriteTime).Take(100).Select(fi => fi.FullName);

        private static void ProcessFile(string filePath)
        {
            try
            {
                Log.Info(("Process file: '" + filePath + "'"));
                MailXml message;
                using (XmlReader xmlReader = XmlReader.Create(filePath))
                    message = (MailXml)new XmlSerializer(typeof(MailXml)).Deserialize(xmlReader);
                SendMail(message);
                Log.Info(("Message to '" + message.ToEmail + "' sended!"));
                MoveFileToSended(filePath);
            }
            catch (Exception ex)
            {
                Log.Error("Sending error: ", ex);
                MoveFileToErrors(filePath);
            }
        }

        private static void MoveFileToSended(string fileName) => MoveFile(fileName, SendedFolder);

        private static void MoveFileToErrors(string fileName) => MoveFile(fileName, ErrorsFolder);

        private static void MoveFile(string fileName, string folderName)
        {
            try
            {
                string uniqueName = GetUniqueName(Path.Combine(folderName, Path.GetFileName(fileName)));
                File.Move(fileName, uniqueName);
                Log.Info(("File '" + fileName + "' moved to '" + uniqueName + "'"));
            }
            catch (Exception ex)
            {
                Log.Error("Error moving File '" + fileName + "' to '" + folderName + "'", ex);
            }
        }

        private static string GetUniqueName(string filePath)
        {
            if (!File.Exists(filePath))
                return filePath;
            int index = 0;
            string newName;
            for (newName = CreateNewName(filePath, index); File.Exists(newName); newName = CreateNewName(newName, index))
                ++index;
            return newName;
        }

        private static string CreateNewName(string filePath, int index)
        {
            string str = Path.GetFileNameWithoutExtension(filePath);
            string directoryName = Path.GetDirectoryName(filePath);
            string extension = Path.GetExtension(filePath);
            if (str.Contains("("))
                str = str.Substring(0, str.IndexOf("("));
            string path2 = string.Format("{0}({1}){2}", str, index, extension);
            return Path.Combine(directoryName, path2);
        }

        public static void SendMail(MailXml message)
        {
            using (SmtpClient smtpClient = new SmtpClient(Settings.Default.SmtpHost, Settings.Default.SmtpPort))
            {
                smtpClient.DeliveryFormat = SmtpDeliveryFormat.International;
                smtpClient.EnableSsl = Settings.Default.UseSSL;
                smtpClient.Credentials = new NetworkCredential(Settings.Default.SmtpUser, Settings.Default.SmtpPass);
                MailAddress from = new MailAddress(message.FromEmail, message.From, Encoding.UTF8);
                MailAddress to = new MailAddress(message.ToEmail, message.To, Encoding.UTF8);
                MailMessage mail = new MailMessage(from, to)
                {
                    Body = message.Body,
                    BodyEncoding = Encoding.UTF8,
                    Subject = message.Title,
                    SubjectEncoding = Encoding.UTF8
                };
                mail.ReplyToList.Add(from);
                smtpClient.Send(mail);
            }
        }
    }
}
