# Covid19-DataExtract
This is a console application that pulls data from data.world for the daily Covid stats and populates a database used to display the global statistics of deaths and cases reported. The Console application is written in C# using the C# 8.0 libraries and runs in about 25 seconds to download the CSV file, extract the data and bulk copy to SQL Server. Once bult copied, the data is transferred to the main table that is is used by the companion website to display and drill down into the statistics globally.

Below is a screen capture of the website site statistics page showing the data in a log/date graph for deaths and cases as well as a chart of the 7 day running average of the current selections displayed in the chart. With this data you can determine if the pandemic is growing or shrinking in a given area.

<img src="https://github.com/LeoMrozek1895/Covid19-DataExtract/blob/master/images/Covid-19-screen-capture.gif?raw=true" alt="Covid-19 site" />

