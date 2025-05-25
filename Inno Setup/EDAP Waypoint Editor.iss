#define MyAppName "EDAP Waypoint Editor"
#define MyAppVersion "1.9.0"
#define MyAppPublisher "Steve Towner"
;#define MyAppURL "http://www.thefoolonthehill.net/drupal/content/controllogix-ladder-conversion-bc4"
#define MyAppExeName "EDAP Waypoint Editor.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{754FF5F1-8532-4C2E-A2C2-A5DB9D8934AC}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
;AppPublisherURL={#MyAppURL}
;AppSupportURL={#MyAppURL}
;AppUpdatesURL={#MyAppURL}

;Installation Pages
DisableReadyPage=True
DisableProgramGroupPage=yes

DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}

AllowUNCPath=False
UninstallDisplayIcon={uninstallexe}

; Compiler Settings
OutputDir=..\Release
OutputBaseFilename={#MyAppName} Setup v{#MyAppVersion}

[Languages]
;Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Files]
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\EDAP Waypoint Editor.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\EDAP Waypoint Editor.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\AsyncIO.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\HtmlAgilityPack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\NaCl.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\NetMQ.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\System.Buffers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\System.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\System.Numerics.Vectors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\System.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\EDAP Waypoint Editor\EDAP Waypoint Editor\bin\Debug\System.ValueTuple.dll"; DestDir: "{app}"; Flags: ignoreversion

[Tasks]
; This section is optional. It defines all of the user-customizable tasks Setup
; will perform during installation. These tasks appear as check boxes and radio
; buttons on the Select Additional Tasks wizard page.
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[CustomMessages]
;CheckForUpdates=Check for updates

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
;Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
;Name: "{group}\{#MyAppName} Help"; Filename: "{app}\{#MyAppName} Help.chm"
;Name: "{group}\{cm:CheckForUpdates}"; Filename: "{app}\Update Program.exe"; IconFilename: "{app}\Update Program.exe"; IconIndex: 0
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[ThirdParty]
UseRelativePaths=True

[Components]
; This section is optional. It defines all of the components Setup will show
; on the Select Components page of the wizard for setup type customization.

[Types]
; This section is optional. It defines all of the setup types Setup will show
; on the Select Components page of the wizard. For examle Full/Compact/Custom

[Run]
Filename: "{app}\{#MyAppExeName}"; Flags: nowait postinstall; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"
;Filename: "{app}\{#MyAppName} Help.chm"; Flags: postinstall shellexec skipifdoesntexist; Description: "View the help file (what's new?)"

[Code]
// shared code for installing the products
#include "scripts\products.iss"
// helper functions
#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"
// actual products
#include "scripts\products\dotnetfx48.iss"
