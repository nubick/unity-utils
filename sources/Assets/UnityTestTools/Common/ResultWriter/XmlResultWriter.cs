using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UnityTest
{
	public class XmlResultWriter
	{
		private StringBuilder resultWriter = new StringBuilder();
		private int indend = 0;
		private string filePath;

		private const string nUnitVersion = "2.6.2-Unity";

		#region Constructors

		public XmlResultWriter(string filePath)
		{
			this.filePath = filePath;
		}

		#endregion

		public void SaveTestResult(string resultsName, ITestResult[] results)
		{
			InitializeXmlFile(resultsName, new ResultSummarizer(results));
			foreach (var result in results)
			{
				WriteResultElement(result);
			}
			TerminateXmlFile();
		}

		private void InitializeXmlFile ( string resultsName, ResultSummarizer summaryResults )
		{
			WriteHeader ();

			DateTime now = DateTime.Now;
			var attributes = new Dictionary<string, string>
				{
					{"name", "Unity Tests"},
					{"total", summaryResults.TestsRun.ToString ()},
					{"errors", summaryResults.Errors.ToString ()},
					{"failures", summaryResults.Failures.ToString ()},
					{"not-run", summaryResults.TestsNotRun.ToString ()},
					{"inconclusive", summaryResults.Inconclusive.ToString ()},
					{"ignored", summaryResults.Ignored.ToString ()},
					{"skipped", summaryResults.Skipped.ToString ()},
					{"invalid", summaryResults.NotRunnable.ToString ()},
					{"date", now.ToString("yyyy-MM-dd")},
					{"time", now.ToString("HH:mm:ss")}
				};

			WriteOpeningElement ("test-results", attributes);
			
			WriteEnvironment();
			WriteCultureInfo();
			WriteTestSuite (resultsName, summaryResults);
			WriteOpeningElement("results");
		}

		private void WriteOpeningElement (string elementName)
		{
			WriteOpeningElement (elementName, new Dictionary<string, string> ());
		}

		private void WriteOpeningElement (string elementName, Dictionary<string, string> attributes)
		{
			WriteOpeningElement (elementName, attributes, false);
		}


		private void WriteOpeningElement (string elementName, Dictionary<string, string> attributes, bool closeImmediatelly)
		{
			WriteIndend ();
			indend++;
			resultWriter.Append ("<");
			resultWriter.Append (elementName);
			foreach (var attribute in attributes)
			{
				resultWriter.AppendFormat (" {0}=\"{1}\"", attribute.Key, attribute.Value);
			}
			if (closeImmediatelly)
			{
				resultWriter.Append (" /");
				indend--;
			}
			resultWriter.AppendLine(">");
		}

		private void WriteIndend ()
		{
			for (int i = 0; i < indend; i++)
			{
				resultWriter.Append ("  ");
			}
		}

		private void WriteClosingElement ( string elementName )
		{
			indend--;
			WriteIndend ();
			resultWriter.AppendLine ("</" + elementName + ">");
		}

		private void WriteHeader ()
		{
			resultWriter.AppendLine ("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			resultWriter.AppendLine ("<!--This file represents the results of running a test suite-->");
		}

		private void WriteEnvironment ()
		{
			var attributes = new Dictionary<string, string>
				{
					{"nunit-version", nUnitVersion},
					{"clr-version", Environment.Version.ToString ()},
					{"os-version", Environment.OSVersion.ToString ()},
					{"platform", Environment.OSVersion.Platform.ToString ()},
					{"cwd", Environment.CurrentDirectory},
					{"machine-name", Environment.MachineName},
					{"user", Environment.UserName},
					{"user-domain", Environment.UserDomainName}
				};
			WriteOpeningElement ("environment", attributes, true);
		}

		private void WriteCultureInfo()
		{
			var attributes = new Dictionary<string, string>
				{
					{"current-culture", CultureInfo.CurrentCulture.ToString ()},
					{"current-uiculture", CultureInfo.CurrentUICulture.ToString ()}
				};
			WriteOpeningElement ("culture-info", attributes, true);
		}

		private void WriteTestSuite (string resultsName, ResultSummarizer summaryResults)
		{
			var attributes = new Dictionary<string, string>
				{
					{"name", resultsName},
					{"type", "Assembly"},
					{"executed", "True"},
					{"result", summaryResults.Success ? "Success" : "Failure"},
					{"success", summaryResults.Success ? "True" : "False"},
					{"time", summaryResults.Duration.ToString ("#####0.000", NumberFormatInfo.InvariantInfo)}
				};
			WriteOpeningElement ("test-suite", attributes);
		}

		private void WriteResultElement(ITestResult result)
		{
			StartTestElement(result);

			switch (result.ResultState)
			{
				case TestResultState.Ignored:
				case TestResultState.NotRunnable:
				case TestResultState.Skipped:
					WriteReasonElement(result);
					break;

				case TestResultState.Failure:
				case TestResultState.Error:
				case TestResultState.Cancelled:
					WriteFailureElement(result);
					break;
				case TestResultState.Success:
				case TestResultState.Inconclusive:
					if (result.Message != null)
						WriteReasonElement(result);
					break;
			}

			WriteClosingElement("test-case");
		}

		private void TerminateXmlFile()
		{
			WriteClosingElement ("results");
			WriteClosingElement ("test-suite");
			WriteClosingElement ("test-results");

			try
			{
				using (var fs = System.IO.File.OpenWrite (filePath))
				using (var sw = new System.IO.StreamWriter(fs, Encoding.UTF8))
				{
					sw.Write (resultWriter.ToString());
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError ("Error while opening file " + filePath);
				UnityEngine.Debug.LogException (e);
			}
		}

		#region Element Creation Helpers

		private void StartTestElement(ITestResult result)
		{
			var attributes = new Dictionary<string, string>
				{
					{"name", result.FullName}, 
					{"executed", result.Executed.ToString ()}
				};
			var resultString = "";
			switch (result.ResultState)
			{
				case TestResultState.Cancelled:
					resultString = TestResultState.Failure.ToString();
					break;
				default:
					resultString = result.ResultState.ToString ();
					break;
			}
			attributes.Add ("result", resultString);
			if (result.Executed)
			{
				attributes.Add ("success", result.IsSuccess.ToString ());
				attributes.Add( "time", result.Duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo));
			}
			WriteOpeningElement ("test-case", attributes);
		}

		private void WriteReasonElement(ITestResult result)
		{
			WriteOpeningElement ("reason");
			WriteOpeningElement ("message");
			WriteCData(result.Message);
			WriteClosingElement ("message");
			WriteClosingElement ("reason");
			
		}

		private void WriteFailureElement(ITestResult result)
		{
			WriteOpeningElement ("failure");
			WriteOpeningElement ("message");
			WriteCData (result.Message);
			WriteClosingElement ("message");
			WriteOpeningElement ("stack-trace");
			if (result.StackTrace != null)
				WriteCData (StackTraceFilter.Filter (result.StackTrace));
			WriteClosingElement ("stack-trace");
			WriteClosingElement ("failure");
		}

		#endregion

		private void WriteCData(string text)
		{
			if (text.Length == 0)
				return;
			resultWriter.AppendFormat ("<![CDATA[{0}]]>", text);
			resultWriter.AppendLine ();
		}
	}
}
