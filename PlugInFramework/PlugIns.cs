using System;
using System.Net;
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

namespace PlugIns
{
    //namespace Testing.Web.ExtractionRules
    //{
    //    //The web framework does not load context values from a data source BUT an extraction rule does automatically.

    //    [DisplayName("Extract Data Source Value")]
    //    [Description("Extracts the defined datasource value and places it in the defined Context parameter.")]
    //    public class ExtractDataSourceValue : ExtractionRule
    //    {
    //        private string dataSourceField = null;
    //        [DisplayName("Data Source Field")]
    //        [Description("The data source field name.  This must be set to the fully qualified data source name along with brackets. IE: {{datasource1.table.name}}")]
    //        public string DataSourceField
    //        {
    //            get { return dataSourceField; }
    //            set { dataSourceField = value; }
    //        }

    //        public override void Extract(object sender, ExtractionEventArgs e)
    //        {
    //            e.WebTest.Context.Add(ContextParameterName, dataSourceField);
    //        }
    //    }
    //}

    [Description("This plugin can be used to set the ParseDependentsRequests property for each request")]
    public class UserIteration : WebTestPlugin
    {
        private bool m_parseDependents = true;
        //private string strSQL = string.Empty;
        public void DBReset(string payRef)
        {
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string strSQL = string.Empty;
            string dbConnetionString = "Data Source=das-pp-shared-sql.database.windows.net;Initial Catalog=das-pp-eas-acc-db;User ID=sreemereddy;Password=p80LghEDiATT8X6Wqt0aZ1x2PVtwzUST6HC";
            connection = new SqlConnection(dbConnetionString);

            //strSQL = "UPDATE employer_account.accounthistory SET removedDate = GetDATE() WHERE payeref like ('500/PF00%') OR payeref like ('100/GDS6%') AND removeddate is null";
            strSQL = "UPDATE employer_account.accounthistory SET removedDate = GetDATE() WHERE payeref = '" + payRef + "' and removeddate is null";

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
            // TODO: Add code to execute before the test.  
            //e.WebTest.AddCommentToResult("Calling DBReset()...");
            //PayRef.PayRef#csv.PayRef	500/PF00500	
            object contextParameterObject;
            if (e.WebTest.Context.TryGetValue("GatewayUsers.GatewayUsers#csv.GATEWAY_PAYESCHEME",
                           out contextParameterObject))
            {
                string PayRef = contextParameterObject.ToString();
                //e.WebTest.AddCommentToResult(PayRef);
                DBReset(PayRef);
            }
            else
            {
                throw new WebTestException("'GatewayUsers.GatewayUsers#csv.GATEWAY_PAYESCHEME' not found");
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

    //public class ParseDependentRequests : WebTestPlugin
    //{
    //    [Description("Random think time")]
    //    public bool Random { get; set; }
    //    public override void PreRequest(object sender, PreRequestEventArgs e)
    //    {
    //        if (Random == true)
    //        {
    //            base.PreRequest(sender, e);
    //            e.Request.ParseDependentRequests = true;
    //        }
    //        else
    //        { 
    //            base.PreRequest(sender, e);
    //            e.Request.ParseDependentRequests = false;
    //        }
    //    }
    //}

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
            //ContextParamOrValueSource = "$WebTestIteration";
            //ContextParamTarget = "DataSource1.SomeDataFile#csv.FieldName";


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
                else //hard coded value
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

            //string ULRN = string.Empty;
            //String randomUln = System.DateTime.UtcNow.ToString("fff");
            String ULRN = GenerateRandomNumberBetweenTwoValues(10, 99).ToString() + DateTime.Now.ToString("ssffffff");

            for (int i = 1; i < 30; i++)
            {
                if (IsValidCheckSum(ULRN))
                {
                    //ULRN = randomUln;
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
            //Random number generated includes the min value, but excludes the max value
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

        [System.ComponentModel.DisplayName("Test against")]
        [System.ComponentModel.Description("Environment to execute perf test against.")]
        public string Environment { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            // Generate new guid with specified output format
            string testEnvironment = string.Empty;

            testEnvironment = Environment;

            e.WebTest.Context[ContextParamTarget] = testEnvironment;

            base.PreWebTest(sender, e);
        }
    }

    [System.ComponentModel.DisplayName("Test Environment")]
    [System.ComponentModel.Description("Environment to execute performance test against")]
    public class SetTestEnvironment : WebTestPlugin
    {
        [System.ComponentModel.DisplayName("Performance test against")]
        [System.ComponentModel.Description("Environment against which the performance test to be executed.")]
        public string Environment { get; set; }

        [System.ComponentModel.DisplayName("Target Context Parameter Name")]
        [System.ComponentModel.Description("Name of the context parameter that will receive the generated value.")]
        public string ContextParamTarget { get; set; }

        public string ContextParamLoginTarget { get; set; }

        public string ContextParamReportingTarget { get; set; }

        public string ContextParamPASTarget { get; set; }

        public override void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            // Generate new guid with specified output format
            string testEnvironment = string.Empty;

            if (Environment == "pp")
            {
                //e.WebTest.Context[ContextParamTarget] = "pp-eas.apprenticeships.sfa.bis.gov.uk";
                e.WebTest.Context[ContextParamTarget] = "pp-eas.apprenticeships.education.gov.uk";
                e.WebTest.Context[ContextParamLoginTarget] = "pp-login.apprenticeships.education.gov.uk";
                e.WebTest.Context[ContextParamReportingTarget] = "pp-reporting.apprenticeships.education.gov.uk";
                //e.WebTest.Context[ContextParamPASTarget] = "pp-pas.apprenticeships.sfa.bis.gov.uk";
                e.WebTest.Context[ContextParamPASTarget] = "pp-pas.apprenticeships.education.gov.uk";
            }
            else if (Environment == "mo")
            {
                e.WebTest.Context[ContextParamTarget] = string.Empty;
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

            //setTrigger = Iter;

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
 }
