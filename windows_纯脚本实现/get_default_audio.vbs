Set objAudio = CreateObject("SAPI.SpVoice")
Set objToken = objAudio.GetAudioOutputs().Item(0)
Wscript.Echo objToken.GetDescription
