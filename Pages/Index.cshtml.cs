using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IEC61850.Client;
using IEC61850.Common;
using System.Threading;

namespace T7_bcu.Pages
{
    public class IndexModel : PageModel
    {
        //Testing 
      private static void reportHandler (Report report, object parameter)
		{
            IndexModel viewdata = new IndexModel();
			
			MmsValue values = report.GetDataSetValues ();
           
			for (int i = 0; i < values.Size(); i++) {
				if (report.GetReasonForInclusion(i) != ReasonForInclusion.REASON_NOT_INCLUDED) {
					if (i == 0) viewdata.ViewData["VphaseA"] = values.GetElement(i);
                    if (i == 1) viewdata.ViewData["VphaseB"] = values.GetElement(i);
                    if (i == 2) viewdata.ViewData["VphaseC"] = values.GetElement(i);
                    if (i == 3) viewdata.ViewData["VphaseAB"] = values.GetElement(i);
                    if (i == 4) viewdata.ViewData["VphaseBC"] = values.GetElement(i);
                    if (i == 5) viewdata.ViewData["VphaseCA"] = values.GetElement(i);
                    if (i == 7) viewdata.ViewData["AphaseA"] = values.GetElement(i);
                    if (i == 8) viewdata.ViewData["AphaseB"] = values.GetElement(i);
                    if (i == 9) viewdata.ViewData["AphaseC"] = values.GetElement(i);
                    if (i == 15) viewdata.ViewData["Power"] = values.GetElement(i);
                    if (i == 16) viewdata.ViewData["Vars"] = values.GetElement(i);
                    if (i == 17) viewdata.ViewData["Truepower"] = values.GetElement(i);
                    if (i == 18) viewdata.ViewData["Pf"] = values.GetElement(i);
                    if (i == 19) viewdata.ViewData["Freq"] = values.GetElement(i);


                }
            }
            		           
		}


		private static bool running = true;


      public static void OnGet()
      {
      
        IedConnection con = new IedConnection ();

			string hostname;

			hostname = "100.100.100.100";

			Console.WriteLine ("Connect to " + hostname);

			try {
				con.Connect (hostname, 102);

				string rcbReference3 = "TEMPLATEMEAS/LLN0.RP.urcbAinA01";
                // string rcbReference3 = "TEMPLATECTRL/LLN0.BR.brcbDinD01";
				ReportControlBlock rcb3 = con.GetReportControlBlock(rcbReference3);

               rcb3.GetRCBValues();

				if (rcb3.IsBuffered())
					Console.WriteLine ("RCB: " + rcbReference3 + " is buffered");

				rcb3.InstallReportHandler(reportHandler, rcb3);

				rcb3.SetOptFlds(ReportOptions.REASON_FOR_INCLUSION | ReportOptions.SEQ_NUM | ReportOptions.TIME_STAMP |
				                ReportOptions.CONF_REV | ReportOptions.ENTRY_ID | ReportOptions.DATA_REFERENCE | ReportOptions.DATA_SET);
				rcb3.SetTrgOps(TriggerOptions.DATA_CHANGED | TriggerOptions.INTEGRITY);				
				rcb3.SetIntgPd(2000);
				rcb3.SetRptEna(true);

				rcb3.SetRCBValues();


				/* run until Ctrl-C is pressed */
				Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
					e.Cancel = true;
					IndexModel.running = false;
				};

				/* stop main loop when connection is lost */
				con.InstallConnectionClosedHandler(delegate(IedConnection connection) {
					Console.WriteLine("Connection closed");
					IndexModel.running = false;
				});

				while (running) {
					Thread.Sleep(10);
				}

				/* Dispose the RCBs when you no longer need them */
			    rcb3.Dispose();

				con.Abort ();

				con.Dispose();
			} catch (IedConnectionException e) {
				Console.WriteLine ("Error: " + e.Message);

				con.Dispose ();
			}

		}

        public static void Main(String [] args)
        {
            OnGet();
        }
    }
}
