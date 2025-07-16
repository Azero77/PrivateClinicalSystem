using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; private set; }
        protected Entity(Guid id)
        {
            Id = id;
        }
        protected Entity()
        {

        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;
            return ((Entity)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
