using System;
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
            //regValues.ToWObject()

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

            var setup = new Feature("WebPBGone");
            var project = new Project("WebPBGone",
                new Dir(@"%ProgramFiles%\WebPBGone",
                    new DirFiles(@"..\..\src\bin\Release\*.dll"),
                    new File(@"..\..\src\bin\Release\WebPBGone.exe")),
                new RegValue(new Id("WebP"), setup, RegistryHive.ClassesRoot, @".webp", "", "WebPBGone"),
                new RegValue(new Id("WebPBGone"), setup, RegistryHive.ClassesRoot, @"WebPBGone", "", "WebP Image"),
                new RegValue(new Id("Icon"), setup, RegistryHive.ClassesRoot, @"WebPBGone\DefaultIcon", "", "C:\\Program Files (x86)\\WebPBGone\\WebPBGone.exe,0"),
                new RegValue(new Id("Shell"), setup, RegistryHive.ClassesRoot, @"WebPBGone\shell", "", "open"),
                new RegKey(setup, RegistryHive.ClassesRoot, @"WebPBGone\shell\open"),
                new RegValue(new Id("Command"), setup, RegistryHive.ClassesRoot, @"WebPBGone\shell\open\command", "", "\"C:\\Program Files (x86)\\WebPBGone\\WebPBGone.exe\" %1")
                ); ;
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
}
