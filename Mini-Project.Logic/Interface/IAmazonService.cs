using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_Project.Logic.Interface
{
    public interface IAmazonService
    {
        string WebScrapper(string uri);
        //string GetCommentDetails(string uri);
    }
}
