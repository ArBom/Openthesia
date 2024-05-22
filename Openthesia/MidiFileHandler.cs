﻿using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Openthesia.FileDialogs;

namespace Openthesia;

public class MidiFileHandler
{
    public static void LoadMidiFile(string filePath)
    {
        var midiFile = MidiFile.Read(filePath);

        MidiFileData.MidiFile = midiFile;
        MidiFileData.TempoMap = midiFile.GetTempoMap();
        MidiFileData.Notes = midiFile.GetNotes();
        MidiFileData.FileName = Path.GetFileName(filePath);

        if (MidiPlayer.Playback != null)
        {
            MidiPlayer.Playback.Stop();
            MidiPlayer.Playback.EventPlayed -= IOHandle.OnEventReceived;

            PlaybackCurrentTimeWatcher.Instance.Stop();
            PlaybackCurrentTimeWatcher.Instance.CurrentTimeChanged -= MidiPlayer.OnCurrentTimeChanged;
            PlaybackCurrentTimeWatcher.Instance.RemovePlayback(MidiPlayer.Playback);
        }

        MidiPlayer.Playback = midiFile.GetPlayback(Settings.ODevice);
        MidiPlayer.Playback.TrackNotes = true;
        MidiPlayer.Playback.TrackProgram = true;
        MidiPlayer.Playback.EventPlayed += IOHandle.OnEventReceived;
        MidiPlayer.Playback.Finished += MidiPlayer.Playback_Finished;

        PlaybackCurrentTimeWatcher.Instance.AddPlayback(MidiPlayer.Playback, TimeSpanType.Midi);
        PlaybackCurrentTimeWatcher.Instance.CurrentTimeChanged += MidiPlayer.OnCurrentTimeChanged;
        PlaybackCurrentTimeWatcher.Instance.Start();

        Program._window.Title = $"Openthesia ({MidiFileData.FileName})";
    }

    public static void LoadMidiFile(MidiFile midi)
    {
        var midiFile = midi;

        MidiFileData.MidiFile = midiFile;
        MidiFileData.TempoMap = midiFile.GetTempoMap();
        MidiFileData.Notes = midiFile.GetNotes();

        if (MidiPlayer.Playback != null)
        {
            MidiPlayer.Playback.Stop();
            MidiPlayer.Playback.EventPlayed -= IOHandle.OnEventReceived;

            PlaybackCurrentTimeWatcher.Instance.Stop();
            PlaybackCurrentTimeWatcher.Instance.CurrentTimeChanged -= MidiPlayer.OnCurrentTimeChanged;
            PlaybackCurrentTimeWatcher.Instance.RemovePlayback(MidiPlayer.Playback);
        }

        MidiPlayer.Playback = midiFile.GetPlayback(Settings.ODevice);
        MidiPlayer.Playback.TrackNotes = true;
        MidiPlayer.Playback.TrackProgram = true;
        MidiPlayer.Playback.EventPlayed += IOHandle.OnEventReceived;
        MidiPlayer.Playback.Finished += MidiPlayer.Playback_Finished;

        PlaybackCurrentTimeWatcher.Instance.AddPlayback(MidiPlayer.Playback, TimeSpanType.Midi);
        PlaybackCurrentTimeWatcher.Instance.CurrentTimeChanged += MidiPlayer.OnCurrentTimeChanged;
        PlaybackCurrentTimeWatcher.Instance.Start();
    }

    public static bool OpenMidiDialog()
    {
        var dialog = new OpenFileDialog()
        {
            Title = "Select a midi file",
            Filter = "midi files (*.mid)|*.mid"
        };
        dialog.ShowOpenFileDialog();

        if (dialog.Success)
        {
            var file = new FileInfo(dialog.Files.First());
            //MidiFileData.FileName = file.Name;
            LoadMidiFile(file.FullName);
            return true;
        }
        return false;
    }
}
