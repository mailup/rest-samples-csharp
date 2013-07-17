using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailUpExample.Entity
{
    public class EmailMessageItemDTO
    {
        public Int32 idList { get; set; }
		public Int32 idNL { get; set; }
		public String Subject { get; set; }
		public String Notes { get; set; }
		public String Content { get; set; }
		public List<EmailDynamicFieldDTO> Fields { get; set; }
		public List<EmailTagDTO> Tags { get; set; }
		public Boolean Embed { get; set; }
		public Boolean IsConfirmation { get; set; }
		public EmailTrackingInfoDTO TrackingInfo = new EmailTrackingInfoDTO();
		public String Head { get; set; }
		private String _body;
		public String Body
		{
			get { return _body; }
			set { _body = value; }
		}
		public EmailMessageItemDTO()
		{
			_body = "<body>";
		}
    }
}