using Ecommerce.Application.DTOs.AuthDtos;
using ECommerceApi.Application.DTOs.Auth;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApi.Application.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterDto dto);
    Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<TokenResponseDto>> RefreshAsync(string refreshToken);
    Task<Result> RevokeAsync(string refreshToken);
}