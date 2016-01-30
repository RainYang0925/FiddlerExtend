using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;

namespace MyFiddler.JMeter
{
    public class HTTPSamplerProxy
    {
        Fiddler.Session session;

        private string EncodeXml(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml))
                return "";

            strHtml = strHtml.Replace("&", "&amp;");
            strHtml = strHtml.Replace("<", "&lt;");
            strHtml = strHtml.Replace(">", "&gt;");
            strHtml = strHtml.Replace("'", "&apos;");
            strHtml = strHtml.Replace("\"", "&quot;");
            return strHtml;

        }

        public HTTPSamplerProxy(Fiddler.Session session)
        {
            this.session = session;
        }
                
        public string Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                string method = session.oRequest.headers.HTTPMethod.ToUpper();

                string pathAndQuery = session.PathAndQuery;

                sb.AppendFormat("<HTTPSamplerProxy guiclass=\"HttpTestSampleGui\" " + "testclass=\"HTTPSamplerProxy\" testname=\"{0}\" enabled=\"true\">", Path);

                if (pathAndQuery.IndexOf("?") >= 0 && method == "GET")
                {
                    sb.Append("<elementProp name=\"HTTPsampler.Arguments\" elementType=\"Arguments\" guiclass=\"HTTPArgumentsPanel\" testclass=\"Arguments\" testname=\"User Defined Variables\" enabled=\"true\">");
                    sb.Append("<collectionProp name=\"Arguments.arguments\">");
                    string query = pathAndQuery.Substring(pathAndQuery.LastIndexOf('?') + 1);
                    string[] paramArray = query.Split('&');
                    foreach (string pa in paramArray)
                    {
                        var paArray = pa.Split('=');
                        if (paArray.Length < 2)
                            continue;
                        sb.AppendFormat("<elementProp name=\"{0}\" elementType=\"HTTPArgument\">", paArray[0]);
                        sb.Append("<boolProp name=\"HTTPArgument.always_encode\">false</boolProp>");
                        sb.AppendFormat("<stringProp name=\"Argument.value\">{0}</stringProp>", paArray[1]);
                        sb.Append("<stringProp name=\"Argument.metadata\">=</stringProp>");
                        sb.AppendFormat("<stringProp name=\"Argument.name\">{0}</stringProp>", paArray[0]);
                        sb.Append("</elementProp>");
                    }
                    sb.Append("</collectionProp>");
                    sb.Append("</elementProp>");
                }
                else
                {
                    sb.Append("<boolProp name=\"HTTPSampler.postBodyRaw\">true</boolProp>");
                    sb.Append("<elementProp name=\"HTTPsampler.Arguments\" elementType=\"Arguments\">");
                    sb.Append("<collectionProp name=\"Arguments.arguments\">");
                    sb.Append("<elementProp name=\"\" elementType=\"HTTPArgument\">");
                    sb.Append("<boolProp name=\"HTTPArgument.always_encode\">false</boolProp>");
                    sb.AppendFormat("<stringProp name=\"Argument.value\">{0}</stringProp>", RequestBody);
                    sb.Append("<stringProp name=\"Argument.metadata\">=</stringProp>");
                    sb.Append("</elementProp>");
                    sb.Append("</collectionProp>");
                    sb.Append("</elementProp>");
                }


                sb.AppendFormat("<stringProp name=\"HTTPSampler.domain\">{0}</stringProp>", session.hostname);
                sb.AppendFormat("<stringProp name=\"HTTPSampler.port\">{0}</stringProp>", Port);
                sb.Append("<stringProp name=\"HTTPSampler.connect_timeout\"></stringProp>");
                sb.Append("<stringProp name=\"HTTPSampler.response_timeout\"></stringProp>");
                sb.AppendFormat("<stringProp name=\"HTTPSampler.protocol\">{0}</stringProp>", session.oRequest.headers.UriScheme);
                sb.Append("<stringProp name=\"HTTPSampler.contentEncoding\"></stringProp>");

                if (method == "GET")
                {
                    sb.AppendFormat("<stringProp name=\"HTTPSampler.path\">{0}</stringProp>", Path);
                }
                else
                {
                    sb.AppendFormat("<stringProp name=\"HTTPSampler.path\">{0}</stringProp>", this.EncodeXml(session.PathAndQuery));
                }



                sb.AppendFormat("<stringProp name=\"HTTPSampler.method\">{0}</stringProp>", method);

                sb.Append("<boolProp name=\"HTTPSampler.follow_redirects\">false</boolProp>");
                sb.Append("<boolProp name=\"HTTPSampler.auto_redirects\">true</boolProp>");
                sb.Append("<boolProp name=\"HTTPSampler.use_keepalive\">true</boolProp>");
                sb.Append("<boolProp name=\"HTTPSampler.DO_MULTIPART_POST\">false</boolProp>");
                sb.Append("<stringProp name=\"HTTPSampler.implementation\">Java</stringProp>");
                sb.Append("<boolProp name=\"HTTPSampler.monitor\">false</boolProp>");
                sb.Append("<stringProp name=\"HTTPSampler.embedded_url_re\"></stringProp>");
                sb.Append("</HTTPSamplerProxy>");
                sb.Append("<hashTree/>");
                return sb.ToString();
            }
        }

        private string Path
        {
            get
            {
                string pathAndQuery = session.PathAndQuery;
                if (pathAndQuery.IndexOf("?") >= 0)
                {
                    pathAndQuery = pathAndQuery.Split('?')[0];
                }
                return System.Net.WebUtility.HtmlEncode(pathAndQuery);
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
