#region License

// Copyright (c) 2005-2013, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace LoginEngine.Packets
{
    #region Usings ...

    using System;
    using System.Data;
    using System.Text;

    using AO.Core;
    using AO.Core.Logger;
    using AO.Database;

    using SmokeLounge.AOtomation.Messaging.GameData;

    #endregion

    /// <summary>
    /// </summary>
    public class CharacterName
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        private static string mandatoryVowel = "aiueo"; /* 5 chars */

        /// <summary>
        /// </summary>
        private static string optionalOrdCon = "vybcfghjqktdnpmrlws"; /* 19 chars */

        /// <summary>
        /// </summary>
        private static string optionalOrdEnd = "nmrlstyzx"; /* 9 chars */

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// </summary>
        public int Breed { get; set; }

        /// <summary>
        /// </summary>
        public int Fatness { get; set; }

        /// <summary>
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// </summary>
        public int HeadMesh { get; set; }

        /// <summary>
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// </summary>
        public int MonsterScale { get; set; }

        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public int Profession { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        private int[] Abis { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public int CheckAgainstDatabase()
        {
            /* name in use */
            if (CharacterDao.CharExists(this.Name) > 0)
            {
                return 0;
            }

            return this.CreateNewChar();
        }

        /// <summary>
        /// </summary>
        /// <param name="charid">
        /// </param>
        public void DeleteChar(int charid)
        {
            var ms = new SqlWrapper();

            try
            {
                /* delete char */
                /* i assume there should be somewhere a flag, caus FC can reenable a deleted char.. */
                string sqlQuery = "DELETE FROM `characters` WHERE ID = " + charid;
                ms.SqlDelete(sqlQuery);
                StatDao.DeleteStats(50000, charid);

                sqlQuery = "DELETE FROM `organizations` WHERE ID = " + charid;
                ms.SqlDelete(sqlQuery);
                sqlQuery = "DELETE FROM `inventory` WHERE ID = " + charid;
                ms.SqlDelete(sqlQuery);
            }
            catch (Exception e)
            {
                Console.WriteLine(this.Name + e.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="profession">
        /// </param>
        /// <returns>
        /// </returns>
        public string GetRandomName(Profession profession)
        {
            var random = new Random();
            byte randomNameLength = 0;
            var randomLength = (byte)random.Next(3, 8);
            var sb = new StringBuilder();
            while (randomNameLength <= randomLength)
            {
                if (random.Next(14) > 4)
                {
                    sb.Append(optionalOrdCon.Substring(random.Next(0, 18), 1));
                    randomNameLength++;
                }

                sb.Append(mandatoryVowel.Substring(random.Next(0, 4), 1));
                randomNameLength++;

                if (random.Next(14) <= 4)
                {
                    continue;
                }

                sb.Append(optionalOrdEnd.Substring(random.Next(0, 8), 1));
                randomNameLength++;
            }

            string name = sb.ToString();
            name = char.ToUpper(name[0]) + name.Substring(1);
            return name;
        }

        /// <summary>
        /// </summary>
        /// <param name="startInSL">
        /// </param>
        /// <param name="charid">
        /// </param>
        public void SendNameToStartPlayfield(bool startInSL, int charid)
        {
            DBCharacter dbCharacter=new DBCharacter{Id=charid,
            Playfield=4001,X=850,Y=43,Z=565};
            if (!startInSL)
            {
                dbCharacter.Playfield = 4582;
                dbCharacter.X = 939;
                dbCharacter.Y = 20;
                dbCharacter.Z = 732;
            }
            CharacterDao.UpdatePosition(dbCharacter);
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private int CreateNewChar()
        {
            int charID = 0;
            switch (this.Breed)
            {
                case 0x1: /* solitus */
                    this.Abis = new[] { 6, 6, 6, 6, 6, 6 };
                    break;
                case 0x2: /* opifex */
                    this.Abis = new[] { 3, 3, 10, 6, 6, 15 };
                    break;
                case 0x3: /* nanomage */
                    this.Abis = new[] { 3, 10, 6, 15, 3, 3 };
                    break;
                case 0x4: /* atrox */
                    this.Abis = new[] { 15, 3, 3, 3, 10, 6 };
                    break;
                default:
                    Console.WriteLine("unknown breed: ", this.Breed);
                    break;
            }

            /*
             * Note, all default values are not specified here as defaults are handled
             * in the CharacterStats Class for us automatically. Also minimises SQL
             * usage for default stats that are never changed from their default value
             *           ~NV
             */
            // Delete orphaned stats for charID
            StatDao.DeleteStats(50000, charID);
            try
            {
                CharacterDao.AddCharacter(new DBCharacter
                                              {
                                                  FirstName = "",
                                                  LastName = "",
                                                  Name = this.Name,
                                                  Username = this.AccountName,
                                              });
            }
            catch (Exception e)
            {
                LogUtil.ErrorException(e);
                return 0;
            }

            try
            {
                /* select new char id */
                    charID = CharacterDao.GetByCharName(this.Name).Id;
            }
            catch (Exception e)
            {
                LogUtil.ErrorException(e);
                return 0;
            }

            // Flags
            StatDao.AddStat(50000, charID, 0, 20);
            // Level
            StatDao.AddStat(50000, charID, 54, 1);
            // SEXXX
            StatDao.AddStat(50000, charID, 59, this.Gender);
            // Headmesh
            StatDao.AddStat(50000, charID, 64, this.HeadMesh);
            // MonsterScale
            StatDao.AddStat(50000, charID, 360, this.MonsterScale);
            // Visual Sex (even better ^^)
            StatDao.AddStat(50000, charID, 369, this.Gender);
            // Breed
            StatDao.AddStat(50000, charID, 4, this.Breed);
            // Visual Breed
            StatDao.AddStat(50000, charID, 367, this.Breed);
            
            // Profession / 60
            StatDao.AddStat(50000, charID, 60, this.Profession);

            // VisualProfession / 368
            StatDao.AddStat(50000, charID, 368, this.Profession);

            // Fatness / 47
            StatDao.AddStat(50000, charID, 47, this.Fatness);

            // Strength / 16
            StatDao.AddStat(50000, charID, 16, this.Abis[0]);

            // Psychic / 21
            StatDao.AddStat(50000, charID, 21, this.Abis[1]);

            // Sense / 20
            StatDao.AddStat(50000, charID, 20, this.Abis[2]);

            // Intelligence / 19
            StatDao.AddStat(50000, charID, 19, this.Abis[3]);

            // Stamina / 18
            StatDao.AddStat(50000, charID, 18, this.Abis[4]);

            // Agility / 17
            StatDao.AddStat(50000, charID, 17, this.Abis[5]);

            // Set HP and NP auf 1
            StatDao.AddStat(50000, charID, 1, 1);
            StatDao.AddStat(50000, charID, 214, 1);
            return charID;
        }

        #endregion
    }
}