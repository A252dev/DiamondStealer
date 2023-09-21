using System.Diagnostics;
using System.Linq.Expressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DiamondStealer
{
    public class Builder
    {
        public static async Task StartBuild(ITelegramBotClient botClient, Update update, string[] data)
        {
            var message = update.Message;

            string path = @".\script.cs";
            string code = @"using System;

namespace Example
{
    class Text
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine(" + '"' + data[1] + '"' + ");\r\n            System.Console.ReadLine();\r\n        }\r\n    }\r\n}";


            string buildName = "DiamondBuild.exe";
            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine(code);
            writer.Close();

            fileStream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);
            /*Console.WriteLine(reader.ReadToEnd());*/
            reader.Close();

            string compiler = @".\Framework64\v4.0.30319\csc.exe";
            string outCompiler = $" /out:{buildName} {path}";
            string compileCommand = compiler + outCompiler;

            ProcessStartInfo psi = new ProcessStartInfo("cmd", "/c " + compileCommand);
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            /*string output = process.StandardOutput.ReadToEnd();*/
            process.WaitForExit();

            /*Console.WriteLine(output);
            Console.WriteLine("Done");*/

            using (FileStream file = new FileStream(buildName, FileMode.Open))
            {
                var filename = Path.GetFileName(buildName);

                await botClient.SendDocumentAsync(
                    chatId: message.Chat.Id,
                    document: InputFile.FromStream(file, filename),
                    caption: "Сборка успешно завершена!");

                file.Close();

                try
                {
                    System.IO.File.Delete(buildName);
                    /*Console.WriteLine("File deleted successfully.");*/
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                return;
            }            
        }
    }
}
