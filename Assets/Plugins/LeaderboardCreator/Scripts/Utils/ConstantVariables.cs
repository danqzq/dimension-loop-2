using Dan.Enums;

namespace Dan
{
    internal static class ConstantVariables
    {
        internal const int MaxExtraStringLength = 100;
        
        internal static string GetServerURL(Routes route = Routes.None, string extra = "")
        {
            return ServerURL + route switch
            {
                Routes.Get => "/get",
                Routes.Upload => "/entry/upload",
                Routes.Ping => "/ping",
                _ => "/"
            } + extra;
        }

        private const string ServerURL = "https://lcv2-server.danqzq.games";
    }
}