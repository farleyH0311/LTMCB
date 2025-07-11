using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

public class SpeechService
{
    public void Speak(string text)
    {
        try
        {
            var voice = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpVoice"));
            voice.GetType().InvokeMember("Speak",
                System.Reflection.BindingFlags.InvokeMethod,
                null, voice, new object[] { text });
        }
        catch (COMException ex)
        {
            Console.WriteLine("Speech error: " + ex.Message);
        }
    }
}

