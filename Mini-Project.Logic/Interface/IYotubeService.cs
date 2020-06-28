using System;
using System.Collections.Generic;
using System.Text;

namespace Mini_Project.Logic
{
    public interface IYotubeService
    {
        string SearchReplies(Uri url);
        string ExtractVideoIdFromUri(Uri uri);
    }
}
