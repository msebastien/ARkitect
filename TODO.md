Some rough ideas/plans about what to do next and potentially how to do it:
- ~~Implement Item bar~~
- ~~Load Item definitions from json files at runtime~~
- ~~Move items and building parts' materials, models and icons to the "Resources" folder~~
- ~~**Implement a StateManager for browsing UI pages.** It will use a "stack" data structure. Push a state to go to a new page, pop the current state to close the page. It is possible to add transition animations when pusing/popping a state. It will be easier than a State Machine as there won't be custom classes for each state. Only a StateManager and State classes are enough. Each page will have a "State" component. Pushing/Popping will be done using UI events, like clicking on a button.~~
- Implement Item library
- Implement Command Pattern to undo/redo actions
- **Use System.IO.Compression to uncompress ZIP archives** containing custom assets and items for the app. This is easier than using Unity's Asset Bundles as it does not require users to use the Unity Editor.