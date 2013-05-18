using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.Network.InternalBus
{
    using ZoneEngine.Network.InternalBus.InternalMessages;

    public interface IInternalMessagePublisher
    {
        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="message">
        /// </param>
        void Publish(object sender, InternalMessage internalMessage);

        #endregion
    }
}
