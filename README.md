# netric

[![Join the chat at https://gitter.im/MaciekLesiczka/netric](https://badges.gitter.im/MaciekLesiczka/netric.svg)](https://gitter.im/MaciekLesiczka/netric?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
Web-based tool for profiling ASP.NET applications. Aim of this project is to create lightweight profiling/monitoring tool with minimal overhead. 

## Why netric
* IL Code injection on runtime - no need for code changes
* Fast, low overhead communication based on ETW
* [FlameGraphs](http://www.brendangregg.com/flamegraphs.html) to visualize bottlenecks in the code
* Charts saved in SVG files


## How to profile ASP.NET application on IIS in 3 steps
1. __Download, unzip the [release](https://github.com/MaciekLesiczka/netric/releases/tag/v0.1-alpha) and install *Netric_x86/x64.msi*__. Choose x86 if you have 32-bit system or if application pool of profiled site has "Enable 32-bit applications" flag set to true.
2. **Choose the site to profile**
    1. Go to downloaded release folder and run

        ```bat
        apman -i "my site"
        ```
        which will enable profiling to site named *my site* . To make sure you use proper site name, go to IIS Manager (inetmgr) and check site names in the "Sites" node
    2. Choose assemblies to instrument. Instrumenting main assemblies of an application is a good place to start (and in the most cases just enough). So for instance, if your application consists of assemblies named like Company.App.Web.dll, Company.App.Domain.dll, Company.App.Infrastructure.dll etc. you may just point all of them like below
       
        ```bat
        apman -asm Company.App*
        ```
        For more information and examples who to choose instrumented assemblies you can type
        
        ```bat
        apman -?
        ```
    3. Reset IIS to apply new configuration
3. __Start *netric.exe*__. Now the profiler is ready. When you start navigating through the site, netric will start collecting metrics  and will be saving visualizations in *flames* subfolder.

## How does it work
Main task for the tool is to gather method execution times from profiled ASP.NET apps and generate Flame Graph for every request. Methods are instrumented on runtime via ICorProfiler API. All metrics are sent through custom ETWs to the separate process which generates SVG charts and stores them on file system.

## How to build it

Run following command in root directory:

```
build.bat
```
