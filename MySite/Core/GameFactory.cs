using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using MySite.Models;
using Newtonsoft.Json;

namespace MySite.Core
{
    public class GameFactory
    {
        /// <summary>
        /// Receiving the information about files from directory for the Grid Memory cards
        /// </summary>
        /// <returns></returns>
        public static async Task<FileDetail[]> GetFilesAsync(string rootPath)
        {
            await Task.Yield();
            var filePath = HostingEnvironment.MapPath($"~{rootPath}filesInfo.json");
            if (filePath != null)
            {
                if (File.Exists(filePath))
                {
                    string str = await ReadTextAsync(filePath);
                    var result = JsonConvert.DeserializeObject<FileDetail[]>(str);
                    return result;
                }

                throw new NotImplementedException(
                    $"Into the specified location {rootPath} was not found filesInfo.json file!");
            }

            throw new NotImplementedException(
                $"The server doesn't know anything about the specified location {rootPath}!");
        }

        private static async Task<string> ReadTextAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StringBuilder sb = new StringBuilder();
                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }

        public static List<CardInfo> ConverToCards(int limit, string path, string lang, FileDetail[] files)
        {
            var result = new List<CardInfo>();
            if (files != null && limit > 0)
            {
                var random = new Random();
                var resCount = limit < files.Length ? limit : files.Length;
                var restfiles = files.OrderBy(c => random.Next()).Take(resCount);
                var cards = restfiles.Select(
                    f => new CardInfo
                    {
                        Url = path + f.path,
                        Name = f.langs.Where(l => l.name == lang).DefaultIfEmpty(f.langs.First())
                            .Select(l => l.value).FirstOrDefault()
                    });
                result.AddRange(cards);
            }
            return result;
        }
    }
}