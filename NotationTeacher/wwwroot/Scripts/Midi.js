// Contains function needed to connect to a midi device.

// Tries to get midi access (gives reference to functions to run).
function setupMidi() {
    try {        
        navigator.requestMIDIAccess()
            .then(onMIDISuccess, onMIDIFailure);        
    }
    catch {
        DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveJavascriptError');
    }
}

function onMIDISuccess(midiAccess) {

    // Tells program that midi access has been got.
    DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveMidiAccess');

    midiAccessFound = true;
    midiAccessObject = midiAccess;

    var foundDevice = false;
    var deviceName = "No device found.";

    // The input device will be the last one it finds.
    for (var input of midiAccess.inputs.values()) {
        input.onmidimessage = getMIDIMessage;
        deviceName = input.name;
        foundDevice = true;
    }

    // Sends outcome to program.
    if (foundDevice) {
        DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveConnectionSuccessful', deviceName);
    }
    else {
        DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoMidiDevices');
    }
}

// Used to make sure any existing events are deleted prior to reconnecting to midi device.
var midiAccessFound = false;
var midiAccessObject = null;

function clearMidiListeners() {
    if (midiAccessFound) {
        for (var input of midiAccessObject.inputs.values()) {
            input.onmidimessage = null;
        }
    }
}

function getMIDIMessage(message) {
    var command = message.data[0];
    var note = message.data[1];
    var velocity = (message.data.length > 2) ? message.data[2] : 0; // A velocity value might not be included with a note off command

    switch (command) {
        case 144: // Note on
            if (velocity > 0) {
                DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoteOn', String(note))
            } else {
                DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoteOff', String(note))
            }
            break;
        case 128: // Note off
            DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoteOff', String(note))
            break;        
    }    
}

function onMIDIFailure() {
    DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoMidiSupport')
}