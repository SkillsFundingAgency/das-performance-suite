using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.WebTesting;
using Microsoft.VisualStudio.QualityTools;
using Microsoft.VisualStudio.TestTools;
using System.Data.SqlClient;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System.Linq;
using Settings;

namespace PlugIns
{
    [Description("This plugin can be used to set the ParseDependentsRequests property for each request")]
    public class UserIteration : WebTestPlugin
    {
        private bool m_parseDependents = true;
        private Configuration config;

        public UserIteration()
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = Assembly.GetExecutingAssembly().ManifestModule.Name + ".config";

            // Get the mapped configuration file
            config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        [System.ComponentModel.DisplayName("Data file to use.")]
        public string DataFile { get; set; }

        public void DBReset(string payRef)
        {
            string dbConnetionString = "Data Source=" + Settings.PerformanceTest.Default.DataSource + ";Initial Catalog=" + Settings.PerformanceTest.Default.InitialCatalog + ";User ID=" + Settings.PerformanceTest.Default.UserID + ";Password=" + Settings.PerformanceTest.Default.Password;
            
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            connection = new SqlConnection(dbConnetionString);

            string strSQL = "UPDATE employer_account.accounthistory SET removedDate = GetDATE() WHERE payeref = '" + payRef + "' and removeddate is null";

            try
            {
                connection.Open();
                adapter.InsertCommand = new SqlCommand(strSQL, connection);
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            adapter.Dispose();
        }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            object contextParameterObject;
            
            if (e.WebTest.Context.TryGetValue(DataFile,
                           out contextParameterObject))
            {
                string PayRef = contextParameterObject.ToString();
                
                DBReset(PayRef);
            }
            else
            {
                throw new WebTestException(DataFile + "'DataFile' not found");
            }            
        }

        public override void PostWebTest(object sender, PostWebTestEventArgs e)
        {
            // TODO: Add code to execute after the test.  
        }

        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            // Code to execute before each request.  
            // Set the ParseDependentsRequests value on the request  
            e.Request.ParseDependentRequests = m_parseDependents;
        }

        // Properties for the plugin.  
        [DefaultValue(true)]
        [Description("All requests will have their ParseDependentsRequests property set to this value")]
        public bool ParseDependents
        {
            get
            {
                return m_parseDependents;
            }
            set
            {
                m_parseDependents = value;
            }
        } 
    }

    public class MyWebRequestPlugin : WebTestRequestPlugin
    {
        public override void PostRequest(object sender, PostRequestEventArgs e)
        {
            Console.WriteLine(e.WebTest.Context.AgentName);
        }
        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            Console.WriteLine(e.Request.Url);
        }

        public void WebTestUserId(object sender, PreRequestEventArgs e)
        {
            Console.WriteLine(e.WebTest.Context.WebTestUserId);
        }

        public void AgentId(object sender, PreRequestEventArgs e)
        {
            Console.WriteLine(e.WebTest.Context.AgentId);
        }
    }

    [System.ComponentModel.Description("WebTestPlugin, Overrides PreWebTest and appends either a context parameter or hard coded string to a specified target Context Parameter. As an example, this plugin could append the built in $WebTestIteration value to an existing datasource column value.")]
    public class AppendValueToContextParameter : WebTestPlugin
    {
        [System.ComponentModel.DefaultValue("DataSource1.SomeDataFile#csv.FieldName")]
        [System.ComponentModel.Description("The target context parameter that you would like to append the source context param value")]
        public string ContextParamTarget { get; set; }


        [System.ComponentModel.DefaultValue("$WebTestIteration")]
        [System.ComponentModel.Description("The source context parameter, or a hard coded string value you would like appended to the target context param")]
        public string ContextParamOrValueSource { get; set; }

        public AppendValueToContextParameter()
        {
            string dateTime = string.Empty;
            dateTime = System.DateTime.Now.ToString("ddMM") + System.DateTime.Now.ToString("HHmmSS");
        }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            //confirm the target context param exists 
            if (e.WebTest.Context.ContainsKey(ContextParamTarget))
            {
                //is the ContextParmOrValueSource a Context Parameter or a hard coded value?
                if (e.WebTest.Context.ContainsKey(ContextParamOrValueSource))
                {
                    e.WebTest.Context[ContextParamTarget] += e.WebTest.Context[ContextParamOrValueSource].ToString();
                }
                else
                {
                    e.WebTest.Context[ContextParamTarget] += ContextParamOrValueSource;
                }
            }
            else
            {
                throw new ArgumentException("Required Context Parameter Target was not found");
            }
        }
    }

    [System.ComponentModel.DisplayName("Unique Reference Number")]
    [System.ComponentModel.Description("Generates a reference number")]
    public class UniqueReferenceNumberPlugin : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            Random randomGenerator = new Random();
            string ULRN = string.Empty;
            string availableCharacters = "123456789";
            //ULRN = System.DateTime.Now.ToString("ddMM") + System.DateTime.Now.ToString("HHmm");

            for (int characterIndex = 0; characterIndex <= 9; characterIndex++)
            {
                ULRN = ULRN + availableCharacters[randomGenerator.Next(availableCharacters.Length)].ToString();
            }

            e.WebTest.Context[ContextParamTarget] = ULRN;
            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("Unique Checksum Number")]
    [System.ComponentModel.Description("Generates a checksum number")]
    public class UniqueLearnerNumber : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated checksum value.")]
        public string ContextParamTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            String ULRN = GenerateRandomNumberBetweenTwoValues(10, 99).ToString() + DateTime.Now.ToString("ssffffff");

            for (int i = 1; i < 30; i++)
            {
                if (IsValidCheckSum(ULRN))
                {
                    break;
                }
                else
                {
                    ULRN = (long.Parse(ULRN) + 1).ToString();
                }
            }
            e.WebTest.Context[ContextParamTarget] = ULRN;
            base.PreWebTest(sender, e);
        }

        private static bool IsValidCheckSum(string uln)
        {
            var ulnCheckArray = uln.ToCharArray()
                                .Select(c => long.Parse(c.ToString()))
                                    .ToList();

            var multiplier = 10;
            long checkSumValue = 0;
            for (var i = 0; i < 10; i++)
            {
                checkSumValue += ulnCheckArray[i] * multiplier;
                multiplier--;
            }

            return checkSumValue % 11 == 10;
        }

        public static int GenerateRandomNumberBetweenTwoValues(int min, int max)
        {
            Random rand = new Random();
            return rand.Next(min, max);
        }
    }

    [System.ComponentModel.DisplayName("Generate Unique First Name")]
    [System.ComponentModel.Description("Generates a unique first name with every iteration")]
    public class UniqueFirstName : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]

        public string ContextParamTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            // Generate new guid with specified output format
            string firstName = string.Empty;
            firstName = "First_" + System.DateTime.Now.ToString("ddMM") + System.DateTime.Now.ToString("HHmmss") + "_" + e.WebTest.Context.WebTestUserId.ToString() + "_" + e.WebTest.Context.WebTestIteration.ToString();

            // Set the context paramaeter with generated lastname
            e.WebTest.Context[ContextParamTarget] = firstName;
            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("Generate Unique Last Name")]
    [System.ComponentModel.Description("Generates a unique last name with every iteration")]
    public class UniqueLastName : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            // Generate new guid with specified output format
            string lastName = string.Empty;

            lastName = "Last_" + System.DateTime.Now.ToString("ddMM") + System.DateTime.Now.ToString("HHmmss") + "_" + e.WebTest.Context.WebTestUserId.ToString() + "_" + e.WebTest.Context.WebTestIteration.ToString();

            // Set the context paramaeter with generated lastname
            e.WebTest.Context[ContextParamTarget] = lastName;

            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("Generate Unique Email")]
    [System.ComponentModel.Description("Generates a unique for the new user")]
    public class UniqueEmail : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            // Generate new guid with specified output format
            string eMail = string.Empty;

            eMail = "First_" + System.DateTime.Now.ToString("ddMM") + System.DateTime.Now.ToString("HHmmss") + "_" + e.WebTest.Context.WebTestUserId.ToString() + "_" + e.WebTest.Context.WebTestIteration.ToString()
            + "@mailinator.com";

            // Set the context paramaeter with generated lastname
            e.WebTest.Context[ContextParamTarget] = eMail;

            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("TLS PlugIn")]
    [System.ComponentModel.Description("TLS PlugIn to resolve the forced closure of connection")]
    //[Description("This plugin will force the underlying System.Net ServicePointManager to negotiate downlevel SSLv3 instead of TLS. WARNING: The servers X509 Certificate will be ignored as part of this process, so verify that you are testing the correct system.")]
    public class TLSPlugin : WebTestPlugin
    {
        [Description("Enable or Disable the plugin functionality")]
        [DefaultValue(true)]
        public bool Enabled { get; set; }
        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            base.PreWebTest(sender, e);

            //For TLS
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //For SSL
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            //we wire up the callback so we can override  behavior and force it to accept the cert
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCB;

            //let them know we made changes to the service point manager
            e.WebTest.AddCommentToResult(this.ToString() + " has made the following modification-> ServicePointManager.SecurityProtocol set to use SSLv3 in WebTest Plugin.");
        }
        public static bool RemoteCertificateValidationCB(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //If it is really important, validate the certificate issuer here.
            //this will accept any certificate
            return true;
        }
    }

    [System.ComponentModel.DisplayName("Test Env")]
    [System.ComponentModel.Description("Testing Environment")]
    public class SetEnvironment : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            e.WebTest.Context[ContextParamTarget] = Settings.PerformanceTest.Default.TestEnvironment;
            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("Test Environment")]
    [System.ComponentModel.Description("Environment to execute performance test against")]
    public class SetTestEnvironment : WebTestPlugin
    {
        //[System.ComponentModel.DisplayName("Target Context Parameter Name")]
        //[System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }
        
        public string ContextParamAccountTarget { get; set; }

        public string ContextParamApprenticesTarget { get; set; }

        public string ContextParamFinanceTarget { get; set; }

        public string ContextParamForecastingTarget { get; set; }

        public string ContextParamPermissionsTarget { get; set; }

        public string ContextParamRecruitTarget { get; set; }

        public string ContextParamLoginTarget { get; set; }

        public string ContextParamReportingTarget { get; set; }

        public string ContextParamPASTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
                e.WebTest.Context[ContextParamAccountTarget] = Settings.AccountsURL.AccountTarget;
                e.WebTest.Context[ContextParamApprenticesTarget] = Settings.ApprenticesURL.ApprenticesTarget;
                e.WebTest.Context[ContextParamFinanceTarget] = Settings.FinanceURL.FinanceTarget;
                e.WebTest.Context[ContextParamForecastingTarget] = Settings.ForecastingURL.ForecastingTarget;
                e.WebTest.Context[ContextParamPermissionsTarget] = Settings.PermissionsURL.PermissionTarget;
                e.WebTest.Context[ContextParamRecruitTarget] = Settings.RecruitURL.RecruitTarget;
                e.WebTest.Context[ContextParamTarget] = Settings.LandingURL.LandingTarget;
                e.WebTest.Context[ContextParamLoginTarget] = Settings.LoginURL.LoginTarget;
                e.WebTest.Context[ContextParamReportingTarget] = Settings.ReportingURL.ReportingTarget;
                e.WebTest.Context[ContextParamPASTarget] = Settings.PASURL.PASTarget;

            if (PerformanceTest.Default.TestEnvironment == "test2")
            {
                e.WebTest.Context[ContextParamApprenticesTarget] = Settings.ApprenticesURLT2.ApprenticesTarget;
                e.WebTest.Context[ContextParamFinanceTarget] = Settings.FinanceURLT2.FinanceTarget;
                e.WebTest.Context[ContextParamForecastingTarget] = Settings.ForecastingURLT2.ForecastingTarget;
            }
            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("Conditional Request")]
    [System.ComponentModel.Description("Submit a request at a desired iteration")]
    public class ConditionalRequest : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        [System.ComponentModel.DisplayName("Set Iteration Number")]
        [System.ComponentModel.Description("Set the iteration count to do something.")]
        public int Iter { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            string setTrigger = Boolean.FalseString;

            if ((e.WebTest.Context.WebTestIteration) % Iter == 0)
            {
                setTrigger = Boolean.TrueString;
            }

            e.WebTest.Context[ContextParamTarget] = setTrigger;

            //e.WebTest.AddCommentToResult(this.ToString() + " Iteration set to: " + Iter);
            //e.WebTest.AddCommentToResult(this.ToString() + " Result: " + (e.WebTest.Context.WebTestIteration) % Iter);
            //e.WebTest.AddCommentToResult(this.ToString() + " Trigger: " + e.WebTest.Context[ContextParamTarget]);
            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("Thinktime")]
    [System.ComponentModel.Description("Set thinktime in seconds")]
    public class SetThinkTime : WebTestPlugin
    {
        string rand = Boolean.FalseString;

        [Description("Enter the think time to be set for all the Requests")]
        public string ThinkTime { get; set; }

        [Description("Random think time")]
        public bool Random { get; set; }

        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            if (Random == true)
            {
                Random rand = new Random();
                e.Request.ThinkTime = rand.Next(1, Convert.ToInt32(ThinkTime) + 1);
            }
            else
            {
                e.Request.ThinkTime = Convert.ToInt32(ThinkTime);
            }
            //e.WebTest.AddCommentToResult(this.ToString() + e.Request.ThinkTime);
        }
    }

    public class ConvertToStringValue : WebTestPlugin
    {
        //PathwayName,FworkCode,ProgType
        string FWId = string.Empty;

        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        object contextParameterObject;
        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            if (e.WebTest.Context.TryGetValue("Apprenticeships.Apprenticeships#csv.PathwayName",
                            out contextParameterObject))
            {
                //e.WebTest.Context[ContextParamTarget] = contextParameterObject.ToString();
                FWId = FWId + contextParameterObject.ToString();
                //e.WebTest.AddCommentToResult(contextParameterObject.ToString());
            }
            else
            {
                throw new WebTestException("'Apprenticeships.Apprenticeships#csv.PathwayName' not found");
            }

            if (e.WebTest.Context.TryGetValue("Apprenticeships.Apprenticeships#csv.FworkCode",
                                        out contextParameterObject))
            {
                //e.WebTest.Context[ContextParamTarget] = contextParameterObject.ToString();
                FWId = FWId + "-" + contextParameterObject.ToString();
                //e.WebTest.AddCommentToResult(contextParameterObject.ToString());
            }
            else
            {
                throw new WebTestException("'Apprenticeships.Apprenticeships#csv.FworkCode' not found");
            }

            if (e.WebTest.Context.TryGetValue("Apprenticeships.Apprenticeships#csv.ProgType",
                                        out contextParameterObject))
            {
                //e.WebTest.Context[ContextParamTarget] = contextParameterObject.ToString();
                FWId = FWId + "-" + contextParameterObject.ToString();
                //e.WebTest.AddCommentToResult(contextParameterObject.ToString());
            }
            else
            {
                throw new WebTestException("'Apprenticeships.Apprenticeships#csv.ProgType' not found");
            }

            e.WebTest.Context[ContextParamTarget] = FWId.ToString();

        }
    }

    [System.ComponentModel.DisplayName("Counter Request")]
    [System.ComponentModel.Description("Submit a request after counter is met")]
    public class Counter : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        [System.ComponentModel.DisplayName("Set Counter")]
        [System.ComponentModel.Description("Set the counter.")]
        public int IterationCounter { get; set; }
      

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            string setTrigger = Boolean.FalseString;

            for (int i = 0; i <= IterationCounter; i++)
            {
                setTrigger = Boolean.TrueString;
            }

            e.WebTest.Context[ContextParamTarget] = setTrigger;

            base.PreWebTest(sender, e);
        }
    }

}
