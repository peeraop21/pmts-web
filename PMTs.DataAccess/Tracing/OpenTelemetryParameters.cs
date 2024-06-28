namespace PMTs.DataAccess.Tracing
{
    public class OpenTelemetryParameters
    {
        public string ActivitySourceName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public string ServiceInstanceId { get; set; }
        public bool RecordException { get; set; }
    }
}
