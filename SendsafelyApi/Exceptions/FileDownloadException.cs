﻿using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Generic upload exception which can be thrown when an exception is thrown during the upload or encryption phase.
    /// The most common reason for this exception is an interrupted internet connection.
    /// </summary>
    [Serializable]
    public class FileDownloadException : BaseException
    {
        public FileDownloadException()
            : base()
        { }

        public FileDownloadException(string message)
            : base(message)
        { }
    }
}
