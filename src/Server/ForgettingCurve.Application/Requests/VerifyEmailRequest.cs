﻿namespace ForgettingCurve.Application.Requests;

public class VerifyEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}