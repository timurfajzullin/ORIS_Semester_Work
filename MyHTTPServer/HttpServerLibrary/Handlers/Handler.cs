﻿using System.Net;


namespace HttpServerLibrary.Handlers;

public abstract class Handler
{
    public Handler Successor { get; set; }
    
    public abstract void HandleRequest(HttpRequestContext context);
}