using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Infrastructure.Services
{
    public interface IPlugin
    {
        Guid Id { get; }

        string Description { get; }

        void DoWork(IScheduler scheduler, params string[] args);

        void Exit();
    }
}
