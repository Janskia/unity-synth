using Minis;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles midi keyboard using Minis plugin. Detects notes played on midi keyboard, maps it to frequencies and appplies it to an oscillator. Based on Minis example code.
/// </summary>
public class MidiController : MonoBehaviour
{
    [SerializeField]
    private Vector2Int range;

    private CompositeOscillator oscillator;

    private int currentNote;

    private void Awake()
    {
        oscillator = GetComponent<CompositeOscillator>();
    }

    private void Start()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change != InputDeviceChange.Added) return;

            MidiDevice midiDevice = device as MidiDevice;
            if (midiDevice == null) return;

            midiDevice.onWillNoteOn += (note, velocity) =>
            {
                // Note that you can't use note.velocity because the state
                // hasn't been updated yet (as this is "will" event). The note
                // object is only useful to specify the target note (note
                // number, channel number, device name, etc.) Use the velocity
                // argument as an input note velocity.
                Debug.Log(string.Format(
                    "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    velocity,
                    (note.device as MidiDevice)?.channel,
                    note.device.description.product
                ));

                if (note.noteNumber > range.x && note.noteNumber < range.y)
                {
                    oscillator.frequency = CalculateFrequency(note.noteNumber);
                    oscillator.isPlaying = true;
                    currentNote = note.noteNumber;
                }
            };

            midiDevice.onWillNoteOff += (note) =>
            {
                Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as MidiDevice)?.channel,
                    note.device.description.product
                ));

                if (note.noteNumber > range.x && note.noteNumber < range.y)
                {
                    if (note.noteNumber == currentNote)
                    {
                        oscillator.isPlaying = false;
                    }
                }
            };
        };
    }

    private float CalculateFrequency(float noteNumber)
    {
        float root = Mathf.Pow(2, 1 / 12f);
        float firstNoteFrequency = 440f / Mathf.Pow(2, 6);
        return firstNoteFrequency * Mathf.Pow(root, noteNumber);
    }
}
