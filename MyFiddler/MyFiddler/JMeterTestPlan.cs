using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Configuration;
using System.Xml;
using MyFiddler.JMeter;
using MyFiddler;
using Fiddler;

namespace MyFiddler.JMeter
{
    /// <summary>
    /// A simple representation of a basic JMeterTestPlan
    /// </summary>
    public class JMeterTestPlan
    {
        // [1]
        private SessionList sessionList;
        private Fiddler.Session[] sessions;

        public JMeterTestPlan(Fiddler.Session[] oSessions, string outputFilename)
        {
            this.sessions = oSessions;
            sessionList = new SessionList(oSessions);
        }


        public string getJmx()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");            
            sb.Append("<jmeterTestPlan version=\"1.2\" properties=\"2.8\" jmeter=\"2.13 r1665067\">");
            sb.Append("<hashTree>");
            sb.Append(sessionList.Xml);
            sb.Append("</hashTree>");
            sb.Append("</jmeterTestPlan>");
            return sb.ToString();
        }
    }  
}