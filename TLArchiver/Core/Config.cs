﻿using System;
using System.Collections.Generic;
using System.Configuration;

namespace TLArchiver.Core
{
    // https://my.telegram.org to obtain your own ApiId and ApiHash

    public class Config
    {
        // Data from app.config
        public string HttpProxyHost { get; set; }
        public int HttpProxyPort { get; set; }
        public string ProxyUserName { get; set; }
        public string ProxyPassword { get; set; }
        public string ApiHash { get; set; }
        public int ApiId { get; set; }
        public string SignInPhoneNumberCode { get; set; }
        public string SignInPhoneNumber { get; set; }
        public string NumberToAuthenticate
        {
            get
            {
                return SignInPhoneNumberCode + SignInPhoneNumber.TrimStart('0');
            }
        }
        public string CodeToAuthenticate { get; set; }
        public string NotRegisteredNumberToSignUp { get; set; }
        public string NumberToSendMessage { get; set; }
        public string UserNameToSendMessage { get; set; }
        public string NumberToGetUserFull { get; set; }
        public string NumberToAddToChat { get; set; }
        public string ExportDirectory { get; set; }
        public int MessagesReadLimit { get; set; }
        public bool CountMessagesAtLaunch { get; set; }

        // Data from the app
        public TLAArchiver Archiver { get; set; }

        public bool IsFromDate { get; set; }
        public bool IsToDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public bool ExportMessages { get; set; }
        public bool ExportFiles { get; set; }
        public bool ExportPhotos { get; set; }
        public bool ExportVideos { get; set; }
        public bool ExportVoiceMessages { get; set; }

        public bool ExportText { get; set; }
        public bool ExportHtml { get; set; }

        public Dictionary<int, string> Contacts { get; set; }

        static public void Load(Config config)
        {
            config.HttpProxyHost = ConfigurationManager.AppSettings["HttpProxyHost"];
            config.HttpProxyPort = Int32.Parse(ConfigurationManager.AppSettings["HttpProxyPort"]);
            config.ProxyUserName = ConfigurationManager.AppSettings["ProxyUserName"];
            config.ProxyPassword = ConfigurationManager.AppSettings["ProxyPassword"];
            config.ApiHash = ConfigurationManager.AppSettings["ApiHash"];
            config.ApiId = Int32.Parse(ConfigurationManager.AppSettings["ApiId"]);
            //config.NumberToAuthenticate = ConfigurationManager.AppSettings["NumberToAuthenticate"];
            config.SignInPhoneNumberCode = ConfigurationManager.AppSettings["SignInPhoneNumberCode"];
            config.SignInPhoneNumber = ConfigurationManager.AppSettings["SignInPhoneNumber"];
            config.CodeToAuthenticate = ConfigurationManager.AppSettings["CodeToAuthenticate"];
            config.NotRegisteredNumberToSignUp = ConfigurationManager.AppSettings["NotRegisteredNumberToSignUp"];
            config.NumberToSendMessage = ConfigurationManager.AppSettings["NumberToSendMessage"];
            config.UserNameToSendMessage = ConfigurationManager.AppSettings["UserNameToSendMessage"];
            config.NumberToGetUserFull = ConfigurationManager.AppSettings["NumberToGetUserFull"];
            config.NumberToAddToChat = ConfigurationManager.AppSettings["NumberToAddToChat"];
            config.ExportDirectory = ConfigurationManager.AppSettings["ExportDirectory"];
            config.MessagesReadLimit = Int32.Parse(ConfigurationManager.AppSettings["MessagesReadLimit"]);
            config.CountMessagesAtLaunch = Boolean.Parse(ConfigurationManager.AppSettings["CountMessagesAtLaunch"]);
        }
    }
}
