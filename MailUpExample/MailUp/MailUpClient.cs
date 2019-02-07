using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;

namespace MailUp
{
    public enum ContentType
    {
        Json,
        Xml
    }

    public class MailUpException : Exception
    {
        private int statusCode;
        public int StatusCode
        {
            set { statusCode = value; }
            get { return statusCode; }
        }

        public MailUpException(int statusCode, String message) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }

    public partial class MailUpClient
    {
        private String logonEndpoint = "https://services.mailup.com/Authorization/OAuth/LogOn";
        public String LogonEndpoint
        {
            get { return logonEndpoint; }
            set { logonEndpoint = value; }
        }

        private String authorizationEndpoint = "https://services.mailup.com/Authorization/OAuth/Authorization";
        public String AuthorizationEndpoint
        {
            get { return authorizationEndpoint; }
            set { authorizationEndpoint = value; }
        }

        private String tokenEndpoint = "https://services.mailup.com/Authorization/OAuth/Token";
        public String TokenEndpoint
        {
            get { return tokenEndpoint; }
            set { tokenEndpoint = value; }
        }

        private String consoleEndpoint = "https://services.mailup.com/API/v1.1/Rest/ConsoleService.svc";
        public String ConsoleEndpoint
        {
            get { return consoleEndpoint; }
            set { consoleEndpoint = value; }
        }

        private String mailstatisticsEndpoint = "https://services.mailup.com/API/v1.1/Rest/MailStatisticsService.svc";
        public String MailstatisticsEndpoint
        {
            get { return mailstatisticsEndpoint; }
            set { mailstatisticsEndpoint = value; }
        }

        private String clientId;
        public String ClientId
        {
            set { clientId = value; }
            get { return clientId; }
        }

        private String clientSecret;
        public String ClientSecret
        {
            set { clientSecret = value; }
            get { return clientSecret; }
        }

        private String callbackUri;
        public String CallbackUri
        {
            set { callbackUri = value; }
            get { return callbackUri; }
        }

        private String listId;
        public String ListId
        {
            set { listId = value; }
            get { return listId; }
        }

        private String accessToken;
        public String AccessToken
        {
            set { accessToken = value; }
            get { return accessToken; }
        }

        private String refreshToken;
        public String RefreshToken
        {
            set { refreshToken = value; }
            get { return refreshToken; }
        }

        private String expirationTime;
        public String ExpirationTime
        {
            set { expirationTime = value; }
            get { return expirationTime; }
        }
        
        public MailUpClient(String clientId, String clientSecret, String callbackUri, String listId)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.callbackUri = callbackUri;
            this.listId = listId;
            LoadToken();
        }

        public String GetLogOnUri()
        {
            String url = logonEndpoint + "?client_id=" + clientId + "&client_secret=" + clientSecret + "&response_type=code&redirect_uri=" + callbackUri;
            return url;
        }

        public void LogOn()
        {
            String url = GetLogOnUri();
            HttpContext.Current.Response.Redirect(url);
        }
        public void LogOnWithUsernamePassword(String username,String password)
        {
            this.RetreiveAccessToken(username, password);
        }
        public String RetreiveAccessToken(String code)
        {
            int statusCode = 0;
            try
            {
                HttpWebRequest wrLogon = (HttpWebRequest)WebRequest.Create(tokenEndpoint + "?code=" + code + "&grant_type=authorization_code");
                wrLogon.AllowAutoRedirect = false;
                wrLogon.KeepAlive = true;
                HttpWebResponse retreiveResponse = (HttpWebResponse)wrLogon.GetResponse();
                statusCode = (int)retreiveResponse.StatusCode;
                Stream objStream = retreiveResponse.GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                String json = objReader.ReadToEnd();
                retreiveResponse.Close();

                ExtractTokensFromJson(json);

                SaveToken();
            }
            catch (WebException wex)
            {
                HttpWebResponse wrs = (HttpWebResponse)wex.Response;
                throw new MailUpException((int)wrs.StatusCode, wex.Message);
            }
            catch (Exception ex)
            {
                throw new MailUpException(statusCode, ex.Message);
            }
            return accessToken;
        }

        public String RetreiveAccessToken(String login, String password)
        {
            int statusCode = 0;
            try
            {
                CookieContainer cookies = new CookieContainer();
                
                String body = "client_id=" + clientId + "&client_secret=" + clientSecret + "&grant_type=password&username="
                                + Uri.EscapeDataString(login) + "&password=" + Uri.EscapeDataString(password);
                HttpWebRequest wrLogon = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
                wrLogon.CookieContainer = cookies;
                wrLogon.AllowAutoRedirect = false;
                wrLogon.KeepAlive = true;
                wrLogon.Method = "POST";
                wrLogon.ContentType = "application/x-www-form-urlencoded";

                String auth = String.Format("{0}:{1}",this.clientId,this.clientSecret);
                wrLogon.Headers["Authorization"] = "Basic "+ Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(auth));

                byte[] byteArray = Encoding.UTF8.GetBytes(body);
                wrLogon.ContentLength = byteArray.Length;
                Stream dataStream = wrLogon.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse tokenResponse = (HttpWebResponse)wrLogon.GetResponse();
                statusCode = (int)tokenResponse.StatusCode;
                Stream objStream = tokenResponse.GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                String json = objReader.ReadToEnd();
                tokenResponse.Close();

                ExtractTokensFromJson(json);

                SaveToken();
            }
            catch (WebException wex)
            {
                HttpWebResponse wrs = (HttpWebResponse)wex.Response;
                throw new MailUpException((int)wrs.StatusCode, wex.Message);
            }
            catch (Exception ex)
            {
                throw new MailUpException(statusCode, ex.Message);
            }
            return accessToken;
        }

        public String RefreshAccessToken()
        {
            int statusCode = 0;
            try
            {
                HttpWebRequest wrLogon = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
                wrLogon.AllowAutoRedirect = false;
                wrLogon.KeepAlive = true;
                wrLogon.Method = "POST";
                wrLogon.ContentType = "application/x-www-form-urlencoded";

                String body = "client_id=" + clientId + "&client_secret=" + clientSecret +
                    "&refresh_token=" + refreshToken + "&grant_type=refresh_token";
                byte[] byteArray = Encoding.UTF8.GetBytes(body);
                wrLogon.ContentLength = byteArray.Length;
                Stream dataStream = wrLogon.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                
                HttpWebResponse refreshResponse = (HttpWebResponse)wrLogon.GetResponse();
                statusCode = (int)refreshResponse.StatusCode;
                Stream objStream = refreshResponse.GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                String json = objReader.ReadToEnd();
                refreshResponse.Close();

                ExtractTokensFromJson(json);

                SaveToken();
            }
            catch (WebException wex)
            {
                HttpWebResponse wrs = (HttpWebResponse)wex.Response;
                throw new MailUpException((int)wrs.StatusCode, wex.Message);
            }
            catch (Exception ex)
            {
                throw new MailUpException(statusCode, ex.Message);
            }
            return accessToken;
        }

        public String CallMethod(String url, String verb, String body, ContentType contentType = ContentType.Json)
        {
            return CallMethod(url, verb, body, contentType, true);
        }

        private String CallMethod(String url, String verb, String body, ContentType contentType = ContentType.Json, bool refresh = true)
        {
            String result = "";
            HttpWebResponse callResponse = null;
            int statusCode = 0;
            try
            {
                
                HttpWebRequest wrLogon = (HttpWebRequest)WebRequest.Create(url);
                wrLogon.AllowAutoRedirect = false;
                wrLogon.KeepAlive = true;
                wrLogon.Method = verb;
                wrLogon.ContentType = GetContentTypeString(contentType);
                wrLogon.ContentLength = 0;
                wrLogon.Accept = GetContentTypeString(contentType);
                wrLogon.Headers.Add("Authorization", "Bearer " + accessToken);

                if (body != null && body != "")
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(body);
                    wrLogon.ContentLength = byteArray.Length;
                    Stream dataStream = wrLogon.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                callResponse = (HttpWebResponse)wrLogon.GetResponse();
                statusCode = (int)callResponse.StatusCode;
                Stream objStream = callResponse.GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                result = objReader.ReadToEnd();
                callResponse.Close();
            }
            catch (WebException wex)
            {
                try
                {
                    HttpWebResponse wrs = (HttpWebResponse)wex.Response;
                    if ((int)wrs.StatusCode == 401 && refresh)
                    {
                        RefreshAccessToken();
                        return CallMethod(url, verb, body, contentType, false);
                    }
                    else throw new MailUpException((int)wrs.StatusCode, wex.Message);
                }
                catch (Exception ex)
                {
                    throw new MailUpException(statusCode, ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new MailUpException(statusCode, ex.Message);
            }
            return result;
        }

        private String ExtractJsonValue(String json, String name)
        {
            String delim = "\"" + name + "\":";
            int start = json.IndexOf(delim) + delim.Length;
            int end1 = json.IndexOf("\"", start + 1);
            if (end1 < 0) end1 = 100000;
            int end2 = json.IndexOf(",", start + 1);
            if (end2 < 0) end2 = 100000;
            int end3 = json.IndexOf("}", start + 1);

            int end = Math.Min(Math.Min(end1, end2), end3);

            if (end > start && start > -1 && end > -1)
            {
                String result = json.Substring(start, end - start);
                if (result.StartsWith("\""))
                {
                    return json.Substring(start + 1, end - start - 1);
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return "";
            }
        }

        private String GetContentTypeString(ContentType cType)
        {
            if (cType == ContentType.Json) return "application/json";
            else return "application/xml";
        }

        public virtual void LoadToken()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["MailUpCookie"];
            if (cookie != null)
            {
                if (!String.IsNullOrEmpty(cookie.Values["access_token"]))
                {
                    accessToken = cookie.Values["access_token"].ToString();
                }
                if (!String.IsNullOrEmpty(cookie.Values["refresh_token"]))
                {
                    refreshToken = cookie.Values["refresh_token"].ToString();
                }
                if (!String.IsNullOrEmpty(cookie.Values["expires_in"]))
                {
                    expirationTime = cookie.Values["expires_in"].ToString();
                }
            }
        }

        public virtual void SaveToken()
        {
            HttpCookie cookie = new HttpCookie("MailUpCookie");
            
            cookie.Values.Add("access_token", accessToken);
            cookie.Values.Add("refresh_token", refreshToken);
            cookie.Values.Add("expires_in", expirationTime);
            cookie.Expires = DateTime.Now.AddDays(30);

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public Int64 ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }

        public void ExtractTokensFromJson(String json)
        {
            accessToken = ExtractJsonValue(json, "access_token");
            refreshToken = ExtractJsonValue(json, "refresh_token");
            int exp = Convert.ToInt32(ExtractJsonValue(json, "expires_in"));
            expirationTime = ToUnixTime(DateTime.Now.AddSeconds(exp)).ToString();
        }
    }
}
