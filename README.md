# Badadoom: Yet another esoteric programming language

#### Intro ####
##### Basics #####

**Badadoom** – dialect of the Brainfuck esoteric language, which uses MIDI files as source codes.

##### Application Area #####

The Badadoom, like the original Brainfuck, is theoretically Turing-complete (assuming an infinite set of cells for the interpreter).

Possible application areas:

 - steganography - the source code of the program can be included in a regular .mid audio file without the possibility to hear changes while listening
 - just for lulz

#### Syntax ####
##### Command Set #####

The code of the program in the .mid file is placed on the tenth (in the case of the beginning of the numbering from 1) channel, which is reserved by the MIDI standard for the percussion (hence the name of the dialect).

As a command the dialect uses the note start event from the MIDI file (hereinafter - NoteOn).

From the  percussion instruments, defined by General MIDI standard, the following commands were selected for the commands:

| Percussion           | Description                                                                                                                                       |
|-------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------|
| 40 Snare Drum 2   | increment the data pointer (to point to the next cell to the right)  .                                                                                                                           |
| 43 Low Tom 1      | decrement the data pointer (to point to the next cell to the left).                                                                                                                            |
| 35 Bass Drum 2    | increment (increase by one) the byte at the data pointer.|
| 51 Ride Cymbal 1  | decrement (decrease by one) the byte at the data pointer.                                                                                                               |
| 44 Pedal Hi-hat   | output the byte at the data pointer.                                                                                                                  |
| 55 Splash Cymbal  | accept one byte of input, storing its value in the byte at the data pointer.                                                                                                     |
| 49 Crash Cymbal 1 | if the byte at the data pointer is zero, then instead of moving the instruction pointer forward to the next command, jump it forward to the command after the matching '52 Chinese Cymbal' command. |
| 52 Chinese Cymbal | if the byte at the data pointer is nonzero, then instead of moving the instruction pointer forward to the next command, jump it back to the command after the matching '49 Crash Cymbal 1' command.                             |

##### Commands Match Table for Badadoom & Brainfuck #####

| Badadoom          | Brainfuck                                                                                                                  |
|-------------------|----------------------------------------------------------------------------------------------------------------------------|
| 40 Snare Drum 2   | >                                                                                                                          |
| 43 Low Tom 1      | <                                                                                                                          |
| 35 Bass Drum 2    | +                                                                                                                          |
| 51 Ride Cymbal 1  | -                                                                                                                          |
| 44 Pedal Hi-hat   | .                                                                                                                          |
| 55 Splash Cymbal  | ,                                                                                                                          |
| 49 Crash Cymbal 1 | [                                                                                                                          |
| 52 Chinese Cymbal | ]																															 |

##### Restrictions on NoteOn #####

The main limitation when placing events on the channel is an unique absolute start time for each. The interpreter executes commands, sorting them out by this indicator. Therefore, in the case of the equality of absolute time, there can be an undefined behavior of the program.

There are no restrictions on other parameters of event. Also there are no restrictions on information on other channels of the .mid file.

#### Repository Content ####

**Badadoom** - interpreter.

**Badadoom to BF** - converter for Badadoom midi in Brainfuck source code.

**BF to Badadoom** - converter for Brainfuck source code to Badadoom midi.

**Examples** - examples of pure Badadoom midi files.