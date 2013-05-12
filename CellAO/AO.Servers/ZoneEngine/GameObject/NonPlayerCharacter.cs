using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject
{
    public class NonPlayerCharacter : Dynel, INamedEntity, ISummoner, IAOEvents, IAOActions
    {
        public string Name
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        public IList<Pet> Pets
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
        }
    }
}