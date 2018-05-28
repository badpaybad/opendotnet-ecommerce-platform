using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;

namespace Core.FrontEnd.Plugins
{
    public class PaymentMethodInternational : IPaymentMethod
    {
        public Guid Id { get { return Guid.Parse("0f8c1e8c-f2bb-40ca-991c-6aee3deb282c"); } }

        public string GetRedirectUrl(PaymentMethod config, Guid paymentTransactionId, string orderCode, long amount
            , string ipAddress, Guid languageId, string siteDomainUrl)
        {
            var secureSecret = Config.SecureSecret;
            var apiUrl = Config.ApiPayUrl;
            var accessCode = Config.AccessCode;
            var merchant = Config.Merchant;
            var returnUrl = Config.ReturnUrl;

            var siteUrl = (string.IsNullOrEmpty(siteDomainUrl) ? "http://onepay.vn" : siteDomainUrl).Trim(new[] { ' ', '/' });

            if (returnUrl.IndexOf("http://", StringComparison.OrdinalIgnoreCase) < 0
                || returnUrl.IndexOf("https://", StringComparison.OrdinalIgnoreCase) < 0)
            {
                returnUrl = siteUrl + "/" + Config.ReturnUrl.Trim(new[] { ' ', '/',':' });
            }

            var langUi = "en";

            VPCRequest conn = new VPCRequest(apiUrl);
            conn.SetSecureSecret(secureSecret);

            // Add the Digital Order Fields for the functionality you wish to use
            // Core Transaction Fields
            conn.AddDigitalOrderField("AgainLink", siteUrl);
            conn.AddDigitalOrderField("Title", merchant);

            conn.AddDigitalOrderField("vpc_Locale", langUi);//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
            conn.AddDigitalOrderField("vpc_Version", Config.PayVersion);
            conn.AddDigitalOrderField("vpc_Command", "pay");
            conn.AddDigitalOrderField("vpc_Merchant", merchant);
            conn.AddDigitalOrderField("vpc_AccessCode", accessCode);
            conn.AddDigitalOrderField("vpc_MerchTxnRef", paymentTransactionId.ToString());
            conn.AddDigitalOrderField("vpc_OrderInfo", orderCode);
            conn.AddDigitalOrderField("vpc_Amount", (amount * 100).ToString());

            conn.AddDigitalOrderField("vpc_ReturnURL", returnUrl);
            // Thong tin them ve khach hang. De trong neu khong co thong tin
            conn.AddDigitalOrderField("vpc_SHIP_Street01", "");
            conn.AddDigitalOrderField("vpc_SHIP_Provice", "");
            conn.AddDigitalOrderField("vpc_SHIP_City", "");
            conn.AddDigitalOrderField("vpc_SHIP_Country", "");
            conn.AddDigitalOrderField("vpc_Customer_Phone", "");
            conn.AddDigitalOrderField("vpc_Customer_Email", "");
            conn.AddDigitalOrderField("vpc_Customer_Id", "");
            // Dia chi IP cua khach hang
            conn.AddDigitalOrderField("vpc_TicketNo", ipAddress);
            // Chuyen huong trinh duyet sang cong thanh toan
            String url = conn.Create3PartyQueryString();

            return url;
        }

        public Enums.ShoppingCartPayStatus QueryDr(Guid paymentTransactionId)
        {
            var accessCode = Config.AccessCode;
            var merchant = Config.Merchant;
            var user = Config.User;
            var password = Config.Password;

            string[,] MyArray =
            {
                {"vpc_AccessCode",accessCode},
                {"vpc_Command","queryDR" },
                {"vpc_MerchTxnRef",paymentTransactionId.ToString()},
                {"vpc_Merchant",merchant},
                {"vpc_Password",password},
                {"vpc_User",user},
                {"vpc_Version",Config.QueryDrVersion}
            };
            var httpServerUtility = HttpContext.Current.Server;

            string postData = "";
            string seperator = "";
            int paras = 7;
            for (int i = 0; i < paras; i++)
            {
                postData = postData + seperator + httpServerUtility.UrlEncode(MyArray[i, 0]) + "=" + httpServerUtility.UrlEncode(MyArray[i, 1]);
                seperator = "&";
            }

            var apiQueryUrl = Config.ApiQueryUrl;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiQueryUrl);

            //  WebRequest request = WebRequest.Create(vpc_Host);
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            //string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.         
            request.UserAgent = "HTTP Client";
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            //    WebResponse response = request.GetResponse();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            //   Response.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            // Display the content.
            //  Response.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            while (responseFromServer.IndexOf(" ") >= 0)
            {
                responseFromServer = responseFromServer.Replace(" ", "");
            }

            if (responseFromServer.IndexOf("vpc_DRExists=N", StringComparison.OrdinalIgnoreCase) >= 0) { }
            if (responseFromServer.IndexOf("vpc_DRExists=Y", StringComparison.OrdinalIgnoreCase) >= 0) { }

            //todo: consider auto update payment transaction status

            if (responseFromServer.IndexOf("vpc_TxnResponseCode=0", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //MemoryMessageBuss.PushCommand(new SuccessPaymentTransaction(paymentTransactionId));
                return Enums.ShoppingCartPayStatus.PaymentSuccess;
            }

            //MemoryMessageBuss.PushCommand(new FailPaymentTransaction(paymentTransactionId));
            return Enums.ShoppingCartPayStatus.PaymentFail;
        }

        public static OnepayIntResult ProcessDr(string vpc_MerchTxnRef, string vpc_OrderInfo, string vpc_Amount)
        {
            vpc_MerchTxnRef = (vpc_MerchTxnRef ?? string.Empty).Trim();
            vpc_Amount = (vpc_Amount ?? string.Empty).Trim();

            var paymentTranasctionId = Guid.Empty;
            PaymentTransaction pt = null;
            ShoppingCart cart = null;
            if (Guid.TryParse(vpc_MerchTxnRef, out paymentTranasctionId))
            {
                using (var db = new CoreEcommerceDbContext())
                {
                    pt = db.PaymentTransactions.SingleOrDefault(i => i.Id == paymentTranasctionId);
                    cart = db.ShoppingCarts.SingleOrDefault(
                        i => i.OrderCode.Equals(vpc_OrderInfo, StringComparison.OrdinalIgnoreCase));
                }
            }
            if (string.IsNullOrEmpty(vpc_MerchTxnRef) || string.IsNullOrEmpty(vpc_Amount)
                || paymentTranasctionId == Guid.Empty || pt == null || cart == null)
            {
                return new PaymentMethodInternational.OnepayIntResult()
                {
                    Message = "Not found payment transaction: " + vpc_MerchTxnRef + " for order code: " + vpc_OrderInfo,
                    Success = false,
                    Status = Enums.ShoppingCartPayStatus.PaymentFail
                };
            }
            //if (pt.Amount != long.Parse(vpc_Amount))
            //{
            //    return new PaymentMethodInternational.OnepayIntResult()
            //    {
            //        Message = "Invalid amount for payment transaction: " + vpc_MerchTxnRef + " for order code: " + vpc_OrderInfo,
            //        Success = false,
            //        Status = Enums.ShoppingCartPayStatus.Fail
            //    };
            //}

            var apiPayUrl = Config.ApiPayUrl;

            string secureSecret = Config.SecureSecret;
            string hashvalidateResult = "";

            // Khoi tao lop thu vien
            PaymentMethodInternational.VPCRequest conn = new PaymentMethodInternational.VPCRequest(apiPayUrl);
            conn.SetSecureSecret(secureSecret);
            // Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(System.Web.HttpContext.Current.Request.QueryString);
            // Lay gia tri tham so tra ve tu cong thanh toan
            String vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode", "");
            string amount = conn.GetResultField("vpc_Amount", "");
            string localed = conn.GetResultField("vpc_Locale", "");
            string command = conn.GetResultField("vpc_Command", "");
            string version = conn.GetResultField("vpc_Version", "");
            string cardType = conn.GetResultField("vpc_Card", "");
            string orderInfo = conn.GetResultField("vpc_OrderInfo", "");
            string merchantID = conn.GetResultField("vpc_Merchant", "");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId", "");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "");
            string transactionNo = conn.GetResultField("vpc_TransactionNo", "");
            string acqResponseCode = conn.GetResultField("vpc_AcqResponseCode", "");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message", "");

            var messageResult = string.Empty;
            var model = new PaymentMethodInternational.OnepayIntResult()
            {
                Message = messageResult,
                Success = false
            };
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                model.Message = "Transaction was paid successful";
                model.Success = true;
                model.Status = Enums.ShoppingCartPayStatus.PaymentSuccess;
                MemoryMessageBuss.PushCommand(new SuccessPaymentTransaction(paymentTranasctionId));
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                model.Message = "Transaction is pending";
                model.Status = Enums.ShoppingCartPayStatus.PaymentProcess;
                MemoryMessageBuss.PushCommand(new ProcessPaymentTransaction(paymentTranasctionId));
            }
            else
            {
                model.Message = "Transaction was not paid successful";
                model.Status = Enums.ShoppingCartPayStatus.PaymentFail;
                MemoryMessageBuss.PushCommand(new FailPaymentTransaction(paymentTranasctionId));
            }

            model.Message += "<br>" + message + $"<br>({vpc_TxnResponseCode} {transactionNo} {acqResponseCode})"
                + $"<br>{vpc_MerchTxnRef} {vpc_OrderInfo}";

            model.Amount = long.Parse(vpc_Amount) / 100;
            model.OrderCode = vpc_OrderInfo;

            return model;
        }

        public class VPCRequest
        {
            Uri _address;
            SortedList<String, String> _requestFields = new SortedList<String, String>(new VPCStringComparer());
            String _rawResponse;
            SortedList<String, String> _responseFields = new SortedList<String, String>(new VPCStringComparer());
            String _secureSecret;


            public VPCRequest(String URL)
            {
                _address = new Uri(URL);
            }

            public void SetSecureSecret(String secret)
            {
                _secureSecret = secret;
            }

            public void AddDigitalOrderField(String key, String value)
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _requestFields.Add(key, value);
                }
            }

            public String GetResultField(String key, String defValue)
            {
                String value;
                if (_responseFields.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return defValue;
                }
            }

            public String GetResultField(String key)
            {
                return GetResultField(key, "");
            }

            private String GetRequestRaw()
            {
                StringBuilder data = new StringBuilder();
                foreach (KeyValuePair<string, string> kvp in _requestFields)
                {
                    if (!String.IsNullOrEmpty(kvp.Value))
                    {
                        data.Append(kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value) + "&");
                    }
                }
                //remove trailing & from string
                if (data.Length > 0)
                    data.Remove(data.Length - 1, 1);
                return data.ToString();
            }

            public string GetTxnResponseCode()
            {
                return GetResultField("vpc_TxnResponseCode");
            }

            //_____________________________________________________________________________________________________
            // Three-Party order transaction processing

            public String Create3PartyQueryString()
            {
                StringBuilder url = new StringBuilder();
                //Payment Server URL
                url.Append(_address);
                url.Append("?");
                //Create URL Encoded request string from request fields 
                url.Append(GetRequestRaw());
                //Hash the request fields
                url.Append("&vpc_SecureHash=");
                url.Append(CreateSHA256Signature(true));
                return url.ToString();
            }

            public string Process3PartyResponse(System.Collections.Specialized.NameValueCollection nameValueCollection)
            {
                foreach (string item in nameValueCollection)
                {
                    if (!item.Equals("vpc_SecureHash") && !item.Equals("vpc_SecureHashType"))
                    {
                        _responseFields.Add(item, nameValueCollection[item]);
                    }

                }

                if (!nameValueCollection["vpc_TxnResponseCode"].Equals("0") && !String.IsNullOrEmpty(nameValueCollection["vpc_Message"]))
                {
                    if (!String.IsNullOrEmpty(nameValueCollection["vpc_SecureHash"]))
                    {
                        if (!CreateSHA256Signature(false).Equals(nameValueCollection["vpc_SecureHash"]))
                        {
                            return "INVALIDATED";
                        }
                        return "CORRECTED";
                    }
                    return "CORRECTED";
                }

                if (String.IsNullOrEmpty(nameValueCollection["vpc_SecureHash"]))
                {
                    return "INVALIDATED";//no sercurehash response
                }
                if (!CreateSHA256Signature(false).Equals(nameValueCollection["vpc_SecureHash"]))
                {
                    return "INVALIDATED";
                }
                return "CORRECTED";
            }

            //_____________________________________________________________________________________________________

            private class VPCStringComparer : IComparer<String>
            {
                /*
                 <summary>Customised Compare Class</summary>
                 <remarks>
                 <para>
                 The Virtual Payment Client need to use an Ordinal comparison to Sort on 
                 the field names to create the SHA256 Signature for validation of the message. 
                 This class provides a Compare method that is used to allow the sorted list 
                 to be ordered using an Ordinal comparison.
                 </para>
                 </remarks>
                 */

                public int Compare(String a, String b)
                {
                    /*
                     <summary>Compare method using Ordinal comparison</summary>
                     <param name="a">The first string in the comparison.</param>
                     <param name="b">The second string in the comparison.</param>
                     <returns>An int containing the result of the comparison.</returns>
                     */

                    // Return if we are comparing the same object or one of the 
                    // objects is null, since we don't need to go any further.
                    if (a == b) return 0;
                    if (a == null) return -1;
                    if (b == null) return 1;

                    // Ensure we have string to compare
                    string sa = a as string;
                    string sb = b as string;

                    // Get the CompareInfo object to use for comparing
                    System.Globalization.CompareInfo myComparer = System.Globalization.CompareInfo.GetCompareInfo("en-US");
                    if (sa != null && sb != null)
                    {
                        // Compare using an Ordinal Comparison.
                        return myComparer.Compare(sa, sb, System.Globalization.CompareOptions.Ordinal);
                    }
                    throw new ArgumentException("a and b should be strings.");
                }
            }

            //______________________________________________________________________________
            // SHA256 Hash Code

            public string CreateSHA256Signature(bool useRequest)
            {
                // Hex Decode the Secure Secret for use in using the HMACSHA256 hasher
                // hex decoding eliminates this source of error as it is independent of the character encoding
                // hex decoding is precise in converting to a byte array and is the preferred form for representing binary values as hex strings. 
                byte[] convertedHash = new byte[_secureSecret.Length / 2];
                for (int i = 0; i < _secureSecret.Length / 2; i++)
                {
                    convertedHash[i] = (byte)Int32.Parse(_secureSecret.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }

                // Build string from collection in preperation to be hashed
                StringBuilder sb = new StringBuilder();
                SortedList<String, String> list = (useRequest ? _requestFields : _responseFields);
                foreach (KeyValuePair<string, string> kvp in list)
                {
                    if (kvp.Key.StartsWith("vpc_") || kvp.Key.StartsWith("user_"))
                        sb.Append(kvp.Key + "=" + kvp.Value + "&");
                }
                // remove trailing & from string
                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);

                // Create secureHash on string
                string hexHash = "";
                using (HMACSHA256 hasher = new HMACSHA256(convertedHash))
                {
                    byte[] hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                    foreach (byte b in hashValue)
                    {
                        hexHash += b.ToString("X2");
                    }
                }
                return hexHash;
            }
        }

        public class OnepayIntResult
        {
            public string Message;
            public bool Success;
            public Enums.ShoppingCartPayStatus Status;
            public string OrderCode;
            public long Amount;
        }

        public class Config
        {
            public static string ApiPayUrl = System.Configuration.ConfigurationManager.AppSettings["OnepayIntApiPayUrl"] ?? "https://mtf.onepay.vn/vpcpay/vpcpay.op";
            public static string ApiQueryUrl = System.Configuration.ConfigurationManager.AppSettings["OnepayIntApiQueryUrl"] ?? "https://mtf.onepay.vn/vpcpay/Vpcdps.op";
            public static string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["OnepayIntApiReturnUrl"] ?? "/Payment/OnepayInternationalDr";

            public static string SecureSecret = System.Configuration.ConfigurationManager.AppSettings["OnepayIntSecureSecret"] ?? "6D0870CDE5F24F34F3915FB0045120DB";

            public static string AccessCode = System.Configuration.ConfigurationManager.AppSettings["OnepayIntAccessCode"] ?? "6BEB2546";

            public static string Merchant = System.Configuration.ConfigurationManager.AppSettings["OnepayIntMerchant"] ?? "TESTONEPAY";
            public static string User = System.Configuration.ConfigurationManager.AppSettings["OnepayIntMerchant"] ?? "TESTONEPAY";
            public static string Password = System.Configuration.ConfigurationManager.AppSettings["OnepayIntMerchant"] ?? "TESTONEPAY";
            public static string QueryDrVersion = System.Configuration.ConfigurationManager.AppSettings["OnepayIntQueryDrVertion"] ?? "1";
            public static string PayVersion = System.Configuration.ConfigurationManager.AppSettings["OnepayIntPayVertion"] ?? "2";

        }
    }

    public class PaymentMethodDomestic : IPaymentMethod
    {
        public Guid Id { get { return Guid.Parse("4867d5c4-0d01-43d3-8cb0-12af8ae023bd"); } }

        public string GetRedirectUrl(PaymentMethod config, Guid paymentTransactionId, string orderCode, long amount, string ipAddress,
            Guid languageId, string siteDomainUrl)
        {
            var siteUrl = (string.IsNullOrEmpty(siteDomainUrl) ? "http://onepay.vn" : siteDomainUrl).Trim(new[] { ' ', '/' });
            var returnUrl = Config.ReturnUrl;
            if (returnUrl.IndexOf("http://", StringComparison.OrdinalIgnoreCase) < 0
                || returnUrl.IndexOf("https://", StringComparison.OrdinalIgnoreCase) < 0)
            {
                returnUrl = siteUrl + "/" + Config.ReturnUrl.Trim(new[] { ' ', '/',':' });
            }

            string secureSecret = Config.SecureSecret;

            // Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan
            VPCRequest conn = new VPCRequest(Config.ApiPayUrl);
            conn.SetSecureSecret(secureSecret);
            // Add the Digital Order Fields for the functionality you wish to use
            // Core Transaction Fields
            conn.AddDigitalOrderField("Title", "onepay paygate");
            conn.AddDigitalOrderField("vpc_Locale", "en");//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
            conn.AddDigitalOrderField("vpc_Version", Config.PayVersion);
            conn.AddDigitalOrderField("vpc_Command", "pay");
            conn.AddDigitalOrderField("vpc_Merchant", Config.Merchant);
            conn.AddDigitalOrderField("vpc_AccessCode", Config.AccessCode);
            conn.AddDigitalOrderField("vpc_MerchTxnRef", paymentTransactionId.ToString());
            conn.AddDigitalOrderField("vpc_OrderInfo", orderCode);
            conn.AddDigitalOrderField("vpc_Amount", (amount * 100).ToString());
            conn.AddDigitalOrderField("vpc_Currency", "VND");
            conn.AddDigitalOrderField("vpc_ReturnURL", returnUrl);
            // Thong tin them ve khach hang. De trong neu khong co thong tin
            conn.AddDigitalOrderField("vpc_SHIP_Street01", "");//"194 Tran Quang Khai");
            conn.AddDigitalOrderField("vpc_SHIP_Provice", "");//"Hanoi");
            conn.AddDigitalOrderField("vpc_SHIP_City", "");//"Hanoi");
            conn.AddDigitalOrderField("vpc_SHIP_Country", "");//"Vietnam");
            conn.AddDigitalOrderField("vpc_Customer_Phone", "");//"043966668");
            conn.AddDigitalOrderField("vpc_Customer_Email", "");//"support@onepay.vn");
            conn.AddDigitalOrderField("vpc_Customer_Id", "");// "onepay_paygate");
            // Dia chi IP cua khach hang
            conn.AddDigitalOrderField("vpc_TicketNo", ipAddress);
            // Chuyen huong trinh duyet sang cong thanh toan
            String url = conn.Create3PartyQueryString();

            return url;
        }

        public Enums.ShoppingCartPayStatus QueryDr(Guid paymentTransactionId)
        {
            string postData = "";
            string seperator = "";
            string resQS = "";
            int paras = 7;
            string vpcURL = Config.ApiQueryUrl;


            string[,] MyArray =
            {
                {"vpc_AccessCode",Config.AccessCode},
                {"vpc_Command","queryDR" },
                {"vpc_MerchTxnRef",paymentTransactionId.ToString()},
                {"vpc_Merchant",Config.Merchant},
                {"vpc_Password",Config.Password},
                {"vpc_User",Config.User},
                {"vpc_Version",Config.QueryDrVersion}
            };

            var httpServerUtility = HttpContext.Current.Server;
            for (int i = 0; i < paras; i++)
            {
                postData = postData + seperator + httpServerUtility.UrlEncode(MyArray[i, 0]) + "=" + httpServerUtility.UrlEncode(MyArray[i, 1]);
                seperator = "&";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(vpcURL);

            //  WebRequest request = WebRequest.Create(vpc_Host);
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            //string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.         
            request.UserAgent = "HTTP Client";
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            //    WebResponse response = request.GetResponse();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            //   Response.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            // Display the content.
            //  Response.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            while (responseFromServer.IndexOf(" ") >= 0)
            {
                responseFromServer = responseFromServer.Replace(" ", "");
            }

            if (responseFromServer.IndexOf("vpc_DRExists=N", StringComparison.OrdinalIgnoreCase) >= 0) { }
            if (responseFromServer.IndexOf("vpc_DRExists=Y", StringComparison.OrdinalIgnoreCase) >= 0) { }

            //todo: consider auto update payment transaction status

            if (responseFromServer.IndexOf("vpc_TxnResponseCode=0", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //MemoryMessageBuss.PushCommand(new SuccessPaymentTransaction(paymentTransactionId));
                return Enums.ShoppingCartPayStatus.PaymentSuccess;
            }

            if (responseFromServer.IndexOf("vpc_TxnResponseCode=300", StringComparison.OrdinalIgnoreCase) >= 0
                || responseFromServer.IndexOf("vpc_TxnResponseCode=100", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //MemoryMessageBuss.PushCommand(new ProcessPaymentTransaction(paymentTransactionId));
                return Enums.ShoppingCartPayStatus.PaymentProcess;
            }

            //MemoryMessageBuss.PushCommand(new FailPaymentTransaction(paymentTransactionId));
            return Enums.ShoppingCartPayStatus.PaymentFail;
        }


        public static OnepayDomResult ProcessDr(string vpc_MerchTxnRef, string vpc_OrderInfo, string vpc_Amount)
        {
            vpc_MerchTxnRef = (vpc_MerchTxnRef ?? string.Empty).Trim();
            vpc_Amount = (vpc_Amount ?? string.Empty).Trim();

            var paymentTranasctionId = Guid.Empty;
            PaymentTransaction pt = null;
            ShoppingCart cart = null;
            if (Guid.TryParse(vpc_MerchTxnRef, out paymentTranasctionId))
            {
                using (var db = new CoreEcommerceDbContext())
                {
                    pt = db.PaymentTransactions.SingleOrDefault(i => i.Id == paymentTranasctionId);
                    cart = db.ShoppingCarts.SingleOrDefault(
                        i => i.OrderCode.Equals(vpc_OrderInfo, StringComparison.OrdinalIgnoreCase));
                }
            }
            if (string.IsNullOrEmpty(vpc_MerchTxnRef) || string.IsNullOrEmpty(vpc_Amount)
                || paymentTranasctionId == Guid.Empty || pt == null || cart == null)
            {
                return new OnepayDomResult()
                {
                    Message = "Not found payment transaction: " + vpc_MerchTxnRef + " for order code: " + vpc_OrderInfo,
                    Success = false,
                    Status = Enums.ShoppingCartPayStatus.PaymentFail
                };
            }
            //if (pt.Amount != long.Parse(vpc_Amount))
            //{
            //    return new PaymentMethodInternational.OnepayIntResult()
            //    {
            //        Message = "Invalid amount for payment transaction: " + vpc_MerchTxnRef + " for order code: " + vpc_OrderInfo,
            //        Success = false,
            //        Status = Enums.ShoppingCartPayStatus.Fail
            //    };
            //}
            string secureSecret = Config.SecureSecret;
            string hashvalidateResult = "";
            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(Config.ApiPayUrl);
            conn.SetSecureSecret(secureSecret);
            // Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(HttpContext.Current.Request.QueryString);

            // Lay gia tri tham so tra ve tu cong thanh toan
            String vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode", "");
            string amount = conn.GetResultField("vpc_Amount", "");
            string localed = conn.GetResultField("vpc_Locale", "");
            string command = conn.GetResultField("vpc_Command", "");
            string version = conn.GetResultField("vpc_Version", "");
            string cardBin = conn.GetResultField("vpc_Card", "");
            string orderInfo = conn.GetResultField("vpc_OrderInfo", "");
            string merchantID = conn.GetResultField("vpc_Merchant", "");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId", "");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "");
            string transactionNo = conn.GetResultField("vpc_TransactionNo", "");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message", "");

            var model = new OnepayDomResult();

            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                model.Message = "Transaction was paid successful";
                model.Status = Enums.ShoppingCartPayStatus.PaymentSuccess;
                model.Success = true;
                MemoryMessageBuss.PushCommand(new SuccessPaymentTransaction(paymentTranasctionId));
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                model.Message = "Transaction is pending";
                model.Status = Enums.ShoppingCartPayStatus.PaymentProcess;
                model.Success = false;

                MemoryMessageBuss.PushCommand(new ProcessPaymentTransaction(paymentTranasctionId));
            }
            else
            {
                model.Message = "Transaction was not paid successful";
                model.Status = Enums.ShoppingCartPayStatus.PaymentFail;
                model.Success = false;

                MemoryMessageBuss.PushCommand(new FailPaymentTransaction(paymentTranasctionId));
            }
            model.Message += "<br>" + message + $"<br>({vpc_TxnResponseCode} {transactionNo})"
                             + $"<br>{vpc_MerchTxnRef} {vpc_OrderInfo}";
            model.Amount = long.Parse(vpc_Amount) / 100;
            model.OrderCode = vpc_OrderInfo;
            return model;
        }


        public class OnepayDomResult
        {
            public string Message;
            public bool Success;
            public Enums.ShoppingCartPayStatus Status;
            public string OrderCode;
            public long Amount;
        }

        public class VPCRequest
        {
            Uri _address;
            SortedList<String, String> _requestFields = new SortedList<String, String>(new VPCStringComparer());
            String _rawResponse;
            SortedList<String, String> _responseFields = new SortedList<String, String>(new VPCStringComparer());
            String _secureSecret;


            public VPCRequest(String URL)
            {
                _address = new Uri(URL);
            }

            public void SetSecureSecret(String secret)
            {
                _secureSecret = secret;
            }

            public void AddDigitalOrderField(String key, String value)
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _requestFields.Add(key, value);
                }
            }

            public String GetResultField(String key, String defValue)
            {
                String value;
                if (_responseFields.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return defValue;
                }
            }

            public String GetResultField(String key)
            {
                return GetResultField(key, "");
            }

            private String GetRequestRaw()
            {
                StringBuilder data = new StringBuilder();
                foreach (KeyValuePair<string, string> kvp in _requestFields)
                {
                    if (!String.IsNullOrEmpty(kvp.Value))
                    {
                        data.Append(kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value) + "&");
                    }
                }
                //remove trailing & from string
                if (data.Length > 0)
                    data.Remove(data.Length - 1, 1);
                return data.ToString();
            }

            public string GetTxnResponseCode()
            {
                return GetResultField("vpc_TxnResponseCode");
            }

            //_____________________________________________________________________________________________________
            // Three-Party order transaction processing

            public String Create3PartyQueryString()
            {
                StringBuilder url = new StringBuilder();
                //Payment Server URL
                url.Append(_address);
                url.Append("?");
                //Create URL Encoded request string from request fields 
                url.Append(GetRequestRaw());
                //Hash the request fields
                url.Append("&vpc_SecureHash=");
                url.Append(CreateSHA256Signature(true));
                return url.ToString();
            }

            public string Process3PartyResponse(System.Collections.Specialized.NameValueCollection nameValueCollection)
            {
                foreach (string item in nameValueCollection)
                {
                    if (!item.Equals("vpc_SecureHash") && !item.Equals("vpc_SecureHashType"))
                    {
                        _responseFields.Add(item, nameValueCollection[item]);
                    }

                }

                if (!nameValueCollection["vpc_TxnResponseCode"].Equals("0") && !String.IsNullOrEmpty(nameValueCollection["vpc_Message"]))
                {
                    if (!String.IsNullOrEmpty(nameValueCollection["vpc_SecureHash"]))
                    {
                        if (!CreateSHA256Signature(false).Equals(nameValueCollection["vpc_SecureHash"]))
                        {
                            return "INVALIDATED";
                        }
                        return "CORRECTED";
                    }
                    return "CORRECTED";
                }

                if (String.IsNullOrEmpty(nameValueCollection["vpc_SecureHash"]))
                {
                    return "INVALIDATED";//no sercurehash response
                }
                if (!CreateSHA256Signature(false).Equals(nameValueCollection["vpc_SecureHash"]))
                {
                    return "INVALIDATED";
                }
                return "CORRECTED";
            }

            //_____________________________________________________________________________________________________

            private class VPCStringComparer : IComparer<String>
            {
                /*
                 <summary>Customised Compare Class</summary>
                 <remarks>
                 <para>
                 The Virtual Payment Client need to use an Ordinal comparison to Sort on 
                 the field names to create the SHA256 Signature for validation of the message. 
                 This class provides a Compare method that is used to allow the sorted list 
                 to be ordered using an Ordinal comparison.
                 </para>
                 </remarks>
                 */

                public int Compare(String a, String b)
                {
                    /*
                     <summary>Compare method using Ordinal comparison</summary>
                     <param name="a">The first string in the comparison.</param>
                     <param name="b">The second string in the comparison.</param>
                     <returns>An int containing the result of the comparison.</returns>
                     */

                    // Return if we are comparing the same object or one of the 
                    // objects is null, since we don't need to go any further.
                    if (a == b) return 0;
                    if (a == null) return -1;
                    if (b == null) return 1;

                    // Ensure we have string to compare
                    string sa = a as string;
                    string sb = b as string;

                    // Get the CompareInfo object to use for comparing
                    System.Globalization.CompareInfo myComparer = System.Globalization.CompareInfo.GetCompareInfo("en-US");
                    if (sa != null && sb != null)
                    {
                        // Compare using an Ordinal Comparison.
                        return myComparer.Compare(sa, sb, System.Globalization.CompareOptions.Ordinal);
                    }
                    throw new ArgumentException("a and b should be strings.");
                }
            }

            //______________________________________________________________________________
            // SHA256 Hash Code

            public string CreateSHA256Signature(bool useRequest)
            {
                // Hex Decode the Secure Secret for use in using the HMACSHA256 hasher
                // hex decoding eliminates this source of error as it is independent of the character encoding
                // hex decoding is precise in converting to a byte array and is the preferred form for representing binary values as hex strings. 
                byte[] convertedHash = new byte[_secureSecret.Length / 2];
                for (int i = 0; i < _secureSecret.Length / 2; i++)
                {
                    convertedHash[i] = (byte)Int32.Parse(_secureSecret.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }

                // Build string from collection in preperation to be hashed
                StringBuilder sb = new StringBuilder();
                SortedList<String, String> list = (useRequest ? _requestFields : _responseFields);
                foreach (KeyValuePair<string, string> kvp in list)
                {
                    if (kvp.Key.StartsWith("vpc_") || kvp.Key.StartsWith("user_"))
                        sb.Append(kvp.Key + "=" + kvp.Value + "&");
                }
                // remove trailing & from string
                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);

                // Create secureHash on string
                string hexHash = "";
                using (HMACSHA256 hasher = new HMACSHA256(convertedHash))
                {
                    byte[] hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                    foreach (byte b in hashValue)
                    {
                        hexHash += b.ToString("X2");
                    }
                }
                return hexHash;
            }
        }
        public class Config
        {
            public static string ApiPayUrl = System.Configuration.ConfigurationManager.AppSettings["OnepayDomApiPayUrl"] ?? "https://mtf.onepay.vn/onecomm-pay/vpc.op";
            public static string ApiQueryUrl = System.Configuration.ConfigurationManager.AppSettings["OnepayDomApiQueryUrl"] ?? "https://mtf.onepay.vn/onecomm-pay/Vpcdps.op";
            public static string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["OnepayDomApiReturnUrl"] ?? "/Payment/OnepayDomesticDr";

            public static string SecureSecret = System.Configuration.ConfigurationManager.AppSettings["OnepayDomSecureSecret"] ?? "A3EFDFABA8653DF2342E8DAC29B51AF0";

            public static string AccessCode = System.Configuration.ConfigurationManager.AppSettings["OnepayDomAccessCode"] ?? "D67342C2";

            public static string Merchant = System.Configuration.ConfigurationManager.AppSettings["OnepayDomMerchant"] ?? "ONEPAY";
            public static string User = System.Configuration.ConfigurationManager.AppSettings["OnepayDomMerchant"] ?? "ONEPAY";
            public static string Password = System.Configuration.ConfigurationManager.AppSettings["OnepayDomMerchant"] ?? "ONEPAY";
            public static string QueryDrVersion = System.Configuration.ConfigurationManager.AppSettings["OnepayDomQueryDrVertion"] ?? "1";
            public static string PayVersion = System.Configuration.ConfigurationManager.AppSettings["OnepayDomPayVertion"] ?? "2";

        }
    }
}