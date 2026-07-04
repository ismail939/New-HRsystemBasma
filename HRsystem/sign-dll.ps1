$certPath = "D:\Projects\C#\HRplusBasma\HRsystem\HRsystemDevCert.pfx"
$dllPath = "D:\Projects\C#\HRplusBasma\HRsystem\bin\Debug\net9.0\HRsystem.dll"
$pwd = ConvertTo-SecureString -String "HRsystem123!" -Force -AsPlainText

# Load the certificate from PFX
$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certPath, $pwd)

# Sign the DLL
Set-AuthenticodeSignature -FilePath $dllPath -Certificate $cert -TimestampServer "http://timestamp.digicert.com" -IncludeChain All -Force -HashAlgorithm SHA256 2>&1

if ($LASTEXITCODE -eq 0 -or $?) {
    Write-Host "DLL signed successfully!"
    Get-AuthenticodeSignature -FilePath $dllPath | Format-List
} else {
    Write-Host "Signing failed with exit code: $LASTEXITCODE"
}