# stateful #

Stateful is a game state management and input library for MonoGame and XNA. It
grew out of my frustration with the Game State Management sample that everybody
and their brother was using in their XNA projects, and when I started to move
towards using MonoGame, I brought it along with me.

Stateful is licensed under the ISC license, under LICENSE.md in this repo.

Contact: Ed Ropple (ed+stateful@edropple.com)

## Current Status ##
Currently, Stateful is essentially feature-complete **for XNA on Windows and
MonoGame on Windows only**. It shouldn't be too hard to create MacOS and Linux
ports on a similar basis; similar code should work for the 360 if you want to
build a project for it.

Eventually (read: when I need it), I'll be adding gesture and touch support for
MonoGame. That should probably work with XNA on Windows Phone 7, too, but I
don't have any interest in supporting that.

## Using Stateful ##
Stateful is a pretty simple core library--just about anybody could do what I've
done here, it's nothing special. (The reason I open-sourced it was in part to
encourage myself to write more modular code, which at times flies in the face
of established practice in the XNA and MonoGame communities--but that's another
story.) There are only a couple of parts you really need to concern yourself
with: **IInputEventer**, **StateManager**, and **Controller**.

*IInputManager* is an interface that defines the input events to which
StateManager subscribes (the events of which it delegates execution to the
Controllers). You can implement this yourself in whatever manner you'd like,
but there's already a *DesktopInputManager* available under MonoGame. It's a
simple polling object and using it will require you to insert a call to the
DesktopInputManager.Update() method into your game loop before any calls to
StateManager.

*StateManager* wrangles the various controllers that you'll use to express
segments of game logic. It's modeled after a stack, and you use push and pop
operations upon it. StateManager includes a small number of events that you can
hook for custom behavior; they're documented in-code. Of particular note should
be OnEmpty, which will be called when you pop your last controller. Stateful
won't close down your application after this - that's still your job. If you
don't implement OnEmpty, it'll throw StatefulExitException. If you don't catch
that, your application will crash and look dumb. Don't look dumb: implement
OnEmpty.

*Controller* is the unit of state for the project. It's an abstract class with
a number of hopefully-obvious implementation members; the required ones can
almost all be just left blank at your discretion, but that would make for a
pretty lame game.