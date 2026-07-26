using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class SPARROW : ValueObject<SPARROW>
    {
        public string MessageType { get; private set; }

        public string SRCIMD { get; private set; }

        public string DeviceId { get; private set; }

        public string Date { get; private set; }

        public string Time { get; private set; }

        public string CardNumber { get; private set; }

        public string Message { get; private set; }

        public decimal Amount { get; private set; }

        public SPARROW(string messageType, string src_imd, string deviceId, string date, string time, string cardNumber, string message, decimal amount)
        {
            this.MessageType = messageType;
            this.SRCIMD = src_imd;
            this.DeviceId = deviceId;
            this.Date = date;
            this.Time = time;
            this.CardNumber = cardNumber;
            this.Message = message;
            this.Amount = amount;
        }

        private SPARROW()
        { }
    }
}
