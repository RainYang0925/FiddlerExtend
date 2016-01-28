using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;

namespace FiddlerExtensions
{
    /// <summary>
    /// A simple representation of a basic JMeterTestPlan
    /// </summary>
    public class JMeterTestPlan
    {
        // [1]
        private SessionList sessionList;
        private Fiddler.Session[] sessions; // A Fiddler session represents a request/response pair

        public JMeterTestPlan()
        {
            sessions = new Fiddler.Session[0];
            sessionList = new SessionList(sessions);
        }

        public JMeterTestPlan(Fiddler.Session[] oSessions, string outputFilename)
        {
            this.sessions = oSessions;
            sessionList = new SessionList(oSessions);
        }

        // [2] Return the final formatted JMX string
        public string Jmx
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
                // [3] Linq used to prettify the XML output because it's
                // just a plain string with no formatting/indenting
                XDocument doc = XDocument.Parse(this.Xml);
                sb.Append(doc.ToString());
                return sb.ToString();
            }
        }

        // Traverse all objects and request their XML representations.
        private string Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                // based upon the version I was using at the time of development
                sb.Append("<jmeterTestPlan version=\"1.2\" properties=\"2.3\">");
                // [4]
                sb.Append(sessionList.Xml);
                sb.Append("</jmeterTestPlan>");
                return sb.ToString();
            }
        }
    }

    /// <summary>
    ///  [5] This class represents a list of HTTP Sampler nodes
    /// </summary>
    public class SessionList
    {
        private Fiddler.Session[] sessions;

        public SessionList()
        {
            sessions = new Fiddler.Session[0];
        }

        public SessionList(Fiddler.Session[] oSessions)
        {
            this.sessions = oSessions;
        }

        // Return the HTTP Sampler nodes
        public string Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (sessions.Length > 0)
                {
                    // [6]
                    sb.Append("<hashTree>");
                    foreach (Fiddler.Session session in sessions)
                    {
                        HTTPSamplerProxy httpSamplerProxy = new HTTPSamplerProxy(session);
                        sb.Append(httpSamplerProxy.Xml);
                    }
                    sb.Append("</hashTree>");
                }
                return sb.ToString();
            }
        }
    }

    /// <summary>
    /// This class represents a HTTP Sampler node
    /// </summary>
    public class HTTPSamplerProxy
    {
        Fiddler.Session session;

        public HTTPSamplerProxy(Fiddler.Session session)
        {
            this.session = session;
        }

        // [7] Return the HTTP Request node
        public string Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("<HTTPSamplerProxy guiclass=\"HttpTestSampleGui\" "
                          + "testclass=\"HTTPSamplerProxy\" testname=\"{0}\" enabled=\"true\">"
                          , Path));
                sb.Append("<boolProp name=\"HTTPSampler.postBodyRaw\">true</boolProp>");
                sb.Append("<elementProp name=\"HTTPsampler.Arguments\" elementType=\"Arguments\">");
                sb.Append("<collectionProp name=\"Arguments.arguments\">");
                sb.Append("<elementProp name=\"\" elementType=\"HTTPArgument\">");
                sb.Append("<boolProp name=\"HTTPArgument.always_encode\">false</boolProp>");
                sb.Append(String.Format("<stringProp name=\"Argument.value\">{0}</stringProp>", RequestBody));
                sb.Append("<stringProp name=\"Argument.metadata\">=</stringProp>");
                sb.Append("</elementProp>");
                sb.Append("</collectionProp>");
                sb.Append("</elementProp>");
                sb.Append(String.Format("<stringProp name=\"HTTPSampler.domain\">{0}</stringProp>", session.host));
                sb.Append(String.Format("<stringProp name=\"HTTPSampler.port\">{0}</stringProp>", Port));
                sb.Append("<stringProp name=\"HTTPSampler.connect_timeout\"></stringProp>");
                sb.Append("<stringProp name=\"HTTPSampler.response_timeout\"></stringProp>");
                sb.Append(String.Format("<stringProp name=\"HTTPSampler.protocol\">{0}</stringProp>"
                          , session.oRequest.headers.UriScheme));
                sb.Append("<stringProp name=\"HTTPSampler.contentEncoding\"></stringProp>");
                sb.Append(String.Format("<stringProp name=\"HTTPSampler.path\">{0}</stringProp>", Path));
                sb.Append(String.Format("<stringProp name=\"HTTPSampler.method\">{0}</stringProp>"
                          , session.oRequest.headers.HTTPMethod.ToUpper()));
                sb.Append("<boolProp name=\"HTTPSampler.follow_redirects\">true</boolProp>");
                sb.Append("<boolProp name=\"HTTPSampler.auto_redirects\">false</boolProp>");
                sb.Append("<boolProp name=\"HTTPSampler.use_keepalive\">true</boolProp>");
                sb.Append("<boolProp name=\"HTTPSampler.DO_MULTIPART_POST\">false</boolProp>");
                sb.Append("<boolProp name=\"HTTPSampler.monitor\">false</boolProp>");
                sb.Append("<stringProp name=\"HTTPSampler.embedded_url_re\"></stringProp>");
                sb.Append("</HTTPSamplerProxy>");                
                return sb.ToString();
            }
        }

        private string Path
        {
            get
            {
                return System.Net.WebUtility.HtmlEncode(session.PathAndQuery);
            }
        }

        private string getPort()
        {
            int port = session.port;
            string protocol = session.oRequest.headers.UriScheme; ;
            if (protocol.ToLower() == ("https") && port == 443)
            {
                return "";
            }
            if (protocol.ToLower() == ("http") && port == 80)
            {
                return "";
            }
            return port.ToString();
        }

        private string Port
        {
            get
            {
                return getPort();
            }
        }

        private string RequestBody
        {
            get
            {
                return System.Net.WebUtility.HtmlEncode(session.GetRequestBodyAsString());
            }
        }

    }
}