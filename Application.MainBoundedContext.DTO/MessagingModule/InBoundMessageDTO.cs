using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class InboundMessageDTO
    {
        public string Phone { get; set; }
        
        public int ReferenceNumber { get; set; }
        
        public int SequenceNumber { get; set; }
        
        public string SMSC { get; set; }
        
        public string TextMessage { get; set; }
        
        public DateTime TimeStamp { get; set; }
        
        public string TimeStampRFC { get; set; }
        
        public int TimeZone { get; set; }

        public int TotalParts { get; set; }
    }
}
