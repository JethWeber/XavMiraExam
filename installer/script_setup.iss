#define MyAppName "XavMira Exam"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "XavMira Technology"
#define MyAppExeName "XavMiraExam.exe"

[Setup]
AppId={{952DC2C7-2876-4793-BF54-101197CE8065}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

DefaultDirName={autopf}\XavMira Exam
DefaultGroupName={#MyAppName}

UninstallDisplayIcon={app}\{#MyAppExeName}

ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

OutputDir=Z:\home\jeth\projects\XavMiraExam\installer
OutputBaseFilename=XavMiraSetup

SetupIconFile=Z:\home\jeth\projects\XavMiraExam\XavMiraExam.Desktop\Assets\XMES_Logo.ico

Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "Z:\home\jeth\projects\XavMiraExam\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent