# Adds the camera/photo permissions WIPEY needs to your Android manifest. Safe to run more than once.
# Put this file in your project folder (the one with Wipey.csproj), then run:
#     powershell -ExecutionPolicy Bypass -File .\add-permissions.ps1
$mf = Join-Path $PSScriptRoot "Platforms\Android\AndroidManifest.xml"
if (-not (Test-Path $mf)) {
    Write-Host "Could not find $mf" -ForegroundColor Red
    Write-Host "Run this script from the folder that contains Wipey.csproj." -ForegroundColor Red
    exit 1
}

$xml = [System.IO.File]::ReadAllText($mf)
$wanted = @(
    @{ Name = 'android.permission.CAMERA';                Line = '<uses-permission android:name="android.permission.CAMERA" />' },
    @{ Name = 'android.permission.WRITE_EXTERNAL_STORAGE'; Line = '<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" android:maxSdkVersion="32" />' },
    @{ Name = 'android.permission.READ_EXTERNAL_STORAGE';  Line = '<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" android:maxSdkVersion="32" />' },
    @{ Name = 'android.permission.READ_MEDIA_IMAGES';      Line = '<uses-permission android:name="android.permission.READ_MEDIA_IMAGES" />' },
    @{ Name = 'android.hardware.camera';                   Line = '<uses-feature android:name="android.hardware.camera" android:required="false" />' },
    @{ Name = 'android.media.action.IMAGE_CAPTURE';        Line = '<queries><intent><action android:name="android.media.action.IMAGE_CAPTURE" /></intent></queries>' }
)

$add = ""
foreach ($w in $wanted) {
    if ($xml -notmatch [regex]::Escape($w.Name)) {
        $add += "`t" + $w.Line + "`r`n"
        Write-Host "Adding  $($w.Name)" -ForegroundColor Cyan
    } else {
        Write-Host "Already there  $($w.Name)" -ForegroundColor DarkGray
    }
}

if ($add -ne "") {
    Copy-Item $mf "$mf.bak" -Force
    $xml = $xml.Replace("</manifest>", $add + "</manifest>")
    [System.IO.File]::WriteAllText($mf, $xml, (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "Done. Backup saved as AndroidManifest.xml.bak" -ForegroundColor Green
} else {
    Write-Host "Nothing to change - all permissions are already in your manifest." -ForegroundColor Green
}
