using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace MailUpExample
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Код, выполняемый при запуске приложения

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Код, выполняемый при завершении работы приложения

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Код, выполняемый при возникновении необрабатываемой ошибки

        }

        void Session_Start(object sender, EventArgs e)
        {
            MailUp.MailUpClient APIClient = new MailUp.MailUpClient(ConfigurationManager.AppSettings["MailUpClientId"],
                                                                    ConfigurationManager.AppSettings["MailUpClientSecret"], 
                                                                    ConfigurationManager.AppSettings["MailUpCallbackUri"]);

            if (ConfigurationManager.AppSettings["MailUpConsoleEndpoint"] != null)
                APIClient.ConsoleEndpoint = ConfigurationManager.AppSettings["MailUpConsoleEndpoint"];

            if (ConfigurationManager.AppSettings["MailUpStatisticsEndpoint"] != null)
                APIClient.MailstatisticsEndpoint = ConfigurationManager.AppSettings["MailUpStatisticsEndpoint"];

            if(ConfigurationManager.AppSettings["MailUpLogon"] != null)
                APIClient.LogonEndpoint = ConfigurationManager.AppSettings["MailUpLogon"];

            if (ConfigurationManager.AppSettings["MailUpAuthorization"] != null)
                APIClient.AuthorizationEndpoint = ConfigurationManager.AppSettings["MailUpAuthorization"];

            if (ConfigurationManager.AppSettings["MailUpToken"] != null)
                APIClient.TokenEndpoint = ConfigurationManager.AppSettings["MailUpToken"];

            Session.Add("MailUpClient", APIClient);

        }

        void Session_End(object sender, EventArgs e)
        {
            // Код, выполняемый при запуске приложения. 
            // Примечание: Событие Session_End вызывается только в том случае, если для режима sessionstate
            // задано значение InProc в файле Web.config. Если для режима сеанса задано значение StateServer 
            // или SQLServer, событие не порождается.

        }

    }
}
