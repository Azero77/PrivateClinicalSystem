using ClinicApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Session
{
    public class Session : Entity
    {
        public Session(Guid id) : base(id)
        {

        }
        public SessionDate SessionDate { get; private set; }
    }
}
