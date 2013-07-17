Imports System.Collections.Generic
Imports System.Configuration
Imports System.Linq
Imports System.Web
Imports System.Web.Security
Imports System.Web.SessionState


Public Class Global_asax
    Inherits System.Web.HttpApplication

    Private Sub Application_Start(sender As Object, e As EventArgs)


    End Sub

    Private Sub Application_End(sender As Object, e As EventArgs)


    End Sub

    Private Sub Application_Error(sender As Object, e As EventArgs)


    End Sub

    Private Sub Session_Start(sender As Object, e As EventArgs)
        Dim APIClient As New MailUp.MailUpClient(ConfigurationManager.AppSettings("MailUpClientId"), ConfigurationManager.AppSettings("MailUpClientSecret"), ConfigurationManager.AppSettings("MailUpCallbackUri"))

        If ConfigurationManager.AppSettings("MailUpConsoleEndpoint") IsNot Nothing Then
            APIClient.ConsoleEndpoint = ConfigurationManager.AppSettings("MailUpConsoleEndpoint")
        End If

        If ConfigurationManager.AppSettings("MailUpStatisticsEndpoint") IsNot Nothing Then
            APIClient.MailstatisticsEndpoint = ConfigurationManager.AppSettings("MailUpStatisticsEndpoint")
        End If

        If ConfigurationManager.AppSettings("MailUpLogon") IsNot Nothing Then
            APIClient.LogonEndpoint = ConfigurationManager.AppSettings("MailUpLogon")
        End If

        If ConfigurationManager.AppSettings("MailUpAuthorization") IsNot Nothing Then
            APIClient.AuthorizationEndpoint = ConfigurationManager.AppSettings("MailUpAuthorization")
        End If

        If ConfigurationManager.AppSettings("MailUpToken") IsNot Nothing Then
            APIClient.TokenEndpoint = ConfigurationManager.AppSettings("MailUpToken")
        End If

        Session.Add("MailUpClient", APIClient)

    End Sub

    Private Sub Session_End(sender As Object, e As EventArgs)


    End Sub

End Class

