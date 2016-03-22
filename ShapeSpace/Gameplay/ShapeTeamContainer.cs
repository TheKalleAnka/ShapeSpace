using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ShapeSpace.Network;

namespace ShapeSpace.Gameplay
{
    public class ShapeTeamContainer
    {
        ShapeTeam team = ShapeTeam.UNKNOWN;
        List<int> playersOnTeam = new List<int>();
        int playerWhoIsBank = -1;

        public Vector2 basePosition { get; private set; }

        public ShapeTeamContainer(ShapeTeam team)
        {
            this.team = team;

            if (GetTeam() == ShapeTeam.GREEN)
                basePosition = new Vector2(-1000, 0);
            else
                basePosition = new Vector2(1000, 0);
        }

        /// <summary>
        /// Adds the player to the team
        /// </summary>
        /// <param name="index">The index the player who is added has on the server</param>
        /// <returns>True if the player is the bank, otherwise false</returns>
        public bool AddPlayer(int index)
        {
            playersOnTeam.Add(index);

            if(playerWhoIsBank == -1)
            {
                playerWhoIsBank = index;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a player from the team
        /// </summary>
        /// <param name="index">The index the player who is added has on the server</param>
        public void RemovePlayer(int index)
        {
            playersOnTeam.Remove(index);

            if (playerWhoIsBank == index)
                playerWhoIsBank = -1;

            return;
        }

        public int GetNumberOfMembers()
        {
            return playersOnTeam.Count;
        }

        public ShapeTeam GetTeam()
        {
            return team;
        }
    }
}
