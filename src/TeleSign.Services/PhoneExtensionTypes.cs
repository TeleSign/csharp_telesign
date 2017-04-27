//-----------------------------------------------------------------------
// <copyright file="PhoneExtensionTypes.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Public</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services
{
    /// <summary>
    /// Defines the types of extensions that can be used with voice calls
    /// </summary>
    public enum PhoneExtensionTypes
    {
        /// <summary>
        /// Telesign default
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// Automated Attendants
        /// </summary>
        AutomatedAttendant = 1,
        /// <summary>
        /// Live Operators
        /// </summary>
        LiveOperator = 2
    }
}
