using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Threading;
using System.Linq;

namespace Proformance_monitor
{
    // Ensure that PerformanceMonitor is public or internal
    

    public partial class PreformanceMonitor : ServiceBase
    {

        private Timer timer;
        private PerformanceMonitor performanceMonitor;
        private string logFilePath = "C:\\Users\\Blu-Ray\\Desktop\\OSoutput\\Logs\\Log.txt"; // Change this to your desired log file path
        private string dataFilePath = "C:\\Users\\Blu-Ray\\Desktop\\OSoutput\\PCWorkloadData.txt"; // Change this to your desired data file path

        public PreformanceMonitor()
        {
            InitializeComponent();
            performanceMonitor = new PerformanceMonitor();
        }

        protected override void OnStart(string[] args)
        {

            timer = new Timer(CollectAndSendData, null, 0, 12*60*60*1000); // 12 hours in milliseconds
        }

        protected override void OnStop()
        {
        }

        private void CollectAndSendData(object state)
        {
            try
            {
                // Collect PC workload data
                string workloadData = performanceMonitor.CollectWorkloadData();

                // Save the actual data to a text file
                SaveDataToFile(workloadData, dataFilePath);

                // Save data to a text file
                string filePath = SaveDataToFile(workloadData);

                // Send an email with the text file attached
                SendEmail(filePath);

                // Log success event to the log file
                LogToFile("PC Workload data sent successfully.");
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                // Log the exception to the log file
                LogToFile($"Error during data collection and email sending: {ex.Message}");
            }
        }

        private void SaveDataToFile(string data, string filePath)
        {
            File.WriteAllText(filePath, data);
        }

        private string SaveDataToFile(string data)
        {
            string filePath = "C:\\Users\\Blu-Ray\\Desktop\\OSoutput\\PCWorkloadData.txt"; // Change this to your desired data file path
            SaveDataToFile(data, filePath);
            return filePath;
        }
        private void LogToFile(string message)
        { 
            
            
           using (StreamWriter writer = new StreamWriter(logFilePath, true)){


                 writer.WriteLine($"{DateTime.Now} - {message}");
           }
            
            
        }
        private void SendEmail(string attachmentFilePath)
        {
            string recipientEmail = "Example@gmail.com";
            string senderEmail = "Example2gmail.com";
            string senderPassword = "Sender_Pass";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(recipientEmail);
                mail.Subject = "PC Workload Data";
                mail.Body = "Please find attached the PC workload data.";

                // Attach the text file with PC workload data
                Attachment attachment = new Attachment(attachmentFilePath);
                mail.Attachments.Add(attachment);

                // Use Google's SMTP server
                using (SmtpClient smtp = new SmtpClient("smtp.office365.com", 587))//google no longer allows Less Secure Apps acess option , Couldn't find a way 
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);
                }
            }
        }


        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
    public class PerformanceMonitor
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private PerformanceCounter hddCounter;
        private PerformanceCounter networkCounter;

        public PerformanceMonitor()
        {
            // Initialize performance counters
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            hddCounter = new PerformanceCounter("LogicalDisk", "% Free Space", "C:"); // Change "C:" to your desired drive
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            if (networkInterface != null)
            {
                networkCounter = new PerformanceCounter("Network Interface", "Bytes Total/sec", networkInterface.Description);
            }
        }

        public string CollectWorkloadData()
        {
            // Collect workload data
            float cpuUsage = cpuCounter.NextValue();
            float ramAvailable = ramCounter.NextValue();
            float hddFreeSpace = hddCounter.NextValue();
            float networkUsage = networkCounter.NextValue();

            // Create a string with the collected data
            return $"CPU Usage: {cpuUsage}%\nRAM Available: {ramAvailable} MB\nHDD Free Space: {hddFreeSpace}%\nNetwork Usage: {networkUsage} Bytes/sec";
        }
    }
}
