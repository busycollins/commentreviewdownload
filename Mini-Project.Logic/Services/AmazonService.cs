using HtmlAgilityPack;
using Mini_Project.Logic.Interface;
using Mini_Project.Logic.Models;
using ScrapySharp.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Mini_Project.Logic.Services
{
    public class AmazonService : IAmazonService
    {
        public string WebScrapper(string uri)
        {
            WebRequest request = WebRequest.Create(uri);

            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            //content from the server was Encoded with Gzip
            var t = ReadFully(dataStream);
            var y = Decompress(t);
            var ms = new MemoryStream(y);
            StreamReader reader = new StreamReader(ms);

            string responseFromServer = reader.ReadToEnd();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseFromServer);
            List<AmazonPageDetails> data = new List<AmazonPageDetails>();            //var title = htmlDocument.DocumentNode.SelectNodes("//div[@id='cm-cr-dp-review-list']");
            var reviews = htmlDocument.DocumentNode.SelectNodes("//span[@data-hook='review-body']");
            var ratings = htmlDocument.DocumentNode.SelectNodes("//i[@data-hook='review-star-rating']");
            var reviewDate = htmlDocument.DocumentNode.SelectNodes("//span[@data-hook= 'review-date']");
            var author = htmlDocument.DocumentNode.SelectNodes("//span[@class='a-profile-name']");
            string generatecsv = string.Empty;
          
            List<AmazonPageDetails> detail = new List<AmazonPageDetails>();
          
            //After webscrapping the website could only get the top 8 reviews
            //There should be another way of getting all the comment because of time limit this is the only way I could think of now
            for (int i = 0; i < reviews.Count;i++)
            {
                AmazonPageDetails details2 = new AmazonPageDetails();
                details2.Reviews = reviews[i].InnerText.Replace("\n\n\n\n\n\n\n\n  \n  \n ", "");
                details2.Ratings = ratings[i].InnerText;
                details2.ReviewDates = reviewDate[i].InnerText;
                details2.UserNames = author[i].InnerText;
                detail.Add(details2);
            }

           
            generatecsv = CreateDataentryTable(detail);

            return generatecsv;
            // return webPage.Html;
        }
        //public HtmlNode GetHtml(string uri)
        //{
        //    var getHtml = WebScrapper(uri);
        //    return getHtml;
        //}

        //public string GetCommentDetails(string uri)
        //{
        //    var html = GetHtml(uri);

        //    var prop = html.OwnerDocument.DocumentNode.SelectSingleNode("//html/head/title").InnerText;
        //    return prop;
        //}
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
        public string CreateDataentryTable(List<AmazonPageDetails> details)
        {
            try
            {
                DataTable dt = new DataTable("Data");
                dt.Columns.Add("Username");
                dt.Columns.Add("Review");
                dt.Columns.Add("star Rating");
                dt.Columns.Add("Date");


                var Data = details;
                foreach (var data in Data)
                {

                    dt.Rows.Add(data.UserNames, data.Reviews, data.Ratings.Substring(0,3), data.ReviewDates
                        );
                }



                StringBuilder builder = new StringBuilder();
                List<string> columnNames = new List<string>();
                List<string> rows = new List<string>();

                foreach (DataColumn column in dt.Columns)
                {
                    columnNames.Add(column.ColumnName);
                }
                builder.Append(string.Join(",", columnNames.ToArray())).Append("\n");

                foreach (DataRow row in dt.Rows)
                {
                    List<string> currentRow = new List<string>();

                    foreach (DataColumn column in dt.Columns)
                    {
                        object item = row[column];
                        currentRow.Add(item.ToString());
                    }

                    rows.Add(string.Join(",", currentRow.ToArray()));
                }
                var file = builder.Append(string.Join("\n", rows.ToArray()));


                return file.ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
