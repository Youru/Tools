using System;
using System.Collections.Generic;

namespace Scrapping.Domain.Model
{
    public class Result
    {
        public bool HasSuceed { get; private set; } = true;
        public List<LinkException> LinkExceptions { get; private set; } = new List<LinkException>();

        public void HasFailed(Link link, Exception exception)
        {
            HasSuceed = false;
            LinkExceptions.Add(new LinkException(link, exception));
        }
    }

    public class LinkException
    {
        public Link Link { get; private set; }
        public Exception Exception { get; private set; }
        public LinkException(Link link, Exception exception)
        {
            Link = link;
            Exception = exception;
        }
    }
}
