using Microsoft.JSInterop;
using NotesAndScales;

namespace NotationTeacher.Shared;

public partial class MidiInputBanner
{
    public enum MidiConnectionStatus
    {
        Connected,
        NotConnected,
        MidiAccessOnly,
        NoMidiDevices,
        NoMidiSupport
    }

    public static readonly IReadOnlyDictionary<MidiConnectionStatus, string> StatusNames = new Dictionary<MidiConnectionStatus, string>()
{
    {MidiConnectionStatus.Connected ,"Connected"},
    {MidiConnectionStatus.NotConnected ,"Not connected"},
    {MidiConnectionStatus.MidiAccessOnly ,"Connected, but failed to get midi devices."},
    {MidiConnectionStatus.NoMidiDevices ,"Couldn't find midi devices."},
    {MidiConnectionStatus.NoMidiSupport ,"Browser doesn't support midi devices."}
};

    public static MidiConnectionStatus ConnectionStatus { get; private set; } = MidiConnectionStatus.NotConnected;

    public static string? DeviceName { get; private set; } = null;

    // Automatically connects to keyboard when it loads on a page.
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Connect();
        }

        return Task.CompletedTask;
    }

    [JSInvokable]
    public static void ReceiveJavascriptError()
    {
        Console.WriteLine("A javascript error occured while trying to get midi access.");
    }

    [JSInvokable]
    public static void ReceiveMidiAccess()
    {
        ConnectionStatus = MidiConnectionStatus.MidiAccessOnly;
        StaticNoteInputService?.OnNoteUpdate();
    }

    [JSInvokable]
    public static void ReceiveConnectionSuccessful(string deviceName)
    {
        Console.WriteLine($"Connected. Device name: {deviceName}");
        ConnectionStatus = MidiConnectionStatus.Connected;
        DeviceName = deviceName;
        StaticNoteInputService?.OnNoteUpdate();
    }

    [JSInvokable]
    public static void ReceiveNoMidiDevices()
    {
        ConnectionStatus = MidiConnectionStatus.NoMidiDevices;
        StaticNoteInputService?.OnNoteUpdate();
    }

    [JSInvokable]
    public static void ReceiveNoMidiSupport()
    {
        ConnectionStatus = MidiConnectionStatus.NoMidiSupport;
        StaticNoteInputService?.OnNoteUpdate();
    }

    protected override void OnParametersSet()
    {
        StaticNoteInputService = NoteInputService;        
        NoteInputService.NoteUpdate += RecieveUpdate;
    }

    public void RecieveUpdate(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private static NoteInputService? StaticNoteInputService = null;

    [JSInvokable]
    public static void ReceiveNoteOn(string note)
    {
        if (StaticNoteInputService is not null && int.TryParse(note, out int midiInt))
        {
            StaticNoteInputService.NoteDown(Note.FromMidiNumber(midiInt));
        }
    }

    [JSInvokable]
    public static void ReceiveNoteOff(string noteMidi)
    {

    }

    public async void Connect()
    {
        ConnectionStatus = MidiConnectionStatus.NotConnected;
        DeviceName = null;

        await JS.InvokeVoidAsync("clearMidiListeners");
        await JS.InvokeVoidAsync("setupMidi");
    }
}
