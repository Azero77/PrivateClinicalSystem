using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClinicApp.Application.Commands.ModifySessionContent;
public record ModifySessionCommand(Guid sessionId, JsonElement content);

