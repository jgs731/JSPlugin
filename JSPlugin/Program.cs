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

        AudioIOPort monoOutput;
        //AudioPluginParameter waveformParameter;

        double noteVolume = 0;
        double desiredNoteVolume = 0;
        double frequency = 0;
        long samplesSoFar = 0;

        public override void Initialize()
        {
            base.Initialize();

            OutputPorts = new AudioIOPort[] { monoOutput = new AudioIOPort("Mono Output", EAudioChannelConfiguration.Mono) };

            //AddParameter(waveformParameter = new AudioPluginParameter
            //{
            //    ID = "Waveform",
            //    Type = EAudioPlug inParameterType.Int,
            //    MinValue = 1,
            //    MaxValue = 4,
            //    DefaultValue = 1,
            //    ValueFormat = "{0:0.0}dB"
            //});
        }

        public override void HandleNoteOn(int noteNumber, float velocity, int sampleOffset)
        {
            frequency = Math.Pow(2, 1.0 / 12.0) * 440;
            desiredNoteVolume = velocity * 0.25f;
        }

        public override void HandleNoteOff(int noteNumber, float velocity, int sampleOffset)
        {
            desiredNoteVolume = 0;
        }

        double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public override void Process()
        {
            base.Process();
            
            int currentSample = 0;
            int nextSample = 0;
            double[] outSamples = monoOutput.GetAudioBuffers()[0];

            nextSample = Host.ProcessEvents();

            do 
            {
                for(int i = currentSample; i < nextSample; i++)
                {
                    noteVolume = Lerp(noteVolume, desiredNoteVolume, 0.001);

                    outSamples[i] = Math.Sin((double)samplesSoFar * 2 * Math.PI * frequency / Host.SampleRate) * noteVolume;
                    samplesSoFar++;
                }
                currentSample = nextSample;
            }
            while (nextSample < outSamples.Length);

            monoOutput.WriteData();
        }
    }
}