To build Visual Studio Extension for Lua on your own PC, for example to pick up the latest bug fixes, make modifications, or contribute back to the project, please use the following instructions.

## Cloning

First, you will require a local copy of our Git repository.

If you do not want to use Git, you can download the latest commit as a ZIP file from the [GitHub page](https://github.com/Microsoft/vslua/).

## Prerequisites

The following list of software is required in order to build Visual Studio Extension for Lua from source. All of these are required for a complete build, though a complete build is not required to be able to contribute. The next section outlines the projects that may be omitted when building Visual Studio Extension for Lua.
Note that you do not all versions of Visual Studio. You'll need VS 2015 to build Visual Studio Extension for Lua for VS 2015. 

<table border="1" cellspacing="0" cellpadding="0">
<tbody>
<tr>
<td valign="top" width="145">
<p><strong>Software</strong></p>
</td>
<td valign="top" width="481">
<p><strong>Download</strong></p>
</td>
</tr>
<tr>
<td valign="top" width="145">
<p>Microsoft Visual Studio Community</p>
</td>
<td valign="top" width="481">
<p>Download <a href="https://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs.aspx">here</a>. </p>
<p>Include the optional feature, Visual Studio Extensibility Tools, during installation. You can find them under Features / Common Tools / Visual Studio Extensibility Tools.</p>
</td>
</tr>
<!-- They don't need a git client to build 
<tr>
<td valign="top" width="145">
<p>msysgit</p>
</td>
<td valign="top" width="481">
<p><a href="http://msysgit.github.io/">http://msysgit.github.io/</a>&nbsp;(Git client)</p>
</td>
</tr>
-->
</tbody>
</table>

## Building with Visual Studio

*Lua.sln** can be opened and built in Visual Studio using the **Build Solution** command. To debug, ensure that **LuaLanguageServicePackage** is selected as the startup project. In Project->Properties->Debug page, select Start external program and choose devenv.exe. In the Start Options panel, set Command line arguments to -rootSuffix exp  and use F5. If an error appears rather than a new instance of VS, ensure the **Project|Debug** settings are correct.

Building in Visual Studio may produce a number of warnings related to potentially incompatible assemblies and missing references. As long as all projects build successfully, these warnings are benign and can be ignored. 
