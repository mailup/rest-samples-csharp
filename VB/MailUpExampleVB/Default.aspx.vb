Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports MailUpExampleVB.Entity

Partial Public Class _Default
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        ' This "code" parameter is used when called back from MailUp logon. 
        ' Need to complete logging on by retreiving the access token.
        Dim mailUp As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)
        If Request.Params("code") IsNot Nothing Then
            If mailUp IsNot Nothing AndAlso mailUp.AccessToken Is Nothing Then
                Dim token As [String] = mailUp.RetreiveAccessToken(Request.Params("code"))
            End If
        End If

        If mailUp IsNot Nothing AndAlso mailUp.AccessToken IsNot Nothing Then
            pAuthorization.InnerText = "Authorized. Token: " + mailUp.AccessToken
        Else
            pAuthorization.InnerText = "Unauthorized"
        End If

        If Not IsPostBack Then
            lstVerb.DataSource = New [String]() {"GET", "POST", "PUT", "DELETE"}
            lstVerb.DataBind()

            lstContentType.DataSource = New [String]() {"JSON", "XML"}
            lstContentType.DataBind()

            lstEndpoint.DataSource = New [String]() {"Console", "MailStatistics"}
            lstEndpoint.DataBind()
        End If
    End Sub

    ' Sign in button - redirects to MailUp Logon page.
    Protected Sub LogOn_ServerClick(sender As Object, e As EventArgs)
        Dim mailUp As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)
        If mailUp IsNot Nothing Then
            mailUp.LogOn()
        End If
    End Sub

    ' Call method button - calls a single API method.
    Protected Sub CallMethod_ServerClick(sender As Object, e As EventArgs)
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)
        Try
            If mailUpInstance IsNot Nothing Then
                Dim resourceURL As [String] = "" & (If(lstEndpoint.SelectedValue = "Console", mailUpInstance.ConsoleEndpoint + txtPath.Text, mailUpInstance.MailstatisticsEndpoint + txtPath.Text))


                Dim strResult As [String] = mailUpInstance.CallMethod(resourceURL, lstVerb.SelectedValue, txtBody.Text, If(lstContentType.SelectedValue = "JSON", MailUp.ContentType.Json, MailUp.ContentType.Xml))

                pResultString.InnerText = txtPath.Text & " returned: " & strResult
            End If
        Catch ex As MailUp.MailUpException
            pResultString.InnerText = ("Exception: " & ex.Message & " with HTTP Status code: ") & ex.StatusCode.ToString()
        End Try
    End Sub

    ' EXAMPLE 1 - IMPORT RECIPIENTS INTO NEW GROUP
    Protected Sub RunExample1_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)
        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""
                Dim objResult As [Object]
                Dim items As New Dictionary(Of [String], [Object])()

                ' Given a default list id (use idList = 1), request for user visible groups
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Groups"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                items = DirectCast(objResult, Dictionary(Of [String], [Object]))
                Dim groups As [Object]() = DirectCast(items("Items"), [Object]())
                Dim groupId As Integer = -1
                For Each group As Dictionary(Of [String], [Object]) In groups
                    Dim name As [Object] = group("Name")
                    If "test import".Equals(name) Then
                        groupId = Integer.Parse(group("idGroup").ToString())
                    End If
                Next

                status += "Given a default list id (use idList = 1), request for user visible groups - OK<br/>"

                ' If the list does not contain a group named “test import”, create it
                If groupId = -1 Then
                    groupId = 100
                    resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Group"
                    Dim groupRequest As [String] = "{""Deletable"":true,""Name"":""test import"",""Notes"":""test import""}"
                    strResult = mailUpInstance.CallMethod(resourceURL, "POST", groupRequest, MailUp.ContentType.Json)
                    objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                    items = DirectCast(objResult, Dictionary(Of [String], [Object]))
                    groups = DirectCast(items("Items"), [Object]())
                    For Each group As Dictionary(Of [String], [Object]) In groups
                        Dim name As [Object] = group("Name")
                        If "test import".Equals(name) Then
                            groupId = Integer.Parse(group("idGroup").ToString())
                        End If
                    Next
                End If
                Session("groupId") = groupId

                status += "If the list does not contain a group named ""test import"", create it - OK<br/>"

                ' Request for dynamic fields to map recipient name and surname
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/Recipient/DynamicFields"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)

                status += "Request for dynamic fields to map recipient name and surname - OK<br/>"

                ' Import recipients to group
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/Group/" & groupId.ToString() & "/Recipients"
                Dim recipientRequest As [String] = "[{""Email"":""test@test.test"",""Fields"":[{""Description"":""String description"",""Id"":1,""Value"":""String value""}]," & """MobileNumber"":"""",""MobilePrefix"":"""",""Name"":""John Smith""}]"
                strResult = mailUpInstance.CallMethod(resourceURL, "POST", recipientRequest, MailUp.ContentType.Json)
                Dim importId As Integer = Integer.Parse(strResult)

                status += "Import recipients to group - OK<br/>"

                ' Check the import result
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/Import/" & importId.ToString()
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)

                status += "Check the import result - OK<br/>"

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " + ex.Message & " with HTTP Status code: ") + ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

    ' EXAMPLE 2 - UNSUBSCRIBE A RECIPIENT FROM A GROUP
    Protected Sub RunExample2_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)

        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""
                Dim objResult As [Object]
                Dim items As New Dictionary(Of [String], [Object])()

                ' Request for recipient in a group
                Dim groupId As Integer = -1
                If Session("groupId") IsNot Nothing Then
                    groupId = CInt(Session("groupId"))
                End If
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/Group/" & groupId & "/Recipients"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)

                status += "Request for recipient in a group - OK<br/>"

                items = DirectCast(objResult, Dictionary(Of [String], [Object]))
                Dim recipients As [Object]() = DirectCast(items("Items"), [Object]())
                If recipients.Length > 0 Then
                    Dim recipient As Dictionary(Of [String], [Object]) = DirectCast(recipients(0), Dictionary(Of [String], [Object]))
                    Dim recipientId As Integer = Integer.Parse(recipient("idRecipient").ToString())

                    ' Pick up a recipient and unsubscribe it
                    resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/Group/" & groupId.ToString() & "/Unsubscribe/" & recipientId.ToString()
                    strResult = mailUpInstance.CallMethod(resourceURL, "DELETE", Nothing, MailUp.ContentType.Json)

                    status += "Pick up a recipient and unsubscribe it - OK<br/>"
                End If

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " + ex.Message & " with HTTP Status code: ") & ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

    ' EXAMPLE 3 - UPDATE A RECIPIENT DETAIL
    Protected Sub RunExample3_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)

        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""
                Dim objResult As [Object]
                Dim items As New Dictionary(Of [String], [Object])()

                ' Request for existing subscribed recipients
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Recipients/Subscribed"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)

                status += "Request for existing subscribed recipients - OK<br/>"

                items = DirectCast(objResult, Dictionary(Of [String], [Object]))
                Dim recipients2 As [Object]() = DirectCast(items("Items"), [Object]())
                If recipients2.Length > 0 Then
                    Dim recipient As Dictionary(Of [String], [Object]) = DirectCast(recipients2(0), Dictionary(Of [String], [Object]))
                    Dim fields As [Object]() = DirectCast(recipient("Fields"), [Object]())
                    If fields.Length = 0 Then
                        Dim arr As [Object]() = New [Object](0) {}
                        Dim dict As New Dictionary(Of [String], [Object])()
                        dict("Id") = 1
                        dict("Value") = "Updated value"
                        dict("Description") = ""
                        arr(0) = dict
                        recipient("Fields") = arr
                    Else
                        Dim dict As Dictionary(Of [String], [Object]) = DirectCast(fields(0), Dictionary(Of [String], [Object]))
                        dict("Id") = 1
                        dict("Value") = "Updated value"
                        dict("Description") = ""
                    End If

                    ' Update the modified recipient
                    Dim recipientRequest As [String] = New JavaScriptSerializer().Serialize(recipient)
                    resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/Recipient/Detail"
                    strResult = mailUpInstance.CallMethod(resourceURL, "PUT", recipientRequest, MailUp.ContentType.Json)

                    status += "Update the modified recipient - OK<br/>"
                End If

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " & ex.Message & " with HTTP Status code: ") + ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

    ' EXAMPLE 4 - CREATE A MESSAGE FROM TEMPLATE
    Protected Sub RunExample4_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)

        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""
                Dim objResult As [Object]
                Dim items As New Dictionary(Of [String], [Object])()

                ' Get the available template list
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Templates"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                Dim templates As [Object]() = DirectCast(objResult, [Object]())
                Dim template As Dictionary(Of [String], [Object]) = DirectCast(templates(0), Dictionary(Of [String], [Object]))
                Dim templateId As Integer = Integer.Parse(template("Id").ToString())

                status += "Get the available template list - OK<br/>"

                ' Create the new message
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Email/Template/" & templateId.ToString()

                strResult = mailUpInstance.CallMethod(resourceURL, "POST", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                items = DirectCast(objResult, Dictionary(Of [String], [Object]))
                Dim emails As [Object]() = DirectCast(items("Items"), [Object]())
                Dim email As Dictionary(Of [String], [Object]) = DirectCast(emails(0), Dictionary(Of [String], [Object]))
                Dim emailId As Integer = Integer.Parse(email("idMessage").ToString())

                status += "Create the new message - OK<br/>"

                ' Request for messages list
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Emails"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)

                status += "Request for messages list - OK<br/>"

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " + ex.Message & " with HTTP Status code: ") & ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

    ' EXAMPLE 5 - CREATE A MESSAGE WITH IMAGES AND ATTACHMENTS
    Protected Sub RunExample5_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)

        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""
                Dim objResult As [Object]
                Dim items As New Dictionary(Of [String], [Object])()

                ' Upload an image
                ' Image bytes can be obtained from file, database or any other source
                Dim wc As New WebClient()
                Dim imageBytes As Byte() = wc.DownloadData("http://images.apple.com/home/images/ios_title_small.png")
                Dim image As [String] = System.Convert.ToBase64String(imageBytes)
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Images"
                Dim imageRequest As [String] = "{""Base64Data"":""" & image & """,""Name"":""Avatar""}"
                strResult = mailUpInstance.CallMethod(resourceURL, "POST", imageRequest, MailUp.ContentType.Json)

                status += "Upload an image - OK<br/>"

                ' Get the images available
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/Images"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                Dim imgSrc As [String] = ""
                Dim srcs As [Object]() = DirectCast(objResult, [Object]())
                If srcs.Length > 0 Then
                    imgSrc = srcs(0).ToString()
                End If

                status += "Get the images available - OK<br/>"

                ' Create and save "hello" message
                Dim message As [String] = "<html><body><p>Hello</p><img src=\""" & imgSrc & "\""/></body></html>"
                message = "<html><body><p>Hello</p><img src=""" & imgSrc & """ /></body></html>"
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Email"

                Dim dto As New EmailMessageItemDTO()
                dto.Subject = "Test Message c#"
                dto.idList = 1
                dto.Content = message
                dto.Embed = True
                dto.IsConfirmation = True
                dto.Fields = New List(Of EmailDynamicFieldDTO)()
                dto.Notes = "Some notes"
                dto.Tags = New List(Of EmailTagDTO)()
                dto.TrackingInfo = New EmailTrackingInfoDTO() With { _
                 .CustomParams = "", _
                 .Enabled = True, _
                 .Protocols = New List(Of [String])() From { _
                  "http" _
                 } _
                }

                Dim ser As New JavaScriptSerializer()

                Dim emailRequest As [String] = ser.Serialize(dto)
                strResult = mailUpInstance.CallMethod(resourceURL, "POST", emailRequest, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                items = DirectCast(objResult, Dictionary(Of [String], [Object]))
                Dim emails As [Object]() = DirectCast(items("Items"), [Object]())
                Dim email As Dictionary(Of [String], [Object]) = DirectCast(emails(0), Dictionary(Of [String], [Object]))
                Dim emailId As Integer = Integer.Parse(email("idMessage").ToString())
                Session("emailId") = emailId

                status += "Create and save ""hello"" message - OK<br/>"

                ' Add an attachment
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Email/" & emailId.ToString() & "/Attachment/1"
                Dim attachment As [String] = "QmFzZSA2NCBTdHJlYW0="
                ' Base64 String
                Dim attachmentRequest As [String] = "{""Base64Data"":""" & attachment & """,""Name"":""TestFile.txt"",""Slot"":1,""idList"":1,""idMessage"":" & emailId.ToString() & "}"
                strResult = mailUpInstance.CallMethod(resourceURL, "POST", attachmentRequest, MailUp.ContentType.Json)

                status += "Add an attachment - OK<br/>"

                ' Retreive message details
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Email/" & emailId.ToString()
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)

                status += "Retreive message details - OK<br/>"

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " & ex.Message & " with HTTP Status code: ") & ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

    ' EXAMPLE 6 - TAG A MESSAGE
    Protected Sub RunExample6_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)

        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""
                Dim objResult As [Object]
                Dim items As New Dictionary(Of [String], [Object])()

                ' Create a new tag
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Tag"
                strResult = mailUpInstance.CallMethod(resourceURL, "POST", """test tag""", MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                Dim tags As [Object]() = DirectCast(objResult, [Object]())
                Dim tag As Dictionary(Of [String], [Object]) = DirectCast(tags(0), Dictionary(Of [String], [Object]))
                Dim tagId As Integer = Integer.Parse(tag("Id").ToString())

                status += "Create a new tag - OK<br/>"

                ' Pick up a message and retrieve detailed informations
                Dim emailId As Integer = -1
                If Session("emailId") IsNot Nothing Then
                    emailId = CInt(Session("emailId"))
                End If
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Email/" & emailId.ToString()
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)

                status += "Pick up a message and retrieve detailed informations - OK<br/>"

                ' Add the tag to the message details and save
                Dim objEmail As Dictionary(Of [String], [Object]) = DirectCast(objResult, Dictionary(Of [String], [Object]))
                tags = DirectCast(objEmail("Tags"), [Object]())
                Dim al As New List(Of [Object])(tags)
                Dim tagItem As New Dictionary(Of [String], [Object])()
                tagItem("Id") = tagId
                tagItem("Enabled") = True
                tagItem("Name") = "test tag"
                al.Add(tagItem)
                objEmail("Tags") = al.ToArray()

                Dim emailUpdateRequest As [String] = New JavaScriptSerializer().Serialize(objEmail)
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Email/" & emailId.ToString()
                strResult = mailUpInstance.CallMethod(resourceURL, "PUT", emailUpdateRequest, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)

                status += "Add the tag to the message details and save - OK<br/>"

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " & ex.Message & " with HTTP Status code: ") & ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

    ' EXAMPLE 7 - SEND A MESSAGE
    Protected Sub RunExample7_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)

        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""
                Dim objResult As [Object]
                Dim items As New Dictionary(Of [String], [Object])()

                ' Get the list of the existing messages
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Emails"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)
                objResult = New JavaScriptSerializer().DeserializeObject(strResult)
                items = DirectCast(objResult, Dictionary(Of [String], [Object]))
                Dim emails As [Object]() = DirectCast(items("Items"), [Object]())
                Dim email As Dictionary(Of [String], [Object]) = DirectCast(emails(0), Dictionary(Of [String], [Object]))
                Dim emailId As Integer = Integer.Parse(email("idMessage").ToString())
                Session("emailId") = emailId

                status += "Get the list of the existing messages - OK<br/>"

                ' Send email to all recipients in the list
                resourceURL = "" & mailUpInstance.ConsoleEndpoint & "/Console/List/1/Email/" & emailId.ToString() & "/Send"
                strResult = mailUpInstance.CallMethod(resourceURL, "POST", Nothing, MailUp.ContentType.Json)

                status += "Send email to all recipients in the list - OK<br/>"

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " & ex.Message & " with HTTP Status code: ") & ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

    ' EXAMPLE 8 - DISPLAY STATISTICS FOR A MESSAGE SENT AT EXAMPLE 7
    Protected Sub RunExample8_ServerClick(sender As Object, e As EventArgs)
        Dim status As [String] = ""
        Dim mailUpInstance As MailUp.MailUpClient = DirectCast(Session("MailUpClient"), MailUp.MailUpClient)

        Try
            If mailUpInstance IsNot Nothing Then
                ' List ID = 1 is used in all example calls
                Dim resourceURL As [String] = ""
                Dim strResult As [String] = ""

                ' Request (to MailStatisticsService.svc) for paged message views list for the previously sent message
                Dim hours As Integer = 4
                Dim emailId As Integer = -1
                If Session("emailId") IsNot Nothing Then
                    emailId = CInt(Session("emailId"))
                End If
                resourceURL = "" & mailUpInstance.MailstatisticsEndpoint & "/Message/" & emailId.ToString() & "/Views/List/Last/" & hours.ToString() & "?pageSize=5&pageNum=0"
                strResult = mailUpInstance.CallMethod(resourceURL, "GET", Nothing, MailUp.ContentType.Json)

                status += "Request (to MailStatisticsService.svc) for paged message views list for the previously sent message - OK<br/>"

                status += "Example methods completed successfully<br/>"
            End If
        Catch ex As MailUp.MailUpException
            status += ("Exception: " & ex.Message & " with HTTP Status code: ") & ex.StatusCode.ToString() & "<br/>"
        End Try

        pExampleResultString.InnerHtml = status
    End Sub

End Class
