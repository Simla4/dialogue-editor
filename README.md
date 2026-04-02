# Dialogue System for Unity

A modular and extensible **branching dialogue tool** built in Unity using **Custom Editors** and **ScriptableObjects**. Designed for narrative-driven games and prototyping, this system emphasizes **clean architecture**, reusability, and editor usability.


https://github.com/user-attachments/assets/1d272c2c-0b72-4af8-a610-844042189968

![DialogueNodes](https://github.com/user-attachments/assets/897bf02f-2470-417a-b5a3-a05adadcd359)

---

##  Technologies Used

- Unity (Editor scripting, 2D UI)
- C#
- ScriptableObjects
- Event Bus architecture
- Generic programming

---

##  Features

-  Branching dialogue with player choices
-  Visual Editor for creating dialogue trees
-  ScriptableObject-based data architecture
-  Decoupled logic using an Event Bus
-  Easy integration into any Unity project
-  Designed to be reusable and scalable

---

## Folder Structure

Assets/
│

├── DialogueSystem/

├── Editor/ # Custom editor tools

├── Runtime/ # Dialogue execution scripts

├── Data/ # ScriptableObject types

└── UI/ # Dialogue UI prefab

└── Examples/

└── ExampleScene/ # Sample scene for demonstration


---

##  How to Use

1. Clone or download the repository.
2. Open the project in Unity.
3. Navigate to `Tools → Dialogue System → Graph Editor` to open the editor window.
4. Create a new Dialogue asset via `Create → Dialogue System → Dialogue`.
5. Use the editor to add dialogue nodes and player choices.
6. Connect nodes to define flow logic.
7. Save the asset and test the system in the runtime example scene.

---

## Example Usage (Runtime)

```csharp
public class DialogueStarter : DialogueMonoBehaviour
{
    void StartDialogue()
    {
        RunDialogue(myDialogueAsset);
    }

    public override void OnChoiceSelected(int choiceIndex)
    {
        // Custom logic based on player selection
    }
}
```


---

## What I Learned
- Editor scripting for designer-friendly tools

- Decoupled system architecture using event-driven design

- Advanced usage of ScriptableObjects for modularity

- Building reusable, extensible dialogue tools

- Balancing UX and developer experience in custom tool creation
