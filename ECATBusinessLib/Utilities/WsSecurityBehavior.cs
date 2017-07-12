using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ecat.Business.Utilities
{
    public class WsSecurityBehavior: IEndpointBehavior
    {
        public MessageInspector MessageInspector { get; set; }

        public WsSecurityBehavior(MessageInspector messageInspector)
        {
            MessageInspector = messageInspector;
        }

        public void Validate(ServiceEndpoint endpoint) { }
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (MessageInspector == null)
            {
                throw new InvalidOperationException("Caller must supply ClientInpsector.");
            }
            clientRuntime.ClientMessageInspectors.Add(MessageInspector);
        }
    }

    public class MessageInspector: IClientMessageInspector
    {
        public MessageHeader[] Headers { get; set; }

        public MessageInspector(params MessageHeader[] headers)
        {
            Headers = headers;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (Headers == null) { return request; }
            for (var i = Headers.Length - 1; i >= 0; i--)
            {
                request.Headers.Insert(0, Headers[i]);
            }
            return request;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }
    }

    public class SecurityHeader: MessageHeader
    {
        public string SystemUser { get; set; }
        public string SystemPassword { get; set; }
        
        public SecurityHeader(string systemUser, string systemPassword)
        {
            SystemUser = systemUser;
            SystemPassword = systemPassword;
        }

        public override string Name => "Security";
        public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", Name, Namespace);
            writer.WriteXmlnsAttribute("wsse", Namespace);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            WriteHeader(writer);
        }

        private void WriteHeader(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("wsu", "Timestamp", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            writer.WriteAttributeString("wsu", "id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", "Timestamp-6557466");

            writer.WriteStartElement("wsu", "Created", XmlConvert.ToString(DateTime.Now, "yyyy-MM-ddTHH:mm:sszzzzzz"));
            writer.WriteEndElement();   //End Created

            writer.WriteStartElement("wsu", "Expires", XmlConvert.ToString(DateTime.Now.AddDays(1), "yyyy-MM-ddTHH:mm:sszzzzzz"));
            writer.WriteEndElement();   //End Expires

            writer.WriteEndElement();   //End Timestamp

            writer.WriteStartElement("wsse", "UsernameToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            writer.WriteXmlnsAttribute("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");

            writer.WriteStartElement("wsse", "Username", null);
            writer.WriteString(SystemUser);
            writer.WriteEndElement();//End Username 

            writer.WriteStartElement("wsse", "Password", null);
            writer.WriteAttributeString("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText");
            writer.WriteString(SystemPassword);
            writer.WriteEndElement();//End Password 

            writer.WriteEndElement();//End UsernameToken
            writer.Flush();

        }
    }
}
