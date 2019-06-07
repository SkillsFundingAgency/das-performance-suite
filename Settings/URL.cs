using System.Configuration;

namespace Settings
{
    public static class AccountsURL
    {
        public static string AccountTarget { get; } = string.Format("accounts.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
        
    }

    public static class ApprenticesURL
    {
        public static string ApprenticesTarget { get; } = string.Format("{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class FinanceURL
    {
        public static string FinanceTarget { get; } = string.Format("finance.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class ForecastingURL
    {
        public static string ForecastingTarget { get; } = string.Format("forecasting.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class PermissionsURL
    {
        public static string PermissionTarget { get; } = string.Format("permissions.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class RecruitURL
    {
        public static string RecruitTarget { get; } = string.Format("recruit.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class LandingURL
    {
        public static string LandingTarget { get; } = string.Format("{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class LoginURL
    {
        public static string LoginTarget { get; } = string.Format("{0}-login.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class ReportingURL
    {
        public static string ReportingTarget { get; } = string.Format("{0}-reporting.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class PASURL
    {
        public static string PASTarget { get; } = string.Format("{0}-pas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }


    //#########################################################################################################################################################//
    public static class AccountsURLT2
    {
        public static string AccountTarget { get; } = string.Format("accounts.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);

    }

    public static class ApprenticesURLT2
    {
        public static string ApprenticesTarget { get; } = string.Format("{0}-empc.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class FinanceURLT2
    {
        public static string FinanceTarget { get; } = string.Format("financev2.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class ForecastingURLT2
    {
        public static string ForecastingTarget { get; } = string.Format("{0}-forecasting.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class PermissionsURLT2
    {
        public static string PermissionTarget { get; } = string.Format("permissions.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class RecruitURLT2
    {
        public static string RecruitTarget { get; } = string.Format("recruit.{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class LandingURLT2
    {
        public static string LandingTarget { get; } = string.Format("{0}-eas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class LoginURLT2
    {
        public static string LoginTarget { get; } = string.Format("{0}-login.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class ReportingURLT2
    {
        public static string ReportingTarget { get; } = string.Format("{0}-reporting.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }

    public static class PASURLT2
    {
        public static string PASTarget { get; } = string.Format("{0}-pas.apprenticeships.education.gov.uk", PerformanceTest.Default.TestEnvironment);
    }
    //#########################################################################################################################################################//
}
