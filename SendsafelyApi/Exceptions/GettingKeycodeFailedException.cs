﻿using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Thrown when we failed to create and add a public key
    /// </summary>
    [Serializable]
    public class GettingKeycodeFailedException : BaseException
    {
        public GettingKeycodeFailedException(string message)
            : base(message)
        {
            ;
        }
    }

}
