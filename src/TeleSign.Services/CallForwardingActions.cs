//-----------------------------------------------------------------------
// <copyright file="CallForwardingActions.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Public</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services
{
    
    /// <summary>
    /// Specifies the action you want TeleSign to perform if this phone number is found to have Call Forwarding enabled. 
    /// </summary>
    public enum CallForwardActions
    {
        /// <summary>
        ///  TeleSign defaults to the value no action—and simply processes your 
        ///  verification request without performing a Call Forwarding detection check.
        /// </summary>
        None,
        /// <summary>
        /// TeleSign continues the verification process as usual, but also returns an indicator
        /// that identifies this phone number as having Call Forwarding enabled (to give you the 
        /// option of processing the phone number).
        /// </summary>
        Flag,
        /// <summary>
        /// If you set this parameter to block, then TeleSign aborts the verification process immediately, 
        /// and returns the Call Status Code 130 - Call Blocked by TeleSign.
        /// </summary>
        Block
    }
}
