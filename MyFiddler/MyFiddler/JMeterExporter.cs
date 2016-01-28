using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Fiddler;
// [1]
[assembly: Fiddler.RequiredVersion("4.6.2.0")]

namespace FiddlerExtensions
{
    /// <summary>
    /// Uses the Fiddler GUI to obtain the filename to export and writes to the
    /// Fiddler log tab, so it cannot be used with FiddlerCore.
    /// </summary>
    // [2]
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
                // [4]
                JMeterTestPlan jMeterTestPlan = new JMeterTestPlan(oSessions, sFilename);
                System.IO.StreamWriter sw = new StreamWriter(sFilename, false, encUTF8NoBOM);
                // [5]
                sw.Write(jMeterTestPlan.Jmx);
                sw.Close();

                Fiddler.FiddlerApplication.Log.LogString("Successfully exported sessions to JMeter Test Plan");
                Fiddler.FiddlerApplication.Log.LogString(String.Format("\t{0}", sFilename));
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