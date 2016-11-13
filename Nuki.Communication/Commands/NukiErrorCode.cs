using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Nuki.Communication.Commands
{
    public enum NukiErrorCode : byte
    {
        /// <summary>
        /// CRC of received command is invalid
        /// </summary>
        ERROR_BAD_CRC = 0xFD,
        /// <summary>
        /// Length of retrieved command payload does not match expected length
        /// </summary>
        ERROR_BAD_LENGTH = 0xFE,
        /// <summary>
        /// Used if no other error code matches
        /// </summary>
        ERROR_UNKNOWN = 0xFF,
        /// <summary>
        /// Returned if public key is being requested via request data command, but keyturner is not in pairing mode 
        /// </summary>
        P_ERROR_NOT_PAIRING = 0x10,
        /// <summary>
        /// Returned if the received authenticator does not match the own calculated authenticator 
        /// </summary>
        P_ERROR_BAD_AUTHENTICATOR = 0x11,
        /// <summary>
        /// Returned if a provided parameter is outside of its valid range 
        /// </summary>
        P_ERROR_BAD_PARAMETER = 0x12,
        /// <summary>
        /// Returned if the maximum number of users has been reached
        /// </summary>
        P_ERROR_MAX_USER = 0x13,
        /// <summary>
        /// Returned if the provided 
        /// authorization id is invalid or the 
        /// payload could not be decrypted 
        /// using the shared key for this 
        /// authorization id 
        /// </summary>
        K_ERROR_NOT_AUTHORIZED = 0x20,
        /// <summary>
        /// Returned if the provided pin 
        /// does not match the stored one 
        /// </summary>
        K_ERROR_BAD_PIN = 0x21,
        /// <summary>
        ///  Returned if the provided nonce 
        /// does not match the last stored 
        /// one of this authorization id or 
        /// has already been used before 
        /// </summary>
        K_ERROR_BAD_NONCE = 0x22,
        /// <summary>
        ///  Returned if a provided 
        /// parameter is outside of its valid 
        /// range 
        /// </summary>
        K_ERROR_BAD_PARAMETER = 0x23,
        /// <summary>
        /// Returned if the desired 
        /// authorization id could not be 
        /// deleted because it does not 
        /// exist 
        /// </summary>
        K_ERROR_INVALID_AUTH_ID = 0x24,
        /// <summary>
        /// Returned if the provided 
        /// authorization id is currently 
        /// disabled 
        /// </summary>
        K_ERROR_DISABLED = 0x25,
        /// <summary>
        /// Returned if the request has 
        /// been forwarded by the Nuki 
        /// Bridge and the provided 
        /// authorization id has not been 
        /// granted remote access 
        /// </summary>
        K_ERROR_REMOTE_NOT_ALLOWED = 0x26,
        /// <summary>
        /// Returned if the provided 
        /// authorization id has not been 
        /// granted access at the current 
        /// time 
        /// </summary>
        K_ERROR_TIME_NOT_ALLOWED = 0x27,
        /// <summary>
        /// Returned on an incoming auto 
        /// unlock request and if an auto 
        /// unlock has already been 
        /// executed within short time 
        /// </summary>
        K_ERROR_AUTO_UNLOCK_TOO_RECENT = 0x40,
        /// <summary>
        /// Returned on an incoming 
        /// unlock request if the request 
        /// has been forwarded by the 
        /// Nuki Bridge and the keyturner 
        /// is unsure about its actual lock 
        /// position 
        /// </summary>
        K_ERROR_POSITION_UNKNOWN = 0x41,
        /// <summary>
        /// Returned if the motor blocks 
        /// </summary>
        K_ERROR_MOTOR_BLOCKED = 0x42,
        /// <summary>
        /// Returned if there is a problem 
        /// with the clutch during motor 
        /// movement 
        /// </summary>
        K_ERROR_CLUTCH_FAILURE = 0x43,
        /// <summary>
        /// Returned if the motor moves 
        /// for a given period of time but 
        /// did not block 
        /// </summary>
        K_ERROR_MOTOR_TIMEOUT = 0x44,
        /// <summary>
        /// Returned on any lock action via 
        /// bluetooth if there is already a 
        /// lock action processing 
        /// </summary>
        K_ERROR_BUSY = 0x45,
        /// <summary>
        /// Returned to signal the successful completion of a
        /// command 
        /// </summary>
        COMPLETE = 0x00,
        /// <summary>
        /// Returned to signal that a command has been accepted
        /// the completion status will be signaled later 
        /// </summary>
        ACCEPTED = 0x01,
    }
}
