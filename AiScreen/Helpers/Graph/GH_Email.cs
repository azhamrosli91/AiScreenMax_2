using Azure.Core;
using Azure.Identity;
using Microsoft.Graph.Models;
using Microsoft.Graph;
using Microsoft.Graph.Users.Item.SendMail;
using MaxSys.Models;
using MaxSys.Interface;
using MaxSystemWebSite.Models.EMAIL;
using MaxSystemWebSite.Models.SETTING;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using static System.Formats.Asn1.AsnWriter;
using System.Net.Http.Headers;
using E_Template.Helpers;

namespace MaxSystemWebSite.Helpers.Graph
{
    public class GH_Email : IEmail
    {
        // Settings object
        private static Settings? _settings;

        // Client secret credential
        private static ClientSecretCredential? _clientSecretCredential;
        // Client configured with application authentication
        private static GraphServiceClient? _appClient;

        //public GraphHelper(IOptions<Settings> settings)
        //{
            //_settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            //// Ensure that required properties are not null
            //if (string.IsNullOrEmpty(_settings.TenantId) ||
            //    string.IsNullOrEmpty(_settings.ClientId) ||
            //    string.IsNullOrEmpty(_settings.ClientSecret))
            //{
            //    throw new ArgumentException("Invalid Graph API settings");
            //}

            //// Initialize the credential using client secret
            //var clientSecretCredential = new ClientSecretCredential(
            //    _settings.TenantId,
            //    _settings.ClientId,
            //    _settings.ClientSecret
            //);

            //// Initialize Graph client with the credential and desired scopes
            //_appClient = new GraphServiceClient(clientSecretCredential, _settings.GraphUserScopes);
        //}

        public void InitGraph(SETTING_EMAIL settings)
        {

            // Ensure that required properties are not null
            if (string.IsNullOrEmpty(settings.TENANT_ID) ||
                string.IsNullOrEmpty(settings.CLIENT_ID) ||
                string.IsNullOrEmpty(settings.CLIENT_SECRET))
            {
                throw new ArgumentException("Invalid Graph API settings");
            }

            // Initialize the credential using client secret
            _clientSecretCredential = new ClientSecretCredential(
                settings.TENANT_ID,
                settings.CLIENT_ID,
                settings.CLIENT_SECRET
            );

            // Initialize Graph client with the credential and desired scopes
            _appClient = new GraphServiceClient(_clientSecretCredential, new[] { settings.GRAPH_USER });
        }

        #region "EMAIL"
        public async Task<(bool success, string message, List<EmailContent_Response>? data)> GetEmailBodyContentByConversationID(string userID, string conversationId)
        {
            try
            {
                // Ensure the Graph client is initialized
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app auth", null);
                }

                // Validate input parameters
                if (string.IsNullOrWhiteSpace(userID) || string.IsNullOrWhiteSpace(conversationId))
                {
                    return (false, "User ID or Conversation ID cannot be null or empty", null);
                }

                // List to store messages from both folders
                var allMessages = new List<Message>();

                // List of folder names to search
                var foldersToSearch = new[] { "inbox", "sentitems" };

                foreach (var folderName in foldersToSearch)
                {
                    // Retrieve all messages in the folder filtered by conversationId
                    var messages = await _appClient.Users[userID]
                        .MailFolders[folderName]
                        .Messages
                        .GetAsync((config) =>
                        {
                            config.QueryParameters.Filter = $"conversationId eq '{conversationId}'";
                            config.QueryParameters.Select = new[] { "id", "from", "isRead", "receivedDateTime", "subject", "body", "hasAttachments" };
                        });

                    foreach (var message in messages.Value)
                    {
                        if (message.Body != null && message.Body.Content != null)
                        {
                            //// Assuming replies are separated by a common pattern like "-----Original Message-----"
                            string pattern = @"<div id=""appendonsend""></div>.*</body></html>";
                            message.Body.Content = Regex.Replace(message.Body.Content, pattern, "", RegexOptions.Singleline);
                        }
                    }

                    if (messages?.Value != null && messages.Value.Count > 0)
                    {
                        allMessages.AddRange(messages.Value);
                    }
                }

                if (allMessages.Count == 0)
                {
                    return (false, "No messages found for the specified conversation in Inbox or Sent Items", null);
                }

                // List to store enriched email responses
                var emailResponse = new List<EmailContent_Response>();

                foreach (var message in allMessages)
                {
                    // Ensure message.Body.Content is treated as HTML
                    string emailBodyContent = message.Body.ContentType == BodyType.Html
                        ? message.Body.Content
                        : $"<pre>{System.Net.WebUtility.HtmlEncode(message.Body.Content)}</pre>";

                    Dictionary<string, string> inlineImages = new Dictionary<string, string>();


                    var attachments = await _appClient.Users[userID]
                                            .Messages[message.Id]
                                            .Attachments
                                            .GetAsync();

                    if (attachments?.Value != null)
                    {
                        foreach (var attachment in attachments.Value)
                        {
                            if (attachment is FileAttachment fileAttachment && fileAttachment.IsInline.GetValueOrDefault(false) && !string.IsNullOrEmpty(fileAttachment.ContentId))
                            {
                                var base64Content = Convert.ToBase64String(fileAttachment.ContentBytes);
                                var contentType = fileAttachment.ContentType; // Get the MIME type

                                // Store inline images for replacement
                                inlineImages[fileAttachment.ContentId] = $"data:{contentType};base64,{base64Content}";
                            }
                        }
                    }

                    // Replace all inline images in the email body
                    foreach (var inlineImage in inlineImages)
                    {
                        emailBodyContent = emailBodyContent.Replace(
                            $"src=\"cid:{inlineImage.Key}\"",
                            $"src=\"{inlineImage.Value}\""
                        ).Replace(
                            $"src='cid:{inlineImage.Key}'",
                            $"src=\"{inlineImage.Value}\""
                        );
                    }

                    // UTC DateTime string
                    string utcDateTimeString = message.ReceivedDateTime.ToString();

                    // Convert to DateTimeOffset
                    DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(utcDateTimeString);

                    // Convert to local time
                    DateTime localDateTime = dateTimeOffset.LocalDateTime;

                    // Format as dd/MM/yyyy hh:mm tt
                    string formattedDate = localDateTime.ToString("dd/MM/yyyy hh:mm:ss tt");

                    emailResponse.Add(new EmailContent_Response
                    {
                        Id = message.Id,
                        Attachments = message.Attachments,
                        BccRecipients = message.BccRecipients,
                        Content = emailBodyContent, // Set the processed HTML content
                        ContentType = message.Body.ContentType,
                        CcRecipients = message.CcRecipients,
                        ConversationId = message.ConversationId,
                        From = message.From,
                        HasAttachments = message.HasAttachments,
                        IsRead = message.IsRead,
                        ReceivedDateTime = message.ReceivedDateTime,
                        ReceivedDateTimeDesc = formattedDate,
                        ReplyTo = message.ReplyTo,
                        Sender = message.Sender,
                        SentDateTime = message.SentDateTime,
                        Subject = message.Subject,
                        ToRecipients = message.ToRecipients
                    });
                }
                emailResponse = emailResponse.OrderByDescending(m => m.ReceivedDateTime).ToList();
                return (true, "Email contents retrieved successfully", emailResponse);
            }
            catch (ServiceException ex)
            {
                // Handle Graph API exceptions
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, List<EmailContent_Response>? data)> GetEmailBodyContentByMessageID(string userID, string messageId)
        {
            try
            {
                // Ensure the Graph client is initialized
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app auth", null);
                }

                // Validate input parameters
                if (string.IsNullOrWhiteSpace(userID) || string.IsNullOrWhiteSpace(messageId))
                {
                    return (false, "User ID or Message ID cannot be null or empty", null);
                }

                // List to store messages from both folders
                var allMessages = new List<Message>();

                // List of folder names to search
                var foldersToSearch = new[] { "inbox", "sentitems" };

                foreach (var folderName in foldersToSearch)
                {
                    // Retrieve all messages in the folder filtered by conversationId
                    var messages = await _appClient.Users[userID]
                        .MailFolders[folderName]
                        .Messages
                        .GetAsync((config) =>
                        {
                            config.QueryParameters.Filter = $"messageId eq '{messageId}'";
                            config.QueryParameters.Select = new[] { "id", "from", "isRead", "receivedDateTime", "subject", "body", "hasAttachments" };
                        });

                    foreach (var message in messages.Value)
                    {
                        if (message.Body != null && message.Body.Content != null)
                        {
                            //// Assuming replies are separated by a common pattern like "-----Original Message-----"
                            string pattern = @"<div id=""appendonsend""></div>.*</body></html>";
                            message.Body.Content = Regex.Replace(message.Body.Content, pattern, "", RegexOptions.Singleline);
                        }
                    }

                    if (messages?.Value != null && messages.Value.Count > 0)
                    {
                        allMessages.AddRange(messages.Value);
                    }
                }

                if (allMessages.Count == 0)
                {
                    return (false, "No messages found for the specified conversation in Inbox or Sent Items", null);
                }

                // List to store enriched email responses
                var emailResponse = new List<EmailContent_Response>();

                foreach (var message in allMessages)
                {
                    // Ensure message.Body.Content is treated as HTML
                    string emailBodyContent = message.Body.ContentType == BodyType.Html
                        ? message.Body.Content
                        : $"<pre>{System.Net.WebUtility.HtmlEncode(message.Body.Content)}</pre>";

                    Dictionary<string, string> inlineImages = new Dictionary<string, string>();


                    var attachments = await _appClient.Users[userID]
                                            .Messages[message.Id]
                                            .Attachments
                                            .GetAsync();

                    if (attachments?.Value != null)
                    {
                        foreach (var attachment in attachments.Value)
                        {
                            if (attachment is FileAttachment fileAttachment && fileAttachment.IsInline.GetValueOrDefault(false) && !string.IsNullOrEmpty(fileAttachment.ContentId))
                            {
                                var base64Content = Convert.ToBase64String(fileAttachment.ContentBytes);
                                var contentType = fileAttachment.ContentType; // Get the MIME type

                                // Store inline images for replacement
                                inlineImages[fileAttachment.ContentId] = $"data:{contentType};base64,{base64Content}";
                            }
                        }
                    }

                    // Replace all inline images in the email body
                    foreach (var inlineImage in inlineImages)
                    {
                        emailBodyContent = emailBodyContent.Replace(
                            $"src=\"cid:{inlineImage.Key}\"",
                            $"src=\"{inlineImage.Value}\""
                        ).Replace(
                            $"src='cid:{inlineImage.Key}'",
                            $"src=\"{inlineImage.Value}\""
                        );
                    }

                    // UTC DateTime string
                    string utcDateTimeString = message.ReceivedDateTime.ToString();

                    // Convert to DateTimeOffset
                    DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(utcDateTimeString);

                    // Convert to local time
                    DateTime localDateTime = dateTimeOffset.LocalDateTime;

                    // Format as dd/MM/yyyy hh:mm tt
                    string formattedDate = localDateTime.ToString("dd/MM/yyyy hh:mm:ss tt");

                    emailResponse.Add(new EmailContent_Response
                    {
                        Id = message.Id,
                        Attachments = message.Attachments,
                        BccRecipients = message.BccRecipients,
                        Content = emailBodyContent, // Set the processed HTML content
                        ContentType = message.Body.ContentType,
                        CcRecipients = message.CcRecipients,
                        ConversationId = message.ConversationId,
                        From = message.From,
                        HasAttachments = message.HasAttachments,
                        IsRead = message.IsRead,
                        ReceivedDateTime = message.ReceivedDateTime,
                        ReceivedDateTimeDesc = formattedDate,
                        ReplyTo = message.ReplyTo,
                        Sender = message.Sender,
                        SentDateTime = message.SentDateTime,
                        Subject = message.Subject,
                        ToRecipients = message.ToRecipients
                    });
                }
                emailResponse = emailResponse.OrderByDescending(m => m.ReceivedDateTime).ToList();
                return (true, "Email contents retrieved successfully", emailResponse);
            }
            catch (ServiceException ex)
            {
                // Handle Graph API exceptions
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, EmailContent_Response? data)> GetEmailBodyContentByV1MessageID(string userID, string messageId)
        {
            try
            {
                // Ensure the Graph client is initialized
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app auth", null);
                }

                // Validate input parameters
                if (string.IsNullOrWhiteSpace(userID) || string.IsNullOrWhiteSpace(messageId))
                {
                    return (false, "User ID or Message ID cannot be null or empty", null);
                }

                // Retrieve the specific message by ID
                var message = await _appClient.Users[userID]
                    .Messages[messageId]
                    .GetAsync((config) =>
                    {
                        // Include 'body' and 'attachments' metadata in the select query
                        config.QueryParameters.Select = new[] { "id", "from", "isRead", "receivedDateTime", "subject", "body", "hasAttachments" };
                    });

                if (message == null)
                {
                    return (false, "Message not found", null);
                }

                // Ensure message.Body.Content is treated as HTML
                string emailBodyContent = message.Body.ContentType == BodyType.Html
                    ? message.Body.Content
                    : $"<pre>{System.Net.WebUtility.HtmlEncode(message.Body.Content)}</pre>";

                Dictionary<string, string> inlineImages = new Dictionary<string, string>();

                // Fetch attachments (even if hasAttachments = false)
                var attachments = await _appClient.Users[userID]
                    .Messages[message.Id]
                    .Attachments
                    .GetAsync();

                if (attachments?.Value != null)
                {
                    foreach (var attachment in attachments.Value)
                    {
                        if (attachment is FileAttachment fileAttachment && fileAttachment.IsInline.GetValueOrDefault(false) && !string.IsNullOrEmpty(fileAttachment.ContentId))
                        {
                            var base64Content = Convert.ToBase64String(fileAttachment.ContentBytes);
                            var contentType = fileAttachment.ContentType; // Get the MIME type

                            // Store inline images for replacement
                            inlineImages[fileAttachment.ContentId] = $"data:{contentType};base64,{base64Content}";
                        }
                    }
                }

                // Replace all inline images in the email body
                foreach (var inlineImage in inlineImages)
                {
                    emailBodyContent = emailBodyContent.Replace(
                        $"src=\"cid:{inlineImage.Key}\"",
                        $"src=\"{inlineImage.Value}\""
                    ).Replace(
                        $"src='cid:{inlineImage.Key}'",
                        $"src=\"{inlineImage.Value}\""
                    );
                }

                // UTC DateTime string
                string utcDateTimeString = message.ReceivedDateTime.ToString();

                // Convert to DateTimeOffset
                DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(utcDateTimeString);

                // Convert to local time
                DateTime localDateTime = dateTimeOffset.LocalDateTime;

                // Format as dd/MM/yyyy hh:mm tt
                string formattedDate = localDateTime.ToString("dd/MM/yyyy hh:mm:ss tt");

                // Construct response
                EmailContent_Response emailResponse = new EmailContent_Response
                {
                    Id = message.Id,
                    Attachments = message.Attachments,
                    BccRecipients = message.BccRecipients,
                    Content = emailBodyContent, // Set the processed HTML content
                    ContentType = message.Body.ContentType,
                    CcRecipients = message.CcRecipients,
                    ConversationId = message.ConversationId,
                    From = message.From,
                    HasAttachments = message.HasAttachments,
                    IsRead = message.IsRead,
                    ReceivedDateTime = message.ReceivedDateTime,
                    ReceivedDateTimeDesc = formattedDate,
                    ReplyTo = message.ReplyTo,
                    Sender = message.Sender,
                    SentDateTime = message.SentDateTime,
                    Subject = message.Subject,
                    ToRecipients = message.ToRecipients
                };

                return (true, "Email content retrieved successfully", emailResponse);
            }
            catch (ServiceException ex)
            {
                // Handle Graph API exceptions
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, List<EmailList> data)> GetEmailListShort(
            string userID,
            string foldername = "inbox",
            string[] attribute = null,
            int count = 10,
            string[] orderby = null,
            DateTime? fromDate = null)
        {
            try
            {
                // Check if the Graph client is initialized
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app auth", null);
                }

                // Validate the userID
                if (string.IsNullOrWhiteSpace(userID))
                {
                    return (false, "User ID cannot be null or empty", null);
                }

                // Validate count
                if (count <= 0)
                {
                    return (false, "Count must be greater than zero", null);
                }

                // Set default values for attribute and orderby if not provided
                attribute ??= new[] { "id", "conversationId", "from", "isRead", "receivedDateTime", "subject" };
                orderby ??= new[] { "receivedDateTime DESC" };

                // Fetch emails from the specified folder
                var result = await _appClient.Users[userID]
                    .MailFolders[foldername]
                    .Messages
                    .GetAsync((config) =>
                    {
                        config.QueryParameters.Select = attribute;
                        config.QueryParameters.Top = count;
                        config.QueryParameters.Orderby = orderby;
                        // Apply filter only if fromDate is provided
                        if (fromDate.HasValue)
                        {
                            string isoDate = fromDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                            config.QueryParameters.Filter = $"receivedDateTime ge {isoDate}";
                        }
                    });

                // Return success with data

                if (result == null)
                {
                    return (false, $"Service Error: Result Empty", null);
                }
                string bodyContent = "";
                List<EmailList> listemail = new List<EmailList>();
                foreach (var item in result.Value)
                {
                    if (item.Body != null)
                    {
                        bodyContent = item.Body.ContentType == BodyType.Html
                       ? item.Body.Content
                       : $"<pre>{System.Net.WebUtility.HtmlEncode(item.Body.Content)}</pre>";
                    }


                    listemail.Add(
                        new EmailList
                        (
                            item.Id,
                            item.ConversationId,
                            item.From?.EmailAddress.Address,
                            item.ToRecipients?.Select(recipient => recipient.EmailAddress.Address).ToList(),
                            item.ReceivedDateTime?.ToLocalTime().ToString(),
                            item.Subject,
                            bodyContent
                        )
                    );
                }

                return (true, "Email list retrieved successfully", listemail);
            }
            catch (ServiceException ex)
            {
                // Handle Graph API service exceptions
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, MessageCollectionResponse data)> GetEmailList(
            string userID,
            string foldername = "inbox",
            string[] attribute = null,
            int count = 10,
            string[] orderby = null)
        {
            try
            {
                // Check if the Graph client is initialized
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app auth", null);
                }

                // Validate the userID
                if (string.IsNullOrWhiteSpace(userID))
                {
                    return (false, "User ID cannot be null or empty", null);
                }

                // Validate count
                if (count <= 0)
                {
                    return (false, "Count must be greater than zero", null);
                }

                // Set default values for attribute and orderby if not provided
                attribute ??= new[] { "id", "conversationId", "from", "isRead", "receivedDateTime", "subject" };
                orderby ??= new[] { "receivedDateTime DESC" };

                // Fetch emails from the specified folder
                var result = await _appClient.Users[userID]
                    .MailFolders[foldername]
                    .Messages
                    .GetAsync((config) =>
                    {
                        config.QueryParameters.Select = attribute;
                        config.QueryParameters.Top = count;
                        config.QueryParameters.Orderby = orderby;
                    });

                // Return success with data
                return (true, "Email list retrieved successfully", result);
            }
            catch (ServiceException ex)
            {
                // Handle Graph API service exceptions
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return (false, $"Error: {ex.Message}", null);
            }
        }

        #endregion
        #region "TEAMS"
        // ---------------------------------------------------------------------------------
        // 1. Get List of Teams (Returns Team IDs)
        // ---------------------------------------------------------------------------------
        public async Task<(bool success, string message, List<Team> teams)> GetTeamsAsync()
        {
            try
            {
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app authentication", null);
                }

                // Retrieve all Teams (requires Team.ReadBasic.All permission)
                var teams = await _appClient.Teams.GetAsync();

                if (teams == null || teams.Value.Count == 0)
                {
                    return (false, "No Teams found in the organization", null);
                }

                return (true, "Teams retrieved successfully", teams.Value);
            }
            catch (ServiceException ex)
            {
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }


        // ---------------------------------------------------------------------------------
        // 2. Get List of Channels within a Team (Returns Channel IDs)
        // ---------------------------------------------------------------------------------
        public async Task<(bool success, string message, List<Channel> channels)> GetChannelsAsync(string teamId)
        {
            try
            {
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app authentication", null);
                }

                var channels = await _appClient.Teams[teamId].Channels.GetAsync();

                if (channels == null || channels.Value.Count == 0)
                {
                    return (false, "No Channels found in this Team", null);
                }

                return (true, "Channels retrieved successfully", channels.Value);
            }
            catch (ServiceException ex)
            {
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }


        // ---------------------------------------------------------------------------------
        // 3. Send a Message to a Teams Channel
        // ---------------------------------------------------------------------------------
        public async Task<(bool success, string message)> SendMessageToChannelAsync(string teamId, string channelId, string message)
        {
            try
            {
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app authentication");
                }

                var chatMessage = new ChatMessage
                {
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = message
                    }
                };

                await _appClient.Teams[teamId].Channels[channelId].Messages.PostAsync(chatMessage);

                return (true, "Message sent successfully to the Teams channel");
            }
            catch (ServiceException ex)
            {
                return (false, $"Service Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
        public async Task<(bool success, string message, string? userId)> GetUserIdByEmailAsync(string userEmail)
        {
            try
            {
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app authentication", null);
                }

                var user = await _appClient.Users[userEmail].GetAsync();

                if (user == null)
                {
                    return (false, "User not found", null);
                }

                return (true, "User ID retrieved successfully", user.Id);
            }
            catch (ServiceException ex)
            {
                return (false, $"Service Error: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }
        //public static async Task<bool> SendEmailAsync(Emai_Template model)
        //{
        //    try
        //    {
        //        // Ensure client isn't null
        //        _ = _appClient ??
        //            throw new NullReferenceException("Graph has not been initialized for app auth");

        //        if (model == null)
        //        {
        //            return false;
        //        }

        //        model.mainTemplate = await model.EmailBodyTemplate();
        //        model.bodyContent = model.mainTemplate.ToString().Replace("[BODY]", model.subTemplate);
        //        (bool success, string template) returnTemp = model.WordReplacer(model.bodyContent);
        //        if (returnTemp.success == false)
        //        {
        //            return false;
        //        }
        //        model.bodyContent = returnTemp.template;

        //        // Create the email message
        //        var message = new Message
        //        {
        //            Subject = model.Subject,
        //            Body = new ItemBody
        //            {
        //                ContentType = BodyType.Html, // Change to HTML content type
        //                Content = model.bodyContent
        //            },
        //            ToRecipients = model.Recipient ?? new List<Recipient>(),
        //            CcRecipients = model.CC ?? new List<Recipient>(),
        //            BccRecipients = model.BCC ?? new List<Recipient>()
        //        };


        //        // Create the request body
        //        var requestBody = new SendMailPostRequestBody
        //        {
        //            Message = message,
        //            SaveToSentItems = false
        //        };

        //        // Send the email
        //        if (model.Setting_Setup != null && !string.IsNullOrEmpty(model.Setting_Setup.SMTP_ACCOUNT)) {  
        //            await _appClient.Users[model.Setting_Setup.SMTP_ACCOUNT].SendMail.PostAsync(requestBody);
        //            return true;
        //        }

        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public async Task<(bool success, string message)> SendEmailAsync(Emai_TemplateSent model)
        {
            try
            {
                // Ensure client isn't null
                _ = _appClient ??
                    throw new NullReferenceException("Graph has not been initialized for app auth");

                if (model == null)
                {
                    return (false, "Graph has not been initialized for app auth");
                }

                model.mainTemplate = await model.EmailBodyTemplate();
                model.bodyContent = model.mainTemplate.ToString().Replace("[BODY]", model.subTemplate);
                (bool success, string template) returnTemp = model.WordReplacer(model.bodyContent);
                if (returnTemp.success == false)
                {
                    return (false, "Failed on word replacer");
                }
                model.bodyContent = returnTemp.template;

                // Create the email message
                var message = new Message
                {
                    Subject = model.Subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = model.bodyContent
                    },
                    ToRecipients = model.Recipient ?? new List<Recipient>(),
                    CcRecipients = model.CC ?? new List<Recipient>(),
                    BccRecipients = model.BCC ?? new List<Recipient>(),
                    Attachments = new List<Attachment>()
                };

                // Add attachments if any
                if (model.Attachments != null && model.Attachments.Any())
                {
                    foreach (var file in model.Attachments)
                    {
                        var fileAttachment = new FileAttachment
                        {
                            OdataType = "#microsoft.graph.fileAttachment",
                            Name = file.FileName,
                            ContentBytes = file.FileContent,
                            ContentType = file.ContentType
                        };

                        message.Attachments.Add(fileAttachment);
                    }
                }

                // Create the request body
                var requestBody = new SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = false
                };

                // Send the email
                if (model.Setting_Setup != null && !string.IsNullOrEmpty(model.Setting_Setup.SMTP_ACCOUNT))
                {
                    await _appClient.Users[model.Setting_Setup.SMTP_ACCOUNT].SendMail.PostAsync(requestBody);
                    return (true, "ok");
                }

                return (false, "setting setup not found");
            }
            catch (Exception ex)
            {
                // Log ex.Message if needed
                return (false, $"failed {ex.Message}");
            }
        }
        public async Task<(bool success, string message)> SendComposeEmailAsync(Emai_TemplateSent model)
        {
            try
            {
                // Ensure client isn't null
                _ = _appClient ??
                    throw new NullReferenceException("Graph has not been initialized for app auth");

                if (model == null)
                {
                    return (false, "Graph has not been initialized for app auth");
                }

                // Create the email message
                var message = new Message
                {
                    Subject = model.Subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = model.bodyContent
                    },
                    ToRecipients = model.Recipient ?? new List<Recipient>(),
                    CcRecipients = model.CC ?? new List<Recipient>(),
                    BccRecipients = model.BCC ?? new List<Recipient>(),
                    Attachments = new List<Attachment>()
                };

                // Add attachments if any
                if (model.Attachments != null && model.Attachments.Any())
                {
                    foreach (var file in model.Attachments)
                    {
                        var fileAttachment = new FileAttachment
                        {
                            OdataType = "#microsoft.graph.fileAttachment",
                            Name = file.FileName,
                            ContentBytes = file.FileContent,
                            ContentType = file.ContentType
                        };

                        message.Attachments.Add(fileAttachment);
                    }
                }

                // Create the request body
                var requestBody = new SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = false
                };

                // Send the email
                if (model.Setting_Setup != null && !string.IsNullOrEmpty(model.Setting_Setup.SMTP_ACCOUNT))
                {
                    await _appClient.Users[model.Setting_Setup.SMTP_ACCOUNT].SendMail.PostAsync(requestBody);
                    return (true, "ok");
                }

                return (false, "setting setup not found");
            }
            catch (Exception ex)
            {
                // Log ex.Message if needed
                return (false, $"failed {ex.Message}");
            }
        }
        #endregion

    }
}
