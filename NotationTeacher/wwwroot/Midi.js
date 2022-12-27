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

    DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveMidiAccess');

    midiAccessFound = true;
    midiAccessObject = midiAccess;

    var foundDevice = false;
    var deviceName = "No device found.";

    for (var input of midiAccess.inputs.values()) {
        input.onmidimessage = getMIDIMessage;
        deviceName = input.name;
        foundDevice = true;
    }

    if (foundDevice) {
        DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveConnectionSuccessful', deviceName);
    }
    else {
        DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoMidiDevices');
    }
}

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
    var velocity = (message.data.length > 2) ? message.data[2] : 0; // a velocity value might not be included with a noteOff command

    switch (command) {
        case 144: // noteOn
            if (velocity > 0) {
                DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoteOn', String(note))
            } else {
                DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoteOff', String(note))
            }
            break;
        case 128: // noteOff
            DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoteOff', String(note))
            break;        
    }    
}

function onMIDIFailure() {
    DotNet.invokeMethodAsync('NotationTeacher', 'ReceiveNoMidiSupport')
}