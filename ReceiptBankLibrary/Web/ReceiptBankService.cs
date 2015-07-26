using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;
using Taxomania.ReceiptBank.Model;

namespace Taxomania.ReceiptBank.Web
{
    public sealed class ReceiptBankService
    {
        private const string BaseUrl = "https://api.receipt-bank.com/api";
        private readonly HttpClient _httpClient;

        #region Init

        public ReceiptBankService(OAuthToken oAuthToken)
        {
            if (oAuthToken.Expires != null) // TODO Check expiry
                throw new ArgumentException("Access token has expired, please refresh");
            _httpClient = new HttpClient(new HttpBaseProtocolFilter {AllowUI = false});
            _httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer",
                oAuthToken.AccessToken);
        }

        #endregion

        public IObservable<ReceiptBankReceipt> PostReceipt(IStorageFile imageFile)
        {
            return Observable.Return(new HttpMultipartFormDataContent())
                .SelectMany(async content =>
                {
                    content.Add(new HttpStreamContent(await imageFile.OpenAsync(FileAccessMode.Read)), "photo");
                    return content;
                })
                .SelectMany(
                    content =>
                        Observable.Return(new HttpRequestMessage(HttpMethod.Post, new Uri(BaseUrl + "/postReceipt"))
                        {
                            Content = content
                        }))
                .SelectMany(async request => await _httpClient.SendRequestAsync(request))
                .SelectMany(async response => await Observable.Create<ReceiptBankReceipt>(async observer =>
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.Content != null)
                        {
                            var error = JsonConvert.DeserializeObject<ReceiptBankError>(
                                await response.Content.ReadAsStringAsync());
                            observer.OnError(new ReceiptBankException
                            {
                                StatusCode = response.StatusCode,
                                Error = error
                            });
                        }
                        else
                        {
                            observer.OnError(new ArgumentException(response.StatusCode.ToString()));
                        }
                    }
                    else
                    {
                        observer.OnNext(
                            JsonConvert.DeserializeObject<ReceiptBankReceipt>(await response.Content.ReadAsStringAsync()));
                        observer.OnCompleted();
                    }
                    return Disposable.Empty;
                }));
        }

        public IObservable<IEnumerable<ReceiptBankReceiptStatus>> GetReceiptsStatus(IEnumerable<long> receiptIds)
        {
            return Observable.Return(JsonConvert.SerializeObject(new ReceiptBankGetReceipts {ReceiptIds = receiptIds}))
                .Select(content => new HttpRequestMessage(HttpMethod.Post, new Uri(BaseUrl + "/getReceiptsStatus"))
                {
                    Content = new HttpStringContent(content, UnicodeEncoding.Utf8, "application/json")
                })
                .SelectMany(async request => await _httpClient.SendRequestAsync(request))
                .SelectMany(
                    async response => await Observable.Create<IEnumerable<ReceiptBankReceiptStatus>>(async observer =>
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            if (response.Content != null)
                            {
                                var error = JsonConvert.DeserializeObject<ReceiptBankError>(
                                    await response.Content.ReadAsStringAsync());
                                observer.OnError(new ReceiptBankException
                                {
                                    StatusCode = response.StatusCode,
                                    Error = error
                                });
                            }
                            else
                            {
                                observer.OnError(new ArgumentException(response.StatusCode.ToString()));
                            }
                        }
                        else
                        {
                            observer.OnNext(
                                JsonConvert.DeserializeObject<IEnumerable<ReceiptBankReceiptStatus>>(
                                    await response.Content.ReadAsStringAsync()));
                            observer.OnCompleted();
                        }
                        return Disposable.Empty;
                    }));
        }
    }
}