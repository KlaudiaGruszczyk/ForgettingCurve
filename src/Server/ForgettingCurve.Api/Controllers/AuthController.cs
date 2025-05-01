using Microsoft.AspNetCore.Mvc;
using MediatR;
using ForgettingCurve.Application.Commands.Login;
using ForgettingCurve.Application.Commands.Register;
using ForgettingCurve.Application.Commands.VerifyEmail;
using ForgettingCurve.Application.Responses;
using ForgettingCurve.Domain.Identity.Exceptions;
using ForgettingCurve.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading.Tasks;
using ForgettingCurve.Application.Requests;

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
            var command = new RegisterCommand
            {
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword
            };

            await _mediator.Send(command);

            return Ok(new RegisterResponse
            {
                Success = true,
                Message = "Registration successful. Please check your email for verification."
            });
        }
        catch (UserAlreadyExistsException ex)
        {
            return Conflict(new RegisterResponse
            {
                Success = false,
                Message = "User already exists with this email."
            });
        }
        catch (UserRegistrationFailedException ex)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "User registration failed. Please check your input."
            });
        }
        catch (Exception)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "An error occurred during registration. Please try again."
            });
        }
    }

    [HttpPost("verify-email")]
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
                IsVerified = true
            });
        }
        catch (UserNotFoundException ex)
        {
            return BadRequest(new VerifyEmailResponse
            {
                IsVerified = false
            });
        }
        catch (InvalidVerificationTokenException ex)
        {
            return BadRequest(new VerifyEmailResponse
            {
                IsVerified = false
            });
        }
        catch (Exception)
        {
            return BadRequest(new VerifyEmailResponse
            {
                IsVerified = false
            });
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token with expiration date and user id</returns>
    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var command = new LoginCommand
            {
                Email = request.Email ?? string.Empty,
                Password = request.Password ?? string.Empty
            };
            
            var result = await _mediator.Send(command);
            
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Type = "validation_error",
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest
            };
            
            foreach (var error in ex.Errors)
            {
                if (!problemDetails.Errors.ContainsKey(error.PropertyName))
                {
                    problemDetails.Errors[error.PropertyName] = new[] { error.ErrorMessage };
                }
            }
            
            return BadRequest(problemDetails);
        }
        catch (AuthenticationException)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "authentication_error",
                Title = "Invalid email or password",
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (AccountNotVerifiedException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Type = "verification_error",
                Title = "Account not verified",
                Status = StatusCodes.Status403Forbidden,
                Detail = "Please verify your email address to continue"
            });
        }
        catch (AccountLockedException ex)
        {
            var details = new ProblemDetails
            {
                Type = "lockout_error",
                Title = "Account locked",
                Status = StatusCodes.Status403Forbidden,
                Detail = ex.Message
            };
            
            if (ex.LockoutEnd.HasValue)
            {
                details.Extensions["lockoutEnd"] = ex.LockoutEnd.Value;
            }
            
            return StatusCode(StatusCodes.Status403Forbidden, details);
        }
    }
} 