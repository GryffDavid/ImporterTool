# 24 Bit Games Asset Importer Tool

Usage:

 - Right Click > Create > Import Settings to create an import settings object in a folder. These settings will apply to the folder it's contained in and any child folders that don't have settings.
- To apply settings manually, right click on or in a folder and then select "Apply Import Settings" from the context menu. You can also manually apply the settings to just one asset (or a combination of assets and folders) with the same function. 
- Settings are automatically applied to newly imported assets, if relevant settings are found.
- If no relevant settings are found, the debug log will provide a message. 
- If multiple settings objects are found in a single folder, only the first settings found will be applied and there will be a message warning the user where the multiple settings were found.

Open the SampleScene with the cube to see how the settings are affected. The cube has Assets/Textures/TestTexture4k applied already.

## Programming Notes
I included access to the texture filter mode to replicate the functionality of the default texture inspector. The default inspector only allows the user to override the anisotropic filter level if the filter mode is not set to "point". I didn't want the user to try and override the anisotropic filtering level for a bunch of assets set to Point filtering and then be confused about why the settings aren't working.

I included the ability to disable or enable the texture or audio settings overrides so that if, for some reason, the user placed a configuration file somewhere that would affect textures and audio and only wanted to apply the audio settings or only wanted to apply the texture settings they have that option. If both audio and texture settings are disabled, the user will receive a warning in the console.

I did consider splitting the audio and texture settings into two separate objects ImportSettingsAudio and ImportSettingsTexture (Or similarly named) that would derive from the base ImportSettings class. But I felt that may be a bit excessive for the small handful of settings that are being changed. Perhaps if the tool was supposed to override all of the texture settings or if the tool had to work on more types of assets that would be a better approach. It would be easy enough to do for the existing framework, if necessary.

I created the ImportEditorLayout so that the functionality of the settings editor would better match what the user would get by default in the texture and audio import settings - visual feedback about the anisotropic filtering level only being applied if the filter mode is not set to point etc. While this does make the functionality of the tool more user-friendly, it does add to the time needed to extend the settings the tool can apply. The use case for this really kind of depends on if this is a tool that will be used internally by people who can be trusted to know how the settings work or if this is a tool that will be sent out into the wild or to a client. It would be very easy to disable the custom layout and just use the default.

The ApplySettings function handles selected files and folders differently. This accounts for if the user makes a selection like this:

![Multiple files and folders selected](https://raw.githubusercontent.com/GryffDavid/READMEImages/master/ImportTool/SelectedFiles.png)

If a folder is selected the script will find the relevant settings for that folder and apply them to everything inside. If a file is selected, it will check if the relevant settings already exist is memory and will apply them if they do, or find new ones if necessary. I did it this way because the Selection.GetFiltered function returns the files and folders out of order. It doesn't retrieve them in any sort of directory hierarchy order:

![Asset hierarchy returned out of order](https://raw.githubusercontent.com/GryffDavid/READMEImages/master/ImportTool/OutOfOrder.png)

Notice how the asset C3PO_WithEyes.jpg is at index 9 despite the asset TestTexture4k.png being in exactly the same folder but being at index number 1. What I wanted to avoid was having to be continually jumping up and down the folder hierarchy and performing a search for settings each time. So instead, settings for the individually selected files are stored in CurrentSettings and don't change unless the directory the current selected file is stored in changes. The selected folders and then handled separately, with the settings being batch applied to the whole folder.

This is the same reason I used the HashSet to get a list of unique folders when applying the settings to a folder. I can't trust the Selection to return anything in a hierarchy order.

When importing multiple files simultaneously, the settings for each new asset are located and then applied. I attempted to get around the need for settings to be located for each new asset and instead finding the settings once and then applying them to all new assets, but I couldn't find a way to do this. The ImportScript is run separately for each asset as it's imported.
