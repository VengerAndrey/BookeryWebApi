using System;

namespace WebApi.Exceptions
{
    public class ShareCRUDException : Exception
    {
        public ShareCRUDException(Guid shareId = new Guid(), string shareName = "", int ownerId = -1)
        {
            ShareId = shareId;
            ShareName = shareName;
            OwnerId = ownerId;
        }

        public Guid ShareId { get; set; }
        public string ShareName { get; set; }
        public int OwnerId { get; set; }
    }
}