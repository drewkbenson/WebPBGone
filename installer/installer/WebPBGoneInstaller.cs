using System;
using WixSharp;

namespace installer
{
    internal static class installer
    {
        private static void Main(string[] args)
        {
            /*
            Decided setting registry values with an action was easier than setting them with the .reg file
            The .reg file method (commented out) was adding an extra "\" before the "%" in the progfile
            var fullSetup = new Feature("Registry Edits");
            IEnumerable<RegValue> regValues = Tasks.ImportRegFile("add_registers.reg").ForEach(r => r.Feature = fullSetup);
            regValues.ToWObject()
            */

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
                new RegValue(new Id("Icon"), setup, RegistryHive.ClassesRoot, @"WebPBGone\DefaultIcon", "", @"C:\Program Files (x86)\WebPBGone\WebPBGone.exe,0"),
                new RegValue(new Id("Shell"), setup, RegistryHive.ClassesRoot, @"WebPBGone\shell", "", "open"),
                new RegKey(setup, RegistryHive.ClassesRoot, @"WebPBGone\shell\open"),
                new RegValue(new Id("Command"), setup, RegistryHive.ClassesRoot, @"WebPBGone\shell\open\command", "", "\"C:\\Program Files (x86)\\WebPBGone\\WebPBGone.exe\" %1")
                );
            project.LicenceFile = @".\License.rtf";
            project.InstallPrivileges = InstallPrivileges.elevated;

            Compiler.BuildMsi(project);


            /* 
            I ran into some issues when installing with the above installer due to permission issues with the keys
            This installer is the above but has the user install the keys manually with a .reg file
            Kinda a hack, kinda don't care
            
            This isn't necessary anymore. I found out why it was claiming that the progress failed on registry write
            When you make a customaction it's something that doesn't get elevated, despite taking place in the .msi
            as it is defined as an after action, rather than something that happens during the install
            */

            var projectLite = new Project("WebPBGoneLite",
                new Dir(@"%ProgramFiles%\WebPBGone",
                    new DirFiles(@"..\..\src\bin\Release\*.dll"),
                    new File(@"..\..\src\bin\Release\WebPBGone.exe"))
                );
            projectLite.LicenceFile = @".\License.rtf";
            projectLite.InstallPrivileges = InstallPrivileges.elevated;

            Compiler.BuildMsi(projectLite);

            /*
            I high key hate this building into 3 MSIs but I should just delete the first two right now

            The point of this one is to allow multiple selections, including entire folders to be recursively gone thru and turned into pngs
            */


            /*
            [HKEY_CLASSES_ROOT\*\shell\WebPBGone]
            "MUIVerb"="WebPBGone"
            "Icon"=""
            "SubCommands"="WebPBGoneDelete;WebPBGoneNoDelete"

            [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneDelete]
            "MUIVerb"="Delete"

            [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneDelete\command]
            "command"="C:\\path\\to\\wasd_program.exe"

            [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneNoDelete]
            "MUIVerb"="No Delete"

            [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneNoDelete\command]
            "command"="C:\\path\\to\\wasd_program.exe"
             */
            var newSetup = new Feature("NewWebPBGone");
            var newProject = new Project("NewWebPBGone",
                new Dir(@"%ProgramFiles%\WebPBGone",
                    new DirFiles(@"..\..\src\bin\Release\*.dll"),
                    new File(@"..\..\src\bin\Release\WebPBGone.exe")),
                new RegValue(new Id("Dropdown"), newSetup, RegistryHive.ClassesRoot, @"*\shell\WebPBGone", "MUIVerb", "WebPBGone"),
                new RegValue(new Id("DropdownIcon"), newSetup, RegistryHive.ClassesRoot, @"*\shell\WebPBGone", "Icon", @"C:\Program Files (x86)\WebPBGone\WebPBGone.exe,0"),
                new RegValue(new Id("DropdownCmds"), newSetup, RegistryHive.ClassesRoot, @"*\shell\WebPBGone", "SubCommands", "WebPBGoneDelete;WebPBGoneNoDelete"),
                new RegValue(new Id("NoDeleteMUI"), newSetup, RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneNoDelete", "MUIVerb", "No Delete"),
                new RegValue(new Id("NoDeleteCMD"), newSetup, RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneNoDelete\command", "", @"C:\Program Files (x86)\WebPBGone\WebPBGone.exe %1"),
                new RegValue(new Id("DeleteMUI"),   newSetup, RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneDelete",   "MUIVerb", "Delete"),
                new RegValue(new Id("DeleteCMD"),   newSetup, RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WebPBGoneDelete\command",   "", @"C:\Program Files (x86)\WebPBGone\WebPBGone.exe %1 --delete")
                );
            newProject.LicenceFile = @".\License.rtf";
            newProject.InstallPrivileges = InstallPrivileges.elevated;

            Compiler.BuildMsi(newProject);
        }
    }
}
