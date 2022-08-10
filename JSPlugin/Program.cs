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
            PluginID = 0x98977AF90D2F50BD;
        }

        AudioIOPort monoOutput;
        //AudioPluginParameter waveformParameter;
        AudioPluginParameter attackParameter;

        double noteVolume = 0;
        double desiredNoteVolume = 0;
        double frequency = 0;
        long samplesSoFar = 0;
        double attackRate = 1.0f;
        long attackTimeDelay = 200; // in ms

        public override void Initialize()
        {
            base.Initialize();

            OutputPorts = new AudioIOPort[] { monoOutput = new AudioIOPort("Mono Output", EAudioChannelConfiguration.Mono) };

            //AddParameter(waveformParameter = new AudioPluginParameter
            //{
            //    ID = "Waveform",
            //    Type = EAudioPluginParameterType.Int,
            //    MinValue = 1,
            //    MaxValue = 4,
            //    DefaultValue = 1,
            //    ValueFormat = "{0:0.0}dB"
            //});

            AddParameter(attackParameter = new AudioPluginParameter
            {
                ID = "Attack",
                Name = "Attack",
                Type = EAudioPluginParameterType.Int,
                MinValue = 0,
                MaxValue = 0.99,
                DefaultValue = 0,
                ValueFormat = "{0:0.0}dB"
            });
        }

        public override void HandleNoteOn(int noteNumber, float velocity, int sampleOffset)
        {
            frequency = Math.Pow(2, (noteNumber - 45) / 12.0) * 440;
            desiredNoteVolume = velocity * 0.25f;
        }

        public override void HandleNoteOff(int noteNumber, float velocity, int sampleOffset)
        {
            desiredNoteVolume = 0;
        }

        public override void HandleParameterChange(AudioPluginParameter parameter, double newNormalizedValue, int sampleOffset)
        {
            base.HandleParameterChange(parameter, newNormalizedValue, sampleOffset);
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
            double attack = GetParameter("Attack").ProcessValue;

            nextSample = Host.ProcessEvents();

            do 
            {
                for(int i = currentSample; i < nextSample; i++)
                {
                    bool updateAttackParam = attackParameter.NeedInterpolationUpdate;
                    
                    noteVolume = Lerp(noteVolume, desiredNoteVolume, 0.001);

                    if (updateAttackParam)
                    {
                        attackRate = attackParameter.GetInterpolatedProcessValue(i) / attackTimeDelay;
                    }

                    outSamples[i] = (Math.Sin((double)samplesSoFar * 2 * Math.PI * frequency / Host.SampleRate) + attackRate) * noteVolume;
                    samplesSoFar++;
                }
                currentSample = nextSample;
            }
            while (nextSample < outSamples.Length);

            monoOutput.WriteData();
        }
    }
}