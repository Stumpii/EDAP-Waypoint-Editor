// requires Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Server 2012 R2, Windows Vista Service Pack 2
// WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
// http://www.microsoft.com/en-us/download/details.aspx?id=48137

[CustomMessages]
dotnetfx48_title=.NET Framework 4.8

dotnetfx48_size=1 MB - 63 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.dotnetfx48_lcid=
de.dotnetfx48_lcid=/lcid 1031


[Code]
const
	dotnetfx48_url = 'https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer';

procedure dotnetfx46(MinVersion: integer);
begin
	if (not netfxinstalled(NetFx4x, '') or (netfxspversion(NetFx4x, '') < MinVersion)) then
		AddProduct('dotnetfx48.exe',
			CustomMessage('dotnetfx48_lcid') + ' /q /passive /norestart',
			CustomMessage('dotnetfx48_title'),
			CustomMessage('dotnetfx48_size'),
			dotnetfx48_url,
			false, false);
end;