using BuberDinner.Application.Authentication.Admin;
using BuberDinner.Application.Authentication.Admin.Queries.Login;
using BuberDinner.Application.Authentication.User;
using BuberDinner.Application.Authentication.User.Commands.Register;
using BuberDinner.Application.Authentication.User.Queries.Login;
using BuberDinner.Contracts.Authentication;

using Mapster;

namespace BuberDinner.Api.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, RegisterCommand>();

        config.NewConfig<LoginRequest, UserLoginQuery>();

        config.NewConfig<LoginRequest, AdminLoginQuery>();

        config.NewConfig<AdminAuthenticationResult, AdminAuthenticationResponse>()
            .Map(dest => dest.Id, src => src.Admin.Id.Value.ToString())
            .Map(dest => dest, src => src.Admin); // ?????????????

        config.NewConfig<UserAuthenticationResult, UserAuthenticationResponse>()
            .Map(dest => dest.Id, src => src.User.Id.Value.ToString())
            .Map(dest => dest, src => src.User); // ?????????????
    }
}