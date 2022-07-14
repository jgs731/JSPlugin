using AudioPlugSharp;

namespace JSPlugin
{
    public class Program : AudioPluginBase
    {
        public Program()
        {
            Company = "MUC";
            Website = "www.jg-tests.co.uk";
            Contact = "contact@my.email";
            PluginName = "JS Bass Plugin";
            PluginCategory = "Instrument|Synth";
            PluginVersion = "1.0.0";

            // Unique 64bit ID for the plugin
            PluginID = 0x94477AF90D2F50BD;
        }

        AudioIOPort monoInput;
        AudioIOPort monoOutput;

        public override void Initialize()
        {
            base.Initialize();

            InputPorts = new AudioIOPort[] { monoInput = new AudioIOPort("Mono Input", EAudioChannelConfiguration.Mono) };
            OutputPorts = new AudioIOPort[] { monoOutput = new AudioIOPort("Mono Output", EAudioChannelConfiguration.Mono) };

            AddParameter(new AudioPluginParameter
            {
                // Add first parameter from document for JSPlugin
            });
        }

        public override void Process()
        {
            base.Process();
        }
    }
}