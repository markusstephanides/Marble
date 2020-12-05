using System.Collections.Generic;
using DotLiquid;

namespace Marble.Messaging.Generation.Models
{
    public class ClientTemplateModel : Drop
    {
        public List<string> UsingDirectives { get; set; }

        public string Namespace { get; set; }

        public string ClassName { get; set; }

        public string ServiceName { get; set; }

        public List<Hash> Procedures { get; set; }
    }
}