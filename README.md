# netric
Web-based tool for profiling ASP.NET applications. Aim of this project is to create lightweight profiling/monitoring tool with minimal overhead. 

## Why netric
* IL Code injection on runtime - no need for code changes
* Fast, low overhead communication based on ETW
* [FlameGraphs](http://www.brendangregg.com/flamegraphs.html) to visualize bottlenecks in the code
* Charts saved in SVG files

## How does it work
Main task for the tool is to gather method execution times from profiled ASP.NET apps and generate Flame Graph for every request. Methods are instrumented on runtime via ICorProfiler API. All metrics are sent through custom ETWs to the separate process which generates SVG charts and stores them on file system.

## How to build it

Run following command in root directory:

```
build.bat
```
