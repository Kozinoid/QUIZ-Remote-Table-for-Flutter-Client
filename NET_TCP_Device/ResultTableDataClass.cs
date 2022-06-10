using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NET_TCP_Device
{
    public class ResultTableDataClass
    {
        public event EventHandler onTableChanged = null;
        //-------------------------------------------------------------------------------------------------------------------------------------
        private List<QUIZTeamDataViewClass> mTeamList = new List<QUIZTeamDataViewClass>();
        public QUIZTeamDataViewClass this[int index]
        {
            get { return mTeamList[index]; }
        }
        public int Count { get { return mTeamList.Count; } }

        //-------------------------------------------------------------------------------------------------------------------------------------
        public void ClearTabl()
        {
            mTeamList.Clear();
            if (onTableChanged != null) onTableChanged(this, new EventArgs());
        }

        public void AddNewTeam(string name, int score)
        {
            mTeamList.Add(new QUIZTeamDataViewClass(name, score));
            if (onTableChanged != null) onTableChanged(this, new EventArgs());
        }

        public void DeleteTeam(string name)
        {
            QUIZTeamDataViewClass team = FindByName(name);
            if (team != null) mTeamList.Remove(team);
            if (onTableChanged != null) onTableChanged(this, new EventArgs());
        }

        public void RefreshTeamScore(string name, int score)
        {
            QUIZTeamDataViewClass team = FindByName(name);
            if (team != null) team.TeamScore = score;
            if (onTableChanged != null) onTableChanged(this, new EventArgs());
        }

        public void RenameTeam(string oldName, string newName)
        {
            QUIZTeamDataViewClass team = FindByName(oldName);
            if (team != null) team.TeamName = newName;
            if (onTableChanged != null) onTableChanged(this, new EventArgs());
        }

        private QUIZTeamDataViewClass FindByName(string name)
        {
            QUIZTeamDataViewClass res = null;
            foreach(QUIZTeamDataViewClass team in mTeamList)
            {
                if (team.TeamName.Equals(name))
                {
                    res = team;
                    break;
                }
            }
            return res;
        }
    }

    public class QUIZTeamDataViewClass
    {
        private string teamName = "";
        private int teamScore = 0;
        private Color teamColor = Color.White;

        public string TeamName
        {
            get { return teamName; }
            set { teamName = value; }
        }

        public int TeamScore
        {
            get { return teamScore; }
            set { teamScore = value; }
        }

        public Color TeamColor
        {
            get { return teamColor; }
            set { teamColor = value; }
        }

        public QUIZTeamDataViewClass(string name, int score)
        {
            teamName = name;
            teamScore = score;
        }
    }
}
