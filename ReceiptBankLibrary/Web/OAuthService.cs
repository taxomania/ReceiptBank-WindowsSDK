using System;
using System.Linq;
using System.Reactive.Linq;
using Windows.Web.Http;
using Newtonsoft.Json;
using Taxomania.ReceiptBank.Model;

namespace Taxomania.ReceiptBank.Web
{
    public static class OAuthService
    {
        private const string OauthEndpoint = "https://api.receipt-bank.com/oauth";
        private const string OauthSandboxEndpoint = "https://app.rb-logistics.com/oauth";

        public static Uri GetOAuthCodeUri(string clientId, string redirectUri, bool sandbox = false)
        {
            var endpoint = (sandbox) ? OauthSandboxEndpoint : OauthEndpoint;
            return new Uri($"{endpoint}/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}",
                UriKind.Absolute);
        }

        public static bool TryGetAccessCodeFromUri(Uri uri, string redirectUri, out string accessCode, out string error)
        {
            var callbackUri = string.Format($"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}");
            if (callbackUri.Equals(redirectUri))
            {
                var queryParams = uri.Query.Substring(1).Split('&');
                foreach (var queryParam in queryParams.Where(queryParam => queryParam.StartsWith("code=")))
                {
                    accessCode =
                        queryParam.Substring(queryParam.IndexOf("code=", StringComparison.Ordinal) + "code=".Length);
                    error = null;
                    return true;
                }
                foreach (var queryParam in queryParams.Where(queryParam => queryParam.StartsWith("error=")))
                {
                    error =
                        queryParam.Substring(queryParam.IndexOf("error=", StringComparison.Ordinal) + "error=".Length);
                    accessCode = null;
                    return false;
                }
            }
            accessCode = null;
            error = null;
            return false;
        }

        public static IObservable<OAuthToken> GetAccessToken(string clientId, string clientSecret,
            string redirectUri, string accessCode, bool sandbox = false)
        {
            var endpoint = (sandbox) ? OauthSandboxEndpoint : OauthEndpoint;
            return Observable.Return(new Uri(
                $"{endpoint}/access_token?grant_type=authorization_code&client_id={clientId}&client_secret={clientSecret}&redirect_uri={redirectUri}&code={accessCode}",
                UriKind.Absolute))
                .SelectMany(OAuth2);
        }

        private static IObservable<OAuthToken> OAuth2(Uri uri)
        {
            // TODO: x-www-form-urlencoded?
            return Observable.Return(new HttpRequestMessage(HttpMethod.Get, uri))
                .SelectMany(async request =>
                {
                    var client = new HttpClient();
                    return await client.SendRequestAsync(request);
                })
                .SelectMany(async response =>
                {
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<OAuthToken>(await response.Content.ReadAsStringAsync());
                });
        }

        public static IObservable<OAuthToken> RefreshAccessToken(string clientId, string clientSecret,
            string redirectUri, string refreshToken, bool sandbox = false)
        {
            var endpoint = (sandbox) ? OauthSandboxEndpoint : OauthEndpoint;
            return Observable.Return(new Uri(
                $"{endpoint}/access_token?grant_type=refresh_token&client_id={clientId}&client_secret={clientSecret}&redirect_uri={redirectUri}&refresh_token={refreshToken}",
                UriKind.Absolute))
                .SelectMany(OAuth2);
        }
    }
}