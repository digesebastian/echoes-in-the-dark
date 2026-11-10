Developer setup notes:

1. Go to folder where you want the project folder to be created. Open Git Bash in this folder
	
2. Set Up Git LFS (Large File Storage). In Git bash:

```git lfs install```


3. clone project. In Git bash:

```git clone https://github.com/digesebastian/echoes-in-the-dark.git```

4. Configure Unity for Version Control

    Open EchoesInTheDarkSource as a project in Unity and adjust the following settings:

    Asset Serialization: Go to Edit > Project Settings > Editor. Set Asset Serialization Mode to Force Text. This keeps .unity (scenes) and .prefab files in YAML format, allowing you to view line-by-line diffs.

    Visible Meta Files: In the same menu, verify Version Control Mode is set to Visible Meta Files.
	
