/*
 * Copyright (c) 2011 - 2016, Apinauten GmbH
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice, this 
 *    list of conditions and the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice, 
 *    this list of conditions and the following disclaimer in the documentation 
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * THIS FILE IS GENERATED AUTOMATICALLY. DON'T MODIFY IT.
 */
using System;
using System.Net;

namespace Com.Apiomat.Frontend
{
    public class ApiomatRequestException : Exception
    {
        private readonly int _returnCode;
        private readonly int _expectedReturnCode;
        private readonly string _reason;
        private readonly Status _status;

        public int ReturnCode { get { return _returnCode; } }
        public int ExpectedReturnCode { get { return _expectedReturnCode; } }
        public string Reason { get { return _reason; } }
        public Status Status { get { return _status; } }

        public ApiomatRequestException()
            : this(Status.NULL.StatusCode, 0, null)
        {
        }
        public ApiomatRequestException(string message)
            : this(Status.NULL.StatusCode, 0, message)
        {
        }
        public ApiomatRequestException(string message, Exception inner)
            : this(Status.NULL.StatusCode, 0, message, inner)
        {
        }

        // Custom constructors

        public ApiomatRequestException(int returnCode, HttpStatusCode expectedReturnCode, string reasonTitle, Exception inner = null)
            : this(returnCode, (int)expectedReturnCode, reasonTitle, inner)
        {
        }

        public ApiomatRequestException(int returnCode, int expectedReturnCode, string reasonTitle, Exception inner = null)
            : this(returnCode, expectedReturnCode, reasonTitle, "", inner)
        {
        }

        public ApiomatRequestException(int returnCode, HttpStatusCode expectedReturnCode, string reasonTitle, string reasonBody, Exception inner = null)
            : this(returnCode, (int)expectedReturnCode, reasonTitle, reasonBody, inner)
        {
        }

        public ApiomatRequestException(int returnCode, int expectedReturnCode, string reasonTitle, string reasonBody, Exception inner = null)
            : base(ParseReason(returnCode, expectedReturnCode, reasonTitle, reasonBody), inner)
        {
            _expectedReturnCode = expectedReturnCode;
            _returnCode = returnCode;
            _reason = Message;
            _status = Status.GetStatusForCode(returnCode);
        }
        /// <summary>
        /// Constructor for creating <see cref="Com.Apiomat.Frontend.ApiomatRequestException"/> from given <see cref="Com.Apiomat.Frontend.Status"/> and given expected code
        /// </summary>
        public ApiomatRequestException(Status status, HttpStatusCode expectedReturnCode, Exception inner = null)
            : this(status.StatusCode, (int)expectedReturnCode, status.ReasonPhrase, inner)
        {
        }

        public ApiomatRequestException(Status status, Exception inner = null)
            : this(status.StatusCode, 0, null, inner)
        {
        }

        private static string ParseReason(int returnCode, int expectedReturnCode, string reasonTitle, string reasonBody)
        {
            string result = "Return code " + returnCode + " does not match expected one (" + expectedReturnCode + ")";
            Status s = Status.GetStatusForCode(returnCode);

            if (reasonTitle != null && reasonTitle.Length > 0)
            {
                result = reasonTitle + " " + reasonBody;
            }
            else if (s != null)
            {
                result = s.ReasonPhrase;
            }
            return result;
        }


    }
}

