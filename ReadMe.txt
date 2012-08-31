CrissCross
----------

CrissCross - alternative user interface for running SSRS reports
Project homepage: http://crisscross.codeplex.com/

Copyright (C) 2011 Ian Richardson
Contact: www.codeulike.com ian@codeulike.com

Licensed under the GPLv2 - see License_Gpl2.txt

Description
-----------

CrissCross is an open-source ASP.NET application that provides an alternative front-end for running SSRS Reports.
That is, a way of running SSRS reports that is like the 'Report Manager' that comes with SSRS, but different
and more customisable. It is built using the standard ASP.NET ReportViewer component and the SSRS web services.

Compatibility
-------------

CrissCross is designed to connect to SQL 2008 SSRS. But it may also work with SQL 2005 SSRS and SQL 2008 R2 SSRS.
(if you find out, let me know).
It is written in ASP.NET 3.5 and can run on most recent versions of IIS and hence most Windows Servers.

This Version
------------

This is an Alpha version of the software - most of the core features are working and stable, but there are some
features that have not been developed yet (see documentation at codeplex site)

Getting Started
---------------

Setting up CrissCross is fairly simple, but does involve some knowledge of IIS and ASP.NET configuration.
So probably best to give it to someone who knows a bit of ASP.NET to set up.

CrissCross is designed to work with a minimum of configuration - basically you just tell it where your SSRS 
server is and then off it goes. However there is additional optional configuration that can be peformed to 
control how it behaves.

1a) If you are building from source, open the solution in VS2008 and build.
   Then publish the CrissCross Web App project to a temp folder from where you can deploy it.
    - choose a folder to publish to 
    - choose 'Only files needed to run this application'
    - check the 'Include files from the App_Data folder' box.

1b) If you downloaded the zipped codeplex download, there is already a built/published version 
in the 'PublishedWebApp' folder

2) Find an IIS server and create a CrissCross Virtual Directory on it.
If you use the IIS server on your SSRS server you've got more chance of getting impersonation working (see below)
but apart from that, any IIS server will do.

3) Copy the published CrissCross web app into your virtual directory

4) Make sure Windows Authentication is available.
On IIS 6 its always an option, but in IIS 7 you might have to install it as an extra windows feature
http://www.iis.net/ConfigReference/system.webServer/security/authentication/windowsAuthentication

5) Make sure the ReportViewer component is installed on the server
If you're on the SSRS server, it'll be installed already, but if not you can install it using the
ReportViewer 2008 SP1 redistributable installer - which is in Resources/ReportViewerSP1.exe
  
6) Edit some web.config settings:

In <appSettings> :

crisscross.ReportServerRootUrl
 - set this to the url of the root folder of your SSRS reports folders. It will usually be something like
   http://your-ssrs-server/reportserver . The ReportViewer component uses this to connect to the
   SSRS server
   
crisscross.ReportServerWebServiceUrl
 - set this to the url of the SSRS web service. It is usually something like:
   http://your-ssrs-server/ReportServer/ReportService2005.asmx . CrissCross uses this to get the report 
   catalogue.
   
crisscross.ImpersonateLoggedOnUser
 - CrissCross can either run the reports as the logged in user, or run as a fixed user. Turning off
   Impersonation and using a Fixed User is easier to get started with, but turning Impersonation on makes
   CrissCross more useful. Note that Impersonation works best when CrissCross is running on the SSRS
   server itself. See the documentation at http://crisscross.codeplex.com/wikipage?title=Impersonation%20Mode for more details.
   
crisscross.FixedSsrsUsername
crisscross.FixedSsrsDomain
crisscross.FixedSsrsPassword
 - If ImpersonationLoggedOnUser is false, these settings must be set to a valid domain/username/password for
   running SSRS reports. For best results, set the web app's Application Pool (in IIS) to run under the same account.
  
crisscross.UseReportHistory
 - CrissCross queries information from the SSRS Report History Log (see connection strings, below). If you'd rather
   not do that, set this value to false. 
   
crisscross.ReportHistoryFormat
 - If UseReportHistory is true, this tells CrissCross what format to read the history in. The default is "2008" (for
   SSRS 2008). If you are using SSRS 2005 then set it to "2005".    
   
Also, further down in the web config:
<identity impersonate="true/false"/>   
 - This must be set to the same true/false value as ImpersonateLoggedOnUser
 
In <ConnectionStrings> :
ReportServerDb
 - Set this to point at your SSRS database (the database that holds SSRS settings). It will usually be a database
   called ReportServer on the SSRS server. CrissCross uses this to fetch info about previous report runs from the 
   report log - hence only the datareader role in necessary for the SQL user.
 
7) If using Impersonation Mode, make sure Integreated Windows Authentication is turned on in IIS.
See: http://crisscross.codeplex.com/wikipage?title=Impersonation%20Mode 
 
8) That should be it; use a browser to navigate to the virtual directory and CrissCross should start running.

Troubleshooting
---------------

CrissCross logs general info to log.txt in the App_Data folder
Elmah is used for error logging, so check Elmah if you are having problems:
- On the local server, browse to (your virtual folder)/elmah.axd to see the error log.
- Or see the XML files in the App_Data\ErrorLog folder.

For documentation, discussions and to log bugs to the Issue Tracker, please go to http://crisscross.codeplex.com/
