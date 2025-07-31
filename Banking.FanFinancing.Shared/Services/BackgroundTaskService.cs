using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Banking.FanFinancing.Shared.Services
{
    public class BackgroundTaskService : BackgroundService
    {
        private readonly string requestResponseLogsPath;
        public IBackgroundTaskQueue TaskQueue { get; }
        int count = 0;
        public BackgroundTaskService(IBackgroundTaskQueue taskQueue, IConfiguration config)
        {

            TaskQueue = taskQueue;
            requestResponseLogsPath = config.GetSection("LogFilePath").GetSection("RequestResponseLogPath").Value ?? "";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                count++;
                var guid = Guid.NewGuid().ToString();
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    await BackgroundTasksException($"Error occurred executing {nameof(workItem)} => {guid}", nameof(workItem), ex, guid);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }

        public async Task BackgroundTasksException(string Error, string ControllerPath, Exception ex, string Guid)
        {
            string myFileName = String.Format("{0}__{1}", DateTime.Now.ToString("yyyyMMddHH").Substring(0, 9), "BankIslami_BackgroundTasksException.txt");
            string myFullPath = Path.Combine(requestResponseLogsPath, myFileName);
            Directory.CreateDirectory(requestResponseLogsPath.ToString());
            var source = new StackTrace(ex)?.GetFrame(0)?.GetMethod();
            if (!File.Exists(myFullPath))
            {
                using (StreamWriter sw = File.CreateText(myFullPath))
                {
                    await sw.WriteLineAsync(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ffff tt") + " |Task: " + ControllerPath);
                    await sw.WriteLineAsync("Error : " + Error);
                    await sw.WriteLineAsync("Request ID : " + Guid);
                    await sw.WriteLineAsync("Source : " + ControllerPath);
                    await sw.WriteLineAsync("Error Path : " + $"{source?.DeclaringType?.FullName}/{source?.Name}");
                    await sw.WriteLineAsync("Error Message : " + ex.Message);
                    await sw.WriteLineAsync("Error Line : " + new StackTrace(ex, true).GetFrame(0));
                    await sw.WriteLineAsync("Inner Exception : " + Convert.ToString(ex?.InnerException));
                    await sw.WriteLineAsync("Error Details : " + ex?.ToString());
                    await sw.WriteLineAsync("\n\n");
                }
            }
            else
            {
                using (var sw = File.AppendText(myFullPath))
                {
                    await sw.WriteLineAsync(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ffff tt") + " |Task: " + ControllerPath);
                    await sw.WriteLineAsync("Error : " + Error);
                    await sw.WriteLineAsync("Request ID : " + Guid);
                    await sw.WriteLineAsync("Source : " + ControllerPath);
                    await sw.WriteLineAsync("Error Path : " + $"{source?.DeclaringType?.FullName}/{source?.Name}");
                    await sw.WriteLineAsync("Error Message : " + ex.Message);
                    await sw.WriteLineAsync("Error Line : " + new StackTrace(ex, true).GetFrame(0));
                    await sw.WriteLineAsync("Inner Exception : " + Convert.ToString(ex?.InnerException));
                    await sw.WriteLineAsync("Error Details : " + ex?.ToString());
                    await sw.WriteLineAsync("\n\n");
                }
            }
        }

        public void LogBackgroundTasks(string Data, string ControllerPath = "==> ")
        {
            try
            {
                string myFileName = String.Format("{0}__{1}", DateTime.Now.ToString("yyyyMMdd"), "BackgroundTaskLogs.txt");
                string myFullPath = Path.Combine(requestResponseLogsPath, myFileName);
                Directory.CreateDirectory(requestResponseLogsPath);
                if (!File.Exists(myFullPath))
                {
                    using StreamWriter sw = File.CreateText(myFullPath);
                    sw.WriteLine(Data);
                }
                else
                {
                    using var sw = File.AppendText(myFullPath);
                    sw.WriteLine(Data);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
