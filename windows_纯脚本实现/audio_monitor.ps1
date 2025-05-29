Add-Type -AssemblyName System.Windows.Forms
# 需在vbs脚本目录运行
function Get-DefaultAudioDeviceName {
    $out = cscript //Nologo ".\get_default_audio.vbs"
    return $out
}

$lastDevice = Get-DefaultAudioDeviceName
Write-Output "Script started. Initial default audio device: $lastDevice"

while ($true) {
    Start-Sleep -Seconds 1
    $curDevice = Get-DefaultAudioDeviceName
    if ($curDevice -ne $lastDevice) {
        Write-Output "Audio device change detected. Old: $lastDevice New: $curDevice"
        $lastDevice = $curDevice
        if (Get-Process -Name cloudmusic -ErrorAction SilentlyContinue) {
            Write-Output "cloudmusic process found. Sending Ctrl+Alt+P."
            [System.Windows.Forms.SendKeys]::SendWait('^%p')
        }
        else {
            Write-Output "cloudmusic process not found."
        }
    }
}