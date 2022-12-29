// Given a div id, will draw note on a stave.

function draw(elementID, noteNames, noteAccidentals, noteDuration, timeSignatureTop, timeSignatureBottom, timeSignatureString, cleff) {
    clearBox(elementID);
    const {
        Renderer,
        Stave,
        StaveNote,
        Voice,
        Formatter,
        Accidental
    } = Vex.Flow;

    const div = document.getElementById(elementID);
    const renderer = new Renderer(div, Renderer.Backends.SVG);

    // Configure the rendering context.
    renderer.resize(500, 200);
    const context = renderer.getContext();

    // Create a stave of width 400 at position 10, 40 on the canvas.
    const stave = new Stave(40, 40, 400);

    // Add a clef and time signature.
    stave.addClef(cleff).addTimeSignature(timeSignatureString);
    
    // Connect it to the rendering context and draw.
    stave.setContext(context).draw();
    document.getElementById(elementID).firstChild.style = "width: 70%; height: 70%"

    // Note array starts as the given list of note letters.
    const keyNames = noteNames;

    // Creates a single note (in time, can include lots of notes like a chord).
    const staveNote = new StaveNote({
        keys: keyNames,
        duration: noteDuration,
        clef: cleff
    });

    // Adds accidental to each note.
    for (let i = 0; i < noteAccidentals.length; i++) {

        if (noteAccidentals[i] != "none") {

            staveNote.addModifier(new Accidental(noteAccidentals[i]), i)
        }
    }

    // Adds note array containing all notes in bar.
    const notes = [staveNote];    

    // Create a voice and add above notes.
    const voice = new Voice({
        num_beats: timeSignatureTop,
        beat_value: timeSignatureBottom
    });
    voice.addTickables(notes);
    
    // Format and justify the notes to 350 pixels.
    new Formatter().joinVoices([voice]).format([voice], 350);
    
    // Render voice
    voice.draw(context, stave);    
}

function clearBox(elementID) {
    document.getElementById(elementID).innerHTML = "";
}