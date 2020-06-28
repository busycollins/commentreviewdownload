using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using System.Text.RegularExpressions;
using System.Linq;
using Mini_Project.Logic.Models;
using System.Data;
using System.IO;

namespace Mini_Project.Logic.Services
{
    public class YotubeService : IYotubeService
    {
        YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBi0ZdfevBX-9VmyjVmt1S4u5M4pLuq9D8" });

        public string SearchReplies(Uri url)
        {
            List<Detail> data = new List<Detail>();
            string videoId = ExtractVideoIdFromUri(url);
            YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBi0ZdfevBX-9VmyjVmt1S4u5M4pLuq9D8" });
            List<string[]> comments = new List<string[]>();
            var commentThreadsListRequest = youtube.CommentThreads.List("snippet,replies"); commentThreadsListRequest.VideoId = videoId;
            commentThreadsListRequest.Order = CommentThreadsResource.ListRequest.OrderEnum.Time;
            commentThreadsListRequest.MaxResults = 1000;
            var commentThreadsListResult = commentThreadsListRequest.Execute();
            string nextPageToken = commentThreadsListResult.NextPageToken;
            int CommentListCount = 0;
            while (nextPageToken != null)
            {
               // nextPageToken = commentThreadsListResult.NextPageToken;
                //commentThreadsListResult = commentThreadsListRequest.Execute();
                commentThreadsListRequest.PageToken = nextPageToken;
                commentThreadsListRequest.VideoId = videoId;
                commentThreadsListRequest.MaxResults = 1000;
                commentThreadsListRequest.Order = CommentThreadsResource.ListRequest.OrderEnum.Time;
                var nextPageResult = commentThreadsListRequest.Execute();
                nextPageToken = nextPageResult.NextPageToken;

                foreach (var result in nextPageResult.Items)
                    commentThreadsListResult.Items.Add(result);
                CommentListCount = commentThreadsListResult.Items.Count();

            }

            string generateCsv = string.Empty;

            foreach (var CommentThread in commentThreadsListResult.Items)
            {
               
                var commentListRequest = youtube.Comments.List("snippet");
                 commentListRequest.Id = CommentThread.Id;
                var commentListResult = commentListRequest.Execute();
                // var topcomments = CommentThread.Snippet.TopLevelComment;
                var Username = commentListResult.Items.Select(x => x.Snippet.AuthorDisplayName).FirstOrDefault();
                var date = commentListResult.Items.Select(x => x.Snippet.PublishedAt).FirstOrDefault();
                var rating = commentListResult.Items.Select(x => x.Snippet.ViewerRating).FirstOrDefault();
                var comment = commentListResult.Items.Select(x => x.Snippet.TextDisplay).FirstOrDefault();
                Detail details = new Detail();
                details.UserName = Username;
                details.Comment = comment;
                details.Datetime = date;
                details.Rating = rating;
                //generateCsv = GenerateCSVString(detail);
                data.Add(details);

            }
            generateCsv = CreateDataentryTable(data);
            return generateCsv;
        }
        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        private static Regex regexExtractId = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
        private static string[] validAuthorities = { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };

        public string ExtractVideoIdFromUri(Uri uri)
        {
            try
            {
                string authority = new UriBuilder(uri).Uri.Authority.ToLower();

                //check if the url is a youtube url
                if (validAuthorities.Contains(authority))
                {
                    //and extract the id
                    var regRes = regexExtractId.Match(uri.ToString());
                    if (regRes.Success)
                    {
                        return regRes.Groups[1].Value;
                    }
                }
            }
            catch { }


            return null;
        }
        //private string GenerateCSVString(Details details)
        //{
        //    // var Data = GetPersonInfo();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("UserName");
        //    sb.Append(",");
        //    sb.Append("Date");
        //    sb.Append(",");
        //    sb.Append("Star rating");
        //    sb.Append(",");
        //    sb.Append("Comment");
        //    sb.Append(",");
        //    sb.Append("Link");
        //    sb.AppendLine();

        //        sb.Append(details.UserName);
        //        sb.Append(",");
        //        sb.Append(details.Comment);
        //    sb.Append(",");
        //    sb.Append(details.Datetime);
        //    sb.Append(",");
        //        sb.AppendLine();

        //    return sb.ToString();
        //}
        public string CreateDataentryTable(List<Detail> details)
        {
            try
            {
                DataTable dt = new DataTable("Data");
                dt.Columns.Add("Username");
                dt.Columns.Add("Review");
                dt.Columns.Add("star Rating");
                dt.Columns.Add("Link");
                dt.Columns.Add("Date");


                var youtubeData = details.ToList();
                foreach (var data in youtubeData)
                {

                    dt.Rows.Add(data.UserName, data.Comment, data.Rating, data.Link, data.Datetime
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

