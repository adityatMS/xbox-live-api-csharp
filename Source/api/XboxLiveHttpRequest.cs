// -----------------------------------------------------------------------
//  <copyright file="XboxLiveHttpRequest.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Net;
    using global::System.Reflection;
    using global::System.Text;
    using global::System.Threading.Tasks;

    public class XboxLiveHttpRequest
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string SignatureHeaderName = "Signature";
        private const string ETagHeaderName = "ETag";
        private const string DateHeaderName = "Date";

        private readonly XboxLiveContextSettings contextSettings;
        internal readonly HttpWebRequest webRequest;
        internal readonly Dictionary<string, string> customHeaders = new Dictionary<string, string>();

        internal XboxLiveHttpRequest(XboxLiveContextSettings settings, string method, string serverName, string pathQueryFragment)
        {
            this.Method = method;
            this.Url = serverName + pathQueryFragment;
            this.contextSettings = settings;
            this.webRequest = (HttpWebRequest)WebRequest.Create(new Uri(this.Url));

            this.SetCustomHeader("Accept-Language", CultureInfo.CurrentUICulture.ToString());

            const string userAgentType = "XSAPI";
#if !WINDOWS_UWP
            string userAgentVersion = typeof(XboxLiveHttpRequest).Assembly.GetName().Version.ToString();
            this.webRequest.UserAgent = userAgentType + "/" + userAgentVersion;
#else
            string userAgentVersion = typeof(XboxLiveHttpRequest).GetTypeInfo().Assembly.GetName().Version.ToString();
#endif

            // Current versions of XSAPI appear to send these headers instead of a standard User-Agent
            this.SetCustomHeader("x-xbl-client-type", userAgentType);
            this.SetCustomHeader("x-xbl-client-version", userAgentVersion);
        }

        public string Method { get; private set; }

        public string Url { get; private set; }

        public string ContractVersion { get; set; }

        public bool RetryAllowed { get; set; }

        private string Headers
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var header in this.customHeaders)
                {
                    sb.AppendFormat("{0}={1};", header.Key, header.Value);
                }

                return sb.ToString();
            }
        }

        public string ContentType { get; set; }

        public string RequestBody { get; set; }

        public Task<XboxLiveHttpResponse> GetResponseWithAuth(System.XboxLiveUser user, HttpCallResponseBodyType httpCallResponseBodyType)
        {
            TaskCompletionSource<XboxLiveHttpResponse> getResponseCompletionSource = new TaskCompletionSource<XboxLiveHttpResponse>();

            user.GetTokenAndSignatureAsync(this.Method, this.Url, this.Headers).ContinueWith(
                tokenTask =>
                {
#if !WINDOWS_UWP
                    var result = tokenTask.Result;
                    this.webRequest.Headers.Add(AuthorizationHeaderName, string.Format("XBL3.0 x={0};{1}", result.XboxUserHash, result.Token));
                    this.webRequest.Headers.Add(SignatureHeaderName, tokenTask.Result.Signature);

                    
                    foreach (KeyValuePair<string, string> customHeader in this.customHeaders)
                    {
                        this.webRequest.Headers.Add(customHeader.Key, customHeader.Value);
                    }
#endif

                    this.GetResponseWithoutAuth(httpCallResponseBodyType).ContinueWith(getResponseTask =>
                    {
                        getResponseCompletionSource.SetResult(getResponseTask.Result);
                    });
                });

            return getResponseCompletionSource.Task;
        }

        public virtual Task<XboxLiveHttpResponse> GetResponseWithoutAuth(HttpCallResponseBodyType httpCallResponseBodyType)
        {
#if !WINDOWS_UWP
            if (!string.IsNullOrEmpty(this.ContractVersion))
            {
                this.webRequest.Headers["x-xbl-contract-version"] = this.ContractVersion;
            }
#endif

            TaskCompletionSource<XboxLiveHttpResponse> getResponseCompletionSource = new TaskCompletionSource<XboxLiveHttpResponse>();

            this.WriteRequestBodyAsync().ContinueWith(writeBodyTask =>
            {
                Task.Factory.FromAsync(this.webRequest.BeginGetResponse, (Func<IAsyncResult, WebResponse>)this.webRequest.EndGetResponse, null)
                    .ContinueWith(getResponseTask =>
                    {
                        bool complete = getResponseTask.IsCompleted;
                        var response = new XboxLiveHttpResponse((HttpWebResponse)getResponseTask.Result, httpCallResponseBodyType);
                        getResponseCompletionSource.SetResult(response);
                    });
            });

            return getResponseCompletionSource.Task;
        }

        /// <summary>
        /// If a request body has been provided, this will write it to the stream.  If there is no request body a completed task
        /// will be returned.
        /// </summary>
        /// <returns>A task that represents to request body write work.</returns>
        /// <remarks>This is used to make request chaining a little bit easier.</remarks>
        private Task WriteRequestBodyAsync()
        {
            if (string.IsNullOrEmpty(this.RequestBody))
            {
                return Task.FromResult(true);
            }

            this.webRequest.ContentType = this.ContentType;

#if !WINDOWS_UWP
            this.webRequest.ContentLength = this.RequestBody.Length;
#endif

            // The explicit cast in the next method should not be necessary, but Visual Studio is complaining
            // that the call is ambiguous.  This removes that in-editor error. 
            return Task.Factory.FromAsync(this.webRequest.BeginGetRequestStream, (Func<IAsyncResult, Stream>)this.webRequest.EndGetRequestStream, null)
                .ContinueWith(t =>
                {
                    using (Stream body = t.Result)
                    {
                        using (StreamWriter sw = new StreamWriter(body))
                        {
                            sw.Write(this.RequestBody);
                            sw.Flush();
                        }
                    }
                });
        }

        public void SetCustomHeader(string headerName, string headerValue)
        {
            this.customHeaders[headerName] = headerValue;
        }

        public static XboxLiveHttpRequest Create(XboxLiveContextSettings settings, string httpMethod, string serverName, string pathQueryFragment)
        {
            return !XboxLiveContext.UseMockData ?
                new MockXboxLiveHttpRequest(settings, httpMethod, serverName, pathQueryFragment) :
                new XboxLiveHttpRequest(settings, httpMethod, serverName, pathQueryFragment);
        }
    }
}