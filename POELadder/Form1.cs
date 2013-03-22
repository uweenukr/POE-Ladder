﻿using System;
using System.Windows.Forms;
using System.Linq;
using PoELadder.JSON;
using PoELadder;
using System.Collections.Generic;

namespace POELadder
{
    public partial class Form1 : Form
    {
        public String LadderAllURL = "http://api.pathofexile.com/leagues";
        public String LadderSingleURL;
        public String selectedLadder;

        PathOfExileJSONLadderAll[] POELadderAll;

        public int SelectedLadderIndex;

        public List<PlayerDB> playerDB = new List<PlayerDB>();

        public Form1()
        {
            InitializeComponent();
        }

        #region GUI Code
    //Form Driven Methods:

        private void Form1_Load(object sender, EventArgs e)
        {
            POELadderAll = JSON.ParseLadderAll(LadderAllURL);

            //Populate the Ladder Drop Down
            for (int i = 0; i < POELadderAll.Length; i++)
            {
                ladderselectBox.Items.Add(POELadderAll[i].id);
            }
            timer2.Enabled = true;
        }

        //A new Ladder has been Selected from the Select Box.
        private void ladderselectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedLadderIndex = ladderselectBox.SelectedIndex;

            //Sets the URL to populate the table
            LadderSingleURL = "http://api.pathofexile.com/ladders/" + ladderselectBox.Text.Replace(" ", "%20") + "?limit=200";

            if (playerDB.Count > 2)
            {
                playerDB.Clear();
            }

            UpdateLadderData();
            UpdateLadderTable();
        }

        //Ladder-auto Refresh - 15 seconds currently
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Auto-refresh
            if (checkBox1.Checked == true)
            {
                UpdateLadderData();
                UpdateLadderTable();
            }
        }

        //Enable or disable the timer on checkbox change
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (ladderselectBox.SelectedItem != null)
            {
                UpdateTimer();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ladderselectBox.SelectedItem != null)
            {
                UpdateLadderData();
                UpdateLadderTable();
            }
        }

    //Custom Methods:

        //Update Table
        private void UpdateLadderTable()
        {
            //Add the Ladder JSON Data to the Player Objects to be displayed in the Ladder Table
            var arrPlayers = new PlayerTable[playerDB.Count];
            for (int i = 0; i < playerDB.Count; i++)
            {
                arrPlayers[i] = new PlayerTable
                {
                    Online = playerDB[i].GetOnlineStatus(),
                    Rank = playerDB[i].GetRank(),
                    Account = playerDB[i].GetAccount(),
                    Chracter = playerDB[i].GetCharacter(),
                    CharacterClass = playerDB[i].GetClass(),
                    Level = playerDB[i].GetLevel(),
                    EXP = playerDB[i].GetExperience(),
                    EXPToNextLevel = playerDB[i].GetEXPToNextLevel(),
                    EXPBehindLeader = playerDB[i].GetEXPBehindLeader(),
                    EXPThisUpdate = playerDB[i].GetEXPThisUpdate(),
                    EST_EXP_Minute = playerDB[i].GetEST_EXP_Minute(),
                    RankChange = playerDB[i].GetRankChange()
                };
            };

            //Apply the ladder data to the Data Grid View
            LadderTable.DataSource = arrPlayers;

            for (int i = 0; i < playerDB.Count; i++)
            {
                if (LadderTable.Rows[i].Cells[4].Equals("Marauder"))
                {
                    LadderTable.Rows[i].Cells[4].Value = "Hey";
                }

            }

            #region LadderTable Formatting
            LadderTable.Columns[0].HeaderText = "Online:";
            LadderTable.Columns[1].HeaderText = "Rank:";
            LadderTable.Columns[2].HeaderText = "Account:";
            LadderTable.Columns[3].HeaderText = "Chracter:";
            LadderTable.Columns[4].HeaderText = "Class:";
            LadderTable.Columns[5].HeaderText = "Level:";
            LadderTable.Columns[6].HeaderText = "Experience:";
            LadderTable.Columns[7].HeaderText = "EXP To Level:";
            LadderTable.Columns[7].ToolTipText = "Experience required to level";
            LadderTable.Columns[8].HeaderText = "EXP/behind:";
            LadderTable.Columns[8].ToolTipText = "Experience behind the leader";
            LadderTable.Columns[9].HeaderText = "EXP/update:";
            LadderTable.Columns[9].ToolTipText = "Experience gained this update";
            LadderTable.Columns[10].HeaderText = "EXP/Minute:";
            LadderTable.Columns[10].ToolTipText = "Estimation of experience gained per minute";
            LadderTable.Columns[11].HeaderText = "Change:";
            LadderTable.Columns[11].ToolTipText = "Change in rank since the last update";
            #endregion
        }

        //Update Ladder Table with the current DB data
        private void UpdateLadderData()
        {
            PathOfExileJSONLadderSingle LadderData = JSON.ParseLadderSingle(LadderSingleURL);

            //Add the Ladder JSON Data to the PlayerDB
            for (int i = 0; i < LadderData.entries.Count; i++)
            {
                //First setup. Not all players added
                if (playerDB.Count < LadderData.entries.Count)
                {
                    PlayerDB NewPlayer = new PlayerDB(
                        LadderData.entries[i].account.name,
                        LadderData.entries[i].character.name,
                        LadderData.entries[i].character.@class);

                    playerDB.Add(NewPlayer);
                }

                for (int j = 0; j < playerDB.Count; j++)
                {
                    
//This should be called when a player who was not part of the first download is now on the ladder.


//                    //New Player found
//                    if (!LadderData.entries[j].account.name.Equals(playerDB[j].GetAccount()) &&
//                        !LadderData.entries[j].character.name.Equals(playerDB[j].GetCharacter()))
//                    {
////This for should probably be change from J J for I J. But it crashes when that happens.

//                        PlayerDB NewPlayer = new PlayerDB(
//                            LadderData.entries[i].account.name, 
//                            LadderData.entries[i].character.name, 
//                            LadderData.entries[i].character.@class);

//                        playerDB.Add(NewPlayer);
//                    }

                    //Player already exist. Update with current information.
                    if (LadderData.entries[i].account.name.Equals(playerDB[j].GetAccount()) &&
                        LadderData.entries[i].character.name.Equals(playerDB[j].GetCharacter()))
                    {
                        playerDB[j].Update(
                            LadderData.entries[i].online,
                            LadderData.entries[i].rank,
                            LadderData.entries[i].character.level,
                            LadderData.entries[i].character.experience,
                            DateTime.UtcNow,
                            LadderData.entries[i].character.experience);
                    }
                }
            }

            //Sort the list:
            playerDB = playerDB.OrderBy(q => q.GetRank()).ToList();
        }

        //Update the Race Timer with the Time Left before the End of the Race
        private void UpdateTimer()
        {
            //If Time is past the start time and less than the end time
            DateTime StartTime = Clock.FormatPOEDate(POELadderAll[SelectedLadderIndex].startAt);
            DateTime EndTime = Clock.FormatPOEDate(POELadderAll[SelectedLadderIndex].endAt);

            DateTime localTime = DateTime.UtcNow;
            DateTime localDate = DateTime.Today;
            String beforeRace = (StartTime - localTime).ToString();
            String duringRace = (EndTime - localTime).ToString();

            //If the race has started but has not ended. Currently running.
            if (localTime > StartTime && localTime < EndTime)
            {
                timerLabel.Text = String.Format(duringRace, "hh:mm:ss").Substring(0, duringRace.LastIndexOf("."));
            }

            else
            {
                //The race has not started
                if (localTime < StartTime)
                {
                    timerLabel.Text = "Starts in " + String.Format(beforeRace, "{0:hh:mm:ss}").Substring(0, beforeRace.LastIndexOf("."));
                }

                //The current ladder has no ending (Perminate leagues)
                else if (EndTime == DateTime.MinValue)
                {
                    timerLabel.Text = "00:00:00";
                }
            }
        }
        #endregion
    }
}
