using System;
using System.Linq;

namespace WebAppInventory
{
    /// <summary>
    /// Inventory error class
    /// </summary>
    public class InventoryError
    {
        private string m_message;
        private Severity m_severity;
        ErrorCode m_errorCode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="severity">Severity of error</param>
        /// <param name="errorCode">Error code</param>
        /// <param name="errorMessageParameters">Parameters to generate error message from the error code.</param>
        public InventoryError(Severity severity, ErrorCode errorCode, string[] errorMessageParameters)
        {
            ErrorCode = errorCode;
            Severity = severity;
            m_message = GetMessageFromErrorCode(errorCode, errorMessageParameters);
        }

        /// <summary>
        /// Get property for the Error code.
        /// </summary>
        public ErrorCode ErrorCode
        {
            get
            {
                return m_errorCode;
            }
            private set
            {
                m_errorCode = value;
            }
        }

        /// <summary>
        /// Get property for severity.
        /// </summary>
        public Severity Severity
        {
            get
            {
                return m_severity;
            }
            private set
            {
                m_severity = value;
            }
        }

        /// <summary>
        /// Gte property for the error message.
        /// </summary>
        public string Message
        {
            get
            {
                return m_message;
            }
        }

        private string GetMessageFromErrorCode(ErrorCode errorCode, string[] parameters)
        {
            switch(errorCode)
            {
                case ErrorCode.EmptyLabelError:
                    return Resource1.EmptyLabelError;
                case ErrorCode.ExpiredItemWarning:
                    if (parameters == null || parameters.Count() == 0)
                    {
                        return Resource1.ExpiredItemNoLabelInfoError;
                    }
                    return string.Format(Resource1.ExpiredItemWarning, parameters[0]);

                case ErrorCode.ItemWithLabelExistsError:
                    return string.Format(Resource1.ItemWithLabelExistsError, parameters[0]);
                case ErrorCode.LabelNotFoundError:
                    return string.Format(Resource1.LabelNotFoundError, parameters[0]); 
                case ErrorCode.Success:
                    return Resource1.Success;
                case ErrorCode.ItemRemovedInfo:
                    return string.Format(Resource1.ItemRemovedInfo, parameters[0]);
                default:
                    throw new NotImplementedException();
            }
        }

    }

    /// <summary>
    /// Error code enumeration
    /// </summary>
    public enum ErrorCode
    { 
        // errors
        EmptyLabelError, // The label is empty or null
        ItemWithLabelExistsError, // Cannot save item, because item with same label already exists
        LabelNotFoundError, // Item with label not found
                            
        // warnings
        ExpiredItemWarning, // The item is expired

        // information
        Success = 200,
        ItemRemovedInfo, //Item was removed from inventory
    }

    /// <summary>
    /// Enumeration for error severity
    /// </summary>
    public enum Severity
    {
        Error = 0,
        Warning,
        Info,
    }

}
