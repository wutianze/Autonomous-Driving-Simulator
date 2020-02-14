﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;
public class SceneLoader : MonoBehaviour {

public void LoadGenerateRoadScene()
{
    SceneManager.LoadSceneAsync(1);
}

public void LoadWarehouseScene()
{
    SceneManager.LoadSceneAsync(2);
}

public void LoadAVCScene()
{
    SceneManager.LoadSceneAsync(3);
}

public void LoadMenuScene()
{
    SceneManager.LoadSceneAsync(0);
}

public void LoadGeneratedTrackScene()
{
    SceneManager.LoadSceneAsync(4);
}

public void QuitApplication()
{
    Application.Quit();
}

public void SetLogDir()
{
    // Show a select folder dialog 
    // onSuccess event: print the selected folder's path
    // onCancel event: print "Canceled"
    // Load file/folder: folder, Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
     FileBrowser.ShowLoadDialog( (path) => { OnSetLogDir(path); }, 
                                    () => { Debug.Log( "Canceled" ); }, 
                                    true, null, "Select Log Folder", "Select" );
}

public void OnSetLogDir(string path)
{
    Debug.Log( "Selected: " + path );
    GlobalState.log_path = path;
}

public void SetScriptFile()
{
    // Show a select folder dialog 
    // onSuccess event: print the selected folder's path
    // onCancel event: print "Canceled"
    // Load file/folder: folder, Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
     FileBrowser.ShowLoadDialog( (path) => { OnSetScriptFile(path); }, 
                                    () => { Debug.Log( "Canceled" ); }, 
                                    false, null, "Select Script File", "Select" );
}

public void OnSetScriptFile(string path)
{
    Debug.Log( "Selected: " + path );
    GlobalState.script_path = path;
}

}
