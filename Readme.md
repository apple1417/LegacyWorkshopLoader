# Legacy Workshop Loader
Enables loading SE2017 Talos Principle mods on legacy patches, as a workaround for the "Steam Workshop content update failed" error.

Note that if the mod is incompatible with these versions of the game this tool will not make it compatible, it will only make the game attempt to load it. You can check if this is the case by scrolling through the console.

![Image](https://i.imgur.com/jJpte04.png)

### Why do I need this?
This is to do with updates changing how mods are downloaded. Mods used to be downloaded into `[steamapps]\common\The Talos Principle\Temp\Workshop\Subscribed`, and the game would load then from there. However, Steam deprecated the API that was being used to do this. With the SE2017 update, they switched to the newer API, and the mod location changed to `[steamapps]\workshop\content\257510\[mod id]\`.

Now issues come from the fact that mods download into the folder for the version they were uploaded on, not the version you're running. SE2017 won't load legacy mods, and legacy patches won't load SE2017 mods.

Now there is (hopefully) a patch coming in the near future that will make SE2017 load legacy mods, but it of course won't help if you're already running a legacy patch. That's where this tool comes in. This tool lets you manually load any SE2017 mod you're subscribed to.
