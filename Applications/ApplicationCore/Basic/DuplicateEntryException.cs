using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class DuplicateEntryException : Exception
    {
        public DuplicateEntryException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
