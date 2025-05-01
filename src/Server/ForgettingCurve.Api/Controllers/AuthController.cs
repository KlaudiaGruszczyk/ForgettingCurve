using Microsoft.AspNetCore.Mvc;
using MediatR;
using ForgettingCurve.Application.Auth.Commands;
using ForgettingCurve.Api.Contracts.Requests;
using ForgettingCurve.Api.Contracts.Responses;

namespace ForgettingCurve.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var command = new RegisterUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword
            };

            await _mediator.Send(command);

            return Ok(new RegisterResponse
            {
                Success = true,
                Message = "Rejestracja udana. Sprawdź email, aby zweryfikować konto."
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new RegisterResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "Wystąpił błąd podczas rejestracji. Spróbuj ponownie."
            });
        }
    }

    [HttpGet("verify-email")]
    [ProducesResponseType(typeof(VerifyEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(VerifyEmailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            var command = new VerifyEmailCommand
            {
                Email = email,
                Token = token
            };

            await _mediator.Send(command);

            return Ok(new VerifyEmailResponse
            {
                Success = true,
                Message = "Email został pomyślnie zweryfikowany."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new VerifyEmailResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception)
        {
            return BadRequest(new VerifyEmailResponse
            {
                Success = false,
                Message = "Wystąpił błąd podczas weryfikacji emaila. Spróbuj ponownie."
            });
        }
    }
} 