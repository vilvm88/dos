﻿using System;

namespace DoOrSave.Core
{
    public interface IJobLogger
    {
        void Information(string message);

        void Warning(string message);

        void Error(Exception exception);
    }
}