using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Common
{
    public class BaseIntegrationEvent
    {
        public string CorelationId { get; set; }
        public DateTime CreationDate { get; set; }
        public BaseIntegrationEvent()
        {
            CorelationId = Guid.NewGuid().ToString();
            CreationDate = DateTime.UtcNow;
        }
        public BaseIntegrationEvent(Guid corelationId, DateTime creationDate)
        {
            CorelationId = corelationId.ToString();
            CreationDate = creationDate;
        }
    }
}
