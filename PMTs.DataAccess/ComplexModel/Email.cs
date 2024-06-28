using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModels
{
    public class EmailRequest
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<string> From { get; set; }
        public List<string> To { get; set; }
        public List<string> CC { get; set; } = null;
    }
}
