using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webgostar.Framework.Base.BaseModels
{
    public abstract class BaseEntity
    {
        [Key, Column(Order = 0)]
        public long Id { get; private set; }
        [Column(Order = 1)]
        public bool IsDelete { get; private set; }
        public Guid PublicId { get; private set; } = Guid.NewGuid();
        public string CreateUserId { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public void SetId(long id)
        {
            Id = id;
        }

        public void SetPublicId(Guid publicId)
        {
            PublicId = publicId;
        }

        public void SetCreate(string createUserId)
        {
            CreateUserId = createUserId;
            CreatedDate = DateTime.Now;
        }

        public void SetDelete()
        {
            IsDelete = true;
        }

        public void SetRecoveryDelete()
        {
            IsDelete = false;
        }
    }
}