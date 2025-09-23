using ClinicApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Shared.IntegrationEvents;
public record UserCreatedIntegrationEvent(UserRole role,string metadata);
