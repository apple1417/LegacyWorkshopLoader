# Legacy Workshop Loader
Enables loading SE2017 Talos Principle mods on legacy patches, as a workaround for the "Steam Workshop content update failed" error.

Note that if the mod is incompatible with these versions of the game this tool will not make it compatible, it will only make the game attempt to load it. You can check if this is the case by scrolling through the console.

![Image](https://i.imgur.com/2i7bv9I.png)

Runs on Linux through mono. Untested on Mac, but probably also works through mono.

### How to use it
1. Subscribe to all mods you want to install.
2. Launch the tool. Only the mods that cause issues will be displayed.
3. Enable all the mods you want in the tool.
4. Unsubscribe from all mods the tool displays.
5. Launch the game.

To update mods:
1. Re-subscribe to the mods you want to update.
2. Launch the tool.
3. Unsubscribe from all mods the tool displays again.
4. Launch the game.

### Why do I need this?
This is to do with updates changing how mods are downloaded. Mods used to be downloaded into:    
`[steamapps]\common\The Talos Principle\Temp\Workshop\Subscribed\`    
However, Steam deprecated the API that was being used to do this. With the SE2017 update, they switched to the newer API, and the mod location changed to:    
`[steamapps]\workshop\content\257510\[mod id]\`.

Now issues come from the fact that mods download into the folder for the version they were uploaded on, not the version you're running. SE2017 won't load legacy mods, and legacy patches won't load SE2017 mods.

Now there is (hopefully) a patch coming in the near future that will make SE2017 load legacy mods, but it of course won't help if you're already running a legacy patch. That's where this tool comes in. This tool lets you manually load any SE2017 mod you're subscribed to.

### How does it work?
Turns out there is another directory the game will load files from:    
`[steamapps]\common\The Talos Principle\Content\Talos\`    
This folder contains the default files for the main campagins.

This tool caches your subscribed workshop files, and then copies them to this folder (or rather symlinks them) as requested. This then leaves you free to unsubscribe from these mods, which will fix the errors.
