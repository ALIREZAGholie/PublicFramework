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
        [Column(Order = 2)]
        public bool IsActive { get; private set; } = true;
        public Guid PublicId { get; private set; } = Guid.NewGuid();
        public long CreateUserId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public string? Description { get; private set; } = null;
        public string? MetaData { get; private set; }

        public void SetId(long id)
        {
            Id = id;
        }

        public void SetPublicId(Guid publicId)
        {
            PublicId = publicId;
        }

        public void SetCreate(long createUserId)
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

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }


    }
}