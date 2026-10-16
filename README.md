# TeamDeathmatch.Lite
This is a small LabAPI plugin for SCP:Secret Laboratory moderators to run Team Deathmatch events.

## Prequisites 
You'll need to install these aditional dependencies
- [LabAPI v1.1.7](https://github.com/northwood-studios/LabAPI)
- [SecretLabNAudio](https://github.com/Axwabo/SecretLabNAudio)
- [NAudio.Core](https://www.nuget.org/packages/NAudio.Core)

Put the mentioned libraries above inside the `dependencies` folder.

## How-to-use
You need to create a dummy with the name `TDM`, there should be the plugin controls located inside the Remote Admin `Dummy`'s section.

For information:
- The plugin only takes effect when it is Enabled through the RA Dummy control
- You don't need to disable it manually when the round is going to end / restart
- Ongoing Round is handled by yourself, you should `RoundLock` when doing this kind of event
- There's plenty utilities ready for use to make ease decisions
