using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using WixSharp;
using WixSharp.CommonTasks;

namespace installer
{
    internal static class installer
    {
        private static void Main(string[] args)
        {
            //Decided setting registry values with an action was easier than setting them with the .reg file
            // The .reg file method (commented out) was adding an extra "\" before the "%" in the progfile
            //var fullSetup = new Feature("Registry Edits");
            //IEnumerable<RegValue> regValues = Tasks.ImportRegFile("add_registers.reg").ForEach(r => r.Feature = fullSetup);

            var project = new Project("WebPBGone",
                new Dir(@"%ProgramFiles%\WebPBGone",
                    new DirFiles(@"..\..\src\bin\Release\*.dll"),
                    new File(@"..\..\src\bin\Release\WebPBGone.exe")),
                //regValues.ToWObject()
                new ManagedAction(RegistryKeys.addRegistryKeys)
                );

            project.LicenceFile = @".\License.rtf";

            Compiler.BuildMsi(project);
        }
    }

    public class RegistryKeys
    {
        [CustomAction]
        public static ActionResult addRegistryKeys(Session session)
        {
            //Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\.webp", null, "WebPBGone");
            var ProgFile = "\"" + Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\WebPBGone\\WebPBGone.exe\" %1";
            Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\shell\Convert to PNG\command", null, ProgFile);
            Registry.SetValue(@"HKEY_CLASSES_ROOT\.webp\OpenWithProgids", @"WebPBGone", 0);
            Registry.SetValue(@"HKEY_CLASSES_ROOT\.webp", null, @"WebPBGone");
            Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\Application", "ApplicationName", "WebPBGone");
            
            return ActionResult.Success;
        }
    }

    public class PlaceFiles
    {
        [CustomAction]
        public static ActionResult addFiles(Session session)
        {
            //Add folder C:\Program Files\WebPBGone

            return ActionResult.Success;
        }
    }
}
