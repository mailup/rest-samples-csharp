using System;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            // This "code" parameter is used when called back from MailUp logon. 
            // Need to complete logging on by retreiving the access token.
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            if (Request.Params["code"] != null)
            {
                if (mailUp != null && mailUp.AccessToken == null)
                {
                    String token = mailUp.RetreiveAccessToken(Request.Params["code"]);                   
                }
            }

            if (mailUp != null && mailUp.AccessToken != null)
            {
                pAuthorization.InnerText = "Authorized. Token: " + mailUp.AccessToken;
            }
            else
            {
                pAuthorization.InnerText = "Unauthorized";
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

        // Call method button - calls a single API method.
        protected void CallMethod_ServerClick(object sender, EventArgs e)
        {
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            try
            {
                if (mailUp != null)
                {
                    String resourceURL = ""+(lstEndpoint.SelectedValue == "Console"? mailUp.ConsoleEndpoint+txtPath.Text : mailUp.MailstatisticsEndpoint+txtPath.Text);


                    String strResult = mailUp.CallMethod(resourceURL, 
                                                         lstVerb.SelectedValue, 
                                                         txtBody.Text,
                                                         lstContentType.SelectedValue == "JSON" ? MailUp.ContentType.Json : MailUp.ContentType.Xml);

                    pResultString.InnerText = txtPath.Text + " returned: " +strResult;
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
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];
            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();                  

                    // Given a default list id (use idList = 1), request for user visible groups
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Groups";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
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

                    status += String.Format("<p>Given a default list id (use idList = 1), request for user visible groups<br/>{0} {1} - OK</p>", "GET", resourceURL);

                    // If the list does not contain a group named “test import”, create it
                    if (groupId == -1)
                    {
                        groupId = 100;
                        resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Group";
                        String groupRequest = "{\"Deletable\":true,\"Name\":\"test import\",\"Notes\":\"test import\"}";
                        strResult = mailUp.CallMethod(resourceURL,
                                                             "POST",
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

                    status += String.Format("<p>If the list does not contain a group named “test import”, create it<br/>{0} {1} - OK</p>", "POST", resourceURL);

                    // Request for dynamic fields to map recipient name and surname
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/Recipient/DynamicFields";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);

                    status += String.Format("<p>Request for dynamic fields to map recipient name and surname<br/>{0} {1} - OK</p>", "GET", resourceURL);

                    // Import recipients to group
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/Group/" + groupId + "/Recipients";
                    String recipientRequest = "[{\"Email\":\"test@test.test\",\"Fields\":[{\"Description\":\"String description\",\"Id\":1,\"Value\":\"String value\"}]," +
                        "\"MobileNumber\":\"\",\"MobilePrefix\":\"\",\"Name\":\"John Smith\"}]";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "POST",
                                                         recipientRequest,
                                                         MailUp.ContentType.Json);
                    int importId = int.Parse(strResult);

                    status += "<p>Import recipients to group - OK<p>";

                    // Check the import result
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/Import/" + importId;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);

                    status += "<p>Check the import result - OK<p>";

                    status += "<p>Example methods completed successfully<p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }

        // EXAMPLE 2 - UNSUBSCRIBE A RECIPIENT FROM A GROUP
        protected void RunExample2_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Request for recipient in a group
                    int groupId = -1;
                    if (Session["groupId"] != null) groupId = (int)Session["groupId"];
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/Group/" + groupId + "/Recipients";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    status += String.Format("<p>Request for recipient in a group<br/>{0} {1} - OK</p>","GET",resourceURL);

                    items = (Dictionary<String, Object>)objResult;
                    Object[] recipients = (Object[])items["Items"];
                    if (recipients.Length > 0)
                    {
                        Dictionary<String, Object> recipient = (Dictionary<String, Object>)recipients[0];
                        int recipientId = int.Parse(recipient["idRecipient"].ToString());

                        // Pick up a recipient and unsubscribe it
                        resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/Group/" + groupId + "/Unsubscribe/" + recipientId;
                        strResult = mailUp.CallMethod(resourceURL,
                                                             "DELETE",
                                                             null,
                                                             MailUp.ContentType.Json);

                        status += String.Format("<p>Pick up a recipient and unsubscribe it<br/>{0} {1} - OK</p>", "DELETE", resourceURL);
                    }

                    status += "<p>Example methods completed successfully</p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }

        // EXAMPLE 3 - UPDATE A RECIPIENT DETAIL
        protected void RunExample3_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Request for existing subscribed recipients
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Recipients/Subscribed";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    status += String.Format("<p>Request for existing subscribed recipients<br/>{0} {1} - OK</p>","GET",resourceURL);

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

                        status += "<p>Modify a recipient from the list - OK</p>";

                        // Update the modified recipient
                        String recipientRequest = new JavaScriptSerializer().Serialize(recipient);
                        resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/Recipient/Detail";
                        strResult = mailUp.CallMethod(resourceURL,
                                                             "PUT",
                                                             recipientRequest,
                                                             MailUp.ContentType.Json);

                        status += String.Format("<p>Update the modified recipient<br/>{0} {1} - OK</p>","PUT",resourceURL);
                    }

                    status += "<p>Example methods completed successfully</p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }

        // EXAMPLE 4 - CREATE A MESSAGE FROM TEMPLATE
        protected void RunExample4_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();
                  
                    // Get the available template list
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Templates";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    Object[] templates = (Object[])objResult;
                    Dictionary<String, Object> template = (Dictionary<String, Object>)templates[0];
                    int templateId = int.Parse(template["Id"].ToString());

                    status += String.Format("<p>Get the available template list<br/>{0} {1} - OK</p>","GET",resourceURL);

                    // Create the new message
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Email/Template/" + templateId;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "POST",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    items = (Dictionary<String, Object>)objResult;
                    Object[] emails = (Object[])items["Items"];
                    Dictionary<String, Object> email = (Dictionary<String, Object>)emails[0];
                    int emailId = int.Parse(email["idMessage"].ToString());

                    status += String.Format("<p>Create the new message<br/>{0} {1} - OK</p>","POST",resourceURL);

                    // Request for messages list
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Emails";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    status += String.Format("<p>Request for messages list<br/>{0} {1} - OK</p>","GET",resourceURL);

                    status += "<p>Example methods completed successfully</p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }

        // EXAMPLE 5 - CREATE A MESSAGE WITH IMAGES AND ATTACHMENTS
        protected void RunExample5_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Upload an image
                    // Image bytes can be obtained from file, database or any other source
                    WebClient wc = new WebClient();
                    byte[] imageBytes = wc.DownloadData("http://images.apple.com/home/images/ios_title_small.png");
                    String image = System.Convert.ToBase64String(imageBytes);
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Images";
                    String imageRequest = "{\"Base64Data\":\"" + image + "\",\"Name\":\"Avatar\"}";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "POST",
                                                         imageRequest,
                                                         MailUp.ContentType.Json);

                    status += String.Format("<p>Upload an image<br/>{0} {1} - OK</p>","PUT",resourceURL);

                    // Get the images available
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/Images";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    String imgSrc = "";
                    Object[] srcs = (Object[])objResult;
                    if (srcs.Length > 0) imgSrc = srcs[0].ToString();

                    status += String.Format("<p>Get the images available<br/>{0} {1} - OK</p>","GET",resourceURL);

                    // Create and save "hello" message
                    String message = "&lt;html&gt;&lt;body&gt;&lt;p&gt;Hello&lt;/p&gt;&lt;img src=\\\"" + imgSrc + "\\\"/&gt;&lt;/body&gt;&lt;/html&gt;";
                    message = "<html><body><p>Hello</p><img src=\"" + imgSrc + "\" /></body></html>";
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Email";

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
                                                         "POST",
                                                         emailRequest,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    items = (Dictionary<String, Object>)objResult;
                    Object[] emails = (Object[])items["Items"];
                    Dictionary<String, Object>email = (Dictionary<String, Object>)emails[0];
                    int emailId = int.Parse(email["idMessage"].ToString());
                    Session["emailId"] = emailId;

                    status += String.Format("<p>Create and save \"hello\" message<br/>{0} {1} - OK</p>","POST",resourceURL);

                    // Add an attachment
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Email/" + emailId + "/Attachment/1";
                    String attachment = "QmFzZSA2NCBTdHJlYW0="; // Base64 String
                    String attachmentRequest = "{\"Base64Data\":\"" + attachment + "\",\"Name\":\"TestFile.txt\",\"Slot\":1,\"idList\":1,\"idMessage\":" + emailId + "}";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "POST",
                                                         attachmentRequest,
                                                         MailUp.ContentType.Json);

                    status += String.Format("<p>Add an attachment<br/>{0} {1} - OK</p>","POST",resourceURL);

                    // Retreive message details
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Email/" + emailId;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    status += String.Format("<p>Retreive message details<br/>{0} {1} - OK</p>","GET",resourceURL);

                    status += "<p>Example methods completed successfully</p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }

        // EXAMPLE 6 - TAG A MESSAGE
        protected void RunExample6_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Create a new tag
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Tag";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "POST",
                                                         "\"test tag\"",
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    Object[] tags = (Object[])objResult;
                    Dictionary<String, Object> tag = (Dictionary<String, Object>)tags[0];
                    int tagId = int.Parse(tag["Id"].ToString());

                    status += String.Format("<p>Create a new tag<br/>{0} {1} - OK</p>","POST",resourceURL);

                    // Pick up a message and retrieve detailed informations
                    int emailId = -1;
                    if (Session["emailId"] != null) emailId = (int)Session["emailId"];
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Email/" + emailId;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    status += String.Format("<p>Pick up a message and retrieve detailed informations<br/>{0} {1} - OK</p>","GET",resourceURL);

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
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Email/" + emailId;
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "PUT",
                                                         emailUpdateRequest,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);

                    status += String.Format("<p>Add the tag to the message details and save<br/>{0} {1} - OK</p>","PUT",resourceURL);

                    status += "<p>Example methods completed successfully</p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }

        // EXAMPLE 7 - SEND A MESSAGE
        protected void RunExample7_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";
                    Object objResult;
                    Dictionary<String, Object> items = new Dictionary<String, Object>();

                    // Get the list of the existing messages
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Emails";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);
                    objResult = new JavaScriptSerializer().DeserializeObject(strResult);
                    items = (Dictionary<String, Object>)objResult;
                    Object[] emails = (Object[])items["Items"];
                    Dictionary<String, Object>  email = (Dictionary<String, Object>)emails[0];
                    int emailId = int.Parse(email["idMessage"].ToString());
                    Session["emailId"] = emailId;

                    status += String.Format("<p>Get the list of the existing messages<br/>{0} {1} - OK</p>","GET",resourceURL);

                    // Send email to all recipients in the list
                    resourceURL = "" + mailUp.ConsoleEndpoint + "/Console/List/1/Email/" + emailId + "/Send";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "POST",
                                                         null,
                                                         MailUp.ContentType.Json);

                    status += String.Format("<p>Send email to all recipients in the list<br/>{0} {1} - OK</p>","POST",resourceURL);

                    status += "<p>Example methods completed successfully</p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }

        // EXAMPLE 8 - DISPLAY STATISTICS FOR A MESSAGE SENT AT EXAMPLE 7
        protected void RunExample8_ServerClick(object sender, EventArgs e)
        {
            String status = "";
            MailUp.MailUpClient mailUp = (MailUp.MailUpClient)Session["MailUpClient"];

            try
            {
                if (mailUp != null)
                {
                    // List ID = 1 is used in all example calls
                    String resourceURL = "";
                    String strResult = "";

                    // Request (to MailStatisticsService.svc) for paged message views list for the previously sent message
                    int hours = 4;
                    int emailId = -1;
                    if (Session["emailId"] != null) emailId = (int)Session["emailId"];
                    resourceURL = "" + mailUp.MailstatisticsEndpoint + "/Message/" + emailId + "/Views/List/Last/" + hours + "?pageSize=5&pageNum=0";
                    strResult = mailUp.CallMethod(resourceURL,
                                                         "GET",
                                                         null,
                                                         MailUp.ContentType.Json);

                    status += String.Format("<p>Request (to MailStatisticsService.svc) for paged message views list for the previously sent message<br/>{0} {1} - OK</p>","GET",resourceURL);

                    status += "<p>Example methods completed successfully</p>";
                }
            }
            catch (MailUp.MailUpException ex)
            {
                status += "Exception: " + ex.Message + " with HTTP Status code: " + ex.StatusCode + "<br/>";
            }

            pExampleResultString.InnerHtml = status;
        }
        
    }
}
