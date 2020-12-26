using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OSMLS.Map;
using static OSMLS.Map.MapService;

namespace OSMLS.Services
{
	public class MapService : MapServiceBase
	{
		public override Task Updates(
			Empty request, IServerStreamWriter<Package> responseStream, ServerCallContext context)
		{
			throw new NotImplementedException();
		}
	}
}