# **Offline Ranking Table: Specialism Brief 3**

## *Introduction*

This component provide a way to *Save* and *Load* an offline ranking table for an arcade game. It is specifically designed to answer a set brief, hence some values are hard-coded, such as the number of *level* (60) and the number of *scores* per level (5).

The **Save** button creates a persistent file on the running system, which can then be loaded with the **Load** button.

On first run, each level has a randomly generated highscore, using a randomly generated *standard deviation*.

## *Behaviour*

[`GeneratingScores`]

[`SaveToFile`]

[`ManipulatingScores`]

## *GeneratingScores*

This *script* is responsible for generating a new highscore board if the program first run, ordering the scores in descending order per level, and saving & loading. It contains three parameters, **Initials**, **Saved Scores** and **Saved Names**

### Initials

A list of initials needs to be provided before running the program, at least one entry. There is no limit on the list of initials. The generated scores will pick randomly in this list to associate a name on first run.

### Saved Scores

This list will be randomly generated on first run and does not need to be change before first run. This will contains all the save scores across all levels. With 5 scores per level and 60 levels, this list will be have 300 entries.

### Saved Names

Similar to *Saved Scores*, this list will contains the names, using the same indexes a the scores for association. It needs to be left empty at first run.
*Note*: Both lists (Saved Scores and Saved Names) should have the `[HideInInspector]` attribute so they are not manually changed by mistake. For the purpose of this demo, they are visible in the inspector to debug the predicted results.

## SaveToFile

This is a *Serialize class* that will be **Serialized** when saving, saved in a file on the system drive, and **deserialized** when loading, to read the informations it contained. This contains only the full lists of *highscores* and *names*.

## ManipulatingScores

This is an example script designed to work with Unity UI in order to show the result on screen. It contains references to UI Texts, methods to show the selected levels score, and the buttons call to save and load. It is also responsible for the entry of new scores durin runtime.

## How To Use

On first run, add a list of initials to the *GeneratingScores* behaviour, then press Play.

Click on the levels number on top to show the generated score for this level.

To add a new score, use the bottom right entry. *Initials* is not limited to 3 letters in this demo, any string can be entered. Enter a score (needs to be an *Integer*) and press Validate. If the score is valid and at least higher than the 5th score, it will appear on the score and update the highscore.

At this point, you can save the project with the *Save*  button. if you press *Stop* and *Play* again, the game will try to automatically load the saved file if one is detected. You should be able to see the new score you have entered.

To try the *Load* option, add another new highscore, and this time press *Load* button. The table will resume it's state before the added highscore.

## Note

The save file is created at `PersistentFolder/saves/RankingTable.save`, where `PersistentFolder` is a path to a persistent data directory, where you can store data that you want to be kept between runs. On Windows, it usually is `%userProfile%\AppData\LocalLow\%companyName%\%applicationName%\`.

[`GeneratingScores`]: #GeneratingScores
[`SaveToFile`]: #SaveToFile
[`ManipulatingScores`]: #ManipulatingScores
