using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Speech.Synthesis;

namespace VirtualMaps
{
    public partial class Commands : PhoneApplicationPage
    {
        SpeechSynthesizer synth;
        public Commands()
        {
            InitializeComponent();
        }

        private void nav_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            speakCommandExample("Navigation.");
        }

        private void curloc_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            speakCommandExample("Where am I?");
        }

        private void modes_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            speakCommandExample("Change my map.");
        }

        private void srchplace_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            speakCommandExample("Search place.");
        }

        private async void speakCommandExample(string cmnd)
        {
            synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync(cmnd);
        }

        private void srchpeople_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            speakCommandExample("Search people.");
        }

        private void callnearby_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            speakCommandExample("Call nearby.");
        }
    }
}