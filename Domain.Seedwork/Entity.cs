using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;

namespace Domain.Seedwork
{
    /// <summary>
    ///   Base class for entities
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class Entity
    {
        #region Fields

        private int? _requestedHashCode;

        #endregion

        #region Properties

        /// <summary>
        ///   Get the persisten object identifier
        /// </summary>
        [DataMember]
        [Key]
        public virtual Guid Id { get; protected set; }

        [DataMember]
        public virtual Guid SequentialId { get; protected set; }

        [DataMember]
        public virtual string CreatedBy { get; set; }

        [DataMember]
        public virtual DateTime CreatedDate { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Check if this entity is transient, ie, without identity at this moment
        /// </summary>
        /// <returns> True if entity is transient, else false </returns>
        public bool IsTransient()
        {
            return Id == Guid.Empty;
        }

        /// <summary>
        ///   Generate identity for this entity
        /// </summary>
        public void GenerateNewIdentity()
        {
            if (IsTransient())
            {
                Id = IdentityGenerator.NewSequentialGuid();

                SequentialId = IdentityGenerator.NewSequentialGuid();
            }
        }

        /// <summary>
        /// Change current identity for a new non transient identity
        /// </summary>
        /// <param name="identity"> the new identity </param>
        /// <param name="sequentialId"> the new sequential identity </param>
        public void ChangeCurrentIdentity(Guid identity, Guid sequentialId, string createdBy, DateTime createdDate)
        {
            if (identity != Guid.Empty)
            {
                Id = identity;
                SequentialId = sequentialId;
                CreatedBy = createdBy;
                CreatedDate = createdDate;
            }
        }

        /// <summary>
        /// Clone Entity
        /// </summary>
        public T Clone<T>()
        {
            T copia;
            var serializer = new DataContractSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this);
                ms.Position = 0;
                copia = (T)serializer.ReadObject(ms);
            }

            return copia;
        }

        #endregion

        #region Overrides Methods

        /// <summary>
        ///   <see cref="M:System.Object.Equals" />
        /// </summary>
        /// <param name="obj"> <see cref="M:System.Object.Equals" /> </param>
        /// <returns> <see cref="M:System.Object.Equals" /> </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (Entity)obj;

            if (item.IsTransient() || IsTransient())
                return false;
            else
                return item.Id == Id;
        }

        /// <summary>
        ///   <see cref="M:System.Object.GetHashCode" />
        /// </summary>
        /// <returns> <see cref="M:System.Object.GetHashCode" /> </returns>
        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = Id.GetHashCode() ^ 31;
                // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
                return (Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        #endregion
    }
}
