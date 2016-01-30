using MyFiddler.JMeter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Fiddler;

namespace MyFiddler
{
    public class SessionList
    {
        private Fiddler.Session[] sessions;               

        public SessionList(Fiddler.Session[] oSessions)
        {
            this.sessions = oSessions;
        }
                
        public string Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (sessions.Length > 0)
                {
                    sb.Append("<LoopController guiclass=\"LoopControlPanel\" testclass=\"LoopController\" testname=\"06selectDeptToApprove\" enabled=\"true\">");
                    sb.Append("<boolProp name=\"LoopController.continue_forever\">false</boolProp>");
                    sb.Append("<stringProp name=\"LoopController.loops\">1</stringProp>");
                    sb.Append("</LoopController>");
                    sb.Append("<hashTree>");                  

                    string configHost = getConfig("host");

                    foreach (Fiddler.Session session in sessions)
                    {
                        if (!String.IsNullOrEmpty(configHost)) {
                            if (session.host.IndexOf(configHost) >= 0 && (session.responseCode == 200 || session.responseCode == 304))
                            {
                                HTTPSamplerProxy httpSamplerProxy = new HTTPSamplerProxy(session);
                                sb.Append(httpSamplerProxy.Xml);
                            }
                            else {
                                if (session.responseCode == 200 || session.responseCode == 304) {
                                    HTTPSamplerProxy httpSamplerProxy = new HTTPSamplerProxy(session);
                                    sb.Append(httpSamplerProxy.Xml);
                                }
                            }
                        }
                        
                    }
                    sb.Append("</hashTree>");
                }
                return sb.ToString();
            }
        }

        private string getConfig(string key)
        {
            string dic = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ImportExport");
            string path = dic + "/app.config";
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                var node = doc.SelectSingleNode("configuration/appSettings/add[@key='" + key + "']");
                return node.Attributes["value"].Value.ToString();
            }
            else {
                return "";
            }
        }
    }  
}
