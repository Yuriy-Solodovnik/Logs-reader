namespace Logs_reader
{
    public class IISLogRecord
    {
        public string Date { get; set; }

        public string Time { get; set; }

        public string ServerIp { get; set; }

        public string HttpVerb { get; set; }

        public string Uri { get; set; }

        public string Query { get; set; }

        public string Port { get; set; }

        public string Username { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public string Referrer { get; set; }

        public string StatusCode { get; set; }

        public string SubStatusCode { get; set; }

        public string Win32StatusCode { get; set; }

        public int TimeTaken { get; set; }
    }
}