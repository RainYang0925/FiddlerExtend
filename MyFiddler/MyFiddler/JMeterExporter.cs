using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Fiddler;
using MyFiddler.JMeter;

[assembly: Fiddler.RequiredVersion("4.6.2.0")]

namespace MyFiddler.FiddlerExtensions
{
    [ProfferFormat("JMeter", "JMeter .jmx Format")]
    public class JMeterExporter : ISessionExporter
    {
        public bool ExportSessions(string sFormat, Session[] oSessions, Dictionary<string, object> dictOptions,
                                    EventHandler<ProgressCallbackEventArgs> evtProgressNotifications)
        {
            bool bResult = true;
            string sFilename = null;

            // [3] Ask the Fiddler GUI to obtain the filename to export to
            sFilename = Fiddler.Utilities.ObtainSaveFilename("Export As " + sFormat, "JMeter Files (*.jmx)|*.jmx");

            if (String.IsNullOrEmpty(sFilename)) return false;

            if (!Path.HasExtension(sFilename)) sFilename = sFilename + ".jmx";

            try
            {

                Encoding encUTF8NoBOM = new UTF8Encoding(false);
                JMeterTestPlan jMeterTestPlan = new JMeterTestPlan(oSessions, sFilename);
                System.IO.StreamWriter sw = new StreamWriter(sFilename, false, encUTF8NoBOM);
                sw.Write(jMeterTestPlan.getJmx());
                sw.Close();
            }
            catch (Exception eX)
            {
                Fiddler.FiddlerApplication.Log.LogString(eX.Message);
                Fiddler.FiddlerApplication.Log.LogString(eX.StackTrace);
                bResult = false;
            }
            return bResult;
        }

        public void Dispose()
        {
        }
    }

}