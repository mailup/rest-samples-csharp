using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailUpExample.Entity
{
    public class EmailTrackingInfoDTO
    {
        public Boolean Enabled { get; set; }
        public List<String> Protocols { get; set; }
        public String CustomParams { get; set; }

        public String ProtocolsToString()
        {
            String ret = "";
            for (Int32 p = 0; p < Protocols.Count; p++)
            {
                String curProtocol = Protocols[p];

                ret += "" + curProtocol.Replace(":", "") + ":";
                if (p < (Protocols.Count - 1))
                {
                    ret += "|";
                }
            }

            return ret;
        }
        public String CustomParamsToString()
        {
            String ret = "";
            if (!String.IsNullOrEmpty(CustomParams))
            {
                if (CustomParams.StartsWith("?"))
                    ret = CustomParams.Substring(1);
                else
                    ret = CustomParams;
            }
            return ret;
        }
    }
}