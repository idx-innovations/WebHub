# WebHub
...is a lightweight and extensible hub for .net, allowing to host simple web pages and rapid backend development.

## It's focusing on... 
- a modular and reusable infrastructure
- serving static content (without requiring to run IIS or Apache)
- working as a central communication relay for integrating more traditional desktop-based applications with web communication layer applications

## Getting started
Please download the entire project and build the 'IDX' solution using Visual Studio. A command line version of the application will start-up by default, using default settings.

**Please note:** Depending an your system configuration, you may have to start Visual Studio with elevated rights in order to registrate the port listening.
**Please note:** By default the CLI will try to write an example settings file to 'D:\DemoSettings.xml'. Please make sure you have sufficient rights. This behavior might be changed by adjusting the command line parameters for the 'IDX.WebHub.CLI' solution. 

## We like to thank
- [the NancyFx team](https://github.com/NancyFx/Nancy) for easing our life when implementing the web communication layer
- [JamesNK](https://github.com/JamesNK/Newtonsoft.Json) for Newtonsoft.Json
- [the Apache team](https://github.com/apache/logging-log4net) for log4net 
- all the good people - named by the code annotations - for providing us with helpful ideas how to solve various challenges we came across

## License
The WebHub is licensed unter MIT.