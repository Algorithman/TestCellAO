using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace AO.Core.Database
{
    public class CharacterDao
    {
        public IEnumerable<DBCharacter> GetAll()
        {
            using (var conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBCharacter>(
                        "SELECT Name, FirstName, LastName, Textures0,Textures1,Textures2,Textures3,Textures4,playfield as Playfield, X,Y,Z,HeadingX,HeadingY,HeadingZ,HeadingW FROM characters");
            }
        }

        public IEnumerable<DBCharacter> GetById(int characterId)
        {
            using (var conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBCharacter>(
                        "SELECT Name, FirstName, LastName, Textures0,Textures1,Textures2,Textures3,Textures4,playfield as Playfield, X,Y,Z,HeadingX,HeadingY,HeadingZ,HeadingW FROM characters where id = @id",
                        new { id = characterId });
            }
        }
    }
}
