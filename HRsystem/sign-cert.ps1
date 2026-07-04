$cert = Get-ChildItem -Path "Cert:\CurrentUser\My" -CodeSigningCert
$pwd = ConvertTo-SecureString -String "HRsystem123!" -Force -AsPlainText
$certPath = "D:\Projects\C#\HRplusBasma\HRsystem\HRsystemDevCert.pfx"
Export-PfxCertificate -Cert $cert -FilePath $certPath -Password $pwd
Write-Host "Certificate exported successfully to: $certPath"