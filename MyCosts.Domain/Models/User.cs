﻿namespace MyCosts.Domain.Models;

public class User
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}