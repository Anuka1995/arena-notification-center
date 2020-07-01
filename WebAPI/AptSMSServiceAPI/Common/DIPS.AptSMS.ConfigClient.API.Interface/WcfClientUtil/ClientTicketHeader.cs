using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class ClientTicketHeader : MessageHeader
    {
        public Guid Ticket { get; private set; }

        public ClientTicketHeader(Guid ticket)
        {
            Ticket = ticket;
        }

        public override string Name
        {
            get
            {
                return "TicketHeader";
            }
        }

        public override string Namespace
        {
            get
            {
                return "DIPS.Authentication";
            }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var serializer = new XmlSerializer(typeof(Guid));
            var textWriter = new StringWriter();
            serializer.Serialize(textWriter, Ticket);
            textWriter.Close();

            var text = textWriter.ToString();

            writer.WriteElementString("Ticket", "Key", text.Trim());
        }
    }
}
