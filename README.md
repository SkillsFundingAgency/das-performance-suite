# das-performance-suite

This is a Visual Studio Load Test performance test suit created using Visual Studio Framework with Load Test extension of VS. 

## Prerequsites and Access requirements

1. MS Visual Studio with Load Test extension
2. MSSQL Management Studio
3. MSDN License
4. Azure Access to execute the test

## Setup

Install Visual Studio along with the VSTS Load Test extension. Once the solution is imported and built all the dependencies will be available within the solution. Once the setup is complete import the copy of latest code from the Git. 

## Framework

The visual studio solution has three projects (ScriptFramework, PluginFramework and Settings), to keep the things simple. ScriptFramework holds the automated business flows (*.webtest) and the test scenarios (*.loadtest). PluginFramework has the custom code functions used by the scripts (things like unique first and last names, unique email, thinktimes, setting test environmet, etc.,). Settings project has the configuration information, db access details, etc., 

### Develop and Execute on Multiple Envrionments

The test framework is designed to work on multiple environments (ex: prep-prod, test2 and test). Update the app config file in Settings project accordingly to work against the respective environments. The app.config file in Settings project needs updated with details to point to the environment to work against.

### Custom Functions

There is collection of custom functions in PluginFramework that can be used for development and execution of Performance Test. They include resetting the payee reference in db, think time, generate a unique first & last name and email, and other functions. Any additional functions will go in here. 

## Automation of Business Flows/Journeys

Journeys identified for load test are recorded using the load test tool, the recorded journeys are automated and parameterised so the tests are robust and emulate the real user behaviour. Each request is enclosed with transaction points to measure and report the response times during the test.

## Load Test Execution

Load test is executed by collectively placing all the business flows (Journeys) automated into a load test a scenario is built with an agreed user load, to hit a target number of iterations during a set period of time. The user load is distributed based on the requirements gathered from the architects/business.

### Running a load test scenario

Build a scenario by grouping business processes into a load test. For each business process added into load test assign the number of users, user ramp-up, think times, pacing time and browser version. 

## Best Practices
Understand the Non-Functioanl requirements thoroughly.
Make sure the dependencies like data setup, configurations, etc are clear enought before load test exectuion.
Manually run through the business flow (Journey) before starting to automate.
Document the journey and make sure it's reviewed by thew concerned stake holders.

