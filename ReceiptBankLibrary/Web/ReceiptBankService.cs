﻿using System;
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
            // if (oAuthToken.Expires != null) // TODO Check expiry
            //   throw new ArgumentException("Access token has expired, please refresh");

            var rootFilter = new HttpBaseProtocolFilter
            {
                AllowUI = false
            };
            rootFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            rootFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            _httpClient = new HttpClient(rootFilter);
            _httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer",
                oAuthToken.AccessToken);
        }

        #endregion

        public IObservable<ReceiptBankReceipt> PostReceipt(IStorageFile imageFile)
        {
            return Observable.Return(new HttpMultipartFormDataContent())
                .SelectMany(async content =>
                {
                    content.Add(new HttpStreamContent(await imageFile.OpenAsync(FileAccessMode.Read)), "photo",
                        imageFile.Name);
                    return content;
                })
                .SelectMany(
                    content =>
                        Observable.Return(new HttpRequestMessage(HttpMethod.Post, new Uri(BaseUrl + "/postReceipt"))
                        {
                            Content = content
                        }))
                .SelectMany(async request => await _httpClient.SendRequestAsync(request))
                .SelectMany(ParseResponseObservable<ReceiptBankReceipt>);
        }

        public IObservable<IEnumerable<ReceiptBankReceiptStatus>> GetReceiptsStatus(IEnumerable<long> receiptIds)
        {
            return Observable.Create<RBReceiptId>(observer =>
            {
                foreach (var receiptId in receiptIds)
                {
                    observer.OnNext(new RBReceiptId {ReceiptId = receiptId});
                }
                observer.OnCompleted();
                return Disposable.Empty;
            })
                .ToList()
                .Select(ids => JsonConvert.SerializeObject(new RBGetReceiptsStatus {ReceiptIds = ids}))
                .Select(content => new HttpRequestMessage(HttpMethod.Post, new Uri(BaseUrl + "/getReceiptsStatus"))
                {
                    Content = new HttpStringContent(content, UnicodeEncoding.Utf8, "application/json")
                })
                .SelectMany(async request => await _httpClient.SendRequestAsync(request))
                .SelectMany(ParseResponseObservable<IEnumerable<ReceiptBankReceiptStatus>>);
        }

        public IObservable<ReceiptBankReceipts<ReceiptBankDetailedReceipt>> GetReceipts(long? receiptId = null,
            bool? newOnly = null)
        {
            return Observable.Return(new RBGetReceipts
            {
                ReceiptId = receiptId,
                New = newOnly
            })
                .Select(fetch =>
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    return JsonConvert.SerializeObject(fetch, settings);
                })
                .Select(content => new HttpRequestMessage(HttpMethod.Post, new Uri(BaseUrl + "/receipts"))
                {
                    Content = new HttpStringContent(content, UnicodeEncoding.Utf8, "application/json")
                })
                .SelectMany(async request => await _httpClient.SendRequestAsync(request))
                .SelectMany(ParseResponseObservable<ReceiptBankReceipts<ReceiptBankDetailedReceipt>>);
        }

        public IObservable<ReceiptBankReceipts<ReceiptBankDetailedInboxReceipt>> GetInboxReceipts()
        {
            return Observable.Return(new HttpRequestMessage(HttpMethod.Post, new Uri(BaseUrl + "/inbox")))
                .SelectMany(async request => await _httpClient.SendRequestAsync(request))
                .SelectMany(ParseResponseObservable<ReceiptBankReceipts<ReceiptBankDetailedInboxReceipt>>);
        }

        private static IObservable<T> ParseResponseObservable<T>(HttpResponseMessage response)
        {
            return Observable.Create<T>(async observer =>
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.Content != null)
                    {
                        var contentType = response.Content.Headers.ContentType.MediaType;
                        if ("application/json".Equals(contentType))
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
                            observer.OnError(new Exception(await response.Content.ReadAsStringAsync()));
                        }
                    }
                    else
                    {
                        observer.OnError(new ArgumentException(response.StatusCode.ToString()));
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var error = JsonConvert.DeserializeObject<ReceiptBankError>(content,
                            new JsonSerializerSettings
                            {
                                MissingMemberHandling = MissingMemberHandling.Error
                            });
                        observer.OnError(new ReceiptBankException
                        {
                            StatusCode = response.StatusCode,
                            Error = error
                        });
                    }
                    catch (JsonSerializationException)
                    {
                        observer.OnNext(
                            JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings
                            {
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            }));
                        observer.OnCompleted();
                    }
                }
                return Disposable.Empty;
            });
        }
    }
}