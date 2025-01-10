using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;

public sealed record GetUserDevicesDto
{
    public List<DeviceDto> Devices { get; init; }

    private GetUserDevicesDto() { } // Hide the constructor to force the use of the Map method

    public static GetUserDevicesDto Map(List<Domain.Entities.Device> devices)
    {
        var DeviceList = new List<DeviceDto>();
        foreach (Domain.Entities.Device device in devices)
        {
            DeviceList.Add(new DeviceDto(){DeviceId = device.Id, Name = device.Name});
        }
        return new GetUserDevicesDto()
        {
            Devices = DeviceList
        };
    }

}

public sealed record DeviceDto
{
    public string Name { get; init; } = null!;
    public Guid DeviceId { get; init; }
}