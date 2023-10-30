﻿using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IMesCommand
    {
        string CommandName { get; }
        Task<MesCommandResponse> Execute(MesCommandRequest input);
    }
}
