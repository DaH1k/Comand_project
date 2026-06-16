using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    public static class UserSession
    {
        public static string CurrentUsername { get; set; } = "Гість";
        public static string CurrentStatus { get; set; } = "Offline";
    }
}

