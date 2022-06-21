using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_TCP_Device
{
    public class ConnectionConstants
    {
        public const int APP_PORT = 8060;                   // TCP Port
        public const int UDP_SEARCH_PORT = 9010;            // UDP порт для отправки данных
        public const string UDP_SEARCH_IP = "235.5.5.254";  // UDP IP address
        public const string APP_ID = "#Q#U#I#Z";  // Код приложения для UDP
        public const string DISCONNECT = "#e#n#d";
        public const string NEW_CLIENT_REQUEST = "#n#e#w";
        public const string NEW_CLIENT_ANSWER = "#y#e#s";
    }

    public class AppConstants
    {
        public const string REFRESH_TEAM_RECORD = "#r#e#f";
        public const string RENAME_TEAM = "#r#e#n";
        public const string ADD_NEW_TEAM = "#a#d#d";
        public const string DELETE_TEAM = "#d#e#l";
        public const string NEW_TABLE = "#t#a#b";
        public const string FULLSCREEN_MODE = "#f#s#m";
    }
}
