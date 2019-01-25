﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using MailUpExample.Entity;

namespace MailUpExample
{
    public partial class _Default : System.Web.UI.Page
    {

        String resultBlock = "<div class=\"spoiler-wrap disabled\">"
                           + "  <div class=\"spoiler-head\">{0}</div>"
                           + "  <div class=\"spoiler-body\">\n"
                           + "      <div class=\"form-group row\">"
                           + "          <div class=\"col-xs-2\">"
                           + "              <label>Verb</label>"
                           + "              <span class=\"form-control example-body\">{1}</span>"
                           + "              </div>\n"
                           + "          <div class=\"col-xs-2\">"
                           + "              <label>Content-Type</label>"
                           + "              <span class=\"form-control example-body\">{2}</span>"
                           + "              </div>\n"
                           + "          <div class=\"col-xs-2\">"
                           + "              <label>Endpoint</label>"
                           + "              <span class=\"form-control example-body\">{3}</span>"
                           + "          </div>\n"
                           + "          <div class=\"col-xs-6\">"
                           + "              <label>Path</label>"
                           + "              <span class=\"form-control example-body\">{4}</span>"
                           + "          </div>\n"
                           + "      </div>\n"
                           + "      <div class=\"form-group\">"
                           + "          <label>Body</label>"
                           + "          <div class=\"form-control example-body\">{5}</div>"
                           + "      </div>"
                           + "      <div class=\"well\">"
                           + "          <div class=\"form-group example-body\">"
                           + "              <label>Response</label>"
                           + "              <div>{6}</div>"
                           + "          </div>"
                           + "      </div>"
                           + "  </div>"
                           + "</div>";

        String successfullyAnswer = "<div class=\"successfullyAnswer\"><strong>{0}</strong></div>";
        String errorAnswer = "<div class=\"errorAnswer\"><strong>{0}</strong></div>";
        
        enum httpMethods
        { 
            GET,
            POST,
            PUT,
            DELETE
        }

        enum contentType
        { 
            JSON,
            XML
        }

        enum endPotint 
        {
            Console, 
            MailStatistics
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pExampleResultBlock1String.InnerHtml = "";
            pExampleResultBlock2String.InnerHtml = "";
            pExampleResultBlock3String.InnerHtml = "";
            pExampleResultBlock4String.InnerHtml = "";
            pExampleResultBlock5String.InnerHtml = "";
            pExampleResultBlock6String.InnerHtml = "";
            pExampleResultBlock7String.InnerHtml = "";
            pExampleResultBlock8String.InnerHtml = "";
            
            // This "code" parameter is used when called back from MailUp logon. 
            // Need to complete logging on by retreiving the access token.
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            if (Request.Params["code"] != null)
            {
                // if (mailUp != null && mailUp.AccessToken == null)
                if (mailUp != null)
                {
                    String token = mailUp.RetreiveAccessToken(Request.Params["code"]);
                }
            }



            if (mailUp != null && mailUp.AccessToken != null)
            {
                pAuthorizationStatus.InnerText = "Authorized";
                pAuthorizationToken.InnerText = "Token: " + mailUp.AccessToken;
                pExpirationTime.InnerText = mailUp.ExpirationTime;
            }
            else
            {
                pAuthorizationStatus.InnerText = "Unauthorized";
                pAuthorizationToken.InnerText = "";
            }

            if (!IsPostBack)
            {
                lstVerb.DataSource = new String[] { "GET", "POST", "PUT", "DELETE" };
                lstVerb.DataBind();

                lstContentType.DataSource = new String[] { "JSON", "XML" };
                lstContentType.DataBind();

                lstEndpoint.DataSource = new String[] { "Console", "MailStatistics" };
                lstEndpoint.DataBind();
            }
        }

        // Sign in button - redirects to MailUp Logon page.
        protected void LogOn_ServerClick(object sender, EventArgs e)
        {
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            if (mailUp != null) mailUp.LogOn();
        }
        // Sign in button - get tokens with password flow.
        protected void LogOnWithUsernamePassword_ServerClick(object sender, EventArgs e)
        {
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            if (mailUp != null) mailUp.LogOnWithUsernamePassword(txtUsr.Text, txtPwd.Text);

            if (mailUp != null && mailUp.AccessToken != null)
            {
                pAuthorizationStatus.InnerText = "Authorized";
                pAuthorizationToken.InnerText = "Token: " + mailUp.AccessToken;
                pExpirationTime.InnerText = mailUp.ExpirationTime;
            }
            else
            {
                pAuthorizationStatus.InnerText = "Unauthorized";
                pAuthorizationToken.InnerText = "";
            }
        }


        protected void RefreshToken_ServerClick(object sender, EventArgs e)
        {
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            
            if (mailUp != null && mailUp.RefreshToken != null)
            {
                mailUp.RefreshAccessToken();
                pAuthorizationToken.InnerText = "Token: " + mailUp.AccessToken;
                pExpirationTime.InnerText = mailUp.ExpirationTime;
            }
            else
            {
                pAuthorizationStatus.InnerText = "Unauthorized";
                pAuthorizationToken.InnerText = "";
            }
        }

        // Call method button - calls a single API method.
        protected void CallMethod_ServerClick(object sender, EventArgs e)
        {
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            try
            {
                if (mailUp != null)
                {
                    String resourceURL = "" + (lstEndpoint.SelectedValue == endPotint.Console.ToString() ? mailUp.ConsoleEndpoint + txtPath.Text : mailUp.MailstatisticsEndpoint + txtPath.Text);


                    String strResult = mailUp.CallMethod(resourceURL,
                                                         lstVerb.SelectedValue,
                                                         txtBody.Text,
                                                         lstContentType.SelectedValue == contentType.JSON.ToString() ? MailUp.ContentType.Json : MailUp.ContentType.Xml);

                    pResultString.InnerText = txtPath.Text + " returned: " + strResult;
                }
            }
            catch (MailUp.MailUpException ex)
            {
                pResultString.InnerText = "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode;
            }
        }

        // EXAMPLE 1 - IMPORT RECIPIENTS INTO NEW GROUP
        protected void RunExample1_ServerClick(object sender, EventArgs e)
        {
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Given a default list id (use idList = mailUp.ListId), request for user visible groups
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Groups";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    items = (Dictionary<String, Object>)objResult;
                    Object[] groups = (Object[])items["Items"];
                    int groupId = -1;
                    foreach (Dictionary<String, Object> group in groups)
                    {
                        Object name = group["Name"];
                        if ("test import".Equals(name)) groupId = int.Parse(group["idGroup"].ToString());
                    }

                    // status += String.Format("<p>Given a default list id (use idList = 1), request for user visible groups<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Given a default list id (use idList = " + mailUp.ListId + "), request for user visible groups";
                    pExampleResultBlock1String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    
                    // If the list does not contain a group named “test import”, create it
                    String request = "";

                    if (groupId == -1)
                    {
                        groupId = 100;
                        apiUrl = "/Console/List/" + mailUp.ListId + "/Group";
                        resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                        String groupRequest = "{\"Deletable\":true,\"Name\":\"test import\",\"Notes\":\"test import\"}";
                        request = groupRequest;
                        strResult = mailUp.CallMethod(resourceURL,
                                                             httpMethods.POST.ToString(),
                                                             groupRequest,
                                                             MailUp.ContentType.Json);
                        objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                        items = (Dictionary<String, Object>)objResult;
                        groups = (Object[])items["Items"];
                        foreach (Dictionary<String, Object> group in groups)
                        {
                            Object name = group["Name"];
                            if ("test import".Equals(name))
                                groupId = int.Parse(group["idGroup"].ToString());
                        }
                    }
                    Session["groupId"] = groupId;

                    // status += String.Format("<p>If the list does not contain a group named “test import”, create it<br/>{0} {1} - OK</p>", httpMethods.POST.ToString(), resourceURL);

                    responseMessage = "If the list does not contain a group named “test import”, create it";
                    pExampleResultBlock1String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.POST.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, request, strResult);
                    
                    // Request for dynamic fields to map recipient name and surname
                    apiUrl = "/Console/Recipient/DynamicFields";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);

                    // status += String.Format("<p>Request for dynamic fields to map recipient name and surname<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Request for dynamic fields to map recipient name and surname";
                    pExampleResultBlock1String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    
                    // Import recipients to group
                    apiUrl = "/Console/Group/" + groupId + "/Recipients";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    String recipientRequest = "[{\"Email\":\"test@test.test\",\"Fields\":[{\"Description\":\"String description\",\"Id\":1,\"Value\":\"String value\"}]," +
                        "\"MobileNumber\":\"\",\"MobilePrefix\":\"\",\"Name\":\"John Smith\"}]";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.POST.ToString(),
                                                         recipientRequest,
                                                         MailUp.ContentType.Json);
                    int importId = int.Parse(strResult);

                    // status += String.Format("<p>Import recipients to group.<br />{0} {1} - OK<p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Import recipients to group";
                    pExampleResultBlock1String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.POST.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, recipientRequest, strResult);

                    // Check the import result
                    apiUrl = "/Console/Import/" + importId;
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);

                    // status += String.Format("<p>Check the import result.<br />{0} {1} - OK<p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Check the import result";
                    pExampleResultBlock1String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock1String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
                pExampleResultBlock1String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }

        }

        // EXAMPLE 2 - UNSUBSCRIBE A RECIPIENT FROM A GROUP
        protected void RunExample2_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            pExampleResultBlock2String.InnerHtml = "";

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Request for recipient in a group
                    int groupId = -1;
                    if (Session["groupId"] != null) groupId = (int)Session["groupId"];
                    apiUrl = "/Console/Group/" + groupId + "/Recipients";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    // status += String.Format("<p>Request for recipient in a group<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Request for recipient in a group";
                    pExampleResultBlock2String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                    items = (Dictionary<String, Object>)objResult;
                    Object[] recipients = (Object[])items["Items"];
                    if (recipients.Length > 0)
                    {
                        Dictionary<String, Object> recipient = (Dictionary<String, Object>)recipients[0];
                        int recipientId = int.Parse(recipient["idRecipient"].ToString());

                        // Pick up a recipient and unsubscribe it
                        apiUrl = "/Console/Group/" + groupId + "/Unsubscribe/" + recipientId;
                        resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                        strResult = mailUp.CallMethod(resourceURL,
                                                             httpMethods.DELETE.ToString(),
                                                             null,
                                                             MailUp.ContentType.Json);

                        // status += String.Format("<p>Pick up a recipient and unsubscribe it<br/>{0} {1} - OK</p>", httpMethods.DELETE.ToString(), resourceURL);

                        responseMessage = "Pick up a recipient and unsubscribe it";
                        pExampleResultBlock2String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.DELETE.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    }

                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock2String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>"
                pExampleResultBlock2String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }

        }

        // EXAMPLE 3 - UPDATE A RECIPIENT DETAIL
        protected void RunExample3_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            pExampleResultBlock3String.InnerHtml = ""; 

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Request for existing subscribed recipients
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Recipients/Subscribed";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    // status += String.Format("<p>Request for existing subscribed recipients<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Request for existing subscribed recipients";
                    pExampleResultBlock3String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                    items = (Dictionary<String, Object>)objResult;
                    Object[] recipients2 = (Object[])items["Items"];
                    if (recipients2.Length > 0)
                    {
                        Dictionary<String, Object> recipient = (Dictionary<String, Object>)recipients2[0];
                        Object[] fields = (Object[])recipient["Fields"];
                        if (fields.Length == 0)
                        {
                            Object[] arr = new Object[1];
                            Dictionary<String, Object> dict = new Dictionary<String, Object>();
                            dict["Id"] = 1;
                            dict["Value"] = "Updated value";
                            dict["Description"] = "";
                            arr[0] = dict;
                            recipient["Fields"] = arr;
                        }
                        else
                        {
                            Dictionary<String, Object> dict = (Dictionary<String, Object>)fields[0];
                            dict["Id"] = 1;
                            dict["Value"] = "Updated value";
                            dict["Description"] = "";
                        }

                        // status += "<p>Modify a recipient from the list - OK</p>";

                        responseMessage = "Modify a recipient from the list";
                        pExampleResultBlock3String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                        // Update the modified recipient
                        String recipientRequest = new JavaScriptSerializer().Serialize(recipient);
                        apiUrl = "/Console/Recipient/Detail";
                        resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                        strResult = mailUp.CallMethod(resourceURL,
                                                             httpMethods.PUT.ToString(),
                                                             recipientRequest,
                                                             MailUp.ContentType.Json);

                        // status += String.Format("<p>Update the modified recipient<br/>{0} {1} - OK</p>", httpMethods.PUT.ToString(), resourceURL);

                        responseMessage = "Update the modified recipient";
                        pExampleResultBlock3String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.PUT.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                    }

                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock3String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
                pExampleResultBlock3String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }

        }

        // EXAMPLE 4 - CREATE A MESSAGE FROM TEMPLATE
        protected void RunExample4_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Get the available template list
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Templates";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    Dictionary<String, Object> template = (Dictionary<String, Object>)objResult;
                    Dictionary<String, Object> dictionaryItem;
                    int templateId = 0;
                    var arrItems = (Object[])template["Items"];
                    if (arrItems.Length > 0)
                    {
                        dictionaryItem = (Dictionary<String, Object>)arrItems[0];
                        templateId = (int)dictionaryItem["Id"];
                        
                        // status += String.Format("<p>Get the available template list<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                        responseMessage = "Get the available template list";
                        pExampleResultBlock4String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                    }
                    else
                    {
                        // status += String.Format("<p>Could not find any template to create a new message from<br/>{0} {1} - FAIL</p>", httpMethods.GET.ToString(), resourceURL);

                        responseMessage = "Could not find any template to create a new message from";
                        pExampleResultBlock4String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    }
                    // Create the new message
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Email/Template/" + templateId;
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.POST.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    items = (Dictionary<String, Object>)objResult;
                    int emailId = int.Parse(items["idMessage"].ToString());

                    // status += String.Format("<p>Create the new message<br/>{0} {1} - OK</p>", httpMethods.POST.ToString(), resourceURL);

                    responseMessage = "Create the new message";
                    pExampleResultBlock4String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.POST.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    
                    // Request for messages list
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/" + mailUp.ListId + "/Emails";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    // status += String.Format("<p>Request for messages list<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Request for messages list";
                    pExampleResultBlock4String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), resourceURL, "", strResult);
                    
                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock4String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
                pExampleResultBlock4String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }

        }

        // EXAMPLE 5 - CREATE A MESSAGE WITH IMAGES AND ATTACHMENTS
        protected void RunExample5_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    byte[] imageBytes = System.IO.File.ReadAllBytes(HttpRuntime.AppDomainAppPath + @"Images\mailup-logo.png");
                    String image = System.Convert.ToBase64String(imageBytes);
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Images";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    String imageRequest = "{\"Base64Data\":\"" + image + "\",\"Name\":\"Avatar\"}";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.POST.ToString(),
                                                         imageRequest,
                                                         MailUp.ContentType.Json);

                    // status += String.Format("<p>Upload an image<br/>{0} {1} - OK</p>", httpMethods.PUT.ToString(), resourceURL);

                    responseMessage = "Upload an image";
                    pExampleResultBlock5String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.PUT.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, image, strResult);
                    
                    // Get the images available
                    apiUrl = "/Console/Images";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    String imgSrc = "";
                    Object[] srcs = (Object[])objResult;
                    if (srcs.Length > 0) imgSrc = srcs[0].ToString();

                    // status += String.Format("<p>Get the images available<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Get the images available";
                    pExampleResultBlock5String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                    // Create and save "hello" message
                    String message = "&lt;html&gt;&lt;body&gt;&lt;p&gt;Hello&lt;/p&gt;&lt;img src=\\\"" + imgSrc + "\\\"/&gt;&lt;/body&gt;&lt;/html&gt;";
                    message = "<html><body><p>Hello</p><img src=\"" + imgSrc + "\" /></body></html>";
                    
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Email";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;

                    EmailMessageItemDTO dto = new EmailMessageItemDTO();
                    dto.Subject = "Test Message c#";
                    dto.idList = 1;
                    dto.Content = message;
                    dto.Embed = true;
                    dto.IsConfirmation = true;
                    dto.Fields = new List<EmailDynamicFieldDTO>();
                    dto.Notes = "Some notes";
                    dto.Tags = new List<EmailTagDTO>();
                    dto.TrackingInfo = new EmailTrackingInfoDTO()
                    {
                        CustomParams = "",
                        Enabled = true,
                        Protocols = new List<String>() { "http" }
                    };

                    JavaScriptSerializer ser = new JavaScriptSerializer();

                    String emailRequest = ser.Serialize(dto);
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.POST.ToString(),
                                                         emailRequest,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    items = (Dictionary<String, Object>)objResult;
                    Dictionary<String, Object> template = (Dictionary<String, Object>)objResult;
                    var emailId = template["idMessage"];
                    Session["emailId"] = emailId;

                    // status += String.Format("<p>Create and save \"hello\" message<br/>{0} {1} - OK</p>", httpMethods.POST.ToString(), resourceURL);

                    responseMessage = "Create and save \"hello\" message";
                    pExampleResultBlock5String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.POST.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, emailRequest, strResult);
                    
                    // Add an attachment
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Email/" + emailId + "/Attachment/1";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    String attachment = "QmFzZSA2NCBTdHJlYW0="; // Base64 String
                    String attachmentRequest = "{\"Base64Data\":\"" + attachment + "\",\"Name\":\"TestFile.txt\",\"Slot\":1,\"idList\":1,\"idMessage\":" + emailId + "}";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.POST.ToString(),
                                                         attachmentRequest,
                                                         MailUp.ContentType.Json);

                    // status += String.Format("<p>Add an attachment<br/>{0} {1} - OK</p>", httpMethods.POST.ToString(), resourceURL);

                    responseMessage = "Add an attachment";
                    pExampleResultBlock5String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.POST.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, attachmentRequest, strResult);
                    
                    // Retreive message details
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Email/" + emailId;
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    // status += String.Format("<p>Retreive message details<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Retreive message details";
                    pExampleResultBlock5String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    
                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock5String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
                pExampleResultBlock5String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }

        }

        // EXAMPLE 6 - TAG A MESSAGE
        protected void RunExample6_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";
                    String requestMessage = "\"test tag\"";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Create a new tag
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Tag";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.POST.ToString(),
                                                         requestMessage,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    Object[] tags;
                    Dictionary<String, Object> tag = (Dictionary<String, Object>)objResult;
                    int tagId = int.Parse(tag["Id"].ToString());

                    // status += String.Format("<p>Create a new tag<br/>{0} {1} - OK</p>", httpMethods.POST.ToString(), resourceURL);

                    responseMessage = "Create a new tag";
                    pExampleResultBlock6String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.POST.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, requestMessage, strResult);
                    

                    // Pick up a message and retrieve detailed informations
                    int emailId = -1;
                    if (Session["emailId"] != null) emailId = (int)Session["emailId"];
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Email/" + emailId;
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    // status += String.Format("<p>Pick up a message and retrieve detailed informations<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Pick up a message and retrieve detailed informations";
                    pExampleResultBlock6String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    

                    // Add the tag to the message details and save
                    Dictionary<String, Object> objEmail = (Dictionary<String, Object>)objResult;
                    tags = (Object[])objEmail["Tags"];
                    List<Object> al = new List<Object>(tags);
                    Dictionary<String, Object> tagItem = new Dictionary<String, Object>();
                    tagItem["Id"] = tagId;
                    tagItem["Enabled"] = true;
                    tagItem["Name"] = "test tag";
                    al.Add(tagItem);
                    objEmail["Tags"] = al.ToArray();

                    String emailUpdateRequest = new JavaScriptSerializer().Serialize(objEmail);
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Email/" + emailId;
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.PUT.ToString(),
                                                         emailUpdateRequest,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    // status += String.Format("<p>Add the tag to the message details and save<br/>{0} {1} - OK</p>", httpMethods.PUT.ToString(), resourceURL);

                    responseMessage = "Add the tag to the message details and save";
                    pExampleResultBlock6String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.PUT.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, emailUpdateRequest, strResult);
                
                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock6String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
                pExampleResultBlock6String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }

        }

        // EXAMPLE 7 - SEND A MESSAGE
        protected void RunExample7_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Get the list of the existing messages
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Emails";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    items = (Dictionary<String, Object>)objResult;
                    Object[] emails = (Object[])items["Items"];
                    Dictionary<String, Object> email = (Dictionary<String, Object>)emails[0];
                    int emailId = int.Parse(email["idMessage"].ToString());
                    Session["emailId"] = emailId;

                    // status += String.Format("<p>Get the list of the existing messages<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Get the list of the existing messages";
                    pExampleResultBlock7String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);

                    // Send email to all recipients in the list
                    apiUrl = "/Console/List/" + mailUp.ListId + "/Email/" + emailId + "/Send";
                    resourceURL = "" + mailUp.ConsoleEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.POST.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);

                    // status += String.Format("<p>Send email to all recipients in the list<br/>{0} {1} - OK</p>", httpMethods.POST.ToString(), resourceURL);

                    responseMessage = "Send email to all recipients in the list";
                    pExampleResultBlock7String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.POST.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    
                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock7String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
                pExampleResultBlock7String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }

        }

        // EXAMPLE 8 - DISPLAY STATISTICS FOR A MESSAGE SENT AT EXAMPLE 7
        protected void RunExample8_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            String responseMessage = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String apiUrl = "";
                    String strResult = "";

                    // Request (to MailStatisticsService.svc) for paged message views list for the previously sent message
                    int hours = 4;
                    int emailId = -1;
                    if (Session["emailId"] != null) emailId = (int)Session["emailId"];
                    apiUrl = "/Message/" + emailId + "/List/Views?pageSize=5&pageNum=0";
                    resourceURL = "" + mailUp.MailstatisticsEndpoint + apiUrl;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         httpMethods.GET.ToString(),
                                                         null,
                                                         MailUp.ContentType.Json);

                    // status += String.Format("<p>Request (to MailStatisticsService.svc) for paged message views list for the previously sent message<br/>{0} {1} - OK</p>", httpMethods.GET.ToString(), resourceURL);

                    responseMessage = "Request (to MailStatisticsService.svc) for paged message views list for the previously sent message";
                    pExampleResultBlock8String.InnerHtml += String.Format(resultBlock, responseMessage, httpMethods.GET.ToString(), contentType.JSON.ToString(), endPotint.Console.ToString(), apiUrl, "", strResult);
                    
                    // status += "<p>Example methods completed successfully</p>";
                    pExampleResultBlock8String.InnerHtml += String.Format(successfullyAnswer, "Example methods completed successfully");
                }
            }
            catch (MailUp.MailUpException ex)
            {
                // status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
                pExampleResultBlock8String.InnerHtml += String.Format(errorAnswer, "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode);
            }
        }
    }
}

