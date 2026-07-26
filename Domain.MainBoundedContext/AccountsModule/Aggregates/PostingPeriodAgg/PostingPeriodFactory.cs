using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg
{
    public static class PostingPeriodFactory
    {
        public static PostingPeriod CreatePostingPeriod(string description, Duration duration)
        {
            var postingPeriod = new PostingPeriod();

            postingPeriod.GenerateNewIdentity();

            postingPeriod.Description = description;

            postingPeriod.Duration = duration;

            postingPeriod.CreatedDate = DateTime.Now;

            postingPeriod.UnLock();

            return postingPeriod;
        }
    }
}
