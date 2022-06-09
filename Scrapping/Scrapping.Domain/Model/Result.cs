using System;

namespace Scrapping.Domain.Model
{
    public class Result
    {
        public Exception Exception { get; private set; }
        public bool IsSuceed { get; private set; } = true;
        public Link Link { get; private set; }

        internal void HasFailed(Link link, Exception exception)
        {
            IsSuceed = false;
            Link = link;
            Exception = exception;
        }
    }
}
