﻿namespace BeyondShopping.Core.Exceptions;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}
