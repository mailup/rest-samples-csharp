Imports System.Collections.Generic
Imports System.Configuration
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Net
Imports System.IO
Imports System.Web

Namespace MailUp
    Public Enum ContentType
        Json
        Xml
    End Enum

    Public Class MailUpException
        Inherits Exception
        Private m_statusCode As Integer
        Public Property StatusCode() As Integer
            Get
                Return m_statusCode
            End Get
            Set(value As Integer)
                m_statusCode = value
            End Set
        End Property

        Public Sub New(statusCode As Integer, message As [String])
            MyBase.New(message)
            Me.StatusCode = statusCode
        End Sub
    End Class

    Partial Public Class MailUpClient
        Private m_logonEndpoint As [String] = "https://services.mailup.com/Authorization/OAuth/LogOn"
        Public Property LogonEndpoint() As [String]
            Get
                Return m_logonEndpoint
            End Get
            Set(value As [String])
                m_logonEndpoint = value
            End Set
        End Property

        Private m_authorizationEndpoint As [String] = "https://services.mailup.com/Authorization/Authorization"
        Public Property AuthorizationEndpoint() As [String]
            Get
                Return m_authorizationEndpoint
            End Get
            Set(value As [String])
                m_authorizationEndpoint = value
            End Set
        End Property

        Private m_tokenEndpoint As [String] = "https://services.mailup.com/Authorization/OAuth/Token"
        Public Property TokenEndpoint() As [String]
            Get
                Return m_tokenEndpoint
            End Get
            Set(value As [String])
                m_tokenEndpoint = value
            End Set
        End Property

        Private m_consoleEndpoint As [String] = "https://services.mailup.com/API/v1/Rest/ConsoleService.svc"
        Public Property ConsoleEndpoint() As [String]
            Get
                Return m_consoleEndpoint
            End Get
            Set(value As [String])
                m_consoleEndpoint = value
            End Set
        End Property

        Private m_mailstatisticsEndpoint As [String] = "https://services.mailup.com/API/v1/Rest/MailStatisticsService.svc"
        Public Property MailstatisticsEndpoint() As [String]
            Get
                Return m_mailstatisticsEndpoint
            End Get
            Set(value As [String])
                m_mailstatisticsEndpoint = value
            End Set
        End Property

        Private m_clientId As [String]
        Public Property ClientId() As [String]
            Get
                Return m_clientId
            End Get
            Set(value As [String])
                m_clientId = value
            End Set
        End Property

        Private m_clientSecret As [String]
        Public Property ClientSecret() As [String]
            Get
                Return m_clientSecret
            End Get
            Set(value As [String])
                m_clientSecret = value
            End Set
        End Property

        Private m_callbackUri As [String]
        Public Property CallbackUri() As [String]
            Get
                Return m_callbackUri
            End Get
            Set(value As [String])
                m_callbackUri = value
            End Set
        End Property

        Private m_accessToken As [String]
        Public Property AccessToken() As [String]
            Get
                Return m_accessToken
            End Get
            Set(value As [String])
                m_accessToken = value
            End Set
        End Property

        Private m_refreshToken As [String]
        Public Property RefreshToken() As [String]
            Get
                Return m_refreshToken
            End Get
            Set(value As [String])
                m_refreshToken = value
            End Set
        End Property


        Public Sub New(clientId As [String], clientSecret As [String], callbackUri As [String])
            Me.m_clientId = clientId
            Me.m_clientSecret = clientSecret
            Me.m_callbackUri = callbackUri
            LoadToken()
        End Sub

        Public Function GetLogOnUri() As [String]
            Dim url As [String] = m_logonEndpoint & "?client_id=" & m_clientId & "&client_secret=" & m_clientSecret & "&response_type=code&redirect_uri=" & m_callbackUri
            Return url
        End Function

        Public Sub LogOn()
            Dim url As [String] = GetLogOnUri()
            HttpContext.Current.Response.Redirect(url)
        End Sub

        Public Function RetreiveAccessToken(code As [String]) As [String]
            Dim statusCode As Integer = 0
            Try
                Dim wrLogon As HttpWebRequest = DirectCast(WebRequest.Create(m_tokenEndpoint & "?code=" & code & "&grant_type=authorization_code"), HttpWebRequest)
                wrLogon.AllowAutoRedirect = False
                wrLogon.KeepAlive = True
                Dim retreiveResponse As HttpWebResponse = DirectCast(wrLogon.GetResponse(), HttpWebResponse)
                statusCode = CInt(retreiveResponse.StatusCode)
                Dim objStream As Stream = retreiveResponse.GetResponseStream()
                Dim objReader As New StreamReader(objStream)
                Dim json As [String] = objReader.ReadToEnd()
                retreiveResponse.Close()

                m_accessToken = ExtractJsonValue(json, "access_token")
                m_refreshToken = ExtractJsonValue(json, "refresh_token")

                SaveToken()
            Catch wex As WebException
                Dim wrs As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                Throw New MailUpException(CInt(wrs.StatusCode), wex.Message)
            Catch ex As Exception
                Throw New MailUpException(statusCode, ex.Message)
            End Try
            Return m_accessToken
        End Function

        Public Function RetreiveAccessToken(login As [String], password As [String]) As [String]
            Dim statusCode As Integer = 0
            Try
                Dim cookies As New CookieContainer()
                Dim wrLogon As HttpWebRequest = DirectCast(WebRequest.Create(m_authorizationEndpoint & "?client_id=" & m_clientId & "&client_secret=" & m_clientSecret & "&response_type=code" & "&username=" & login & "&password=" & password), HttpWebRequest)
                wrLogon.AllowAutoRedirect = False
                wrLogon.KeepAlive = True
                wrLogon.CookieContainer = cookies
                Dim authorizationResponse As HttpWebResponse = DirectCast(wrLogon.GetResponse(), HttpWebResponse)
                statusCode = CInt(authorizationResponse.StatusCode)
                Dim objStream As Stream = authorizationResponse.GetResponseStream()
                Dim objReader As New StreamReader(objStream)
                Dim json As [String] = objReader.ReadToEnd()
                authorizationResponse.Close()

                Dim code As [String] = ExtractJsonValue(json, "code")

                RetreiveAccessToken(code)
            Catch wex As WebException
                Dim wrs As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                Throw New MailUpException(CInt(wrs.StatusCode), wex.Message)
            Catch ex As Exception
                Throw New MailUpException(statusCode, ex.Message)
            End Try
            Return m_accessToken
        End Function

        Public Function RefreshAccessToken() As [String]
            Dim statusCode As Integer = 0
            Try
                Dim wrLogon As HttpWebRequest = DirectCast(WebRequest.Create(m_tokenEndpoint), HttpWebRequest)
                wrLogon.AllowAutoRedirect = False
                wrLogon.KeepAlive = True
                wrLogon.Method = "POST"
                wrLogon.ContentType = "application/x-www-form-urlencoded"

                Dim body As [String] = "client_id=" & m_clientId & "&client_secret=" & m_clientSecret & "&refresh_token=" & m_refreshToken & "&grant_type=refresh_token"
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(body)
                wrLogon.ContentLength = byteArray.Length
                Dim dataStream As Stream = wrLogon.GetRequestStream()
                dataStream.Write(byteArray, 0, byteArray.Length)
                dataStream.Close()

                Dim refreshResponse As HttpWebResponse = DirectCast(wrLogon.GetResponse(), HttpWebResponse)
                statusCode = CInt(refreshResponse.StatusCode)
                Dim objStream As Stream = refreshResponse.GetResponseStream()
                Dim objReader As New StreamReader(objStream)
                Dim json As [String] = objReader.ReadToEnd()
                refreshResponse.Close()

                m_accessToken = ExtractJsonValue(json, "access_token")
                m_refreshToken = ExtractJsonValue(json, "refresh_token")

                SaveToken()
            Catch wex As WebException
                Dim wrs As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                Throw New MailUpException(CInt(wrs.StatusCode), wex.Message)
            Catch ex As Exception
                Throw New MailUpException(statusCode, ex.Message)
            End Try
            Return m_accessToken
        End Function

        Public Function CallMethod(url As [String], verb As [String], body As [String], contentTypeParam As ContentType) As [String]
            Return CallMethod(url, verb, body, contentTypeParam, True)
        End Function

        Private Function CallMethod(url As [String], verb As [String], body As [String], contentTypeParam As ContentType, refresh As Boolean) As [String]
            Dim result As [String] = ""
            Dim callResponse As HttpWebResponse = Nothing
            Dim statusCode As Integer = 0
            Try

                Dim wrLogon As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
                wrLogon.AllowAutoRedirect = False
                wrLogon.KeepAlive = True
                wrLogon.Method = verb
                wrLogon.ContentType = GetContentTypeString(contentTypeParam)
                wrLogon.ContentLength = 0
                wrLogon.Accept = GetContentTypeString(contentTypeParam)
                wrLogon.Headers.Add("Authorization", "Bearer " & m_accessToken)

                If body IsNot Nothing AndAlso body <> "" Then
                    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(body)
                    wrLogon.ContentLength = byteArray.Length
                    Dim dataStream As Stream = wrLogon.GetRequestStream()
                    dataStream.Write(byteArray, 0, byteArray.Length)
                    dataStream.Close()
                End If

                callResponse = DirectCast(wrLogon.GetResponse(), HttpWebResponse)
                statusCode = CInt(callResponse.StatusCode)
                Dim objStream As Stream = callResponse.GetResponseStream()
                Dim objReader As New StreamReader(objStream)
                result = objReader.ReadToEnd()
                callResponse.Close()
            Catch wex As WebException
                Try
                    Dim wrs As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                    If CInt(wrs.StatusCode) = 401 AndAlso refresh Then
                        RefreshAccessToken()
                        Return CallMethod(url, verb, body, contentTypeParam, False)
                    Else
                        Throw New MailUpException(CInt(wrs.StatusCode), wex.Message)
                    End If
                Catch ex As Exception
                    Throw New MailUpException(statusCode, ex.Message)
                End Try
            Catch ex As Exception
                Throw New MailUpException(statusCode, ex.Message)
            End Try
            Return result
        End Function

        Private Function ExtractJsonValue(json As [String], name As [String]) As [String]
            Dim delim As [String] = """" & name & """:"""
            Dim start As Integer = json.IndexOf(delim) + delim.Length
            Dim [end] As Integer = json.IndexOf("""", start + 1)
            If [end] > start AndAlso start > -1 AndAlso [end] > -1 Then
                Return json.Substring(start, [end] - start)
            Else
                Return ""
            End If
        End Function

        Private Function GetContentTypeString([cType] As ContentType) As [String]
            If [cType] = ContentType.Json Then
                Return "application/json"
            Else
                Return "application/xml"
            End If
        End Function

        Public Overridable Sub LoadToken()
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies("MailUpCookie")
            If cookie IsNot Nothing Then
                If Not [String].IsNullOrEmpty(cookie.Values("access_token")) Then
                    m_accessToken = cookie.Values("access_token").ToString()
                End If
                If Not [String].IsNullOrEmpty(cookie.Values("refresh_token")) Then
                    m_refreshToken = cookie.Values("refresh_token").ToString()
                End If
            End If
        End Sub

        Public Overridable Sub SaveToken()
            Dim cookie As New HttpCookie("MailUpCookie")

            cookie.Values.Add("access_token", m_accessToken)
            cookie.Values.Add("refresh_token", m_refreshToken)
            cookie.Expires = DateTime.Now.AddDays(30)

            HttpContext.Current.Response.Cookies.Add(cookie)
        End Sub
    End Class
End Namespace
