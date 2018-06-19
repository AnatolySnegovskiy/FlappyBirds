// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// Handler for UI buttons on the scene.  Also performs some
// necessary setup (initializing the firebase app, etc) on
// startup.
public class UIHandler : MonoBehaviour
{
    static public UIHandler FireBase;
    ArrayList leaderBoard;
    Vector2 scrollPosition = Vector2.zero;
    private Vector2 controlsScrollViewVector = Vector2.zero;

    public GUISkin fb_GUISkin;
    public List<object> leaders_board = new List<object>();
    private const int MaxScores = 5;
    private string logText = "";
    private string email = "";
    private int score = 100;
    private Vector2 scrollViewVector = Vector2.zero;
    bool UIEnabled = true;
    long childScore;
    String nikname;
    const int kMaxLogSize = 16382;
    Font ArialFont;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    private void Awake()
    {
        FireBase = this;
    }


    void Start()
    {

        ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        
        dependencyStatus = FirebaseApp.CheckDependencies();
        if (dependencyStatus != DependencyStatus.Available)
        {
            FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = FirebaseApp.CheckDependencies();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        else
        {
            InitializeFirebase();
        }
    }

    // Initialize the Firebase database:
    void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://flappy-birds-testing.firebaseio.com/");
        if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);

        leaderBoard = new ArrayList();
        leaderBoard.Add("Firebase Top " + MaxScores.ToString() + " Scores");
        FirebaseDatabase.DefaultInstance.GetReference("Leaders").OrderByChild("score").ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
        {
              if (e2.DatabaseError != null)
              {
                  Debug.LogError(e2.DatabaseError.Message);
                  return;
              }
              string title = leaderBoard[0].ToString();
              leaderBoard.Clear();
              leaderBoard.Add(title);
              if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
              {
                  foreach (var childSnapshot in e2.Snapshot.Children)
                  {
                      if (childSnapshot.Child("score") == null
                    || childSnapshot.Child("score").Value == null)
                      {
                          Debug.LogError("Bad data in sample.  Did you forget to call SetEditorDatabaseUrl with your project id?");
                          break;
                      }
                      else
                      {
                          Dictionary<string, object> newScoreMap = new Dictionary<string, object>();


                          newScoreMap["score"] = childSnapshot.Child("score").Value;
                          newScoreMap["email"] = childSnapshot.Child("email").Value;

                          leaders_board.Add(newScoreMap);

               
                         

                          leaderBoard.Insert(1, childSnapshot.Child("score").Value.ToString()
                        + "  " + childSnapshot.Child("email").Value.ToString());
                      }
                  }
              }

          };

      
        
    }



    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);
        logText += s + "\n";

        while (logText.Length > kMaxLogSize)
        {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }

        scrollViewVector.y = int.MaxValue;
    }

    // A realtime database transaction receives MutableData which can be modified
    // and returns a TransactionResult which is either TransactionResult.Success(data) with
    // modified data or TransactionResult.Abort() which stops the transaction with no changes.
    TransactionResult AddScoreTransaction(MutableData mutableData)
    {
        List<object> leaders = mutableData.Value as List<object>;

        if (leaders == null)
        {
            leaders = new List<object>();
        }

        nikname = null;
        childScore = 0;
        long max_score_base=0;
        for (int i = 0; i < leaders.Count; i++)
        {
            childScore = (long)((Dictionary<string, object>)leaders[i])["score"];

            nikname = (String)((Dictionary<string, object>)leaders[i])["email"];

            if (childScore < score && email== nikname)
            {
                leaders.RemoveAt(i);
            }
            if( max_score_base < childScore)
            {
                max_score_base = childScore;
            }
        }



    // Now we add the new score as a new entry that contains the email address and score.



        if(max_score_base < score)
        {
            Dictionary<string, object> newScoreMap = new Dictionary<string, object>();

            
            newScoreMap["score"] = score;
            newScoreMap["email"] = email;


            leaders.Add(newScoreMap);

            // You must set the Value to indicate data at that location has changed.
            mutableData.Value = leaders;
        }

        return TransactionResult.Success(mutableData);
    }

public void AddScore(string emails, int scores)
{
    score = scores;
    email = emails;

    if (score == 0 || string.IsNullOrEmpty(email))
    {
        DebugLog("invalid score or email.");
        return;
    }

    DebugLog(String.Format("Attempting to add score {0} {1}", email, score.ToString()));

    DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("Leaders");

    DebugLog("Running Transaction...");
    // Use a transaction to ensure that we do not encounter issues with
    // simultaneous updates that otherwise might create more than MaxScores top scores.
    reference.RunTransaction(AddScoreTransaction)
      .ContinueWith(task =>
      {
          if (task.Exception != null)
          {
              DebugLog(task.Exception.ToString());
          }
          else if (task.IsCompleted)
          {
              DebugLog("Transaction complete.");
          }
      });
}

}
