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

            //I ran into some issues when installing with the above installer due to permission issues with the keys
            //This installer is the above but has the user install the keys manually with a .reg file
            //Kinda a hack, kinda don't care

            var projectLite = new Project("WebPBGoneLite",
                new Dir(@"%ProgramFiles%\WebPBGone",
                    new DirFiles(@"..\..\src\bin\Release\*.dll"),
                    new File(@"..\..\src\bin\Release\WebPBGone.exe"))
                );
            project.LicenceFile = @".\License.rtf";

            Compiler.BuildMsi(projectLite);
        }

        static void RegistryFailure(SetupEventArgs e)
        {
            
        }
    }

    class RegistryKeys
    {
        [CustomAction]
        public static ActionResult addRegistryKeys(Session session)
        {
            //Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\.webp", null, "WebPBGone");
            try
            {
                var ProgFile = "\"" + Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\WebPBGone\\WebPBGone.exe\" %1";
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\shell\Convert to PNG\command", null, ProgFile);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\.webp\OpenWithProgids", @"WebPBGone", 0);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\.webp", null, @"WebPBGone");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\Application", "ApplicationName", "WebPBGone");
            }
            catch
            {
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
    }
}
