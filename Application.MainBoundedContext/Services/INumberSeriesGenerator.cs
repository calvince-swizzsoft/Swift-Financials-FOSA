using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.Services
{
    public interface INumberSeriesGenerator
    {


        string GetNextNumber(string code, ServiceHeader serviceHeader);

    }
}
