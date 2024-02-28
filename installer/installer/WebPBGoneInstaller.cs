using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;
using WixSharp;

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
                    new WixSharp.File(@"..\..\src\bin\Release\WebPBGone.exe")),
                //regValues.ToWObject()
                new ManagedAction(RegistryKeys.addRegistryKeys)
                );
            project.LicenceFile = @".\License.rtf";
            project.InstallPrivileges = InstallPrivileges.elevated;


            Compiler.BuildMsi(project);

            //I ran into some issues when installing with the above installer due to permission issues with the keys
            //This installer is the above but has the user install the keys manually with a .reg file
            //Kinda a hack, kinda don't care

            var projectLite = new Project("WebPBGoneLite",
                new Dir(@"%ProgramFiles%\WebPBGone",
                    new DirFiles(@"..\..\src\bin\Release\*.dll"),
                    new WixSharp.File(@"..\..\src\bin\Release\WebPBGone.exe"))
                );
            projectLite.LicenceFile = @".\License.rtf";
            projectLite.InstallPrivileges = InstallPrivileges.elevated;

            Compiler.BuildMsi(projectLite);
        }
    }

    public class RegistryKeys
    {
        [CustomAction]
        public static ActionResult addRegistryKeys(Session session)
        {
            //Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\.webp", null, "WebPBGone");
            try
            {
                var x86Path = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                var ProgFile = "\"" + x86Path + "\\WebPBGone\\WebPBGone.exe\" %1";

                /*
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\shell\Convert to PNG\command", null, ProgFile);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\.webp\OpenWithProgids", @"WebPBGone", 0);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\.webp", null, @"WebPBGone");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\Application", "ApplicationName", "WebPBGone");
                */
                Registry.SetValue(@"HKEY_CLASSES_ROOT\.webp", null, "WebPBGone");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone", null, "WebP Image");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone", "Content Type", "image/webp");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone", "PerceivedType", "image");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone", "ImageOptionFlags", 00000001, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\DefaultIcon", null, x86Path + "\\WebPBGone\\WebPBGone.exe,0");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\WebPBGone\shell", null, "open");
                Registry.ClassesRoot.CreateSubKey(@"HKEY_CLASSES_ROOT\WebPBGone\shell\open");
                var commandKey = Registry.ClassesRoot.CreateSubKey(@"HKEY_CLASSES_ROOT\WebPBGone\shell\open\command");
                commandKey.SetValue("", x86Path + "\\WebPBGone\\WebPBGone.exe \"%1\"");

                /*
                
                [HKEY_CLASSES_ROOT\.webp]
                @= "WebPBGone"

                [HKEY_CLASSES_ROOT\WebPBGone]
                @= "WebP Image"
                "Content Type" = "image/webp"
                "PerceivedType" = "image"
                "ImageOptionFlags" = dword:00000001

                [HKEY_CLASSES_ROOT\WebPBGone\DefaultIcon]
                @= "[ProgramFilesFolderX86]\\WebPBGone\\WebPBGone.exe,0"
                
                [HKEY_CLASSES_ROOT\WebPBGone\shell]
                @= "open"
                        
                [HKEY_CLASSES_ROOT\WebPBGone\shell\open]

                [HKEY_CLASSES_ROOT\WebPBGone\shell\open\command]
                @= "[ProgramFilesFolderX86]\\WebPBGone\\WebPBGone.exe \"%1\""

                */
            }
            catch
            {
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
    }
}
